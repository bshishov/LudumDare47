using UnityEngine.UI;
using Utils;

public class UIDebugText : Singleton<UIDebugText>
{
    public Text text;
    public string DebugText;
    
    public void ShowDebugText(string debugText) {
        text.text = text.text + "\n" + debugText;
    }
}
