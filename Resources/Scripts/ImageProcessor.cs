using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageProcessor : MonoBehaviour
{

    public List<Texture2D> textures;
    public string textureName;
    public int textureCount;

    public List<RenderTexture> outputTexs;

    private Shader processorShader;
    private Material processorMaterial;


    // Start is called before the first frame update
    void Start()
    {
        // Load shader
        processorShader = Shader.Find("Hidden/ImageProcessor");

        // Create material for the shader
        processorMaterial = new Material(processorShader);

        // Load textures
        for (int i = 0; i < textureCount; ++i)
        {
            textures.Add((Texture2D)Resources.Load(textureName + i));
        }


        // Pass textures to shader and process them all
        foreach (Texture2D texture in textures)
        {
            // Set texture in the material
            processorMaterial.SetTexture("_MainTex", texture);

            // Blit the data into a new texture (should we save it as images?)
            RenderTexture temp = new RenderTexture(texture.width, texture.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            Graphics.Blit(texture, temp, processorMaterial);
            outputTexs.Add(temp);

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
