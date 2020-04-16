﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscMenu : MonoBehaviour
{
    const string MENU_BUTTON = "Cancel";
    [SerializeField] GameObject _menu;
    CHARACTER_CONTROL_STATE _previousState;
    private void Awake()
    {
        _menu.SetActive(false);
    }
    //This should probably be replaced with a central class which controlls what window should be shown?
    void Update()
    {
        if (Input.GetButtonDown(MENU_BUTTON))
        {
            if (CharacterState.Control_State != CHARACTER_CONTROL_STATE.MENU)
            {
                _previousState = CharacterState.Control_State;
                CharacterState.SetControlState(CHARACTER_CONTROL_STATE.MENU);
                _menu.SetActive(true);
            }
            else if (_menu.activeSelf)
            {//If menu object is active, turn it off and set characterState to previousState
                ExitMenu();
            }
        }
    }

    public void ExitMenu()
    {
        CharacterState.SetControlState(_previousState);
        _menu.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}