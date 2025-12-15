using UnityEngine;

public class KeyItem : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GetKey();  // Œ®‚ğ’Ç‰Á

            Destroy(gameObject); // Œ®‚ğÁ‚·
        }
    }
}
