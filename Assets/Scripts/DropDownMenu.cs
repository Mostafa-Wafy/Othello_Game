using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownMenu : MonoBehaviour
{

    public static int gameMode {get; set;}
    public TMPro.TMP_Dropdown myDropMenu;

    public void DropDownManager(){

        if(myDropMenu.value == 0) gameMode = 0;
        else if(myDropMenu.value == 1) gameMode = 1;
        else if(myDropMenu.value == 2) gameMode = 2;
        Debug.Log(gameMode);

    }

}
