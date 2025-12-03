using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodTextMessage : MessageFunction
{
    [Header("描画設定")]
    public Camera playerCamera;
    public Texture2D brushTexture;
    public Color paintColor = Color.red;
    public float brushSize = 0.1f;
    public float paintStrength = 1f;
    public float maxDistance = 5f;

    private RaycastHit hitInfo;
    private Dictionary<Renderer, Texture2D> originalTextures = new();
    private Dictionary<Renderer, RenderTexture> paintRenderTextures = new();

    private bool isActivate = false;

    void Update()
    {
        // 左クリックで印を残す
        if (Input.GetMouseButton(0) && isActivate == true)
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, maxDistance))
            {
                Renderer hitRenderer = hitInfo.collider.GetComponent<Renderer>();
                if (hitRenderer != null) PaintToSurface(hitRenderer, hitInfo);
            }
        }
    }

    public override void Activate(Vector3 playerPosition)
    {
        // UIから発動：プレイヤーの正面に印を残す
        Vector3 origin = playerPosition + Vector3.up * 1f;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out hitInfo, maxDistance))
        {
            Renderer hitRenderer = hitInfo.collider.GetComponent<Renderer>();
            if (hitRenderer != null) PaintToSurface(hitRenderer, hitInfo);
        }

        isActivate = true;
    }

    void PaintToSurface(Renderer renderer, RaycastHit hit)
    {
        if (!paintRenderTextures.TryGetValue(renderer, out RenderTexture paintRT))
        {
            Texture2D originalTex = renderer.material.mainTexture as Texture2D;

            if (originalTex == null)
            {
                RenderTexture originalRT = renderer.material.mainTexture as RenderTexture;
                if (originalRT != null) originalTex = ConvertRenderTextureToTexture2D(originalRT);
                else
                {
                    originalTex = new Texture2D(512, 512);
                    Color[] colors = new Color[originalTex.width * originalTex.height];
                    for (int i = 0; i < colors.Length; i++) colors[i] = Color.white;
                    originalTex.SetPixels(colors); originalTex.Apply();
                }
            }

            originalTextures[renderer] = originalTex;
            paintRT = new RenderTexture(originalTex.width, originalTex.height, 0) { enableRandomWrite = true };
            paintRT.Create();

            Graphics.Blit(originalTex, paintRT);
            paintRenderTextures[renderer] = paintRT;
            renderer.material.mainTexture = paintRT;
        }

        DrawOnRenderTexture(paintRT, hit.lightmapCoord.x, hit.lightmapCoord.y);
    }

    void DrawOnRenderTexture(RenderTexture targetRT, float uvX, float uvY)
    {
        RenderTexture.active = targetRT;

        Texture2D tempTexture = new Texture2D(targetRT.width, targetRT.height, TextureFormat.RGBA32, false);
        tempTexture.ReadPixels(new Rect(0, 0, targetRT.width, targetRT.height), 0, 0); tempTexture.Apply();

        int pixelX = Mathf.RoundToInt(uvX * targetRT.width);
        int pixelY = Mathf.RoundToInt(uvY * targetRT.height);

        int brushRadiusX = Mathf.RoundToInt(brushSize * targetRT.width / 2f);
        int brushRadiusY = Mathf.RoundToInt(brushSize * targetRT.height / 2f);

        int startX = Mathf.Max(0, pixelX - brushRadiusX);
        int endX = Mathf.Min(targetRT.width, pixelX + brushRadiusX);
        int startY = Mathf.Max(0, pixelY - brushRadiusY);
        int endY = Mathf.Min(targetRT.height, pixelY + brushRadiusY);

        Color[] brushPixels = brushTexture.GetPixels();
        int brushWidth = brushTexture.width;
        int brushHeight = brushTexture.height;

        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                float u = (float)(x - (pixelX - brushRadiusX)) / (2f * brushRadiusX);
                float v = (float)(y - (pixelY - brushRadiusY)) / (2f * brushRadiusY);

                if (u >= 0 && u <= 1 && v >= 0 && v <= 1)
                {
                    Color brushPixelColor = brushTexture.GetPixelBilinear(u, v);
                    Color originalPixelColor = tempTexture.GetPixel(x, y);
                    Color blendedColor = Color.Lerp(originalPixelColor, paintColor, brushPixelColor.a * paintStrength);
                    tempTexture.SetPixel(x, y, blendedColor);
                }
            }
        }

        tempTexture.Apply();
        Graphics.Blit(tempTexture, targetRT);
        RenderTexture.active = null;
        Destroy(tempTexture);
    }

    Texture2D ConvertRenderTextureToTexture2D(RenderTexture rt)
    {
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0); tex.Apply();
        RenderTexture.active = null;
        return tex;
    }

    void OnApplicationQuit()
    {
        foreach (var entry in paintRenderTextures)
        {
            entry.Value.Release();
            Destroy(entry.Value);
        }
    }

}
