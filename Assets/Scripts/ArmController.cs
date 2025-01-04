using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour {

	private float pxWidth = 0.0625f; // pixel size
	private int armOffsetNormalY = -0; // in pixels
	private int armOffsetflopY = -0; // in pixels
	private int armOffsetNormalX = -0; // in pixels
	private float armOffsetFlopX = 6; // in pixels
    public float equipmentRotationOffset;

    private SpriteRenderer equipmentSprite;
    private SpriteRenderer armSprite;

    private float startRotation;
    private float endRotation;
    private bool isSwinging;

    private enum Direction { Up, Down, Left, Right }

    private Direction GetDirection(Vector2 difference)
    {
        float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        if (angle > -45 && angle <= 45) return Direction.Right; // 1:30 -> 4:30, char looking right. 
        if (angle > 45 && angle <= 135) return Direction.Up;
        if (angle > 135 || angle <= -135) return Direction.Left;
        return Direction.Down; // else char is looking down
    }

    void SetSpriteOrderAndOffset() // Determine the direction char/arm is aiming. Flop/layer the arm accordiningly.
	{
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        difference.z = 0; // Ignore the Z-axis for 2D

        Direction dir = GetDirection(new Vector2(difference.x, difference.y));

        switch (dir)
        {
            case Direction.Right: // Character looking right
                transform.localScale = new Vector3(1, 1, 1);
                transform.localPosition = new Vector2(armOffsetNormalX, armOffsetNormalY) * pxWidth;
                armSprite.sortingOrder = 4;
                equipmentSprite.sortingOrder = 3;
                break;

            case Direction.Down: // Character looking down
                transform.localScale = new Vector3(1, 1, 1);
                transform.localPosition = new Vector2(armOffsetNormalX, armOffsetNormalY) * pxWidth;
                armSprite.sortingOrder = 4;
                equipmentSprite.sortingOrder = 3;
                break;

            case Direction.Up: // Character looking up
                transform.localScale = new Vector3(1, -1, 1);
                transform.localPosition = new Vector2(armOffsetFlopX, armOffsetflopY) * pxWidth;
                armSprite.sortingOrder = 1;
                equipmentSprite.sortingOrder = 0;
                break;

            case Direction.Left: // Character looking left
                transform.localScale = new Vector3(1, -1, 1);
                transform.localPosition = new Vector2(armOffsetNormalX, armOffsetNormalY) * pxWidth;
                armSprite.sortingOrder = 0;
                equipmentSprite.sortingOrder = 1;
                break;
        }
	}

    public void Swing(float swingRange, float swingSpeed, string weaponTag) // not currently used
    {
        if (isSwinging == false)
        {
            isSwinging = true;
            startRotation = (this.transform.rotation.z - swingRange / 2); // start swigning sword (half of swing range) back from where it is now.
            endRotation = (startRotation + swingRange); // end swing after swing has gone its entire range.
        }
    }

    void SetRotation() // Based on mouse position, set the rotation of the arm (aim at mouse).
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.root.position; // difference between mouse pos and character.
        difference.Normalize(); // make 0-1
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg; // find angle in degrees
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
    }

    void Start()
	{
        armSprite = GetComponent<SpriteRenderer>();
        Transform equipmentSlotR = transform.Find("equipmentSlotR");
        if (equipmentSlotR == null)
        {
            Debug.LogError("equipmentSlotR not found under this object!");
            return;
        }

        Transform equipment = equipmentSlotR.GetChild(0);

        if (equipment == null)
        {
            Debug.LogError("No equipment found under equipmentSlotR!");
            return;
        }
        equipmentSprite = equipment.GetComponent<SpriteRenderer>();
    }

    void Update () 
	{
        SetSpriteOrderAndOffset();
        SetRotation();
    }
}
