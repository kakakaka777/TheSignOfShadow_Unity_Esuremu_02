using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject Player2;      // 切り替えるプレイヤー
    [SerializeField] GameObject Player1;   // 今のプレイヤー（消す対象）
    [SerializeField] GameObject UI;   // 今のプレイヤー（消す対象）
    [SerializeField] GameObject Biolear;
    [SerializeField] GameObject Biolea2r;
    [SerializeField] Transform startPoint;

    [SerializeField] MessageFunction[] messageFunctions; //ドアを通ったあと、メッセージ機能を封印したいため

    private void Start()
    {
       

    }

    private void Update()
    {
        // タグ "DeadBody" が付いたすべてのオブジェクトを削除
        GameObject[] allDeadBodies = GameObject.FindGameObjectsWithTag("DeadBody");
        foreach (GameObject ghost in allDeadBodies)
        {
            if (ghost != null)
            {
                Destroy(ghost);

            }
            else return;



            //Debug.Log($"死体オブジェクトを {allDeadBodies.Length} 個削除しました");
        }

    }
    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player") || other.CompareTag("Player1"))
        {
            if (messageFunctions != null)
            {
                foreach (var mf in messageFunctions)
                {
                    if (mf != null)
                    {
                        mf.canUse = false;   // ← ここで封印
                    }
                }
            }
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("ドアに触れた！プレイヤー交代するよ");
        
            // 今のプレイヤーを非表示に
            if (Player1 != null) Player1.SetActive(false);

            // 次プレイヤーを表示
            if (Player2 != null) Player2.SetActive(true);
            Player2.transform.position = startPoint.position;

            PlayerManager.playerID = 1;

            if (Player2 != null) UI.SetActive(false);

            //if (nextPlayer != null) Biolea2r.SetActive(true);
            //if (nextPlayer != null) Biolear.SetActive(true);
            

            // このドアを消す
            Destroy(this.gameObject);
        }
        else if (other.CompareTag("Player1"))
        {
            Debug.Log("ドアに触れた！プレイヤー交代するよ");


           


            // 今のプレイヤーを非表示に
            if (Player1 != null) Player2.SetActive(false);

            // 次プレイヤーを表示
            if (Player2 != null) Player1.SetActive(true);
            Player1.transform.position = startPoint.position;

            PlayerManager.playerID = 0;

            if (Player2 != null) UI.SetActive(false);

            //if (nextPlayer != null) Biolea2r.SetActive(true);
            //if (nextPlayer != null) Biolear.SetActive(true);


            // このドアを消す
            Destroy(this.gameObject);
        }
        //else if (other.CompareTag("Player2"))
        //{
        //    Debug.Log("ドアに触れた！プレイヤー交代するよ");


        //    PlayerID.playerID = 1;


        //    // 今のプレイヤーを非表示に
        //    if (currentPlayer != null) nextPlayer.SetActive(false);

        //    // 次プレイヤーを表示
        //    if (nextPlayer != null) currentPlayer.SetActive(true);
        //    currentPlayer.transform.position = startPoint.position;

        //    if (nextPlayer != null) UI.SetActive(false);

        //    //if (nextPlayer != null) Biolea2r.SetActive(true);
        //    //if (nextPlayer != null) Biolear.SetActive(true);


        //    // このドアを消す
        //    Destroy(this.gameObject);
        //}
    }

}
