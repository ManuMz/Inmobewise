using UnityEngine;
using UnityEngine.UI;

public class InputFieldController : MonoBehaviour
{
    [SerializeField]
    private InputFieldType inputFieldType;
    [SerializeField]
    private InputField inputField;
    [SerializeField]
    private Animator inputFieldAnimator;
    [SerializeField]
    private Text message;

    private string currentState;

    //Propiedad de solo lectura
    public InputFieldType InputFieldType {
        get { return inputFieldType; }
    }
    private void Awake()
    {
        SetStart();  
    }
    public void SetStart()
    {
        SetAnimationSetter();
    }
    private void SetAnimationSetter()
    {
        inputFieldAnimator = GetComponent<Animator>();  
    }

    public string GetInputValue() //obtener el texto del inputField
    {
        return inputField.text;
    }
    public void SetInputValue(string newText) //escribir un nuevo texto
    {
        inputField.text = newText;
    }
    
    public InputField GetInputField()
    {
        return inputField;  
    }

    public void SetMessage(string newMessage)
    {
        message.text = newMessage;
    }

    public void SetAnimationState(string newState)
    {
        if (currentState == newState) return;

        inputFieldAnimator.Play(newState);
        
        currentState = newState;
    }
}
