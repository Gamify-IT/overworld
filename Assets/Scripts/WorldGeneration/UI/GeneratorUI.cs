using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;

public class GeneratorUI : MonoBehaviour
{
    #region Attributes
    //WorldGenerator
    [SerializeField] private GeneratorUIManager uiManager;
    private AreaData areaData;
    private AreaInformation areaIdentifier;
    private AreaInformationData areaInformation;
    private AccessabilitySettings accessabilitySettings;

    [SerializeField] InterfaceInfo infoUI;

    //Area
    [SerializeField] private TextMeshProUGUI areaText;

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
    [SerializeField] private Slider accessabilitySlider;
    [SerializeField] private TMP_InputField seedInput;

    [SerializeField] private Button generateLayoutButton;
    [SerializeField] private Button continueButton;

    //Content
    [SerializeField] private Slider amountMinigamesSlider;
    [SerializeField] private TextMeshProUGUI maxMinigamesText;
    [SerializeField] private TextMeshProUGUI amountMinigamesText;
    [SerializeField] private Button generateMinigamesButton;
    [SerializeField] private Toggle minigamesToggle;

    [SerializeField] private Slider amountNpcsSlider;
    [SerializeField] private TextMeshProUGUI maxNpcsText;
    [SerializeField] private TextMeshProUGUI amountNpcsText;
    [SerializeField] private Button generateNpcsButton;
    [SerializeField] private Toggle npcsToggle;

    [SerializeField] private Slider amountBooksSlider;
    [SerializeField] private TextMeshProUGUI maxBooksText;
    [SerializeField] private TextMeshProUGUI amountBooksText;
    [SerializeField] private Button generateBooksButton;
    [SerializeField] private Toggle booksToggle;

    [SerializeField] private Slider amountTeleportersSlider;
    [SerializeField] private TextMeshProUGUI maxTeleportersText;
    [SerializeField] private TextMeshProUGUI amountTeleportersText;
    [SerializeField] private Button generateTeleporterButton;
    [SerializeField] private Toggle teleporterToggle;

    [SerializeField] private Slider amountDungeonsSlider;
    [SerializeField] private Image dungeonSliderKnob;
    [SerializeField] private Image dungeonSliderFill;
    [SerializeField] private TextMeshProUGUI maxDungeonsText;
    [SerializeField] private TextMeshProUGUI amountDungeonsText;
    [SerializeField] private Button generateDungeonsButton;
    [SerializeField] private Toggle dungeonsToggle;

    [SerializeField] private Button generateAllContentButton;
    [SerializeField] private Button saveAreaButton;

    #endregion

    #region Setup

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

        SetupPanels();

        if(areaIdentifier.IsDungeon())
        {
            areaText.text = "DUNGEON " + areaIdentifier.GetWorldIndex() + "-" + areaIdentifier.GetDungeonIndex();
            SetupDungeon();
        }
        else
        {
            areaText.text = "WORLD " + areaIdentifier.GetWorldIndex();
            SetupWorld();
        }

        SetupStyleDrowdown();

        SetupGeneratorTypeDrowdown();

        SetupAccessabilitySlider();       

        if(areaData.IsGeneratedArea())
        {
            seedInput.text = areaData.GetAreaMapData().GetLayout().GetSeed();
        }
        else
        {
            GenerateSeedButtonPressed();
        }

        SetupMaxValues();

