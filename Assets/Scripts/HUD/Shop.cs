using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{

    public GameObject ShopPanel;

    void Start()
    {
        
    }

    public void closeShop()
    {
        ShopPanel.SetActive(false);
    }

}
