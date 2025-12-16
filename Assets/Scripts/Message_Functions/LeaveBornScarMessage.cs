using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveBornScarMessage :MessageFunction
{
    public GameObject scratchDecalPrefab;
    public float maxDistance = 5f;

    [SerializeField] int bornScarNumberMax = 1;


    public Camera playerCamera;
    [SerializeField] Camera playerCamera_02;

    private int bornScarNumber = 0;

    void Update()
    {

        if (canUse == false)
        {
            return;
        }

        // 左クリックされたら印を残す
        // 回数制限必要っぽいな
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;

            bornScarNumber += 1;
            if (bornScarNumber <= bornScarNumberMax)
            {
                if (PlayerManager.playerID == 0)
                {
                    Ray ray01 = playerCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray01, out hit, maxDistance))
                    {
                        CreateScratchMark(hit);
                    }
                }
                if (PlayerManager.playerID == 1)
                {
                    Ray ray02 = playerCamera_02.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray02, out hit, maxDistance))
                    {
                        CreateScratchMark(hit);
                    }
                }
            }


            Debug.Log("LeaveBornScar（焼死体）できる回数: " + bornScarNumber + "/" + bornScarNumberMax);

            Debug.Log("PlayerID: " + PlayerManager.playerID);

        }

        // 左クリックされたら印を残す
        if (Input.GetKeyDown(KeyCode.O))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                CreateScratchMark(hit);
            }
        }
    }

    public override void OnActivate(Vector3 playerPosition)
    {
        bornScarNumber = 0;

        Debug.Log("焼死体（メッセージを残す機能)を実行する");
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
        

        // 表面に正対する回転
        Quaternion baseRot = Quaternion.LookRotation(hit.normal);

        // 法線方向（前方向）に対してランダムにひねる
        Quaternion randomRoll = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        GameObject scratch = Instantiate(
            scratchDecalPrefab,
            hit.point + hit.normal * 0.01f,
            baseRot * randomRoll  // ランダム回転を合成
        );

        // スケールをランダムに（元のスケールを基準に倍率をかける）
        //float scale = Random.Range(minScale, maxScale);
        //scratch.transform.localScale *= scale;

        scratch.transform.SetParent(hit.collider.transform);

       
    }
}
