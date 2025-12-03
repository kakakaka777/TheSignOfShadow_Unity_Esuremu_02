using UnityEngine;

public class MoveDirectional_Trap : MonoBehaviour
{
    public enum MoveMode
    {
        OneWay,     // 一度だけ指定距離を移動
        PingPong    // 往復
    }

    [Header("移動設定")]
    [Tooltip("移動方向（上・下・左・右）")]
    public Vector3 moveDirection = Vector3.right;

    [Tooltip("進む距離")]
    [Min(0f)]
    public float moveDistance = 3f;

    [Tooltip("移動速度")]
    [Min(0.01f)]
    public float moveSpeed = 1f;

    [Tooltip("往復 or 一度だけ移動")]
    public MoveMode moveMode = MoveMode.PingPong;

    [Tooltip("ローカル座標系で動くか")]
    public bool useLocalSpace = true;

    [Tooltip("ゲーム開始と同時に動かすか")]
    public bool playOnAwake = true;

    [Header("発動条件（任意）")]
    [Tooltip("この TrapActivator が発動中のみ動く")]
    public Activator_Trap activator;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool hasMovedOneWay = false;
    private float pingPongTime = 0f;

    private void Start()
    {
        // 自動で取得
        if (activator == null)
            activator = GetComponent<Activator_Trap>();

        startPos = useLocalSpace ? transform.localPosition : transform.position;

        // 方向を正規化
        Vector3 dir = moveDirection.normalized;

        // 目的地
        targetPos = startPos + dir * moveDistance;
    }

    private void Update()
    {
        // 発動条件が false なら停止
        if (!IsActivated()) return;

        // モードごとに移動処理
        switch (moveMode)
        {
            case MoveMode.OneWay:
                MoveOneWay();
                break;
            case MoveMode.PingPong:
                MovePingPong();
                break;
        }
    }

    private bool IsActivated()
    {
        // activator が無い → playOnAwake のみで動く
        if (activator == null)
            return playOnAwake;

        // activator がある → isActive が true のときだけ動く
        return activator.isActive;
    }

    // ─────────────────────────────
    // ■ 一度だけ移動するタイプ
    // ─────────────────────────────
    private void MoveOneWay()
    {
        if (hasMovedOneWay) return;

        Vector3 current = useLocalSpace ? transform.localPosition : transform.position;
        Vector3 next = Vector3.MoveTowards(current, targetPos, moveSpeed * Time.deltaTime);

        ApplyPosition(next);

        if (Vector3.Distance(next, targetPos) < 0.01f)
        {
            hasMovedOneWay = true; // 到達したら終了
        }
    }

    // ─────────────────────────────
    // ■ PingPong (往復移動)
    // ─────────────────────────────
    private void MovePingPong()
    {
        pingPongTime += Time.deltaTime * moveSpeed;

        float t = Mathf.PingPong(pingPongTime, 1f);
        Vector3 pos = Vector3.Lerp(startPos, targetPos, t);

        ApplyPosition(pos);
    }

    // 座標反映
    private void ApplyPosition(Vector3 pos)
    {
        if (useLocalSpace)
            transform.localPosition = pos;
        else
            transform.position = pos;
    }

    // 位置リセット
    public void ResetPosition()
    {
        hasMovedOneWay = false;
        pingPongTime = 0f;

        ApplyPosition(startPos);
    }
}
