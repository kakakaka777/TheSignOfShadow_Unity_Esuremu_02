using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [Header("宝箱のパーツ")]
    public Transform chestTop;      // 上のフタ
    public float openAngle = -90f;  // 開く角度
    public float openSpeed = 2f;    // 開くスピード

    [Header("宝箱の状態")]
    public bool isOpened = false;

    [Header("開けられる距離")]
    public float openDistance = 2f;

    private Transform playerT;

    void Start()
    {
        // Playerタグを持つオブジェクトを取得
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            playerT = p.transform;
        }
    }

    void Update()
    {
        if (isOpened) return;
        if (playerT == null) return;

        // プレイヤーとの距離を計算（Colliderが不要になる）
        float distance = Vector3.Distance(playerT.position, transform.position);

        // 距離内かつ E キーで開ける
        if (distance <= openDistance && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpened = true;
        StartCoroutine(OpenTop());

        // GameManager に鍵入手を通知（必要なら削除可）
        GameManager.Instance?.GetKey();
    }

    System.Collections.IEnumerator OpenTop()
    {
        Quaternion startRot = chestTop.localRotation;
        Quaternion endRot = Quaternion.Euler(openAngle, 0, 0);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * openSpeed;
            chestTop.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
    }
}
