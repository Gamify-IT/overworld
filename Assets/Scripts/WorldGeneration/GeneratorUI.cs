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

    //Content
    [SerializeField] private TMP_InputField amountMinigames;
    [SerializeField] private TMP_InputField amountNPCs;
    [SerializeField] private TMP_InputField amountBooks;
    [SerializeField] private TMP_InputField amountTeleporter;
    [SerializeField] private TMP_InputField amountDungeons;
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
        stypeDropdown.value = 1;

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
        generator.GenerateMinigames(int.Parse(amountMinigames.text), currentArea, offset);
    }

    public void GenerateNpcsButtonPressed()
    {
        Vector2Int offset = GetOffset();
        generator.GenerateNPCs(int.Parse(amountNPCs.text), currentArea, offset);
    }

    public void GenerateBooksButtonPressed()
    {
        Vector2Int offset = GetOffset();
        generator.GenerateBooks(int.Parse(amountBooks.text), currentArea, offset);
    }

    public void GenerateTeleporterButtonPressed()
    {
        Vector2Int offset = GetOffset();
        generator.GenerateTeleporter(int.Parse(amountTeleporter.text), currentArea, offset);
    }

    public void GenerateDungeonsButtonPressed()
    {
        Vector2Int offset = GetOffset();
        generator.GenerateDungeons(int.Parse(amountDungeons.text), currentArea, offset);
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
        generator.GenerateMinigames(int.Parse(amountMinigames.text), currentArea, offset);
        generator.GenerateNPCs(int.Parse(amountNPCs.text), currentArea, offset);
        generator.GenerateBooks(int.Parse(amountBooks.text), currentArea, offset);
        generator.GenerateTeleporter(int.Parse(amountTeleporter.text), currentArea, offset);
        generator.GenerateDungeons(int.Parse(amountDungeons.text), currentArea, offset);
    }

    public void SaveAreaButtonPressed()
    {
        generator.SaveArea();
    }
    #endregion

    private Vector2Int GetOffset()
    {
        Vector2Int offset = new Vector2Int(int.Parse(offsetX.text), int.Parse(offsetY.text));
        return offset;
    }
}
