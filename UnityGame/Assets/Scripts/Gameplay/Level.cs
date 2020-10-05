using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Utils;

namespace Gameplay
{
    public class Level : Singleton<Level>
    {
        public int MaxTurns = 10;

        public int CurrentTurn => GetCurrentTurn().Number;
        public bool IsPaused => Time.timeScale < 0.5f;

        private readonly Dictionary<int, Entity> _entities = new Dictionary<int, Entity>();
        private Entity _playerEntity;
        private readonly LinkedList<ICommand> _turnQueue = new LinkedList<ICommand>();
        private readonly Stack<Turn> _history = new Stack<Turn>();
        private float _currentRollbackCd;
        private const float RollbackCd = 0.08f;
        private int _lastEntityId;
        private ShowTurns _turnsUi;

        private void Awake()
        {
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        }

        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            var foundEntities = GameObject.FindObjectsOfType<Entity>();
            _playerEntity = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
            foreach (var entity in foundEntities)
            {
                var newId = GetEntityId();
                entity.Initialize(newId);
                _entities.Add(newId, entity);
                
            }
            _turnQueue.Clear();
            _history.Clear();
            
            _turnsUi = GameObject.FindObjectOfType<ShowTurns>(true);
            if (_turnsUi != null)
                _turnsUi.Initialize(MaxTurns);
        }

        int GetEntityId()
        {
            return _lastEntityId++;
        }

        void Update()
        {
            // If paused - do nothing
            if(IsPaused)
                return;
            
            if (_turnQueue.Count > 0)
            {
                var first = _turnQueue.First.Value;
                _turnQueue.RemoveFirst();
                Exec(first);
            }
            else
            {
                HandleInput();
            }
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
            {
                NewTurn(Direction.Right);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                NewTurn(Direction.Left);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                NewTurn(Direction.Front);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                NewTurn(Direction.Back);
            }

            if (_currentRollbackCd >= RollbackCd && Input.GetKey(KeyCode.R))
            {
                RollbackTurn();
                _currentRollbackCd = 0.0f;
            }

            _currentRollbackCd += Time.deltaTime;
        }

        private void NewTurn(Direction dir)
        {
            var currentTurn = GetCurrentTurn();
            var currentTurnNumber = -1;
            if (currentTurn != null)
                currentTurnNumber = currentTurn.Number;
            _history.Push(new Turn(currentTurnNumber + 1));
            
            if(_turnsUi != null)
                _turnsUi.NextTurn();
            
            // Player moves first
            var playerId = _playerEntity.Id;
            Dispatch(new MoveCommand(playerId, dir, true));

            // Rest of the objects move afterwards
            foreach (var entity in _entities.Values)
            {
                if(entity.IsActive)
                    entity.OnTurnStarted(this);
            }
        }

        private void RollbackTurn()
        {
            if (_history.Count > 0)
            {
                if(_turnsUi != null)
                    _turnsUi.BackTurn();
                
                var turn = _history.Pop();
                foreach (var change in turn.IterateChangesFromNewestToOldest())
                {
                    Revert(change);
                }
            }
        }

        private void Revert(IChange change)
        {
            if (change.TargetId >= 0)
            {
                var target = _entities[change.TargetId];
                target.Revert(this, change);
            }
        }
        
        private void Exec(ICommand command)
        {
            Debug.Log($"Executing command {command} for {command.TargetId}");

            if (command.TargetId >= 0)
            {
                var target = _entities[command.TargetId];
                var currentTurn = GetCurrentTurn();
                foreach (var change in target.Execute(this, command))
                {
                    Debug.Log($"Change: {change.TargetId}: {change}");
                    currentTurn.Changelog.Push(change);        
                }
            }
        }

        public void Dispatch(ICommand command)
        {
            Debug.Log($"Queued command {command} for {command.TargetId}");
            _turnQueue.AddLast(command);   
        }

        public void DispatchEarly(ICommand command)
        {
            Debug.Log($"Queued command {command} for {command.TargetId}");
            _turnQueue.AddFirst(command);
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
            var spawnedObject = Instantiate(prefab, 
                Utils.LevelToWorld(entityPosition), 
                Utils.DirectionToRotation(entityOrientation));
            var entity = spawnedObject.GetComponent<Entity>();
            if (entity != null)
            {
                var newEntityId = GetEntityId();
                entity.Initialize(newEntityId);
                _entities.Add(newEntityId, entity);
                return entity;
            }

            return null;
        }

        private Entity GetEntityById(int entityId)
        {
            return _entities.ContainsKey(entityId) ? _entities[entityId] : null;
        }

        public void DestroyEntity(int entityId)
        {
            var entity = GetEntityById(entityId);
            if (entity != null)
            {
                Destroy(entity.gameObject);
                _entities.Remove(entityId);
            }
        }

        public IEnumerable<Entity> GetActiveEntitiesInRadius(Vector2Int position, int radius)
        {
            return _entities.Values
                .Where(entity => entity.IsActive && 
                                 Utils.IsInsideRadius(position, entity.Position, radius));
        }
    }
}
