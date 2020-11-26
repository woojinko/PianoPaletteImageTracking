using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlacementCursorManager : MonoBehaviour
{
    [Header("Visual Items")]

    [SerializeField]
    [Tooltip("Prefab : Used to visualise where an object will be placed on planes - instanced by 'PlacementCursorManager'")]
    public GameObject placementCursorPrefab;


    [SerializeField] 
    public Pose placementCursorPose; // used as a way to expose the placement and position of the spot where they raycast touches a surface

    [SerializeField]
    public bool placementCursorIsSurface = false;

    [SerializeField]
    public bool visible = true;

    private ARRaycastManager arRaycastManager;

    private GameObject placementCursor;
    


    void Awake()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        placementCursor = Instantiate(placementCursorPrefab) as GameObject; 
    }

    void Update()
    {
        UpdateCursorPose();
        UpdateCursorIndicator();
    }

    private void UpdateCursorPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var arRaycastHits = new List<ARRaycastHit>();
        arRaycastManager.Raycast(screenCenter, arRaycastHits, TrackableType.Planes);
        placementCursorIsSurface = arRaycastHits.Count > 0;
        if (placementCursorIsSurface)
        {
            placementCursorPose = arRaycastHits[0].pose;
        }
    }

    private void UpdateCursorIndicator()
    {
        if (placementCursorIsSurface && visible)
        {
            placementCursor.SetActive(true);
            placementCursor.transform.SetPositionAndRotation(placementCursorPose.position, placementCursorPose.rotation);
        }
        else
        {
            placementCursor.SetActive(false);
        }
    }

    public void ToggleCursorActive()
    {
        // if (visible) {
        //     placementCursor.SetActive(false);
        //     visible = false;
        // }else {
        //     placementCursor.SetActive(true);
        //     placementCursor.transform.SetPositionAndRotation(placementCursorPose.position, placementCursorPose.rotation);
        //     visible = true;
        // }
        visible = !visible;
    }
}
