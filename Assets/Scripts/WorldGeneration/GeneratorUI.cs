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
    private GeneratorManager generator;
    private AreaInformation currentArea;
    private AreaData areaData;
    private AreaInformationData areaInformation;

    //Panels
    [SerializeField] private GameObject generatorPanel;
    [SerializeField] private GameObject smallGeneratorPanel;
    [SerializeField] private GameObject areaSettings;
    [SerializeField] private GameObject content;

    //Area Settings
    [SerializeField] private TMP_InputField sizeX;    
    [SerializeField] private TMP_InputField sizeY;
    [SerializeField] private TMP_InputField offsetX;
    [SerializeField] private TMP_InputField offsetY;
    [SerializeField] private TMP_Dropdown stypeDropdown;
    [SerializeField] private Slider accessabilitySlider;
    [SerializeField] private Button generateLayoutButton;

    //Content
    [SerializeField] private TMP_InputField amountMinigames;
    [SerializeField] private TMP_InputField amountNPCs;
    [SerializeField] private TMP_InputField amountBooks;
    [SerializeField] private TMP_InputField amountTeleporter;
    [SerializeField] private TMP_InputField amountDungeons;
    [SerializeField] private Button generateMinigamesButton;
    [SerializeField] private Button generateNpcsButton;
    [SerializeField] private Button generateBooksButton;
    [SerializeField] private Button generateTeleporterButton;
    [SerializeField] private Button generateDungeonsButton;
    [SerializeField] private Button generateAllContentButton;
    [SerializeField] private Button saveAreaButton;
    #endregion

    /// <summary>
    ///     This function sets up the generator UI with the given values
    /// </summary>
    /// <param name="generator">The generator object</param>
    /// <param name="areaData">The data of the current area</param>
    /// <param name="areaInformation">Additional parameters for area generation</param>
    public void Setup(GeneratorManager generator, AreaData areaData, AreaInformationData areaInformation)
    {
        this.generator = generator;        
        this.areaData = areaData;
        this.areaInformation = areaInformation;
        currentArea = areaData.GetArea();

        smallGeneratorPanel.SetActive(false);
        areaSettings.SetActive(true);
        content.SetActive(false);
        generatorPanel.SetActive(true);

        if(currentArea.IsDungeon())
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

        accessabilitySlider.value = 0.5f;

        if(areaData.IsGeneratedArea())
        {
            stypeDropdown.value = (int) areaData.GetAreaMapData().GetWorldStyle();

            amountMinigames.text = areaData.GetAreaMapData().GetMinigameSpots().Count.ToString();
            amountNPCs.text = areaData.GetAreaMapData().GetNpcSpots().Count.ToString();
            amountBooks.text = areaData.GetAreaMapData().GetBookSpots().Count.ToString();
            amountTeleporter.text = areaData.GetAreaMapData().GetTeleporterSpots().Count.ToString();
            amountDungeons.text = areaData.GetAreaMapData().GetTeleporterSpots().Count.ToString();
        }
        else
        {
            stypeDropdown.value = (int) WorldStyle.CUSTOM;
            amountMinigames.text = "";
            amountNPCs.text = "";
            amountBooks.text = "";
            amountTeleporter.text = "";
            amountDungeons.text = "";
        }
    }

    /// <summary>
    ///     This function sets up the size and offset input fields for a world
    /// </summary>
    private void SetupWorld()
    {
        sizeX.text = areaInformation.GetSize().x.ToString();
        sizeY.text = areaInformation.GetSize().y.ToString();

        offsetX.text = areaInformation.GetOffset().x.ToString();
        offsetY.text = areaInformation.GetOffset().y.ToString();

        sizeX.enabled = false;
        sizeY.enabled = false;
        offsetX.enabled = false;
        offsetY.enabled = false;
    }

    /// <summary>
    ///     This function sets up the size and offset input fields for a dungeon
    /// </summary>
    private void SetupDungeon()
    {
        sizeX.text = areaInformation.GetSize().x.ToString();
        sizeY.text = areaInformation.GetSize().y.ToString();

        offsetX.text = areaInformation.GetOffset().x.ToString();
        offsetY.text = areaInformation.GetOffset().y.ToString();

        sizeX.enabled = true;
        sizeY.enabled = true;
        offsetX.enabled = false;
        offsetY.enabled = false;
    }

    public void MinimizeButtonPressed()
    {
        generatorPanel.SetActive(false);
        smallGeneratorPanel.SetActive(true);
        generator.ActivateCameraMovement();
    }

    public void MaximizeButtonPressed()
    {
        smallGeneratorPanel.SetActive(false);
        generatorPanel.SetActive(true);
        generator.DeactivateCameraMovement();
    }

    #region Area Settings Buttons
    public void ResetToCustomButtonPressed()
    {
        generator.ResetToCustom();
        stypeDropdown.value = 0;
    }

    public void GenerateLayoutButtonPressed()
    {
        Vector2Int size = new Vector2Int(int.Parse(sizeX.text), int.Parse(sizeY.text));
        WorldStyle style = (WorldStyle) stypeDropdown.value;
        float accessability = accessabilitySlider.value;

        CustomAreaMapData areaMapData = new CustomAreaMapData(style);
        generator.CreateLayout(size, areaMapData, accessability);

        ResetContentPanel();
    }

    /// <summary>
    ///     This function resets the content panel
    /// </summary>
    private void ResetContentPanel()
    {
        amountMinigames.text = "0";
        OnMinigameAmountChange();
        amountNPCs.text = "0";
        amountBooks.text = "0";
        amountTeleporter.text = "0";
        amountDungeons.text = "0";
    }

    public void ContinueButtonPressed()
    {
        areaSettings.SetActive(false);
        content.SetActive(true);
    }
    #endregion

    #region Content Buttons

    #region Generation Buttons
    public void GenerateMinigamesButtonPressed()
    {
        Vector2Int offset = GetOffset();
        int amount;
        try
        {
            amount = int.Parse(amountMinigames.text);
        }
        catch (System.FormatException e)
        {
            amount = 1;
        }
        generator.GenerateMinigames(amount, currentArea, offset);
        CheckSaveWorldButtonStatus();
    }

    public void GenerateNpcsButtonPressed()
    {
        Vector2Int offset = GetOffset();
        int amount;
        try
        {
            amount = int.Parse(amountNPCs.text);
        }
        catch(System.FormatException e)
        {
            amount = 0;
        }
        generator.GenerateNPCs(amount, currentArea, offset);
    }

    public void GenerateBooksButtonPressed()
    {
        Vector2Int offset = GetOffset();
        int amount;
        try
        {
            amount = int.Parse(amountBooks.text);
        }
        catch (System.FormatException e)
        {
            amount = 0;
        }
        generator.GenerateBooks(amount, currentArea, offset);
    }

    public void GenerateTeleporterButtonPressed()
    {
        Vector2Int offset = GetOffset();
        int amount;
        try
        {
            amount = int.Parse(amountTeleporter.text);
        }
        catch (System.FormatException e)
        {
            amount = 0;
        }
        generator.GenerateTeleporter(amount, currentArea, offset);
    }

    public void GenerateDungeonsButtonPressed()
    {
        Vector2Int offset = GetOffset();
        int amount;
        try
        {
            amount = int.Parse(amountDungeons.text);
        }
        catch (System.FormatException e)
        {
            amount = 0;
        }
        generator.GenerateDungeons(amount, currentArea, offset);
    }
    #endregion

    public void ReturnButtonPressed()
    {
        content.SetActive(false);
        areaSettings.SetActive(true);
    }

    public void GenerateAllContentButtonPressed()
    {
        Vector2Int offset = GetOffset();
        GenerateMinigamesButtonPressed();
        GenerateNpcsButtonPressed();
        GenerateBooksButtonPressed();
        GenerateTeleporterButtonPressed();
        GenerateDungeonsButtonPressed();
    }

    public void SaveAreaButtonPressed()
    {
        generator.SaveArea();
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
    ///     This function is called when the value of the <c>Amount of Minigames</c> input field value is changed and sets the <c>Generate Minigames</c>
    ///     button active or inactive, based on the entered value
    /// </summary>
    public void OnMinigameAmountChange()
    {
        bool validAmountOfMinigames = true;
        if (amountMinigames.text.Equals(""))
        {
            validAmountOfMinigames = false;
        }
        else
        {
            int amount;
            try
            {
                amount = int.Parse(amountMinigames.text);
                if (amount == 0)
                {
                    validAmountOfMinigames = false;
                }
            }
            catch (System.FormatException e)
            {
                validAmountOfMinigames = false;
            }
        }
        generateMinigamesButton.interactable = validAmountOfMinigames;

        CheckGenerateAllContentButtonStatus();
        CheckSaveWorldButtonStatus();
    }

    /// <summary>
    ///     This function is called when the value of the <c>Amount of NPCs</c> input field value is changed and sets the <c>Generate NPCs</c>
    ///     button active or inactive, based on the entered value
    /// </summary>
    public void OnNpcAmountChange()
    {
        if (amountNPCs.text.Equals(""))
        {
            generateNpcsButton.interactable = false;
        }
        else
        {
            generateNpcsButton.interactable = true;
        }
        CheckGenerateAllContentButtonStatus();
    }

    /// <summary>
    ///     This function is called when the value of the <c>Amount of Books</c> input field value is changed and sets the <c>Generate Books</c>
    ///     button active or inactive, based on the entered value
    /// </summary>
    public void OnBookAmountChange()
    {
        if (amountBooks.text.Equals(""))
        {
            generateBooksButton.interactable = false;
        }
        else
        {
            generateBooksButton.interactable = true;
        }
        CheckGenerateAllContentButtonStatus();
    }

    /// <summary>
    ///     This function is called when the value of the <c>Amount of Teleporter</c> input field value is changed and sets the <c>Generate Teleporter</c>
    ///     button active or inactive, based on the entered value
    /// </summary>
    public void OnTeleporterAmountChange()
    {
        if (amountTeleporter.text.Equals(""))
        {
            generateTeleporterButton.interactable = false;
        }
        else
        {
            generateTeleporterButton.interactable = true;
        }
        CheckGenerateAllContentButtonStatus();
    }

    /// <summary>
    ///     This function is called when the value of the <c>Amount of Dungeons</c> input field value is changed and sets the <c>Generate Dungeons</c>
    ///     button active or inactive, based on the entered value
    /// </summary>
    public void OnDungeonsAmountChange()
    {
        if (amountDungeons.text.Equals(""))
        {
            generateDungeonsButton.interactable = false;
        }
        else
        {
            generateDungeonsButton.interactable = true;
        }
        CheckGenerateAllContentButtonStatus();
    }

    /// <summary>
    ///     This function is called, when the value of any <c>Amount of content</c> input field value is changed and sets the <c>Generate All Content</c>
    ///     button active or inactive, based on the entered value
    /// </summary>
    private void CheckGenerateAllContentButtonStatus()
    {
        bool allValid = true;
        if(!generateMinigamesButton.IsInteractable())
        {
            allValid = false;
        }
        if (!generateNpcsButton.IsInteractable())
        {
            allValid = false;
        }
        if (!generateBooksButton.IsInteractable())
        {
            allValid = false;
        }
        if (!generateTeleporterButton.IsInteractable())
        {
            allValid = false;
        }
        if (!generateDungeonsButton.IsInteractable())
        {
            allValid = false;
        }
        generateAllContentButton.interactable = allValid;
    }

    /// <summary>
    ///     This function is called, when the value of the <c>Amount of Minigames</c> input field value is changed and sets the <c>Save Area</c>
    ///     button active, if at least one minigame exists, or inactive, otherwise
    /// </summary>
    private void CheckSaveWorldButtonStatus()
    {
        if(!generator.GetAreaData().IsGeneratedArea())
        {
            saveAreaButton.interactable = false;
        }
        else if(generator.GetAreaData().GetAreaMapData().GetMinigameSpots().Count > 0)
        {
            saveAreaButton.interactable = true;
        }
        else
        {
            saveAreaButton.interactable = false;
        }
    }
    #endregion

    private Vector2Int GetOffset()
    {
        Vector2Int offset = new Vector2Int(int.Parse(offsetX.text), int.Parse(offsetY.text));
        return offset;
    }
}
