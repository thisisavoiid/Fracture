using UnityEngine;

public class ButtonClickHelper : MonoBehaviour
{
    public void StartMatch()
    {
        GameFlowManager.Instance?.StartMatchFlow();
    }
}
