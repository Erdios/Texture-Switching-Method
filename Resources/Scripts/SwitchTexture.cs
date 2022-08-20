using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTexture : MonoBehaviour
{
    // public
    public GameObject[] targets;
    public MaterialsManager materialsManager;
    
    // private
    private DistanceDetector[] distanceDetectors;

    bool CheckFoveated(GameObject target)
    {
        foreach (DistanceDetector d in distanceDetectors)
        {
            if (d.DetectFovea(target) == false)
                return false;
        }

        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        distanceDetectors = GetComponentsInChildren<DistanceDetector>();
        targets = GameObject.FindGameObjectsWithTag("SceneObjects"); 
    }

    // Update is called once per frame
    void Update()
    {
        
        foreach (GameObject target in targets)
        {
            // change the texture color of the obejct
            Renderer renderer = target.GetComponent<Renderer>();

            if (CheckFoveated(target))
                materialsManager.BackToOriginalMat(target);
            
            else
                materialsManager.SwitchToPeripheryMat(target);
                
        }
    }
}
