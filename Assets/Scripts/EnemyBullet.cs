using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int damageAmount;
    public float bulletSpeed = 3f; // faster than the player?
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        direction = PlayerController.instance.transform.position - transform.position;
        direction.Normalize();
        direction = direction * bulletSpeed; // * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.velocity = direction * bulletSpeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Wall")
        {
            Destroy(gameObject);
        }
        if (other.tag == "Player")
        {
            PlayerController.instance.TakeDamage(damageAmount);//deals damage to player
            Destroy(gameObject);

        }
    }
}
