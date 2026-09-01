using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:213f1e98-965c-454d-967d-2ab41ac0209a
	public partial class VolumeMenuPanel
	{
		public const string Name = "VolumeMenuPanel";
		
		[SerializeField]
		public UnityEngine.UI.Slider GlobalVolume;
		[SerializeField]
		public TMPro.TextMeshProUGUI GlobalVolumeValue;
		[SerializeField]
		public UnityEngine.UI.Slider MusicVolume;
		[SerializeField]
		public TMPro.TextMeshProUGUI MusicVolumeValue;
		[SerializeField]
		public UnityEngine.UI.Slider SoundVolume;
		[SerializeField]
		public TMPro.TextMeshProUGUI SoundVolumeValue;
		[SerializeField]
		public UnityEngine.UI.Button BackToDefault;
		[SerializeField]
		public UnityEngine.UI.Button Back;
		
		private VolumeMenuPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			GlobalVolume = null;
			GlobalVolumeValue = null;
			MusicVolume = null;
			MusicVolumeValue = null;
			SoundVolume = null;
			SoundVolumeValue = null;
			BackToDefault = null;
			Back = null;
			
			mData = null;
		}
		
		public VolumeMenuPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		VolumeMenuPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new VolumeMenuPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
