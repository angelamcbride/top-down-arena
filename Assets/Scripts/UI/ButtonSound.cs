using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverSound();
    }

    public void HoverSound()
    {
        AudioManager.Instance.PlaySound("Beep");
    }
    public void ButtonClickSound()
    {
        AudioManager.Instance.PlaySound("Beep");
    }
}
