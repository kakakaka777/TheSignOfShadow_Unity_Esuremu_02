using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hasami_Function_Special : MonoBehaviour
{
    [Header("アゴ（モデルの Transform をアタッチ）")]
    public Transform leftJaw;
    public Transform rightJaw;

    [Header("閉じたときの角度（ローカルEuler角）")]
    public Vector3 leftClosedEuler = new Vector3(0, 0, 40);
    public Vector3 rightClosedEuler = new Vector3(0, 0, -40);

    [Header("タイミング設定")]
    [Min(0.1f)] public float closeSpeed = 8f;
    [Min(0f)] public float closedWaitTime = 0.5f;
    [Min(0.1f)] public float openSpeed = 2f;

    [Header("条件①: ジャンプ高さ")]
    [Tooltip("プレイヤーがこの高さ（Y座標の上昇量）に達したら閉じる")]
    [Min(0.1f)] public float jumpHeightToClamp = 2f;

    [Header("条件②: 待機時間")]
    [Tooltip("Trigger 内にこの秒数ずっと滞在したら閉じる")]
    [Min(0f)] public float staySecondsToClamp = 2f;

    [Header("プレイヤー判定")]
    public string[] playerTags = { "Player", "Player1", "Player2", "Player3", "Player4" };

    

    // ===============================
    // 内部変数
    // ===============================
    private Quaternion leftOpenRot;
    private Quaternion rightOpenRot;

    private bool isRunning = false;
    private GameObject latchedPlayer = null;

    // 監視用コルーチン
    private Coroutine monitorCoroutine = null;
    private float playerEntryY = 0f;  // プレイヤーが乗った時のY座標

    private void Start()
    {
        if (leftJaw != null) leftOpenRot = leftJaw.localRotation;
        if (rightJaw != null) rightOpenRot = rightJaw.localRotation;

        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Hasami_Function_Special] OnTriggerEnter: {other.name}");

        if (!IsPlayer(other.gameObject)) return;
        if (isRunning) return;
        if (monitorCoroutine != null) return;  // 既に監視中なら無視

        latchedPlayer = other.gameObject;
        playerEntryY = other.transform.position.y;

        // 両方の条件を同時に監視開始
        monitorCoroutine = StartCoroutine(MonitorPlayerBehavior());
        Debug.Log($"[Hasami_Function_Special] 監視開始 (基準Y={playerEntryY:F2})");
        Debug.Log($"  → 条件①: {jumpHeightToClamp}m上昇で閉じる");
        Debug.Log($"  → 条件②: {staySecondsToClamp}秒待機で閉じる");
    }

    private void OnTriggerExit(Collider other)
    {
        if (latchedPlayer == null) return;

        if (other.gameObject == latchedPlayer && !isRunning)
        {
            Debug.Log("[Hasami_Function_Special] プレイヤーが出た。監視キャンセル。");

            if (monitorCoroutine != null)
            {
                StopCoroutine(monitorCoroutine);
                monitorCoroutine = null;
            }

            latchedPlayer = null;
        }
    }

    // ===============================
    // 両方の条件を同時に監視
    // ===============================
    private IEnumerator MonitorPlayerBehavior()
    {
        float elapsedTime = 0f;

        while (true)
        {
            // プレイヤーが出た/消えた
            if (latchedPlayer == null)
            {
                monitorCoroutine = null;
                yield break;
            }

            // 現在のプレイヤーY座標
            float currentY = latchedPlayer.transform.position.y;
            float heightGained = currentY - playerEntryY;
            Debug.Log($"プレイヤーのジャンプ高さ {heightGained:F2}m に到達！");
            // ==============================
            // 条件①: 一定の高さに達したら閉じる
            // ==============================
            if (heightGained >= jumpHeightToClamp)
            {
                Debug.Log($"[Hasami_Function_Special] 条件①達成！ジャンプ高さ {heightGained:F2}m に到達！閉じます！");
                monitorCoroutine = null;

                if (!isRunning && latchedPlayer != null)
                {
                    yield return StartCoroutine(ClampRoutine());
                }
                yield break;
            }

            // ==============================
            // 条件②: 待機時間が経過したら閉じる
            // ==============================
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= staySecondsToClamp)
            {
                Debug.Log($"[Hasami_Function_Special] 条件②達成！{staySecondsToClamp}秒経過！閉じます！");
                monitorCoroutine = null;

                if (!isRunning && latchedPlayer != null)
                {
                    yield return StartCoroutine(ClampRoutine());
                }
                yield break;
            }

            yield return null;
        }
    }

    // ===============================
    // プレイヤー判定
    // ===============================
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

    // ===============================
    // 挟み動作（閉じる→待つ→開く）
    // ===============================
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

        // ② 閉じたまま少し待つ
        if (closedWaitTime > 0f)
            yield return new WaitForSeconds(closedWaitTime);

        // ③ ゆっくり開いて元の角度に戻る
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
