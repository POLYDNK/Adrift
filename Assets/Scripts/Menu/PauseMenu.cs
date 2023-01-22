using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool PausedStatus = false;
    public GameObject pauseMenuUI;
    public GameObject inventoryUI;
    public GameObject characterUI;
    public GameObject settingsUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
          if (PausedStatus)
          {
            ResumeGame();
            inventoryUI.SetActive(false);
            characterUI.SetActive(false);
            settingsUI.SetActive(false);
          }
          else
          {
            PauseGame();
          }

        }
    }
    void ResumeGame()
    {
      pauseMenuUI.SetActive(false);
      Time.timeScale = 1f;
      PausedStatus = false;
    }
    void PauseGame()
    {
      pauseMenuUI.SetActive(true);
      Time.timeScale = 0f;
      PausedStatus = true;
    }
    public void ExitGame () //method exit the game, attach to exitgamebutton
      {
        Debug.Log("Exiting Adrift..."); //For testing purposes, text shows in log
        Application.Quit(); //exits and quits the game application
      }
      public void LoadMenuScene()
      {
        SceneManager.LoadScene("TitleMenuScene");
      }
}
