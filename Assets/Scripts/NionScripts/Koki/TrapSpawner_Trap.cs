using UnityEngine;

public class TrapSpawner_Trap : MonoBehaviour
{
    [Header("生成プレハブ")]
    [Tooltip("生成する矢・毒矢・エフェクトなどのプレハブ")]
    public GameObject prefab;

    [Header("スポーン位置")]
    [Tooltip("複数のスポーンポイント（未指定なら自分の位置から生成）")]
    public Transform[] spawnPoints;

    [Tooltip("スポーンポイントをランダムに選ぶか")]
    public bool useRandomSpawnPoint = true;

    [Tooltip("生成したオブジェクトをこのオブジェクトの子にするか")]
    public bool parentToSpawner = false;

    [Header("時間設定")]
    [Tooltip("最初の生成までの遅延時間")]
    [Min(0f)]
    public float firstDelay = 0f;

    [Tooltip("生成間隔（秒）")]
    [Min(0.1f)]
    public float spawnInterval = 3f;

    [Tooltip("シーン開始と同時に自動スタート")]
    public bool playOnAwake = true;

    [Tooltip("ループさせるか（false の場合、一回生成して終了）")]
    public bool loop = true;

    [Tooltip("最大生成数（0以下で無制限）")]
    public int maxSpawnCount = 0;

    [Header("発動条件(任意)")]
    public Activator_Trap activator;

    private float timer = 0f;
    private bool isRunning = false;
    private int spawnedCount = 0;

    private void Start()
    {
        // 最初の遅延時間ぶんだけマイナスからスタート
        timer = -firstDelay;

        if (playOnAwake)
        {
            isRunning = true;
        }
    }

    private void Update()
    {
        if (!isRunning) return;

        if (activator != null && !activator.isActive) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            Spawn();

            if (maxSpawnCount > 0 && spawnedCount >= maxSpawnCount)
            {
                isRunning = false;
            }

            if (!loop)
            {
                isRunning = false;
            }
        }
    }

    private void Spawn()
    {
        if (prefab == null) return;

        Vector3 pos;
        Quaternion rot;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform sp;

            if (useRandomSpawnPoint)
            {
                int index = Random.Range(0, spawnPoints.Length);
                sp = spawnPoints[index];
            }
            else
            {
                sp = spawnPoints[0];
            }

            pos = sp.position;
            rot = sp.rotation;
        }
        else
        {
            pos = transform.position;
            rot = transform.rotation;
        }

        GameObject obj = Instantiate(prefab, pos, rot);

        if (parentToSpawner && obj != null)
        {
            obj.transform.SetParent(transform);
        }

        spawnedCount++;
    }

    public void StartSpawn()
    {
        spawnedCount = 0;
        timer = -firstDelay;
        isRunning = true;
    }

    public void StopSpawn()
    {
        isRunning = false;
    }
}
