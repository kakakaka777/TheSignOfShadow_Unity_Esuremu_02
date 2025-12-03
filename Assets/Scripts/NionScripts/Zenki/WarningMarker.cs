using UnityEngine;

public class WarningMarker : MonoBehaviour
{
    public bool isActive = true; // èÌÇ…óLå¯Ç≈Ç‡OK
    private bool hasShown = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive || hasShown) return;

        if (other.CompareTag("Player"))
        {
            hasShown = true;
            WarningUIController.Instance?.ShowWarning();
        }
    }
}
