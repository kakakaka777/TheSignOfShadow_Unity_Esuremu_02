using System.Collections;
using UnityEngine;

public class ShojiDoor : MonoBehaviour
{
    [Header("入力設定")]
    [Tooltip("障子を開くのに使うキー")]
    public KeyCode openKey = KeyCode.E;

    [Header("共通設定")]
    [Tooltip("開くのにかかる時間（秒）")]
    public float slideTime = 0.5f;

    [Tooltip("ローカル座標で動かす場合はオン（通常はオン推奨）")]
    public bool useLocalPosition = true;

    [System.Serializable]
    public class ShojiPanel
    {
        public Transform target;
        public Vector3 openOffset = new Vector3(1f, 0f, 0f);

        [HideInInspector] public Vector3 closedPos;
        [HideInInspector] public Vector3 openPos;
    }

    [Header("障子パネル設定")]
    public ShojiPanel shojiL;
    public ShojiPanel shojiR;

    [Header("プレイヤー距離設定（任意）")]
    [Tooltip("距離判定の対象にするプレイヤー達（必要な場合だけ登録）")]
    public Transform[] players;

    [Tooltip("この距離以内の時だけ開く。0以下なら距離制限無し（＝元の挙動）。")]
    public float openDistance = 0f;

    [Tooltip("距離計算の中心にする位置。未設定なら自分自身を使う")]
    public Transform distanceOrigin;

    private bool isOpening = false;
    private bool isOpened = false;

    private void Start()
    {
        if (shojiL == null || shojiL.target == null ||
            shojiR == null || shojiR.target == null)
        {
            Debug.LogError("ShojiDoor: ShojiL または ShojiR の target が設定されていません。");
            enabled = false;
            return;
        }

        // 距離中心が未指定なら自分を使う
        if (distanceOrigin == null)
        {
            distanceOrigin = transform;
        }

        // 初期位置（閉じ）
        if (useLocalPosition)
        {
            shojiL.closedPos = shojiL.target.localPosition;
            shojiR.closedPos = shojiR.target.localPosition;
        }
        else
        {
            shojiL.closedPos = shojiL.target.position;
            shojiR.closedPos = shojiR.target.position;
        }

        // 開き位置
        shojiL.openPos = shojiL.closedPos + shojiL.openOffset;
        shojiR.openPos = shojiR.closedPos + shojiR.openOffset;
    }

    private void Update()
    {
        if (isOpened || isOpening) return;

        // ★距離条件チェック（openDistance <= 0で距離条件OFF → 常にtrue）
        if (!IsPlayerInRange())
            return;

        // キー入力で開く（元の動作）
        if (Input.GetKeyDown(openKey))
        {
            StartCoroutine(OpenShoji());
        }
    }

    /// <summary>
    /// 距離条件を満たすプレイヤーがいれば true を返す。
    /// openDistance <= 0 または players未設定なら距離制限OFF。
    /// </summary>
    private bool IsPlayerInRange()
    {
        // 距離制限OFF → もとの挙動に戻る
        if (openDistance <= 0f || players == null || players.Length == 0)
        {
            return true;
        }

        float sqrRange = openDistance * openDistance;
        Vector3 originPos = distanceOrigin.position;

        foreach (var p in players)
        {
            if (p == null) continue;
            if (!p.gameObject.activeInHierarchy) continue;

            float sqrDist = (p.position - originPos).sqrMagnitude;

            if (sqrDist <= sqrRange)
            {
                return true; // 1人でも近ければOK
            }
        }
        return false;
    }

    private IEnumerator OpenShoji()
    {
        isOpening = true;
        float elapsed = 0f;

        while (elapsed < slideTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideTime);

            Vector3 posL = Vector3.Lerp(shojiL.closedPos, shojiL.openPos, t);
            Vector3 posR = Vector3.Lerp(shojiR.closedPos, shojiR.openPos, t);

            if (useLocalPosition)
            {
                shojiL.target.localPosition = posL;
                shojiR.target.localPosition = posR;
            }
            else
            {
                shojiL.target.position = posL;
                shojiR.target.position = posR;
            }

            yield return null;
        }

        isOpened = true;
        isOpening = false;
    }
}
