using UnityEngine;

public class AddRigidbodyToChildren : MonoBehaviour
{
    [Tooltip("Minimum random velocity")]
    public Vector3 minVelocity = new Vector3(-1f, -1f, -1f);
    [Tooltip("Maximum random velocity")]
    public Vector3 maxVelocity = new Vector3(1f, 1f, 1f);

  


    public void AddForce()
    {
        // Get all child transforms
        foreach (Transform child in transform)
        {
            // Add Rigidbody component if it doesn't already exist
            Rigidbody rb = child.gameObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = child.gameObject.AddComponent<Rigidbody>();
            }

            // Set a random velocity
            Vector3 randomVelocity = new Vector3(
                Random.Range(minVelocity.x, maxVelocity.x),
                Random.Range(minVelocity.y, maxVelocity.y),
                Random.Range(minVelocity.z, maxVelocity.z)
            );

            rb.velocity = randomVelocity;
        }
    }
}
