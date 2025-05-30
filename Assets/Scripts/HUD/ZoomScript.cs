using TMPro;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
///     This script manages the zooming of the game and the minimap.
/// </summary>
public class ZoomScript : MonoBehaviour
{
    public AudioClip clickSound;
    private AudioSource audioSource;

    //minigame Zoomlevels
    public int zoomLevel = -40;
    private int maxZoomLevel = -30;
    private int minZoomLevel = -60;

    //normal camera zoom levels
    private readonly int[] gameZoomLevelX = { 320, 355, 425 };
    private readonly int[] gameZoomLevelY = { 180, 200, 240 };
    private int gameZoomLevel = 1;

    private readonly float minimapIconResizeValue = 9;
    private PixelPerfectCamera pixelCam;

    //KeyCodes
    private KeyCode minimapZoomIn;
    private KeyCode minimapZoomOut;
    private KeyCode gameZoomIn;
    private KeyCode gameZoomOut;

    public static string areaName = "Loading...";

    /// <summary>
    ///     The <c>Start</c> function is called after the object is initialized.
    ///     This function sets up the references of the object.
    /// </summary>
    private void Start()
    {
        // get components
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject miniMapCam = GameObject.Find("Minimap Camera");
        pixelCam = GameObject.Find("Main Camera").GetComponent<PixelPerfectCamera>();

        miniMapCam.transform.SetParent(player.transform);
        miniMapCam.transform.localPosition = new Vector3(0, 0, zoomLevel);

        // set up key bindings
        minimapZoomIn = GameManager.Instance.GetKeyCode(Binding.MINIMAP_ZOOM_IN);
        minimapZoomOut = GameManager.Instance.GetKeyCode(Binding.MINIMAP_ZOOM_OUT);
        gameZoomIn = GameManager.Instance.GetKeyCode(Binding.GAME_ZOOM_IN);
        gameZoomOut = GameManager.Instance.GetKeyCode(Binding.GAME_ZOOM_OUT);
        GameEvents.current.onKeybindingChange += UpdateKeybindings;

        InitializeAudio();
    }

    /// <summary>
    ///     This function adds new audio sources and sets clip with click sound
    /// </summary>
    private void InitializeAudio()
    {
        audioSource=GetComponent<AudioSource>();
        if(audioSource == null)
        {
            audioSource=gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip=clickSound;
        audioSource.playOnAwake=false;
    }

    /// <summary>
    ///     The <c>Update</c> function is called once every frame.
    ///     This function checks if the game or minimap needs to be zoomed out or in.
    /// </summary>
    private void Update()
    {
        //zoom in
        if (Input.GetKeyDown(minimapZoomIn) && !PauseMenu.menuOpen && !PauseMenu.subMenuOpen && !OpenLeaderboard.menuOpen)
        {
            MinimapZoomIn();
        }

        //zoom out
        if (Input.GetKeyDown(minimapZoomOut) && !PauseMenu.menuOpen && !PauseMenu.subMenuOpen && !OpenLeaderboard.menuOpen)
        {
            MinimapZoomOut();
        }

        //zoom game in
        if (Input.GetKeyDown(gameZoomIn) && !PauseMenu.menuOpen && !PauseMenu.subMenuOpen && !OpenLeaderboard.menuOpen)
        {
            GameZoomIn(); 
        }

        //zoom game out
        if (Input.GetKeyDown(gameZoomOut) && !PauseMenu.menuOpen && !PauseMenu.subMenuOpen && !OpenLeaderboard.menuOpen)
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

    private void OnDestroy()
    {
        GameEvents.current.onKeybindingChange -= UpdateKeybindings;
    }

    /// <summary>
    ///     This function zooms in the minimap by one step
    /// </summary>
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
        PlayClickSound();
    }

    /// <summary>
    ///     This function zooms out the minimap by one step
    /// </summary>
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
        PlayClickSound();
    }

    /// <summary>
    ///     This function zooms out the game by one step
    /// </summary>
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

    /// <summary>
    ///     This function zooms in the game by one step
    /// </summary>
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

    /// <summary>
    ///     This function updates the keybindings
    /// </summary>
    /// <param name="binding">The binding that changed</param>
    private void UpdateKeybindings(Binding binding)
    {
        if (binding == Binding.MINIMAP_ZOOM_IN)
        {
            minimapZoomIn = GameManager.Instance.GetKeyCode(Binding.MINIMAP_ZOOM_IN);
        }
        else if (binding == Binding.MINIMAP_ZOOM_OUT)
        {
            minimapZoomOut = GameManager.Instance.GetKeyCode(Binding.MINIMAP_ZOOM_OUT);
        }
        else if (binding == Binding.GAME_ZOOM_IN)
        {
            gameZoomIn = GameManager.Instance.GetKeyCode(Binding.GAME_ZOOM_IN);
        }
        else if (binding == Binding.GAME_ZOOM_OUT)
        {
            gameZoomOut = GameManager.Instance.GetKeyCode(Binding.GAME_ZOOM_OUT);
        }
    }
    /// <summary>
    /// This function is called by the zoom buttons.
    /// This function plays the click sound.
    /// </summary>
    private void PlayClickSound()
    {
        if(clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

}