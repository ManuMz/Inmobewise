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
        fullNameInput = UIController.sharedInstance.GetInputField(
            InputFieldType.fullNameRegister);
        userNameInput = UIController.sharedInstance.GetInputField(
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
            UIController.sharedInstance.SetInputFieldMessage(
              InputFieldType.fullNameRegister, "Campo requerido",
              INPUTFIELD_MESSAGE);
            //return false;
        }
        else
        {
            UIController.sharedInstance.SetInputFieldMessage(
             InputFieldType.fullNameRegister, "",
             IDLE_INPUTFIELD);
            //return true;
        }
    }
    private void ValidateUserName(string userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            UIController.sharedInstance.SetInputFieldMessage(
               InputFieldType.userNameRegister, "Campo requerido",
               INPUTFIELD_MESSAGE);
            //return false;

        }
        else if (userName.Length<4)
        {
            UIController.sharedInstance.SetInputFieldMessage(
              InputFieldType.userNameRegister, "Debe tener minimo 4 caracteres",
              INPUTFIELD_MESSAGE);
            //return false;
        }
        else
        {
            UIController.sharedInstance.SetInputFieldMessage(
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
        UIController.sharedInstance.ShowPanel(PanelType.login);
    }

}
