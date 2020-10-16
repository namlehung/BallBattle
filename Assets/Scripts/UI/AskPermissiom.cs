using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AskPermissiom : MonoBehaviour
{
    private bool isNeedGoARMode;
    // Start is called before the first frame update
    void Start()
    {
        isNeedGoARMode = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isNeedGoARMode)
        {
            if(GamePermission.HasPermissionToUseAR())
            {
                SceneController.GotoARScene();
            }
            else
            {
                GameController.gameControllerInstance.PauseGame(false);
                isNeedGoARMode = false;
            }
        }
    }

    public void onPlayARbuttonClick()
    {
        GameController.gameControllerInstance.PauseGame(true);
        if(GamePermission.HasPermissionToUseAR())
        {
            SceneController.GotoARScene();
        }
        else
        {
            GamePermission.RequestPermission();
            StartCoroutine(WaitForPermission());
        }
    }

    IEnumerator WaitForPermission()
    {
        yield return new WaitForSeconds(0.5f);
        isNeedGoARMode = true;
    }
}
