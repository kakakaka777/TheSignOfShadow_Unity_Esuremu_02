using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSpawnController : MonoBehaviour
{
    public enum ScaleAxis { X, Y }
    public enum BillboardMode { Full, YawOnly } // Full: 完全にカメラ向き / YawOnly: 水平回転のみ

    [Header("Spawn")]
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private Transform spawnedParent;

    [Header("Spawn Area (Box)")]
    [SerializeField] private Vector3 boxCenterLocal = Vector3.zero;
    [SerializeField] private Vector3 boxSizeLocal = new Vector3(5f, 2f, 5f);
    [SerializeField] private bool showGizmos = true;

    [Header("Ground Check")]
    [Tooltip("Box内のランダム点の少し上から下向きにRaycastします")]
    [SerializeField] private float rayStartHeight = 10f;
    [SerializeField] private float rayLength = 30f;

    [Tooltip("ここに指定が1つでも入っていたら、" + "そのColliderに当たった時だけ生成します（最優先）")]
    [SerializeField] private List<Collider> allowedGroundColliders = new List<Collider>();

    [Tooltip("allowedGroundCollidersが空の場合に使う、生成可能な地面Layer")]
    [SerializeField] private LayerMask groundLayerMask = ~0;

    [Tooltip("空文字なら無視。指定したTagのColliderに当たった時だけ生成。")]
    [SerializeField] private string requiredGroundTag = "";

    [Tooltip("地面に当たった地点から少し浮かせたい時")]
    [SerializeField] private Vector3 spawnOffset = Vector3.up * 0.02f;

    [Header("Random Attempts")]
    [SerializeField] private int maxAttempts = 30;

    [Header("Scale Animation (Spawn FX)")]
    [SerializeField] private bool enableScaleFx = true;
    [SerializeField] private ScaleAxis scaleAxis = ScaleAxis.Y;
    [SerializeField] private float scaleDuration = 0.18f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float startAxisScale = 0.0f; // 0で完全に潰れてから出る
    [SerializeField] private bool useUnscaledTime = true;

    [Header("Billboard")]
    [SerializeField] private bool enableBillboard = true;
    [SerializeField] private BillboardMode billboardMode = BillboardMode.YawOnly;
    [SerializeField] private Camera targetCamera;

    // =========================
    // 外から呼ぶ入口（プレイヤー死亡時）
    // =========================
    public GameObject Spawn()
    {
        if (!spawnPrefab)
        {
            Debug.LogError("[DeathSpawnController] spawnPrefab が未設定です。");
            return null;
        }

        Vector3 spawnPos;
        Quaternion spawnRot;

        if (!TryGetSpawnPose(out spawnPos, out spawnRot))
        {
            // 地面が見つからない/条件合わない
            return null;
        }

        var go = Instantiate(spawnPrefab, spawnPos, spawnRot, spawnedParent);

        // Billboard
        if (enableBillboard)
        {
            var bb = go.GetComponent<BillboardToCamera>();
            if (!bb) bb = go.AddComponent<BillboardToCamera>();
            bb.SetCamera(targetCamera ? targetCamera : Camera.main);
            bb.SetMode(billboardMode);
        }

        // Scale FX
        if (enableScaleFx)
        {
            var fx = go.GetComponent<ScaleAxisPopFx>();
            if (!fx) fx = go.AddComponent<ScaleAxisPopFx>();
            fx.Play(scaleAxis, scaleDuration, scaleCurve, startAxisScale, useUnscaledTime);
        }

        return go;
    }

    // =========================
    // 実処理
    // =========================
    private bool TryGetSpawnPose(out Vector3 pos, out Quaternion rot)
    {
        pos = default;
        rot = Quaternion.identity;

        // Boxは「このオブジェクトのTransform」を基準にして回転も追従させる
        for (int i = 0; i < Mathf.Max(1, maxAttempts); i++)
        {
            Vector3 localRandom = new Vector3(
                Random.Range(-boxSizeLocal.x * 0.5f, boxSizeLocal.x * 0.5f),
                Random.Range(-boxSizeLocal.y * 0.5f, boxSizeLocal.y * 0.5f),
                Random.Range(-boxSizeLocal.z * 0.5f, boxSizeLocal.z * 0.5f)
            );

            Vector3 worldPoint = transform.TransformPoint(boxCenterLocal + localRandom);

            Vector3 rayOrigin = worldPoint + Vector3.up * rayStartHeight;
            var ray = new Ray(rayOrigin, Vector3.down);

            if (!Physics.Raycast(ray, out RaycastHit hit, rayLength, groundLayerMask, QueryTriggerInteraction.Ignore))
                continue;

            if (!IsAllowedGround(hit.collider))
                continue;

            pos = hit.point + spawnOffset;

            // 回転はデフォルトでPrefabの向きそのまま。
            // 必要なら「地面の法線に合わせる」などに変えてOK
            rot = Quaternion.identity;

            return true;
        }

        return false;
    }

    private bool IsAllowedGround(Collider col)
    {
        if (!col) return false;

        // 1) 指定Colliderがあるなら、それに当たった時だけOK
        if (allowedGroundColliders != null && allowedGroundColliders.Count > 0)
        {
            for (int i = 0; i < allowedGroundColliders.Count; i++)
            {
                if (allowedGroundColliders[i] == col) return PassTagCheck(col);
            }
            return false;
        }

        // 2) LayerMask は Raycast で既に絞ってるのでここでは Tag だけ見る
        return PassTagCheck(col);
    }

    private bool PassTagCheck(Collider col)
    {
        if (string.IsNullOrEmpty(requiredGroundTag)) return true;
        return col.CompareTag(requiredGroundTag);
    }

    // =========================
    // Gizmos
    // =========================
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(boxCenterLocal, boxSizeLocal);
    }
}

