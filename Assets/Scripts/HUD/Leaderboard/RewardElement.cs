using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardElement : MonoBehaviour
{
    [SerializeField] private TMP_Text playername;
    [SerializeField] private TMP_Text reward;
    [SerializeField] private GameObject crownIcon;
    [SerializeField] private GameObject silverIcon;
    [SerializeField] private GameObject bronzeIcon;
    [SerializeField] private TMP_Text place; //platzierung anzeigen?


    public void Setup(string playername, int reward, int place, bool showCrown)  //, bool showSilver)
    {
        this.playername.text = playername.Replace("_", " ");
        this.reward.text = reward.ToString();
        this.place.text = place.ToString();
        if (showCrown && place == 1)
        {
            crownIcon.gameObject.SetActive(showCrown);
        } else if (showCrown && place == 2) { 
        silverIcon.gameObject.SetActive(showCrown); }
        else
        {
            bronzeIcon.gameObject.SetActive(showCrown);
        }
       
    }
  
}