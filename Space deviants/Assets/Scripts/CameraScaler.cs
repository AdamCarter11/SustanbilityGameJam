using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    void Start()
    {
        ScaleCameraToScreen();
    }

    void ScaleCameraToScreen()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }

        float targetAspect = 16f / 9f;  // You can adjust this based on your desired aspect ratio
        float currentAspect = (float)Screen.width / Screen.height;

        // Calculate the desired orthographic size
        float orthoSize = mainCamera.orthographicSize * (targetAspect / currentAspect);

        // Set the orthographic size
        mainCamera.orthographicSize = orthoSize;
    }
}
