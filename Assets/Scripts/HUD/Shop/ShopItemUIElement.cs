using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUIElement : MonoBehaviour
{
    
        [SerializeField] private TMP_Text title;
        [SerializeField] private Image image;
    

    public void Setup(string title,  Sprite image)
    {
        this.title.text = title.Replace("_", " ");
        this.image.sprite = image;
    }

}
