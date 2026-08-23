using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoSingleton<InputManager>
{
    private PlayerInput playerInput;

    public InputAction attack;
    public InputAction jump;
    public InputAction Horizontal;
    public InputAction Vertical;
    public InputAction dash;
    public InputAction pause;
    public InputAction back;
    public InputAction goOn;
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.ActivateInput();
        
        attack = playerInput.actions.FindAction("Attack");
        jump = playerInput.actions.FindAction("Jump");
        Horizontal = playerInput.actions.FindAction("Horizontal");
        Vertical = playerInput.actions.FindAction("Vertical");
        dash = playerInput.actions.FindAction("Dash");
        pause = playerInput.actions.FindAction("Pause");
        back = playerInput.actions.FindAction("Back");
        goOn = playerInput.actions.FindAction("Continue");
    }

    void Update()
    {
        switch (GameManager.Instance.gameState)
        {
            case GameState.Paused:
                SetActionInput("Pause", false);
                SetActionInput("Back", false);
                SetActionInput("Continue", true);
                break;
            case GameState.Playing:
                SetActionInput("Pause", true);
                SetActionInput("Back", false);
                SetActionInput("Continue", false);
                break;
            case GameState.UI:
                SetActionInput("Pause", false);
                SetActionInput("Back", true);
                SetActionInput("Continue", false);
                break;
        }
    }
    
    public bool IsMapEnabled(string map)
    {
        return playerInput.actions.FindActionMap(map).enabled;
    }

    public void SetMapInput(string map,bool enable)
    {
        if (enable)
        {
            playerInput.actions.FindActionMap(map).Enable();
        }
        else
        {
            playerInput.actions.FindActionMap(map).Disable();
        }
    }
    
    public bool IsActionEnabled(string action)
    {
        return playerInput.actions.FindAction(action).enabled;
    }
    
    public void SetActionInput(string action,bool enable)
    {
        if (enable)
        {
            playerInput.actions.FindAction(action).Enable();
        }
        else
        {
            playerInput.actions.FindAction(action).Disable();
        }
    }
}
