using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.SetCheckpoint(transform.position);
        }
    }
}
