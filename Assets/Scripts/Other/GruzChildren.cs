using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class GruzChildren : MonoBehaviour
{
    public GameObject[] children;

    private int deadCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEventListener("Burst",Burst);
        EventManager.Instance.AddEventListener("GruzDead",GruzDead);
    }

    // Update is called once per frame
    void Update()
    {
        if (deadCount == children.Length)
        {
            EventManager.Instance.EventTrigger("GruzAllDead");
        }
               
    }

    public void Burst()
    {
        foreach (GameObject child in children)
        {
            if (child != null)
            {
                child.SetActive(true);
                child.GetComponent<Gruz>().player = GameObject.FindGameObjectWithTag("Player");
            }
        }
    }

    public void GruzDead()
    {
        deadCount++;
    }

    public void OnDisable()
    {
        EventManager.Instance.RemoveEventListener("Burst",Burst);
        EventManager.Instance.RemoveEventListener("GruzDead",GruzDead);
    }
}
