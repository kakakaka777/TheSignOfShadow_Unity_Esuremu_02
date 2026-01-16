using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hasami_SpecialFunction : MonoBehaviour
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

    [Header("プレイヤー滞在条件")]
    [Tooltip("Trigger 内にこの秒数ずっと滞在したら閉じる")]
    [Min(0f)]
    public float staySecondsToClamp = 3f;

    [Header("プレイヤー判定")]
    public string[] playerTags = { "Player", "Player1", "Player2", "Player3", "Player4" };

    [Header("死亡処理（Death_Trap と連携）")]
    public Death_Trap deathTrap;

    private Quaternion leftOpenRot;
    private Quaternion rightOpenRot;

    private bool isRunning = false;
    private GameObject latchedPlayer = null;

    // ★追加：滞在待ちコルーチン管理
    private Coroutine stayCoroutine = null;

    private void Start()
    {
        if (leftJaw != null) leftOpenRot = leftJaw.localRotation;
        if (rightJaw != null) rightOpenRot = rightJaw.localRotation;

        if (deathTrap == null) deathTrap = GetComponent<Death_Trap>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Clamp_Trap] OnTriggerEnter: {other.name}");

        if (!IsPlayer(other.gameObject)) return;
        if (isRunning) return;              // 既に挟み動作中なら無視
        if (stayCoroutine != null) return;  // 既に滞在カウント中なら二重起動しない

        latchedPlayer = other.gameObject;

        // ★入ったら3秒カウント開始
        stayCoroutine = StartCoroutine(WaitStayThenClamp());
    }

    // ★追加：途中で出たらキャンセル
    private void OnTriggerExit(Collider other)
    {
        if (latchedPlayer == null) return;

        if (other.gameObject == latchedPlayer && !isRunning)
        {
            Debug.Log("[Clamp_Trap] Player left before clamp. Cancel.");

            if (stayCoroutine != null)
            {
                StopCoroutine(stayCoroutine);
                stayCoroutine = null;
            }

            latchedPlayer = null;
        }
    }

    private IEnumerator WaitStayThenClamp()
    {
        float elapsed = 0f;

        while (elapsed < staySecondsToClamp)
        {
            // 途中で出た / 消えた
            if (latchedPlayer == null)
            {
                stayCoroutine = null;
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 3秒達成
        stayCoroutine = null;

        if (!isRunning && latchedPlayer != null)
        {
            yield return StartCoroutine(ClampRoutine());
        }
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
            if (deathTrap != null)
            {
                deathTrap.SendMessage("ForceKill", latchedPlayer, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
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
