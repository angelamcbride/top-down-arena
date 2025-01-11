using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    public float MaxStamina = 100f;
    public float StaminaRegenRate = 30f; // Stamina points per second
    private float DelayAfterDepletion = 2f;
    public float CurrentStamina;

    public GameObject StaminaBar; // The shrinking bar UI
    public GameObject EmptySpace; // The flashing background UI
    public Color FlashColor1 = Color.red; // First flash color
    public Color FlashColor2 = Color.yellow; // Second flash color
    private Color EmptyColor = new Color(0.07381579f, 0.06710526f, 0.085f);
    private Image emptySpaceImage; // Reference to the empty space's Image component
    private bool isRegenDelayed = false;

    private void Start()
    {
        CurrentStamina = MaxStamina;

        // Cache the Image component for flashing
        if (EmptySpace != null)
        {
            emptySpaceImage = EmptySpace.GetComponent<Image>();
        }

        UpdateStaminaBar();
    }

    private void UpdateStaminaBar()
    {
        if (StaminaBar != null)
        {
            StaminaBar.transform.localScale = new Vector3(CurrentStamina / MaxStamina, 1, 1);
        }
    }

    public void UseStamina(float amount)
    {
        if (isRegenDelayed) return;

        CurrentStamina -= amount;
        if (CurrentStamina <= 0)
        {
            CurrentStamina = 0;
            StartCoroutine(DelayRegen());
        }
        UpdateStaminaBar();
    }

    private IEnumerator DelayRegen()
    {
        isRegenDelayed = true;
        if (EmptySpace != null)
        {
            StartCoroutine(FlashEmptySpace());
        }
        yield return new WaitForSeconds(DelayAfterDepletion);
        isRegenDelayed = false;
    }

    private IEnumerator FlashEmptySpace()
    {
        if (emptySpaceImage == null) yield break;

        for (int i = 0; i < 6; i++) // Flash for 3 seconds (6 flashes, alternating)
        {
            emptySpaceImage.color = i % 2 == 0 ? FlashColor1 : FlashColor2;
            yield return new WaitForSeconds(0.25f);
        }
        emptySpaceImage.color = EmptyColor;
    }

    private void FixedUpdate()
    {
        if (!isRegenDelayed && CurrentStamina < MaxStamina)
        {
            CurrentStamina += (StaminaRegenRate / 1f) * Time.fixedDeltaTime;
            CurrentStamina = Mathf.Min(CurrentStamina, MaxStamina);
            UpdateStaminaBar();
        }
    }
}
