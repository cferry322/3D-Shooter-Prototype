using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float _lagTime;
    public float _attackTime;// lag time until attack
    public bool _isMeleeEnemy;
    public int health = 2;
    public GameObject explosion;
    public float playerRange = 10f;
    public float meleeRange = 1f;
    public bool isAttacking = false;

    public Rigidbody2D rb;
    public float moveSpeed;

    public bool shouldShoot;
    public float fireRate = 0.5f; // every half second
    public float shotCounter; // how long waiting between bullets
    public GameObject bullet;
    public GameObject meleeAttack;
    public Transform firePoint;

    public Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator is null on the Enemy Controller Gameobject");
        }
    }

    bool InMeleeRange()
    {
        //if in range attack
        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) <= meleeRange)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }
    void Move()
    {

        if (isAttacking = false)
        {
            float playerDistance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
            
            if ( (playerDistance < playerRange) && (InMeleeRange() == false))
            {
                //Get Player Direction
                Vector3 playerDirection = PlayerController.instance.transform.position - transform.position; // move towards player
            
                //Move Toward Player
                rb.velocity = playerDirection.normalized * moveSpeed;
                _animator.SetTrigger("Walk");
            }

            else if (playerDistance > playerRange)
            {

                rb.velocity = Vector2.zero;
                _animator.SetTrigger("Idle");
            }
        }

        else if (isAttacking) 
        {
            rb.velocity = Vector2.zero;
        }

    }
    
    // Update is called once per frame
    void Update()
    {
        Move();
        Shoot();
    }

    void Shoot()
    {
        if (shouldShoot)
        {
            shotCounter -= Time.deltaTime;

            if (shotCounter <= 0)
            {
                Instantiate(bullet, firePoint.position, firePoint.rotation);
                shotCounter = fireRate; // reset counter
            }
        }
    }

    public void TakeDamage()
    {
        health --;
        if (health <=0)
        {
            Destroy(gameObject);
            Instantiate(explosion,transform.position,transform.rotation);
        }

    }

    void ZeroMoveSpeed()
    {
        Vector3 playerDirection = PlayerController.instance.transform.position - transform.position; // move towards player

        rb.velocity = playerDirection.normalized * 0f; // just zeroes out movespeed
    }

    void OnCollisionEnter2D (Collision2D other)
    {
        if (other.transform.tag == "Player")
        {   
            if (_isMeleeEnemy == true);
            {
                if (InMeleeRange())
                {
                    ZeroMoveSpeed();
                    _animator.SetTrigger("Attack");
                    isAttacking = true;
                    StartCoroutine(MeleeAttack(_attackTime));

                }
            }
        }
    }
    

    private IEnumerator MeleeAttack(float WaitTime)
    {
        yield return new WaitForSeconds(WaitTime);
        Instantiate(meleeAttack,transform.position,transform.rotation);
        
        StartCoroutine(EndLag(_lagTime));    
    }

    IEnumerator EndLag(float lagTime)
    {
        yield return new WaitForSeconds(lagTime);
        _animator.SetTrigger("Idle");
        isAttacking = false;
    }
}
