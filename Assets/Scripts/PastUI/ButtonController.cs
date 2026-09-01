using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 控制UI按钮的动画和声音效果
/// </summary>
public class ButtonController : MonoBehaviour,IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        anim.Play("ButtonEnter");
        AudioManager.Instance.PlaySound("UI/ui_change_selection",false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            anim.Play("ButtonPressed");
            AudioManager.Instance.PlaySound("UI/ui_button_confirm",false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.Play("ButtonExit");
    }
}
