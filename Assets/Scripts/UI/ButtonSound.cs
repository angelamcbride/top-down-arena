using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound();
    }

    public void PlaySound()
    {
        AudioManager.Instance.PlaySound("PlayerHit");
    }
}
