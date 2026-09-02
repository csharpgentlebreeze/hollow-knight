using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:84af1412-f195-4b1c-85ea-7aa8310ef26c
	public partial class PausePanel
	{
		public const string Name = "PausePanel";
		
		[SerializeField]
		public UnityEngine.UI.Button Continue;
		[SerializeField]
		public UnityEngine.UI.Button Option;
		[SerializeField]
		public UnityEngine.UI.Button BackToMainMenu;
		
		private PausePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Continue = null;
			Option = null;
			BackToMainMenu = null;
			
			mData = null;
		}
		
		public PausePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		PausePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new PausePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
