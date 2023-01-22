using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour
{
    //variables
    public Button titleText;
    public Button returnMainMenu;
    public Button exitGame;
    public bool escapeMenuOpen = false;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // checks if "ESC" is pressed in a given frame
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            // pause the game
            if(!escapeMenuOpen)
            {
                escapeMenuOpen = true;
                //Cursor.lockState = CursorLockMode.None;
                titleText.gameObject.SetActive(true);
                returnMainMenu.gameObject.SetActive(true);
                exitGame.gameObject.SetActive(true);
            }

            // resume game
            else
            {
                escapeMenuOpen = false;
                //Cursor.lockState = CursorLockMode.Locked;
                titleText.gameObject.SetActive(false);
                returnMainMenu.gameObject.SetActive(false);
                exitGame.gameObject.SetActive(false);
            }
        }
    }

    public void ReturnToMainMenu(int scene)
    {
        SceneManager.LoadScene(0);  // title scene is 0 i believe ?
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
