using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
///     This class is used to visualize the shop items in the shop
/// </summary>
public class ShopItemUIElement : MonoBehaviour
{
    
    [SerializeField] private TMP_Text title;
    [SerializeField] private Image image;
    [SerializeField] private GameObject boughtImage;
    [SerializeField] private Image coinItemImage;

    /// <summary>
    /// Sets up the shop item with the items description, image, bought status, price
    /// </summary>
    /// <param name="title">The title of the shop item.</param>
    /// <param name="image">The image of the shop item.</param>
    /// <param name="bought">The status of the shop item for the current player.</param>
    /// <param name="showCoin">Whether to show a crown icon based on the player's rank.</param>
    public void Setup(string title,  Sprite image, bool bought, bool showCoin)
    {
        this.title.text = title.Replace("_", " ");
        this.image.sprite = image;
        boughtImage.SetActive(bought);
        coinItemImage.enabled = showCoin;

    }

}
