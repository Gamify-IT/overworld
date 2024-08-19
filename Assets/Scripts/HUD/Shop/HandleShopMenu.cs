using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleShopMenu : MonoBehaviour
{

    public GameObject ShopPanel;

    void Start()
    {
        
    }

    public void closeShop()
    {
        ShopPanel.SetActive(false);
        Time.timeScale = 1f;
    }

}
