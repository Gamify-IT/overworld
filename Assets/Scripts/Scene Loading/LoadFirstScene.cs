using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFirstScene : MonoBehaviour
{
    public VectorValue startingPosition;
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("Player");
        //Here you can specify the starting World/Scene
        SceneManager.LoadScene("World 1", LoadSceneMode.Additive);
        //Add HUD over it
        SceneManager.LoadScene("Player HUD", LoadSceneMode.Additive);
        // Set the desired starting position
        startingPosition.initialValue = new Vector2(-15f, 41f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
