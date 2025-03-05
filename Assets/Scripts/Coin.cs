using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    public float fadeDuration = 1.5f;
    private Material coinMaterial;
    private Color startColor;
    private Collider coinCollider;
    private Renderer coinRenderer;

    AudioManager audioManager;

    public float riseHeight = 0.5f;  // Height to rise before disappearing
    public float spinSpeedMultiplier = 3f; // Increases rotation speed while fading

    void Start()
    {
        coinRenderer = GetComponent<Renderer>();

        audioManager = FindObjectOfType<AudioManager>();

        // Clone the material so each coin has a separate instance
        coinMaterial = new Material(coinRenderer.material);
        coinRenderer.material = coinMaterial;

        startColor = coinMaterial.color;
        coinCollider = GetComponent<Collider>();

        // Ensure transparency is enabled
        coinMaterial.SetFloat("_Surface", 1); // Transparent
        coinMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        coinMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        coinMaterial.EnableKeyword("_ALPHABLEND_ON");

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // If ball (player) hits the coin
        {
            CoinsManager.instance.IncreaseScore();
            coinCollider.enabled = false; // Disable further collisions

            audioManager.Play("Coin Pick");
            StartCoroutine(FadeAndDestroy()); // Start fade effect
        }
    }

    IEnumerator FadeAndDestroy()
    {
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(0, riseHeight, 0);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeDuration;

            float alpha = Mathf.Lerp(1, 0, progress);

            Color newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            coinMaterial.color = newColor;
            coinMaterial.SetColor("_BaseColor", newColor); // Apply to URP Simple Lit

            // Rise up
            transform.position = Vector3.Lerp(startPos, targetPos, progress);

            // Spin faster
            transform.Rotate(0, 0, spinSpeedMultiplier * 360 * Time.deltaTime);

            // Manually force Unity to refresh the material
            coinRenderer.material = coinMaterial;

            yield return null;
        }

        Destroy(gameObject);
    }
}
