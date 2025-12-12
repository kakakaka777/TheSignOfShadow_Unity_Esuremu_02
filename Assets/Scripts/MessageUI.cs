using System.Collections;
using UnityEngine;
using TMPro;

public class MessageUI : MonoBehaviour
{
    public static MessageUI Instance { get; private set; }

    [Header("UI参照")]
    [Tooltip("表示するテキスト(TMP)")]
    [SerializeField] private TMP_Text messageText;

    [Header("表示設定")]
    [Tooltip("表示時間（秒）。0以下なら表示しっぱなし")]
    [SerializeField] private float defaultDuration = 2.0f;

    [Tooltip("表示開始時に必ず有効化する")]
    [SerializeField] private bool forceEnableObject = true;

    private Coroutine currentRoutine;

    private void Awake()
    {
        // シングルトン（シーンに1つ置く前提）
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 初期は非表示にしておく
        HideImmediate();
    }

    /// <summary>
    /// 他スクリプトから呼ぶ用：メッセージ表示（既定秒数で自動非表示）
    /// </summary>
    public static void Show(string message)
    {
        if (Instance == null)
        {
            Debug.LogWarning("MessageUI.Instance がシーンに存在しません。MessageUI を配置してください。");
            return;
        }
        Instance.ShowInternal(message, Instance.defaultDuration);
    }

    /// <summary>
    /// 他スクリプトから呼ぶ用：表示秒数を指定
    /// </summary>
    public static void Show(string message, float duration)
    {
        if (Instance == null)
        {
            Debug.LogWarning("MessageUI.Instance がシーンに存在しません。MessageUI を配置してください。");
            return;
        }
        Instance.ShowInternal(message, duration);
    }

    /// <summary>
    /// 他スクリプトから呼ぶ用：即消す
    /// </summary>
    public static void Hide()
    {
        if (Instance == null) return;
        Instance.HideImmediate();
    }

    private void ShowInternal(string message, float duration)
    {
        if (messageText == null)
        {
            Debug.LogError("MessageUI: messageText が未設定です（TMP_Text をアサインしてください）");
            return;
        }

        if (forceEnableObject && !gameObject.activeSelf)
            gameObject.SetActive(true);

        messageText.text = message;
        messageText.gameObject.SetActive(true);

        // 既に表示中なら上書きしてタイマーをリセット
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        if (duration > 0f) currentRoutine = StartCoroutine(AutoHide(duration));
        else currentRoutine = null;
    }

    private IEnumerator AutoHide(float duration)
    {
        yield return new WaitForSeconds(duration);
        HideImmediate();
    }

    private void HideImmediate()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        if (messageText != null)
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }
    }
}
