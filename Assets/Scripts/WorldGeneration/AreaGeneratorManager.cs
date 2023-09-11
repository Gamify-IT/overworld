using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class AreaGeneratorManager : MonoBehaviour
{
    #region Attributes
    //UI
    [SerializeField] private CameraMovement cameraMovement;

    //Data
    private AreaInformation currentArea;
    private AreaData areaData;
    #endregion

    private void Awake()
    {
        currentArea = GetAreInformation();
        areaData = GetAreaData();



        LoadAreaScene();      
    }

    #region GetAreaInformation
    /// <summary>
    ///     This function converts a given string to an <c>AreaInformation</c>
    /// </summary>
    /// <param name="area">The string to be converted</param>
    /// <returns>The converted <c>AreaInformation</c>, if valid, the default world 1 otherwise</returns>
    private AreaInformation GetAreInformation()
    {
        Optional<string> areaToLoad = BrowserVariable.TryToReadVariable("GeneratorArea");
        if(areaToLoad.IsPresent())
        {
            string area = areaToLoad.Value();
            Optional<AreaInformation> areaInformation;
            if (area.Contains("-"))
            {
                areaInformation = ConvertDungeon(area);
            }
            else
            {
                areaInformation = ConvertWorld(area);
            }

            if (areaInformation.IsPresent())
            {
                return areaInformation.Value();
            }
            else
            {
                Debug.LogError("Inavlid area specified, loading world 1 instead");
                return new AreaInformation(1, new Optional<int>());
            }
        }
        else
        {            
            Debug.LogError("No area specified, loading world 1 instead");
            return new AreaInformation(1, new Optional<int>());
        }        
    }

    /// <summary>
    ///     This function tries to convert a string into a dungeon identifier
    /// </summary>
    /// <param name="area">the string to be converted</param>
    /// <returns>The converted dungeon, if valid, an empty <c>Optional</c> otherwise</returns>
    private Optional<AreaInformation> ConvertDungeon(string area)
    {
        Optional<AreaInformation> optionalAreaInformation = new Optional<AreaInformation>();
        string[] parts = area.Split("-");
        if (parts.Length == 2)
        {
            bool success = int.TryParse(parts[0], out int worldIndex);
            if (success)
            {
                AreaInformation areaInformation = new AreaInformation(worldIndex, new Optional<int>());
                success = int.TryParse(parts[1], out int dungeonIndex);
                if (success)
                {
                    areaInformation.SetDungeonIndex(dungeonIndex);
                    optionalAreaInformation.SetValue(areaInformation);
                }
            }
        }
        return optionalAreaInformation;
    }

    /// <summary>
    ///     This function tries to convert a string into a world identifier
    /// </summary>
    /// <param name="area">the string to be converted</param>
    /// <returns>The converted world, if valid, an empty <c>Optional</c> otherwise</returns>
    private Optional<AreaInformation> ConvertWorld(string area)
    {
        Optional<AreaInformation> optionalAreaInformation = new Optional<AreaInformation>();
        bool success = int.TryParse(area, out int worldIndex);
        if (success)
        {
            AreaInformation areaInformation = new AreaInformation(worldIndex, new Optional<int>());
            optionalAreaInformation.SetValue(areaInformation);
        }
        return optionalAreaInformation;
    }
    #endregion

    #region GetAreaData
    /// <summary>
    ///     This function retrieves the needed data from the backend
    /// </summary>
    /// <returns></returns>
    private AreaData GetAreaData()
    {
        //TODO: load data from backend

        //Workaround: use local json files
        return LoadLocalData();
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
            path = "Areas/Dungeon" + currentArea.GetWorldIndex() + "-" + currentArea.GetWorldIndex();            
        }
        else
        {
            path = "Areas/World" + currentArea.GetWorldIndex();
        }
        
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;
        AreaDTO areaDTO = AreaDTO.CreateFromJSON(json);
        AreaData areaData = AreaData.ConvertDtoToData(areaDTO);
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

        GeneratorManager generator = FindObjectOfType<GeneratorManager>();
        if (generator != null)
        {
            generator.Setup(areaData, cameraMovement);
        }
    }
}
