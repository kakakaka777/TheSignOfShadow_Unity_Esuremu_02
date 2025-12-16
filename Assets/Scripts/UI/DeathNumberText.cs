using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathNumberText : MonoBehaviour
{
    [SerializeField] private TMP_Text deathText;
    [SerializeField] private string prefix = "Deaths: ";

    

    // Start is called before the first frame update

    private void Awake()
    {
        deathText = GetComponent<TMP_Text>();
    }
    void Start()
    {
        
    }

    private void Update()
    {
        deathText.text = prefix + PlayerManager.deathNumber.ToString();
    }
}
    
