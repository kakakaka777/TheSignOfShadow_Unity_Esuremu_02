using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGimmik : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("ドアに触れた！プレイヤー交代するよ");

            // タグ "DeadBody" が付いたすべてのオブジェクトを削除
            GameObject[] allDeadBodies = GameObject.FindGameObjectsWithTag("DeadBody");
            foreach (GameObject ghost in allDeadBodies)
            {
                Destroy(ghost);
            }
            Debug.Log($"死体オブジェクトを {allDeadBodies.Length} 個削除しました");

        }
    }
}
