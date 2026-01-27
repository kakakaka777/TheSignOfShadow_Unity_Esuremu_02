using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneiMessage : MessageFunction
{
    [Header("幻影設定")]
    [Tooltip("幻影として表示するプレイヤーモデルのプレハブ（半透明マテリアル推奨）")]
    public GameObject geneiPrefab;

    [Tooltip("設置できる幻影の最大数")]
    [SerializeField] private int geneiNumberMax = 3;

    [Tooltip("幻影を設置する最大距離")]
    [SerializeField] private float maxDistance = 5f;

    [Header("動き記録設定")]
    [Tooltip("記録する秒数（この秒数前の動きを幻影にする）")]
    [SerializeField] private float recordDuration = 5f;

    [Tooltip("記録の更新間隔（秒）")]
    [SerializeField] private float recordInterval = 0.1f;

    [Tooltip("幻影が再生するアニメーションの長さ（秒）")]
    [SerializeField] private float geneiPlaybackDuration = 5f;

    [Tooltip("幻影を繰り返し再生するか")]
    [SerializeField] private bool loopPlayback = true;

    [Header("幻影の見た目設定")]
    [Tooltip("幻影の透明度 (0-1)")]
    [Range(0f, 1f)]
    [SerializeField] private float geneiAlpha = 0.5f;

    [Tooltip("幻影の色")]
    [SerializeField] private Color geneiColor = new Color(0.8f, 0.9f, 1f, 0.5f);

    [Tooltip("URP/HDRP使用時はtrue（Built-in RPの場合はfalse）")]
    [SerializeField] private bool useURP = false;

    // 現在設置されている幻影の数
    private int geneiNumber = 0;

    // 記録用のリスト
    private List<GeneiPositionRecord> positionHistory = new List<GeneiPositionRecord>();
    private float lastRecordTime = 0f;

    // 現在のプレイヤーTransformを取得
    private Transform CurrentPlayer
    {
        get
        {
            if (PlayerManager.playerID == 0) return player01;
            else return player02;
        }
    }

    private void Update()
    {
        // プレイヤーの動きを常に記録
        RecordPlayerMovement();

        if (canUse == false)
        {
            return;
        }

        // 左クリックで幻影を設置
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TryPlaceGenei();
        }
    }

    /// <summary>
    /// プレイヤーの位置と回転を記録する
    /// </summary>
    private void RecordPlayerMovement()
    {
        if (CurrentPlayer == null) return;

        // 一定間隔で記録
        if (Time.time - lastRecordTime >= recordInterval)
        {
            lastRecordTime = Time.time;

            // 新しい記録を追加
            positionHistory.Add(new GeneiPositionRecord(
                CurrentPlayer.position,
                CurrentPlayer.rotation,
                Time.time
            ));

            // 古い記録を削除（recordDuration秒以上前のもの）
            float cutoffTime = Time.time - recordDuration;
            positionHistory.RemoveAll(record => record.timestamp < cutoffTime);
        }
    }

    /// <summary>
    /// 幻影を設置しようとする
    /// </summary>
    private void TryPlaceGenei()
    {
        geneiNumber++;

        if (geneiNumber <= geneiNumberMax)
        {
            RaycastHit hit;
            Camera currentCamera = (PlayerManager.playerID == 0) ? player01_FPCamera : player02_FPCamera;

            if (currentCamera != null)
            {
                Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, maxDistance))
                {
                    CreateGenei(hit);
                }
                else
                {
                    // 床に当たらなくてもプレイヤーの前に設置
                    Vector3 placePosition = CurrentPlayer.position + CurrentPlayer.forward * 2f;
                    CreateGeneiAtPosition(placePosition, CurrentPlayer.rotation);
                }
            }
        }
        else
        {
            geneiNumber = geneiNumberMax;
            Debug.Log("幻影の設置上限に達しました");
        }

        Debug.Log("幻影を設置できる回数: " + geneiNumber + "/" + geneiNumberMax);
        Debug.Log("PlayerID: " + PlayerManager.playerID);
    }

    public override void OnActivate(Vector3 playerPosition)
    {
        // カウントをリセット
        geneiNumber = 0;

        Debug.Log("幻影メッセージ（プレイヤーの動きを幻影として残す機能）を実行する");

        // メッセージUIなどから発動された時の処理
        RaycastHit hit;
        Vector3 rayOrigin = playerPosition + Vector3.up * 1f;
        Vector3 rayDir = transform.forward;

        if (Physics.Raycast(rayOrigin, rayDir, out hit, maxDistance))
        {
            CreateGenei(hit);
        }
    }

    /// <summary>
    /// Raycastヒット位置に幻影を作成
    /// </summary>
    private void CreateGenei(RaycastHit hit)
    {
        // 地面の法線に合わせて回転を調整
        Vector3 placePosition = hit.point;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * CurrentPlayer.rotation;

        CreateGeneiAtPosition(placePosition, rotation, hit.collider.transform);
    }

    /// <summary>
    /// 指定位置に幻影を作成
    /// </summary>
    private void CreateGeneiAtPosition(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (geneiPrefab == null)
        {
            Debug.LogWarning("幻影プレハブが設定されていません");
            return;
        }

        // 記録された動きデータをコピー
        List<GeneiPositionRecord> recordedMovement = new List<GeneiPositionRecord>(positionHistory);

        if (recordedMovement.Count < 2)
        {
            Debug.Log("記録データが不足しています。少し動いてから設置してください。");
            return;
        }

        // 幻影のルートオブジェクトを作成
        GameObject geneiRoot = new GameObject("Genei_Root");
        geneiRoot.transform.position = position;
        geneiRoot.transform.rotation = rotation;

        if (parent != null)
        {
            geneiRoot.transform.SetParent(parent);
        }

        // 幻影モデルをインスタンス化
        GameObject geneiModel = Instantiate(geneiPrefab, position, rotation, geneiRoot.transform);

        // 幻影の見た目を設定（半透明化）
        SetGeneiAppearance(geneiModel);

        // 幻影の動きを再生するコンポーネントを追加
        GeneiPlayback playback = geneiRoot.AddComponent<GeneiPlayback>();
        playback.Initialize(recordedMovement, geneiModel.transform, geneiPlaybackDuration, loopPlayback, position);

        Debug.Log("幻影を設置しました（記録フレーム数: " + recordedMovement.Count + "）");
    }

    /// <summary>
    /// 幻影の見た目を半透明に設定
    /// </summary>
    private void SetGeneiAppearance(GameObject genei)
    {
        // すべてのRendererを取得して半透明化
        Renderer[] renderers = genei.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            // マテリアルのインスタンスを作成して変更
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                // マテリアルを半透明モードに設定
                if (useURP)
                {
                    SetMaterialTransparentURP(materials[i]);
                }
                else
                {
                    SetMaterialTransparentBuiltIn(materials[i]);
                }

                Color color = geneiColor;
                color.a = geneiAlpha;

                // メインカラーを設定（シェーダーによってプロパティ名が異なる）
                if (materials[i].HasProperty("_BaseColor"))
                {
                    materials[i].SetColor("_BaseColor", color);
                }
                if (materials[i].HasProperty("_Color"))
                {
                    materials[i].SetColor("_Color", color);
                }
            }
            renderer.materials = materials;
        }
    }

    /// <summary>
    /// Built-in Render Pipeline用の半透明設定
    /// </summary>
    private void SetMaterialTransparentBuiltIn(Material mat)
    {
        if (mat.HasProperty("_Mode"))
        {
            mat.SetFloat("_Mode", 3); // Transparent mode
        }
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    /// <summary>
    /// URP用の半透明設定
    /// </summary>
    private void SetMaterialTransparentURP(Material mat)
    {
        // URP Lit シェーダー用の設定
        if (mat.HasProperty("_Surface"))
        {
            mat.SetFloat("_Surface", 1); // 0 = Opaque, 1 = Transparent
        }
        if (mat.HasProperty("_Blend"))
        {
            mat.SetFloat("_Blend", 0); // 0 = Alpha, 1 = Premultiply, 2 = Additive, 3 = Multiply
        }
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
}

/// <summary>
/// プレイヤーの位置・回転を記録するためのクラス（共有用）
/// </summary>
[System.Serializable]
public class GeneiPositionRecord
{
    public Vector3 position;
    public Quaternion rotation;
    public float timestamp;

    public GeneiPositionRecord(Vector3 pos, Quaternion rot, float time)
    {
        position = pos;
        rotation = rot;
        timestamp = time;
    }
}

/// <summary>
/// 幻影の動きを再生するコンポーネント
/// </summary>
public class GeneiPlayback : MonoBehaviour
{
    private Transform geneiTransform;
    private float playbackDuration;
    private bool loop;
    private Vector3 basePosition;

    private float playbackTime = 0f;
    private bool isInitialized = false;

    // 正規化された動きデータ
    private class NormalizedRecord
    {
        public Vector3 relativePosition;
        public Quaternion rotation;
        public float normalizedTime; // 0-1の正規化された時間

        public NormalizedRecord(Vector3 pos, Quaternion rot, float time)
        {
            relativePosition = pos;
            rotation = rot;
            normalizedTime = time;
        }
    }

    private List<NormalizedRecord> normalizedData = new List<NormalizedRecord>();

    public void Initialize(List<GeneiPositionRecord> rawMovementData, Transform genei, float duration, bool shouldLoop, Vector3 placedPosition)
    {
        geneiTransform = genei;
        playbackDuration = duration;
        loop = shouldLoop;
        basePosition = placedPosition;

        if (rawMovementData == null || rawMovementData.Count < 2)
        {
            Debug.LogWarning("動きデータが不正です");
            return;
        }

        // 最初と最後のタイムスタンプを取得
        Vector3 firstPosition = rawMovementData[0].position;
        float firstTime = rawMovementData[0].timestamp;
        float lastTime = rawMovementData[rawMovementData.Count - 1].timestamp;

        float totalDuration = lastTime - firstTime;
        if (totalDuration <= 0) totalDuration = 1f;

        // データを正規化
        foreach (var record in rawMovementData)
        {
            // 相対位置に変換（最初の位置からのオフセット）
            Vector3 relativePos = record.position - firstPosition;
            float normalizedTime = (record.timestamp - firstTime) / totalDuration;

            normalizedData.Add(new NormalizedRecord(relativePos, record.rotation, normalizedTime));
        }

        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized || normalizedData.Count < 2) return;

        // 再生時間を進める
        playbackTime += Time.deltaTime / playbackDuration;

        if (playbackTime >= 1f)
        {
            if (loop)
            {
                playbackTime = 0f;
            }
            else
            {
                playbackTime = 1f;
            }
        }

        // 現在の時間に対応する位置を補間で計算
        UpdateGeneiPosition();
    }

    private void UpdateGeneiPosition()
    {
        // 現在の再生時間に最も近い2つのキーフレームを見つける
        NormalizedRecord before = normalizedData[0];
        NormalizedRecord after = normalizedData[normalizedData.Count - 1];

        for (int i = 0; i < normalizedData.Count - 1; i++)
        {
            if (normalizedData[i].normalizedTime <= playbackTime &&
                normalizedData[i + 1].normalizedTime >= playbackTime)
            {
                before = normalizedData[i];
                after = normalizedData[i + 1];
                break;
            }
        }

        // 補間率を計算
        float segmentDuration = after.normalizedTime - before.normalizedTime;
        float t = 0f;
        if (segmentDuration > 0)
        {
            t = (playbackTime - before.normalizedTime) / segmentDuration;
        }

        // 位置と回転を補間
        Vector3 interpolatedRelativePos = Vector3.Lerp(before.relativePosition, after.relativePosition, t);
        Quaternion interpolatedRot = Quaternion.Slerp(before.rotation, after.rotation, t);

        // 設置位置を基準に相対位置を適用
        geneiTransform.position = basePosition + interpolatedRelativePos;
        geneiTransform.rotation = interpolatedRot;
    }
}
