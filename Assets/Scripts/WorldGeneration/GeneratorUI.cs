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
    private AreaInformation areaIdentifier;
    //private AreaData areaData;
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
    [SerializeField] private Button continueButton;

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
    /// <param name="uiManager">The generator object</param>
    /// <param name="areaData">The data of the current area</param>
    /// <param name="areaInformation">Additional parameters for area generation</param>
    public void Setup(GeneratorUIManager uiManager, AreaData areaData, AreaInformationData areaInformation)
    {
        this.uiManager = uiManager;        
        this.areaInformation = areaInformation;
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

        accessabilitySlider.value = 0.5f;

        if(areaData.IsGeneratedArea())
        {
            stypeDropdown.value = (int) areaData.GetAreaMapData().GetWorldStyle();

            continueButton.interactable = true;

            amountMinigames.text = areaData.GetAreaMapData().GetMinigameSpots().Count.ToString();
            amountNPCs.text = areaData.GetAreaMapData().GetNpcSpots().Count.ToString();
            amountBooks.text = areaData.GetAreaMapData().GetBookSpots().Count.ToString();
            amountTeleporter.text = areaData.GetAreaMapData().GetTeleporterSpots().Count.ToString();
            amountDungeons.text = areaData.GetAreaMapData().GetTeleporterSpots().Count.ToString();
        }
        else
        {
            stypeDropdown.value = (int) WorldStyle.CUSTOM;

            continueButton.interactable = false;

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

        offsetX.text = areaInformation.GetObjectOffset().x.ToString();
        offsetY.text = areaInformation.GetObjectOffset().y.ToString();

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

        offsetX.text = areaInformation.GetObjectOffset().x.ToString();
        offsetY.text = areaInformation.GetObjectOffset().y.ToString();

        sizeX.enabled = true;
        sizeY.enabled = true;
        offsetX.enabled = false;
        offsetY.enabled = false;
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
        float accessability = accessabilitySlider.value;

        uiManager.GenerateLayout(size, style, accessability);

        continueButton.interactable = true;

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
        int amount;
        try
        {
            amount = int.Parse(amountMinigames.text);
        }
        catch (System.FormatException e)
        {
            Debug.LogError(e.ToString());
            amount = 1;
        }
        uiManager.GenerateMinigames(amount);
        CheckSaveWorldButtonStatus();
    }

    public void GenerateNpcsButtonPressed()
    {
        int amount;
        try
        {
            amount = int.Parse(amountNPCs.text);
        }
        catch(System.FormatException e)
        {
            Debug.LogError(e.ToString());
            amount = 0;
        }
        uiManager.GenerateNpcs(amount);
    }

    public void GenerateBooksButtonPressed()
    {
        int amount;
        try
        {
            amount = int.Parse(amountBooks.text);
        }
        catch (System.FormatException e)
        {
            Debug.LogError(e.ToString());
            amount = 0;
        }
        uiManager.GenerateBooks(amount);
    }

    public void GenerateTeleporterButtonPressed()
    {
        int amount;
        try
        {
            amount = int.Parse(amountTeleporter.text);
        }
        catch (System.FormatException e)
        {
            Debug.LogError(e.ToString());
            amount = 0;
        }
        uiManager.GenerateTeleporters(amount);
    }

    public void GenerateDungeonsButtonPressed()
    {
        int amount;
        try
        {
            amount = int.Parse(amountDungeons.text);
        }
        catch (System.FormatException e)
        {
            Debug.LogError(e.ToString());
            amount = 0;
        }
        uiManager.GenerateDungeons(amount);
    }
    #endregion

    public void ReturnButtonPressed()
    {
        content.SetActive(false);
        areaSettings.SetActive(true);
    }

    public void GenerateAllContentButtonPressed()
    {
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
                Debug.LogError(e.ToString());
                validAmountOfMinigames = false;
            }
        }
        generateMinigamesButton.interactable = validAmountOfMinigames;

        CheckGenerateAllContentButtonStatus();
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
