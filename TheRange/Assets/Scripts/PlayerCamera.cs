using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerCamera : MonoBehaviour
{
    [Header("--Settings--")]
    [Tooltip("Sensitivity of the camera movement")]
    public float sensitivity = 3f;
    public float maxRotX = 45f;
    public float minRotX = -55;
    public float bodyRotationSpeed = 1f;

    [Header("--Player reference--")]
    public GameObject player;  
    public GameObject arms;  

    [HideInInspector]
    public float rotCamX;
    [HideInInspector]
    public float rotCamY;

    private void Awake()
    {
        HideCursor();
    }

    private void LateUpdate()
    {
        CameraPos();
    }

    void CameraPos()
    {
        rotCamY = Input.GetAxis("Mouse X") * sensitivity;
        rotCamX += Input.GetAxis("Mouse Y") * sensitivity;

        //Clamping the rotX value
        rotCamX = Mathf.Clamp(rotCamX, minRotX, maxRotX);

        //calculate rotations
        var cameraRotation = new Vector3(-rotCamX, transform.eulerAngles.y, 0);
        var bodyRotation = new Vector3(0, transform.eulerAngles.y + rotCamY, 0);
        transform.eulerAngles = cameraRotation;

        //rotate player body
        //player.transform.eulerAngles = Vector3.Lerp(player.transform.eulerAngles, bodyRotation, Time.deltaTime * bodyRotationSpeed);
        player.transform.eulerAngles = bodyRotation;
    }

    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

}
