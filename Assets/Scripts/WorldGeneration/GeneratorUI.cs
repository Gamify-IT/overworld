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
    #endregion

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
        List<WorldConnection> worldConnections = new List<WorldConnection>();
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

    }

    public void GenerateNpcsButtonPressed()
    {

    }

    public void GenerateBooksButtonPressed()
    {

    }

    public void GenerateTeleporterButtonPressed()
    {

    }

    public void GenerateDungeonsButtonPressed()
    {

    }
    #endregion

    public void ReturnButtonPressed()
    {
        content.SetActive(false);
        areaSettings.SetActive(true);
    }

    public void GenerateAllContentButtonPressed()
    {

    }

    public void SaveAreaButtonPressed()
    {

    }
    #endregion
}
