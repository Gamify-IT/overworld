using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class ZoomScript : MonoBehaviour
{
    //minigame Zoomlevels
    public int zoomLevel = -40;
    private int maxZoomLevel = -30;
    private int minZoomLevel = -60;

    //normal camera zoom levels
    private int[] gameZoomLevelX = new int[] { 320, 355, 425 };
    private int[] gameZoomLevelY = new int[] { 180, 200, 240 };
    private int gameZoomLevel = 1;

    private float minimapIconResizeValue = (float)9;
    private PixelPerfectCamera pixelCam;

    public static string areaName = "Loading...";

    // Start is called before the first frame update
    void Start()
    {
        //get the player and attach the minimap to him
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject miniMapCam = GameObject.Find("Minimap Camera");
        //get the pixel perfect camera component
        pixelCam = GameObject.Find("Main Camera").GetComponent<PixelPerfectCamera>();
        miniMapCam.transform.SetParent(player.transform);
        //set the local transform of the minimap camera to be above the player, zoomed out by the specified zoomLevel
        miniMapCam.transform.localPosition = new Vector3(0, 0, zoomLevel);
    }

    // Update is called once per frame
    void Update()
    {
        //zoom in
        if (Input.GetKeyDown(KeyCode.P))
        {
            MinimapZoomIn();
        }

        //zoom out
        if (Input.GetKeyDown(KeyCode.O))
        {
            MinimapZoomOut();
        }

        //zoom game in
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            GameZoomIn();
        }

        //zoom game out
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            GameZoomOut();
        }

        GameObject.FindGameObjectWithTag("AreaName").GetComponent<TextMeshProUGUI>().text = areaName;

        foreach (GameObject minimapIcon in GameObject.FindGameObjectsWithTag("MinimapIcon"))
        {
            minimapIcon.transform.localScale = new Vector3(-1 * zoomLevel / minimapIconResizeValue,
                -1 * zoomLevel / minimapIconResizeValue, 0);
        }
    }

    //zoom In handling
    public void MinimapZoomIn()
    {
        //restrict zoom level
        if (zoomLevel < maxZoomLevel)
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

    //zoom Out handling
    public void MinimapZoomOut()
    {
        //restrict zoom level
        if (zoomLevel > minZoomLevel)
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

    //zoom game in handling
    public void GameZoomOut()
    {
        //restrict to 3 possible zoomLevels
        if (gameZoomLevel < 2)
        {
            gameZoomLevel += 1;
            //change the reference resolution, which effectively changes the zoom level
            pixelCam.refResolutionX = gameZoomLevelX[gameZoomLevel];
            pixelCam.refResolutionY = gameZoomLevelY[gameZoomLevel];
            //also zoom out the minimap at the same time and change the max and min zoom levels
            maxZoomLevel -= 10;
            minZoomLevel -= 10;
            MinimapZoomOut();
        }
    }

    //zoom game out handling
    public void GameZoomIn()
    {
        //restrict to 3 possible zoomLevels
        if (gameZoomLevel > 0)
        {
            gameZoomLevel -= 1;
            //change the reference resolution, which effectively changes the zoom level
            pixelCam.refResolutionX = gameZoomLevelX[gameZoomLevel];
            pixelCam.refResolutionY = gameZoomLevelY[gameZoomLevel];
            //also zoom in the minimap at the same time and change the max and min zoom levels
            maxZoomLevel += 10;
            minZoomLevel += 10;
            MinimapZoomIn();
        }
    }
}