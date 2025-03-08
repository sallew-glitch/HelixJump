using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoinsManager : MonoBehaviour
{

    public static CoinsManager instance;
    public float rotationSpeed = 45f;

    private int score = 0;
    
    public GameObject coin;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        SpinCoins();
    }

    public void SpawnCoins(GameObject newRing, float yPos)
    {
        Transform[] allChildren = newRing.GetComponentsInChildren<Transform>(false);

        Transform[] newChildrenArray = allChildren.Where(child => (child != newRing.transform && child.GetComponent<MeshRenderer>().material.name == "Safe (Instance)")).ToArray();

        int index = Random.Range(0, newChildrenArray.Length + 1);

        for (int i = 0; i < newChildrenArray.Length; i++)
        {
            if (i == index)
            {
                Quaternion rotation = newChildrenArray[i].rotation; // Use world rotation
                Vector3 eulerRotation = rotation.eulerAngles;

                float trueZRotation = (eulerRotation.y <= 180) ? eulerRotation.y : eulerRotation.y - 360;
                trueZRotation += 67.5f;
                Debug.Log("trueZ " + trueZRotation);

                float distance = 2.2f; // Get distance from center
                Quaternion finalRotation = Quaternion.Euler(-90, 0, trueZRotation); // Reconstruct rotation

                // Offset starts along Y before rotation
                Vector3 offset = new Vector3(0, distance, 0);
                Vector3 worldPosition = finalRotation * offset;

                GameObject newCoin = Instantiate(coin, new Vector3(worldPosition.x, yPos + 1, worldPosition.z), finalRotation);
                newCoin.transform.parent = newRing.transform;
                newCoin.tag = "Coin";

                // Add a Collider to detect collection
                if (!newCoin.GetComponent<SphereCollider>())
                {
                    SphereCollider collider = newCoin.AddComponent<SphereCollider>();
                    collider.isTrigger = true; // Must be trigger
                    collider.radius = 0.3f; // Adjust radius as needed
                }

                // Add a Rigidbody (set to kinematic so it doesn’t fall)
                if (!newCoin.GetComponent<Rigidbody>())
                {
                    Rigidbody rb = newCoin.AddComponent<Rigidbody>();
                    rb.isKinematic = true;
                }

                // Add Coin component to handle fading
                if (!newCoin.GetComponent<Coin>())
                {
                    newCoin.AddComponent<Coin>();
                }
            }
        }
    }

    public void SpinCoins()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");

        foreach (GameObject coin in coins)
        {
            coin.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }

    public void IncreaseScore()
    {
        score++; // Increase counter
        Debug.Log("Coins Collected: " + score);
    }
}
