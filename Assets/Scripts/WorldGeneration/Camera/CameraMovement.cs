using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Camera cameraObject;

    [SerializeField] float zoomSpeed;
    [SerializeField] float minCameraSize;
    [SerializeField] float maxCameraSize;

    private bool active = true;
    private Vector3 dragOrigin;

    // Update is called once per frame
    void Update()
    {
        if(!active)
        {
            return;
        }

        PanCamera();

        if(Input.mouseScrollDelta.y > 0)
        {
            ZoomIn();
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            ZoomOut();
        }
    }

    public void Activate()
    {
        active = true;
    }

    public void Deactivate()
    {
        active = false;
    }

    private void PanCamera()
    {
        //save position where drag started
        if(Input.GetMouseButtonDown(0))
        {
            dragOrigin = cameraObject.ScreenToWorldPoint(Input.mousePosition);
        }

        if(Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - cameraObject.ScreenToWorldPoint(Input.mousePosition);

            cameraObject.transform.position += difference;
        }
    }

    private void ZoomIn()
    {
        float newSize = (1f - zoomSpeed) * cameraObject.orthographicSize;
        cameraObject.orthographicSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);
    }

    private void ZoomOut()
    {
        float newSize = (1f + zoomSpeed) * cameraObject.orthographicSize;
        cameraObject.orthographicSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);
    }
}
