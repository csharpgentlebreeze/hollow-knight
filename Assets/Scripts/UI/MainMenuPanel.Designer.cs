using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:d29bcc65-c7c3-4847-a310-6b2038a3a74b
	public partial class MainMenuPanel
	{
		public const string Name = "MainMenuPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button StartGame;
		[SerializeField]
		public UnityEngine.UI.Button Option;
		[SerializeField]
		public UnityEngine.UI.Button Achievement;
		[SerializeField]
		public UnityEngine.UI.Button QuitGame;
		
		private MainMenuPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			StartGame = null;
			Option = null;
			Achievement = null;
			QuitGame = null;
			
			mData = null;
		}
		
		public MainMenuPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		MainMenuPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new MainMenuPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
