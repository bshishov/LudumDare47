using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Gameplay
{
    public class Level : Singleton<Level>
    {
        public int MaxTurns = 10;

        private readonly Dictionary<int, Entity> _entities = new Dictionary<int, Entity>();
        private Entity _playerEntity;
        private readonly LinkedList<ICommand> _turnQueue = new LinkedList<ICommand>();
        private readonly Stack<Turn> _history = new Stack<Turn>();
        private float _currentRollbackCd;
        private const float RollbackCd = 0.08f;
        private int _lastEntityId = 0; 

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
        }

        int GetEntityId()
        {
            return _lastEntityId++;
        }

        void Update()
        {
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
        
        public static readonly Vector3 CellCenter = new Vector3(0.5f, 0, 0.5f);
        public static readonly Vector3 CellSize = new Vector3(1f, 0, 1f);
        public static Vector2Int WorldToLevel(Vector3 worldPos)
        {
            var pos = worldPos - CellCenter;
            return new Vector2Int(
                Mathf.RoundToInt(pos.x),
                Mathf.RoundToInt(pos.z)
            );
        }

        public static Vector3 LevelToWorld(Vector2Int pos)
        {
            return new Vector3(pos.x + CellCenter.x, CellCenter.y, pos.y + CellCenter.z);
        }

        public static Quaternion DirectionToRotation(Direction dir)
        {
            switch (dir)
            {
                case Direction.Front:
                    return Quaternion.Euler(0, 0, 0);
                case Direction.Right:
                    return Quaternion.Euler(0, 90, 0);
                case Direction.Back:
                    return Quaternion.Euler(0, 180, 0);
                case Direction.Left:
                    return Quaternion.Euler(0, 270, 0);
                default:
                    return Quaternion.identity;        
            }
        }

        public static Direction DirectionFromForwardVector(Vector3 fwd)
        {
            if (Mathf.Abs(fwd.x) > Mathf.Abs(fwd.z))
            {
                // Left or right
                if (fwd.x > 0)
                    return Direction.Right;
                return Direction.Left;
            }
            
            // up or down
            if (fwd.z > 0)
                return Direction.Front;
            return Direction.Back;
        }

        public Entity Spawn(GameObject prefab, Vector2Int entityPosition, Direction entityOrientation)
        {
            var spawnedObject = Instantiate(prefab, LevelToWorld(entityPosition), DirectionToRotation(entityOrientation));
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

        public Entity GetEntityById(int entityId)
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
    }
}
