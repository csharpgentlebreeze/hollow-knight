using Cysharp.Threading.Tasks;
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
		public IArchitecture GetArchitecture()
		{
			return GameArchitecture.Interface;
		}
		protected override void OnInit(IUIData uiData = null)
		{
			anim = GetComponent<Animator>();
			resLoader = ResLoader.Allocate();
			mData = uiData as PausePanelData ?? new PausePanelData();
			// please add init code here
			this.GetModel<IRunTimeDataModel>().GameStatus.Value = GameState.Paused;
			Continue.onClick.AddListener(() =>
			{
				this.GetModel<IRunTimeDataModel>().GameStatus.Value = GameState.Playing;
				BackAndClose().Forget();
			});
			Option.onClick.AddListener(() =>
			{
				OpenPanel<OptionMenuPanel>().Forget();
			});
			BackToMainMenu.onClick.AddListener(() =>
			{
				this.GetModel<IRunTimeDataModel>().GameStatus.Value = GameState.Menu;
				Back().Forget();
			});
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			this.SendCommand(new PushCommand(this));
		}
		
		protected override void OnShow()
		{
			this.GetModel<IRunTimeDataModel>().WantoEsc.Register(Esc);
		}
		
		protected override void OnHide()
		{
			this.GetModel<IRunTimeDataModel>().WantoEsc.UnRegister(Esc);
		}
		
		protected override void OnClose()
		{
			Continue.onClick.RemoveAllListeners();
			Option.onClick.RemoveAllListeners();
			BackToMainMenu.onClick.RemoveAllListeners();
		}
		
		//OpenAndHide
		private async UniTask OpenPanel<T>(UILevel level = UILevel.Common,IUIData data = null,string assetBundleName = null, string prefabName = null) where T : UIPanel
		{
			anim.Play("FadeOut");
			await anim.WaitAnimationEnd("FadeOut", 0, this.GetCancellationTokenOnDestroy());
			UIKit.HidePanel(name);
			UIKit.OpenPanel<T>(level,data,assetBundleName,prefabName);
		}
		
		private async UniTask BackAndClose()
		{ 
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
			if (value)
			{
				this.GetModel<IRunTimeDataModel>().GameStatus.Value = GameState.Playing;
				BackAndClose().Forget();
			}
		}
		
		private async UniTask Back()
		{ 
			anim.Play("FadeOut");
			await anim.WaitAnimationEnd("FadeOut", 0, this.GetCancellationTokenOnDestroy());
			CloseSelf();
			resLoader.LoadSceneAsync("MainMenu");
		}
	}
}
