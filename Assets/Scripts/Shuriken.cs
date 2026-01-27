using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 5f;

    // 撃った人（Player or Enemy）
    public GameObject owner;

    private bool isDead = false;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (isDead) return;
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        // ===== 撃った本人には当たらない =====
        if (other.gameObject == owner)
        {
            return;
        }

        // ===== Playerに当たった（敵の手裏剣のみ）=====
        if (other.CompareTag("Player"))
        {
            Debug.Log("プレイヤー被弾！");
            isDead = true;

            // ★ ここでは絶対に timeScale を触らない
            Destroy(gameObject);
            return;
        }

        // ===== Enemyに当たった（プレイヤーの手裏剣）=====
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("敵にヒット！");
            isDead = true;
            Destroy(gameObject);
            return;
        }

        // ===== 壁・床 =====
        isDead = true;
        Destroy(gameObject);
    }
}
