using UnityEngine;

namespace UIF.Scripts.Transitions
{
    public static class Utils
    {
        public static CanvasGroup GetOrCreateCanvas(GameObject gameObject)
        {
            var canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
                return canvasGroup;

            return gameObject.AddComponent<CanvasGroup>();
        }   
        
        public static Vector2 DirectionToScreenDelta(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new Vector2(0, -Screen.height);
                case Direction.Down:
                    return new Vector2(0, Screen.height);
                case Direction.Left:
                    return new Vector2(Screen.width, 0);
                case Direction.Right:
                    return new Vector2(-Screen.width, 0);
                default:
                    return Vector2.zero;
            }
        }
    }
}