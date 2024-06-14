using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardElement : MonoBehaviour
{
    [SerializeField] private TMP_Text playername;
    [SerializeField] private TMP_Text reward;
   
    [SerializeField] private TMP_Text place; //platzierung anzeigen?
    

    public void Setup(string playername,  int reward, int score, int place, int number)
    {
        this.playername.text = playername.Replace("_", " ");
        this.reward.text = reward + "/" + score; //int kann ansonsten nicht als string gespeichert werden
        this.place.text = place + "/" + number;
    }
}
