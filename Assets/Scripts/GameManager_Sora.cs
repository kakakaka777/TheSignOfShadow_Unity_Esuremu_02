using UnityEngine;

public class GameManager_Sora : MonoBehaviour
{
    public static GameManager_Sora Instance;

    public bool hasKey = false;

    void Awake()
    {
        // ★複数置かれても1つに統一（鍵がリセットされる事故を防ぐ）
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // シーン切替があるなら必須
    }

    public void GetKey()
    {
        hasKey = true;
        Debug.Log("鍵を入手しました！ hasKey = true");
    }
}
