using System.Collections;
using UnityEngine;

public class ShojiDoubleDoor : MonoBehaviour
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
        [Tooltip("この障子オブジェクト（Transform）")]
        public Transform target;

        [Tooltip("閉じた位置からどれだけ動かすか（向き＋距離）")]
        public Vector3 openOffset = new Vector3(1f, 0f, 0f);

        // 内部用
        [HideInInspector] public Vector3 closedPos;
        [HideInInspector] public Vector3 openPos;
    }

    [Header("障子パネル設定")]
    [Tooltip("左側の障子")]
    public ShojiPanel shojiL;

    [Tooltip("右側の障子")]
    public ShojiPanel shojiR;

    private bool isOpening = false;
    private bool isOpened = false;

    private void Start()
    {
        // nullチェック
        if (shojiL == null || shojiL.target == null ||
            shojiR == null || shojiR.target == null)
        {
            Debug.LogError("ShojiDoubleDoor: ShojiL または ShojiR の target が設定されていません。");
            enabled = false;
            return;
        }

        // 初期位置（閉じた位置）を保存
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

        // 開いた位置 = 閉じた位置 + オフセット
        shojiL.openPos = shojiL.closedPos + shojiL.openOffset;
        shojiR.openPos = shojiR.closedPos + shojiR.openOffset;
    }

    private void Update()
    {
        if (isOpened || isOpening) return;

        if (Input.GetKeyDown(openKey))
        {
            StartCoroutine(OpenShoji());
        }
    }

    private IEnumerator OpenShoji()
    {
        isOpening = true;
        float elapsed = 0f;

        while (elapsed < slideTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideTime);

            Vector3 newPosL = Vector3.Lerp(shojiL.closedPos, shojiL.openPos, t);
            Vector3 newPosR = Vector3.Lerp(shojiR.closedPos, shojiR.openPos, t);

            if (useLocalPosition)
            {
                shojiL.target.localPosition = newPosL;
                shojiR.target.localPosition = newPosR;
            }
            else
            {
                shojiL.target.position = newPosL;
                shojiR.target.position = newPosR;
            }

            yield return null;
        }

        isOpened = true;
        isOpening = false;
    }
}
