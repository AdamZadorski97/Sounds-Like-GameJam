using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowedObject : MonoBehaviour
{
    // Layer mask to check for specific layers
    public LayerMask targetLayerMask;
    // Detection range for triggering enemies
    public float detectionRange = 5f;
    // Flag to indicate if the script is active
    private bool isActive = false;
    // List to store all enemies on the scene
    private List<EnemyController> allEnemies = new List<EnemyController>();

    void Awake()
    {
        // Start the coroutine to activate the script after 5 seconds
        StartCoroutine(ActivateAfterDelay(5f));
    }

    IEnumerator ActivateAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);
        // Find all enemies in the scene
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        allEnemies.AddRange(enemies);
        // Activate the script
        isActive = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the script is active
        if (!isActive) return;

        // Check if the collided object's layer is in the target layer mask
        if ((targetLayerMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            // Get the collision point
            Vector3 hitPoint = collision.contacts[0].point;

            // Iterate through each enemy
            foreach (EnemyController enemy in allEnemies)
            {
                // Check if the enemy is within the detection range
                if (Vector3.Distance(hitPoint, enemy.transform.position) <= detectionRange)
                {
                    Debug.Log("Agro enemy");
                    // Invoke the Agro method
                    enemy.Agro(hitPoint);
                }
            }
        }
    }

    // Draw the detection range using Gizmos
    private void OnDrawGizmosSelected()
    {
        // Only draw if the script is attached to a GameObject
        if (gameObject != null)
        {
            // Draw a sphere representing the detection range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }
}
