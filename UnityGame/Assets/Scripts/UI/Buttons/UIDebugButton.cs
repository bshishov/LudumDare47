using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UIDebugButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonPressed);
    }
    private void OnButtonPressed()
    {
        GamePersist.Instance.ClearAllSaveData();
    }
}
