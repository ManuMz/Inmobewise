using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class RegisterController : MonoBehaviour
{
    //Variables Globales de la clase referentes a InputFields
    private InputField fullNameInput;
    private InputField userNameInput;
    private InputField emailInput;  
    private InputField phoneNumberInput;
    private InputField passwordInput;
    private InputField confirmPasswordInput;

    //Estados de animacion en InputFields
    private const string IDLE_INPUTFIELD = "IdleInputField";
    private const string INPUTFIELD_MESSAGE = "InputFieldMessage";
    void Start()
    {
        Register();
    }

    void Update()
    {
         
    }

    private void Register()
    {
        fullNameInput = UIController.Instance.GetInputField(
            InputFieldType.fullNameRegister);
        userNameInput = UIController.Instance.GetInputField(
            InputFieldType.userNameRegister);

        fullNameInput.onValueChanged.AddListener(
                ValidateFullName
            );
        userNameInput.onValueChanged.AddListener(
            ValidateUserName
            );
    }

    private void ValidateFullName(string fullName)
    {
        if (string.IsNullOrEmpty(fullName)) 
        {
            UIController.Instance.SetInputFieldMessage(
              InputFieldType.fullNameRegister, "Campo requerido",
              INPUTFIELD_MESSAGE);
            //return false;
        }
        else
        {
            UIController.Instance.SetInputFieldMessage(
             InputFieldType.fullNameRegister, "",
             IDLE_INPUTFIELD);
            //return true;
        }
    }
    private void ValidateUserName(string userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            UIController.Instance.SetInputFieldMessage(
               InputFieldType.userNameRegister, "Campo requerido",
               INPUTFIELD_MESSAGE);
            //return false;

        }
        else if (userName.Length<4)
        {
            UIController.Instance.SetInputFieldMessage(
              InputFieldType.userNameRegister, "Debe tener minimo 4 caracteres",
              INPUTFIELD_MESSAGE);
            //return false;
        }
        else
        {
            UIController.Instance.SetInputFieldMessage(
              InputFieldType.userNameRegister, "",
              IDLE_INPUTFIELD);
            //return true;
        }
    }
    private void ValidateEmail()
    {
        
        
    }
    private void ValidatePassword()
    {

    }
    private void ConfirmPassword()
    {

    }
    public void BackToLogin()
    {
        UIController.Instance.ShowPanel(PanelType.login);
    }

}
