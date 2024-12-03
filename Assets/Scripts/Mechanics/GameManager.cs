using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private Vector3 checkpointPosition;
    private float gaugeLevel;
    public DrawShapes drawShapes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (drawShapes == null)
            {
                drawShapes = GetComponent<DrawShapes>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetGaugeLevel(float level)
    {
        gaugeLevel = level;
    }

    public float GetGaugeLevel()
    {
        return gaugeLevel;
    }

    public void SetCheckpoint(Vector3 position)
    {
        checkpointPosition = position;
    }

    public Vector3 GetCheckpointPosition()
    {
        return checkpointPosition;
    }
}
