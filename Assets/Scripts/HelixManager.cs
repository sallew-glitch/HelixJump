using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelixManager : MonoBehaviour
{
    public GameObject[] rings;

    public int noOfRings;
    public float ringDistance = 5f;
    float yPos;

    private void Start()
    {

        noOfRings = GameManager.currentLevelIndex + 5;
        for (int i = 0; i < noOfRings; i++)
        {
            if (i == 0)
            {
                //Spawn 1st Ring
                SpawnRings(0);
            }
            else
            {
                //Spawn the middle Rings except the 0 and the last one
                SpawnRings(Random.Range(1, rings.Length - 1));
            }
        }

        //Spawn the last one
        SpawnRings(rings.Length - 1);
    }

    void SpawnRings(int index)
    {
        GameObject newRing = Instantiate(rings[index], new Vector3(transform.position.x, yPos, transform.position.z), Quaternion.identity);
        yPos -= ringDistance;
        newRing.transform.parent = transform;
    }
}
