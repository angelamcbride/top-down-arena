using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMobController : MonoBehaviour
{
    [SerializeField] private float mySpeed = 3f;
    [SerializeField] private float jumpSpeed = 3f;
    [SerializeField] protected float collideDamage = -6f;
    [SerializeField] private float DamageMultiplier = 5f;
    [SerializeField] private string AmmoType = "greenMagic";
    [SerializeField] private float AmmoSpeed = 8f;
    [SerializeField] private float FireRate = 0.5f;
    [SerializeField] protected float TimeBetweenRandomStates = 2f;
    [SerializeField] private float heartDropChance = 0.5f;

    [SerializeField] private Transform shootPoint;

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

    public virtual void OnMobSpawned()
    {
        MonsterSpawner.AdjustActiveMobCount(1); // Increment the active mob count
    }

    public virtual void Die()
    {
        MonsterSpawner.AdjustActiveMobCount(-1); // Decrement the active mob count
        if (Random.value < heartDropChance)
        {
            GameObject heart = Resources.Load<GameObject>("Prefabs/Items/Heart");
            Instantiate(heart, transform.position, Quaternion.identity);
        }
        Destroy(transform.parent.gameObject); // Destroy the parent object which contains the mob and its shadow.
    }

    protected Vector2 LookAtPlayer(Vector3 originPos)
    {
        Vector2 difference = player.transform.position - originPos - new Vector3(0, .5f, 0); // difference between mouse pos and character.
        difference.Normalize(); //make 0-1
        return new Vector2(difference.x, difference.y);
    }

    protected float DistanceFromPlayer()
    {
        return Vector2.Distance(player.transform.position, transform.root.position);
    }

    protected virtual void ChasePlayer()
    {
        movement_vector = LookAtPlayer(transform.root.position);
        float speedModifier = DistanceFromPlayer() < 2f ? 1.7f : 1f;
        rb2d.AddForce(Vector2.ClampMagnitude(movement_vector, 1) * currentSpeed / speedModifier, ForceMode2D.Force);
    }

    protected IEnumerator Jump()
    {
        anim.Play("mob_jump_anim");
        currentSpeed = jumpSpeed;
        this.gameObject.layer = 12; //Jumping layer
        rb2d.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(.9f);
        currentSpeed = mySpeed;
        this.gameObject.layer = 8; //Normal player layer
    }

    protected void ShootPlayer()
    {
        if (Time.time > shootTimer) //timer is up
        {
            shootTimer = Time.time + FireRate; //add time to timer
            GameObject bullet = (GameObject)Instantiate(Resources.Load("prefabs/ammo/ammo_" + AmmoType));
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
            bullet.transform.position = shootPoint.position;
            bullet.GetComponent<AmmoController>().ShootAmmo(DamageMultiplier, AmmoSpeed, AmmoType, LookAtPlayer(shootPoint.position), gameObject, 5f);
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
        rb2d.AddForce(Vector2.ClampMagnitude(movement_vector, 1) * currentSpeed, ForceMode2D.Force);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "PlayerParent")
        {
            WhenMobHitsPlayer(other.gameObject);
        }
        else if (other.gameObject.layer == 11 || other.gameObject.layer == 9) //if on jumpable layer
        {
            StartCoroutine(Jump());
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

    protected virtual void WhenMobHitsPlayer(GameObject player)
    {
        StartCoroutine(Jump());
        state = "runAway";
    }

    protected virtual void RunAway()
    {
        if (DistanceFromPlayer() < 0.25f && DistanceFromPlayer() > 0f) // Jump if player is on top of mob
        {
            StartCoroutine(Jump());
        }
        movement_vector = LookAtPlayer(transform.root.position);
        rb2d.AddForce(-Vector2.ClampMagnitude(movement_vector, 1) * currentSpeed * Time.fixedDeltaTime, ForceMode2D.Force);
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
        player = GameObject.FindGameObjectWithTag("PlayerTarget");
        rb2d = transform.parent.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentSpeed = mySpeed;
        MobSpecificStart();
    }

    protected virtual void MobSpecificStart()
    {
        state = "chasePlayer";
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
