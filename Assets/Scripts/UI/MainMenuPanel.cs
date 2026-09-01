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
		
		public IArchitecture GetArchitecture()
		{
			return GameArchitecture.Interface;
		}
		
		protected override void OnInit(IUIData uiData = null)
		{
			anim = GetComponent<Animator>();
			mData = uiData as MainMenuPanelData ?? new MainMenuPanelData();
			// please add init code here
			StartGame.onClick.AddListener(() =>
			{
				
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
		}
		
		protected override void OnHide()
		{
			
		}
		
		protected override void OnClose()
		{
			StartGame.onClick.RemoveAllListeners();
			Option.onClick.RemoveAllListeners();
			QuitGame.onClick.RemoveAllListeners();
		}
		
		//OpenAndHide
		private async UniTask OpenPanel<T>() where T : UIPanel
		{
			anim.Play("FadeOut");
			await anim.WaitAnimationEnd("FadeOut", 0, this.GetCancellationTokenOnDestroy());
			UIKit.HidePanel(name);
			UIKit.OpenPanel<T>();
		}

		private async UniTask BackAndClose()
		{ 
			anim.Play("FadeOut");
			await anim.WaitAnimationEnd("FadeOut", 0, this.GetCancellationTokenOnDestroy());
			CloseSelf();
			this.SendCommand(new PopCommmand());
			var panel = this.SendCommand(new PeekCommand());
			string panelName = panel.GameObjName ?? panel.PanelType.Name;
			UIKit.ShowPanel(panelName);
		}
		
		private void Esc(bool value)
		{
			if (value)BackAndClose().Forget();
		}
	}
}
