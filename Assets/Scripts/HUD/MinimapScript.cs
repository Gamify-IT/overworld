using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MinimapScript : MonoBehaviour
{
    public int zoomLevel;

    public static string areaName = "Loading...";
    // Start is called before the first frame update
    void Start()
    {
        //set the zoom level
        zoomLevel = -30;
        //get the player and attach the minimap to him
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject miniMapCam = GameObject.Find("Minimap Camera");
        miniMapCam.transform.SetParent(player.transform);
        //set the local transform of the minimap camera to be above the player, zoomed out by the specified zoomLevel
        miniMapCam.transform.localPosition = new Vector3(0, 0, zoomLevel);
        
        foreach(GameObject minimapIcon in GameObject.FindGameObjectsWithTag("MinimapIcon"))
        {
            minimapIcon.transform.localScale = new Vector3(-1*zoomLevel/5, -1*zoomLevel/5, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //zoom in
        if (Input.GetKeyDown(KeyCode.P))
        {
            ZoomIn();
        }
        //zoom out
        if (Input.GetKeyDown(KeyCode.O))
        {
            ZoomOut();
        }

        GameObject.FindGameObjectWithTag("AreaName").GetComponent<TextMeshProUGUI>().text = areaName;
    }
    //zoom In handling
    public void ZoomIn()
    {
        //restrict zoom level
        if (zoomLevel < -20)
        {
            //zoom in
            zoomLevel += 10;
            GameObject.Find("Minimap Camera").transform.localPosition = new Vector3(0, 0, zoomLevel);

            foreach(GameObject minimapIcon in GameObject.FindGameObjectsWithTag("MinimapIcon"))
            {
                minimapIcon.transform.localScale = new Vector3(-1*zoomLevel/5, -1*zoomLevel/5, 0);
            }

        }
        
    }
    //zoom Out handling
    public void ZoomOut()
    {
        //restrict zoom level
        if (zoomLevel > -50)
        {
            //zoom out
            zoomLevel -= 10;
            GameObject.Find("Minimap Camera").transform.localPosition = new Vector3(0, 0, zoomLevel);
            
            foreach(GameObject minimapIcon in GameObject.FindGameObjectsWithTag("MinimapIcon"))
            {
                minimapIcon.transform.localScale = new Vector3(-1*zoomLevel/5, -1*zoomLevel/5, 0);
            }
        }

    }
}
