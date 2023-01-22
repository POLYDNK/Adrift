using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; // needed to access unitys audio enginer
using UnityEngine.UI; //needed to access UI elements

public class SettingsMenu : MonoBehaviour
{

    public AudioMixer audioMixer; //variable for audioMixer in uniter, will use later once music is implememnted.
    public TMPro.TMP_Dropdown resolutionDropdown; //variable to place created TMPDropdown in UI
    Resolution[] gameResolutions; //array to hold different type of resolutions, will later implement.

    void Start()
    {
      //Creating a list of resolutions that can be chosen in the settings menu >> resolution dropdown
       gameResolutions = Screen.resolutions;
       resolutionDropdown.ClearOptions(); //clears default options on dropdown SettingsMenu

       List<string> resOptions = new List<string>(); //used to convert the list of resolutions in Resolution[] array
       int currentResolutionIndex = 0; //default 0 index for resolution

       for (int i = 0; i < gameResolutions.Length; i++)
       {
         string option = gameResolutions[i].width + " x " + gameResolutions[i].height;
         resOptions.Add(option);
         //loop to add the elements taken from array into the option list as a string
        if (gameResolutions[i].width == Screen.currentResolution.width && gameResolutions[i].height == Screen.currentResolution.height)
            {
              currentResolutionIndex = i;
            }
       }

      resolutionDropdown.AddOptions(resOptions); //adds converted resolutions in List to dropdown UI
       //AddOptions takes a list of strings, not an array
      resolutionDropdown.value = currentResolutionIndex;
      resolutionDropdown.RefreshShownValue(); //displays resolution
    }
    public void SetVolume (float vol) //method to take in a float to adjust volume for game
    {
      Debug.Log(vol); //used for testing to check volume is working
      audioMixer.SetFloat("volumeParam", vol);
      /*used to reference audioMixer for vol control in unity, must use exact
      name used in audiomixer, in this case i called it "volumeParam" and set it to vol.  */
    }

    public void SetResolution(int resolutionIndex)
    {
      //sets resolution to our current resolution on computer
      Resolution resolution = gameResolutions[resolutionIndex];
      Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQuality (int qualityIndex)
    {
      QualitySettings.SetQualityLevel(qualityIndex);
      //used to reference graphics quality in unity
    }

    public void SetFullscreen (bool isFullscreen) //method to set fullscreen mode
    {
      Screen.fullScreen = isFullscreen; //used to set access to fullscreen mode in unity to isFullscreen
      Debug.Log(isFullscreen); //used for testing to see is fullscreen button working
    }

  /*  public void SetBrightness ()
    {
      //need to still implement brightness - Gerald
    }
    */
}
