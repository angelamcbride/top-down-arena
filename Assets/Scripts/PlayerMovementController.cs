using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour {
	private float speed_walking = 3f; //in seconds
	private float speed_running = 5f;
	private float speed_dashing = 25f;
	private float dashRate = .05f;
	private float tapSpeed = 0.5f;
	private float shortJumpRate = .4f;
	private float longJumpRate = .65f;

	public KeyCode upKey = KeyCode.W;
	public KeyCode downKey = KeyCode.S;
	public KeyCode leftKey = KeyCode.A;
	public KeyCode rightKey = KeyCode.D;
	public KeyCode jumpKey = KeyCode.Space;
	public KeyCode dashKey = KeyCode.LeftAlt;
	public bool lockAxis = true;

	public string state;

	private Animator anim;
	private Animator anim_arm;
	private Rigidbody2D rb2d;

	private float speed;
	private string axis;

	private float lastTapTime;
	private float lastJumpTime;
	private float lastDashTime;


	private Transform ChildWithTag(string tag)
	{
		Transform childFound = null;
		foreach (Transform child in transform)
			if (child.CompareTag (tag))
			{
				childFound = child;
			} 
		if (childFound == null)
		{
			Debug.Log("no child found");
		}
		return childFound;
	}


	// Set the movement speed of the character. Inform the animation controller, so that the characters animation matches the speed.
	void SetSpeed ()
	{
		anim.SetBool ("isWalking", false); //Reset everything to false.
		anim.SetBool ("isRunning", false);
		anim.SetBool ("isDashing", false);
		anim_arm.SetBool ("isWalking", false);
		anim_arm.SetBool ("isRunning", false);

		switch(state)
		{
		case "walking":
			speed = speed_walking;
			anim.SetBool ("isWalking", true);
			anim_arm.SetBool ("isWalking", true);
			break;
		case "running":
			speed = speed_running;
			anim.SetBool ("isRunning", true);
			anim_arm.SetBool ("isRunning", true);
			break;
		case "dashing":
			speed = speed_dashing;
			anim.SetBool ("isDashing", true);
			break;
		default:
			speed = 0;
			break;
		}
	}

	//Only allow X or Y axis to be used one at a time. i.e. If moving on X last, Y is 0 even if two keys are held down.
	void SetAxis()
	{
		if (Input.GetKeyDown (upKey) || Input.GetKeyDown (downKey)) //Lock Axis Y if up/down is pressed last to avoid diagonal movement.
		{
			axis = "Y";
		} else if (Input.GetKeyDown (leftKey) || Input.GetKeyDown (rightKey)) //Lock Axis X if left/right is pressed last to avoid diagonal movement.
		{
			axis = "X";
		} 
	}

	void LookAtDir() //Make the character look in the direction of the mouse
	{
		Vector3 difference = Camera.main.ScreenToWorldPoint (Input.mousePosition) - transform.position; //difference between mouse pos and character.
		difference.Normalize(); //make 0-1
		float rotationZ = Mathf.Atan2 (difference.y, difference.x) * Mathf.Rad2Deg; //find angle in degrees
		if (rotationZ < 45 && rotationZ > -45)//1:30 -> 4:30, char looking right. 
		{
			anim.SetFloat ("x", 1);
			anim.SetFloat ("y", 0);
		} 
		else if (rotationZ < -45 && rotationZ > -100)//4:30 -> 7:30, char looking down. 
		{
			anim.SetFloat ("x", 0);
			anim.SetFloat ("y", -1);
		} 
		else if (rotationZ < 135 && rotationZ > 45) //char looking up
		{
			anim.SetFloat ("x", 0);
			anim.SetFloat ("y", 1);
		} 
		else //char looking left
		{
			anim.SetFloat ("x", -1);
			anim.SetFloat ("y", 0);
		}
	}

	void MovePlayer()
	{
		float input_x = Input.GetAxisRaw ("Horizontal");
		float input_y = Input.GetAxisRaw ("Vertical");
		Vector2 movement_vector = new Vector2 (input_x, input_y);

		if (movement_vector != Vector2.zero)
		{
			if (axis == "Y" && lockAxis == true) //Lock axis to Y
			{
				rb2d.MovePosition (rb2d.position + (new Vector2 (0, input_y) * Time.deltaTime * speed));
			}

			else if (axis == "X" && lockAxis == true) //Lock axis to X
			{
				rb2d.MovePosition (rb2d.position + (new Vector2 (input_x, 0) * Time.deltaTime * speed));
			} 
			else if (lockAxis == false)
			{
				Vector2 move = new Vector2 (input_x, input_y);
				rb2d.MovePosition (rb2d.position + (Vector2.ClampMagnitude(move, 1) * Time.deltaTime * speed));
			}
		} 
		else
		{
			state = "idle";
		}
	}		

	private IEnumerator dash(string prevState) //Make player super speedy for short period of time (dashRate)
	{
		state = "dashing";
		yield return new WaitForSeconds(dashRate);
		state = prevState;
	}

	void CollideOffDuringJump()
	{
		if(Time.time > lastJumpTime) //if player is jumping
		{
			this.gameObject.layer = 8; //Normal player layer
		}
		else
		{
			this.gameObject.layer = 12; //Jumping layer
		}
	}

	void PlayerInput()
	{
		///// WALK/RUN /////
		if (Input.GetKeyDown (upKey) || Input.GetKeyDown (downKey) || Input.GetKeyDown (leftKey) || Input.GetKeyDown (rightKey))
		{
			if (Time.time - lastTapTime < tapSpeed)
			{
				state = "running";
			}
			else
			{
				state = "walking";
			}
			lastTapTime = Time.time;
		}

		///// DASH /////
		if (Input.GetKeyDown (dashKey)) //&& (Time.time - lastDashTime < dashRate))
		{
			StartCoroutine (dash (state));
		}

		///// JUMP/SHORTJUMP /////
		if (Input.GetKeyDown(jumpKey) && Time.time > lastJumpTime)  //If jump key is pressed and char is not already jumping.
		{
            AudioManager.Instance.PlaySound("Jump");
            anim.SetBool ("shortJump", false);
			anim.Play ("Long Jump Blend Tree");
			lastJumpTime = Time.time + longJumpRate; //Add cooldown to timer.
		}
		if (Input.GetKeyUp (jumpKey) && Time.time < lastJumpTime && lastJumpTime - Time.time > (longJumpRate/3)*2) //Short jump if key is un-pressed during first 3rd of jump.
		{
			lastJumpTime -= (longJumpRate - shortJumpRate); //subtract time from jump cooldown timer, because jump was shortened.
			anim.SetBool ("shortJump", true);
		}
		anim.SetBool ("isJumping", Time.time < lastJumpTime);
		anim_arm.SetBool ("isJumping",  Time.time < lastJumpTime);
			
	}
	void Start() 
	{
		rb2d = gameObject.GetComponentInParent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		anim_arm = ChildWithTag("playerArmR").gameObject.GetComponent<Animator>();
		state = "idle";
	}

	void FixedUpdate()  
	{
		MovePlayer ();
	}

	void Update () 
	{
		SetSpeed();
		SetAxis();
		PlayerInput();
		LookAtDir();
		CollideOffDuringJump();
	}
}
