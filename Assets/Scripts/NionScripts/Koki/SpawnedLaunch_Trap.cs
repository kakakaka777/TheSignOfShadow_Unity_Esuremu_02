using UnityEngine;

public class SpawnedLaunch_Trap : MonoBehaviour
{
    [Header("飛ばすスピード")]
    [Min(0f)]
    public float speed = 10f;

    private Rigidbody rb;
    private bool launched = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // ★スポナーから呼ぶ
    public void SetSpawnPointAndLaunch(Transform sp)
    {
        if (launched) return;
        launched = true;

        if (sp == null)
        {
            Debug.LogWarning("[SpawnedLaunch_Trap] sp が null です");
            return;
        }

        Vector3 dir = sp.forward.normalized;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = dir * speed;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            // Rigidbodyなしの保険（基本はRB推奨）
            transform.position += dir * speed * Time.deltaTime;
        }
    }
}
