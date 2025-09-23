using UnityEngine;

public interface IDataPersistence
{
    void LoadDataCollection(ProductosDataCollection data);

    void SaveDataCollection(ref ProductosDataCollection data);


    void LoadData(ProductoData data);

    void SaveData(ref ProductoData data);   
}
