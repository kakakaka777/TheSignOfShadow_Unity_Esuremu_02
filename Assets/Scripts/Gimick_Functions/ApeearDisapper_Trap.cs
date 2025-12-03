using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApeearDisapper_Trap : MonoBehaviour
{
    [Header("基本設定")]
    [Tooltip("開始までの待ち時間")]
    [Min(0f)]
    public float startDelay = 0f;

    [Tooltip("出現/消失を切り替える間隔秒数")]
    [Min(0.1f)]
    public float toggleInterval = 1.0f;

    [Tooltip("何回切り替えるか。0なら無限ループ")]
    [Min(0)]
    public int toggleCount = 0;

    [Tooltip("最初は見えている状態にするか")]
    public bool startVisible = true;

    [Tooltip("ゲーム開始と同時に自動再生するか")]
    public bool playOnAwake = true;

    [Header("見た目 / 当たり判定 切り替え")]
    public bool toggleRenderers = true;
    public bool toggleColliders = true;

    [Header("発動条件(任意)")]
    [Tooltip("指定した場合、この TrapActivator が発動中のみ動く")]
    public Activator_Trap activator;

    private Renderer[] renderers;
    private Collider[] colliders;

    private Coroutine routine;
    private bool currentVisible;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);
    }

    private void Start()
    {
        SetVisible(startVisible);

        if (playOnAwake)
        {
            StartToggle();
        }
        

        activator = GetComponent<Activator_Trap>();

    }

    private void Update()
    {
        if (activator.isActive)
        {
            StartToggle();
        }

    }
    private void OnEnable()
    {
        if (playOnAwake && routine == null && Application.isPlaying)
        {
            StartToggle();
        }
    }

    private void OnDisable()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }

    public void StartToggle()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(ToggleRoutine());
    }

    private IEnumerator ToggleRoutine()
    {
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        int count = 0;

        while (toggleCount == 0 || count < toggleCount)
        {
            // 発動条件チェック
            if (activator != null && !activator.isActive)
            {
                yield return null;
                continue;
            }

            SetVisible(!currentVisible);
            count++;

            yield return new WaitForSeconds(toggleInterval);
        }

        routine = null;
    }

    private void SetVisible(bool visible)
    {
        currentVisible = visible;

        if (toggleRenderers && renderers != null)
        {
            foreach (var r in renderers)
            {
                if (r == null) continue;
                r.enabled = visible;
            }
        }

        if (toggleColliders && colliders != null)
        {
            foreach (var c in colliders)
            {
                if (c == null) continue;
                c.enabled = visible;
            }
        }
    }
}
