using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerUI : MonoBehaviour
{
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;
    public Image previewImage;

    void Update()
    {
        Color color = new Color(redSlider.value, greenSlider.value, blueSlider.value);
        previewImage.color = color;
    }

    public void OnAddToPaletteButton()
    {
        Color color = new Color(redSlider.value, greenSlider.value, blueSlider.value);
        ColorPaletteManager.Instance.AddColor(color);
        Debug.Log("色をパレットに追加: " + color);
    }
}