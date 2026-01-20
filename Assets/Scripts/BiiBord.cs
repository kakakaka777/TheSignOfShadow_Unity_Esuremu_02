using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiiBord : MonoBehaviour
{
    [SerializeField] private bool yAxisOnly = false;   // trueにするとY回転だけ追従
    [SerializeField] private Vector3 worldUp = default; // default(0,0,0)ならVector3.up扱い

    private Quaternion lookOffset = Quaternion.identity;

    void Start()
    {
        if (PlayerManager.playTransform == null) return;

        Vector3 up = (worldUp == Vector3.zero) ? Vector3.up : worldUp;

        Vector3 dir = PlayerManager.playTransform.position - transform.position;
        if (yAxisOnly) dir.y = 0f;

        if (dir.sqrMagnitude < 0.000001f) return;

        // 「開始時にLookAtした回転」と「今の回転」の差分(オフセット)を保存
        Quaternion lookAtStart = Quaternion.LookRotation(dir.normalized, up);
        lookOffset = Quaternion.Inverse(lookAtStart) * transform.rotation;
    }

    void Update()
    {
        if (PlayerManager.playTransform == null) return;

        Vector3 up = (worldUp == Vector3.zero) ? Vector3.up : worldUp;

        Vector3 dir = PlayerManager.playTransform.position - transform.position;
        if (yAxisOnly) dir.y = 0f;

        if (dir.sqrMagnitude < 0.000001f) return;

        Quaternion lookNow = Quaternion.LookRotation(dir.normalized, up);

        // LookAt回転に、開始時のオフセットを足す
        transform.rotation = lookNow * lookOffset;
    }
}
