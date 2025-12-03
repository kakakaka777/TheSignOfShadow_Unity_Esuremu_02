using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Camera playerCamera; // プレイヤーのカメラ
    public Texture2D brushTexture; // 描画に使用するブラシテクスチャ（丸、四角など）
    public Color paintColor = Color.red; // 描画色
    public float brushSize = 0.1f; // ブラシのサイズ (UV空間での相対的なサイズ)
    public float paintStrength = 1.0f; // 描画の濃さ

    private RaycastHit hitInfo;
    private Dictionary<Renderer, Texture2D> originalTextures = new Dictionary<Renderer, Texture2D>();
    private Dictionary<Renderer, RenderTexture> paintRenderTextures = new Dictionary<Renderer, RenderTexture>();

    void Update()
    {
        if (Input.GetMouseButton(0)) // 左クリックを押し続けている間
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo))
            {
                // 当たったオブジェクトのRendererを取得
                Renderer hitRenderer = hitInfo.collider.GetComponent<Renderer>();
                if (hitRenderer != null)
                {
                    // そのオブジェクトの現在のレンダーテクスチャを取得（なければ作成）
                    RenderTexture paintRT;
                    if (!paintRenderTextures.TryGetValue(hitRenderer, out paintRT))
                    {
                        // オリジナルのテクスチャをバックアップ
                        Texture2D originalTex = hitRenderer.material.mainTexture as Texture2D;
                        if (originalTex == null)
                        {
                            // もしオリジナルがRenderTextureなら、それをコピーして新しいRenderTextureにする
                            RenderTexture originalRT = hitRenderer.material.mainTexture as RenderTexture;
                            if (originalRT != null)
                            {
                                originalTex = ConvertRenderTextureToTexture2D(originalRT);
                            }
                            else
                            {
                                // オリジナルテクスチャがない場合は、純粋な色で初期化されたテクスチャを作成
                                originalTex = new Texture2D(512, 512); // デフォルトサイズ
                                Color[] colors = new Color[originalTex.width * originalTex.height];
                                for (int i = 0; i < colors.Length; i++) colors[i] = Color.white; // 初期色
                                originalTex.SetPixels(colors);
                                originalTex.Apply();
                            }
                        }
                        originalTextures[hitRenderer] = originalTex; // オリジナルをバックアップ

                        paintRT = new RenderTexture(originalTex.width, originalTex.height, 0);
                        paintRT.enableRandomWrite = true;
                        paintRT.Create();

                        // 初期化のためにオリジナルテクスチャをコピー
                        Graphics.Blit(originalTex, paintRT);

                        paintRenderTextures[hitRenderer] = paintRT;
                        hitRenderer.material.mainTexture = paintRT; // マテリアルのメインテクスチャをレンダーテクスチャに設定
                    }

                    // UV座標を取得し、レンダーテクスチャに描画
                    DrawOnRenderTexture(paintRT, hitInfo.lightmapCoord.x, hitInfo.lightmapCoord.y);
                    // hitInfo.textureCoord でテクスチャのUV座標が取得できますが、
                    // Unity 2021以降では `lightmapCoord` の方が正確なUV座標を提供することが多いです。
                    // どちらか適切な方を選んでください。
                }
            }
        }
    }

    private void DrawOnRenderTexture(RenderTexture targetRT, float uvX, float uvY)
    {
        // レンダーテクスチャをアクティブにする
        RenderTexture.active = targetRT;

        // 一時的なテクスチャを作成して、現在のレンダーテクスチャの内容を読み込む
        Texture2D tempTexture = new Texture2D(targetRT.width, targetRT.height, TextureFormat.RGBA32, false);
        tempTexture.ReadPixels(new Rect(0, 0, targetRT.width, targetRT.height), 0, 0);
        tempTexture.Apply();

        // 描画位置をピクセル座標に変換
        int pixelX = Mathf.RoundToInt(uvX * targetRT.width);
        int pixelY = Mathf.RoundToInt(uvY * targetRT.height);

        // ブラシの半径をピクセル単位で計算
        int brushRadiusX = Mathf.RoundToInt(brushSize * targetRT.width / 2f);
        int brushRadiusY = Mathf.RoundToInt(brushSize * targetRT.height / 2f);

        // ブラシの範囲を計算
        int startX = Mathf.Max(0, pixelX - brushRadiusX);
        int endX = Mathf.Min(targetRT.width, pixelX + brushRadiusX);
        int startY = Mathf.Max(0, pixelY - brushRadiusY);
        int endY = Mathf.Min(targetRT.height, pixelY + brushRadiusY);

        // ブラシテクスチャのピクセルデータを取得
        Color[] brushPixels = brushTexture.GetPixels();
        int brushWidth = brushTexture.width;
        int brushHeight = brushTexture.height; // Texture2D.height は推奨されません

        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                // ブラシテクスチャ上の対応する座標
                float u = (float)(x - (pixelX - brushRadiusX)) / (2f * brushRadiusX);
                float v = (float)(y - (pixelY - brushRadiusY)) / (2f * brushRadiusY);

                if (u >= 0 && u <= 1 && v >= 0 && v <= 1) // ブラシの範囲内
                {
                    Color brushPixelColor = brushTexture.GetPixelBilinear(u, v);

                    // 元のピクセルとブラシの色をブレンド
                    Color originalPixelColor = tempTexture.GetPixel(x, y);

                    // ブラシのアルファ値に基づいてブレンド
                    Color blendedColor = Color.Lerp(originalPixelColor, paintColor, brushPixelColor.a * paintStrength);

                    tempTexture.SetPixel(x, y, blendedColor);
                }
            }
        }

        tempTexture.Apply(); // 変更を適用

        // 更新されたテクスチャをレンダーテクスチャに書き戻す
        Graphics.Blit(tempTexture, targetRT);

        // アクティブなレンダーテクスチャを元に戻す
        RenderTexture.active = null;

        // 一時テクスチャを破棄
        Destroy(tempTexture);
    }

    // RenderTextureをTexture2Dに変換するヘルパー関数
    private Texture2D ConvertRenderTextureToTexture2D(RenderTexture rt)
    {
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        return tex;
    }

    void OnApplicationQuit()
    {
        // アプリケーション終了時に生成したRenderTextureを解放
        foreach (var entry in paintRenderTextures)
        {
            entry.Value.Release();
            Destroy(entry.Value);
        }
    }
}
