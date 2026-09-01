using QFramework;

public class PushCommand<T> : AbstractCommand where T : UIPanel
{
    protected override void OnExecute()
    {
        this.GetSystem<IUIPanelStackSystem>().Push<T>();
    }
}

public class PushCommand : AbstractCommand
{
    private readonly IPanel _panel;
    public PushCommand(IPanel panel)
    {
        _panel = panel;
    }
    protected override void OnExecute()
    {
        this.GetSystem<IUIPanelStackSystem>().Push(_panel);
    }
}

public class PopCommmand : AbstractCommand<PanelInfo>
{
    protected override PanelInfo OnExecute()
    {
        return this.GetSystem<IUIPanelStackSystem>().Pop();
    }
}

public class PeekCommand : AbstractCommand<PanelInfo>
{
    protected override PanelInfo OnExecute()
    {
        return this.GetSystem<IUIPanelStackSystem>().Peek();
    }
}
