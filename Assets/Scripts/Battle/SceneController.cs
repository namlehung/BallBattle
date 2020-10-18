using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static bool GotoARScene()
    {
        if(GamePermission.HasPermissionToUseAR())
        {
            Debug.Log("go to AR scene");
            GameController.gameControllerInstance.PrepareForARGame();
            SceneManager.LoadScene("AR");
            return true;
        }
        return false;
    }
    public static void GotoMainScene()
    {
        Debug.Log("go to Main scene");
        GameController.gameControllerInstance.BackToNoneARGame();
        SceneManager.LoadScene("Main");
    }
}
