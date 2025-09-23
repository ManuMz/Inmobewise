using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RingPiece : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Image ringPiece;

    public Image GetIcon()
    {
        return icon;
    }

    public void SetIconFromURL(string url)
    {
        StartCoroutine(DownloadTexture(url));
    }

    public Image GetRingPiece()
    {
        return ringPiece;
    }


    IEnumerator DownloadTexture(string url)
    {
        using (UnityWebRequest getTextureRequest = UnityWebRequestTexture.GetTexture(url))
        {
            UnityWebRequestAsyncOperation asyncGetTextureOperation = getTextureRequest.SendWebRequest();

            //Progress
            while (!asyncGetTextureOperation.isDone)
            {
                yield return null;
            }

            //Request
            if (!getTextureRequest.downloadHandler.text.Contains("404 Not Found"))
            {
                int size = (int)icon.GetComponent<RectTransform>().rect.width;

                Texture2D newTexture = Texture2DHelper.scaleTexture(DownloadHandlerTexture.GetContent(getTextureRequest), size);

                icon.sprite = Sprite.Create(
                    newTexture,
                    new Rect(0, 0, newTexture.width, newTexture.height),
                    new Vector2(0.5f, 0.5f)
                    );
            }
        }
    }

}
