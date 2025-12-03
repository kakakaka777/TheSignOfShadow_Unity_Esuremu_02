using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [Header("ˆÚ“®‚·‚éƒV[ƒ“‚Ì–¼‘O")]
    [SerializeField] string sceneName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frames
    void Update()
    {
        
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
