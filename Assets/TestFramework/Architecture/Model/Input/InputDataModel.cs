using QFramework;
using UnityEngine;

public interface IInputDataModel : IModel
{
    public BindableProperty<Vector2> Move { get; }
    public BindableProperty<bool> WantoAttack { get; }
    public BindableProperty<bool> WantoJump { get; }
    public BindableProperty<bool> WantoDash { get; }
    public BindableProperty<bool> WantoEsc { get; }
}

public class InputDataModel : AbstractModel , IInputDataModel
{
    public BindableProperty<Vector2> Move { get; } = new BindableProperty<Vector2>();
    public BindableProperty<bool> WantoAttack { get; } = new BindableProperty<bool>();
    public BindableProperty<bool> WantoJump { get; } = new BindableProperty<bool>();
    public BindableProperty<bool> WantoDash { get; } = new BindableProperty<bool>();
    public BindableProperty<bool> WantoEsc { get; } = new BindableProperty<bool>();
    protected override void OnInit()
    {
        
    }
}