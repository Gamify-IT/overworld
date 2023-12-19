using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using System.Runtime.InteropServices;

/// <summary>
///     This class manages the generator and inspector modes and sets up the correct scenes
/// </summary>
public class AreaGeneratorManager : MonoBehaviour
{
    /// <summary>
    ///     Import of the overworld close methode
    /// </summary>
    [DllImport("__Internal")]
    private static extern void CloseOverworld();

    #region Attributes
    //UI
    [SerializeField] private CameraMovement cameraController;

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Button demoButton;
    [SerializeField] private Button quitButton;

    //Data
    private string courseID;
    private bool demoMode;
    private AreaInformation currentArea;
    private AreaData areaData;
    #endregion

    private void Awake()
    {
        Setup();
    }

    private async void Setup()
    {
#if UNITY_EDITOR
        courseID = "";
#else
        courseID = Application.absoluteURL.Split("/")[^2];
#endif
        SetupUI();
        demoMode = false;

        Optional<AreaInformation> areaIdentifier = GetAreaInformation();
        if(areaIdentifier.IsPresent())
        {
            currentArea = areaIdentifier.Value();
        }
        else
        {
            //Show error screen
            infoText.text = "INVALID AREA PROVIDED";
            demoButton.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(true);
            return;
        }

        Optional<AreaData> result = await GetAreaData();
        if(result.IsPresent())
        {
            areaData = result.Value();
            LoadAreaScene();
        }
        else
        {
            //Show error screen
            infoText.text = "COULD NOT GET THE AREA DATA";
            demoButton.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(true);
        }
    }

    #region UI

    /// <summary>
    ///     This function sets up the initial state of the UI elements
    /// </summary>
    private void SetupUI()
    {
        panel.SetActive(true);
        infoText.text = "LOADING AREA DATA AND SETTING UP " + GameSettings.GetGamemode().ToString();
        demoButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
    }

    /// <summary>
    ///     This function is called by the <c>PLAY DEMO</c> button and loads the default area map for the selected area
    /// </summary>
    public void DemoButtonPressed()
    {
        areaData = new AreaData(currentArea, new Optional<CustomAreaMapData>());
        demoMode = true;
        LoadAreaScene();
    }

    /// <summary>
    ///     This function is called by the <c>QUIT</c> button and closes the Overworld
    /// </summary>
    public void QuitButtonPressed()
    {
        CloseOverworld();
    }

    #endregion

    #region GetAreaInformation
    /// <summary>
    ///     This function retrieves the area to edit / view from the URL
    /// </summary>
    /// <returns>An optional containing the provided <c>AreaInformation</c>, if valid, an empty optional otherwise</returns>
    private Optional<AreaInformation> GetAreaInformation()
    {
#if UNITY_EDITOR
        //skipping area retrieval in editor, using area 1-1 instead
        return new Optional<AreaInformation>(new AreaInformation(1, new Optional<int>(1)));
#endif

        //get gamemode parameter of url
        string urlPart = Application.absoluteURL.Split("/")[^1];

        //split in parts
        string[] areaParts = urlPart.Split("-");

        if(areaParts.Length == 3)
        {
            bool successWorldIndex = int.TryParse(areaParts[1], out int worldIndex);
            bool successDungeonIndex = int.TryParse(areaParts[2], out int dungeonIndex);

            if(successWorldIndex && successDungeonIndex)
            {
                Optional<int> optionalDungeonIndex = new Optional<int>();
                if(dungeonIndex != 0)
                {
                    optionalDungeonIndex.SetValue(dungeonIndex);
                }
                return new Optional<AreaInformation>(new AreaInformation(worldIndex, optionalDungeonIndex));
            }
            else
            {
                Debug.Log("Invalid area provided: " + urlPart);
                return new Optional<AreaInformation>();
            }
        }
        else
        {
            Debug.Log("Invalid area provided: " + urlPart);
            return new Optional<AreaInformation>();
        }     
    }

    #endregion

