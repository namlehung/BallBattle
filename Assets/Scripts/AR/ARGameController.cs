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
    private bool isStartGame;
    void Awake()
    {
        planeManager = GetComponent<ARPlaneManager>();
        planeManager.planesChanged += onPlaneDetection;
        _arRaycastManager = GetComponent<ARRaycastManager>();
        isStartGame = false;
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
        #if UNITY_EDITOR
        if(spawnedObject == null)
        {
            spawnedObject = Instantiate(gameObjectToInstantiate,new Vector3(0,-1,0),Quaternion.identity);
            spawnedObject.name = "Battle";
            GameController.isPlayInARMode = true;
            spawnedObject.transform.parent = null;
            spawnedObject.transform.localScale = Vector3.one*0.05f;
        } 
        else
        {
            //spawnedObject.transform.position = hitPose.position;
            //spawnedObject.transform.rotation = hitPose.rotation;
        }
        #endif

        if(detectedPlane != null)
        {
            Debug.Log("namlh debug Plane position: " + detectedPlane.transform.position);
            if(spawnedObject == null)
            {
                spawnedObject = Instantiate(gameObjectToInstantiate,detectedPlane.transform.position,detectedPlane.transform.rotation);
                spawnedObject.name = "Battle";
                GameController.isPlayInARMode = true;
                spawnedObject.transform.parent = null;
                spawnedObject.transform.localScale = Vector3.one*0.05f;
            } 
            else
            {
                spawnedObject.transform.position = detectedPlane.transform.position;
                //spawnedObject.transform.rotation = hitPose.rotation;
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

            if(isStartGame)
            {
                GameController.gameControllerInstance.SetARPointerDown(hitPose.position);
            }
        }
    }

    public void StartARGame()
    {
        if(spawnedObject == null)
            return;
        isStartGame = true;
        GameController.gameControllerInstance.PauseGame(false);
    }

    public void PauseARGame()
    {
         if(spawnedObject == null)
            return;
        isStartGame = false;
        GameController.gameControllerInstance.PauseGame(true);
    }
}
