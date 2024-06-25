using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowedObject : MonoBehaviour
{
    // Layer mask to check for specific layers
    public LayerMask targetLayerMask;

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object's layer is in the target layer mask
        if ((targetLayerMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            // Get the collision point
            Vector3 hitPoint = collision.contacts[0].point;

            // Find all objects with EnemyController component
            EnemyController[] enemyControllers = FindObjectsOfType<EnemyController>();

            // Invoke Agro method on each found EnemyController
            foreach (EnemyController enemy in enemyControllers)
            {
                enemy.Agro(hitPoint);
            }
        }
    }
}
