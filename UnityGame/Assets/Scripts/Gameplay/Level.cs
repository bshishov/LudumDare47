﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Properties;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Gameplay
{
    public class Level : MonoBehaviour
    {
        public enum GameState
        {
            WaitingForPlayerCommand,
            ExecutingTurnCommands,
            SkipTurn,
            CatGirlDied,
            PlayerDied,
            Win
        }

        public int MaxTurns = 10;
        public Entity CatGirl;
        public int CurrentTurnNumber => GetCurrentTurn()?.Number ?? -1;
        public bool IsPaused => Time.timeScale < 0.5f;
        public int CollectedStars { get; private set; }
        public event Action TurnCompleted;
        public event Action TurnRollbackSucceeds; 
        public event Action TurnRollbackDenied;
        public event Action<GameState> StateChanged;
        public event Action StarCollected;
        public event Action<Entity> EntitySpawned;
        public event Action<Entity> EntityKilled;

        private bool CanRollbackFromCurrentState =>
            _state == GameState.WaitingForPlayerCommand ||
            _state == GameState.PlayerDied ||
            _state == GameState.CatGirlDied;

        private GameState _state = GameState.WaitingForPlayerCommand;
        private readonly Dictionary<int, Entity> _entities = new Dictionary<int, Entity>();
        private Entity _playerEntity;
        private Movable _playerMovable;
        private readonly Stack<Turn> _history = new Stack<Turn>();
        private float _timeSinceRollbackPressed;
        private const float RollbackCd = 0.08f;
        private int _lastEntityId;

        //Save Data
        private string _sceneName;

        private void Awake()
        {
            // Level requires UI
            SceneLoading.TryLoadSceneAdditive("UI");
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
            _playerMovable = _playerEntity.GetComponent<Movable>();

            foreach (var entity in foundEntities)
            {
                var newId = GetNewEntityId();
                entity.Initialize(this, newId);
                _entities.Add(newId, entity);
            }

            SwitchState(GameState.WaitingForPlayerCommand);
            _sceneName = SceneManager.GetActiveScene().name;
            Common.SetActiveLevel(this);

            //fixed rollback for test
            PlayerStats.Instance.SetFixedRollBack();
        }

        int GetNewEntityId()
        {
            _lastEntityId++;
            return _lastEntityId;
        }

        void Update()
        {
            if (IsPaused)
                return;

            if (_state == GameState.ExecutingTurnCommands)
            {
                ExecuteCommands();
            }

            _timeSinceRollbackPressed += Time.deltaTime;
        }

        private void ExecuteCommands()
        {
            while (true)
            {
                var command = GetCurrentTurn().PopCommand();
                if (command != null)
                {
                    Exec(command);
                } else
                {
                    break;
                }
            }

            HandleTurnEnd();
        }

        public Turn GetCurrentTurn()
        {
            if (_history.Count == 0)
                return null;
            
            return _history.Peek();
        }

        public void PlayerRollback()
        {
            if (CanRollbackFromCurrentState)
            {
                if (_timeSinceRollbackPressed >= RollbackCd)
                {
                    TryRollbackLastCompletedTurn();
                    _timeSinceRollbackPressed = 0.0f;
                }
            }
            //Turns skipping stop after player pressed RollBack button
            else if (_state == GameState.SkipTurn)
            {
                SwitchState(GameState.WaitingForPlayerCommand);
               
            }
            StopCoroutine("SkipTurn");
        }

        public void PlayerMove(Direction dir)
        {
            if (_state == GameState.WaitingForPlayerCommand)
            {
                if (!_playerMovable.CanMove(this, dir))
                {
                    // If player cant move, discard the turn attempt
                    return;
                }

                // Player moves first
                var playerId = _playerEntity.Id;
                Dispatch(new MoveCommand(playerId, dir, true));

                // Rest of the objects move afterwards
                foreach (var entity in _entities.Values)
                {
                    if (entity.IsActive)
                        entity.OnAfterPlayerMove(this);
                }

                // Proceed to executing turns
                SwitchState(GameState.ExecutingTurnCommands);
            }
        }

        private void HandleTurnEnd()
        {
            // All turn command are finished
            var turn = GetCurrentTurn();
            turn.Complete();

            TurnCompleted?.Invoke();

            if (!_playerEntity.IsActive)
            {
                SwitchState(GameState.PlayerDied);
                StopCoroutine("SkipTurn");
            } 
            else if (CatGirl != null && !CatGirl.IsActive)
            {
                SwitchState(GameState.CatGirlDied);
                StopCoroutine("SkipTurn");
            } 
            else if (turn.Number >= MaxTurns - 1)
            {
                // If it was the last turn and everyone is alive
                SwitchState(GameState.Win);
                //One star given for complete level
                CollectStar();

                //TODO test give some rollback
                PlayerStats.Instance.AddRollbackNumber(25);
                AddCollectedStars();
                
                SaveLevelState();
                StopCoroutine("SkipTurn");
            } 
            else if (_state != GameState.SkipTurn)
            {
                SwitchState(GameState.WaitingForPlayerCommand);
            } 
            else
            {
                SwitchState(GameState.SkipTurn);
            }

            // Starting new turn
            var newTurnNumber = turn.Number + 1;
            _history.Push(new Turn(newTurnNumber));
        }

        private void SaveLevelState()
        {
            if (GamePersist.Instance != null)
            {
                GamePersist.Instance.SaveLevelData(_sceneName, CollectedStars);
            }
        }

        private void TryRollbackLastCompletedTurn()
        {
            if (_history.Count == 0)
            {
                Debug.LogWarning("Trying to rollback an empty history");
                TurnRollbackDenied?.Invoke();
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
            if ((_history.Count > 0) && (PlayerStats.Instance.NumberOfRollback > 0))
            {
                var rollbackTurn = _history.Peek();
                RevertTurnChanges(rollbackTurn);
                _history.Pop(); // Late pop for changes to revert durn correct turn number
                PlayerStats.Instance.RemoveRollbackNumber();
                
                // Rollback now finished, notify all active atm entities about that
                // NOTE: "current turn" will be the new, incoplmete one  
                foreach (var entity in _entities.Values.Where(e => e.IsActive))
                    entity.AfterTurnRollback(this);

                TurnRollbackSucceeds?.Invoke();
            }
            else
            {
                TurnRollbackDenied?.Invoke();
            }

            // Start new "Incomplete turn"
            if (_history.Count == 0)
                _history.Push(new Turn(0));
            else
                _history.Push(new Turn(_history.Peek().Number + 1));


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
            _state = state;
            StateChanged?.Invoke(state);
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

        public IEnumerable<Entity> GetAllActiveEntities()
        {
            return _entities.Values.Where(entity => entity.IsActive);
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

            var spawnedObject = Instantiate(prefab,
                Utils.LevelToWorld(entityPosition),
                Utils.DirectionToRotation(entityOrientation));
            var entity = spawnedObject.GetComponent<Entity>();
            if (entity != null)
            {
                var newEntityId = GetNewEntityId();
                entity.Initialize(this, newEntityId);
                _entities.Add(newEntityId, entity);
                EntitySpawned?.Invoke(entity);

                return entity;
            }

            Debug.LogWarning($"Failed to spawn prefab {prefab}");
            return null;
        }

        public void Kill(int entityId)
        {
            var entity = _entities[entityId];
            EntityKilled?.Invoke(entity);
            Destroy(entity.gameObject);
            _entities.Remove(entityId);
        }

        public IEnumerable<Entity> GetActiveEntitiesInRadius(Vector2Int position, int radius)
        {
            return _entities.Values
                .Where(entity => entity.IsActive &&
                                 Utils.IsInsideRadius(position, entity.Position, radius));
        }

        public void SkipLevel()
        {
            if (_state != GameState.SkipTurn)
            {
                StartCoroutine("SkipTurn");
                _state = GameState.SkipTurn;
            }
        }

        private IEnumerator SkipTurn()
        {
            while (GetCurrentTurn().Number <= MaxTurns - 1)
            {
                if (_state == GameState.SkipTurn)
                {
                    foreach (var entity in _entities.Values)
                    {
                        if (entity.IsActive)
                            entity.OnAfterPlayerMove(this);
                    }
                    ExecuteCommands();
                }

                yield return new WaitForSeconds(.2f);
            }
        }

        public void CollectStar()
        {
            CollectedStars++;
            StarCollected?.Invoke();
        }
        
        public void LoseStar()
        {
            CollectedStars--;
        }
        
        private void AddCollectedStars()
        {
            //player can't collected stars that's already were collected
            if (GamePersist.Instance.LevelData.ContainsKey(_sceneName))
            {
                var newCollectedStars = CollectedStars - GamePersist.Instance.LevelData[_sceneName];
                PlayerStats.Instance.AddStars(newCollectedStars);
            } else
            {
                PlayerStats.Instance.AddStars(CollectedStars);
            }
        }
    }
}
