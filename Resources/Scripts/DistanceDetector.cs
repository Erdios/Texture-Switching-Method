using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class DistanceDetector: MonoBehaviour
{
    //private Transform target;
    private Camera cam;
    private Foveate foveate;
    


    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        foveate = GetComponent<Foveate>();  
    }


    public bool DetectFovea(GameObject gameobject)
    { 
        Transform target = gameobject.GetComponent<Transform>();
        Renderer renderer = gameobject.GetComponent<Renderer>();

        // calculate the clip cooridnate of the object (center point)
        Vector4 objCenter = cam.WorldToScreenPoint(target.localToWorldMatrix * new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
        

        // calculate the screen cooridnate of the object (max and min point)
        Vector3 min = renderer.bounds.min;
        Vector3 max = renderer.bounds.max;

        Vector4 minScreenPos = cam.WorldToScreenPoint(  new Vector4(min.x, min.y, min.z, 1.0f));
        Vector4 maxScreenPos = cam.WorldToScreenPoint( new Vector4(max.x, max.y, max.z, 1.0f));

        // use the min and max to get the screen radius for the target object
        double objRadius = Math.Sqrt((maxScreenPos.x- minScreenPos.x ) * (maxScreenPos.x - minScreenPos.x) + (maxScreenPos.y - minScreenPos.y)* (maxScreenPos.y - minScreenPos.y)) *0.5;

        // calculate screen coordinate of fovea point, this will be the center of the focus area
        float foveaX = foveate.foveaX * cam.pixelWidth;
        float foveaY = foveate.foveaY * cam.pixelHeight;


        if (Vector2.Distance(objCenter, new Vector2(foveaX, foveaY)) < (foveate.focusAreaRadius + objRadius))
        { 
            return true;
        }

        return false;
    }

    
}
