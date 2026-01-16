using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeLimit : MonoBehaviour
{
    public float MaxTimer = 30f;
    private float CountDown = 30f;
    [SerializeField]
    private TextMeshProUGUI TimeOverText;
    [SerializeField] private string prefix = "êßå¿éûä‘ ";
    [SerializeField] GameObject messageSelectUI;




    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (UiManager.isTimeCountDown == false) return;

        if (UiManager.isTimeCountStart == true)
        {
            CountDown = MaxTimer;
            UiManager.isTimeCountStart = false;
        }


        CountDown -= Time.deltaTime;
        CountDown = Mathf.Clamp(CountDown, 0f, MaxTimer);
        //Debug.Log("isTimeCountDown : " + UiManager.isTimeCountDown);

        // éûä‘Çï\é¶Ç∑ÇÈ
        TimeOverText.text =  prefix + CountDown.ToString("f2");

        

        // countdownÇ™0à»â∫Ç…Ç»Ç¡ÇΩÇ∆Ç´
        if (CountDown <= 0)
        {

            CountDown = 0.00f;

            UiManager.isTimeCountDown = false;

            if (PlayerManager.isDamageOnlyOnce == true) return;

            TimeOverText.text = 0 + CountDown.ToString("f2");

            PlayerManager.isDamageOnlyOnce = true;
            messageSelectUI.SetActive(true);


        }
    }
}
