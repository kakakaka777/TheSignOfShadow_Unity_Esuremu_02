using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhisperMessage : MessageFunction
{
    [Header("===== 録音設定 =====")]
    [Tooltip("録音する最大秒数")]
    [SerializeField] private float maxRecordingTime = 10f;

    [Tooltip("録音のサンプリングレート")]
    [SerializeField] private int sampleRate = 44100;

    [Header("===== 設置設定 =====")]
    [Tooltip("設置できるささやきの最大数")]
    [SerializeField] private int whisperNumberMax = 3;

    [Tooltip("ささやきを設置できる最大距離")]
    [SerializeField] private float maxPlaceDistance = 10f;

    [Header("===== 再生設定 =====")]
    [Tooltip("ささやきが聞こえ始める距離")]
    [SerializeField] private float hearingDistance = 5f;

    [Tooltip("再生後、次に再生するまでの待ち時間（秒）")]
    [SerializeField] private float repeatInterval = 5f;

    [Tooltip("距離による音量減衰（近いほど大きく）")]
    [SerializeField] private bool useDistanceAttenuation = true;

    [Tooltip("最小音量（距離が最大の時）")]
    [Range(0f, 1f)]
    [SerializeField] private float minVolume = 0.1f;

    [Tooltip("最大音量（距離が0の時）")]
    [Range(0f, 1f)]
    [SerializeField] private float maxVolume = 1f;

    [Header("===== エフェクト設定 =====")]
    [Tooltip("ささやき設置時に生成するエフェクト（任意）")]
    [SerializeField] private GameObject whisperEffectPrefab;

    [Tooltip("ささやきアイコン（任意・設置場所に表示）")]
    [SerializeField] private GameObject whisperIconPrefab;

    // ===== 内部変数 =====
    private int whisperNumber = 0;
    private bool isRecording = false;
    private AudioClip recordedClip;
    private string microphoneName;
    private float recordingStartTime;

    // 現在のプレイヤーTransform
    private Transform CurrentPlayer
    {
        get
        {
            if (PlayerManager.playerID == 0) return player01;
            else return player02;
        }
    }

    private Camera CurrentCamera
    {
        get
        {
            if (PlayerManager.playerID == 0) return player01_FPCamera;
            else return player02_FPCamera;
        }
    }

    // ===== Unity ライフサイクル =====

    private void Start()
    {
        // マイクデバイスを取得
        if (Microphone.devices.Length > 0)
        {
            microphoneName = Microphone.devices[0];
            Debug.Log("使用するマイク: " + microphoneName);
        }
        else
        {
            Debug.LogWarning("マイクが見つかりません。録音機能は使用できません。");
        }
    }

    private void Update()
    {
        if (!canUse) return;

        // 左クリック：録音開始 or 録音停止して設置
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!isRecording)
            {
                StartRecording();
            }
            else
            {
                StopRecordingAndPlace();
            }
        }

        // 右クリック：録音キャンセル
        if (Input.GetKeyDown(KeyCode.Mouse1) && isRecording)
        {
            CancelRecording();
        }

        // 録音時間の上限チェック
        if (isRecording && Time.time - recordingStartTime >= maxRecordingTime)
        {
            Debug.Log("録音時間の上限に達しました。自動的に設置します。");
            StopRecordingAndPlace();
        }
    }

    // ===== MessageFunction 継承 =====

    public override void OnActivate(Vector3 playerPosition)
    {
        whisperNumber = 0;
        Debug.Log("ささやきメッセージ機能を起動しました");
        Debug.Log("左クリック: 録音開始/停止して設置");
        Debug.Log("右クリック: 録音キャンセル");
    }

    // ===== 録音機能 =====

    /// <summary>
    /// 録音を開始する
    /// </summary>
    private void StartRecording()
    {
        if (string.IsNullOrEmpty(microphoneName))
        {
            Debug.LogWarning("マイクが利用できません");
            return;
        }

        if (whisperNumber >= whisperNumberMax)
        {
            Debug.Log("ささやきの設置上限に達しています: " + whisperNumber + "/" + whisperNumberMax);
            return;
        }

        isRecording = true;
        recordingStartTime = Time.time;

        // マイクから録音開始
        recordedClip = Microphone.Start(microphoneName, false, Mathf.CeilToInt(maxRecordingTime), sampleRate);

        Debug.Log("録音開始... (左クリックで停止して設置、右クリックでキャンセル)");
    }

    /// <summary>
    /// 録音を停止してささやきを設置する
    /// </summary>
    private void StopRecordingAndPlace()
    {
        if (!isRecording) return;

        isRecording = false;

        // 録音停止
        int recordingLength = Microphone.GetPosition(microphoneName);
        Microphone.End(microphoneName);

        if (recordingLength <= 0)
        {
            Debug.Log("録音データがありません");
            return;
        }

        // 録音データをトリミング（実際に録音した長さだけ切り出す）
        AudioClip trimmedClip = TrimAudioClip(recordedClip, recordingLength);

        // 設置場所を決定
        Vector3 placePosition;
        Transform parentTransform = null;

        RaycastHit hit;
        if (CurrentCamera != null)
        {
            Ray ray = CurrentCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, maxPlaceDistance))
            {
                placePosition = hit.point;
                parentTransform = hit.collider.transform;
            }
            else
            {
                // 当たらなかった場合はプレイヤーの前方に設置
                placePosition = CurrentPlayer.position + CurrentPlayer.forward * 2f;
            }
        }
        else
        {
            placePosition = CurrentPlayer.position + CurrentPlayer.forward * 2f;
        }

        // ささやきを設置
        CreateWhisper(placePosition, trimmedClip, parentTransform);

        whisperNumber++;
        Debug.Log("ささやきを設置しました: " + whisperNumber + "/" + whisperNumberMax);
    }

    /// <summary>
    /// 録音をキャンセルする
    /// </summary>
    private void CancelRecording()
    {
        if (!isRecording) return;

        isRecording = false;
        Microphone.End(microphoneName);
        recordedClip = null;

        Debug.Log("録音をキャンセルしました");
    }

    /// <summary>
    /// AudioClipを実際の録音長さにトリミングする
    /// </summary>
    private AudioClip TrimAudioClip(AudioClip clip, int sampleLength)
    {
        if (clip == null || sampleLength <= 0) return null;

        float[] samples = new float[sampleLength * clip.channels];
        clip.GetData(samples, 0);

        AudioClip trimmedClip = AudioClip.Create(
            "WhisperRecording",
            sampleLength,
            clip.channels,
            clip.frequency,
            false
        );
        trimmedClip.SetData(samples, 0);

        return trimmedClip;
    }

    // ===== ささやき設置 =====

    /// <summary>
    /// ささやきオブジェクトを作成する
    /// </summary>
    private void CreateWhisper(Vector3 position, AudioClip clip, Transform parent = null)
    {
        // 空のGameObjectを作成
        GameObject whisperObj = new GameObject("Whisper_" + whisperNumber);
        whisperObj.transform.position = position;

        if (parent != null)
        {
            whisperObj.transform.SetParent(parent);
        }

        // WhisperZoneTriggerコンポーネントを追加
        WhisperZoneTrigger trigger = whisperObj.AddComponent<WhisperZoneTrigger>();
        trigger.Initialize(
            clip,
            hearingDistance,
            repeatInterval,
            useDistanceAttenuation,
            minVolume,
            maxVolume,
            player01,
            player02
        );

        // エフェクトがあれば生成
        if (whisperEffectPrefab != null)
        {
            GameObject effect = Instantiate(whisperEffectPrefab, position, Quaternion.identity, whisperObj.transform);
        }

        // アイコンがあれば生成
        if (whisperIconPrefab != null)
        {
            GameObject icon = Instantiate(whisperIconPrefab, position, Quaternion.identity, whisperObj.transform);
        }
    }
}

