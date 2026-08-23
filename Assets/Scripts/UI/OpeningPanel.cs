using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningPanel : BasePanel
{
    public void Play()
    {
        Opening opening = FindObjectOfType<Opening>();
        if (opening != null)
        {
            opening.prologue.Play();
        }
    }
}
