using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeLimit : MonoBehaviour
{
    
    public float CountDown = 30f;
    [SerializeField]
    private TextMeshProUGUI TimeOverText;
    [SerializeField] private string prefix = "制限時間 ";
    [SerializeField] GameObject messageSelectUI;





    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        // 時間をカウントダウンする
        CountDown -= Time.deltaTime;

        // 時間を表示する
        TimeOverText.text =  prefix + CountDown.ToString("f0") + "秒";

        // countdownが0以下になったとき
        if (CountDown <= 0)
        {

            CountDown = 0f;

            if (PlayerManager.isDamageOnlyOnce == true) return;

            PlayerManager.isDamageOnlyOnce = true;
            messageSelectUI.SetActive(true);


        }
    }
}
