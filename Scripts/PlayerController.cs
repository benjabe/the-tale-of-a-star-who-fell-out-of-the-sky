using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float jumpHeight = 10.0f;
    [SerializeField] private float bulletSpeed = 15.0f;
    [SerializeField] private int   burstMaxDamage = 7;
    [SerializeField] private float burstChargeTime = 2.0f;
    [SerializeField] private float burstChargeRadius = 4.5f;

    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask interactable;
    [SerializeField] private GameObject bulletPrefab = null;

    BoxCollider2D col;
    Rigidbody2D rb;

    SoundManager soundManager;

    float horizontal;

    // Powerups
    bool burst = false;
    float burstCharge = 0.0f;

    bool hover = false;

    int maxJumps = 1;
    int jumps = 0;

    float jumpDelay = 0.2f;
    float timeSinceLastJump = 0.2f;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        soundManager = GameObject.Find("GameManager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        timeSinceLastJump += Time.deltaTime;
        if (Input.GetAxisRaw("Horizontal") != 0.0f)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            transform.position += new Vector3(
                horizontal,
                0.0f,
                0.0f
            ) * speed * Time.deltaTime;
        }

        rb.gravityScale = 1.0f;
        if (IsGrounded())
        {
            if (timeSinceLastJump > jumpDelay)
            {
                jumps = 0;
                if (Input.GetButtonDown("Jump"))
                {
                    rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
                    jumps = 1;
                    timeSinceLastJump = 0.0f;
                }
            }
        }
        else
        {
            jumps = Mathf.Clamp(jumps, 1, maxJumps);
            if (jumps < maxJumps && Input.GetButtonDown("Jump") && timeSinceLastJump > jumpDelay)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
                jumps++;
                timeSinceLastJump = 0.0f;
            }
            if (rb.velocity.y < 0.0f)
            {
                if (hover && Input.GetButton("Jump"))
                {
                    rb.gravityScale = 0.5f;
                    rb.velocity = new Vector2(
                        rb.velocity.x,
                        Mathf.Clamp(rb.velocity.y, 0.0f, -1.0f)
                    );
                }
                else
                {
                    rb.gravityScale = 1.7f;
                }
            }
            else if (!Input.GetButton("Jump"))
            {
                rb.gravityScale = 1.4f;
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            // Shoot
            GameObject bullet = Instantiate(bulletPrefab);
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            direction.Normalize();
            bullet.transform.position = transform.position +
                new Vector3(direction.x, direction.y, 0.0f) * 0.88f +
                new Vector3(0.5f, -0.5f, 0.0f);
            bullet.GetComponent<Bullet>().velocity = direction * bulletSpeed;
        }

        if (Input.GetButtonDown("Interact"))
        {
            RaycastHit2D hit = Physics2D.BoxCast(
                new Vector2(col.bounds.min.x, col.bounds.min.y),
                new Vector2(1.0f, 1.0f),
                0.0f,
                new Vector2(Mathf.Sign(horizontal), 0.0f),
                1.0f,
                interactable
            );
            if (hit)
            {
                hit.transform.GetComponent<DialogueHandler>().StartStory();
                if (hit.transform.gameObject.CompareTag("Powerup"))
                {
                    switch (hit.transform.name.ToLower())
                    {
                        case "burst":
                            burst = true;
                            break;
                        case "hover":
                            hover = true;
                            break;
                        case "double jump":
                            maxJumps = 2;
                            break;
                        default:
                            break;
                    }
                    Destroy(hit.transform.gameObject);
                }
            }
        }

        if (burst)
        {
            if (Input.GetMouseButton(1))
            {
                burstCharge += Time.deltaTime;
                if (burstCharge > burstChargeTime)
                {
                    burstCharge = burstChargeTime;
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                Debug.Log("Burst");
                // Perform burst attack
                GameObject[] objects = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject go in objects)
                {
                    int damage = Mathf.CeilToInt(burstMaxDamage * burstCharge / burstChargeTime);
                    Debug.Log(damage);
                    go.GetComponent<Enemy>().TakeDamage(damage);
                }
            }
        }
    }

    private bool IsGrounded() {
        return Physics2D.BoxCast(
            new Vector2(col.bounds.center.x, col.bounds.min.y),
            new Vector2(0.80f, 0.05f), 
            0.0f, 
            Vector2.down, 
            0.0f,
            ground
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Area"))
        {
            Debug.Log("Entered area: " + collision.name);
            soundManager.PlayMusic(collision.gameObject);
            if (collision.gameObject.name == "End")
            {
                GameObject.Find("Canvas").GetComponent<CutsceneManager>().End();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Hazard"))
        {
            GameObject.Find("Canvas").GetComponent<CutsceneManager>().Death();
        }
    }
}
