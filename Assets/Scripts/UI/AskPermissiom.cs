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
        // if(isNeedGoARMode)
        // {
        //     isNeedGoARMode = false;
        //     if(GamePermission.HasPermissionToUseAR())
        //     {
        //         SceneController.GotoARScene();
        //     }
        //     else
        //     {
        //         GameController.gameControllerInstance.PauseGame(false);
        //         transform.GetComponent<ToggleButtonController>().SetToggle(false);
        //     }
        // }
    }

    public void AskAndGoARScene()
    {
        //GameController.gameControllerInstance.PauseGame(true);
        if(GamePermission.HasPermissionToUseAR())
        {
            SceneController.GotoARScene();
        }
        else
        {
            GamePermission.RequestPermission();
            isNeedGoARMode = true;
            //StartCoroutine(WaitForPermission());
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
    #if !UNITY_EDITOR
        if(hasFocus && isNeedGoARMode)
        {
            isNeedGoARMode = false;
            CheckResult();
        }
    #endif
    }

    private void CheckResult()
    {
        if(GamePermission.HasPermissionToUseAR())
        {
            SceneController.GotoARScene();
        }
        else
        {
            GameController.gameControllerInstance.PauseGame(false);
            transform.GetComponent<ToggleButtonController>().SetToggle(false);
        }
    }
    IEnumerator WaitForPermission()
    {
        yield return new WaitForSeconds(0.5f);
        isNeedGoARMode = true;
    }
}
