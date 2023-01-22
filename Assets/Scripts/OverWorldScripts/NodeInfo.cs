using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NodeInfo : MonoBehaviour
{
    
    public int goldReward;
    public int BoozeReward;
    public int MoraleReward;
    public int Enemies;
    public string description;
    public static TextMeshProUGUI textBox;
    public static GameObject textnode;
    public GameObject node;
    public static string sceneName;
    public string goToScene;
    public GameObject LoadingScreen;
    public Image LoadingBarFill;
    public static bool startLoading;
    public GameObject goldBox;
    public TextMeshProUGUI goldBoxText;
    public GameObject boozeBox;
    public TextMeshProUGUI boozeBoxText;
    public GameObject moraleBox;
    public TextMeshProUGUI moraleBoxText;
    //public GameObject LoadScreen;
    //public static bool displayTextBox;
    private struct Information{
        int Gold;
        int Booze;
        int Morale;
        int Enemies;
        string NodeDescription;
    };

    public void Awake()
    {
        textnode = GameObject.Find("Text (TMP)");
        sceneName = goToScene;
        node = this.gameObject;
        textBox=textnode.GetComponent<TextMeshProUGUI>();
        goldBox = GameObject.Find("Gold");
        goldBoxText = goldBox.GetComponent<TextMeshProUGUI>();
        boozeBox = GameObject.Find("Booze");
        boozeBoxText = boozeBox.GetComponent<TextMeshProUGUI>();
        moraleBox = GameObject.Find("Morale");
        moraleBoxText = moraleBox.GetComponent<TextMeshProUGUI>();
        //LoadScreen = GameObject.Find("LoadScreen");
    }
    public void showTextBox()
    {
        textBox.color = Color.white;
        textBox.text = "Gold Reward: "+ goldReward + "\n";
        textBox.text += "Booze Reward: " + BoozeReward + "\n";
        textBox.text += "Morale Reward: " + MoraleReward + "\n";
        textBox.text += "Enemies: " + Enemies + "\n";
        textBox.text += description;
        if(PlayerMove.getCurrNode()== PlayerMove.getDestination())
        {
            textBox.text += "<color=red>\nYou're already here! \n Go Somewhere else!";
        }
        else if(!NodeLines.isNeighbor(PlayerMove.getCurrNode(),PlayerMove.getDestination()))
        {
            //string no = "\nCannot Travel to \nthis Destination yet.";
            
            textBox.text +=  "<color=red>\nCannot Travel to \nthis Destination \n<i> Matey</i>.</color>" ;
           // textBox.color = Color.red;
        }
        textnode.SetActive(true);
    }

    public static void hideTextBox()
    {
        textBox.text = "";
        textnode.SetActive(false);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static void loadScene()
    {
        if (sceneName != "")
        {
            //SceneManager.LoadScene(sceneName);
            //new LoadScene(sceneName);
            startLoading = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (NodeClick.clickobj == this.gameObject)
        {
            if (PlayerMove.displayTextBox == true)
            {
                this.showTextBox();
            }
        }
        if (startLoading == true)
        {
            startLoading = false; 
            LoadScene(sceneName);
        }
    }

    public void LoadScene(string sceneId)
    {

        StartCoroutine(LoadSceneAsync(sceneId));
    }

    IEnumerator LoadSceneAsync(string sceneId)
    {
        string ogText1 = goldBoxText.text;
        string ogText2 = boozeBoxText.text;
        string ogText3 = moraleBoxText.text;


        goldBoxText.text = "";
        boozeBoxText.text = "";
        moraleBoxText.text = "";
        //LoadScreen.SetActive(true);
        LoadingScreen.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);


        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            LoadingBarFill.fillAmount = progressValue;

            yield return null;
        }


       
    }

}
