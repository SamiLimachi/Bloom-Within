using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public List<int> keys = new List<int>();

    public void AddKey(int keyID)
    {
        if (!keys.Contains(keyID))
        {
            keys.Add(keyID);
            Debug.Log("Llave " + keyID + " recogida");
        }
    }

    public bool HasKey(int keyID)
    {
        return keys.Contains(keyID);
    }
}