// =====================================================
// 2) カメラ向きに合わせて回転（ビルボード）
// =====================================================
public class BillboardToCamera : MonoBehaviour
{
    private Camera cam;
    private DeathSpawnController.BillboardMode mode = DeathSpawnController.BillboardMode.YawOnly;

    public void SetCamera(Camera c) => cam = c;
    public void SetMode(DeathSpawnController.BillboardMode m) => mode = m;

    private void LateUpdate()
    {
        if (!cam) cam = Camera.main;
        if (!cam) return;

        Vector3 toCam = cam.transform.position - transform.position;
        if (toCam.sqrMagnitude < 0.0001f) return;

        if (mode == DeathSpawnController.BillboardMode.Full)
        {
            transform.rotation = Quaternion.LookRotation(-toCam, Vector3.up);
        }
        else
        {
            // 水平回転だけ（上下は起こさない）
            toCam.y = 0f;
            if (toCam.sqrMagnitude < 0.0001f) return;
            transform.rotation = Quaternion.LookRotation(-toCam.normalized, Vector3.up);
        }
    }
}

// =====================================================
// 1) X or Y軸スケールで表示演出（ポップ）
// =====================================================
public class ScaleAxisPopFx : MonoBehaviour
{
    private Coroutine co;

    public void Play(DeathSpawnController.ScaleAxis axis, float duration, AnimationCurve curve, float startAxisScale, bool unscaled)
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(CoPlay(axis, duration, curve, startAxisScale, unscaled));
    }

    private IEnumerator CoPlay(DeathSpawnController.ScaleAxis axis, float duration, AnimationCurve curve, float startAxisScale, bool unscaled)
    {
        Vector3 baseScale = transform.localScale;
        Vector3 startScale = baseScale;

        if (axis == DeathSpawnController.ScaleAxis.X) startScale.x = startAxisScale;
        else startScale.y = startAxisScale;

        transform.localScale = startScale;

        float t = 0f;
        duration = Mathf.Max(0.0001f, duration);

        while (t < 1f)
        {
            float dt = unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            t += dt / duration;

            float k = curve != null ? curve.Evaluate(Mathf.Clamp01(t)) : Mathf.Clamp01(t);

            Vector3 s = baseScale;
            if (axis == DeathSpawnController.ScaleAxis.X)
                s.x = Mathf.Lerp(startAxisScale, baseScale.x, k);
            else
                s.y = Mathf.Lerp(startAxisScale, baseScale.y, k);

            transform.localScale = s;
            yield return null;
        }

        transform.localScale = baseScale;
        co = null;
    }
}
