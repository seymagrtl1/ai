using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpOnTrigger : MonoBehaviour
{
    public float moveUpDistance = 2f; // Distance to move up
    public float moveUpSpeed = 2f; // Speed of moving up
    public float delayBeforeMove = 5f; // Delay before moving up

    private bool shouldMoveUp = false;
    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        if (shouldMoveUp)
        {
            // Move towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveUpSpeed * Time.deltaTime);

            // Stop moving if we have reached the target position
            if (transform.position == targetPosition)
            {
                shouldMoveUp = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Start the coroutine to move up after a delay
            StartCoroutine(MoveUpAfterDelay());
        }
    }

    private IEnumerator MoveUpAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delayBeforeMove);

        // Set the target position and flag to start moving
        targetPosition = new Vector3(transform.position.x, transform.position.y + moveUpDistance, transform.position.z);
        shouldMoveUp = true;
    }
}
