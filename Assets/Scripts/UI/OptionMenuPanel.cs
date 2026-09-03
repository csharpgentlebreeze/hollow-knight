using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class OptionMenuPanelData : UIPanelData
	{
	}
	public partial class OptionMenuPanel : UIPanel, IController
	{
		public Animator anim;
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
			mData = uiData as OptionMenuPanelData ?? new OptionMenuPanelData();
			// please add init code here
			Volume.onClick.AddListener(() =>
			{
				OpenPanel<VolumeMenuPanel>().Forget();
			});
			Back.onClick.AddListener(() =>
			{
				BackAndClose().Forget();
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
			this.GetModel<IRunTimeDataModel>().WantoEsc.UnRegister(Esc);
			Volume.onClick.RemoveAllListeners();
			Back.onClick.RemoveAllListeners();
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
		
		private void Esc(bool value)
		{
			if (value)BackAndClose().Forget();
		}
	}
}
