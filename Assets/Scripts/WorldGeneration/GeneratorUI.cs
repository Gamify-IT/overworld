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
    #endregion

    private void Awake()
    {
        currentArea = new AreaInformation(1, new Optional<int>());
    }

    /// <summary>
    ///     This function sets up the generator UI with the given values
    /// </summary>
    /// <param name="generator">The generator object</param>
    /// <param name="size">The size of the area</param>
    /// <param name="offset">The offset of the area (only for worlds relevant)</param>
    /// <param name="style">The style of the area</param>
    /// <param name="accessability">The percentage of walkable space</param>
    /// <param name="worldConnections">A list of connection points to other worlds (only for worlds relevant)</param>
    public void Setup(GeneratorManager generator, Vector2Int size, Vector2Int offset, WorldStyle style, float accessability, List<WorldConnection> worldConnections)
    {
        this.generator = generator;

        smallGeneratorPanel.SetActive(false);
        areaSettings.SetActive(true);
        content.SetActive(false);
        generatorPanel.SetActive(true);
        
        sizeX.text = size.x.ToString();
        sizeY.text = size.y.ToString();

        offsetX.text = offset.x.ToString();
        offsetY.text = offset.y.ToString();

        stypeDropdown.ClearOptions();
        List<string> options = System.Enum.GetNames(typeof(WorldStyle)).ToList();
        stypeDropdown.AddOptions(options);
        stypeDropdown.value = (int) style;

        accessabilitySlider.value = accessability;
    }

    public void MinimizeButtonPressed()
    {
        generatorPanel.SetActive(false);
        smallGeneratorPanel.SetActive(true);
    }

    public void MaximizeButtonPressed()
    {
        smallGeneratorPanel.SetActive(false);
        generatorPanel.SetActive(true);        
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
        Vector2Int offset = new Vector2Int(int.Parse(offsetX.text), int.Parse(offsetY.text));
        WorldStyle style = (WorldStyle) stypeDropdown.value;
        float accessability = accessabilitySlider.value;
        Optional<List<WorldConnection>> worldConnections = new Optional<List<WorldConnection>>();
        generator.CreateLayout(size, offset, style, accessability, worldConnections);
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
        if(amountMinigames.text.Equals(""))
        {
            generateMinigamesButton.interactable = false;
        }
        else
        {
            generateMinigamesButton.interactable = true;
        }
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
    #endregion

    private Vector2Int GetOffset()
    {
        Vector2Int offset = new Vector2Int(int.Parse(offsetX.text), int.Parse(offsetY.text));
        return offset;
    }
}
