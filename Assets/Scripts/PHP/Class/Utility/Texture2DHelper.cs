using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Texture2DHelper
{
    public static Texture2D scaleTexture(Texture2D source, int size)
    {
        Texture2D result = new Texture2D(size, size, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)size);
        float incY = (1.0f / (float)size);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % size), incY * ((float)Mathf.Floor(px / size)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }
}
