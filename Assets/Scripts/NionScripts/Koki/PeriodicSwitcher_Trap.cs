using UnityEngine;

public class PeriodicSwitcher_Trap : MonoBehaviour
{
    [Header("対象（オンオフしたいもの）")]
    public GameObject[] targetObjects;
    public Collider[] targetColliders;
    public MonoBehaviour[] targetComponents;

    [Header("時間設定")]
    [Tooltip("ON の時間（秒）")]
    [Min(0.1f)]
    public float onDuration = 2f;

    [Tooltip("OFF の時間（秒）")]
    [Min(0.1f)]
    public float offDuration = 2f;

    [Tooltip("開始時に ON かどうか")]
    public bool startOn = true;

    [Tooltip("シーン開始と同時に動作開始するか")]
    public bool playOnAwake = true;

    [Tooltip("ループさせるか（false なら一度状態が切り替わったら終了）")]
    public bool loop = true;

    [Header("発動条件(任意)")]
    [Tooltip("指定した場合、この Activator_Trap が発動中のみ動く")]
    public Activator_Trap activator;

    private float timer = 0f;
    private bool isOn = false;
    private bool isRunning = false;

    private void Start()
    {
        SetState(startOn);

        if (playOnAwake)
        {
            isRunning = true;
        }
    }

    private void Update()
    {
        if (activator != null && !activator.isActive)
        {
            if (isOn)
            {
                SetState(false);
            }
            return;
        }

        if (!isRunning) return;

        timer += Time.deltaTime;

        if (isOn)
        {
            if (timer >= onDuration)
            {
                SetState(false);
                if (!loop) isRunning = false;
            }
        }
        else
        {
            if (timer >= offDuration)
            {
                SetState(true);
                if (!loop) isRunning = false;
            }
        }
    }

    private void SetState(bool enable)
    {
        isOn = enable;
        timer = 0f;

        if (targetObjects != null)
        {
            foreach (var obj in targetObjects)
            {
                if (obj == null) continue;
                obj.SetActive(enable);
            }
        }

        if (targetColliders != null)
        {
            foreach (var col in targetColliders)
            {
                if (col == null) continue;
                col.enabled = enable;
            }
        }

        if (targetComponents != null)
        {
            foreach (var comp in targetComponents)
            {
                if (comp == null) continue;
                comp.enabled = enable;
            }
        }
    }

    // 外から再開したいとき用
    public void StartLoop()
    {
        isRunning = true;
        timer = 0f;
    }

    // 外から止めたいとき用
    public void StopLoop()
    {
        isRunning = false;
        SetState(false);
    }
}
