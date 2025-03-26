using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PowerupManager : MonoBehaviour
{

    public static PowerupManager instance;
    public float rotationSpeed = 45f;

    public GameObject heart;

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
        SpinHearts();
    }

    public void SpawnHearts(GameObject newRing, float yPos)
    {
        Transform[] allChildren = newRing.GetComponentsInChildren<Transform>(false);

        Transform[] newChildrenArray = allChildren.Where(child => (child != newRing.transform && child.GetComponent<MeshRenderer>().material.name == "Safe (Instance)")).ToArray();

        int index = Random.Range(0, newChildrenArray.Length + 1);

        for (int i = 0; i < newChildrenArray.Length; i++)
        {
            if (i == index)
            {
                /*Quaternion rotation = newChildrenArray[i].rotation; // Use world rotation
                Vector3 eulerRotation = rotation.eulerAngles;

                //float trueZRotation = (eulerRotation.y <= 180) ? eulerRotation.y : eulerRotation.y - 360;
                //trueZRotation += 67.5f;

                float distance = 2.2f; // Get distance from center
                Quaternion finalRotation = Quaternion.Euler(-90, 0, 0); // Reconstruct rotation

                // Offset starts along Y before rotation
                Vector3 offset = new Vector3(0, distance, 0);
                Vector3 worldPosition = finalRotation * offset;
                */

                float angle = Random.Range(0f, Mathf.PI * 2f);

                // Fixed radius from the center
                float radius = 2.2f;

                // Calculate random position on the circle
                Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

                // Get world position relative to the ring
                Vector3 worldPosition = newRing.transform.position + offset;

                GameObject newHeart = Instantiate(heart, new Vector3(worldPosition.x, yPos + 1, worldPosition.z), Quaternion.Euler(0, 0, 0));
                newHeart.transform.parent = newRing.transform;
                newHeart.tag = "Heart";

                // Add a Collider to detect collection
                if (!newHeart.GetComponent<SphereCollider>())
                {
                    SphereCollider collider = newHeart.AddComponent<SphereCollider>();
                    collider.isTrigger = true; // Must be trigger
                    collider.radius = 0.3f; // Adjust radius as needed
                }

                // Add a Rigidbody (set to kinematic so it doesn’t fall)
                if (!newHeart.GetComponent<Rigidbody>())
                {
                    Rigidbody rb = newHeart.AddComponent<Rigidbody>();
                    rb.isKinematic = true;
                }

                // Add Coin component to handle fading
                if (!newHeart.GetComponent<Heart>())
                {
                    newHeart.AddComponent<Heart>();
                }
            }
        }
    }

    public void SpinHearts()
    {
        GameObject[] hearts = GameObject.FindGameObjectsWithTag("Heart");

        foreach (GameObject heart in hearts)
        {
            heart.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }

    public void AddLife()
    {
        Debug.Log("Add Life");
        FindObjectOfType<GameManager>().addOrReduceLives(1);
        Debug.Log("Lives : " + GameManager.currentLives);
    }
}
