using QFramework;

public class InitVolumeCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        IVolumeModel model = this.GetModel<IVolumeModel>();
        model.GlobalVolume.Value = 10;
        model.MusicVolume.Value = 10;
        model.SoundVolume.Value = 10;
    }
}

public class GetGlobalVolumeCommand : AbstractCommand<int>
{
    protected override int OnExecute()
    {
        return this.GetModel<IVolumeModel>().GlobalVolume.Value;
    }
}

public class GetMusicVolumeCommand : AbstractCommand<int>
{
    protected override int OnExecute()
    {
        return this.GetModel<IVolumeModel>().MusicVolume.Value;
    }
}

public class GetSoundVolumeCommand : AbstractCommand<int>
{
    protected override int OnExecute()
    {
        return this.GetModel<IVolumeModel>().SoundVolume.Value;
    }
}

public class SetGlobalVolumeCommand : AbstractCommand
{
    private int _volume;
    public SetGlobalVolumeCommand(int volume)
    {
        _volume = volume;
    }
    protected override void OnExecute()
    {
        this.GetModel<IVolumeModel>().GlobalVolume.Value = _volume;
    }
}

public class SetMusicVolumeCommand : AbstractCommand
{
    private int _volume;
    public SetMusicVolumeCommand(int volume)
    {
        _volume = volume;
    }
    protected override void OnExecute()
    {
        this.GetModel<IVolumeModel>().MusicVolume.Value = _volume;
    }
}

public class SetSoundVolumeCommand : AbstractCommand
{
    private int _volume;
    public SetSoundVolumeCommand(int volume)
    {
        _volume = volume;
    }
    protected override void OnExecute()
    {
        this.GetModel<IVolumeModel>().SoundVolume.Value = _volume;
    }
}


