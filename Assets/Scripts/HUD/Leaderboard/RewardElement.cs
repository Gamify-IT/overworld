using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardElement : MonoBehaviour
{
    [SerializeField] private TMP_Text playername;
    [SerializeField] private TMP_Text reward;
   
    [SerializeField] private TMP_Text place; //platzierung anzeigen?
    

    public void Setup(string playername,  int reward, int place)
    {
        this.playername.text = playername.Replace("_", " ");
        this.reward.text = reward.ToString(); 
        this.place.text = place.ToString() ;
    }
}
