using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedTrapActivator : MonoBehaviour
{
    public GameObject[] traps; // Array of traps to activate
    public float activationDelay = 5f; // Delay in seconds

    private bool trapsActivated = false;

    private void Start() {
        foreach (GameObject trap in traps)
        {
            trap.SetActive(false); // Activate each trap
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !trapsActivated)
        {
            StartCoroutine(ActivateTraps());
        }
    }

    private IEnumerator ActivateTraps()
    {
        yield return new WaitForSeconds(activationDelay);
        foreach (GameObject trap in traps)
        {
            trap.SetActive(true); // Activate each trap
        }
        trapsActivated = true;
    }
}