/// <summary>
/// ささやきの再生を管理するコンポーネント
/// プレイヤーが近づくと音声を再生する
/// 洞窟風の残響エフェクト付き
/// </summary>
public class WhisperZoneTrigger : MonoBehaviour
{
    // ===== 設定値（Initializeで設定） =====
    private AudioClip whisperClip;
    private float hearingDistance;
    private float repeatInterval;
    private bool useDistanceAttenuation;
    private float minVolume;
    private float maxVolume;
    private Transform player01;
    private Transform player02;

    // ===== 洞窟エフェクト設定 =====
    private bool useCaveEffect = true;

    // ===== 内部変数 =====
    private AudioSource whisperAudioSource;
    private AudioReverbFilter reverbFilter;
    private AudioLowPassFilter lowPassFilter;
    private AudioEchoFilter echoFilter;
    private float lastPlayTime = -1000f;
    private bool isInitialized = false;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(
        AudioClip clip,
        float distance,
        float interval,
        bool distanceAttenuation,
        float minVol,
        float maxVol,
        Transform p1,
        Transform p2)
    {
        whisperClip = clip;
        hearingDistance = distance;
        repeatInterval = interval;
        useDistanceAttenuation = distanceAttenuation;
        minVolume = minVol;
        maxVolume = maxVol;
        player01 = p1;
        player02 = p2;

        // AudioSourceを追加
        whisperAudioSource = gameObject.AddComponent<AudioSource>();
        whisperAudioSource.clip = whisperClip;
        whisperAudioSource.playOnAwake = false;
        whisperAudioSource.loop = false;
        whisperAudioSource.spatialBlend = 1f; // 3Dサウンド

        // 3Dサウンド設定
        whisperAudioSource.rolloffMode = AudioRolloffMode.Logarithmic; // より自然な減衰
        whisperAudioSource.minDistance = 1f;
        whisperAudioSource.maxDistance = hearingDistance;
        whisperAudioSource.spread = 60f; // 音の広がり（洞窟っぽく）
        whisperAudioSource.dopplerLevel = 0f; // ドップラー効果オフ

        // 洞窟風エフェクトを追加
        SetupCaveAudioEffects();

        isInitialized = true;
    }

