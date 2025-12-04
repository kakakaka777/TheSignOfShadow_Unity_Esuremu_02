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

    private bool hasStuck = false;

    private void OnCollisionEnter(Collision collision)
    {
        // すでに刺さっていたら何もしない（多重判定防止）
        if (hasStuck) return;
        hasStuck = true;

        // Rigidbody がある場合、動きを完全に止める
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
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

        // 当たった場所＆向きをそれっぽく調整（任意）
        if (collision.contactCount > 0)
        {
            ContactPoint hit = collision.contacts[0];

            // 接触点に位置を合わせる
            transform.position = hit.point;

            // hit.point に合わせたあと、forward 方向に少しだけ押し込む
            //transform.position = hit.point - transform.forward * 0.1f;

            // 法線の逆方向を向かせて「刺さってる」感じにする
            //Vector3 forward = -hit.normal;
            //if (forward.sqrMagnitude > 0.001f)
            //{
            //    transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            //}

            // 当たったオブジェクトの子にする（動く床にも追従）
            transform.SetParent(collision.collider.transform);
        }

        // 一定時間後に自動で消える
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyAfterSeconds);
        Destroy(gameObject);
    }
}
