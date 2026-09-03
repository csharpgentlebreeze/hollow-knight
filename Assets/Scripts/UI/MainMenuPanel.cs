using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class MainMenuPanelData : UIPanelData
	{
	}
	public partial class MainMenuPanel : UIPanel, IController
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
			mData = uiData as MainMenuPanelData ?? new MainMenuPanelData();
			// please add init code here
			this.GetModel<IRunTimeDataModel>().GameStatus.Value = GameState.Menu;
			StartGame.onClick.AddListener(() =>
			{
				this.GetModel<IRunTimeDataModel>().GameStatus.Value = GameState.Playing;
				Begin().Forget();
			});
			Option.onClick.AddListener(() =>
			{
				OpenPanel<OptionMenuPanel>().Forget();
			});
			QuitGame.onClick.AddListener(() =>
			{
				Application.Quit();
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
			StartGame.onClick.RemoveAllListeners();
			Option.onClick.RemoveAllListeners();
			QuitGame.onClick.RemoveAllListeners();
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

		private async UniTask BackAndClose()
		{
			if (mIsClosing) return;
			mIsClosing = true;
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

		private async UniTask Begin()
		{
			if (mIsClosing) return;
			mIsClosing = true;
			anim.Play("FadeOut");
			await anim.WaitAnimationEnd("FadeOut", 0, this.GetCancellationTokenOnDestroy());
			resLoader.LoadSceneAsync("Opening",onStartLoading: (op) =>
			{
				op.completed += (op) =>
				{
					UIKit.OpenPanel<Chapter>();
				};
			});
			// OnOpen 时把自己 Push 进了自定义 UI 栈，这里不会再 Peek/Show 任何面板，
			// 必须自己 Pop 掉，否则这条记录会一直悬空在栈底：它引用的 PanelInfo
			// 在 CloseSelf() 时会被 UIKit 回收进对象池并被其它面板复用/改写，
			// 之后任何 PeekCommand 都可能读到内容错乱、指向已销毁面板的脏数据。
			this.SendCommand(new PopCommmand());
			CloseSelf();
		}
		
		private void Esc(bool value)
		{
			if (value)Application.Quit();
		}
	}
}
