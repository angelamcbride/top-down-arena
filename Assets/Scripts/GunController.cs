using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

	public float speed = 10;
	public float damageMultiplier = 5;
	public string ammoType = "silverBullet";
    public float StaminaCostPerBullet = 10f;
    public float knockback = 10f;
    private float fireCooldown = 0.2f;
    private float lastFireTime = 0f;

    public GameObject player; // Ammo needs to know this, so the player can't accidentally shoot themselves.
    private StaminaController staminaController;
    private CameraController cameraController;
    

    private void Start()
    {
        SetOwner();
    }

    private void SetOwner()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        staminaController = player.GetComponent<StaminaController>();
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
    }

    Vector2 PlayerLookVector()
	{
        Vector2 difference = Camera.main.ScreenToWorldPoint (Input.mousePosition) - transform.root.position; // difference between mouse pos and character.
		difference.Normalize(); // make 0-1
		return new Vector2 (difference.x,difference.y);
	}

	void PlayerInputs()
	{
		if (Input.GetMouseButtonDown (0) && !GameStateManager.Instance.IsPaused)
		{
            ShootGun(player);
            //StartCoroutine(cameraController.CameraShake(0.5f, 0.1f, 0.05f));
        }
	}

    private void ShootGun(GameObject gunOwner)
    {
        if(staminaController.CurrentStamina > 0  && Time.time > lastFireTime + fireCooldown)
        {
            staminaController.UseStamina(StaminaCostPerBullet);
            AudioManager.Instance.PlaySound("Gunshot");
            Transform shootFrom = this.gameObject.transform.GetChild(0);
            GameObject bullet = (GameObject)Instantiate(Resources.Load("prefabs/ammo/ammo_" + ammoType));
            bullet.transform.position = shootFrom.position;
            bullet.GetComponent<AmmoController>().ShootAmmo(damageMultiplier, speed, ammoType, PlayerLookVector(), gunOwner, knockback);
        }
    }

	void Update () 
	{
        if(!GameStateManager.Instance.IsPaused)
        {
            PlayerInputs();
        }
	}
}
