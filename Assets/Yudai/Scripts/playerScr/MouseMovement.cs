using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{

    public float mouseSensitivity = 100f;

    float xRotation = 0f;
    float YRotation = 0f;

    void Start()
    {
        //Locking the cursor to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //control rotation around x axis (Look up and down)
        xRotation -= mouseY;

        //we clamp the rotation so we cant Over-rotate (like in real life)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //control rotation around y axis (Look up and down)
        YRotation += mouseX;

        //applying both rotations
        transform.localRotation = Quaternion.Euler(xRotation, YRotation, 0f);

        // マーキングメニュー中は視点を動かさない
        if (Input.GetKey(KeyCode.Q)) return;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        YRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, YRotation, 0f);

        // UI操作中はマウス無視（カーソルがロックされてないとき）
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        YRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, YRotation, 0f);
    }
}