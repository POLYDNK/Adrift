using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Serialization;

public class PanelSelector : MonoBehaviour
{
    //for relative position
    RectTransform rt;

    //variablez
    private float scrollSpeed = 0f;
    int tPos;
    GameObject wheel;

    int selectedMenuObject = 0;

    [FormerlySerializedAs("Button1")] public Button button1;
    [FormerlySerializedAs("Button2")] public Button button2;
    [FormerlySerializedAs("Button3")] public Button button3;
    [FormerlySerializedAs("Button4")] public Button button4;

    

    void Start()
    {
        wheel = GameObject.Find("FunnyWheel");      
    }

    // Update is called once per frame
    void Update()
    {
        //fetch rect transform info during each frame
        rt = GetComponent<RectTransform>();

        //pseudo-scroll rect. Making this by scratch in order to maybe make it compatible with a selection arrow.
        //scroll up
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && rt.offsetMin.y > -85f)
        {
            scrollSpeed = -200f;
            wheel.GetComponent<SpinWheel>().SetWheelOtherWay();
            //transform.Translate(0, -1f * scrollSpeed * Time.deltaTime, 0, Space.World);
        }
        //scroll down
        if (Input.GetAxis("Mouse ScrollWheel") < 0f && rt.offsetMin.y < 210f)
        {
            scrollSpeed = 200f;
            wheel.GetComponent<SpinWheel>().SetWheel();
            //transform.Translate(0, scrollSpeed * Time.deltaTime, 0, Space.World);
        }

        //translate panel each frame
        if (scrollSpeed != 0f)
        {
            transform.Translate(0, scrollSpeed * Time.deltaTime, 0, Space.World);
            scrollSpeed = scrollSpeed > 0f ? scrollSpeed - 0.8f : scrollSpeed + 0.8f;
            if (Math.Abs(scrollSpeed) < 1f || rt.offsetMin.y <= -85f || rt.offsetMin.y >= 210f)
                scrollSpeed = 0f;

        }

        tPos = (int)rt.offsetMin.y;
        switch (tPos)
        {
            case int tPos when (tPos > 128):
                button4.Select();
                selectedMenuObject = 0;
                //Button1.DoStateTransition(SelectionState.Highlighted, false);
                break;
            case int tPos when (tPos <= 128 && tPos > 55):
                button3.Select();
                selectedMenuObject = 1;
                //Button2.DoStateTransition(SelectionState.Highlighted, false);
                break;
            case int tPos when (tPos <= 55 && tPos > -16):
                button2.Select();
                selectedMenuObject = 2;
                //Button3.DoStateTransition(SelectionState.Highlighted, false);
                break;
            case int tPos when (tPos <= 16):
                button1.Select();
                selectedMenuObject = 3;
                //Button4.DoStateTransition(SelectionState.Highlighted, false);
                break;
        }

        //invoke a button click when you press Enter
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ClickBigButton();
        }
    }

    //Function to allow you to click anywhere on the screen in order to invoke whatever button is aligned with the arrow
    public void ClickBigButton()
    {
        switch (selectedMenuObject)
        {
            case 0:
                button4.onClick.Invoke();
                break;
            case 1:
                button3.onClick.Invoke();
                break;
            case 2:
                button2.onClick.Invoke();
                break;
            case 3:
                button1.onClick.Invoke();
                break;
        }
    }
}
