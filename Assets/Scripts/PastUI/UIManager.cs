using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum E_UI_Layer
{
    Bot,
    Mid,
    Top,
    System
}
/// <summary>
/// 管理UI界面，提供显示、隐藏、获取界面等功能
/// </summary>
public class UIManager : MonoSingleton<UIManager>
{
    private Dictionary<string, BasePanel> panelDict = new Dictionary<string, BasePanel>();
    private Stack<string> panelStack = new Stack<string>();
    private RectTransform canvas;
    private Texture2D cursor;

    private Transform bot;
    private Transform mid;
    private Transform top;
    private Transform system;
    private void Awake()
    {
        GameObject obj = ResourceManager.Instance.Load<GameObject>("UI/Canvas");
        canvas = obj.GetComponent<RectTransform>();
        DontDestroyOnLoad(obj);
        
        bot = canvas.Find("Bot");
        mid = canvas.Find("Mid");
        top = canvas.Find("Top");
        system = canvas.Find("System");
        
        obj = ResourceManager.Instance.Load<GameObject>("UI/EventSystem");
        DontDestroyOnLoad(obj);
        
        cursor = ResourceManager.Instance.Load<Texture2D>("UI/Cursor");
    }

    private void Start()
    {
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);

        /*InputManager.Instance.back.performed += (context) =>
        {
            AudioManager.Instance.PlaySound("UI/ui_button_confirm",false);
            BackToLast();
        };*/
    }
    
    public Transform GetLayer(E_UI_Layer layer) //获取UI层级的Transform，方便直接在外部进行操作
    {
        switch (layer)
        {
            case E_UI_Layer.Bot:
                return bot;
            case E_UI_Layer.Mid:
                return mid;
            case E_UI_Layer.Top:
                return top;
            case E_UI_Layer.System:
                return system;
        }

        return null;
    }
    
    public void ShowPanel<T>(string panelName,E_UI_Layer layer = E_UI_Layer.Mid,UnityAction<T> callback = null) where T : BasePanel //显示界面，提供界面名称、所在层级和回调函数（回调函数在界面加载完成后调用，参数为加载完成的界面组件）
    {
        if(panelDict.ContainsKey(panelName))
        {
            panelDict[panelName].ShowMe();
            panelDict[panelName].gameObject.SetActive(true);
            if(callback != null)
                callback(panelDict[panelName] as T);
            return;
        }
        ResourceManager.Instance.LoadAsync<GameObject>("UI/" + panelName, (obj) =>
        {
            Transform parent = bot;
            switch (layer)
            {
                case E_UI_Layer.Mid:
                    parent = mid;
                    break;
                case E_UI_Layer.Top:
                    parent = top;
                    break;
                case E_UI_Layer.System:
                    parent = system;
                    break;
            }
            
            obj.transform.SetParent(parent,false);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            
            /*(obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.zero;*/

            T panel = obj.GetComponent<T>();
            if(callback != null)
                callback(panel);
            
            panel.ShowMe();
            
            panelDict.Add(panelName, panel);
            if (layer == E_UI_Layer.Mid)
            {
                panelStack.Push(panelName);
            }
            
        });
    }
    
    public void HidePanel(string panelName) //隐藏界面，提供界面名称，如果界面存在则调用界面组件的HideMe方法并取消激活界面对象，否则不进行任何操作
    {
        if (panelDict.ContainsKey(panelName))
        { 
            panelDict[panelName].HideMe();
            panelDict[panelName].gameObject.SetActive(false);
        }
    }

    public void ClosePanel(string panelName)
    {
        if (panelDict.ContainsKey(panelName))
        {
            panelDict[panelName].HideMe();
            BasePanel panel = panelDict[panelName];
            panelDict.Remove(panelName);
            Destroy(panel.gameObject);
        }
    }
    
    public void BackToLast() //返回上一个界面，如果界面栈中有界面则弹出栈顶界面并隐藏销毁它，然后显示栈顶的下一个界面，否则不进行任何操作
    {
        if(panelStack.Count > 0)
        {
            string topPanel = panelStack.Pop();
            /*if (topPanel == "PausePanel")
            {
                (panelDict[topPanel] as PausePanel).Continue();
                return;
            }*/
            StartCoroutine(panelDict[topPanel].WaitAndHide(topPanel, () =>
            {
                Destroy(panelDict[topPanel].gameObject);
                panelDict.Remove(topPanel);
                ShowPanel<BasePanel>(panelStack.Peek());
            }));
        }
    }

    public void ClearPanel()
    {
        panelDict.Clear();
    }

    public T GetPanel<T>(string name) where T : BasePanel //获取界面组件，提供界面名称，返回对应类型的界面组件，如果界面不存在则返回null
    {
        if(panelDict.ContainsKey(name))
        {
            return panelDict[name] as T;
        }
        return null;
    }

    public static void AddCustomEventListener(UIBehaviour control,EventTriggerType type,UnityAction<BaseEventData> listener) //为UI控件添加自定义事件监听器，提供控件、事件类型和回调函数（回调函数在事件触发时调用，参数为事件数据）
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = control.gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(listener);
        trigger.triggers.Add(entry);
    }
}
