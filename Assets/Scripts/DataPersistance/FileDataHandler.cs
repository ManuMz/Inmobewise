using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Unity.VisualScripting;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFolderName = "";
    private string dataFileName = "";

    //Constructor
    public FileDataHandler(string dataDirPath, string dataFolderName, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFolderName = dataFolderName;
        this.dataFileName = dataFileName;   
    }


    /*public ProductoData Load()
    {
        ProductoData loadedData = null;
    }*/

    public void Save (ProductoData data)
    {
        string fullPath = dataDirPath + "/" + dataFolderName + "/" + dataFileName;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            
            string dataResponse = JsonUtility.ToJson(data,true);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {

                    writer.Write(dataResponse);
                }
            }
        }
        catch (Exception ex) 
        {
            Debug.LogError($"Error al guardar los datos en el archivo {fullPath} , {ex.Message}"); 
        }
    }
}

