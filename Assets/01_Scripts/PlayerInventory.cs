using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public List<int> keys = new List<int>();
    private InventoryUI inventoryUI;
    private void Start()
    {
        inventoryUI = FindObjectOfType<InventoryUI>(); // busca la UI en la escena
    }
    public void AddKey(int keyID)
    {
        if (!keys.Contains(keyID))
        {
            keys.Add(keyID);
            Debug.Log("Llave " + keyID + " recogida");
            if (inventoryUI != null)
                inventoryUI.ShowKey();
        }

    }

    public bool HasKey(int keyID)
    {
        return keys.Contains(keyID);
    }
}
