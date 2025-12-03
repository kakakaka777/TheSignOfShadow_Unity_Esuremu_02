using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkingMenuUI : MonoBehaviour
{
    public GameObject menuRoot;
    public Image topImage;
    public Image rightImage;
    public Image bottomImage;
    public Image leftImage;

    private Image[] directionImages;

    private void Awake()
    {
        // directionImages配列をAwakeでまとめておく
        directionImages = new Image[] { topImage, rightImage, bottomImage, leftImage };

        // menuRootがnullなら警告を出して非表示にする
        if (menuRoot != null)
        {
            menuRoot.SetActive(false);
        }
        else
        {
            Debug.LogWarning("MarkingMenuUI: menuRootが設定されていません。");
        }
    }

    // マーキングメニューを表示・更新
    public void ShowMenu(List<Color> colors, int selectedIndex)
    {
        if (menuRoot == null)
        {
            Debug.LogWarning("MarkingMenuUI: ShowMenu 呼び出し時に menuRoot が null です。");
            return;
        }

        menuRoot.SetActive(true);

        for (int i = 0; i < directionImages.Length; i++)
        {
            // nullチェックを追加
            if (directionImages[i] == null)
            {
                Debug.LogWarning($"MarkingMenuUI: directionImages[{i}] が設定されていません。");
                continue;
            }

            if (i < colors.Count)
            {
                directionImages[i].gameObject.SetActive(true);
                directionImages[i].color = colors[i];
                // 選択中の色は少し大きく表示
                directionImages[i].transform.localScale = (i == selectedIndex) ? Vector3.one * 1.2f : Vector3.one;
            }
            else
            {
                directionImages[i].gameObject.SetActive(false);
            }
        }
    }

    // マーキングメニューを非表示にする
    public void HideMenu()
    {
        if (menuRoot == null)
        {
            Debug.LogWarning("MarkingMenuUI: HideMenu 呼び出し時に menuRoot が null です。");
            return;
        }
        menuRoot.SetActive(false);
    }
}