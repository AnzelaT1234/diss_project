using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDirt : MonoBehaviour
{
    [SerializeField]
    private GameObject dirt;
    public Vector3 center;

    [SerializeField]
    private GameObject canvas;

    void Start()
    {
        for (int i=0; i<5; i++)
        {
            spawn();
        }
    }

    public void spawn()
    {
        Vector3 spawnPos = GetRandomPositionInCircle(center, 10.0F);

        GameObject newDirt;
        newDirt = Instantiate(dirt, spawnPos, Quaternion.identity);
        newDirt.transform.SetParent(canvas.transform, false);
    }

    private Vector3 GetRandomPositionInCircle(Vector3 center, float radius)
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);  // Random angle
        float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;  // Random distance (sqrt for uniform distribution)

        float x = center.x + distance * Mathf.Cos(angle);
        float y = center.y + distance * Mathf.Sin(angle);
        float z = center.z; 

        return new Vector3(x, y, z);
    }
}
