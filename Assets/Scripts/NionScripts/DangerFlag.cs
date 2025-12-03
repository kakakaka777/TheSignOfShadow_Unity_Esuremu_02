using UnityEngine;

public class DangerFlag : MonoBehaviour
{
    private bool hasShown = false;

    private GameObject owner;
    private int ownerRespawnCount = 0;

    public void SetOwner(GameObject player)
    {
        owner = player;

        PlayerControllerTest p = player.GetComponent<PlayerControllerTest>();
        if (p != null)
        {
            ownerRespawnCount = p.GetRespawnCount(); // ★記録時のリスポーンカウント
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasShown) return;

        if (other.CompareTag("Player"))
        {
            PlayerControllerTest current = other.GetComponent<PlayerControllerTest>();
            if (current != null)
            {
                // ★設置者かつ、同じリスポーンカウントのときは表示しない
                if (other.gameObject == owner && current.GetRespawnCount() == ownerRespawnCount)
                {
                    Debug.Log("設置者本人（リスポーン前）なので表示しない");
                    return;
                }
            }

            hasShown = true;
            Debug.Log("警告UIを表示します！");
            WarningUIController.Instance?.ShowWarning();
        }
    }
}
