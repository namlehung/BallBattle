using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyController : MonoBehaviour
{
    public float EnergyGeneratePerSec;
    public float MaxEnergy = 6.0f;
    private float currentEnergy;

    // Start is called before the first frame update
    private Slider fillbar;
    private Slider filltrans;
    void Start()
    {
        fillbar  = transform.GetComponent<Slider>();
        filltrans  = transform.GetChild(2).GetComponent<Slider>();
        currentEnergy = 0;
    }

    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }

    public bool SpentEnergy(float value)
    {
        if(value < currentEnergy)
        {
            currentEnergy -= value;
            return true;
        }
        return false;
    }
    public void ResetEnergy()
    {
        currentEnergy = 0;
    }
    // Update is called once per frame
    void Update()
    {
        currentEnergy += EnergyGeneratePerSec*Time.deltaTime;
        if(currentEnergy >= MaxEnergy)
        {
            currentEnergy = MaxEnergy;
        }
        SetFillBar();
    }
    public void SetFillBar()
    {
        float fillbarvalue = currentEnergy/MaxEnergy;
        //Debug.Log("set fill bar: " + fillbarvalue);
        fillbar.value = fillbarvalue;
        int energy = (int)(currentEnergy);
        filltrans.value = 1 - (energy < 3 ? energy*0.168f : energy*0.167f);
    }
}
