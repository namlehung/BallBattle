using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotEnoughEnergy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Animation anim =transform.GetComponent<Animation>();
        anim.Play();
        Destroy(gameObject,0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
