using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;



public class iOSPostProcessBuild : MonoBehaviour
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        // Verifica que el target sea iOS
        if (target == BuildTarget.iOS)
        {
            string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");

            // Cargar el archivo Info.plist
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            // Obtener el diccionario raíz de Info.plist
            PlistElementDict rootDict = plist.root;

            // Añadir las claves de permisos de localización
            rootDict.SetString("NSLocationWhenInUseUsageDescription", "La aplicación necesita acceder a tu ubicación mientras está en uso.");
            rootDict.SetString("NSLocationAlwaysUsageDescription", "La aplicación necesita acceder a tu ubicación en todo momento.");

            // Guardar los cambios en Info.plist
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}
#endif
