using UnityEngine;
using Firebase.Auth;
using System.Threading.Tasks;
using System;
using System.Net.Mail;

public class FirebaseManager
{
    private FirebaseUser user;//usuario Firebase
    private FirebaseAuth auth; //variable de autenticacion 
    private void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;//Acceso a la clase
                                                          //firebase
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private void AuthStateChanged(object sender, 
        System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                && auth.CurrentUser.IsValid();
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                //displayName = user.DisplayName ?? "";
                //emailAddress = user.Email ?? "";
                //photoUrl = user.PhotoUrl ?? "";
            }
        }
    }


    /// <summary>
    /// Inicio de sesion y autenticacion con Google
    /// </summary>
    /// <param name="googleIdToken"></param>
    /// <param name="googleAccessToken"></param>
    public void SignInGoogle(string googleIdToken, string googleAccessToken)
    {
        try
        {
            var credential = Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);

            auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
                    return;
                }

                Firebase.Auth.AuthResult result = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);
            });
        }catch(Exception e)
        {
            Debug.LogError("Error en SignInGoogle: " + e.Message);
        }
    }

    public void SignOutGoogle()
    {
        auth.SignOut(); 
    }
    void OnDestroy()
    {
        auth.StateChanged -=  AuthStateChanged;
        auth = null;
    }

}
