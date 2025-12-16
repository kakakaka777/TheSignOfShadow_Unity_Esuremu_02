using System.Collections;
using UnityEngine;

public class ArrowLife_Trap : MonoBehaviour
{
    [Header("刺さってから何秒後に消すか")]
    [Min(0.1f)]
    public float destroyAfterSeconds = 3f;

    [Header("刺さったときに止めたいコンポーネント")]
    [Tooltip("MoveDirectional_Trap など、矢の移動に関わるスクリプトを入れる")]
    public MonoBehaviour[] componentsToDisableOnStick;

    [Header("刺さり判定")]
    [Tooltip("刺さり対象とするレイヤー（地面・壁など）。何も指定しない場合は全てに刺さる")]
    public LayerMask stickLayers = ~0;   // デフォルト：全部OK

    private bool hasStuck = false;

    private void OnTriggerEnter(Collider other)
    {
        // すでに刺さっていたら何もしない
        if (hasStuck) return;

        // 刺さり対象レイヤーでなければ無視（例：プレイヤーには刺さるけど見た目OKなら切っても良い）
        if ((stickLayers.value & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

        hasStuck = true;

        // Rigidbody がある場合、物理挙動を完全に止める
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;   // 以降は物理で動かさない
        }

        // 移動系スクリプトなどを停止
        if (componentsToDisableOnStick != null)
        {
            foreach (var comp in componentsToDisableOnStick)
            {
                if (comp == null) continue;
                comp.enabled = false;
            }
        }

        // ========= ここから「刺さり」見た目処理 =========

        // 1) 相手コライダー上の「一番近い点」を取得
        //    → Trigger でも使える ContactPoint 的な代替
        Vector3 contactPos = other.ClosestPoint(transform.position);

        // 2) その位置に矢の中心を合わせる
        transform.position = contactPos;

        // 3) ちょっとだけ矢の forward 方向にめり込ませたい場合はここを有効化
        // float penetrationDepth = 0.05f;
        // transform.position = contactPos - transform.forward * penetrationDepth;

        // 4) 刺さった相手の子にする（動く床・動く敵にも追従させたいとき）
        transform.SetParent(other.transform);

        // ========= 刺さり処理ここまで =========

        // 一定時間後に自動で消える
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyAfterSeconds);
        Destroy(gameObject);
    }
}
