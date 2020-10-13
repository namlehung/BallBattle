using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameController : MonoBehaviour
{
    public int GameTimePerMatch;
    private static bool isFirstLaunch = true;
    private static bool isGamePause = false;

    private float totalTime;
    private float timeLimit;
    private TextMeshProUGUI timeRemain;
    private TextMeshProUGUI txtGamePause;
    // Start is called before the first frame update
    void Start()
    {
        if(GameController.isFirstLaunch) 
        {
            GameController.isFirstLaunch = false;
        }
        totalTime = 0;
        timeLimit = GameTimePerMatch;
        timeRemain = GameObject.Find("Canvas/Game_UI/Time/Timeremain").GetComponentInChildren<TextMeshProUGUI>();
        txtGamePause = GameObject.Find("Canvas/Game_UI/txtGamePause").GetComponent<TextMeshProUGUI>();
        txtGamePause.gameObject.SetActive(false);
    }

    public void CheckAndGotoAR()
    {
    #if PLATFORM_ANDROID
        if(GamePermission.HasPermissionToUseAR())
        {
            SceneController.GotoARScene();
        }
    #endif
    }
    void OnApplicationFocus(bool hasFocus)
    {
        PauseGame(!hasFocus);
    }

    // Update is called once per frame
    void Update()
    {
        if(isGamePause)
            return;

        totalTime += Time.deltaTime;
        timeLimit -= Time.deltaTime;
        timeRemain.text = Mathf.RoundToInt(timeLimit) + "s";
    }

    public static bool IsGamePause()
    {
        return isGamePause;
    }
   public void PauseGame(bool pause)
   {
       if(isGamePause != pause)
       {
           isGamePause = pause;
           if(isGamePause)
            {
                txtGamePause.gameObject.SetActive(true);
                Time.timeScale = 0;
                AudioListener.pause = true;
            }
            else
            {
                txtGamePause.gameObject.SetActive(false);
                Time.timeScale = 1;
                AudioListener.pause = false;
            }
       }
   }
}
