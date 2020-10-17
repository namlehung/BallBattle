using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(ARRaycastManager))]
public class ARGameController : MonoBehaviour
{

    public GameObject gameObjectToInstantiate;
    //public TextMeshProUGUI debugText;
    private GameObject spawnedObject;
    private ARRaycastManager _arRaycastManager;
    private Vector2 touchPosition;

    ARPlaneManager planeManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private ARPlane detectedPlane;

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
        if(detectedPlane == null)
        {
            if(updatePlane.Count>0)
            {
                detectedPlane = updatePlane[0];
            }
        }
        else
        {
            foreach(ARPlane arplane in updatePlane)
            {
                if(detectedPlane == arplane)
                {

                }
                else
                {
                    arplane.gameObject.SetActive(false);
                }
            }
        }
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
        detectedPlane = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(detectedPlane != null)
        {
            //Debug.Log("namlh debug Plane position: " + detectedPlane.transform.position);
            if(GameController.gameControllerInstance.IsGamePause() == true)
            {
                GameController.gameControllerInstance.SetUpARForBattle(detectedPlane.transform.position,detectedPlane.transform.rotation,detectedPlane.transform.localScale);
            }
        }
        if(GameController.gameControllerInstance.IsGamePause() == false)
        {
            if(planeManager.enabled)
            {
                planeManager.enabled = false;
            }
        }
        else
        {
            if(planeManager.enabled == false)
            {
                planeManager.enabled = true;
            }
        }
        //debugText.text = "AR Session State: " + ARSession.state;
        if(!TryGetTouchPosition(out Vector2 touchPosition))
            return;
        
        if(_arRaycastManager.Raycast(touchPosition,hits,TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            //hitPose.position.y = 0;
            //Instantiate(gameObjectToInstantiate,hitPose.position,hitPose.rotation);
            Debug.Log("namlh debug tap on plane: " + hitPose.position);

            GameController.gameControllerInstance.SetARPointerDown(hitPose.position);
        }
    }
}
