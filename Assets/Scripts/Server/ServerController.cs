using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ServerController : MonoBehaviour
{

    private const string urlServices = "https://arvispace.com/InmobewiseApp/servicios/";
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //CONSUMO DE API REST

    public async UniTask Login(Action action)
    {

    }
    public async UniTask Register(Action action)
    {
        string path = urlServices + "";

        WWWForm registerForm = new WWWForm();
        //registerForm.AddField("", );

        await RESTApi.HTTPPOST(path,registerForm);
    }

    //SUBIDA - DESCARGA DE ASSETS
}
