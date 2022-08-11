using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script triggers the loading if a player enters an area
//usage: world: worldNumber = worldIndex, dungeonNumber = 0
//       dungeon: worldNumber = worldIndex, dungeonNumber = dungeonIndex
public class EnterArea : MonoBehaviour
{
    [SerializeField] private int worldNumber;
    [SerializeField] private int dungeonNumber;

    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.tag.Equals("Player"))
        {
            Invoke("triggerLoading", 3f);
        }
    }

    private void triggerLoading()
    {
        GameManager.instance.loadWorld(worldNumber, dungeonNumber);
    }
}
