using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WarningEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Animation anim =transform.GetComponent<Animation>();
        anim.Play();
        Destroy(gameObject,0.4f);
    }

    public void SetText(string text)
    {
        transform.GetComponentInChildren<TextMeshPro>().text = text;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
