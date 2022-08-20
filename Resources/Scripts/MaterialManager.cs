using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MaterialManager : MonoBehaviour
{
    public Material material;
    public Texture2D originalTex;
    public Texture2D normalTex;
    public string objectName;

    public bool willGenerateMipmap;

    public void InitiateMaterial()
    {

        // get the original texture and normal texture for the object
        objectName = gameObject.name.Substring(0, 5);
        originalTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Rocks and Boulders 2/Rocks/Source/Textures/" + objectName + ".tif", typeof(Texture2D));
        normalTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Rocks and Boulders 2/Rocks/Source/Textures/" + objectName + "_nmp.tif", typeof(Texture2D));

        // Load peripheryTexture
        material = GetComponent<Renderer>().material;
        material.mainTexture = originalTex;
        material.SetTexture("_BumpMap", normalTex);


        // choose the mipmap of the texture by mode
        if (material != null && willGenerateMipmap)
        {
            Texture2D peripheryTex, peripheryNormTex;
            RenderTexture finalTex, finalNormTex;

            // generate mipmap with "super rock tex"
            peripheryTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Resources/Textures/PeripheryTexture.png", typeof(Texture2D));
            peripheryNormTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Resources/Textures/PeripheryNormal.png", typeof(Texture2D));


            if (peripheryTex != null)
            {
                finalTex = new RenderTexture(material.mainTexture.width, material.mainTexture.height, 0);
                finalTex.useMipMap = true;
                finalTex.autoGenerateMips = false;
                finalTex.filterMode = FilterMode.Trilinear;
                finalTex.Create();

                finalNormTex = new RenderTexture(peripheryNormTex.width, peripheryNormTex.width, 0);
                finalNormTex.useMipMap = true;
                finalNormTex.autoGenerateMips = false;
                finalNormTex.filterMode = FilterMode.Trilinear;
                finalNormTex.Create();

                Shader copyShader = Shader.Find("Unlit/CopyShader");
                Material copyMaterial = new Material(copyShader);

                Graphics.SetRenderTarget(finalTex, 0);
                Graphics.Blit(material.mainTexture, copyMaterial);
                Graphics.SetRenderTarget(finalTex, 1);
                Graphics.Blit(peripheryTex, copyMaterial);


                Graphics.SetRenderTarget(finalNormTex, 0);
                Graphics.Blit(normalTex, copyMaterial);
                Graphics.SetRenderTarget(finalNormTex, 1);
                Graphics.Blit(peripheryNormTex, copyMaterial);

                material.mainTexture = finalTex;
                material.SetTexture("_BumpMap", finalNormTex);

            }
        }

    }

    void OnApplicationQuit()
    {
        if (originalTex != null && willGenerateMipmap)
        {
            material.mainTexture = originalTex;
            material.SetTexture("_BumpMap", normalTex);
        }
    }

  

    // this function should only be called in MaterialsManager class
    public void BackToOriginalMaterial()
    {
        GetComponent<Renderer>().material = material;
    }

}
