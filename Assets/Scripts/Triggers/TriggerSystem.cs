using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class TriggerSystem : MonoBehaviour
{
    private BoxCollider boxCollider;

    [SerializeField] private UnityEvent onPlayerTriggerEnter;
    [SerializeField] private UnityEvent onPlayerTriggerExit;

    [SerializeField] private UnityEvent onEnemyTriggerEnter;
    [SerializeField] private UnityEvent onEnemyTriggerExit;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true; // Ensure the collider is set as a trigger
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
            onPlayerTriggerEnter.Invoke();
        if (other.GetComponent<EnemyController>())
        {
            Debug.Log("Enemy Trigger");
            onEnemyTriggerEnter.Invoke();
        }
        
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
            onPlayerTriggerExit.Invoke();
        if (other.GetComponent<EnemyController>())
            onEnemyTriggerExit.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; // Set the Gizmo color

        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider>();

        if (boxCollider != null)
        {
            // Use the collider's transform to get the correct position and size
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
    }
}
