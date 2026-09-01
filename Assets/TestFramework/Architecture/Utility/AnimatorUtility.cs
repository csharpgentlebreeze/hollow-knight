using System.Threading;
using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine;

public static class AnimatorUtility
{
    public static async UniTask WaitAnimationEnd(this Animator self, string clipName, int layer, CancellationToken token)
    {
        int stateHash = Animator.StringToHash(clipName);

        await UniTask.WaitUntil(() =>
        {
            var info = self.GetCurrentAnimatorStateInfo(layer);
            return stateHash == info.shortNameHash && !self.IsInTransition(layer);
        }, cancellationToken: token);

        await UniTask.WaitWhile(() =>
        {
            var info = self.GetCurrentAnimatorStateInfo(layer);
            return stateHash == info.shortNameHash && info.normalizedTime < 1;
        },cancellationToken : token);
    } 
}
