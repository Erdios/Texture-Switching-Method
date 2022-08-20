using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectObjects : MonoBehaviour
{

    // public
    public MaterialsManager materialsManager;

    
    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        
        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        if (collision.gameObject.tag == "SceneObjects")
        {
            materialsManager.BackToOriginalMat(collision.gameObject);
        }
    }



    void OnCollisionExit(Collision collision)
    {

        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        if (collision.gameObject.tag == "SceneObjects")
        {
            materialsManager.SwitchToPeripheryMat(collision.gameObject);
            
        }

    }


}
