using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("プレイヤーの Transform")]
    public Transform player;            // ← Inspector でプレイヤーを入れる

    [Header("手裏剣プレハブ")]
    public GameObject shurikenPrefab;   // ← 手裏剣の Prefab を入れる

    [Header("発射位置")]
    public Transform firePoint;         // ← 発射位置の Empty オブジェクト

    [Header("攻撃設定")]
    public float fireInterval = 1.5f;   // 発射間隔
    public float shurikenSpeed = 20f;   // 飛ぶ速さ

    private float timer = 0f;

    void Update()
    {
        if (player == null || firePoint == null || shurikenPrefab == null)
            return;

        timer += Time.deltaTime;

        if (timer >= fireInterval)
        {
            Shoot();
            timer = 0f;
        }
    }

    void Shoot()
    {
        // プレイヤーへの方向を計算
        Vector3 direction = (player.position - firePoint.position).normalized;

        // その方向を向いた回転を作成
        Quaternion lookRot = Quaternion.LookRotation(direction);

        // 手裏剣生成（プレイヤー方向に向ける）
        GameObject shuriken = Instantiate(shurikenPrefab, firePoint.position, lookRot);

        // Rigidbody に速度を与えて飛ばす
        Rigidbody rb = shuriken.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shuriken.transform.forward * shurikenSpeed;
        }
    }
}
