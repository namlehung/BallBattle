using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARToggleButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(GameController.isPlayInARMode)
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
    }

    public void AROff()
    {
       Debug.Log("-------------AR Off------------");
       GameController.gameControllerInstance.PauseGame(true);
       GameController.isPlayInARMode = false;
       SceneController.GotoMainScene();
    }
}
