using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * this script starts the Game. It loads the defined scenes, with the defined overlays (HUD).
 */
public class LoadFirstScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //always load Player Scene
        SceneManager.LoadScene("Player");
        //Here you can specify the starting World/Scene
        SceneManager.LoadScene("World 1", LoadSceneMode.Additive);
        //Add HUD over it
        SceneManager.LoadScene("Player HUD", LoadSceneMode.Additive);
    }
}