using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MultiImageTracker : MonoBehaviour
{
    [SerializeField] private List<GameObject> _prefabsToSpawn;

    private ARTrackedImageManager _trackedImageManager;

    private Dictionary<string, GameObject> _arObjects;

    void Awake()
    {
        _trackedImageManager = GetComponent<ARTrackedImageManager>();

        _arObjects = new Dictionary<string, GameObject>();

        _trackedImageManager.trackablesChanged.AddListener(OnImageTrackedChanged);

        SetupSceneObjects();
    }

    void OnDisable()
    {
        _trackedImageManager.trackablesChanged.RemoveListener(OnImageTrackedChanged);
    }

    private void OnImageTrackedChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateTrackedImage(trackedImage);
        }
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateTrackedImage(trackedImage);
        }
        foreach (var trackedImage in eventArgs.removed)
        {
            UpdateTrackedImage(trackedImage.Value);
        }
    }

    private void UpdateTrackedImage(ARTrackedImage trackedImage)
    {
        if(trackedImage == null)
        {
            return;
        }

        if(trackedImage.trackingState is TrackingState.Limited or TrackingState.None)
        {
            _arObjects[trackedImage.referenceImage.name].SetActive(false);
            return;
        }

        _arObjects[trackedImage.referenceImage.name].SetActive(true);
        _arObjects[trackedImage.referenceImage.name].transform.position = trackedImage.transform.position;
        //_arObjects[trackedImage.referenceImage.name].transform.rotation = trackedImage.transform.rotation;
    }

    void SetupSceneObjects()
    {
        foreach (var prefab in _prefabsToSpawn)
        {
            GameObject arObject = Instantiate(prefab, Vector3.zero, prefab.transform.rotation);
            arObject.name = prefab.name;
            arObject.SetActive(false);
            _arObjects.Add(arObject.name, arObject);
        }
    }
}
