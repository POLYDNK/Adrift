using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public byte red;
    public byte blue;
    public byte green;
    public bool highlightIn=true;
    public bool objectHighlighted = false;
    public Color32 originalColor;
    bool initial = false;
    public static GameObject clickobj;
    
    RaycastHit hit;
    Ray ray;
    
    public GameObject obj;
    // Update is called once per frame
    void Update()
    {
        if (objectHighlighted == true)
        {
            obj.GetComponent<Renderer>().material.color = new Color32(red, green, blue, 255);
        }
    }

    private void OnMouseOver()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, 100.0f))
        {
            
            obj = hit.transform.gameObject;
            
            if (initial == false)
            {
                originalColor = obj.GetComponent<Renderer>().material.color;
                red = originalColor.r;
                blue = originalColor.b;
                green = originalColor.g;
                initial = true;
            }
            if (Input.GetMouseButtonDown(0))
            {
                clickobj = obj;

            }
            if (objectHighlighted == false)
            {
                objectHighlighted = true;
                if (NodeLines.isNeighbor(PlayerMove.currNode, obj))
                {
                    StartCoroutine(highlight());
                }
                else
                {
                    StartCoroutine(grayOut());
                }
            }
        }
    }
    public static GameObject GetClickObj()
    {
        //if (clickobj != null)
        {
            return clickobj;
        }

    }
    private void OnMouseExit()
    {
        objectHighlighted = false;
        StopCoroutine(highlight());
        red = originalColor.r;
        blue = originalColor.b;
        green = originalColor.g;
        obj.GetComponent<Renderer>().material.color = originalColor;
        highlightIn = true;
        obj = null;
        clickobj = null;
        //NodeInfo.hideTextBox();
    }
    IEnumerator highlight()
    {
        while (objectHighlighted == true)
        {
            yield return new WaitForSeconds(.05f);
            if (highlightIn == true)
            {
                if (red >= 244 || blue >= 244 || green >= 244)
                {
                    highlightIn = false;
                }
                else
                {
                    red += 10;
                    green += 10;
                    blue += 10;
                }
            }
            if (highlightIn == false)
            {
                if(red <= originalColor.r || blue <= originalColor.b || green <= originalColor.g)
                {
                    highlightIn = true;
                }
                else
                {
                    red -= 10;
                    green -= 10;
                    blue -= 10;
                }
            }

        }
    }

    IEnumerator grayOut()
    {
        while (objectHighlighted == true)
        {
            yield return new WaitForSeconds(.05f);
            red = 32;
            green = 32;
            blue = 32;

        }
    }
}
