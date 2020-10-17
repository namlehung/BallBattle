using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARToggleButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(GameController.gameControllerInstance.isPlayInARMode)
        {
            transform.GetComponent<ToggleButtonController>().SetToggle(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AROn()
    {
        Debug.Log("-------------AR On------------");
        if(GameController.gameControllerInstance.isPlayInARMode == false)
        {
            transform.GetComponent<ToggleButtonController>().SetToggle(false);
        }
    }

    public void AROff()
    {
       Debug.Log("-------------AR Off------------");
       SceneController.GotoMainScene();
    }

    public void PauseGame()
    {
        GameController.gameControllerInstance.PauseGame(true);
        transform.GetComponent<AskPermissiom>().AskAndGoARScene();
    }
}
