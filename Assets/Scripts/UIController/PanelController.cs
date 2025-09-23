using UnityEngine;
using static UIController;

public class PanelController : MonoBehaviour
{
    public PanelType panelType;

    public void SendUITrigger(int triggerIndex)
    {
        UIController.sharedInstance.SendUITrigger(triggerIndex);
    }
}
