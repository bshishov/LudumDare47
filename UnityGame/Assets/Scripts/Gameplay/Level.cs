using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using Utils.Debugger;

namespace Gameplay
{
    public class Level : Singleton<Level>
    {
        public int MaxTurns = 10;

        private int _currentTurn = 0;
        private List<Entity> _entities;
        private Entity _playerEntity;
        private readonly Queue<ICommand> _turnQueue = new Queue<ICommand>();
        private readonly List<ICommand> _history = new List<ICommand>();

        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            _currentTurn = 0;
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
            Debugger.Default.Display("Turn", _currentTurn);
            if (_turnQueue.Count > 0)
            {
                Exec(_turnQueue.Dequeue());
            }
            else
            {
                HandleInput();
            }
        }

        private void HandleInput()
        {
            var playerId = _playerEntity.Id;
            //var playerId = -1;
            
            //var h = Input.GetAxis("Horizontal");
            //var v = Input.GetAxis("Vertical");
            
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                Dispatch(new MoveCommand(playerId, Direction.Right, true));
            
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                Dispatch(new MoveCommand(playerId, Direction.Left, true));
            
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                Dispatch(new MoveCommand(playerId, Direction.Up, true));
            
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                Dispatch(new MoveCommand(playerId, Direction.Down, true));
        }

        private void Exec(ICommand command)
        {
            Debug.Log($"Executing command {command} for {command.TargetId}");

            if (command.TargetId >= 0)
            {
                var target = _entities[command.TargetId];
                target.Execute(this, command);
            }
            
            _history.Add(command);
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
