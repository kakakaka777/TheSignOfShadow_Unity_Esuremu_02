using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    ISelectHandler, IDeselectHandler
{
    [SerializeField] private GameObject hoverFrame; // 枠Image

    private void Awake()
    {
        if (hoverFrame != null)
        {
            hoverFrame.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverFrame != null)
            hoverFrame.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverFrame != null)
            hoverFrame.SetActive(false);
    }

    // キーボード操作やゲームパッドで選択されたときも光らせる場合
    public void OnSelect(BaseEventData eventData)
    {
        if (hoverFrame != null)
            hoverFrame.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (hoverFrame != null)
            hoverFrame.SetActive(false);
    }
}
