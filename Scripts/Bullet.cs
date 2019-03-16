using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] AudioClip audioClip;
    public Vector3 velocity;

    private float time = 0.0f;

    private void Start()
    {
        GameObject.Find("GameManager").GetComponent<SoundManager>().PlaySound(audioClip);
    }

    private void Update()
    {
        transform.position += velocity * Time.deltaTime;
        time += Time.deltaTime;
        if (time > 2.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }
            Destroy(gameObject);
        }
    }
}
