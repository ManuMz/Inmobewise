using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

public class NativeShareHandler : MonoBehaviour
{
    public static NativeShareHandler sharedInstance;

    private void Awake()
    {
        if (sharedInstance)
            Destroy(gameObject);
        else
            sharedInstance = this;
    }
    public void Share(UnityAction before, UnityAction successCallback)
    {

        StartCoroutine(TakeScreenshotAndShare(before, successCallback));
    }

    public void Share(Texture2D texture, UnityAction succesCallBack)
    {
        StartCoroutine(IShare(texture, succesCallBack));
    }


    IEnumerator IShare(Texture2D texture, UnityAction succesCallBack)
    {


        yield return new WaitForEndOfFrame();

        System.Guid nameScreen = System.Guid.NewGuid();
        string filePath = Path.Combine(Application.temporaryCachePath, nameScreen + ".png");
        File.WriteAllBytes(filePath, texture.EncodeToPNG());

        NativeShare nativeShare = new NativeShare();
        nativeShare.AddFile(filePath)
            .SetSubject("Inmobiwise").SetText("").SetUrl("")
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();

        succesCallBack();
    }


    IEnumerator TakeScreenshotAndShare(UnityAction before, UnityAction successCallback)
    {
        before.Invoke();

        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);
        NativeShare nativeShare = new NativeShare();
        nativeShare.AddFile(filePath)
            .SetSubject("").SetText("").SetUrl("")
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();

        successCallback.Invoke();

    }
}
