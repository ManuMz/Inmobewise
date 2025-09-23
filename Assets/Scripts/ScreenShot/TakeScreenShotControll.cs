using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System;

public class TakeScreenShotController : MonoBehaviour
{
    public static TakeScreenShotController sharedInstance;

    [SerializeField]
    private delegate void Finished();
    [SerializeField]
    private event Finished OnFinished;
    
    [SerializeField]
    private GameObject background;
    [SerializeField]
    private RawImage preview;
    [SerializeField]
    private Texture2D currentTex;
    [SerializeField]
    private GameObject waterMark;
    [SerializeField]
    private GameObject RingMenu;
    [SerializeField]
    private GameObject AnimShot;
    [SerializeField]
    private GameObject instanceShot;
    [SerializeField]
    private GameObject groupCanvas;
    [SerializeField]
    private GameObject btnDelete, btnShare, btnSave;
    private void Awake()
    {
        if (sharedInstance)
        {
            Destroy(gameObject);
        }
        else
        {
            sharedInstance = this;
        }
       
    }

    public void ScreenShoot(UnityAction beforeCallBack, UnityAction successCallBack, Texture2D texture = null)
    {
        beforeCallBack();
        successCallBack += () => {
            
            Show();
        };
        gameObject.SetActive(true);
        StartCoroutine(IScreenShoot(successCallBack));
    }

    public void ScreenShoot()
    {
        ScreenShoot(() =>
        {
            groupCanvas.SetActive(false);
            waterMark.SetActive(true);
        }, () =>
        {
            groupCanvas.SetActive(true);
            waterMark.SetActive(false);
        });
    }

    void Show()
    {
        background.GetComponent<Image>().raycastTarget = true;
        background.GetComponent<Image>().DOFade(.65f, .3f).OnComplete(() =>
        {
            preview.DOFade(1, .3f);
            btnDelete.transform.DOScale(1, .3f).SetEase(Ease.InElastic);
            btnSave.transform.DOScale(1, .3f).SetEase(Ease.InElastic);
            btnShare.transform.DOScale(1, .3f).SetEase(Ease.InElastic);
        });
    }

    void Hide()
    {

        OnFinished?.Invoke();
        preview.DOFade(0, .3f).OnComplete(() => {
            background.GetComponent<Image>().DOFade(0f, .3f);
            background.GetComponent<Image>().raycastTarget = false;
        });
        btnDelete.transform.DOScale(0, .3f).SetEase(Ease.OutElastic);
        btnSave.transform.DOScale(0, .3f).SetEase(Ease.OutElastic);
        btnShare.transform.DOScale(0, .3f).SetEase(Ease.OutElastic);

    }

    private IEnumerator IScreenShoot(UnityAction successCallBack, Texture2D texture = null)
    {
        yield return new WaitForEndOfFrame();
        if (texture == null)
        {
            Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            ss.Apply();

            preview.texture = ss;
            currentTex = ss;
        }
        else
        {
            currentTex = texture;
        }


        yield return new WaitForEndOfFrame();
        successCallBack();
    }

    public void Desechar()
    {
        preview.texture = null;
        currentTex = null;
        Hide();
    }

    public void Save()
    {
        StartCoroutine(ISave());
    }


    IEnumerator ISave()
    {
        NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);
        permission = NativeGallery.RequestPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);

        if (permission == NativeGallery.Permission.Denied)
        {
            Debug.Log("Not allowed");
            yield break; // Salir si no se permite el acceso
        }

        // Generar un nombre basado en la fecha y hora
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = "inmobiwise_" + timestamp + ".png";

        Debug.Log("Permission result: " + NativeGallery.SaveImageToGallery(currentTex, "Inmobiwise", fileName));

        Destroy(currentTex);

        yield return new WaitForEndOfFrame();
        Hide();
    }

    public void Share()
    {
        NativeShareHandler.sharedInstance.Share(currentTex, () => {
            Hide();
        });
    }

    public void Shot()
    {
        if (!instanceShot)
        {
            Transform canvas = GameObject.Find("Canvas").transform;
            instanceShot = GameObject.Instantiate(AnimShot, canvas);


        }
    }


    public void TakeAndSaveScreenShot()
    {
        StartCoroutine(TakeScreenshotAndSave());

    }



    private IEnumerator TakeScreenshotAndSave()
    {
        RingMenu.SetActive(false);
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();
        NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);
        permission = NativeGallery.RequestPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);
        // If we weren't denied but told to ask, this will handle the case if the user denied it.
        // otherwise if it was denied then we return and do not attempt to save the screenshot
        if (permission == NativeGallery.Permission.Denied)
        {
            Debug.Log("Not allowed");
            //DebugTesting.debugger.text = "Not Allowed ";
            //yield return null;
        }
        else
        {
            //DebugTesting.debugger.text = "Allowed ";
        }
        // Save the screenshot to Gallery/Photos
        Debug.Log("Permission result: " + NativeGallery.SaveImageToGallery(ss, "Inmobiwise", "Inmobiwise_Image.png"));

        //DebugTesting.debugger.text = "Permission result: " + NativeGallery.SaveImageToGallery(ss, "ArviSpaceAR", "Arvis_Image.png");
        // To avoid memory leaks
        Destroy(ss);

        yield return new WaitForEndOfFrame();
        RingMenu.SetActive(true);
    }

    public void ScreenShotAndSave(UnityAction beforeCallBack, UnityAction successCallback)
    {
        StartCoroutine(IScreenShotAndSave(beforeCallBack, successCallback));
    }



    private IEnumerator IScreenShotAndSave(UnityAction beforeCallBack, UnityAction successCallback)
    {
        beforeCallBack.Invoke();
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();
        NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);
        //DebugTesting.debugger.text = "Va a preguntar ";
        permission = NativeGallery.RequestPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);
        // If we weren't denied but told to ask, this will handle the case if the user denied it.
        // otherwise if it was denied then we return and do not attempt to save the screenshot
        if (permission == NativeGallery.Permission.Denied)
        {
            Debug.Log("Not allowed");
            //DebugTesting.debugger.text = "Not Allowed ";
            //yield return null;
        }
        // Save the screenshot to Gallery/Photos
        System.Guid nameScreen = System.Guid.NewGuid();

        Debug.Log("Permission result: " + NativeGallery.SaveImageToGallery(ss, "Inmobiwise", nameScreen.ToString() + ".png"));

        // To avoid memory leaks
        Destroy(ss);

        yield return new WaitForEndOfFrame();
        successCallback.Invoke();
    }


    private void PickImage(int maxSize)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                // Assign texture to a temporary quad and destroy it after 5 seconds
                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
                quad.transform.forward = Camera.main.transform.forward;
                quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

                Material material = quad.GetComponent<Renderer>().material;
                if (!material.shader.isSupported) // happens when Standard shader is not included in the build
                    material.shader = Shader.Find("Legacy Shaders/Diffuse");

                material.mainTexture = texture;

                Destroy(quad, 5f);

                // If a procedural texture is not destroyed manually, 
                // it will only be freed after a scene change
                Destroy(texture, 5f);
            }
        }, "Select a PNG image", "image/png");

        Debug.Log("Permission result: " + permission);
    }



    #region Captura/ toma de video 


    //private void PickVideo()
    //{
    //    NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
    //    {
    //        Debug.Log("Video path: " + path);
    //        if (path != null)
    //        {
    //            // Play the selected video
    //            Handheld.PlayFullScreenMovie("file://" + path);
    //        }
    //    }, "Select a video");

    //    Debug.Log("Permission result: " + permission);
    //}

    #endregion
}