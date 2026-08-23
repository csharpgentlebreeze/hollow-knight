using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPanel : BasePanel
{
    public void SetBossName(string name)
    {
        GetComponentInChildren<TMPro.TextMeshProUGUI>().text = name;
    }
}
