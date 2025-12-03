using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAfterSecond1_Trap : MonoBehaviour
{
    [SerializeField] GameObject player;

    [Header("殺されるタグ")]
    [Tooltip("Player1,Player2など随時好きに追加してくれ")]
    [SerializeField] bool player1 = true;
    //[SerializeField] bool player2 = true;
    //[SerializeField] bool player3 = true;

    

    void OnTriggerEnter(Collider other)
    {
        if (player1 == true)
        {
            if (other.CompareTag("Player"))
            {
                

                // その Collider がアタッチされている GameObject を取得
                GameObject otherGameObject = other.gameObject;

                // 取得した GameObject を使って何か処理を行う
                Debug.Log("当たったオブジェクトの名前: " + otherGameObject.name);
            }
        }
        //else if (player2 == true)
        //{
        //    if (other.CompareTag("Player2") && !isDying)
        //    {

        //    }
        //}
        //else if (player3 == true)
        //{
        //    if (other.CompareTag("Player1") && !isDying)
        //    {

        //    }
        //}
    }



}
