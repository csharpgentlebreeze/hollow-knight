using System.Collections.Generic;
using QFramework;
using UnityEngine;

public interface IUIPanelStackSystem : ISystem
{
    public Stack<PanelInfo> mUIStack { get; }
    public void Push<T>() where T : UIPanel;
    public void Push(IPanel view);
    public PanelInfo Pop();
    public PanelInfo Peek();
}
public class UIPanelStackSystem : AbstractSystem, IUIPanelStackSystem
{
    public Stack<PanelInfo> mUIStack { get; } = new Stack<PanelInfo>();
    
    protected override void OnInit()
    {
        
    }

    public void Push<T>() where T : UIPanel
    {
        Push(UIKit.GetPanel<T>());
    }

    public void Push(IPanel view)
    {
        if (view != null)
        {
            mUIStack.Push(view.Info);
        }
    }
        
    public PanelInfo Pop()
    {
        if (mUIStack.Count > 0)
        {
            return mUIStack.Pop();
        }
        return null;
    }
    
    public PanelInfo Peek()
    {
        if (mUIStack.Count > 0)
        {
            return mUIStack.Peek();
        }
        return null;
    }
}
