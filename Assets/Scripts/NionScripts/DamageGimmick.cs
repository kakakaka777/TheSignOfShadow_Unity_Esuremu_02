using UnityEngine;

public class DamageGimmick : MonoBehaviour
{
    public float damageAmount = 50f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Œ»İ‚ÌHP‚Éƒ_ƒ[ƒW‚ğ—^‚¦‚é
                player.currentHP -= damageAmount;

                // HP‚ª0ˆÈ‰º‚Å‚È‚¯‚ê‚Î€–Só‘Ô‚É“ü‚ç‚È‚¢‚æ‚¤’²®
                if (player.currentHP <= 0 && !player.gameObject.activeSelf)
                {
                    // ‚·‚Å‚É€‚ñ‚Å‚éê‡‚Í–³‹
                    return;
                }

                // ƒvƒŒƒCƒ„[‚Ì€–Sˆ—‚ª©“®‚Å“ü‚Á‚Ä‚é‚Ì‚Å”C‚¹‚ÄOK
            }
        }
    }
}
