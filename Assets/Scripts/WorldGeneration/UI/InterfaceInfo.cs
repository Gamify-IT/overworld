using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
///     This class handles the UI elements of the help sections
/// </summary>
public class InterfaceInfo : MonoBehaviour
{
    public static InterfaceInfo Instance { get; private set; }

    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI infoHeader;
    [SerializeField] private ScrollRect infoScrollRect;
    [SerializeField] private TextMeshProUGUI infoContent;
    [SerializeField] private Button infoCloseButton;
    [SerializeField] private Button infoCloseAndViewButton;

    /// <summary>
    /// Sets up the singleton instance 
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            infoPanel.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Generator Infos

    public void GeneratorHelpButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            string header = "AREA GENERATION - INFORMATION";
            string body = "INSPECT AREA: \n " +
                "To inspect the current area use the 'SHOW MAP' Button in the to right corner. To get back, simply click the same button again. \n " +
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
                "If you placed at least one minigame, and in a dungeon exactly one dungeon exit, you can save the area with the 'SAVE AREA' button. ";
            DisplayInfo(header, body, true, false);
        }
    }

    public void SizeInfoButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            Tuple<int, int> sizeLimit = GetSizeLimits();

            string header = "SIZE SETTING";
            string body = "With the size setting, you can specifiy the dimensions of the generated area, with the 'X' input defining the width, the 'Y' input the height, measured in tiles. \n " +
                "You can only change the size of a dungeon area, a world area must have a fixed size. An area need to be at between " + sizeLimit.Item1 + " and " + sizeLimit.Item2 + " tiles in width and height.";
            DisplayInfo(header, body, true, false);
        }
    }

    /// <summary>
    ///     This function reads to generator settings from the local file and sets up the variables needed
    /// </summary>
    private Tuple<int, int> GetSizeLimits()
    {
        string path = "GameSettings/GeneratorSettings";
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        string json = targetFile.text;
        GenerationSettings generationSettings = GenerationSettings.CreateFromJSON(json);

        return new Tuple<int, int>(generationSettings.minAreaSize, generationSettings.maxAreaSize);
    }

    public void StyleInfoButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
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
            DisplayInfo(header, body, true, false);
        }
    }

    public void TypeInfoButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
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
                "'ISLANDS - CELLULAR AUTOMATA': This generator results in many smaller, well shaped islands, connected with narrow pathways \n" +
                "\n" +
                "'ISLANDS - DRUNKARDS WALK': This generator results in many smaller, chaoticly shaped islands, connected with narrow pathways";
            DisplayInfo(header, body, true, false);
        }
    }

    public void AccessableInfoButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            string header = "ACCESSABLE SETTING";
            string body = "The accessable setting defines how much area the player will be able to explore. A higher value translates to more exploreable space.";
            DisplayInfo(header, body, true, false);
        }
    }

    public void SeedInfoButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            string header = "SEED SETTING";
            string body = "The seed is used to generate different layouts. With the same seed and other settings, the exact same layout is reproduced. \n" +
                "\n" +
                "You can change the 'style' setting, to see the layout in different themes with only very slight adjustments. \n" +
                "\n" +
                "In case you want to use the same layout in another course, you can copy the seed and use it there with the same settings to effectively copy the layout.";
            DisplayInfo(header, body, true, false);
        }
    }

    public void MinigamesGenerationInfoButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
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
            DisplayInfo(header, body, true, false);
        }
    }

    public void NpcsGenerationInfoButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
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
            DisplayInfo(header, body, true, false);
        }
    }

    public void BooksGenerationInfoButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
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
            DisplayInfo(header, body, true, false);
        }
    }

    public void TeleporterGenerationInfoButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
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
            DisplayInfo(header, body, true, false);
        }
    }

    public void DungeonsGenerationInfoButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
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
            DisplayInfo(header, body, true, false);
        }
    }

    #endregion

    #region Inspector Infos

    public void InspectorHelpButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            string header = "AREA INSPECTION - INFORMATION";
            string body = "INSPECT AREA: \n " +
                "To inspect the current area use the 'SHOW MAP' Button in the to right corner. To get back, simply click the same button again. \n " +
                "\n" +
                "CAMERA MOVEMENT: \n " +
                "To move the camera, hold down the left mouse botton and drag the map around with the mouse. To zoom in and out, use the mouse wheel. \n" +
                "\n" +
                "FEATURES: \n" +
                "You can switch the icons of an object type on or off by using the respective checkbox.";
            DisplayInfo(header, body, true, false);
        }
    }

    public void MinigamesInspectorInfoButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            string header = "MINIGAME SPOTS";
            string body = "A minigame spot represents a position for a minigame. There, students can enter a game focused on a part of the content of the lecture. \n" +
                "\n" +
                "To enable or disable the icons on the layout, use the respective checkbox.";
            DisplayInfo(header, body, true, false);
        }
    }

    public void NpcsInspectorInfoButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            string header = "NPC SPOTS";
            string body = "A npc spot represents a position for a npc. That is a Non-Player-Character, that is standing around and the students can talk to. \n" +
                "\n" +
                "The npcs are meant to provide additional information about the content required in a minigame, therefore, they are usually placed near their corresponding minigame. \n " +
                "\n" +
                "To enable or disable the icons on the layout, use the respective checkbox.";
            DisplayInfo(header, body, true, false);
        }
    }

    public void BooksInspectorInfoButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            string header = "BOOK SPOTS";
            string body = "A book spot represents a position for a book. They are similar to npcs, but contain more content and continuous text. \n" +
                "They are also, like npcs, placed near their corresponding minigame. \n" +
                "\n" +
                "To enable or disable the icons on the layout, use the respective checkbox.";
            DisplayInfo(header, body, true, false);
        }
    }

    public void TeleporterInspectorInfoButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            string header = "TELEPORTER SPOTS";
            string body = "A teleporter spot represents a position for a teleporter. The students can activate them and then use them instantly travel to another activated teleporter. \n " +
                "That helps to get around the area faster and reduce walking times. \n " +
                "\n " +
                "To enable or disable the icons on the layout, use the respective checkbox.";
            DisplayInfo(header, body, true, false);
        }
    }

    public void DungeonsInspectorInfoButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            string header = "DUNGEON SPOTS";
            string body = "A dungeon spot represents a position for a dungeon entrance. There, students can enter or leave a dungeon. \n" +
                "\n" +
                "To enable or disable the icons on the layout, use the respective checkbox. \n ";
            DisplayInfo(header, body, true, false);
        }
    }

    public void BarriersInfoButtonPressed()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            string header = "BARRIER SPOTS";
            string body = "A dungeon spot represents a position for a world or dungeon barrier. They block of access to worlds and dungeons which have not been unlocked yet. \n" +
                "\n" +
                "The amount of barriers spots depends on the type of area. \n " +
                "A world contains a barrier for each dungeon and for each connected world that are further back in the order. \n" +
                "A dungeon does not contain any barriers. \n" +
                "\n" +
                "To enable or disable the icons on the layout, use the respective checkbox.";
            DisplayInfo(header, body, true, false);
        }
    }

    #endregion

    #region Selector Infos

    public void AreaSelectionInfoButtonPressed()
    {
        Debug.Log("active");
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            string header = "Area Info";
            string body = "In this menu, you can choose for which course you want to create or update Worlds and Dungeons. \n" +
                            "\n" + "<b>Course ID</b>: Choose the Course for which you want to create a new area. \n" +
                            "\n" + "<b>World</b>: Enter the World Index for the area you want to create. \n" +
                            "\n" + "<b>Dungeon:</b> Enter the Dungeon Index if you want to create a new Dungeon.\n" +
                            "\n" + "<b>Hint</b>: If there is already an existing world for this index, the correspondending world will be updated.";
            DisplayInfo(header, body, true, false);
        }
    }

    public void WorldInfoButtonPressed()
    {
        Debug.Log("active");
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            string header = "World Info";
            string body = "Each World has an index to indentify it.";
            DisplayInfo(header, body, true, false);
        }
    }
    public void DungeonInfoButtonPressed()
    {
        Debug.Log("active");
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            string header = "Dungeon Info";
            string body = "Each World can have up to four Dungeons. \n" +
                            "If you just want to create or update a new World, leave the field empty.";
            DisplayInfo(header, body, true, false);
        }
    }

    public void DisplayErrorInfo()
    {
        Debug.Log("active");
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            string header = "Invalid Values";
            string body = "One or more entered values are not correct. \n" 
                            + "\n" + "Please check the info boxes for more information anbd try again.";
            DisplayInfo(header, body, true, false);
        }
    }

    #endregion

    /// <summary>
    ///     This function closes the help panel
    /// </summary>
    public void InfoCloseButtonPressed()
    {
        infoPanel.SetActive(false);
    }

    /// <summary>
    ///     This function displays the provided content and enables / disables the closing buttons
    /// </summary>
    /// <param name="header">The header of the information</param>
    /// <param name="content">The information provided</param>
    /// <param name="closeable">If the info panel can be closed</param>
    /// <param name="closeAndView">If the info panel can be closed together with the generator / inspector UI</param>
    public void DisplayInfo(string header, string content, bool closeable, bool closeAndView)
    {
        infoHeader.text = header;
        infoContent.text = content;
        infoScrollRect.verticalNormalizedPosition = 1f;
        infoCloseButton.gameObject.SetActive(closeable);

        if(infoCloseAndViewButton != null)
        {
            infoCloseAndViewButton.gameObject.SetActive(closeAndView);
        }        

        infoPanel.SetActive(true);
    }
}
