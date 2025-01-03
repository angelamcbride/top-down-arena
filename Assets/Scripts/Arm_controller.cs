using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm_controller : MonoBehaviour {

	private float startRotation;
	private float endRotation;
	private bool isSwinging;

	public void swing(float swingRange, float swingSpeed, string weaponTag)
	{
		if (isSwinging == false)
		{
			isSwinging = true;
			startRotation = (this.transform.rotation.z - swingRange / 2); //start swigning sword (half of swing range) back from where it is now.
			endRotation = (startRotation + swingRange); //end swing after swing has gone its entire range.
		}
	}
			
}
