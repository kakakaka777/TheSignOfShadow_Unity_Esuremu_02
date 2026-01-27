using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [Header("宝箱のパーツ")]
    public Transform chestTop;
    public float openAngle = -90f;
    public float openSpeed = 2f;

    [Header("宝箱の状態")]
    public bool isOpened = false;

    [Header("開けられる距離")]
    public float openDistance = 2f;

    private Transform playerT;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) playerT = p.transform;
    }

    void Update()
    {
        if (isOpened) return;
        if (playerT == null) return;

        float distance = Vector3.Distance(playerT.position, transform.position);

        if (distance <= openDistance && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpened = true;
        StartCoroutine(OpenTop());

        GameManager_Sora.Instance?.GetKey();

        // ★メッセージ
        MessageUI.Show("You get a key.");

        // ★確認用ログ（ここが true になってるかが重要）
        Debug.Log($"[TreasureChest] hasKey = {GameManager_Sora.Instance != null && GameManager_Sora.Instance.hasKey}");
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
