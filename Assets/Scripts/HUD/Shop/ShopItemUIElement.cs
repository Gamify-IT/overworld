using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUIElement : MonoBehaviour
{
    
        [SerializeField] private TMP_Text title;
        [SerializeField] private Image image;
        [SerializeField] private GameObject boughtImage;
        [SerializeField] private Image coinItemImage;

    public void Setup(string title,  Sprite image, bool bought, bool showCoin)
    {
        this.title.text = title.Replace("_", " ");
        this.image.sprite = image;
        boughtImage.SetActive(bought);
        coinItemImage.enabled = showCoin;

    }

}
