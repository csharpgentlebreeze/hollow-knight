using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    public AnimatorStateInfo stateInfo;
    public AnimatorClipInfo[] clipInfo;
    public string currentClip;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        clipInfo = animator.GetCurrentAnimatorClipInfo(0);
    }
    
    public void Play(string name)
    {
        animator.Play(name);
        currentClip = name;
    }

    public bool IsEnd()
    {
        if (stateInfo.normalizedTime >= 0.99f && stateInfo.IsName(currentClip))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /*public void AddEvent(string eventName, float normalizedTime, int i_parameter = 0, string s_parameter = null, float f_parameter = 0f,Object message = null)
    {
        AnimationEvent animationEvent = new AnimationEvent();
        animationEvent.functionName = eventName;
        animationEvent.time = normalizedTime *  currentClip.length;
        animationEvent.floatParameter = normalizedTime;
        animationEvent.intParameter = i_parameter;
        animationEvent.stringParameter = s_parameter;
        animationEvent.objectReferenceParameter = message;
        currentClip.AddEvent(animationEvent);
    }*/
    
}
