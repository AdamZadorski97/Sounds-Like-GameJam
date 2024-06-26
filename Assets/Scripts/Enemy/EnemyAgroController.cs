using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAgroController : MonoBehaviour
{
    public float detectionRange = 5f;
    private List<EnemyController> allEnemies = new List<EnemyController>();


    private void Start()
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        allEnemies.AddRange(enemies);
    }
    public void AgroEnemies()
    {
        // Check if the collided object's layer is in the target layer mask

        // Get the collision point
        Vector3 point = transform.position;

        // Iterate through each enemy
        foreach (EnemyController enemy in allEnemies)
        {
            // Check if the enemy is within the detection range
            if (Vector3.Distance(point, enemy.transform.position) <= detectionRange)
            {
                Debug.Log("Agro enemy");
                // Invoke the Agro method
                enemy.Agro(point);
            }
        }

    }


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
