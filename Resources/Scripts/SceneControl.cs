
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SceneControl : MonoBehaviour
{
    public int sceneID = 0;
    public float displayTime = 5.0f;
    private int sceneCount = 0;
    private bool frozeScene = false;
    private List<float> displayTimeList;
    private static SceneControl instance = null;

    void Awake()
    {
        // if this prefab hasn't be in the scene yet, leave it in the scene
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            return;
        }

        // if not, prevent recreating the object in a new scene
        Destroy(this.gameObject);
    }

   

    private void Start()
    {
        // initiate the parameters
        sceneCount = SceneManager.sceneCountInBuildSettings;
        frozeScene = false;

        // store display time for every scenes
        displayTimeList = new List<float>{ 30.0f, 15.0f, 5.0f, 30.0f, 15.0f, 5.0f, 30.0f, 15.0f, 5.0f };
        displayTime = displayTimeList[0];
    }

    private void Update()
    {
        // check if the time is passing
        if (frozeScene == false)
        // Timer count down
            displayTime -= Time.deltaTime;

        // Check whether the display time for the current scene is long enough
        if (displayTime < 0)
        {
            // move to next scene by adding the scene ID
            sceneID++;

            // if there is no more scene to display, end the application
            if (sceneID >= sceneCount)
            {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
                Application.Quit();

            }

            // display scene based on scene ID
            SceneManager.LoadScene(sceneID);
            displayTime = displayTimeList[sceneID];

        }

        // check if the space key is pressed, if so, change the status of timer
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            frozeScene = !frozeScene;
        }


    }

}
