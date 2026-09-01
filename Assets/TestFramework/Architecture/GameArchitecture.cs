using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class GameArchitecture : Architecture<GameArchitecture>
{
    //×¢²áÄ£¿é
    protected override void Init()
    {
        //Model
        RegisterModel<IInputDataModel>(new InputDataModel());
        RegisterModel<IVolumeModel>(new VolumeModel());
        //System
        RegisterSystem<IUIPanelStackSystem>(new UIPanelStackSystem());
        //Utility
    }
}
