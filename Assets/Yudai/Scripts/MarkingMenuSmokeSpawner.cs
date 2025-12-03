using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkingMenuSmokeSpawner : MonoBehaviour
{
    public KeyCode markingKey = KeyCode.Q;
    public GameObject smokePrefab;
    public Transform spawnPoint;
    public MarkingMenuUI markingMenuUI;

    private int selectedIndex = 0;
    private bool isMarking = false;
    private Vector2 markingMouseStart;

    void Update()
    {
        if (Input.GetKeyDown(markingKey))
        {
            isMarking = true;
            markingMouseStart = Input.mousePosition;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            var colors = ColorPaletteManager.Instance.GetColors();
            if (colors.Count > 0)
                markingMenuUI.ShowMenu(colors, selectedIndex);
        }
        else if (Input.GetKeyUp(markingKey))
        {
            isMarking = false;

            var colors = ColorPaletteManager.Instance.GetColors();
            if (colors.Count > 0 && selectedIndex >= 0 && selectedIndex < colors.Count)
                SpawnSmoke(colors[selectedIndex]);

            markingMenuUI.HideMenu();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (isMarking)
        {
            UpdateSelectionByMouseDelta();
        }
    }

    void UpdateSelectionByMouseDelta()
    {
        var colors = ColorPaletteManager.Instance.GetColors();
        if (colors.Count == 0) return;

        Vector2 delta = (Vector2)Input.mousePosition - markingMouseStart;
        if (delta.magnitude < 10f) return;

        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        angle = (angle + 360f) % 360f;

        int sectorCount = colors.Count;
        selectedIndex = Mathf.FloorToInt(angle / (360f / sectorCount));
        selectedIndex = Mathf.Clamp(selectedIndex, 0, sectorCount - 1);

        markingMenuUI.ShowMenu(colors, selectedIndex);
        Debug.Log($"角度: {angle:F1}° → インデックス: {selectedIndex}, 色: {colors[selectedIndex]}");
    }

    void SpawnSmoke(Color color)
    {
        Vector3 spawnPosition = spawnPoint.position;

        // カメラ中央からレイキャスト
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            // Y座標だけ更新（X/ZはspawnPointの位置）
            spawnPosition.y = hit.point.y;
        }

        GameObject smoke = Instantiate(smokePrefab, spawnPosition, Quaternion.identity);

        ParticleSystem ps = smoke.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.startColor = new ParticleSystem.MinMaxGradient(color);

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.material = new Material(renderer.material);
                renderer.material.SetColor("_Color", color);
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