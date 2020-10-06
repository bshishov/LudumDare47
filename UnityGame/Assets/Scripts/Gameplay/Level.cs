using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Gameplay
{
    public class Level : Singleton<Level>
    {
        enum GameState
        {
            WaitingForPlayerCommand,
            ExecutingTurnCommands,
            CatGirlDied,
            PlayerDied
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
        private UILoad _uiLoad;

        private void Awake()
        {
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        }

        void Start()
        {
            var foundEntities = GameObject.FindObjectsOfType<Entity>();
            _playerEntity = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
            foreach (var entity in foundEntities)
            {
                var newId = GetNewEntityId();
                entity.Initialize(this, newId);
                _entities.Add(newId, entity);
                
            }
            _history.Clear();
            
            _uiTurns = GameObject.FindObjectOfType<ShowTurns>(true);
            if (_uiTurns != null)
                _uiTurns.Initialize(MaxTurns);
            
            _uiWinLose = GameObject.FindObjectOfType<UIWinLose>(true);
            _uiLoad = GameObject.FindObjectOfType<UILoad>(true);
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
            
            if (CanRollbackFromCurrentState && _history.Count > 0)
            {
                if (_timeSinceRollbackPressed >= RollbackCd && Input.GetKey(KeyCode.R))
                {
                    if (_uiWinLose != null)
                    {
                        _uiWinLose.HideLoseWindow();
                        _uiWinLose.HideWinWindow();
                    }

                    RollbackTurn();
                    _timeSinceRollbackPressed = 0.0f;
                }
            }

            _timeSinceRollbackPressed += Time.deltaTime;
        }

        public Turn GetCurrentTurn()
        {
            if (_history.Count > 0)
                return _history.Peek();
            return null;
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                NewTurn(Direction.Right);

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                NewTurn(Direction.Left);

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                NewTurn(Direction.Front);

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                NewTurn(Direction.Back);
        }

        private void NewTurn(Direction dir)
        {
            var currentTurn = GetCurrentTurn();
            var turn = new Turn(currentTurn?.Number + 1 ?? 0);
            _history.Push(turn);

            // Player moves first
            var playerId = _playerEntity.Id;
            Dispatch(new MoveCommand(playerId, dir, true));

            // Rest of the objects move afterwards
            foreach (var entity in _entities.Values)
            {
                if(entity.IsActive)
                    entity.OnTurnStarted(this);
            }
            
            // Proceed to executing turns
            SwitchState(GameState.ExecutingTurnCommands);
        }

        private void HandleTurnEnd()
        {
            var turn = GetCurrentTurn();
            turn.Complete();
            if(_uiTurns != null)
              _uiTurns.NextTurn();
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
                {
                    Debug.Log("Showing CAT DIED");
                    _uiWinLose.ShowLoseWindow(FailReason.CatDied);
                }
            }
            else
            {
                if(turn.Number >= MaxTurns && _uiWinLose != null)
                    _uiWinLose.ShowWinWindow();
                SwitchState(GameState.WaitingForPlayerCommand);
            }
        }
        
        private void RollbackTurn()
        {
            if (_history.Count == 0)
            {
                Debug.LogWarning("Trying to rollback an empty history");
                return;
            }

            var rollbackTurn = _history.Pop();
            
            foreach (var change in rollbackTurn.IterateChangesFromNewestToOldest())
                Revert(change);

            foreach (var entity in _entities.Values)
            {
                if (entity.IsActive)
                    entity.OnTurnRolledBack(this);
            }

            if(_uiTurns != null)
                _uiTurns.BackTurn();

            _state = GameState.WaitingForPlayerCommand;
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
