using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Gameplay
{
    public class Level : Singleton<Level>
    {
        public int MaxTurns = 10;

        private List<Entity> _entities;
        private Entity _playerEntity;
        private readonly Queue<ICommand> _turnQueue = new Queue<ICommand>();
        private readonly Stack<Turn> _history = new Stack<Turn>();
        private float _currentRollbackCd;
        private const float RollbackCd = 0.08f;

        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            _entities = GameObject.FindObjectsOfType<Entity>().ToList();
            _playerEntity = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
            var id = 0;
            foreach (var entity in _entities)
            {
                entity.Initialize(id);
                id++;
            }
            _turnQueue.Clear();
            _history.Clear();
        }

        void Update()
        {
            if (_turnQueue.Count > 0)
            {
                Exec(_turnQueue.Dequeue());
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
                NewTurn(Direction.Up);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                NewTurn(Direction.Down);
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
            var currentTurnNumber = 0;
            if (currentTurn != null)
                currentTurnNumber = currentTurn.Number;
            _history.Push(new Turn(currentTurnNumber + 1));
            
            // Player moves first
            var playerId = _playerEntity.Id;
            Dispatch(new MoveCommand(playerId, dir, true));

            // Rest of the objects move afterwards
            foreach (var entity in _entities)
            {
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
            _turnQueue.Enqueue(command);   
        }

        public Entity GetEntityAt(Vector2Int position)
        {
            return _entities.FirstOrDefault(entity => entity.Position == position);
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
                case Direction.Up:
                    return Quaternion.Euler(0, 0, 0);
                case Direction.Right:
                    return Quaternion.Euler(0, 90, 0);
                case Direction.Down:
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
                return Direction.Up;
            return Direction.Down;
        }
    }
}
