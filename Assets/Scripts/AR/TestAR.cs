using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(ARRaycastManager))]
public class TestAR : MonoBehaviour
{

    public GameObject gameObjectToInstantiate;
    public TextMeshProUGUI debugText;
    private GameObject spawnedObject;
    private ARRaycastManager _arRaycastManager;
    private Vector2 touchPosition;

    ARPlaneManager planeManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    void Awake()
    {
        planeManager = GetComponent<ARPlaneManager>();
        planeManager.planesChanged += onPlaneDetection;
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }

    void onPlaneDetection(ARPlanesChangedEventArgs list)
    {
        List<ARPlane> newPlane = list.added;
        List<ARPlane> lostPlane = list.removed;
        List<ARPlane> updatePlane = list.updated;
        //debugText.text = "newPlane " + newPlane.Count + " lostPlane: " + lostPlane.Count + " updatePlane: " + updatePlane.Count;
    }
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        debugText.text = "AR Session State: " + ARSession.state;
        if(!TryGetTouchPosition(out Vector2 touchPosition))
            return;
        
        if(_arRaycastManager.Raycast(touchPosition,hits,TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            hitPose.position.y = 0;
            //Instantiate(gameObjectToInstantiate,hitPose.position,hitPose.rotation);
            if(spawnedObject == null)
            {
                 spawnedObject = Instantiate(gameObjectToInstantiate,hitPose.position,hitPose.rotation);
            } 
            else
            {
                spawnedObject.transform.position = hitPose.position;
                //spawnedObject.transform.rotation = hitPose.rotation;
            }
        }
    }
}
