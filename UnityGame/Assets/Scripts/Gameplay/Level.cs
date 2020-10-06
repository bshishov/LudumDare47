using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Utils.Debugger;

namespace Gameplay
{
    public class Level : Singleton<Level>
    {
        enum GameState
        {
            WaitingForPlayerCommand,
            ExecutingTurnCommands,
            CatGirlDied,
            PlayerDied,
            Win
        }

        public int MaxTurns = 10;
        public Entity CatGirl;

        public int CurrentTurnNumber => GetCurrentTurn()?.Number ?? -1;
        public bool IsPaused => Time.timeScale < 0.5f;

        private bool CanRollbackFromCurrentState => 
            _state == GameState.WaitingForPlayerCommand || 
            _state == GameState.PlayerDied ||
            _state == GameState.CatGirlDied;

        private GameState _state = GameState.WaitingForPlayerCommand;
        private readonly Dictionary<int, Entity> _entities = new Dictionary<int, Entity>();
        private Entity _playerEntity;
        private readonly Stack<Turn> _history = new Stack<Turn>();
        private float _timeSinceRollbackPressed;
        private const float RollbackCd = 0.08f;
        private int _lastEntityId;

        // UI
        private ShowTurns _uiTurns;
        private UIWinLose _uiWinLose;

        private void Awake()
        {
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        }

        void Start()
        {
            // Start with turn 0
            _history.Clear();
            _history.Push(new Turn(0));
            
            // All entities MUST BE INITIALIZED AT SOME TURN
            // Turn0 by default
            var foundEntities = GameObject.FindObjectsOfType<Entity>();
            _playerEntity = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
            foreach (var entity in foundEntities)
            {
                var newId = GetNewEntityId();
                entity.Initialize(this, newId);
                _entities.Add(newId, entity);
            }

            _uiTurns = GameObject.FindObjectOfType<ShowTurns>(true);
            if (_uiTurns != null)
                _uiTurns.Initialize(MaxTurns);
            
            _uiWinLose = GameObject.FindObjectOfType<UIWinLose>(true);
            _state = GameState.WaitingForPlayerCommand;
        }

        int GetNewEntityId()
        {
            _lastEntityId++;
            return _lastEntityId;
        }

        void Update()
        {
            if(IsPaused)
                return;

            Debugger.Default.Display("Level/Turn", GetCurrentTurn().Number);
            
            if (_state == GameState.ExecutingTurnCommands)
            {
                while (true)
                {
                    var command = GetCurrentTurn().PopCommand();
                    if (command != null)
                    {
                        Exec(command);
                    }
                    else
                    {
                        break;
                    }
                }
                
                HandleTurnEnd();
            }
            else if (_state == GameState.WaitingForPlayerCommand)
                HandleInput();
            
            if (CanRollbackFromCurrentState)
            {
                if (_timeSinceRollbackPressed >= RollbackCd && Input.GetKey(KeyCode.R))
                {
                    if (_uiWinLose != null)
                    {
                        _uiWinLose.HideLoseWindow();
                        _uiWinLose.HideWinWindow();
                    }

                    RollbackLastCompletedTurn();
                    _timeSinceRollbackPressed = 0.0f;
                }
            }

            _timeSinceRollbackPressed += Time.deltaTime;
        }

        public Turn GetCurrentTurn()
        {
            // There should ALWAYS be a turn
            return _history.Peek();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                PlayerMove(Direction.Right);

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                PlayerMove(Direction.Left);

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                PlayerMove(Direction.Front);

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                PlayerMove(Direction.Back);
        }

        private void PlayerMove(Direction dir)
        {
            // Player moves first
            var playerId = _playerEntity.Id;
            Dispatch(new MoveCommand(playerId, dir, true));

            // Rest of the objects move afterwards
            foreach (var entity in _entities.Values)
            {
                if(entity.IsActive)
                    entity.OnAfterPlayerMove(this);
            }
            
            // Proceed to executing turns
            SwitchState(GameState.ExecutingTurnCommands);
        }

        private void HandleTurnEnd()
        {
            // All turn command are finished
            var turn = GetCurrentTurn();
            turn.Complete();
            Debug.Log($"Turn {turn.Number} completed");

            if (!_playerEntity.IsActive)
            {
                SwitchState(GameState.PlayerDied);
                if(_uiWinLose != null)
                    _uiWinLose.ShowLoseWindow(FailReason.PlayerDied);
                
            }
            else if (CatGirl != null && !CatGirl.IsActive)
            {
                SwitchState(GameState.CatGirlDied);
                if (_uiWinLose != null)
                    _uiWinLose.ShowLoseWindow(FailReason.CatDied);
            }
            else if(turn.Number >= MaxTurns)
            {
                // If it was the last turn and everyobe is alive
                SwitchState(GameState.Win);
                if(_uiWinLose != null)
                    _uiWinLose.ShowWinWindow();
            }
            else
            {
                SwitchState(GameState.WaitingForPlayerCommand);
            }
            
            // Starting new turn
            var newTurnNumber = turn.Number + 1;
            Debug.Log($"Starting turn {newTurnNumber}");
            _history.Push(new Turn(newTurnNumber));
            if(_uiTurns != null)
                _uiTurns.NextTurn();
        }
        