    /// <summary>
    /// 洞窟風の音響エフェクトをセットアップ
    /// </summary>
    private void SetupCaveAudioEffects()
    {
        // ===== リバーブフィルター（残響）=====
        // 洞窟のような広い空間での反響を再現
        reverbFilter = gameObject.AddComponent<AudioReverbFilter>();
        reverbFilter.reverbPreset = AudioReverbPreset.Cave; // 洞窟プリセット！

        // カスタム調整（より不気味に）
        reverbFilter.dryLevel = 0f;           // 原音レベル
        reverbFilter.room = -400f;            // 部屋の反響（-10000〜0）
        reverbFilter.roomHF = -1000f;         // 高周波の反響
        reverbFilter.roomLF = 0f;             // 低周波の反響
        reverbFilter.decayTime = 3.0f;        // 残響が消えるまでの時間（秒）
        reverbFilter.decayHFRatio = 0.5f;     // 高周波の減衰率
        reverbFilter.reflectionsLevel = -500f;// 初期反射レベル
        reverbFilter.reflectionsDelay = 0.02f;// 初期反射の遅延
        reverbFilter.reverbLevel = 200f;      // 後期残響レベル
        reverbFilter.reverbDelay = 0.04f;     // 後期残響の遅延
        reverbFilter.diffusion = 100f;        // 拡散（エコーの密度）
        reverbFilter.density = 100f;          // 密度

        // ===== ローパスフィルター =====
        // こもった感じを出す（壁に反射した音っぽく）
        lowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
        lowPassFilter.cutoffFrequency = 2500f;  // 高音をカット（1000〜5000が洞窟っぽい）
        lowPassFilter.lowpassResonanceQ = 1.5f; // 共鳴

        // ===== エコーフィルター =====
        // 洞窟の壁に反射するエコー
        echoFilter = gameObject.AddComponent<AudioEchoFilter>();
        echoFilter.delay = 300f;        // エコーの遅延（ミリ秒）
        echoFilter.decayRatio = 0.4f;   // エコーの減衰率（0〜1）
        echoFilter.wetMix = 0.8f;       // エフェクト音の混合率
        echoFilter.dryMix = 1f;         // 原音の混合率
    }

