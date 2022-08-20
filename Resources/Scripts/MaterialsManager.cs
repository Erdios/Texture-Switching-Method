using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MaterialsManager : MonoBehaviour
{
    // public
    [HideInInspector] public Dictionary<Material, int> materials = new Dictionary<Material, int>();
    [HideInInspector] public int peripheryCount = 0;
    public Material peripheryMat;
    public bool willBlur;

    // private
    private Renderer[] renderers;
    private Texture2D peripheryTex, peripheryNormTex;



    
    void Start()
    { 
        renderers = GetComponentsInChildren<Renderer>();
        MaterialManager[] materialManager = GetComponentsInChildren<MaterialManager>();

        int count = 0;


        peripheryTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Resources/Textures/PeripheryTexture.png", typeof(Texture2D));
        peripheryNormTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Resources/Textures/PeripheryNormal.png", typeof(Texture2D));

        peripheryMat.mainTexture = peripheryTex;
        peripheryMat.SetTexture("_BumpMap", peripheryNormTex);

        foreach (Transform child in transform)
        {
            // load propreate material for the object
            materialManager[count].InitiateMaterial();

            if (willBlur)
            {
                if (!materials.ContainsKey(child.GetComponent<Renderer>().material))
                    materials.Add(child.GetComponent<Renderer>().material, 1);
                else
                    materials[child.GetComponent<Renderer>().material]++;
            }

            // if doing the blur and texture switching, the following code run, if not, skip
            if (materialManager[count].willGenerateMipmap)
            {
                //child is your child transform
                SwitchToPeripheryMat(child.gameObject);
            }
            count++;
        }
    }

    public void SwitchToPeripheryMat(GameObject gameobject)
    {
        Renderer renderer = gameobject.GetComponent<Renderer>();

        if (renderer.sharedMaterial != peripheryMat)
        {
            materials[renderer.material]--;
            peripheryCount++;
            renderer.sharedMaterial = peripheryMat;
            
        }
    }

    public void BackToOriginalMat(GameObject gameobject)
    {

        Renderer renderer = gameobject.GetComponent<Renderer>();

        if (renderer.sharedMaterial == peripheryMat)
        {
            gameobject.GetComponent<MaterialManager>().BackToOriginalMaterial();
            materials[renderer.material]++;
            peripheryCount--;
        }
    }
}