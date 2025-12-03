using UnityEngine;

public class WarningUIController : MonoBehaviour
{
    public GameObject dangerUI;

    public static WarningUIController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowWarning()
    {
        dangerUI.SetActive(true);
        Invoke(nameof(HideWarning), 2.5f);
    }

    void HideWarning()
    {
        dangerUI.SetActive(false);
    }
}
