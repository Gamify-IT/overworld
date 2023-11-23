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
    [SerializeField] private ScrollRect feedbackScrollRect;
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

    #region Info Buttons

    public void HelpButtonPressed()
    {
        if(feedbackPanel.activeSelf)
        {
            feedbackPanel.SetActive(false);
        }
        else
        {
            string header = "AREA GENERATION - INFORMATION";
            string body = "INSPECT AREA: \n " +
                "To inspect the current area use the 'GENERATOR MAP' Button in the to right corner. To get back, simply click the same button again. \n " +
                "\n" +
                "CAMERA MOVEMENT: \n " +
                "To move the camera, hold down the left mouse botton and drag the map around with the mouse. To zoom in and out, use the mouse wheel. \n" +
                "\n" +
                "GENERATION PROCESS: \n" +
                "\n" +
                "LAYOUT GENERATION: \n " +
                "To generate an area, you first need to generate a layout by selecting a 'Style', a 'Type' and an 'Accessable' value represeting the size of the walkable space. For more information, use the 'i' button next to them. \n" +
                "Furthermore, you can either use a random 'Seed' or use a specific one. The seed, together with the other values, defines an area. Using the same 'Type' and 'Accessability' with the same 'Seed' results in the same layout. \n " +
                "After selecting your parameters, use the 'GENERATE LAYOUT' button in the bottom middle. \n " +
                "Once a good layout is generated, proceed with the 'CONTINUE' button in the bottom right. \n" +
                "\n" +
                "CONTENT GENERATION: \n " +
                "Next, you can select the amount of object-type you want and add them with the respective 'GENERATE' button, or all of them at once with the 'GENERATE ALL' button, located in the bottom middle. \n" +
                "You will receive feedback, whether or not the objects could be placed or not. In case they could not, you can either try again, change the values or go back and generate another layout using the 'RETURN' button. \n " +
                "If you placed at least one minigame, you can save the area with the 'SAVE AREA' button. ";
            DisplayFeedback(header, body, true);
        }
    }

    public void SizeInfoButtonPressed()
    {
        if (feedbackPanel.activeSelf)
        {
            feedbackPanel.SetActive(false);
        }
        else
        {
            string header = "SIZE SETTING";
            string body = "With the size setting, you can specifiy the dimensions of the generated area, with the 'X' input defining the width, the 'Y' input the height. \n " +
                "You can only change the size of a dungeon area, a world area must have a fixed size.";
            DisplayFeedback(header, body, true);
        }
    }

    public void StyleInfoButtonPressed()
    {
        if (feedbackPanel.activeSelf)
        {
            feedbackPanel.SetActive(false);
        }
        else
        {
            string header = "STYLE SETTING";
            string body = "The style setting defines the theme of the generated area. Currently, there are four themes: \n" +
                "\n" +
                "'CAVE': An underground tunnel system \n" +
                "\n" +
                "'SAVANNA': A grassland enviroment surrounded by water and steep cliffs \n " +
                "\n" +
                "'BEACH': A collection of sandy islands surrounded by the sea, connected with pathways in shallow water \n" +
                "\n" +
                "'FOREST': A forest with pathways and clearings";
            DisplayFeedback(header, body, true);
        }
    }

    public void TypeInfoButtonPressed()
    {
        if (feedbackPanel.activeSelf)
        {
            feedbackPanel.SetActive(false);
        }
        else
        {
            string header = "TYPE SETTING";
            string body = "The type setting defines the generator used and thereby which characteristics the area has. The different types are: \n" +
                "\n" +
                "'CELLULAR AUTOMATA': This generator results in smaller pathways, spread around the entirety of the area \n" +
                "\n" +
                "'DRUNKARDS WALK': This generator results in few bigger areas connected by pathways \n" +
                "\n" +
                "'ISLANDS - CA': This generator results in many smaller, well shaped islands, connected with narrow pathways \n" +
                "\n" +
                "'ISLANDS _ DW': This generator results in many smaller, chaoticly shaped islands, connected with narrow pathways";
            DisplayFeedback(header, body, true);
        }
    }

    public void AccessableInfoButtonPressed()
    {
        if (feedbackPanel.activeSelf)
        {
            feedbackPanel.SetActive(false);
        }
        else
        {
            string header = "ACCESSABLE SETTING";
            string body = "The accessable setting defines, how much area the player will be able to explore. A higher value translates to more exploreable space.";
            DisplayFeedback(header, body, true);
        }
    }

    public void SeedInfoButtonPressed()
    {
        if (feedbackPanel.activeSelf)
        {
            feedbackPanel.SetActive(false);
        }
        else
        {
            string header = "SEED SETTING";
            string body = "The seed is used to generate different layouts. With the same seed and other settings, the exact same layout is reproduced. \n" +
                "\n" +
                "You can change the 'style' setting, to see the layout in different themes with only very slight adjustments. \n" +
                "\n" +
                "In case you want to use the same layout in another course, you can copy the seed and use it there with the same settings to effectively copy the layout.";
            DisplayFeedback(header, body, true);
        }
    }

    public void MinigamesInfoButtonPressed()
    {
        if (feedbackPanel.activeSelf)
        {
            feedbackPanel.SetActive(false);
        }
        else
        {
            string header = "MINIGAME SPOTS";
            string body = "A minigame spot represents a position for a minigame. There, students can enter a game focused on a part of the content of the lecture. \n" +
                "\n" +
                "The amount of minigame spots can be set with the slider and the currently selected amount can be placed with the 'GENERATE' button. \n " +
                "To remove all minigame spots, simply set the slider to '0' and press the 'GENERATE' button. \n" +
                "\n" +
                "To enable or disable the icons on the layout, use the respective checkbox.";
            DisplayFeedback(header, body, true);
        }
    }

    public void NpcsInfoButtonPressed()
    {
        if (feedbackPanel.activeSelf)
        {
            feedbackPanel.SetActive(false);
        }
        else
        {
            string header = "NPC SPOTS";
            string body = "A npc spot represents a position for a npc. That is a Non-Player-Character, that is standing around and the students can talk to. \n" +
                "\n" +
                "The npcs are meant to provide additional information about the content required in a minigame, therefore, they are placed near their corresponding minigame. \n " +
                "To ensure that, it is recommended to generate them after the minigame spots have been placed. \n " +
                "\n" +
                "The amount of npc spots can be set with the slider and the currently selected amount can be placed with the 'GENERATE' button. \n " +
                "To remove all npc spots, simply set the slider to '0' and press the 'GENERATE' button. \n" +
                "\n" +
                "To enable or disable the icons on the layout, use the respective checkbox.";
            DisplayFeedback(header, body, true);
        }
    }

    public void BooksInfoButtonPressed()
    {
        if (feedbackPanel.activeSelf)
        {
            feedbackPanel.SetActive(false);
        }
        else
        {
            string header = "BOOK SPOTS";
            string body = "A book spot represents a position for a book. They are similar to npcs, but contain more content and continuous text. \n" +
                "They are also, like npcs, placed near their corresponding minigame. To ensure that, it is recommended to generate them after the minigame spots have been placed \n " +
                "\n" +
                "The amount of book spots can be set with the slider and the currently selected amount can be placed with the 'GENERATE' button. \n " +
                "To remove all book spots, simply set the slider to '0' and press the 'GENERATE' button. \n" +
                "\n" +
                "To enable or disable the icons on the layout, use the respective checkbox.";
            DisplayFeedback(header, body, true);
        }
    }

    public void TeleporterInfoButtonPressed()
    {
        if (feedbackPanel.activeSelf)
        {
            feedbackPanel.SetActive(false);
        }
        else
        {
            string header = "TELEPORTER SPOTS";
            string body = "A teleporter spot represents a position for a teleporter. The students can activate them and then use them instantly travel to another activated teleporter. \n " +
                "That helps to get around the area faster and reduce walking times. \n " +
                "\n " +
                "The amount of teleporter spots can be set with the slider and the currently selected amount can be placed with the 'GENERATE' button. \n " +
                "To remove all teleporter spots, simply set the slider to '0' and press the 'GENERATE' button. \n" +
                "\n" +
                "To enable or disable the icons on the layout, use the respective checkbox.";
            DisplayFeedback(header, body, true);
        }
    }

    public void DungeonsInfoButtonPressed()
    {
        if (feedbackPanel.activeSelf)
        {
            feedbackPanel.SetActive(false);
        }
        else
        {
            string header = "DUNGEON SPOTS";
            string body = "A dungeon spot represents a position for a dungeon entrance. There, students can enter or leave a dungeon. \n" +
                "\n" +
                "The amount of dungeon spots can be set with the slider and the currently selected amount can be placed with the 'GENERATE' button. \n " +
                "To remove all dungeon spots, simply set the slider to '0' and press the 'GENERATE' button. \n" +
                "\n" +
                "To enable or disable the icons on the layout, use the respective checkbox. \n " +
                "\n" +
                "A world area can have multiple dungeon spots, but is also allowed to have none at all. \n" +
                "Each dungeon is required to have exactly one dungeon spot, which is the exit. Therefore, in a dungeon area, the amount of dungeon spots cannot be changed, but the position can still be changed using the 'GENERATE' button.";
            DisplayFeedback(header, body, true);
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
        CheckSaveWorldButtonStatus();

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
        feedbackScrollRect.verticalNormalizedPosition = 1f;
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
