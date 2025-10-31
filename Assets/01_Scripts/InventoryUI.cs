using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Image keyIcon;

    void Start()
    {
        keyIcon.enabled = false;
    }

    public void ShowKey()
    {
        keyIcon.enabled = true;
    }
}
