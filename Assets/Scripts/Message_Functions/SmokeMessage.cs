using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeMessage : MessageFunction
{
    
    public GameObject smokePrefab;
    public float maxDistance = 5f;

    [SerializeField] int smokeNumberMax = 6;
    [SerializeField] int smokeNumber = 6;


    [SerializeField] float minScale = 0.05f;
    [SerializeField] float maxScale = 0.1f;



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

            smokeNumber += 1;
            if (smokeNumber <= smokeNumberMax)
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
            else if (smokeNumber >= smokeNumberMax)
            {
                smokeNumber = smokeNumberMax;
            }



            Debug.Log("LeaveScarできる回数: " + smokeNumber + "/" + smokeNumberMax);

            Debug.Log("PlayerID: " + PlayerManager.playerID);

        }

    }


    public override void OnActivate(Vector3 playerPosition)
    {
        Debug.Log("LeaveScar（メッセージを残す機能)を実行する");


        // メッセージUIなどから発動されたときの処理（例：周囲に印）
        RaycastHit hit;
        Vector3 rayOrigin = playerPosition + Vector3.up * 1f;
        Vector3 rayDir = transform.forward;

        smokeNumber = 0;

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
        Quaternion randomRoll = Quaternion.Euler(0f, 0f, Random.Range(0f, 15f));

        GameObject scratch = Instantiate(
            smokePrefab,
            hit.point + hit.normal * 0.01f,
            baseRot * randomRoll  // ランダム回転を合成
        );

        // スケールをランダムに（元のスケールを基準に倍率をかける）
        //float scale = Random.Range(minScale, maxScale);
        //scratch.transform.localScale *= scale;

        scratch.transform.SetParent(hit.collider.transform);
    }

    //public override void OnActivate(Vector3 playerPosition)
    //{
    //    var colors = ColorPaletteManager.Instance.GetColors();
    //    if (colors.Count == 0)
    //    {
    //        Debug.LogWarning("パレットに色が登録されていません！");
    //        return;
    //    }

    //    // 選ばれた感情（例：最後に追加された色）
    //    Color selectedColor = colors[colors.Count - 1];

    //    // スポーン位置（レイキャストでYを調整）
    //    Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : playerPosition;
    //    Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    //    if (Physics.Raycast(ray, out RaycastHit hit, 100f)) spawnPosition.y = hit.point.y;

    //    // 煙を生成＆色を設定
    //    GameObject smoke = Instantiate(smokePrefab, spawnPosition, Quaternion.identity);
    //    ParticleSystem ps = smoke.GetComponent<ParticleSystem>();
    //    if (ps != null)
    //    {
    //        var main = ps.main;
    //        main.startColor = selectedColor;

    //        var renderer = ps.GetComponent<ParticleSystemRenderer>();
    //        if (renderer != null)
    //        {
    //            renderer.material = new Material(renderer.material);
    //            renderer.material.SetColor("_Color", selectedColor);
    //        }

    //        ps.Clear();
    //        ps.Play();
    //    }
    //    else
    //    {
    //        Debug.LogWarning("煙に ParticleSystem が見つかりません！");
    //    }
    //}



}
