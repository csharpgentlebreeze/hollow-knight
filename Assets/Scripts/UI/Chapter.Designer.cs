using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:2ff399cd-a591-48df-ad12-d35b823631d0
	public partial class Chapter
	{
		public const string Name = "Chapter";
		
		
		private ChapterData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public ChapterData Data
		{
			get
			{
				return mData;
			}
		}
		
		ChapterData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new ChapterData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
