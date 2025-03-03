using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rb;
    public float bounceForce = 400f;

    GameManager gameManager;

    public GameObject splitPrefab;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnCollisionEnter(Collision other)
    {
        rb.velocity = new Vector3 (rb.velocity.x, bounceForce * Time.deltaTime, rb.velocity.z);
        FindObjectOfType<AudioManager>().Play("Land");

        GameObject newSplit = Instantiate(splitPrefab, new Vector3(transform.position.x, other.transform.position.y + 0.19f, transform.position.z), transform.rotation);
        newSplit.transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);
        newSplit.transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + Random.Range(0, 180)));
        newSplit.transform.parent = other.transform;

        string materialName = other.transform.GetComponent<MeshRenderer>().material.name;

        if(materialName == "Safe (Instance)" && !GameManager.gameOver)
        {
            //Debug.Log("You are safe");
        }
        if(materialName == "Unsafe (Instance)" && !GameManager.gameOver)
        {
            gameManager.Hit();
            
        }
        if (materialName == "LastRing (Instance)" && !GameManager.levelWin && !GameManager.gameOver)
        {
            gameManager.Win();
        }
    }
}
