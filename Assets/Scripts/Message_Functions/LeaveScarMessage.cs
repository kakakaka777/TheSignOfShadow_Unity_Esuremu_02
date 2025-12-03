using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveScarMessage : MessageFunction
{
    public GameObject scratchDecalPrefab;
    public float maxDistance = 5f;
    public Camera playerCamera;

    void Update()
    {
        // 左クリックされたら印を残す
        if (Input.GetKeyDown(KeyCode.I))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                CreateScratchMark(hit);
            }
        }
    }

    public override void Activate(Vector3 playerPosition)
    {
        // メッセージUIなどから発動されたときの処理（例：周囲に印）
        RaycastHit hit;
        Vector3 rayOrigin = playerPosition + Vector3.up * 1f;
        Vector3 rayDir = transform.forward;

        if (Physics.Raycast(rayOrigin, rayDir, out hit, maxDistance))
        {
            CreateScratchMark(hit);
        }
    }

    void CreateScratchMark(RaycastHit hit)
    {
        GameObject scratch = Instantiate(
            scratchDecalPrefab,
            hit.point + hit.normal * 0.01f,
            Quaternion.LookRotation(hit.normal)
        );

        scratch.transform.SetParent(hit.collider.transform);
    }
}

