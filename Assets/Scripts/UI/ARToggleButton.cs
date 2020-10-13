using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ARToggleButton : MonoBehaviour
{
    // Start is called before the first frame update
    static bool isAROn = false;
    private Animator animatorBtnToggle;

    private bool isAnimationEnd;

    void Start()
    {
        animatorBtnToggle = transform.GetComponentInChildren<Animator>();
        animatorBtnToggle.speed = 10;
        //Debug.Log("start AR toggle button: ");
        if(SceneManager.GetActiveScene().name == "AR")
        {
            isAROn = true;
            animatorBtnToggle.SetBool("IsON",isAROn);
        }
        isAnimationEnd = true;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void onButtonARToggleClick()
    {
        if(isAnimationEnd)
        {
            isAnimationEnd = false;
            animatorBtnToggle.speed = 1.0f;
            isAROn = !isAROn;
            animatorBtnToggle.SetBool("IsON",isAROn);
            StartCoroutine(EndButtonToggleAnim());
        }
    }

    public void SetARstatus(bool isON)
    {
        isAROn = isON;
    }

    public IEnumerator EndButtonToggleAnim()
    {
        yield return new WaitForSecondsRealtime(animatorBtnToggle.speed);

        isAnimationEnd = true;
        if(isAROn == false)
        {
            SceneController.GotoMainScene();
        }
        else
        {
            SceneController.GotoARScene();
        }
    }
}
