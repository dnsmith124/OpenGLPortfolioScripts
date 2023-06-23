using System.Collections;
using UnityEngine;

public class SpellBehavior : MonoBehaviour
{
    public float maxTravelDistance = 50f; // Maximum travel distance
    public float disappearDelay = 2f; // Delay before disappearing after collision

    private Vector3 startingPosition;

    private void Start()
    {
        // Save the starting position of the spell
        startingPosition = transform.position;
    }

    private void Update()
    {
        // Check if the spell has traveled its maximum distance
        if (Vector3.Distance(startingPosition, transform.position) >= maxTravelDistance)
        {
            Destroy(gameObject); // Destroy the spell
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Start the Disappear coroutine when the spell collides with something
        StartCoroutine(DisappearAfterDelay());
    }

    private IEnumerator DisappearAfterDelay()
    {
        yield return new WaitForSeconds(disappearDelay); // Wait for the specified delay

        Destroy(gameObject); // Destroy the spell
    }
}
