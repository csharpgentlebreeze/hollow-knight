using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class PausePanelData : UIPanelData
	{
	}
	public partial class PausePanel : UIPanel, IController
	{
		public Animator anim;
		private ResLoader resLoader;
		private bool mIsClosing;
		private CancellationTokenSource tokenSource;
		public IArchitecture GetArchitecture()
		{
			return GameArchitecture.Interface;
		}
		protected override void OnInit(IUIData uiData = null)
		{
			tokenSource = new CancellationTokenSource();
			anim = GetComponent<Animator>();
			resLoader = ResLoader.Allocate();
			mData = uiData as PausePanelData ?? new PausePanelData();
			// please add init code here
			this.GetModel<IRunTimeDataModel>().GameStatus.Value = GameState.Paused;
			Continue.onClick.AddListener(() =>
			{
				ResumeAndClose().Forget();
			});
			Option.onClick.AddListener(() =>
			{
				OpenPanel<OptionMenuPanel>().Forget();
			});
			BackToMainMenu.onClick.AddListener(() =>
			{
				Back().Forget();
			});
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			this.SendCommand(new PushCommand(this));
		}
		
		protected override void OnShow()
		{
			mIsClosing = false;
			this.GetModel<IRunTimeDataModel>().WantoEsc.Register(Esc);
		}
		
		protected override void OnHide()
		{
			this.GetModel<IRunTimeDataModel>().WantoEsc.UnRegister(Esc);
		}
		
		protected override void OnClose()
		{
			this.GetModel<IRunTimeDataModel>().GameStatus.Value = GameState.Playing;
			Continue.onClick.RemoveAllListeners();
			Option.onClick.RemoveAllListeners();
			BackToMainMenu.onClick.RemoveAllListeners();
			resLoader.Recycle2Cache();
			resLoader = null;
		}
		
		//OpenAndHide
		private async UniTask OpenPanel<T>(UILevel level = UILevel.Common,IUIData data = null,string assetBundleName = null, string prefabName = null) where T : UIPanel
		{
			if (mIsClosing) return;
			mIsClosing = true;
			anim.Play("FadeOut");
			await anim.WaitAnimationEnd("FadeOut", 0, this.GetCancellationTokenOnDestroy());
			UIKit.HidePanel(name);
			UIKit.OpenPanel<T>(level,data,assetBundleName,prefabName);
		}

		// 恢复游玩：淡出动画需要 Time.timeScale 恢复才能播放，
		// 但 GameStatus 要等本面板真正关闭（OnHide 注销掉自己的 Esc 监听）之后才切换，
		// 否则 GameManager 会在本面板的 Esc 监听尚未注销时就把 WantoEsc.Register(Paused) 重新挂上，
		// 造成同一次 Esc 输入被本面板和 GameManager 同时处理的竞态。
		private async UniTask ResumeAndClose()
		{
			if (mIsClosing) return;
			mIsClosing = true;
			Time.timeScale = 1f;
			anim.Play("FadeOut");
			await anim.WaitAnimationEnd("FadeOut", 0, this.GetCancellationTokenOnDestroy());
			this.SendCommand(new PopCommmand());
			var panel = this.SendCommand(new PeekCommand());
			if (panel != null)
			{
				string panelName = panel.GameObjName ?? panel.PanelType.Name;
				UIKit.GetPanel(panelName).Show();
			}
			CloseSelf();
		}

		private void Esc(bool value)
		{
			if (value) ResumeAndClose().Forget();
		}

		private async UniTask Back()
		{
			if (mIsClosing) return;
			mIsClosing = true;
			Time.timeScale = 1f;
			anim.Play("FadeOut");
			await anim.WaitAnimationEnd("FadeOut", 0,this.GetCancellationTokenOnDestroy());
			// 同 MainMenuPanel.Begin()：这里直接换场景，不会 Peek/Show 其它面板，
			// 必须先把自己 Pop 掉，避免悬空的 PanelInfo 引用被对象池复用后污染后续的 PeekCommand。
			this.SendCommand(new PopCommmand());
			CloseSelf();
			this.GetModel<IRunTimeDataModel>().GameStatus.Value = GameState.Menu;
			resLoader.LoadSceneAsync("MainMenu");
		}
	}
}
