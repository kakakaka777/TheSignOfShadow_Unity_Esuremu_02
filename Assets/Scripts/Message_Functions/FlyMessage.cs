using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMessage : MessageFunction
{
    public GameObject flyPrefab;
    


    [SerializeField] int flyKabashirasNumberMax = 6;

    
    public float maxDistance = 5f;

   


    private int flyKabashirasNumber = 0;

    private void Update()
    {
        if (canUse == false)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;

            flyKabashirasNumber += 1;
            if (flyKabashirasNumber <= flyKabashirasNumberMax)
            {
                if (PlayerManager.playerID == 0)
                {
                    Ray ray01 = player01_FPCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray01, out hit, maxDistance))
                    {
                        CreateScratchMark(hit);
                    }
                }
                if (PlayerManager.playerID == 1)
                {
                    Ray ray02 = player02_FPCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray02, out hit, maxDistance))
                    {
                        CreateScratchMark(hit);
                    }
                }
            }
            else if (flyKabashirasNumber >= flyKabashirasNumberMax)
            {
                flyKabashirasNumber = flyKabashirasNumberMax;

                
            }

            Debug.Log("Release the Haeできる回数: " + flyKabashirasNumber + "/" + flyKabashirasNumberMax);

            Debug.Log("PlayerID: " + PlayerManager.playerID);

        }
       

    }

    public override void OnActivate(Vector3 playerPosition)
    {




        // メッセージUIなどから発動されたときの処理（例：周囲に印）
        RaycastHit hit;
        Vector3 rayOrigin = playerPosition + Vector3.up * 1f;
        Vector3 rayDir = transform.forward;

        flyKabashirasNumber = 0;


        if (Physics.Raycast(rayOrigin, rayDir, out hit, maxDistance))
        {
            CreateScratchMark(hit);
        }

        Debug.Log("蝿を放つ（メッセージを残す機能)を実行する"); 
        

        

    }

    void CreateScratchMark(RaycastHit hit)
    {
        // 表面に正対する回転
        Quaternion baseRot = Quaternion.LookRotation(hit.normal);

        // 法線方向（前方向）に対してランダムにひねる
        Quaternion randomRoll = Quaternion.Euler(0f, 0f, Random.Range(0f, 15f));

        GameObject scratch = Instantiate(
            flyPrefab,
            hit.point + hit.normal * 0.01f,
            baseRot * randomRoll  // ランダム回転を合成
        );

        // スケールをランダムに（元のスケールを基準に倍率をかける）
        //float scale = Random.Range(minScale, maxScale);
        //scratch.transform.localScale *= scale;

        scratch.transform.SetParent(hit.collider.transform);
    }
}
