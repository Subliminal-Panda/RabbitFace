using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class AlphaController : MonoBehaviour
{
    public Material targetMaterial; // Reference to the material you want to modify
    [Range(0f, 1f)]
    public float alphaValue = 0f; // Serialized variable for alpha value

    void Start()
    {
        if (targetMaterial == null)
        {
            Debug.LogError("Target material is not assigned!");
            return;
        }

        // // Ensure the alpha value is within the valid range
        // alphaValue = Mathf.Clamp01(alphaValue);

        // // Set the initial alpha value in the material's color property
        // Color materialColor = targetMaterial.color;
        // materialColor.a = alphaValue;
        // targetMaterial.color = materialColor;
    }

    void Update() 
    {
        // Ensure the alpha value is within the valid range
        alphaValue = Mathf.Clamp01(alphaValue);

        // Set the initial alpha value in the material's color property
        Color materialColor = targetMaterial.color;
        materialColor.a = alphaValue;
        targetMaterial.color = materialColor;
    }

    void UpdateAlphaValue()
    {
        if (targetMaterial == null)
        {
            return;
        }

        // Ensure the alpha value is within the valid range
        alphaValue = Mathf.Clamp01(alphaValue);

        // Set the alpha value in the material's color property
        Color materialColor = targetMaterial.color;
        materialColor.a = alphaValue;
        targetMaterial.color = materialColor;
    }

    public void SetAlphaValue(float newValue) 
    {
        Debug.Log("setting alpha value!");
        Debug.Log(newValue);
        alphaValue = newValue;
    }
}
