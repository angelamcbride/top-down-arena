using UnityEngine;

public class Heart : MonoBehaviour
{
    public float healthReplenishAmount = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySound("tone");
            HealthController playerHealth = other.transform.ChildWithTag("healthBar").GetComponent<HealthController>();
            if (playerHealth != null)
            {
                playerHealth.ModifyHealth(healthReplenishAmount);
            }
            Destroy(gameObject);
        }
    }
}
