using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScript : MonoBehaviour
{
    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
        print("restart");
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
