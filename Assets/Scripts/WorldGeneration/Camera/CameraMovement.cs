using UnityEngine;

/// <summary>
///     This class manages the camera for the generator and inspector gamemode, 
///     it can be moved by holding down the left mouse button and moving the mouse,
///     additionally, it can be zoomed in and out with the mouse wheel
/// </summary>
public class CameraMovement : MonoBehaviour
{
    [SerializeField] Camera cameraObject;

    //zoom speed and min/max zoom values
    [SerializeField] float zoomSpeed;
    [SerializeField] float minCameraSize;
    [SerializeField] float maxCameraSize;

    private bool active = false;
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

    /// <summary>
    ///     This funciton renables the camera movement
    /// </summary>
    public void Activate()
    {
        active = true;
    }

    /// <summary>
    ///     This funciton disables the camera movement
    /// </summary>
    public void Deactivate()
    {
        active = false;
    }

    /// <summary>
    ///     This funciton handles the camera movement
    /// </summary>
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

    /// <summary>
    ///     This function zooms the camera in on step, until the minimum size is reached
    /// </summary>
    private void ZoomIn()
    {
        float newSize = (1f - zoomSpeed) * cameraObject.orthographicSize;
        cameraObject.orthographicSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);
    }

    /// <summary>
    ///     This function zooms the camera out on step, until the maximum size is reached
    /// </summary>
    private void ZoomOut()
    {
        float newSize = (1f + zoomSpeed) * cameraObject.orthographicSize;
        cameraObject.orthographicSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);
    }
}
