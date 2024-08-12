using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUIElement : MonoBehaviour
{
    
        [SerializeField] private TMP_Text title;
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text price;


    public void Setup(string title,  Sprite image, int price)
    {
        this.title.text = title.Replace("_", " ");
        this.image.sprite = image;
        this.price.text = price.ToString();
    }

}
