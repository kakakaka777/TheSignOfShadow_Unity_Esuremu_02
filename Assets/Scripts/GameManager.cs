using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool hasKey = false;

    void Awake()
    {
        Instance = this;
    }

    public void GetKey()
    {
        hasKey = true;
        Debug.Log("Œ®‚ğ“üè‚µ‚Ü‚µ‚½I");
    }
}
