using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class GameArchitecture : Architecture<GameArchitecture>
{
    //×¢²áÄ£¿é
    protected override void Init()
    {
        ResKit.Init();
        //Model
        RegisterModel<IVolumeModel>(new VolumeModel());
        RegisterModel<IRunTimeDataModel>(new RunTimeDataModel());
        //System
        RegisterSystem<IUIPanelStackSystem>(new UIPanelStackSystem());
        //Utility
    }
}
