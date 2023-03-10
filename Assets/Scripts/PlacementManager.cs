using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlacementManager : MonoBehaviour
{
    public GameObject prefab;

    public GameObject spawnedObject;

    public ARRaycastManager arRaycastManager;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public GameObject arCamera;

    // private Vector2 touchPosition = default;
    private bool prefabSet;

    public GameObject PlacementRotateButtons;

    // Start is called before the first frame update
    void Awake()
    {
        arRaycastManager =
            GameObject.Find("XROrigin").GetComponent<ARRaycastManager>();
        prefabSet = false;
        PlacementRotateButtons.SetActive(false);
        arCamera = GameObject.Find("MainCamera");
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    public void SetPrefab(GameObject newPrefab)
    {
        prefab = newPrefab;
        prefabSet = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (prefabSet)
        {
            if (!TryGetTouchPosition(out Vector2 touchPosition))
            {
                return;
            }
            if (
                arRaycastManager
                    .Raycast(touchPosition,
                    hits,
                    TrackableType.PlaneWithinPolygon)
            )
            {
                var hitPose = hits[0].pose;

                // if (touchPosition.y > 300)
                if (!IsPointerOverUIObject(Input.GetTouch(0)))
                {
                    if (spawnedObject == null)
                    {
                        spawnedObject =
                            Instantiate(prefab,
                            hitPose.position,
                            Quaternion.Euler(0, 180, 0));
                        PlacementRotateButtons.SetActive(true);
                        if (
                            GameObject
                                .Find("GameManager")
                                .GetComponent<GameManagerScript>()
                                .HapticFeedback
                        )
                        {
                            Handheld.Vibrate();
                        }
                    }
                    else
                    {
                        spawnedObject.transform.position = hitPose.position;
                    }
                }
            }
        }
    }

    private bool IsPointerOverUIObject(Touch touch)
    {
        PointerEventData eventDataCurrentPosition =
            new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position =
            new Vector2(touch.position.x, touch.position.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll (eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
