using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f; // Amount of damage to deal to the player

    private Transform target;
    private float speed;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Ensure Rigidbody is kinematic to move it manually
        }
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    public void SetTarget(Transform target, float speed)
    {
        this.target = target;
        this.speed = speed;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        // Check if the object the projectile collided with has the PlayerController component
        //  PlayerController player = other.GetComponent<PlayerController>();

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController.Instance.TakeDamage(1);
            Destroy(gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // Destroy the projectile if it collides with the ground layer
            Destroy(gameObject);
        }
        else
        {
            // Handle other trigger cases
            HandleOtherTrigger(other);
        }
    }

    void HandleOtherTrigger(Collider other)
    {
        // Implement behavior for other types of triggers
        // Example: Print the name of the object the projectile collided with
        Debug.Log("Projectile triggered with: " + other.gameObject.name);

        // You can add additional behavior here based on the type of object triggered with
    }

    public void Bounce(Vector3 bounceDirection)
    {
        // Change the projectile's direction to the bounce direction
        rb.isKinematic = false;
        rb.velocity = Vector3.zero; // Reset the velocity
        rb.AddForce(bounceDirection * speed, ForceMode.Impulse);
    }
}
