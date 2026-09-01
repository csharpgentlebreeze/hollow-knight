using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:9df0da78-151b-45db-97a3-373a7d96af49
	public partial class OptionMenuPanel
	{
		public const string Name = "OptionMenuPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button Game;
		[SerializeField]
		public UnityEngine.UI.Button Volume;
		[SerializeField]
		public UnityEngine.UI.Button Video;
		[SerializeField]
		public UnityEngine.UI.Button Keyboard;
		[SerializeField]
		public UnityEngine.UI.Button Back;
		
		private OptionMenuPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Game = null;
			Volume = null;
			Video = null;
			Keyboard = null;
			Back = null;
			
			mData = null;
		}
		
		public OptionMenuPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		OptionMenuPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new OptionMenuPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
