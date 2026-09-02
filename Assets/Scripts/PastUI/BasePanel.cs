using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class BasePanel : MonoBehaviour
{
    private Dictionary<string,List<UIBehaviour>> controlDict = new Dictionary<string,List<UIBehaviour>>();

    public Animator animator;
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        FindChildrenControl<Button>();
        FindChildrenControl<Text>();
        FindChildrenControl<Image>();
        FindChildrenControl<Slider>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<TextMeshProUGUI>();
        /*foreach (string key in controlDict.Keys)
        {
            if (controlDict[key] is TextMeshProUGUI)
            {
                print(key);
                foreach (var value in controlDict[key])
                {
                    print(value.GetType());
                }
            }
            
        }*/
    }
    public virtual void ShowMe()
    {
        
    }

    public virtual void HideMe()
    {
        
    }

    protected virtual void OnClick(string btnName)
    {
        
    }

    public IEnumerator WaitAndHide(string panelName,UnityAction callback = null)
    {
        animator.Play("FadeOut");
        yield return new WaitForSecondsRealtime(0.5f);
        /*UIManager.Instance.HidePanel(panelName);*/
        callback?.Invoke();
    }
    
    public IEnumerator WaitAndClose(string panelName,UnityAction callback = null)
    {
        animator.Play("FadeOut");
        yield return new WaitForSecondsRealtime(0.5f);
        /*UIManager.Instance.ClosePanel(panelName);*/
        callback?.Invoke();
    }
    

    protected T GetControl<T>(string controlName) where T : UIBehaviour
    {
        if(controlDict.ContainsKey(controlName))
        {
            for (int i = 0; i < controlDict[controlName].Count; i++)
            {
                if(controlDict[controlName][i] is T)
                {
                    return controlDict[controlName][i] as T;
                }
            }
        }
        return null;
    }

    private void FindChildrenControl<T>() where T : UIBehaviour
    {
        T[] controls = GetComponentsInChildren<T>();
        for (int i = 0; i < controls.Length; i++)
        {
            string objName = controls[i].name;
            if(controlDict.ContainsKey(objName))
            {
                controlDict[objName].Add(controls[i]);
            }
            else
            {
                controlDict.Add(objName, new List<UIBehaviour>(){controls[i]});
            }

            if (controls[i] is Button)
            {
                (controls[i] as Button).onClick.AddListener(() =>
                {
                    OnClick(objName);
                });
            }
        }
    }
}
