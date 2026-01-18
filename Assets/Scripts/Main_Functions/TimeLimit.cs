using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeLimit : MonoBehaviour
{
    [Header("Timer")]
    public float MaxTimer = 30f;
    private float CountDown = 30f;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI topText;        // 上の通常テキスト
    [SerializeField] private TextMeshProUGUI reflectText;    // 下の反転テキスト（反射）

    [Header("UI")]
    [SerializeField] private GameObject messageSelectUI;

    void Update()
    {
        if (UiManager.isTimeCountDown == false) return;

        // スタート指示が来た瞬間に 30:00 をまず表示（いきなり 29:99 にならない）
        if (UiManager.isTimeCountStart == true)
        {
            CountDown = MaxTimer;
            UiManager.isTimeCountStart = false;

            SetTimeText(CountDown);
            return;
        }

        CountDown -= Time.deltaTime;
        if (CountDown <= 0f)
        {
            CountDown = 0f;
            SetTimeText(0f);
            UiManager.isTimeCountDown = false;

            if (PlayerManager.isDamageOnlyOnce) return;

            PlayerManager.isDamageOnlyOnce = true;
            if (messageSelectUI) messageSelectUI.SetActive(true);
            return;
        }

        SetTimeText(CountDown);
    }

    void SetTimeText(float time)
    {
        // 秒:1/100秒 表示（30秒なら 30:00）
        int totalCenti = Mathf.FloorToInt(time * 100f);
        int sec = totalCenti / 100;
        int centi = totalCenti % 100;

        string s = $"{sec:0}:{centi:00}";

        if (CountDown <= 0)
        {
            s = $"{sec:00}:{centi:00}";

        }


        if (topText) topText.text = s;
        if (reflectText) reflectText.text = s;
    }
}

