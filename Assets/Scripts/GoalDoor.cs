using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDoor : MonoBehaviour
{
    [Header("入力設定")]
    public KeyCode interactKey = KeyCode.E;

    [Header("ドアのスライド設定")]
    public Transform door;
    public Vector3 openOffset = new Vector3(1f, 0f, 0f);
    public float openTime = 0.5f;
    public bool useLocalPosition = true;

    [Header("プレイヤーと距離条件")]
    [Tooltip("空でもOK（キー押下時に Player/Player1 を自動探索します）")]
    public Transform[] players;

    public float openDistance = 2f;

    [Tooltip("距離の中心。未設定なら door → transform の順で使う")]
    public Transform distanceOrigin;

    [Header("メッセージ")]
    public float noKeyMessageCooldown = 0.8f;
    private float nextNoKeyMessageTime = 0f;

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

        // 距離原点のデフォルトを door に寄せる（pivotズレ対策）
        if (distanceOrigin == null)
            distanceOrigin = door != null ? door : transform;

        if (useLocalPosition) closedPos = door.localPosition;
        else closedPos = door.position;

        openPos = closedPos + openOffset;
    }

    private void Update()
    {
        if (isOpened || isOpening) return;

        // ★「開けようとした時」だけ判定する
        if (!Input.GetKeyDown(interactKey)) return;

        // ★毎回、プレイヤーを拾い直す（死んで切り替わってもOK）
        RefreshPlayers();

        bool inRange = IsAnyPlayerInRange(out float nearestDist);

        bool hasKey = (GameManager_Sora.Instance != null && GameManager_Sora.Instance.hasKey);

        // ★デバッグログ（今だけ有効でOK）
        Debug.Log($"[GoalDoor] KeyPressed={interactKey} inRange={inRange} nearestDist={nearestDist:0.00} hasKey={hasKey} playersCount={(players == null ? 0 : players.Length)}");

        // 距離外なら何もしない（要件に無いのでメッセージも出さない）
        if (!inRange) return;

        // 鍵がないならメッセージ（キー押下時のみ）
        if (!hasKey)
        {
            if (Time.time >= nextNoKeyMessageTime)
            {
                MessageUI.Show("You don't have the key.");
                nextNoKeyMessageTime = Time.time + noKeyMessageCooldown;
            }
            return;
        }

        // 鍵あり＆距離内＆キー押下 → 開く
        StartCoroutine(OpenDoor());
    }

    private void RefreshPlayers()
    {
        var list = new List<Transform>();

        AddByTagSafe(list, "Player");
        AddByTagSafe(list, "Player1");

        // インスペクターで指定がある場合も混ぜたいならここで追加でもOKだが、
        // 今回は「確実に今いるプレイヤー」を優先して探索結果で置き換える
        players = list.ToArray();
    }

    private void AddByTagSafe(List<Transform> list, string tag)
    {
        try
        {
            var found = GameObject.FindGameObjectsWithTag(tag);
            foreach (var go in found)
            {
                if (go != null && go.activeInHierarchy)
                    list.Add(go.transform);
            }
        }
        catch
        {
            // タグ未登録などは無視
        }
    }

    private bool IsAnyPlayerInRange(out float nearestDist)
    {
        nearestDist = float.MaxValue;

        if (players == null || players.Length == 0) return false;

        float sqrRange = openDistance * openDistance;
        Vector3 origin = distanceOrigin.position;

        bool any = false;

        foreach (var p in players)
        {
            if (p == null) continue;
            if (!p.gameObject.activeInHierarchy) continue;

            float sqrDist = (p.position - origin).sqrMagnitude;
            float dist = Mathf.Sqrt(sqrDist);
            if (dist < nearestDist) nearestDist = dist;

            if (sqrDist <= sqrRange) any = true;
        }

        return any;
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

            if (useLocalPosition) door.localPosition = newPos;
            else door.position = newPos;

            yield return null;
        }

        isOpened = true;
        isOpening = false;
    }
}
