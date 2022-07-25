using TMPro;
using UnityEngine;

/*
 * This scripts manages the minimap. It manages the zoom level as well as the size of icons displayed in the minimap.
 */
public class MinimapScript : MonoBehaviour
{
    public int zoomLevel;
    private float minimapIconResizeValue = (float)6.3;

    public static string areaName = "Loading...";

    /*
     * The Start function is called when the object is initialized and sets up the starting values and state of the object. 
     */
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
    }

    /*
     * The Update function is called every frame. It updates all values according to the changes happened since the last frame. 
     * It increases or decreases the size of the minimap if a input happened and adjusts the icon size. 
     */
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

        //display the current AreaName under the minimap
        GameObject.FindGameObjectWithTag("AreaName").GetComponent<TextMeshProUGUI>().text = areaName;

        foreach (GameObject minimapIcon in GameObject.FindGameObjectsWithTag("MinimapIcon"))
        {
            minimapIcon.transform.localScale = new Vector3(-1 * zoomLevel / minimapIconResizeValue,
                -1 * zoomLevel / minimapIconResizeValue, 0);
        }
    }

    /*
     * This function zooms in the minimap by one step.
     */
    public void ZoomIn()
    {
        //restrict zoom level
        if (zoomLevel < -20)
        {
            //zoom in
            zoomLevel += 10;
            GameObject.Find("Minimap Camera").transform.localPosition = new Vector3(0, 0, zoomLevel);

            foreach (GameObject minimapIcon in GameObject.FindGameObjectsWithTag("MinimapIcon"))
            {
                minimapIcon.transform.localScale = new Vector3(-1 * zoomLevel / minimapIconResizeValue,
                    -1 * zoomLevel / minimapIconResizeValue, 0);
            }
        }
    }

    /*
     * This function zooms out the minimap by one step.
     */
    public void ZoomOut()
    {
        //restrict zoom level
        if (zoomLevel > -50)
        {
            //zoom out
            zoomLevel -= 10;
            GameObject.Find("Minimap Camera").transform.localPosition = new Vector3(0, 0, zoomLevel);

            foreach (GameObject minimapIcon in GameObject.FindGameObjectsWithTag("MinimapIcon"))
            {
                minimapIcon.transform.localScale = new Vector3(-1 * zoomLevel / minimapIconResizeValue,
                    -1 * zoomLevel / minimapIconResizeValue, 0);
            }
        }
    }
}