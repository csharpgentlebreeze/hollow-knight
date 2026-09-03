using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine;

// 使用范式：
// var handle = UniTaskManager.Instance.Run(async h =>
// {
//     for (int i = 0; i < 10; i++)
//     {
//         await h.WaitWhilePausedAsync();
//         await UniTask.Delay(500, cancellationToken: h.CancellationToken);
//     }
// }, name: "DemoLoop", timeoutMs: 10000);
//
// UniTaskManager.Instance.Pause(handle.Id);
// UniTaskManager.Instance.Resume(handle.Id);
// UniTaskManager.Instance.Cancel(handle.Id);

public enum UniTaskState
{
    Pending,
    Running,
    Paused,
    Succeeded,
    Faulted,
    Canceled,
    TimedOut,
}

public class UniTaskHandle
{
    public int Id { get; }
    public string Name { get; }
    public UniTaskState State { get; internal set; }
    public float StartTime { get; }
    public CancellationToken CancellationToken { get; }
    public bool IsPaused { get; private set; }

    internal readonly CancellationTokenSource TimeoutCts;
    internal readonly CancellationTokenSource ManualCts;
    internal readonly CancellationTokenSource LinkedCts;

    private UniTaskCompletionSource mResumeSignal;

    internal UniTaskHandle(int id, string name, CancellationTokenSource timeoutCts, CancellationTokenSource manualCts,
        CancellationTokenSource linkedCts)
    {
        Id = id;
        Name = name;
        State = UniTaskState.Pending;
        StartTime = Time.realtimeSinceStartup;
        TimeoutCts = timeoutCts;
        ManualCts = manualCts;
        LinkedCts = linkedCts;
        CancellationToken = linkedCts.Token;
    }

    internal void Pause()
    {
        if (IsPaused) return;
        IsPaused = true;
        mResumeSignal = new UniTaskCompletionSource();
    }

    internal void Resume()
    {
        if (!IsPaused) return;
        IsPaused = false;
        mResumeSignal?.TrySetResult();
        mResumeSignal = null;
    }

    public async UniTask WaitWhilePausedAsync()
    {
        while (IsPaused)
        {
            await mResumeSignal.Task;
        }
    }
}

public class UniTaskManager : MonoSingleton<UniTaskManager>
{
    private readonly Dictionary<int, UniTaskHandle> mTasks = new Dictionary<int, UniTaskHandle>();
    private int mNextId;

    public UniTaskHandle Run(Func<UniTaskHandle, UniTask> taskFunc, string name = null,
        int timeoutMs = -1, CancellationToken externalToken = default)
    {
        var id = ++mNextId;
        name = string.IsNullOrEmpty(name) ? $"Task#{id}" : name;

        var manualCts = new CancellationTokenSource();
        CancellationTokenSource timeoutCts = timeoutMs > 0 ? new CancellationTokenSource(timeoutMs) : null;

        CancellationTokenSource linkedCts = timeoutCts != null
            ? CancellationTokenSource.CreateLinkedTokenSource(manualCts.Token, timeoutCts.Token, externalToken)
            : CancellationTokenSource.CreateLinkedTokenSource(manualCts.Token, externalToken);

        var handle = new UniTaskHandle(id, name, timeoutCts, manualCts, linkedCts);
        mTasks[id] = handle;

        RunInternal(handle, taskFunc).Forget();

        return handle;
    }

    private async UniTaskVoid RunInternal(UniTaskHandle handle, Func<UniTaskHandle, UniTask> taskFunc)
    {
        handle.State = UniTaskState.Running;
        LogKit.I($"[UniTaskManager] Task#{handle.Id} '{handle.Name}' started.");

        try
        {
            await taskFunc(handle);

            handle.State = UniTaskState.Succeeded;
            var elapsed = Time.realtimeSinceStartup - handle.StartTime;
            LogKit.I($"[UniTaskManager] Task#{handle.Id} '{handle.Name}' succeeded. Elapsed={elapsed:F2}s.");
        }
        catch (OperationCanceledException)
        {
            if (handle.TimeoutCts != null && handle.TimeoutCts.IsCancellationRequested)
            {
                handle.State = UniTaskState.TimedOut;
                LogKit.W($"[UniTaskManager] Task#{handle.Id} '{handle.Name}' timed out.");
            }
            else
            {
                handle.State = UniTaskState.Canceled;
                LogKit.I($"[UniTaskManager] Task#{handle.Id} '{handle.Name}' canceled.");
            }
        }
        catch (Exception ex)
        {
            handle.State = UniTaskState.Faulted;
            LogKit.E($"[UniTaskManager] Task#{handle.Id} '{handle.Name}' faulted.");
            LogKit.E(ex);
        }
        finally
        {
            handle.LinkedCts.Dispose();
            handle.ManualCts.Dispose();
            handle.TimeoutCts?.Dispose();
            mTasks.Remove(handle.Id);
        }
    }

    public bool Pause(int taskId)
    {
        if (!mTasks.TryGetValue(taskId, out var handle) || handle.State != UniTaskState.Running) return false;

        handle.Pause();
        handle.State = UniTaskState.Paused;
        LogKit.I($"[UniTaskManager] Task#{handle.Id} '{handle.Name}' paused.");
        return true;
    }

    public bool Resume(int taskId)
    {
        if (!mTasks.TryGetValue(taskId, out var handle) || handle.State != UniTaskState.Paused) return false;

        handle.Resume();
        handle.State = UniTaskState.Running;
        LogKit.I($"[UniTaskManager] Task#{handle.Id} '{handle.Name}' resumed.");
        return true;
    }

    public bool Cancel(int taskId)
    {
        if (!mTasks.TryGetValue(taskId, out var handle)) return false;

        handle.Resume();
        handle.ManualCts.Cancel();
        return true;
    }

    public UniTaskState? GetState(int taskId)
    {
        return mTasks.TryGetValue(taskId, out var handle) ? handle.State : (UniTaskState?)null;
    }

    public IReadOnlyCollection<UniTaskHandle> GetAllTasks()
    {
        return mTasks.Values;
    }

    public void CancelAll()
    {
        // 快照一份再遍历：Cancel() 可能同步触发任务的取消延续并从 mTasks 中移除自己，
        // 直接遍历 mTasks.Values 会导致“集合已修改”异常。
        foreach (var handle in new List<UniTaskHandle>(mTasks.Values))
        {
            handle.Resume();
            handle.ManualCts.Cancel();
        }
    }

    protected override void OnDestroy()
    {
        CancelAll();
        base.OnDestroy();
    }

    protected override void OnApplicationQuit()
    {
        CancelAll();
        base.OnApplicationQuit();
    }
}
