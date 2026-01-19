using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UiPulse : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] float speed = 2f;
    [SerializeField] float intensity = 0.5f; // 0~1

    Color baseColor;

    void Awake()
    {
        if (!img) img = GetComponent<Image>();
        baseColor = img.color;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f; // 0..1
        float k = 1f + t * intensity; // 1..(1+intensity)
        img.color = new Color(baseColor.r * k, baseColor.g * k, baseColor.b * k, baseColor.a);
    }
}
