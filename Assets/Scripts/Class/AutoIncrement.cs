using UnityEngine;

public class AutoIncrement
{
    private static int id=1;
    public int GenerateId() 
    {
        return id++;
      

    }
}
