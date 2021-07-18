using UnityEngine;

namespace Gameplay
{
    public class KeyInputDetector : MonoBehaviour
    {
        private void Update()
        {
            var level = Common.CurrentLevel;
            
            if (level == null)
                return;
            
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                level.PlayerMove(Direction.Right);

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                level.PlayerMove(Direction.Left);

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                level.PlayerMove(Direction.Front);

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                level.PlayerMove(Direction.Back);

            if (Input.GetKey(KeyCode.R))
                level.PlayerRollback();
        }
    }
}