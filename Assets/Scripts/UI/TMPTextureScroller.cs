using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMPTextureScroller : MonoBehaviour
{
    public Vector2 scrollSpeed = new Vector2(0.5f, 0f);

    private TMP_Text tmpText;
    private Material material;
    private Vector2 offset;

    void Start()
    {
        tmpText = GetComponent<TMP_Text>();
        if (tmpText != null)
        {
            material = tmpText.fontMaterial;
        }
    }

    void Update()
    {
        if (material != null)
        {
            offset += scrollSpeed * Time.deltaTime;
            // TMPのフェイステクスチャオフセット
            material.SetTextureOffset("_FaceTex", offset);
        }
    }
}