    /// <summary>
    /// 距離に応じてエフェクトの強さを調整
    /// 近いと原音が強く、遠いと残響が強くなる
    /// </summary>
    private void UpdateCaveEffectByDistance(float distance)
    {
        if (!useCaveEffect) return;

        float t = Mathf.Clamp01(distance / hearingDistance);

        // 遠いほど残響が強くなる
        if (reverbFilter != null)
        {
            reverbFilter.dryLevel = Mathf.Lerp(0f, -2000f, t);      // 遠いと原音が小さく
            reverbFilter.reverbLevel = Mathf.Lerp(-500f, 500f, t);  // 遠いと残響が大きく
            reverbFilter.decayTime = Mathf.Lerp(2.0f, 4.0f, t);     // 遠いと残響が長く
        }

        // 遠いほどこもった音に
        if (lowPassFilter != null)
        {
            lowPassFilter.cutoffFrequency = Mathf.Lerp(4000f, 1500f, t);
        }

        // 遠いほどエコーが強く
        if (echoFilter != null)
        {
            echoFilter.wetMix = Mathf.Lerp(0.5f, 1f, t);
            echoFilter.decayRatio = Mathf.Lerp(0.3f, 0.6f, t);
        }
    }

    private void Update()
    {
        if (!isInitialized) return;

        // 最も近いプレイヤーとの距離を計算
        float distanceToPlayer = GetNearestPlayerDistance();

        // 範囲内かつ再生間隔が経過していれば再生
        if (distanceToPlayer <= hearingDistance)
        {
            // 再生中でなく、インターバルが経過していれば再生
            if (!whisperAudioSource.isPlaying && Time.time - lastPlayTime >= repeatInterval)
            {
                PlayWhisper(distanceToPlayer);
            }

            // 距離による音量・エフェクト調整（再生中も更新）
            if (whisperAudioSource.isPlaying)
            {
                if (useDistanceAttenuation)
                {
                    UpdateVolume(distanceToPlayer);
                }
                UpdateCaveEffectByDistance(distanceToPlayer);
            }
        }
    }

    /// <summary>
    /// 最も近いプレイヤーとの距離を取得
    /// </summary>
    private float GetNearestPlayerDistance()
    {
        float distance = float.MaxValue;

        if (player01 != null)
        {
            float d1 = Vector3.Distance(transform.position, player01.position);
            if (d1 < distance) distance = d1;
        }

        if (player02 != null)
        {
            float d2 = Vector3.Distance(transform.position, player02.position);
            if (d2 < distance) distance = d2;
        }

        return distance;
    }

    /// <summary>
    /// ささやきを再生する
    /// </summary>
    private void PlayWhisper(float distance)
    {
        UpdateVolume(distance);
        UpdateCaveEffectByDistance(distance);
        whisperAudioSource.Play();
        lastPlayTime = Time.time;

        Debug.Log("ささやきを再生（洞窟エフェクト付き）: " + gameObject.name);
    }

    /// <summary>
    /// 距離に応じて音量を更新
    /// </summary>
    private void UpdateVolume(float distance)
    {
        if (useDistanceAttenuation)
        {
            // 距離が0なら最大、hearingDistanceなら最小
            float t = Mathf.Clamp01(distance / hearingDistance);
            whisperAudioSource.volume = Mathf.Lerp(maxVolume, minVolume, t);
        }
        else
        {
            whisperAudioSource.volume = maxVolume;
        }
    }

    // ===== デバッグ用：範囲を可視化 =====
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, hearingDistance);
    }
}
