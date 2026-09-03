using QFramework;
using UnityEngine;

/// <summary>
/// 管理游戏状态，控制游戏的暂停和继续
/// </summary>
public enum GameState
{
    Menu,
    Playing,
    Paused,
}

/// <summary>
/// 多个模块需要访问和修改的运行时数据，减少了管理层复杂的事件注册,简化事件和命令
/// </summary>
public interface IRunTimeDataModel : IModel
{
    public BindableProperty<GameState> GameStatus { get;}
    
    public BindableProperty<Vector2> Move { get; }
    public BindableProperty<bool> WantoAttack { get; }
    public BindableProperty<bool> WantoJump { get; }
    public BindableProperty<bool> WantoDash { get; }
    public BindableProperty<bool> WantoEsc { get; }
    public BindableProperty<bool> WantoSpace { get; }
}

public class RunTimeDataModel : AbstractModel, IRunTimeDataModel
{
    public BindableProperty<GameState> GameStatus { get;} = new BindableProperty<GameState>(GameState.Menu);
    
    public BindableProperty<Vector2> Move { get; } = new BindableProperty<Vector2>();
    public BindableProperty<bool> WantoAttack { get; } = new BindableProperty<bool>();
    public BindableProperty<bool> WantoJump { get; } = new BindableProperty<bool>();
    public BindableProperty<bool> WantoDash { get; } = new BindableProperty<bool>();
    public BindableProperty<bool> WantoEsc { get; } = new BindableProperty<bool>();
    public BindableProperty<bool> WantoSpace { get; } = new BindableProperty<bool>();
    protected override void OnInit()
    {
        
    }
}
