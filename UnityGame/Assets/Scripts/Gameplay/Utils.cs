using UnityEngine;

namespace Gameplay
{
    public static class Utils
    {
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
        
        public static Vector2Int MoveDelta(Direction dir)
        {
            switch (dir)
            {
                case Direction.Front:
                    return Vector2Int.up;
                case Direction.Right:
                    return Vector2Int.right;
                case Direction.Back:
                    return Vector2Int.down;
                case Direction.Left:
                    return Vector2Int.left;
                default:
                    return Vector2Int.zero;
            }
        }
        
        public static Direction RevertDirection(Direction direction)
        {
            /*
            Front 0  ->  Back 2
            Right 1  ->  Left 3
            Back  2  ->  Front 0
            Left  3  ->  Right 1
             */
            return (Direction) (((int) direction + 2) % 4);
        }

        public static Direction RelativeDirection(Direction absoluteHitDirection, Direction entityOrientation)
        {
            /*
            Front = 0,
            Right = 1,
            Back = 2,
            Left = 3
             */
            var absolute = (int)absoluteHitDirection;
            var orientation = (int)entityOrientation;
            
            
            /* absoluteHitDirection   entityOrientation    Relative
             * Front  0               Front 0              Front 0
             * Front  0               Right 1              Left  3
             * Front  0               Back  2              Back  2
             * Front  0               Left  3              Right 1
             * Right  1               Front 0              Right 1
             * Right  1               Right 1              Fron  0
             */

            return (Direction)((absolute - orientation + 4) % 4);
        }

        public static bool IsInsideRadius(Vector2Int source, Vector2Int target, int radius)
        {
            return Mathf.Abs(source.x - target.x) <= radius && 
                   Mathf.Abs(source.y - target.y) <= radius;
        }
    }
}