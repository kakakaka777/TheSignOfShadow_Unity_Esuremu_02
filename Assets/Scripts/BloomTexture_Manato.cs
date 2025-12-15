using UnityEngine;

[ExecuteInEditMode]
public class BloomTexture : MonoBehaviour
{
    [SerializeField]
    private Shader _shader;
    // 全体の強度
    [SerializeField, Range(0, 1f)]
    public float _strength = 0.3f;
    // ブラーの強度
    [SerializeField, Range(1, 64)]
    public int _blur = 20;
    // 明るさのしきい値
    [SerializeField, Range(0, 1f)]
    public float _threshold = 0.3f;
    // RenderTextureサイズの分母
    [SerializeField, Range(1, 12)]
    public int _ratio = 1;

    [SerializeField, Range(1f, 10f)]
    private float _offset = 1f;

    private Material _material;
    private float[] _weights = new float[10];

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_material == null)
        {
            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;
        }
        _OnRenderImage(src, dest);
    }

    private void _OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        int renderTextureX = src.width / _ratio;
        int renderTextureY = src.height / _ratio;
        RenderTexture tmp = CreateRenderTexture(renderTextureX, renderTextureY);
        RenderTexture tmp2 = CreateRenderTexture(renderTextureX, renderTextureY);

        // Bloom
        _material.SetFloat("_Strength", _strength);
        _material.SetFloat("_Threshold", _threshold);
        _material.SetFloat("_Blur", _blur);
        _material.SetTexture("_Tmp", tmp);
        Graphics.Blit(src, tmp, _material, 0);

        // ガウシアンブラー
        UpdateWeights();
        _material.SetFloatArray("_Weights", _weights);
        float x = _offset / tmp2.width;
        float y = _offset / tmp2.height;
        _material.SetVector("_Offset", new Vector4(x, 0, 0, 0));
        Graphics.Blit(src, tmp2, _material, 1);
        _material.SetVector("_Offset", new Vector4(0, y, 0, 0));
        Graphics.Blit(tmp2, dest, _material, 1);

        RenderTexture.ReleaseTemporary(tmp);
        RenderTexture.ReleaseTemporary(tmp2);
    }

    /// <summary>
    /// RenderTextureの生成
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    private RenderTexture CreateRenderTexture(int width, int height)
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
        renderTexture.filterMode = FilterMode.Bilinear;
        return renderTexture;
    }

    /// <summary>
    /// 重みの計算
    /// </summary>
    private void UpdateWeights()
    {
        float total = 0;
        float d = _blur * _blur * 0.01f;
        for (int i = 0; i < _weights.Length; i++)
        {
            float x = 1.0f + i * 2f;
            float w = Mathf.Exp(-0.5f * (x * x) / d);
            _weights[i] = w;
            // xとyがあるので2倍
            if (i > 0)
            {
                w *= 2.0f;
            }
            total += w;
        }
        // 正規化
        for (int i = 0; i < _weights.Length; i++)
        {
            _weights[i] /= total;
        }
    }
}