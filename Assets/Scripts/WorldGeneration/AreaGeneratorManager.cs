using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public class AreaGeneratorManager : MonoBehaviour
{
    #region Attributes
    //UI
    [SerializeField] private CameraMovement cameraController;

    //Data
    private string courseID;
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

        currentArea = GetAreaInformation();
        areaData = await GetAreaData();
        LoadAreaScene();
    }

    #region GetAreaInformation
    /// <summary>
    ///     This function converts a given string to an <c>AreaInformation</c>
    /// </summary>
    /// <param name="area">The string to be converted</param>
    /// <returns>The converted <c>AreaInformation</c>, if valid, the default world 1 otherwise</returns>
    private AreaInformation GetAreaInformation()
    {
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
                return new AreaInformation(worldIndex, optionalDungeonIndex);
            }
            else
            {
                Debug.Log("Invalid gamemode provided: " + urlPart + ", loading world 1 instead");
                return new AreaInformation(1, new Optional<int>());
            }
        }
        else
        {
            Debug.Log("Invalid gamemode provided: " + urlPart + ", loading world 1 instead");
            return new AreaInformation(1, new Optional<int>(1));
        }     
    }

    #endregion

    #region GetAreaData
    /// <summary>
    ///     This function retrieves the needed data from the backend
    /// </summary>
    /// <returns></returns>
    private async UniTask<AreaData> GetAreaData()
    {
#if UNITY_EDITOR
        //use local files in editor
        Debug.Log("Load from local files");
        return LoadLocalData();
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
            return areaData;
        }
        else
        {
            return null;
            //TODO: error screen
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
            areaManager.SetupGenerator(courseID, areaData, currentArea, cameraController);
        }
        else if(gamemode == Gamemode.INSPECT)
        {
            areaManager.SetupInspector(courseID, areaData, currentArea, cameraController);
        }
    }
}
