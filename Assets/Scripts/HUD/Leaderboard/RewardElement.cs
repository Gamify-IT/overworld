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
    [SerializeField] private TMP_Text place;

    /// <summary>
    /// Sets up the reward element with the player's name, reward amount, rank, and icons based on their position.
    /// </summary>
    /// <param name="playername">The name of the player.</param>
    /// <param name="reward">The reward amount the player receives.</param>
    /// <param name="place">The rank or place of the player (1st, 2nd, 3rd, etc.).</param>
    /// <param name="showCrown">Whether to show a crown icon based on the player's rank.</param>
    public void Setup(string playername, int reward, int place, bool showCrown)  
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
