using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonLogic : MonoBehaviour
{
    [SerializeField] GameObject tutorialPanel;
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void OpenTutorialPanel()
    {
        tutorialPanel.SetActive(true);
    }
    public void CloseTutorialpanel()
    {
        tutorialPanel.SetActive(false);
    }
}
