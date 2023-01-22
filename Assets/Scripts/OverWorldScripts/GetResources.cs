using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetResources : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform goldTbox;
    public Transform boozeTbox;
    public Transform moraleTbox;
    public GameObject Canvas;
    public int gold;
    public int booze;
    public int morale;    

    void Start()
    {
        gold = getGold();
        booze = getBooze();
        morale = getMorale();
        Canvas=this.gameObject;
        goldTbox = Canvas.transform.Find("Gold");
        boozeTbox = Canvas.transform.Find("Booze");
        moraleTbox = Canvas.transform.Find("Morale");
        goldTbox.GetComponent<TMPro.TextMeshProUGUI>().SetText("Gold: " + gold);
        boozeTbox.GetComponent<TMPro.TextMeshProUGUI>().SetText("Booze: " + booze);
        moraleTbox.GetComponent<TMPro.TextMeshProUGUI>().SetText("Morale: " + morale);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //will update these functions once resource system is implemented//
    public int getGold()
    {
        return 456;
    }

    public int getBooze()
    {
        return 132;
    }

    public int getMorale()
    {
        return 152;
    }
}