        SetupObjectSliders();      
    }

    /// <summary>
    ///     This function sets the reference to the <c>GeneratorUIManager</c> component
    /// </summary>
    /// <param name="uiManager">The new reference</param>
    public void SetUIManager(GeneratorUIManager uiManager)
    {
        this.uiManager = uiManager;
    }

    /// <summary>
    ///     This function sets the initiale state of each panel
    /// </summary>
    private void SetupPanels()
    {
        smallGeneratorPanel.SetActive(false);
        areaSettings.SetActive(true);
        content.SetActive(false);
        generatorPanel.SetActive(true);
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
            sizeX.text = areaData.GetAreaMapData().GetLayout().GetTileSprites().GetLength(0).ToString();
            sizeY.text = areaData.GetAreaMapData().GetLayout().GetTileSprites().GetLength(1).ToString();
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
    ///     This function sets up the style dropdown menu
    /// </summary>
    private void SetupStyleDrowdown()
    {
        stypeDropdown.ClearOptions();
        List<string> styleOptions = System.Enum.GetNames(typeof(WorldStyle)).ToList();
        stypeDropdown.AddOptions(styleOptions);
        OnStyleChange();
    }

    /// <summary>
    ///     This function sets up the generator type dropdown menu
    /// </summary>
    private void SetupGeneratorTypeDrowdown()
    {
        generatorTypeDropdown.ClearOptions();
        List<string> generatorTypeOptions = System.Enum.GetNames(typeof(LayoutGeneratorType)).ToList();
        List<string> formattedOptions = new List<string>();

        foreach (string style in generatorTypeOptions)
        {
            formattedOptions.Add(style.Replace("_", " "));
        }
        generatorTypeDropdown.AddOptions(formattedOptions);

        if (areaData.IsGeneratedArea())
        {
            generatorTypeDropdown.value = (int)areaData.GetAreaMapData().GetLayout().GetGeneratorType();
        }
    }

    /// <summary>
    ///     This function sets up the accessability slider menu
    /// </summary>
    private void SetupAccessabilitySlider()
    {
        accessabilitySlider.minValue = 0;
        accessabilitySlider.maxValue = Enum.GetNames(typeof(Accessability)).Length - 1;
        accessabilitySettings = GetAccessabilitySettings();
        if (areaData.IsGeneratedArea())
        {
            int accessability = areaData.GetAreaMapData().GetLayout().GetAccessability();
            switch (areaData.GetAreaMapData().GetLayout().GetGeneratorType())
            {
                case LayoutGeneratorType.CELLULAR_AUTOMATA:
                    if (accessability == accessabilitySettings.verySmallCA)
                    {
                        accessabilitySlider.value = (int)Accessability.VERY_SMALL;
                    }
                    else if (accessability == accessabilitySettings.smallCA)
                    {
                        accessabilitySlider.value = (int)Accessability.SMALL;
                    }
                    else if (accessability == accessabilitySettings.normalCA)
                    {
                        accessabilitySlider.value = (int)Accessability.NORMAL;
                    }
                    else if (accessability == accessabilitySettings.bigCA)
                    {
                        accessabilitySlider.value = (int)Accessability.BIG;
                    }
                    else if (accessability == accessabilitySettings.veryBigCA)
                    {
                        accessabilitySlider.value = (int)Accessability.VERY_BIG;
                    }
                    break;

                case LayoutGeneratorType.DRUNKARDS_WALK:
                    if (accessability == accessabilitySettings.verySmallDW)
                    {
                        accessabilitySlider.value = (int)Accessability.VERY_SMALL;
                    }
                    else if (accessability == accessabilitySettings.smallDW)
                    {
                        accessabilitySlider.value = (int)Accessability.SMALL;
                    }
                    else if (accessability == accessabilitySettings.normalDW)
                    {
                        accessabilitySlider.value = (int)Accessability.NORMAL;
                    }
                    else if (accessability == accessabilitySettings.bigDW)
                    {
                        accessabilitySlider.value = (int)Accessability.BIG;
                    }
                    else if (accessability == accessabilitySettings.veryBigDW)
                    {
                        accessabilitySlider.value = (int)Accessability.VERY_BIG;
                    }
                    break;

                case LayoutGeneratorType.ISLAND_CELLULAR_AUTOMATA:
                    if (accessability == accessabilitySettings.verySmallIslandCA)
                    {
                        accessabilitySlider.value = (int)Accessability.VERY_SMALL;
                    }
                    else if (accessability == accessabilitySettings.smallIslandCA)
                    {
                        accessabilitySlider.value = (int)Accessability.SMALL;
                    }
                    else if (accessability == accessabilitySettings.normalIslandCA)
                    {
                        accessabilitySlider.value = (int)Accessability.NORMAL;
                    }
                    else if (accessability == accessabilitySettings.bigIslandCA)
                    {
                        accessabilitySlider.value = (int)Accessability.BIG;
                    }
                    else if (accessability == accessabilitySettings.veryBigIslandCA)
                    {
                        accessabilitySlider.value = (int)Accessability.VERY_BIG;
                    }
                    break;

                case LayoutGeneratorType.ISLAND_DRUNKARDS_WALK:
                    if (accessability == accessabilitySettings.verySmallIslandDW)
                    {
                        accessabilitySlider.value = (int)Accessability.VERY_SMALL;
                    }
                    else if (accessability == accessabilitySettings.smallIslandDW)
                    {
                        accessabilitySlider.value = (int)Accessability.SMALL;
                    }
                    else if (accessability == accessabilitySettings.normalIslandDW)
                    {
                        accessabilitySlider.value = (int)Accessability.NORMAL;
                    }
                    else if (accessability == accessabilitySettings.bigIslandDW)
                    {
                        accessabilitySlider.value = (int)Accessability.BIG;
                    }
                    else if (accessability == accessabilitySettings.veryBigIslandDW)
                    {
                        accessabilitySlider.value = (int)Accessability.VERY_BIG;
                    }
                    break;
            }
        }
        else
        {
            accessabilitySlider.value = (int)Accessability.NORMAL;
        }
    }

    /// <summary>
    ///     This function fetches the accessability settings from the local json file
    /// </summary>
    /// <returns>The accessability Settings</returns>
    private AccessabilitySettings GetAccessabilitySettings()
    {
        string path = "GameSettings/AccessabilitySettings";
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;
        AccessabilitySettings accessabilitySettings = AccessabilitySettings.CreateFromJSON(json);
        return accessabilitySettings;
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

    /// <summary>
    ///     This function sets up the object slider and texts
    /// </summary>
    private void SetupObjectSliders()
    {
        if (areaData.IsGeneratedArea())
        {
            stypeDropdown.value = (int)areaData.GetAreaMapData().GetLayout().GetStyle();

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
            stypeDropdown.value = (int)WorldStyle.CUSTOM;

            continueButton.interactable = false;

            amountMinigamesText.text = "";
            amountNpcsText.text = "";
            amountBooksText.text = "";
            amountTeleportersText.text = "";
            amountDungeonsText.text = "";
        }
    }

    #endregion

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
        ResetArea();        
    }

    private async void ResetArea()
    {
        infoUI.DisplayInfo("RESETING AREA...", "RESETING THE AREA TO DEFAULT ...", false, false);

        bool success = await uiManager.ResetToDefault();

        if(success)
        {
            stypeDropdown.value = 0;
            continueButton.interactable = false;
            infoUI.DisplayInfo("AREA RESET", "RESET THE CURRENT AREA TO DEFAULT", true, false);
        }
        else
        {
            infoUI.DisplayInfo("RESET", "COULD NOT RESET THE CURRENT AREA TO DEFAULT", true, false);
        }
    }

    public void GenerateLayoutButtonPressed()
    {
        StartCoroutine(GenerateLayoutCoroutine());
    }

    private IEnumerator GenerateLayoutCoroutine()
    {
        infoUI.DisplayInfo("GENERATING LAYOUT...", "", false, false);

        yield return null;

        GenerateLayout();
    }

    private void GenerateLayout()
    {
        Vector2Int size = new Vector2Int(int.Parse(sizeX.text), int.Parse(sizeY.text));
        WorldStyle style = (WorldStyle)stypeDropdown.value;
        LayoutGeneratorType layoutGeneratorType = (LayoutGeneratorType)generatorTypeDropdown.value;
        int accessability = ParseAccessability(layoutGeneratorType, size);
        string seed = seedInput.text;

        uiManager.GenerateLayout(size, style, layoutGeneratorType, accessability, seed);

        continueButton.interactable = true;

        ResetContentPanel();

        infoUI.DisplayInfo("LAYOUT GENERATED", "SUCCESSFULLY GENERATED A LAYOUT", true, true);
    }

    //get correct accessability value for selected generator type
    private int ParseAccessability(LayoutGeneratorType layoutGeneratorType, Vector2Int size)
    {
        switch(layoutGeneratorType)
        {
            case LayoutGeneratorType.CELLULAR_AUTOMATA:
                return ParseCellularAutomataAccessability();

            case LayoutGeneratorType.DRUNKARDS_WALK:
                return ParseDrunkardsWalkAccessability(size);

            case LayoutGeneratorType.ISLAND_CELLULAR_AUTOMATA:
                return ParseIslandsCAAccessability();

            case LayoutGeneratorType.ISLAND_DRUNKARDS_WALK:
                return ParseIslandsDWAccessability();

            default:
                return 0;
        }
    }

    //get correct accessability value for CA
    private int ParseCellularAutomataAccessability()
    {
        Accessability accessability = (Accessability) accessabilitySlider.value;

        switch(accessability)
        {
            case Accessability.VERY_SMALL:
                return accessabilitySettings.verySmallCA;

            case Accessability.SMALL:
                return accessabilitySettings.smallCA;

            case Accessability.NORMAL:
                return accessabilitySettings.normalCA;

            case Accessability.BIG:
                return accessabilitySettings.bigCA;

            case Accessability.VERY_BIG:
                return accessabilitySettings.veryBigCA;
        }

        return 0;
    }

    //get correct accessability value for DW
    private int ParseDrunkardsWalkAccessability(Vector2Int size)
    {
        Accessability accessability = (Accessability)accessabilitySlider.value;

        float xFactor = (size.x / 100f);
        float yFactor = (size.y / 100f);

        float sizeFactor = 0.5f * (xFactor + yFactor);
        Debug.Log("Size factor: " + sizeFactor);

        switch (accessability)
        {
            case Accessability.VERY_SMALL:
                return Mathf.RoundToInt(accessabilitySettings.verySmallDW * sizeFactor);

            case Accessability.SMALL:
                return Mathf.RoundToInt(accessabilitySettings.smallDW * sizeFactor);

            case Accessability.NORMAL:
                return Mathf.RoundToInt(accessabilitySettings.normalDW * sizeFactor);

            case Accessability.BIG:
                return Mathf.RoundToInt(accessabilitySettings.bigDW * sizeFactor);

            case Accessability.VERY_BIG:
                return Mathf.RoundToInt(accessabilitySettings.veryBigDW * sizeFactor);
        }

        return 0;
    }

    //get correct accessability value for Islands-CA
    private int ParseIslandsCAAccessability()
    {
        Accessability accessability = (Accessability)accessabilitySlider.value;

        switch (accessability)
        {
            case Accessability.VERY_SMALL:
                return accessabilitySettings.verySmallIslandCA;

            case Accessability.SMALL:
                return accessabilitySettings.smallIslandCA;

            case Accessability.NORMAL:
                return accessabilitySettings.normalIslandCA;

            case Accessability.BIG:
                return accessabilitySettings.bigIslandCA;

            case Accessability.VERY_BIG:
                return accessabilitySettings.veryBigIslandCA;
        }

        return 0;
    }

    //get correct accessability value for Islands-DW
    private int ParseIslandsDWAccessability()
    {
        Accessability accessability = (Accessability)accessabilitySlider.value;

        switch (accessability)
        {
            case Accessability.VERY_SMALL:
                return accessabilitySettings.verySmallIslandDW;

            case Accessability.SMALL:
                return accessabilitySettings.smallIslandDW;

            case Accessability.NORMAL:
                return accessabilitySettings.normalIslandDW;

            case Accessability.BIG:
                return accessabilitySettings.bigIslandDW;

            case Accessability.VERY_BIG:
                return accessabilitySettings.veryBigIslandDW;
        }

        return 0;
    }

    /// <summary>
    ///     This function resets the content panel
    /// </summary>
    private void ResetContentPanel()
    {
        amountMinigamesSlider.value = 0;
        OnMinigameAmountChange();

        amountNpcsSlider.value = 0;
        OnNpcAmountChange();

        amountBooksSlider.value = 0;
        OnBookAmountChange();

        amountTeleportersSlider.value = 0;
        OnTeleporterAmountChange();

        amountDungeonsSlider.value = 0;
        OnDungeonsAmountChange();
    }

    public void ContinueButtonPressed()
    {
        if(areaIdentifier.IsDungeon())
        {
            amountDungeonsSlider.maxValue = 1;
            amountDungeonsSlider.direction = Slider.Direction.RightToLeft;
            dungeonSliderKnob.color = new Color32(150, 150, 150, 255);
            dungeonSliderFill.color = new Color32(150, 150, 150, 255);
            amountDungeonsSlider.interactable = false;

            amountDungeonsText.text = "1";
            maxDungeonsText.text = "1";
        }

        AddWorldConnectionBarriers();
        CheckSaveWorldButtonStatus();

        areaSettings.SetActive(false);
        content.SetActive(true);
    }

    //try to add world connection barriers, if not already set
    private void AddWorldConnectionBarriers()
    {
        uiManager.AddWorldConnectionBarriers();
    }
    #endregion

    #region Content Buttons

    #region Generation Buttons
    public void GenerateMinigamesButtonPressed()
    {
        StartCoroutine(GenerateMinigamesCoroutine());
    }

    private IEnumerator GenerateMinigamesCoroutine()
    {
        int amountMinigames = (int)amountMinigamesSlider.value;
        if(amountMinigames > 0)
        {
            infoUI.DisplayInfo("GENERATING CONTENT ...", "TRYING TO GENERATE " + amountMinigames + " MINIGAMES", false, false);
        }
        else
        {
            infoUI.DisplayInfo("REMOVING ALL MINIGAMES ...", "", false, false);
        }        

        yield return null;

        bool success = GenerateMinigames();
        CheckSaveWorldButtonStatus();

        string feedback;
        if (amountMinigames > 0)
        {
            feedback = "CREATED: \n" + amountMinigames + " MINIGAME SPOTS";
        }
        else
        {
            feedback = "REMOVED ALL MINIGAME SPOTS";
        }

        DisplayGenerationFeedback(success, feedback);
    }

    private bool GenerateMinigames()
    {
        int amountMinigames = (int)amountMinigamesSlider.value;
        return uiManager.GenerateMinigames(amountMinigames);
    }

    public void GenerateNpcsButtonPressed()
    {
        StartCoroutine(GenerateNpcsCoroutine());
    }

    private IEnumerator GenerateNpcsCoroutine()
    {
        int amountNpcs = (int)amountNpcsSlider.value;
        if (amountNpcs > 0)
        {
            infoUI.DisplayInfo("GENERATING CONTENT ...", "TRYING TO GENERATE " + amountNpcs + " NPCS", false, false);
        }
        else
        {
            infoUI.DisplayInfo("REMOVING ALL NPCS ...", "", false, false);
        }

        yield return null;

        bool success = GenerateNpcs();

        string feedback;
        if (amountNpcs > 0)
        {
            feedback = "CREATED: \n" + amountNpcs + " NPC SPOTS";
        }
        else
        {
            feedback = "REMOVED ALL NPC SPOTS";
        }

        DisplayGenerationFeedback(success, feedback);
    }

    private bool GenerateNpcs()
    {
        int amountNpcs = (int)amountNpcsSlider.value;
        return uiManager.GenerateNpcs(amountNpcs);
    }

    public void GenerateBooksButtonPressed()
    {
        StartCoroutine(GenerateBooksCoroutine());
    }

    private IEnumerator GenerateBooksCoroutine()
    {
        int amountBooks = (int)amountBooksSlider.value;
        if (amountBooks > 0)
        {
            infoUI.DisplayInfo("GENERATING CONTENT ...", "TRYING TO GENERATE " + amountBooks + " BOOKS", false, false);
        }
        else
        {
            infoUI.DisplayInfo("REMOVING ALL BOOKS ...", "", false, false);
        }

        yield return null;

        bool success = GenerateBooks();

        string feedback;
        if (amountBooks > 0)
        {
            feedback = "CREATED: \n" + amountBooks + " BOOK SPOTS";
        }
        else
        {
            feedback = "REMOVED ALL BOOK SPOTS";
        }

        DisplayGenerationFeedback(success, feedback);
    }

    private bool GenerateBooks()
    {
        int amountBooks = (int)amountBooksSlider.value;
        return uiManager.GenerateBooks(amountBooks);
    }

    public void GenerateTeleporterButtonPressed()
    {
        StartCoroutine(GenerateTeleporterCoroutine());
    }

    private IEnumerator GenerateTeleporterCoroutine()
    {
        int amountTeleporter = (int)amountTeleportersSlider.value;
        if (amountTeleporter > 0)
        {
            infoUI.DisplayInfo("GENERATING CONTENT ...", "TRYING TO GENERATE " + amountTeleporter + " TELEPORTER", false, false);
        }
        else
        {
            infoUI.DisplayInfo("REMOVING ALL TELEPORTER ...", "", false, false);
        }

        yield return null;

        bool success = GenerateTeleporter();

        string feedback;
        if (amountTeleporter > 0)
        {
            feedback = "CREATED: \n" + amountTeleporter + " TELEPORTER SPOTS";
        }
        else
        {
            feedback = "REMOVED ALL TELEPORTER SPOTS";
        }

        DisplayGenerationFeedback(success, feedback);
    }

    private bool GenerateTeleporter()
    {
        int amountTeleporters = (int)amountTeleportersSlider.value;
        return uiManager.GenerateTeleporters(amountTeleporters);
    }

    public void GenerateDungeonsButtonPressed()
    {
        StartCoroutine(GenerateDungeonsCoroutine());
    }

    private IEnumerator GenerateDungeonsCoroutine()
    {
        int amountDungeons = (int)amountDungeonsSlider.value;
        if (amountDungeons > 0)
        {
            infoUI.DisplayInfo("GENERATING CONTENT ...", "TRYING TO GENERATE " + amountDungeons + " DUNGEONS", false, false);
        }
        else
        {
            infoUI.DisplayInfo("REMOVING ALL DUNGEONS ...", "", false, false);
        }

        yield return null;

        bool success = GenerateDungeons();
        CheckSaveWorldButtonStatus();

        string feedback;
        if (amountDungeons > 0)
        {
            feedback = "CREATED: \n" + amountDungeons + " DUNGEONS SPOTS";
        }
        else
        {
            feedback = "REMOVED ALL DUNGEONS SPOTS";
        }

        DisplayGenerationFeedback(success, feedback);
    }

    private bool GenerateDungeons()
    {
        int amountDungeons = (int)amountDungeonsSlider.value;
        return uiManager.GenerateDungeons(amountDungeons);
    }

    private void DisplayGenerationFeedback(bool success, string feedbackText)
    {
        string header;
        string content;

        if (success)
        {
            header = "GENERATION SUCCESSFUL";
            content = feedbackText;
        }
        else
        {
            header = "GENERATION FAILED";
            content = "COULD NOT CREATE ALL OBJECTS, \n \n PLEASE TRY AGAIN, \n CHANGE THE AMOUNT OF OBJECTS, \n OR \n CREATE ANOTHER LAYOUT";
        }
        infoUI.DisplayInfo(header, content, true, true);
    }
    #endregion

    public void ReturnButtonPressed()
    {
        content.SetActive(false);
        areaSettings.SetActive(true);
    }

    public void GenerateAllContentButtonPressed()
    {
        StartCoroutine(GenerateAllContentCoroutine());
    }

    private IEnumerator GenerateAllContentCoroutine()
    {
        string header = "GENERATING CONTENT ...";
        string contentHeader = "CREATING";
        string content = "";

        int amountMinigames = (int)amountMinigamesSlider.value;
        int amountNpcs = (int)amountNpcsSlider.value;
        int amountBooks = (int)amountBooksSlider.value;
        int amountTeleporter = (int)amountTeleportersSlider.value;
        int amountDungeons = (int)amountDungeonsSlider.value;

        if (amountMinigames == 0 &&
            amountNpcs == 0 &&
            amountBooks == 0 &&
            amountTeleporter == 0 &&
            amountDungeons == 0)
        {
            header = "RESETING ALL CONTENT ...";
            contentHeader = "";
            content = "";
        }
        else
        {
            if (amountMinigames > 0)
            {
                content += "\n " + (int)amountMinigamesSlider.value + " MINIGAME SPOTS";
            }
            if (amountNpcs > 0)
            {
                content += "\n " + (int)amountNpcsSlider.value + " NPC SPOTS";
            }
            if (amountBooks > 0)
            {
                content += "\n " + (int)amountBooksSlider.value + " BOOK SPOTS";
            }
            if (amountTeleporter > 0)
            {
                content += "\n " + (int)amountTeleportersSlider.value + " TELEPORTER SPOTS";
            }
            if (amountDungeons > 0)
            {
                content += "\n " + (int)amountDungeonsSlider.value + " DUNGEON SPOTS";
            }
        }

        infoUI.DisplayInfo(header, contentHeader + content, false, false);

        yield return null;

        Tuple<bool, string> result = GenerateAllContent();
        bool success = result.Item1;
        CheckSaveWorldButtonStatus();

        if (success)
        {
            header = "GENERATION SUCCESSFUL";
            if (amountMinigames == 0 &&
                amountNpcs == 0 &&
                amountBooks == 0 &&
                amountTeleporter == 0 &&
                amountDungeons == 0)
            {
                contentHeader = "REMOVED ALL CONTENT";
                content = "";
            }
            else
            {
                contentHeader = "CREATED";
            }
        }
        else
        {
            header = "GENERATION FAILED";
            contentHeader = result.Item2;
            content = "";
        }

        infoUI.DisplayInfo(header, contentHeader + content, true, success);
    }

    private Tuple<bool, string> GenerateAllContent()
    {
        uiManager.ResetObjects();
        CheckSaveWorldButtonStatus();

        if (!GenerateDungeons())
        {
            //could not create dungeon spots -> reset all and stop 
            uiManager.ResetObjects();
            return new Tuple<bool, string>(false, "COULD NOT CREATE ALL DUNGEONS");
        }

        if (!GenerateMinigames())
        {
            //could not create minigame spots -> reset all and stop 
            uiManager.ResetObjects();
            return new Tuple<bool, string>(false, "COULD NOT CREATE ALL MINIGAMES");
        }

        if (!GenerateNpcs())
        {
            //could not create npc spots -> reset all and stop 
            uiManager.ResetObjects();
            return new Tuple<bool, string>(false, "COULD NOT CREATE ALL NPCS");
        }

        if (!GenerateBooks())
        {
            //could not create book spots -> reset all and stop 
            uiManager.ResetObjects();
            return new Tuple<bool, string>(false, "COULD NOT CREATE ALL BOOKS");
        }

        if (!GenerateTeleporter())
        {
            //could not create teleporter spots -> reset all and stop 
            uiManager.ResetObjects();
            return new Tuple<bool, string>(false, "COULD NOT CREATE ALL TELEPORTER");
        }

        //all spots created
        return new Tuple<bool, string>(true, ""); ;
    }

    public void SaveAreaButtonPressed()
    {
        SaveArea();
    }

    private async void SaveArea()
    {
        infoUI.DisplayInfo("SAVING...", "SAVING THE CURRENT AREA...", false, false);

        bool success = await uiManager.SaveArea();

        if(success)
        {
            infoUI.DisplayInfo("AREA SAVED", "SAVED THE CURRENT AREA", true, false);
        }
        else
        {
            infoUI.DisplayInfo("AREA NOT SAVED", "COULD NOT SAVE THE CURRENT AREA", true, false);
        }        
    }
    #endregion

    #region Content Toggle

    public void ToggleMinigames()
    {
        uiManager.DisplayMinigames(minigamesToggle.isOn);
    }

    public void ToggleNpcs()
    {
        uiManager.DisplayNpcs(npcsToggle.isOn);
    }

    public void ToggleBooks()
    {
        uiManager.DisplayBooks(booksToggle.isOn);
    }

    public void ToggleTeleporter()
    {
        uiManager.DisplayTeleporter(teleporterToggle.isOn);
    }

    public void ToggleDungeons()
    {
        uiManager.DisplayDungeons(dungeonsToggle.isOn);
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
