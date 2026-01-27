using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject unlockPanel;
    public TextMeshProUGUI unlockText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockMessage(string messageName)
    {
        unlockPanel.SetActive(true);
        unlockText.text =
            "新たなメッセージ機能:\n「" + messageName + "」が解除された";
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        unlockPanel.SetActive(false);
    }
}
