using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("追尾するターゲット（プレイヤー）")]
    public Transform target;

    [Header("オフセット（プレイヤーからの距離）")]
    public Vector3 offset = new Vector3(0, 1, -6);

    [Header("追従の滑らかさ")]
    public float followSpeed = 8f;

    void LateUpdate()
    {
        if (target == null) return;

        // 回転したプレイヤーに合わせたカメラの位置
        Vector3 targetPos = target.TransformPoint(offset);

        // 滑らかに移動
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        // プレイヤーを見る
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
