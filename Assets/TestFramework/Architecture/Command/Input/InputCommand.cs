using QFramework;

public class InputCommand : AbstractCommand
{
    public InputData _data;
    protected override void OnExecute()
    {
        IRunTimeDataModel model= this.GetModel<IRunTimeDataModel>();
        model.Move.Value = _data.Move;
        model.WantoAttack.Value = _data.WantoAttack;
        model.WantoJump.Value = _data.WantoJump;
        model.WantoDash.Value = _data.WantoDash;
        model.WantoEsc.Value = _data.WantoEsc;
    }
}
