using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAnimation : MonoBehaviour 
{
	//public Animation anim;
	IEnumerator Start() 
	{
		GetComponent<Animator>().Play("explosion");
		yield return new WaitForSeconds(.65f);
		Object.Destroy (this.gameObject);
	}

}
