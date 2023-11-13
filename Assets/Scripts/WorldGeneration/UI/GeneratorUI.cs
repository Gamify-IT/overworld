using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class GeneratorUI : MonoBehaviour
{
    #region Attributes
    //WorldGenerator
    private GeneratorUIManager uiManager;
    private AreaData areaData;
    private AreaInformation areaIdentifier;
    private AreaInformationData areaInformation;

    //Panels
    [SerializeField] private GameObject generatorPanel;
    [SerializeField] private GameObject smallGeneratorPanel;
    [SerializeField] private GameObject areaSettings;
    [SerializeField] private GameObject content;

    //Area Settings
    [SerializeField] private TMP_InputField sizeX;    
    [SerializeField] private TMP_InputField sizeY;
    [SerializeField] private TMP_Dropdown stypeDropdown;
    [SerializeField] private TMP_Dropdown generatorTypeDropdown;
    [SerializeField] private TMP_InputField accessabilityInput;
    [SerializeField] private TMP_InputField seedInput;

    [SerializeField] private Button generateLayoutButton;
    [SerializeField] private Button continueButton;

    //Content
    [SerializeField] private Slider amountMinigamesSlider;
    [SerializeField] private TextMeshProUGUI maxMinigamesText;
    [SerializeField] private TextMeshProUGUI amountMinigamesText;
    [SerializeField] private Button generateMinigamesButton;

    [SerializeField] private Slider amountNpcsSlider;
    [SerializeField] private TextMeshProUGUI maxNpcsText;
    [SerializeField] private TextMeshProUGUI amountNpcsText;
    [SerializeField] private Button generateNpcsButton;

    [SerializeField] private Slider amountBooksSlider;
    [SerializeField] private TextMeshProUGUI maxBooksText;
    [SerializeField] private TextMeshProUGUI amountBooksText;
    [SerializeField] private Button generateBooksButton;

    [SerializeField] private Slider amountTeleportersSlider;
    [SerializeField] private TextMeshProUGUI maxTeleportersText;
    [SerializeField] private TextMeshProUGUI amountTeleportersText;
    [SerializeField] private Button generateTeleporterButton;

    [SerializeField] private Slider amountDungeonsSlider;
    [SerializeField] private TextMeshProUGUI maxDungeonsText;
    [SerializeField] private TextMeshProUGUI amountDungeonsText;
    [SerializeField] private Button generateDungeonsButton;

    [SerializeField] private Button generateAllContentButton;
    [SerializeField] private Button saveAreaButton;
    #endregion

    /// <summary>
    ///     This function sets up the generator UI with the given values
    /// </summary>
    /// <param name="uiManager">The generator object</param>
    /// <param name="areaData">The data of the current area</param>
    /// <param name="areaInformation">Additional parameters for area generation</param>
    public void Setup(GeneratorUIManager uiManager, AreaData areaData, AreaInformationData areaInformation)
    {
        this.uiManager = uiManager;        
        this.areaInformation = areaInformation;
        this.areaData = areaData;
        areaIdentifier = areaData.GetArea();

        Debug.Log("CurrentArea: " + areaIdentifier.GetWorldIndex() + "-" + areaIdentifier.GetDungeonIndex());

        smallGeneratorPanel.SetActive(false);
        areaSettings.SetActive(true);
        content.SetActive(false);
        generatorPanel.SetActive(true);

        if(areaIdentifier.IsDungeon())
        {
            SetupDungeon();
        }
        else
        {
            SetupWorld();
        }

        stypeDropdown.ClearOptions();
        List<string> options = System.Enum.GetNames(typeof(WorldStyle)).ToList();
        stypeDropdown.AddOptions(options);

        accessabilityInput.text = "50";

        if(areaData.IsGeneratedArea())
        {
            seedInput.text = areaData.GetAreaMapData().GetLayout().GetSeed();
        }
        else
        {
            GenerateSeedButtonPressed();
        }

        SetupMaxValues();

        if(areaData.IsGeneratedArea())
        {
            stypeDropdown.value = (int) areaData.GetAreaMapData().GetLayout().GetStyle();

            continueButton.interactable = true;

            amountMinigamesText.text = areaData.GetAreaMapData().GetMinigameSpots().Count.ToString();
            amountMinigamesSlider.value = areaData.GetAreaMapData().GetMinigameSpots().Count;

            amountNpcsText.text = areaData.GetAreaMapData().GetNpcSpots().Count.ToString();
            amountNpcsSlider.value = areaData.GetAreaMapData().GetNpcSpots().Count;

            amountBooksText.text = areaData.GetAreaMapData().GetBookSpots().Count.ToString();
            amountBooksSlider.value = areaData.GetAreaMapData().GetBookSpots().Count;

            amountTeleportersText.text = areaData.GetAreaMapData().GetTeleporterSpots().Count.ToString();
            amountTeleportersSlider.value = areaData.GetAreaMapData().GetTeleporterSpots().Count;

            amountDungeonsText.text = areaData.GetAreaMapData().GetTeleporterSpots().Count.ToString();
            amountDungeonsSlider.value = areaData.GetAreaMapData().GetSceneTransitionSpots().Count;
        }
        else
        {
            stypeDropdown.value = (int) WorldStyle.CUSTOM;

            continueButton.interactable = false;

            amountMinigamesText.text = "";
            amountNpcsText.text = "";
            amountBooksText.text = "";
            amountTeleportersText.text = "";
            amountDungeonsText.text = "";
        }
    }

    /// <summary>
    ///     This function sets up the size and offset input fields for a world
    /// </summary>
    private void SetupWorld()
    {
        sizeX.text = areaInformation.GetSize().x.ToString();
        sizeY.text = areaInformation.GetSize().y.ToString();

        sizeX.enabled = false;
        sizeY.enabled = false;
    }

    /// <summary>
    ///     This function sets up the size and offset input fields for a dungeon
    /// </summary>
    private void SetupDungeon()
    {
        if(areaData.IsGeneratedArea())
        {
            sizeX.text = areaData.GetAreaMapData().GetLayout().GetTiles().GetLength(0).ToString();
            sizeY.text = areaData.GetAreaMapData().GetLayout().GetTiles().GetLength(1).ToString();
        }
        else
        {
            sizeX.text = areaInformation.GetSize().x.ToString();
            sizeY.text = areaInformation.GetSize().y.ToString();
        }        

        sizeX.enabled = true;
        sizeY.enabled = true;

        amountDungeonsSlider.enabled = false;
    }

    /// <summary>
    ///     This function sets the max values for all objects
    /// </summary>
    private void SetupMaxValues()
    {
        int maxMinigames = GameSettings.GetMaxMinigames();
        amountMinigamesSlider.maxValue = maxMinigames;
        maxMinigamesText.text = maxMinigames.ToString();

        int maxNpcs = GameSettings.GetMaxNpcs();
        amountNpcsSlider.maxValue = maxNpcs;
        maxNpcsText.text = maxNpcs.ToString();

        int maxBooks = GameSettings.GetMaxBooks();
        amountBooksSlider.maxValue = maxBooks;
        maxBooksText.text = maxBooks.ToString();

        int maxTeleporter = GameSettings.GetMaxTeleporters();
        amountTeleportersSlider.maxValue = maxTeleporter;
        maxTeleportersText.text = maxTeleporter.ToString();

        int maxDungeons = GameSettings.GetMaxDungeons();
        amountDungeonsSlider.maxValue = maxDungeons;
        maxDungeonsText.text = maxDungeons.ToString();
    }

    public void MinimizeButtonPressed()
    {
        generatorPanel.SetActive(false);
        smallGeneratorPanel.SetActive(true);
        uiManager.ActivateCameraMovement();
    }

    public void MaximizeButtonPressed()
    {
        smallGeneratorPanel.SetActive(false);
        generatorPanel.SetActive(true);
        uiManager.DeactivateCameraMovement();
    }

    #region Area Settings Buttons
    public void GenerateSeedButtonPressed()
    {
        string seed = Time.time.ToString();
        seedInput.text = seed;
    }

    public void ResetToCustomButtonPressed()
    {
        uiManager.ResetToDefault();
        stypeDropdown.value = 0;
        continueButton.interactable = false;
    }

    public void GenerateLayoutButtonPressed()
    {
        Vector2Int size = new Vector2Int(int.Parse(sizeX.text), int.Parse(sizeY.text));
        WorldStyle style = (WorldStyle) stypeDropdown.value;
        LayoutGeneratorType layoutGeneratorType = (LayoutGeneratorType) generatorTypeDropdown.value;
        int accessability = int.Parse(accessabilityInput.text);
        string seed = seedInput.text;

        uiManager.GenerateLayout(size, style, layoutGeneratorType, accessability, seed);

        continueButton.interactable = true;

        ResetContentPanel();
    }

    /// <summary>
    ///     This function resets the content panel
    /// </summary>
    private void ResetContentPanel()
    {
        amountMinigamesText.text = "0";
        OnMinigameAmountChange();
        amountNpcsText.text = "0";
        amountBooksText.text = "0";
        amountTeleportersText.text = "0";
        amountDungeonsText.text = "0";
    }

    public void ContinueButtonPressed()
    {
        if(areaIdentifier.IsDungeon())
        {
            amountDungeonsSlider.maxValue = 1;
            amountDungeonsSlider.value = 1;
            amountDungeonsText.text = "1";
            maxDungeonsText.text = "1";
            GenerateDungeonsButtonPressed();
        }        

        areaSettings.SetActive(false);
        content.SetActive(true);
    }
    #endregion

    #region Content Buttons

    #region Generation Buttons
    public void GenerateMinigamesButtonPressed()
    {
        int amountMinigames = (int) amountMinigamesSlider.value;
        uiManager.GenerateMinigames(amountMinigames);
        CheckSaveWorldButtonStatus();
    }

    public void GenerateNpcsButtonPressed()
    {
        int amountNpcs = (int) amountNpcsSlider.value;
        uiManager.GenerateNpcs(amountNpcs);
    }

    public void GenerateBooksButtonPressed()
    {
        int amountBooks = (int) amountBooksSlider.value;
        uiManager.GenerateBooks(amountBooks);
    }

    public void GenerateTeleporterButtonPressed()
    {
        int amountTeleporters = (int) amountTeleportersSlider.value;
        uiManager.GenerateTeleporters(amountTeleporters);
    }

    public void GenerateDungeonsButtonPressed()
    {
        int amountDungeons = (int) amountDungeonsSlider.value;
        uiManager.GenerateDungeons(amountDungeons);
    }
    #endregion

    public void ReturnButtonPressed()
    {
        content.SetActive(false);
        areaSettings.SetActive(true);
    }

    public void GenerateAllContentButtonPressed()
    {
        uiManager.ResetObjects();
        GenerateMinigamesButtonPressed();
        GenerateNpcsButtonPressed();
        GenerateBooksButtonPressed();
        GenerateTeleporterButtonPressed();
        GenerateDungeonsButtonPressed();
    }

    public void SaveAreaButtonPressed()
    {
        uiManager.SaveArea();
    }
    #endregion

    #region ButtonStatusManagement
    /// <summary>
    ///     This function is called when the <c>Style Dropdown</c> value is changed and sets the <c>Generate Layout</c> button active or inactive,
    ///     based on the selected value
    /// </summary>
    public void OnStyleChange()
    {
        WorldStyle style = (WorldStyle)stypeDropdown.value;
        if (style == WorldStyle.CUSTOM)
        {
            generateLayoutButton.interactable = false;
        }
        else
        {
            generateLayoutButton.interactable = true;
        }
    }

    /// <summary>
    ///     This function is called when the value of the <c>Amount of Minigames</c> slider is changed and sets the amount text to this value and the <c>Generate Minigames</c>
    ///     button active or inactive, based on the entered value
    /// </summary>
    public void OnMinigameAmountChange()
    {
        int amountMinigames = (int) amountMinigamesSlider.value;
        amountMinigamesText.text = amountMinigames.ToString();
    }

    /// <summary>
    ///     This function is called when the value of the <c>Amount of NPCs</c> slider is changed and sets the amount text to this value
    /// </summary>
    public void OnNpcAmountChange()
    {
        int amountNpcs = (int) amountNpcsSlider.value;
        amountNpcsText.text = amountNpcs.ToString();
    }

    /// <summary>
    ///     This function is called when the value of the <c>Amount of Books</c> slider is changed and sets the amount text to this value
    /// </summary>
    public void OnBookAmountChange()
    {
        int amountBooks = (int)amountBooksSlider.value;
        amountBooksText.text = amountBooks.ToString();
    }

    /// <summary>
    ///     This function is called when the value of the <c>Amount of Teleporter</c> slider is changed and sets the amount text to this value
    /// </summary>
    public void OnTeleporterAmountChange()
    {
        int amountTeleporters = (int)amountTeleportersSlider.value;
        amountTeleportersText.text = amountTeleporters.ToString();
    }

    /// <summary>
    ///     This function is called when the value of the <c>Amount of Dungeons</c> slider is changed and sets the amount text to this value
    /// </summary>
    public void OnDungeonsAmountChange()
    {
        int amountDungeons = (int)amountDungeonsSlider.value;
        amountDungeonsText.text = amountDungeons.ToString();
    }

    /// <summary>
    ///     This function is called, when the amount of Minigames is changed and sets the <c>Save Area</c>
    ///     button active, if at least one minigame exists, or inactive, otherwise
    /// </summary>
    private void CheckSaveWorldButtonStatus()
    {
        bool validArea = uiManager.IsAreaSaveable();
        saveAreaButton.interactable = validArea;
    }
    #endregion
}