        private void RollbackLastCompletedTurn()
        {
            if (_history.Count == 0)
            {
                Debug.LogWarning("Trying to rollback an empty history");
                return;
            }
            
            // Current turn might be the incompleted one (waiting for player input)
            // Incopleted turns might be on top of the stack
            if (!_history.Peek().IsCompleted)
            {
                var incompleteTurn = _history.Peek();
                RevertTurnChanges(incompleteTurn);
                _history.Pop(); // Late pop for changes to revert durn correct turn number
            }
    
            // Now, revert a completed turn (that was the players intent)
            if (_history.Count > 0)
            {
                var rollbackTurn = _history.Peek();
                RevertTurnChanges(rollbackTurn);
                _history.Pop(); // Late pop for changes to revert durn correct turn number

                if(_uiTurns != null)
                    _uiTurns.BackTurn();
            }
            
            // Start new "Incomplete turn"
            if (_history.Count == 0)
                _history.Push(new Turn(0));
            else
                _history.Push(new Turn(_history.Peek().Number + 1));

            // Rollback now finished, notify all active atm entities about that
            // NOTE: "current turn" will be the new, incoplmete one  
            foreach (var entity in _entities.Values.Where(e => e.IsActive))
                entity.AfterTurnRollback(this);
            
            // Proceed to reading inputs
            SwitchState(GameState.WaitingForPlayerCommand);
        }

        private void RevertTurnChanges(Turn turn)
        {
            foreach (var change in turn.IterateChangesFromNewestToOldest())
                Revert(change);
        }

        private void Revert(IChange change)
        {
            if (_entities.TryGetValue(change.TargetId, out var target))
                target.Revert(this, change);
            else
                Debug.LogWarning($"Trying to revert change {change} but entityId is missing: {change.TargetId}");
        }
        
        private void Exec(ICommand command)
        {
            if (_entities.TryGetValue(command.TargetId, out var target))
                foreach (var change in target.Execute(this, command))
                    GetCurrentTurn().LogChange(change);
            else
                Debug.LogWarning($"Trying to execute command {command} but entityId is invalid: {command.TargetId}");
        }

        private void SwitchState(GameState state)
        {
            Debug.Log($"Level: {state}");
            _state = state;
        }

        public void Dispatch(ICommand command)
        {
            GetCurrentTurn().PushCommand(command);   
        }

        public void DispatchEarly(ICommand command)
        {
            GetCurrentTurn().PushCommandEarly(command);
        }

        public Entity GetActiveEntityAt(Vector2Int position)
        {
            return _entities.Values.FirstOrDefault(entity => entity.IsActive && entity.Position == position);
        }

        public IEnumerable<Entity> GetActiveEntitiesAt(Vector2Int position)
        {
            return _entities.Values.Where(entity => entity.IsActive && entity.Position == position);
        }

        public Entity Spawn(GameObject prefab, Vector2Int entityPosition, Direction entityOrientation)
        {
            if (prefab == null)
            {
                Debug.LogWarning($"Trying to spawn null prefab at {entityPosition}");
                return null;
            }

            var prefabEntity = prefab.GetComponent<Entity>();
            if (prefabEntity == null)
            {
                Debug.LogWarning($"Prefab {prefab} missing Entity Component");
                return null;
            }

            var objectType = prefabEntity.ObjectType;
            foreach (var obstacle in GetActiveEntitiesAt(entityPosition))
            {
                if (CollisionConfig.ObjectsCollide(objectType, obstacle.ObjectType))
                {
                    Debug.Log($"Trying to spawn object {prefab} that collides with {obstacle}");
                    return null;
                }
            }
            
            var spawnedObject = Instantiate(prefab, 
                Utils.LevelToWorld(entityPosition), 
                Utils.DirectionToRotation(entityOrientation));
            var entity = spawnedObject.GetComponent<Entity>();
            if (entity != null)
            {
                var newEntityId = GetNewEntityId();
                entity.Initialize(this, newEntityId);
                _entities.Add(newEntityId, entity);
                return entity;
            }
            
            Debug.LogWarning($"Failed to spawn prefab {prefab}");
            return null;
        }

        private Entity GetEntityById(int entityId)
        {
            return _entities.ContainsKey(entityId) ? _entities[entityId] : null;
        }

        public void Despawn(int entityId)
        {
            var entity = _entities[entityId];
            Destroy(entity.gameObject);
            _entities.Remove(entityId);
        }

        public IEnumerable<Entity> GetActiveEntitiesInRadius(Vector2Int position, int radius)
        {
            return _entities.Values
                .Where(entity => entity.IsActive && 
                                 Utils.IsInsideRadius(position, entity.Position, radius));
        }
    }
}
