using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

	public float speed = 10;
	public float damageMultiplier = 5;
	public string ammoType = "silverBullet";
    public GameObject GunOwner; // Ammo needs to know this so the player can't accidentally shoot themselves.

    private GameObject FindPlayerObject() // Super janky implementation
    {
        Transform current = transform;

        // Traverse up 4 levels
        for (int i = 0; i < 4; i++)
        {
            if (current.parent != null)
            {
                current = current.parent;
            }
            else
            {
                Debug.LogError("Can't find the player!");
            }
        }
        return current.gameObject;
    }

	Vector2 PlayerLookVector()
	{
        GunOwner = FindPlayerObject();

        Vector2 difference = Camera.main.ScreenToWorldPoint (Input.mousePosition) - transform.root.position; // difference between mouse pos and character.
		difference.Normalize(); // make 0-1
		return new Vector2 (difference.x,difference.y);
	}

	void PlayerInputs()
	{
		if (Input.GetMouseButtonDown (0))
		{
            AudioManager.Instance.PlaySound("Gunshot");
            Transform shootFrom = this.gameObject.transform.GetChild(0);
			GameObject bullet = (GameObject)Instantiate (Resources.Load ("prefabs/ammo/ammo_" + ammoType));
			bullet.transform.position = shootFrom.position;
			bullet.GetComponent<AmmoController>().ShootAmmo(damageMultiplier, speed, ammoType, PlayerLookVector(), GunOwner);
		}
	}

	void Update () 
	{
		PlayerInputs ();
	}
}
