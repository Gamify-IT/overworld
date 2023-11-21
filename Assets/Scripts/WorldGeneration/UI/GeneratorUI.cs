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
    [SerializeField] private GameObject feedbackPanel;

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
    [SerializeField] private TextMeshProUGUI maxDungeonsText;
    [SerializeField] private TextMeshProUGUI amountDungeonsText;
    [SerializeField] private Button generateDungeonsButton;
    [SerializeField] private Toggle dungeonsToggle;

    [SerializeField] private Button generateAllContentButton;
    [SerializeField] private Button saveAreaButton;

    //Feedback
    [SerializeField] private TextMeshProUGUI feedbackHeader;
    [SerializeField] private TextMeshProUGUI feedbackContent;
    [SerializeField] private Button feedbackCloseButton;
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
            SetupDungeon();
        }
        else
        {
            SetupWorld();
        }

        stypeDropdown.ClearOptions();
        List<string> options = System.Enum.GetNames(typeof(WorldStyle)).ToList();
        stypeDropdown.AddOptions(options);
        OnStyleChange();

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
    ///     This function sets the initiale state of each panel
    /// </summary>
    private void SetupPanels()
    {
        smallGeneratorPanel.SetActive(false);
        areaSettings.SetActive(true);
        content.SetActive(false);
        generatorPanel.SetActive(true);
        feedbackPanel.SetActive(false);
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
        DisplayFeedback("RESETING AREA...", "RESETING THE AREA TO DEFAULT ...", false);

        stypeDropdown.value = 0;
        continueButton.interactable = false;

        bool success = await uiManager.ResetToDefault();

        if(success)
        {
            DisplayFeedback("AREA RESET", "RESET THE CURRENT AREA TO DEFAULT", true);
        }
        else
        {
            DisplayFeedback("RESET", "COULD NOT RESET THE CURRENT AREA TO DEFAULT", true);
        }
    }

    public void GenerateLayoutButtonPressed()
    {
        DisplayFeedback("GENERATION LAYOUT...", "BLUB", false);

        Vector2Int size = new Vector2Int(int.Parse(sizeX.text), int.Parse(sizeY.text));
        WorldStyle style = (WorldStyle) stypeDropdown.value;
        LayoutGeneratorType layoutGeneratorType = (LayoutGeneratorType) generatorTypeDropdown.value;
        int accessability = int.Parse(accessabilityInput.text);
        string seed = seedInput.text;

        uiManager.GenerateLayout(size, style, layoutGeneratorType, accessability, seed);

        continueButton.interactable = true;

        ResetContentPanel();

        DisplayFeedback("LAYOUT GENERATED", "SUCCESSFULLY GENERATED A LAYOUT", true);
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
        bool success = GenerateMinigames();
        CheckSaveWorldButtonStatus();

        string feedback;
        if((int)amountMinigamesSlider.value > 0)
        {
            feedback = "CREATED: \n" + (int)amountMinigamesSlider.value + " MINIGAME SPOTS";
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
        bool success = GenerateNpcs();

        string feedback;
        if ((int)amountNpcsSlider.value > 0)
        {
            feedback = "CREATED: \n" + (int)amountNpcsSlider.value + " NPC SPOTS";
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
        bool success = GenerateBooks();

        string feedback;
        if ((int)amountBooksSlider.value > 0)
        {
            feedback = "CREATED: \n" + (int)amountBooksSlider.value + " BOOK SPOTS";
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
        bool success = GenerateTeleporter();

        string feedback;
        if ((int)amountTeleportersSlider.value > 0)
        {
            feedback = "CREATED: \n" + (int)amountTeleportersSlider.value + " TELEPORTER SPOTS";
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
        bool success = GenerateDungeons();

        string feedback;
        if ((int)amountDungeonsSlider.value > 0)
        {
            feedback = "CREATED: \n" + (int)amountDungeonsSlider.value + " DUNGEON SPOTS";
        }
        else
        {
            feedback = "REMOVED ALL DUNGEON SPOTS";
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
        DisplayFeedback(header, content, true);
    }
    #endregion

    public void ReturnButtonPressed()
    {
        content.SetActive(false);
        areaSettings.SetActive(true);
    }

    public void GenerateAllContentButtonPressed()
    {
        bool success = GenerateAllContent();
        CheckSaveWorldButtonStatus();

        string feedback;
        if((int)amountMinigamesSlider.value == 0 &&
            (int)amountNpcsSlider.value == 0 &&
            (int)amountBooksSlider.value == 0 &&
            (int)amountTeleportersSlider.value == 0 &&
            (int)amountDungeonsSlider.value == 0)
        {
            feedback = "RESET ALL CONTENT";
        }
        else
        {
            feedback = "CREATED:";
            if ((int)amountMinigamesSlider.value > 0)
            {
                feedback += "\n " + (int)amountMinigamesSlider.value + " MINIGAME SPOTS";
            }
            if ((int)amountNpcsSlider.value > 0)
            {
                feedback += "\n " + (int)amountNpcsSlider.value + " NPC SPOTS";
            }
            if ((int)amountBooksSlider.value > 0)
            {
                feedback += "\n " + (int)amountBooksSlider.value + " BOOK SPOTS";
            }
            if ((int)amountTeleportersSlider.value > 0)
            {
                feedback += "\n " + (int)amountTeleportersSlider.value + " TELEPORTER SPOTS";
            }
            if ((int)amountDungeonsSlider.value > 0)
            {
                feedback += "\n " + (int)amountDungeonsSlider.value + " DUNGEON SPOTS";
            }
        }        

        DisplayGenerationFeedback(success, feedback);
    }

    private bool GenerateAllContent()
    {
        uiManager.ResetObjects();
        CheckSaveWorldButtonStatus();

        if (!GenerateDungeons())
        {
            //could not create dungeon spots -> reset all and stop 
            uiManager.ResetObjects();
            return false;
        }

        if (!GenerateMinigames())
        {
            //could not create minigame spots -> reset all and stop 
            uiManager.ResetObjects();
            return false;
        }

        if (!GenerateNpcs())
        {
            //could not create npc spots -> reset all and stop 
            uiManager.ResetObjects();
            return false;
        }

        if (!GenerateBooks())
        {
            //could not create book spots -> reset all and stop 
            uiManager.ResetObjects();
            return false;
        }

        if (!GenerateTeleporter())
        {
            //could not create teleporter spots -> reset all and stop 
            uiManager.ResetObjects();
            return false;
        }

        //all spots created
        return true;
    }

    public void SaveAreaButtonPressed()
    {
        SaveArea();
    }

    private async void SaveArea()
    {
        DisplayFeedback("SAVING...", "SAVING THE CURRENT AREA...", false);

        bool success = await uiManager.SaveArea();

        if(success)
        {
            DisplayFeedback("AREA SAVED", "SAVED THE CURRENT AREA", true);
        }
        else
        {
            DisplayFeedback("AREA NOT SAVED", "COULD NOT SAVE THE CURRENT AREA", true);
        }        
    }
    #endregion

    #region Feedback
    public void FeedbackCloseButtonPressed()
    {
        feedbackPanel.SetActive(false);
    }

    private void DisplayFeedback(string header, string content, bool closeable)
    {
        feedbackHeader.text = header;
        feedbackContent.text = content;
        feedbackCloseButton.gameObject.SetActive(closeable);

        feedbackPanel.SetActive(true);
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
