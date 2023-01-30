using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconBar : MonoBehaviour
{
    public int value, maxValue, spacing;
    public GameObject emptyIcon, fullIcon;
    private List<GameObject> icons = new List<GameObject>(); 

    public void SetMaxValue(int value)
    {
        maxValue = value;
        RefreshIcons();
    }

    public void SetValue(int value)
    {   
        this.value = value;
        RefreshIcons();
    }

    public void RefreshIcons() {
        foreach(GameObject icon in icons) GameObject.Destroy(icon);
        icons.Clear();
        for(int i = 0; i < maxValue; i++) {
            GameObject icon = Instantiate(i < value ? fullIcon : emptyIcon, this.transform);
            icon.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(spacing * i, 0);
            icons.Add(icon);
        }
    }
}
