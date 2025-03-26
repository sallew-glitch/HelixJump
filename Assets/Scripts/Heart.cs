using UnityEngine;
using System.Collections;

public class Heart : MonoBehaviour
{
    public float fadeDuration = 1.5f;
    private Material heartMaterial;
    private Color startColor;
    private Collider heartCollider;
    private Renderer heartRenderer;

    AudioManager audioManager;

    public float riseHeight = 0.5f;  // Height to rise before disappearing
    public float spinSpeedMultiplier = 3f; // Increases rotation speed while fading

    void Start()
    {
        heartRenderer = GetComponent<Renderer>();

        audioManager = FindObjectOfType<AudioManager>();

        if (heartRenderer == null)
        {
            Debug.LogError("Heart Renderer not found!");
            return;
        }

        // Clone the material to avoid modifying the original
        heartMaterial = new Material(heartRenderer.material);
        heartRenderer.material = heartMaterial;

        startColor = heartMaterial.color;
        heartCollider = GetComponent<Collider>();

        // Ensure transparency settings
        SetMaterialTransparent();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // If ball (player) hits the heart
        {
            PowerupManager.instance.AddLife();
            heartCollider.enabled = false; // Disable further collisions

            audioManager.Play("Powerup");
            StartCoroutine(FadeAndDestroy()); // Start fade effect
        }
    }

    IEnumerator FadeAndDestroy()
    {
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(0, riseHeight, 0);

        // Get all materials (not just one)
        Material[] materials = heartRenderer.materials;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeDuration;

            float alpha = Mathf.Lerp(1, 0, progress);

            // Apply fading to ALL materials
            for (int i = 0; i < materials.Length; i++)
            {
                Color newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
                materials[i].SetColor("_BaseColor", newColor);
                materials[i].SetColor("_Color", newColor);
            }

            // Move up
            transform.position = Vector3.Lerp(startPos, targetPos, progress);

            // Spin faster
            transform.Rotate(0, spinSpeedMultiplier * 360 * Time.deltaTime, 0);

            yield return null;
        }

        Destroy(gameObject);
    }



    void SetMaterialTransparent()
    {
        if (heartMaterial == null) return;

        heartMaterial.SetFloat("_Surface", 1); // Set as Transparent
        heartMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        heartMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        heartMaterial.EnableKeyword("_ALPHABLEND_ON");

        // Ensure correct shader mode (works for Standard and URP shaders)
        if (heartMaterial.HasProperty("_Mode")) heartMaterial.SetFloat("_Mode", 2); // 2 = Fade
        if (heartMaterial.HasProperty("_SrcBlend")) heartMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        if (heartMaterial.HasProperty("_DstBlend")) heartMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        if (heartMaterial.HasProperty("_ZWrite")) heartMaterial.SetFloat("_ZWrite", 0);

        heartMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        heartMaterial.SetOverrideTag("RenderType", "Transparent");

        Debug.Log("Material transparent set");
    }
}