    #region GetAreaData
    /// <summary>
    ///     This function retrieves the needed data from the backend
    /// </summary>
    /// <returns></returns>
    private async UniTask<Optional<AreaData>> GetAreaData()
    {
#if UNITY_EDITOR
        //use local files in editor
        Debug.Log("Load from local files");
        return new Optional<AreaData>(LoadLocalData());
#endif

        //load data from backend
        string path = GameSettings.GetOverworldBackendPath() + "/courses/" + courseID + "/areaMaps/" + currentArea.GetWorldIndex();
        if(currentArea.IsDungeon())
        {
            path += "/dungeon/" + currentArea.GetDungeonIndex();
        }

        Optional<AreaDTO> areaDTO = await RestRequest.GetRequest<AreaDTO>(path);
        if(areaDTO.IsPresent())
        {
            AreaData areaData = AreaData.ConvertDtoToData(areaDTO.Value());
            return new Optional<AreaData>(areaData);
        }
        else
        {
            Debug.LogError("Error loading area map");
            return new Optional<AreaData>();
        }
    }

    /// <summary>
    ///     This function loads the current area data from a local json file
    /// </summary>
    /// <returns>The <c>AreaData</c> stored in the local files</returns>
    private AreaData LoadLocalData()
    {
        string path;
        if (currentArea.IsDungeon())
        {
            path = "Areas/Dungeon" + currentArea.GetWorldIndex() + "-" + currentArea.GetDungeonIndex();            
        }
        else
        {
            path = "Areas/World" + currentArea.GetWorldIndex();
        }
        
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;
        AreaDTO areaDTO = AreaDTO.CreateFromJSON(json);
        AreaData areaData = AreaData.ConvertDtoToData(areaDTO);
        areaData.SetArea(currentArea);
        return areaData;
    }
    #endregion

    /// <summary>
    ///     This function start the correct scene and sets it up
    /// </summary>
    private async void LoadAreaScene()
    {
        if(currentArea.IsDungeon())
        {
            await SceneManager.LoadSceneAsync("Dungeon", LoadSceneMode.Additive);            
        }
        else
        {
            string sceneName = "World " + currentArea.GetWorldIndex();
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        GameObject areaManagerObject = GameObject.FindGameObjectWithTag("AreaManager");
        if (areaManagerObject == null)
        {
            infoText.text = "Something went wrong :(";
            demoButton.gameObject.SetActive(false);

            Debug.LogError("Area Manager Object not found");
            return;
        }

        AreaManager areaManager = areaManagerObject.GetComponent<AreaManager>();
        if (areaManager == null)
        {
            infoText.text = "Something went wrong :(";
            demoButton.gameObject.SetActive(false);

            Debug.LogError("Area Manager Script not found");
            return;
        }

        panel.SetActive(false);

        Gamemode gamemode = GameSettings.GetGamemode();
        if (gamemode == Gamemode.GENERATOR)
        {
            areaManager.SetupGenerator(courseID, this, areaData, currentArea, cameraController, true, demoMode);
        }
        else if(gamemode == Gamemode.INSPECTOR)
        {
            areaManager.SetupInspector(courseID, areaData, currentArea, cameraController, demoMode);
        }
    }

    /// <summary>
    ///     This function reload the correct scene to reset the layout
    /// </summary>
    /// <param name="areaData">The data to set up the area with</param>
    public async void ReloadArea(AreaData areaData)
    {
        if (currentArea.IsDungeon())
        {
            await SceneManager.UnloadSceneAsync("Dungeon");
            await SceneManager.LoadSceneAsync("Dungeon", LoadSceneMode.Additive);
        }
        else
        {
            string sceneName = "World " + currentArea.GetWorldIndex();
            await SceneManager.UnloadSceneAsync(sceneName);
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        GameObject areaManagerObject = GameObject.FindGameObjectWithTag("AreaManager");
        if (areaManagerObject == null)
        {
            Debug.LogError("Area Manager Object not found");
            return;
        }

        AreaManager areaManager = areaManagerObject.GetComponent<AreaManager>();
        if (areaManager == null)
        {
            Debug.LogError("Area Manager Script not found");
            return;
        }

        Gamemode gamemode = GameSettings.GetGamemode();
        if (gamemode == Gamemode.GENERATOR)
        {
            areaManager.SetupGenerator(courseID, this, areaData, currentArea, cameraController, false, demoMode);
        }
    }
}
