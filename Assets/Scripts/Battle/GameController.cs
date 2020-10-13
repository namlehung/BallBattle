using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    private static bool isFirstLaunch = true;
    void Start()
    {
        if(GameController.isFirstLaunch) 
        {
            //ARToggleButton.SetARstatus(SceneController.GotoARScene());
            GameController.isFirstLaunch = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

   
}
