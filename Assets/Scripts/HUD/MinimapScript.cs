using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.Find("player");
        GameObject miniMapCam = GameObject.Find("Minimap Camera");
        miniMapCam.transform.SetParent(player.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
