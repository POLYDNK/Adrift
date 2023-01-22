using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //used to access scenemanager for scene loading
using UnityEngine.UI;
public class LevelLoader : MonoBehaviour
  {
    public GameObject loadingScreen; //reference to loadingscreen in unity
    public Slider slider; //reference to slider in unity
    public TMPro.TMP_Text progressLoadText; //reference to progressload text in unity
    public void LoadLevel (string name) //load specific scene by name, ex "Overworld, Battle, Chapter1"
    //public void LoadLevel (int sceneIndex) - use int sceneindex to load scenes based on number in queue
    {
      StartCoroutine(LoadAsynchronously(name));
    }
    //LoadAsynchronously - loads specified scenes
    IEnumerator LoadAsynchronously (string name)
      {
        AsyncOperation operation = SceneManager.LoadSceneAsync(name); //Loads chosen scene asynchronously in the background, keeping current scene running while loading new scene
        //status information of scene  is placed into operation

        loadingScreen.SetActive(true);
        while (operation.isDone == false) //loops until processes is done, each loop displays progress info
        {
          float progress;
          progress = Mathf.Clamp01(operation.progress /.9f); //clamps the value from 0 to 9, affects transition time in unity
          slider.value = progress; //sets slider value from 0 to 1
          progressLoadText.text = progress * 100f + "%"; //sets and displays  loading screen progress by percentage(100)
          Debug.Log(progress); //message to show current progress from 0 to 1 of scene loading
          yield return null; //yield tells Unity to pause until next frame before continuing loop

        }
      }
  }
