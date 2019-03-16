using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int health = 5;
    [SerializeField] protected float speed = 1.0f;
    [SerializeField] protected float maxAggressionDistance = 10.0f;

    protected PlayerController player;

    float distanceToMovePreviousFrame = 0.0f;
    float positionPreviousFrame = 0.0f;

    protected float timeBetweenJumps = 2.0f;
    protected float timeSinceLastJump = 2.0f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < maxAggressionDistance)
        {
            timeSinceLastJump += Time.deltaTime;
            float distanceMovedPreviousFrame = transform.position.x - positionPreviousFrame;
            float xBefore = transform.position.x;
            float distanceToMove = (player.transform.position - transform.position).normalized.x *
                speed * Time.deltaTime;

            transform.position += new Vector3(
                distanceToMove,
                0.0f,
                0.0f
            );

            float distanceActuallyMoved = Mathf.Abs(transform.position.x - xBefore);

            if ((distanceMovedPreviousFrame) < distanceToMovePreviousFrame / 2.0f &&
                timeSinceLastJump > timeBetweenJumps)
            {
                GetComponent<Rigidbody2D>().AddForce(Vector2.up * 4.0f, ForceMode2D.Impulse);
                timeSinceLastJump = 0.0f;
            }

            distanceToMovePreviousFrame = distanceToMove;
            positionPreviousFrame = xBefore;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
