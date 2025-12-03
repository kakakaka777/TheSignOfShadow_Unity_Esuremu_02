using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelToggle : MonoBehaviour
{
    public GameObject panel; // ColorPickerPanelなど

    private bool isOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) // Cキーで開閉切り替え
        {
            isOpen = !isOpen;
            panel.SetActive(isOpen);

            // マウスのロック・カーソル表示切り替え
            if (isOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}