using System.Collections;
using UnityEngine;

public class Clamp_Trap : MonoBehaviour
{
    [Header("アゴ（モデルの Transform をアタッチ）")]
    [Tooltip("左側のアゴの Transform")]
    public Transform leftJaw;

    [Tooltip("右側のアゴの Transform")]
    public Transform rightJaw;

    [Header("閉じたときの角度（ローカルEuler角）")]
    [Tooltip("左アゴが閉じたときの localEulerAngles")]
    public Vector3 leftClosedEuler = new Vector3(0, 0, 40);

    [Tooltip("右アゴが閉じたときの localEulerAngles")]
    public Vector3 rightClosedEuler = new Vector3(0, 0, -40);

    [Header("タイミング設定")]
    [Tooltip("閉じる速さ（数値が大きいほど速く動く）")]
    [Min(0.1f)]
    public float closeSpeed = 8f;

    [Tooltip("閉じた状態をキープする時間（秒）")]
    [Min(0f)]
    public float closedWaitTime = 0.5f;

    [Tooltip("開く速さ（数値が大きいほど速く動く）")]
    [Min(0.1f)]
    public float openSpeed = 2f;

    [Header("プレイヤー判定")]
    [Tooltip("プレイヤーとして扱うタグ")]
    public string[] playerTags = { "Player", "Player1", "Player2", "Player3", "Player4" };

    [Header("死亡処理（Death_Trap と連携）")]
    [Tooltip("同じ罠用オブジェクトに付いている Death_Trap。無くてもOK")]
    public Death_Trap deathTrap;

    // 開いた角度を保存
    private Quaternion leftOpenRot;
    private Quaternion rightOpenRot;

    private bool isRunning = false;
    private GameObject latchedPlayer = null;

    private void Start()
    {
        // 起動時の角度を「開いた状態」として記録
        if (leftJaw != null)
            leftOpenRot = leftJaw.localRotation;
        if (rightJaw != null)
            rightOpenRot = rightJaw.localRotation;

        // deathTrap 未設定なら自動取得を試みる
        if (deathTrap == null)
            deathTrap = GetComponent<Death_Trap>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // デバッグ用ログ（ちゃんと呼ばれているか確認用）
        Debug.Log($"[Clamp_Trap] OnTriggerEnter: {other.name}");

        if (!IsPlayer(other.gameObject)) return;   // プレイヤー以外は無視
        if (isRunning) return;                     // 既に動作中なら二重発動しない

        latchedPlayer = other.gameObject;
        StartCoroutine(ClampRoutine());
    }

    private bool IsPlayer(GameObject obj)
    {
        if (playerTags == null) return false;

        foreach (var tag in playerTags)
        {
            if (string.IsNullOrEmpty(tag)) continue;
            if (obj.CompareTag(tag)) return true;
        }
        return false;
    }

    private IEnumerator ClampRoutine()
    {
        isRunning = true;

        // ① バチン！と閉じる
        Quaternion leftStart = leftJaw != null ? leftJaw.localRotation : Quaternion.identity;
        Quaternion rightStart = rightJaw != null ? rightJaw.localRotation : Quaternion.identity;

        Quaternion leftClosedRot = Quaternion.Euler(leftClosedEuler);
        Quaternion rightClosedRot = Quaternion.Euler(rightClosedEuler);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * closeSpeed;
            float lerpT = Mathf.Clamp01(t);

            if (leftJaw != null)
                leftJaw.localRotation = Quaternion.Slerp(leftStart, leftClosedRot, lerpT);
            if (rightJaw != null)
                rightJaw.localRotation = Quaternion.Slerp(rightStart, rightClosedRot, lerpT);

            yield return null;
        }

        // ② 閉じきったタイミングで確定死亡
        if (latchedPlayer != null)
        {
            // Death_Trap があればそちらに任せる
            if (deathTrap != null)
            {
                // ★ Death_Trap 側に public void ForceKill(GameObject target) を用意しておく想定
                deathTrap.SendMessage("ForceKill", latchedPlayer, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                // Death_Trap が無い場合は、とりあえず直接消す（仮実装）
                Destroy(latchedPlayer);
            }
        }

        // ③ 閉じたまま少し待つ
        if (closedWaitTime > 0f)
            yield return new WaitForSeconds(closedWaitTime);

        // ④ ゆっくり開いて元の角度に戻る
        Quaternion leftClosedNow = leftJaw != null ? leftJaw.localRotation : Quaternion.identity;
        Quaternion rightClosedNow = rightJaw != null ? rightJaw.localRotation : Quaternion.identity;

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            float lerpT = Mathf.Clamp01(t);

            if (leftJaw != null)
                leftJaw.localRotation = Quaternion.Slerp(leftClosedNow, leftOpenRot, lerpT);
            if (rightJaw != null)
                rightJaw.localRotation = Quaternion.Slerp(rightClosedNow, rightOpenRot, lerpT);

            yield return null;
        }

        latchedPlayer = null;
        isRunning = false;
    }
}
