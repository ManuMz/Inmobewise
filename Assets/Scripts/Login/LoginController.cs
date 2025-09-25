using UnityEngine;

public class LoginController : MonoBehaviour
{
  
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void RegisterHere()
    {
        UIController.Instance.ShowPanel(PanelType.register);
    }
}
