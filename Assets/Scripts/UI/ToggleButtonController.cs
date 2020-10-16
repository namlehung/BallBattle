using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonController : MonoBehaviour
{
    public GameObject targetGo;
    public string functionOn;
    public string functionOff;
    public bool IsToggleOn = false;
    private Animator animatorBtnToggle;

    private bool isAnimationEnd;

    // Start is called before the first frame update
    void Start()
    {
        animatorBtnToggle = transform.GetComponentInChildren<Animator>();
        animatorBtnToggle.speed = 10;
        Button button = transform.GetComponentInChildren<Button>();
        button.onClick.AddListener(onButtonARToggleClick); 
        //Debug.Log("start AR toggle button: ");
        animatorBtnToggle.SetBool("IsON",IsToggleOn);
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
            IsToggleOn = !IsToggleOn;
            animatorBtnToggle.SetBool("IsON",IsToggleOn);
            StartCoroutine(EndButtonToggleAnim());
        }
    }

    public void SetToggle(bool isON)
    {
        IsToggleOn = isON;
        if(animatorBtnToggle)
        {
            animatorBtnToggle.SetBool("IsON",IsToggleOn);
        }
        isAnimationEnd = true;
    }

    public IEnumerator EndButtonToggleAnim()
    {
        yield return new WaitForSecondsRealtime(animatorBtnToggle.speed);

        isAnimationEnd = true;
        if(IsToggleOn == false)
        {
            targetGo.SendMessage(functionOff);
        }
        else
        {
            targetGo.SendMessage(functionOn);
        }
    }
}
