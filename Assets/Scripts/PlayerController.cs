using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public Rigidbody2D rb2d;
    public float moveSpeed = 5f;
    private Vector2 _moveInput; // Vector2 is an X and Y Value
    private Vector2 _mouseInput;
    public float mouseSensitivity = 1f;
    public Camera viewCam;
    public int currentAmmo;

    public GameObject bulletImpact;

    public Animator gunAnimator;

    public int currentHealth;
    public int maxHealth = 100;
    public GameObject deadScreen;
    public GameObject playerHurtScreen;
    public GameObject playerHealScreen;
    public float screenFadeTime = 0.5f;
    private bool hasDied;
    public Text healthText, ammoText;
    public Animator anim;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthText.text = currentHealth.ToString() + "%";
        ammoText.text = currentAmmo.ToString();
        rb2d = GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasDied)
        {
            Move();
            Shoot();
            if (_moveInput != Vector2.zero)
            {
                anim.SetBool("IsMoving",true);
            }

            else 
            {
                anim.SetBool("IsMoving",false);
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        healthText.text = currentHealth.ToString()+"%";
        
        if (currentHealth <= 0)
        {
            deadScreen.SetActive(true);
            hasDied = true;
            currentHealth = 0;
        }
        else 
        {
            playerHurtScreen.SetActive(true);
            StartCoroutine(FadeHurtScreen(0.5f));
        }
    }

    IEnumerator FadeHurtScreen(float WaitTime)
    {
        yield return new WaitForSeconds(WaitTime);
        playerHurtScreen.SetActive(false);
    }

    IEnumerator FadeHealScreen(float WaitTime)
    {
        yield return new WaitForSeconds(WaitTime);
        playerHealScreen.SetActive(false);
    }

    public void AddHealth(int healAmount)
    {
        //adds health amount, but limits by max health
        currentHealth += healAmount;
        

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        healthText.text = currentHealth.ToString() + "%";
        playerHealScreen.SetActive(true);
        StartCoroutine(FadeHealScreen(screenFadeTime));
    }


    public void UpdateAmmoUI()
    {
        ammoText.text = currentAmmo.ToString();
    }


    void Move()
    {
        _moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // this moves object up in relation to the posiition the attached gameobject is rotated
        Vector3 moveHorizontal = transform.up * -_moveInput.x;    
        Vector3 moveVertical = transform.right * _moveInput.y; //_moveInput.y * -1 for inverted
        
        rb2d.velocity = (moveHorizontal + moveVertical) * moveSpeed;

        //player view control
        _mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - _mouseInput.x);
        viewCam.transform.localRotation = Quaternion.Euler(viewCam.transform.localRotation.eulerAngles + new Vector3(0f,_mouseInput.y, 0f));
    }
    
    void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentAmmo > 0)
            {
                Ray ray = viewCam.ViewportPointToRay(new Vector3(0.5f,0.5f,0f));
                RaycastHit hit;
            
                //if something is hit
                if (Physics.Raycast(ray, out hit))
                {
                    //Debug.Log("I'm looking at " + hit.transform.name);
                    Instantiate(bulletImpact, hit.point, transform.rotation); // instantiate 
                    
                    if (hit.transform.tag == "Enemy")
                    {
                        hit.transform.parent.GetComponent<EnemyController>().TakeDamage();
                    }
                }
                else
                {
                    Debug.Log("I'm looking at nothing");
                }

                currentAmmo --;
                gunAnimator.SetTrigger("Shoot");
                
                UpdateAmmoUI();
            }
        }

    }
}
