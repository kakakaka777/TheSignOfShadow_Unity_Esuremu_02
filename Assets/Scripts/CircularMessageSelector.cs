using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CircularMessageSelector : MonoBehaviour
{
    [System.Serializable]
    public struct Option
    {
        public string name;
        public MessageFunction function;
        public TextMeshProUGUI text;
    }

    public Option[] options;
    public Transform playerCenter; // プレイヤー位置
    [SerializeField] float radius = 2f;      // 円の半径
    private int selectedIndex = 0;

    [SerializeField] float rotateSpeed = 1.5f;

    void Start()
    {
    }

    void Update()
    {
        PositionUIInCircle();
        UpdateVisuals();

        if (Input.GetKeyDown(KeyCode.RightArrow)) { selectedIndex = (selectedIndex - 1 + options.Length) % options.Length; UpdateVisuals(); }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { selectedIndex = (selectedIndex + 1) % options.Length; UpdateVisuals(); }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Vector3 pos = playerCenter.position;
            options[selectedIndex].function.Activate(pos);
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }

        //if (Input.GetKeyDown(KeyCode.Tab)) { gameObject.SetActive(!gameObject.activeSelf); }


    }

    void PositionUIInCircle()
    {
        int total = options.Length;

        // 段構成ごとの最大数
        int lowerMax = 5;
        int middleMax = 5;
        int upperMax = 5;

        float baseRadius = radius;
        float rotateOffset = Time.time * rotateSpeed;

        // 各段の設定
        float[] yOffsets = { 0f, 0.6f, 1.2f };             // Y軸オフセット
        float[] radii = { baseRadius, baseRadius * 0.9f, baseRadius * 0.8f };

        for (int i = 0; i < total; i++)
        {
            // 段の判定
            int tier = (i < lowerMax) ? 0 : (i < lowerMax + middleMax) ? 1 : 2;
            int tierStartIndex = (tier == 0) ? 0 : (tier == 1) ? lowerMax : lowerMax + middleMax;
            int tierItemIndex = i - tierStartIndex;
            int tierItemCount = Mathf.Min((tier == 0) ? lowerMax : (tier == 1) ? middleMax : upperMax, total - tierStartIndex);

            // 円周上の位置計算
            float angle = tierItemIndex * Mathf.PI * 2f / tierItemCount + rotateOffset;
            float wave = Mathf.Sin(i * 0.5f + Time.time) * 0.05f;
            Vector3 offset = new Vector3(Mathf.Cos(angle), wave + yOffsets[tier], Mathf.Sin(angle)) * radii[tier];
            Vector3 worldPos = playerCenter.position + offset;

            // UI要素の配置とビジュアル更新
            options[i].text.transform.position = worldPos;
            options[i].text.transform.LookAt(playerCenter.position);
            options[i].text.transform.Rotate(0f, 180f, 0f);
            options[i].text.text = options[i].name;
            options[i].text.fontSize = 0.8f;
        }



    }

    void UpdateVisuals()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].text.color = (i == selectedIndex) ? Color.red : Color.white;
            float scale = 1f + Mathf.Sin(Time.time * 3f) * 0.1f; // 呼吸する感じ
            options[i].text.transform.localScale = (i == selectedIndex)
                ? Vector3.one * scale
                : Vector3.one;

        }
    }

}
