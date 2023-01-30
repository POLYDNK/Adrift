using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NodeInfo : MonoBehaviour
{
    
    public int goldReward;
    [FormerlySerializedAs("BoozeReward")] public int boozeReward;
    [FormerlySerializedAs("MoraleReward")] public int moraleReward;
    [FormerlySerializedAs("Enemies")] public int enemies;
    public string description;
    public static TextMeshProUGUI TextBox;
    public static GameObject Textnode;
    public GameObject node;
    public static string SceneName;
    public string goToScene;
    [FormerlySerializedAs("LoadingScreen")] public GameObject loadingScreen;
    [FormerlySerializedAs("LoadingBarFill")] public Image loadingBarFill;
    public static bool StartLoading;
    public GameObject goldBox;
    public TextMeshProUGUI goldBoxText;
    public GameObject boozeBox;
    public TextMeshProUGUI boozeBoxText;
    public GameObject moraleBox;
    public TextMeshProUGUI moraleBoxText;
    //public GameObject LoadScreen;
    //public static bool displayTextBox;
    private struct Information{
        int gold;
        int booze;
        int morale;
        int enemies;
        string nodeDescription;
    };

    public void Awake()
    {
        Textnode = GameObject.Find("Text (TMP)");
        SceneName = goToScene;
        node = this.gameObject;
        TextBox=Textnode.GetComponent<TextMeshProUGUI>();
        goldBox = GameObject.Find("Gold");
        goldBoxText = goldBox.GetComponent<TextMeshProUGUI>();
        boozeBox = GameObject.Find("Booze");
        boozeBoxText = boozeBox.GetComponent<TextMeshProUGUI>();
        moraleBox = GameObject.Find("Morale");
        moraleBoxText = moraleBox.GetComponent<TextMeshProUGUI>();
        //LoadScreen = GameObject.Find("LoadScreen");
    }
    public void ShowTextBox()
    {
        TextBox.color = Color.white;
        TextBox.text = "Gold Reward: "+ goldReward + "\n";
        TextBox.text += "Booze Reward: " + boozeReward + "\n";
        TextBox.text += "Morale Reward: " + moraleReward + "\n";
        TextBox.text += "Enemies: " + enemies + "\n";
        TextBox.text += description;
        if(PlayerMove.GetCurrNode()== PlayerMove.GetDestination())
        {
            TextBox.text += "<color=red>\nYou're already here! \n Go Somewhere else!";
        }
        else if(!NodeLines.IsNeighbor(PlayerMove.GetCurrNode(),PlayerMove.GetDestination()))
        {
            //string no = "\nCannot Travel to \nthis Destination yet.";
            
            TextBox.text +=  "<color=red>\nCannot Travel to \nthis Destination \n<i> Matey</i>.</color>" ;
           // textBox.color = Color.red;
        }
        Textnode.SetActive(true);
    }

    public static void HideTextBox()
    {
        TextBox.text = "";
        Textnode.SetActive(false);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static void LoadScene()
    {
        if (SceneName != "")
        {
            //SceneManager.LoadScene(sceneName);
            //new LoadScene(sceneName);
            StartLoading = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (NodeClick.Clickobj == this.gameObject)
        {
            if (PlayerMove.DisplayTextBox == true)
            {
                this.ShowTextBox();
            }
        }
        if (StartLoading == true)
        {
            StartLoading = false; 
            LoadScene(SceneName);
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
        loadingScreen.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);


        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBarFill.fillAmount = progressValue;

            yield return null;
        }


       
    }

}
