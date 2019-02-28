using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneController : MonoBehaviour {

    public void LoadScene(string sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }

    public void Quit()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
}
