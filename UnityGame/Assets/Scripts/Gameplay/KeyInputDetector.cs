using UnityEngine;


namespace Gameplay
{
    public class KeyInputDetector : MonoBehaviour
    {

        private void Update()
        {
            DetectInput();
        }

        private void DetectInput()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                Level.Instance.PlayerMove(Direction.Right);

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                Level.Instance.PlayerMove(Direction.Left);

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                Level.Instance.PlayerMove(Direction.Front);

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                Level.Instance.PlayerMove(Direction.Back);
        }
    }
}