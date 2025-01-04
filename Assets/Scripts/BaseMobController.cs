using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMobController : MonoBehaviour
{
    public float mySpeed = 2f;
    public float jumpSpeed = 3f;
    public float collideDamage = -6f;
    public float DamageMultiplier = 5f;
    public string AmmoType = "greenMagic";
    public float AmmoSpeed = 8f;
    public float FireRate = .5f;
    public float TimeBetweenRandomStates = 2f;

    protected GameObject player;
    protected GameObject obsticle;
    protected Rigidbody2D rb2d;
    protected Animator anim;
    protected float currentSpeed;
    protected string state;
    protected float shootTimer = 0;
    protected float stateTimer = 0;
    protected float randomMoveTimer = 0;
    protected Vector2 movement_vector;
    protected List<string> randomActions = new List<string>(new string[] { "chasePlayer", "runAway", "moveRandom" });

    private Transform ChildWithTag(string tag)
    {
        Transform childFound = null;
        foreach (Transform child in transform)
            if (child.CompareTag(tag))
            {
                childFound = child;
            }
        if (childFound == null)
        {
            Debug.Log("no child found");
        }
        return childFound;
    }

    protected Vector2 LookAtPlayer()
    {
        Vector2 difference = player.transform.position - transform.root.position - new Vector3(0, .5f, 0); // difference between mouse pos and character.
        difference.Normalize(); //make 0-1
        return new Vector2(difference.x, difference.y);
    }

    protected float DistanceFromPlayer()
    {
        return Vector2.Distance(player.transform.position, transform.root.position);
    }

    protected void ChasePlayer()
    {
        if (DistanceFromPlayer() < 2f && DistanceFromPlayer() > 0f) // slow down during approach
        {
            movement_vector = LookAtPlayer();
            rb2d.MovePosition(rb2d.position + (Vector2.ClampMagnitude(movement_vector, 1) * Time.deltaTime * currentSpeed / 1.7f));
        }

        else if (DistanceFromPlayer() > 1f && DistanceFromPlayer() > 2f)
        {
            movement_vector = LookAtPlayer();
            rb2d.MovePosition(rb2d.position + (Vector2.ClampMagnitude(movement_vector, 1) * Time.deltaTime * currentSpeed));
        }
    }

    private IEnumerator Jump()
    {
        anim.Play("mob_jump_anim");
        currentSpeed = jumpSpeed;
        this.gameObject.layer = 12; //Jumping layer
        yield return new WaitForSeconds(.9f);
        currentSpeed = mySpeed;
        this.gameObject.layer = 8; //Normal player layer
    }

    protected void ShootPlayer()
    {
        if (Time.time > shootTimer) //timer is up
        {
            shootTimer = Time.time + FireRate; //add time to timer
            Transform shootFrom = ChildWithTag("ShootFrom");
            GameObject bullet = (GameObject)Instantiate(Resources.Load("prefabs/ammo/ammo_" + AmmoType));
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
            bullet.transform.position = shootFrom.position;
            bullet.GetComponent<AmmoController>().ShootAmmo(DamageMultiplier, AmmoSpeed, AmmoType, LookAtPlayer(), gameObject);
        }
    }
    protected virtual void MoveRandom() //randomly move a direction
    {
        if (Time.time > randomMoveTimer) //timer is up, change directions.
        {
            randomMoveTimer = Time.time + 1f; //add time to timer
            movement_vector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)); //pick random x,y coordinates
            movement_vector.Normalize(); //make 0-1
        }
        rb2d.MovePosition(rb2d.position + (Vector2.ClampMagnitude(movement_vector, 1) * Time.deltaTime * currentSpeed));
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 11 || other.gameObject.layer == 9) //if on jumpable layer
        {
            StartCoroutine(Jump());
        }
        if (other.gameObject.tag == "Player")
        {
            WhenMobHitsPlayer();
        }
        if (other.gameObject.tag == "Player")
        {
            HealthController healthScript = other.gameObject.GetComponent<HealthController>();
            if (healthScript != null) //Sometimes health script is null because object is in the process of dying. If it isn't dead/null we can access its health script.
            {
                healthScript.AddHealth(collideDamage);
            }
            else
            {
                Debug.Log("health script null");
            }
        }
        else //if mob hits a wall, pick a random direction to move in.
        {
            WhenMobHitsWall();
        }
    }

    protected virtual void WhenMobHitsWall()
    {
        state = "moveRandom";
        randomMoveTimer = 0; //reset timer
    }

    protected virtual void WhenMobHitsPlayer()
    {
        state = "runAway";
    }

    protected void RunAway()
    {
        Vector2 movement_vector = LookAtPlayer();
        rb2d.MovePosition(rb2d.position - (Vector2.ClampMagnitude(movement_vector, 1) * Time.deltaTime * currentSpeed));
    }

    protected void ActionPattern() //randomize state the mob is in
    {
        if (Time.time > stateTimer) //timer is up
        {
            stateTimer = Time.time + TimeBetweenRandomStates; //add time to timer
            state = randomActions[Random.Range(0, randomActions.Count)];
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player1")[0];
        rb2d = transform.parent.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentSpeed = mySpeed;
        MobSpecificStart();
    }

    protected virtual void MobSpecificStart()
    {
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case "chasePlayer":
                ChasePlayer();
                break;
            case "runAway":
                RunAway();
                break;
            case "moveRandom":
                MoveRandom();
                break;
            default:
                ChasePlayer();
                break;
        }
    }

    protected virtual void Update()
    {
    }
}
