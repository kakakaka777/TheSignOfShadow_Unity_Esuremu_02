using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject nextPlayer;      // 切り替えるプレイヤー
    [SerializeField] GameObject currentPlayer;   // 今のプレイヤー（消す対象）
    [SerializeField] GameObject UI;   // 今のプレイヤー（消す対象）
    [SerializeField] GameObject Biolear;
    [SerializeField] GameObject Biolea2r;
    [SerializeField] Transform startPoint;

    private void Start()
    {
       

    }

    private void Update()
    {
        // タグ "DeadBody" が付いたすべてのオブジェクトを削除
        GameObject[] allDeadBodies = GameObject.FindGameObjectsWithTag("DeadBody");
        foreach (GameObject ghost in allDeadBodies)
        {
            Destroy(ghost);
        }
        Debug.Log($"死体オブジェクトを {allDeadBodies.Length} 個削除しました");
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("ドアに触れた！プレイヤー交代するよ");

            



            // 今のプレイヤーを非表示に
            if (currentPlayer != null) currentPlayer.SetActive(false);

            // 次プレイヤーを表示
            if (nextPlayer != null) nextPlayer.SetActive(true);
            nextPlayer.transform.position = startPoint.position;


            if (nextPlayer != null) UI.SetActive(false);

            //if (nextPlayer != null) Biolea2r.SetActive(true);
            //if (nextPlayer != null) Biolear.SetActive(true);
            

            // このドアを消す
            Destroy(this.gameObject);
        }
        if (other.CompareTag("Player1"))
        {
            Debug.Log("ドアに触れた！プレイヤー交代するよ");





            // 今のプレイヤーを非表示に
            if (currentPlayer != null) nextPlayer.SetActive(false);

            // 次プレイヤーを表示
            if (nextPlayer != null) currentPlayer.SetActive(true);
            currentPlayer.transform.position = startPoint.position;

            if (nextPlayer != null) UI.SetActive(false);

            //if (nextPlayer != null) Biolea2r.SetActive(true);
            //if (nextPlayer != null) Biolear.SetActive(true);


            // このドアを消す
            Destroy(this.gameObject);
        }
    }

}
