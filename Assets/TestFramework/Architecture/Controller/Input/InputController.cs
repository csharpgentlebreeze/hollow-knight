using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;

public struct InputData
{
    public Vector2 Move;
    public bool WantoAttack;
    public bool WantoJump;
    public bool WantoDash;
    public bool WantoEsc;
    public bool WantoSpace;
}

public class InputController : PersistentMonoSingleton<InputController>, IController
{
    private InputData _data = new InputData();
    //复用命令对象，以免造成GC
    private InputCommand _command = new InputCommand();
    
    public InputActionReference attackAction;
    public InputActionReference jumpAction;
    public InputActionReference moveAction;
    public InputActionReference dashAction;
    public InputActionReference escAction;
    public InputActionReference spaceAction;

    public IArchitecture GetArchitecture()
    {
        return GameArchitecture.Interface;
    }
    
    private void Awake()
    {
        base.Awake();
        /*GetArchitecture().RegisterUtility<InputUtility>(this);*/
    }

    void Update()
    {
        ProcessInputData();
    }

    public void ToggleAction(bool enable)
    {
        InputActionReference[] all = {attackAction, jumpAction, moveAction, dashAction, escAction, spaceAction};
        foreach (var ac in all)
        {
            if (ac == null) continue;
            else if(enable)ac.action.Enable();
            else ac.action.Disable();
        }
    }

    private void ProcessInputData()
    {
        _data.Move = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        _data.WantoAttack = attackAction != null && attackAction.action.WasPressedThisFrame();
        _data.WantoJump = jumpAction != null && jumpAction.action.WasPressedThisFrame();
        _data.WantoDash = dashAction != null && dashAction.action.WasPressedThisFrame();
        _data.WantoEsc = escAction != null && escAction.action.WasPressedThisFrame();
        _data.WantoSpace = spaceAction != null && spaceAction.action.WasPressedThisFrame();
        _command._data = _data;
        this.SendCommand(_command);
    }
}
