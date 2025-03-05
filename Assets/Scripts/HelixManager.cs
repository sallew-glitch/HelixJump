using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

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
                SpawnRings(0, true);
            }
            else
            {
                //Spawn the middle Rings except the first and the last one
                SpawnRings(Random.Range(1, rings.Length - 1), Random.Range(0, 2) == 1 ? true : false);
            }
        }

        //Spawn the last one
        SpawnRings(rings.Length - 1, false);


    }

    void SpawnRings(int index, bool choice)
    {
        GameObject newRing = Instantiate(rings[index], new Vector3(transform.position.x, yPos, transform.position.z), Quaternion.identity);

        if (choice)
            CoinsManager.instance.SpawnCoins(newRing, yPos);

        yPos -= ringDistance;
        newRing.transform.parent = transform;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
