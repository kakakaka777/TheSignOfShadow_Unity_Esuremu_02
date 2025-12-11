using System.Collections;
using UnityEngine;

public class GoalDoor : MonoBehaviour
{
    [Header("ドアのスライド設定")]
    [Tooltip("スライドさせるドア本体(Transform)")]
    public Transform door;

    [Tooltip("閉じた位置からどれだけ動かすか（向き＋距離）")]
    public Vector3 openOffset = new Vector3(1f, 0f, 0f);

    [Tooltip("開くのにかかる時間（秒）")]
    public float openTime = 0.5f;

    [Tooltip("ローカル座標で動かす場合はオン（通常はオン推奨）")]
    public bool useLocalPosition = true;

    [Header("プレイヤーと距離条件")]
    [Tooltip("距離判定に使うプレイヤー達（2人まで想定）")]
    public Transform[] players;  // ← 単体から配列に変更！

    [Tooltip("この距離以内にプレイヤーが来たら開く")]
    public float openDistance = 2f;

    [Tooltip("距離の中心にしたい位置。未設定ならこのオブジェクトの位置を使う")]
    public Transform distanceOrigin;

    private bool isOpening = false;
    private bool isOpened = false;

    private Vector3 closedPos;
    private Vector3 openPos;

    private void Start()
    {
        if (door == null)
        {
            Debug.LogError("GoalDoor: door が設定されていません。");
            enabled = false;
            return;
        }

        // 距離中心が未設定なら自分を使う
        if (distanceOrigin == null)
        {
            distanceOrigin = transform;
        }

        // 閉じた位置の記録
        if (useLocalPosition)
        {
            closedPos = door.localPosition;
        }
        else
        {
            closedPos = door.position;
        }

        openPos = closedPos + openOffset;

        // players が空なら Tag=Player を補完する（1人でも2人でも拾う）
        if (players == null || players.Length == 0)
        {
            GameObject[] found = GameObject.FindGameObjectsWithTag("Player");
            players = new Transform[found.Length];
            for (int i = 0; i < found.Length; i++)
                players[i] = found[i].transform;
        }
    }

    private void Update()
    {
        if (isOpened || isOpening) return;

        // 鍵を持っていなければ開かない
        if (GameManager.Instance == null || !GameManager.Instance.hasKey) return;

        // いずれかのプレイヤーが距離内であれば開く
        if (!IsAnyPlayerInRange()) return;

        // 開く
        StartCoroutine(OpenDoor());
    }

    /// <summary>
    /// players に登録されたプレイヤーのうち1人でも距離内なら true
    /// </summary>
    private bool IsAnyPlayerInRange()
    {
        if (players == null || players.Length == 0) return false;

        float sqrRange = openDistance * openDistance;
        Vector3 origin = distanceOrigin.position;

        foreach (var p in players)
        {
            if (p == null) continue;
            if (!p.gameObject.activeInHierarchy) continue;

            float sqrDist = (p.position - origin).sqrMagnitude;
            if (sqrDist <= sqrRange)
            {
                return true; // 誰か1人でも範囲内ならOK
            }
        }
        return false;
    }

    private IEnumerator OpenDoor()
    {
        isOpening = true;

        float elapsed = 0f;
        while (elapsed < openTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / openTime);

            Vector3 newPos = Vector3.Lerp(closedPos, openPos, t);

            if (useLocalPosition)
                door.localPosition = newPos;
            else
                door.position = newPos;

            yield return null;
        }

        isOpened = true;
        isOpening = false;
    }
}
