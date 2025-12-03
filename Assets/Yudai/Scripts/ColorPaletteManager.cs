using System.Collections.Generic;
using UnityEngine;

public class ColorPaletteManager : MonoBehaviour
{
    public static ColorPaletteManager Instance;

    private List<Color> savedColors = new List<Color>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddColor(Color color)
    {
        if (!savedColors.Contains(color) && savedColors.Count < 4)
        {
            savedColors.Add(color);
        }
    }

    public List<Color> GetColors()
    {
        return savedColors;
    }
}