using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class VolumeMenuPanelData : UIPanelData
	{
	}
	public partial class VolumeMenuPanel : UIPanel, IController
	{
		public Animator anim;
		public IArchitecture GetArchitecture()
		{
			return GameArchitecture.Interface;
		}
		
		protected override void OnInit(IUIData uiData = null)
		{
			anim = GetComponent<Animator>();
			mData = uiData as VolumeMenuPanelData ?? new VolumeMenuPanelData();
			// please add init code here
			GlobalVolume.onValueChanged.AddListener(volume =>
			{
				GlobalVolumeValue.text = volume.ToString();
				this.SendCommand(new SetGlobalVolumeCommand((int)volume));
			});
			MusicVolume.onValueChanged.AddListener(volume =>
			{
				MusicVolumeValue.text = volume.ToString();
				this.SendCommand(new SetMusicVolumeCommand((int)volume));
			});
			SoundVolume.onValueChanged.AddListener(volume =>
			{
				SoundVolumeValue.text = volume.ToString();
				this.SendCommand(new SetSoundVolumeCommand((int)volume));
			});
			BackToDefault.onClick.AddListener(() =>
			{
				this.SendCommand(new InitVolumeCommand());
				GlobalVolume.value = 10;
				GlobalVolumeValue.text = "10";
				MusicVolume.value = 10;
				MusicVolumeValue.text = "10";
				SoundVolume.value = 10;
				SoundVolumeValue.text = "10";
			});
			Back.onClick.AddListener(() =>
			{
				BackAndClose().Forget();
			});
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			this.SendCommand(new PushCommand(this));
			int Global = this.SendCommand(new GetGlobalVolumeCommand());
			int Music = this.SendCommand(new GetMusicVolumeCommand());
			int sound = this.SendCommand(new GetSoundVolumeCommand());
			GlobalVolume.value = Global;
			GlobalVolumeValue.text = Global.ToString();
			MusicVolume.value = Music;
			MusicVolumeValue.text = Music.ToString();
			SoundVolume.value = sound;
			SoundVolumeValue.text = sound.ToString();
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
			this.GetModel<IRunTimeDataModel>().WantoEsc.UnRegister(Esc);
			GlobalVolume.onValueChanged.RemoveAllListeners();
			MusicVolume.onValueChanged.RemoveAllListeners();
			SoundVolume.onValueChanged.RemoveAllListeners();
			BackToDefault.onClick.RemoveAllListeners();
			Back.onClick.RemoveAllListeners();
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
			if (value)BackAndClose().Forget();
		}
	}
}
