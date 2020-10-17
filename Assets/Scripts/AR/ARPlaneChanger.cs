using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class ARPlaneChanger : MonoBehaviour
{
    public Material[] materials;
    private int pos = 0;
    public MeshRenderer currentGroundPlane;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextPlane()
    {
        pos = pos +1;
        if(pos >= materials.Length)
        {
            pos = 0;
        }
        MeshRenderer[] meshRenderers = GameObject.Find("AR Session Origin").GetComponent<ARPlaneManager>().GetComponentsInChildren<MeshRenderer>();
       for(int i  = 0;i<meshRenderers.Length;i++)
        {
            meshRenderers[i].material = materials[pos];
        }
        LineRenderer[] lineRenderers = GameObject.Find("AR Session Origin").GetComponent<ARPlaneManager>().GetComponentsInChildren<LineRenderer>();
        for(int j = 0;j<lineRenderers.Length;j++)
        {
            lineRenderers[j].enabled = (pos == 0);
        }
        currentGroundPlane.material = materials[pos];
    }
}
