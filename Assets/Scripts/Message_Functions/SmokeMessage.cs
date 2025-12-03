using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeMessage : MessageFunction
{
    [SerializeField] GameObject smokePrefab;
    [SerializeField] Transform spawnPoint; // プレイヤーから取得 or 外部設定

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Activate(transform.position); // 自身の位置を起点に煙を発動
        }
    }


    public override void Activate(Vector3 playerPosition)
    {
        var colors = ColorPaletteManager.Instance.GetColors();
        if (colors.Count == 0)
        {
            Debug.LogWarning("パレットに色が登録されていません！");
            return;
        }

        // 選ばれた感情（例：最後に追加された色）
        Color selectedColor = colors[colors.Count - 1];

        // スポーン位置（レイキャストでYを調整）
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : playerPosition;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 100f)) spawnPosition.y = hit.point.y;

        // 煙を生成＆色を設定
        GameObject smoke = Instantiate(smokePrefab, spawnPosition, Quaternion.identity);
        ParticleSystem ps = smoke.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.startColor = selectedColor;

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.material = new Material(renderer.material);
                renderer.material.SetColor("_Color", selectedColor);
            }

            ps.Clear();
            ps.Play();
        }
        else
        {
            Debug.LogWarning("煙に ParticleSystem が見つかりません！");
        }
    }


}
