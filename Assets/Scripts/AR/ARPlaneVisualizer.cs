using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class ARPlaneVisualizer : MonoBehaviour
{
    public GameObject planeNormalPrefab;
    public GameObject planeVisualizePrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ARPlaneVisualizerOn()
    {
        Debug.Log("----------ARPlaneVisualizer On ------------");
        ARPlaneManager aRPlaneManager = GameObject.Find("AR Session Origin").GetComponent<ARPlaneManager>();
        aRPlaneManager.planePrefab = planeVisualizePrefab;
    }

    public void ARPlaneVisualizerOff()
    {
        Debug.Log("-----------ARPlaneVisualizer Off-----------");
        ARPlaneManager aRPlaneManager = GameObject.Find("AR Session Origin").GetComponent<ARPlaneManager>();
        aRPlaneManager.planePrefab = planeNormalPrefab;
    }
}
