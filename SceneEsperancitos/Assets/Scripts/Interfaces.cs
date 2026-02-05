using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interfaces : MonoBehaviour
{
    // Start is called before the first frame update
    public void IrScenne(string sceneName)
    {
        Debug.Log("CLICK OK");
        SceneManager.LoadScene(sceneName);
    }

    public void ConfigurationMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game is exit");
    }

    public void VolverMenu()
    {
        Debug.Log("CLICK OK");
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex > 0)
        {
            SceneManager.LoadScene(currentSceneIndex - 1);
        }
        else
        {
            Debug.LogWarning("No hay una escena anterior para volver.");
        }
    }

    // configuraciones de audio
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void MuteVolume(bool isMuted)
    {
        AudioListener.pause = isMuted;
    }
    
}
