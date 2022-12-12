using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementUIElement : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text progress;
    [SerializeField] private GameObject status;

    public void Setup(string title, string description, Sprite image, int progress, int amountRequired, bool completed)
    {
        this.title.text = title;
        this.description.text = description;
        this.image.sprite = image;
        this.progress.text = progress + "/" + amountRequired;
        status.SetActive(!completed);
    }
}
