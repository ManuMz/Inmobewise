using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>//unica instancia
                                                         //del objeto
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadScene(string sceneIndex)
    {
        var scene = SceneManager.LoadSceneAsync(sceneIndex);
        scene.allowSceneActivation = true; //carga de escena de manera inmediata

    }
}
