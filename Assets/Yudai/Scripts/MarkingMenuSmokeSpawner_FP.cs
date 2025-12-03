using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkingMenuSmokeSpawner_FP : MonoBehaviour
{
    public KeyCode markingKey = KeyCode.Q;
    public GameObject smokePrefab;
    public Transform spawnPoint;
    public Transform cameraTransform; // ← 視線の基準になるカメラ

    public float markingSensitivity = 0.5f; // 方向検出用感度

    private List<Color> colorOptions = new List<Color>()
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow
    };

    private int selectedIndex = 0;
    private bool isMarking = false;
    private Vector2 markingMouseStart;

    void Update()
    {
        if (Input.GetKeyDown(markingKey))
        {
            isMarking = true;
            markingMouseStart = Input.mousePosition;

            // 一時的にマウスロック解除（視点操作停止）
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetKeyUp(markingKey))
        {
            isMarking = false;

            SpawnSmoke(colorOptions[selectedIndex]);

            // ロックを戻す
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
        Vector2 currentMousePos = Input.mousePosition;
        Vector2 delta = currentMousePos - markingMouseStart;

        if (delta.magnitude < 10f)
            return; // 微小な動きは無視

        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        angle = (angle + 360f) % 360f;

        int sectorCount = colorOptions.Count;
        selectedIndex = Mathf.FloorToInt(angle / (360f / sectorCount));

        Debug.Log($"[MarkingMenu] Angle: {angle}°, Index: {selectedIndex}, Color: {colorOptions[selectedIndex]}");
    }

    void SpawnSmoke(Color color)
    {
        Debug.Log("煙を出します！ 色：" + color);

        GameObject smoke = Instantiate(smokePrefab, spawnPoint.position, Quaternion.identity);
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
    }
}
