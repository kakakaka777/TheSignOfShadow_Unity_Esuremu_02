using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("プレイヤーの Transform")]
    public Transform player;

    [Header("手裏剣プレハブ")]
    public GameObject shurikenPrefab;

    [Header("発射位置")]
    public Transform firePoint;

    [Header("発射速度")]
    public float shurikenSpeed = 15f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject shuriken = Instantiate(
            shurikenPrefab,
            firePoint.position,
            firePoint.rotation
        );

        shuriken.GetComponent<Shuriken>().owner = gameObject;
        Rigidbody rb = shuriken.GetComponent<Rigidbody>();
        rb.velocity = firePoint.forward * shurikenSpeed;
    }
}
