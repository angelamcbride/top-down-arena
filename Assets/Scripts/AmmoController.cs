using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoController : MonoBehaviour 
{
    [SerializeField] private float damage = 5f;
    [SerializeField] private float distanceMax = 10f;
    [SerializeField] private string myAmmoType = "silverBullet";

	private bool shot;
	private float CalculatedDamage;
	private float mySpeed;
	private Vector2 startPos;
	private Vector2 shootVector;
    private GameObject ammoShooter;
    private float knockbackOnImpact;

	private Rigidbody2D rb2d;

	public void ShootAmmo(float damageMultiplier, float speed, string ammoType, Vector2 playerLookVector, GameObject gunOwner, float knockback = 5f)
	{
        ammoShooter = gunOwner;

        if (ammoType == myAmmoType && shot == false)
		{
			CalculatedDamage = damage * damageMultiplier;
			mySpeed = speed;
			startPos = transform.position;
			shootVector = playerLookVector;
            knockbackOnImpact = knockback;
            shot = true;
		}
	}

	void FixedUpdate()
	{
		if (shot == true)
		{
			float distance = Vector2.Distance(startPos, transform.position);
			if (distance > distanceMax)
			{
				Object.Destroy (gameObject);
			} 
			else
			{
				rb2d.MovePosition (rb2d.position + (Vector2.ClampMagnitude (shootVector, 1) * Time.deltaTime * mySpeed));
			}
		}
	}

	void Start () 
	{
		rb2d = GetComponent<Rigidbody2D>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
        if (other.gameObject != ammoShooter) // Don't let the shooter shoot themselves
        {
            if (other.isTrigger)
            {
                GameObject hitObject = other.gameObject;
                if (hitObject.tag == "Player" || other.gameObject.tag == "Enemy")
                {
                    hitObject.transform.parent.GetComponent<Rigidbody2D>().AddForce(shootVector* knockbackOnImpact);
                    HealthController healthScript = other.transform.ChildWithTag("healthBar").GetComponent<HealthController>();
                    if (healthScript != null) //Sometimes health script is null because object is in the process of dying. If it isn't dead/null we can access its health script.
                    {
                        healthScript.ModifyHealth(CalculatedDamage);
                    }
                    else
                    {
                        Debug.Log("Health controller is null");
                    }
                }
            }
            Object.Destroy(gameObject);
        }
	}
}
