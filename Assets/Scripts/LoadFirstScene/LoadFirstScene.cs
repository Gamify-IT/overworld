using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Here you can specify the starting World/Scene
        SceneManager.LoadScene("Area 1");
        //Add HUD over it
        SceneManager.LoadScene("Player HUD", LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
