using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Rendering;

public interface IVolumeModel : IModel
{
    public BindableProperty<int> GlobalVolume { get; }
    public BindableProperty<int> MusicVolume { get; }
    public BindableProperty<int> SoundVolume { get; }
}

public class VolumeModel : AbstractModel, IVolumeModel
{
    public BindableProperty<int> GlobalVolume { get; } = new BindableProperty<int>(10);
    public BindableProperty<int> MusicVolume { get; } = new BindableProperty<int>(10);
    public BindableProperty<int> SoundVolume { get; } = new BindableProperty<int>(10);
    protected override void OnInit()
    {
        
    }
}
