using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuuka : MonoBehaviour
{

    [Header("フェード設定")]
    [Min(0f)] public float fadeDuration = 4f;   // 透明になるまでの秒数
    [Min(0f)] public float startDelay = 0f;     // フェード開始までの待ち時間（不要なら0）
    
    private float startAlpha = 1.0f;

    SpriteRenderer spriteRenderer;
    //Renderer[] meshRenderers;
    //CanvasGroup canvasGroup;

    void Awake()
    {
        //canvasGroup = GetComponentInChildren<CanvasGroup>(true);
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        //meshRenderers = GetComponentsInChildren<Renderer>(true);
    }

    void OnEnable()
    {
        StartCoroutine(FadeOutThenDestroy());
    }

    IEnumerator FadeOutThenDestroy()
    {
        if (startDelay > 0f)
        {
            yield return new WaitForSeconds(startDelay);
        }
        
        // 初期アルファ取得（UIがあればUI優先）
        //if (canvasGroup != null) startAlpha = canvasGroup.alpha;
        else if (spriteRenderer !=  null) startAlpha = spriteRenderer.color.a;
        //else if (meshRenderers.Length > 0 && meshRenderers[0].sharedMaterial != null && meshRenderers[0].sharedMaterial.HasProperty("_Color"))
        //    startAlpha = meshRenderers[0].sharedMaterial.color.a;

        float t = 0f;

        // 0秒なら即消す
        if (fadeDuration <= 0f)
        {
            SetAlpha(0f);
            Destroy(gameObject);
            yield break;
        }

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            SetAlpha(a);
            yield return null;
        }

        SetAlpha(0f);
        Destroy(this.gameObject);
    }

    void SetAlpha(float a)
    {
        //// UI
        //if (canvasGroup != null) canvasGroup.alpha = a;

        // 2D Sprite
        //for (int i = 0; i < spriteRenderers.Length; i++)
        //{
        //    var c = spriteRenderers[i].color;
        //    c.a = a;
        //    spriteRenderers[i].color = c;
        //}

        var c = spriteRenderer.color;
        c.a = a;
        spriteRenderer.color = c;

        //// 3D Mesh（※マテリアル側が透明対応シェーダーじゃないと見た目は変わりません）
        //for (int i = 0; i < meshRenderers.Length; i++)
        //{
        //    var r = meshRenderers[i];
        //    if (r == null) continue;

        //    foreach (var m in r.materials) // material: インスタンス化される（個別に透過させたい時はこれでOK）
        //    {
        //        if (m != null && m.HasProperty("_Color"))
        //        {
        //            var c = m.color;
        //            c.a = a;
        //            m.color = c;
        //        }
        //    }
        //}
    }
}
