using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElemScript : MonoBehaviour
{
    Text txt;
    private static string toDisplay;
    private static int toEdit = 0;
    // Start is called before the first frame update
    void Start()
    {
        txt = gameObject.GetComponent<Text>();
        txt.text = "Hello World";
    }
    public static void edit(string str)
    {
        toDisplay = str;
        toEdit = 1;
    }


    // Update is called once per frame
    void Update()
    {
        if (toEdit == 1)
        {
            txt.text = "Edited: " + toDisplay;
        }
    }
}
