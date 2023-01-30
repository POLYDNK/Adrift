using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GetResources : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform goldTbox;
    public Transform boozeTbox;
    public Transform moraleTbox;
    [FormerlySerializedAs("Canvas")] public GameObject canvas;
    public int gold;
    public int booze;
    public int morale;    

    void Start()
    {
        gold = GetGold();
        booze = GetBooze();
        morale = GetMorale();
        canvas=this.gameObject;
        goldTbox = canvas.transform.Find("Gold");
        boozeTbox = canvas.transform.Find("Booze");
        moraleTbox = canvas.transform.Find("Morale");
        goldTbox.GetComponent<TMPro.TextMeshProUGUI>().SetText("Gold: " + gold);
        boozeTbox.GetComponent<TMPro.TextMeshProUGUI>().SetText("Booze: " + booze);
        moraleTbox.GetComponent<TMPro.TextMeshProUGUI>().SetText("Morale: " + morale);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //will update these functions once resource system is implemented//
    public int GetGold()
    {
        return 456;
    }

    public int GetBooze()
    {
        return 132;
    }

    public int GetMorale()
    {
        return 152;
    }
}

