using System.Threading;
using Cysharp.Threading.Tasks;
using QAssetBundle;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.Video;

namespace QFramework.Example
{
	public class ChapterData : UIPanelData
	{
	}
	public partial class Chapter : UIPanel, IController
	{
		public Animator anim;
		public VideoPlayer prologue;
		public VideoPlayer intro;

		private VideoPlayer nowVideo;
		private ResLoader  resLoader;
		private CancellationTokenSource tokenSource;
		public IArchitecture GetArchitecture()
		{
			return GameArchitecture.Interface;
		}
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as ChapterData ?? new ChapterData();
			resLoader = ResLoader.Allocate();
			anim = GetComponent<Animator>();
			tokenSource = new CancellationTokenSource();

			resLoader.Add2Load<VideoClip>(Prologue_webm.BundleName, Prologue_webm.PROLOGUE, (success,res) =>
			{
				if (success)
				{
					prologue = CreateVideoPlayer(Prologue_webm.PROLOGUE, res.Asset as VideoClip);
				}
			});
			resLoader.Add2Load<VideoClip>(Intro_webm.BundleName, Intro_webm.INTRO, (success,res) =>
			{
				if (success)
				{
					intro = CreateVideoPlayer(Intro_webm.INTRO, res.Asset as VideoClip);
				}
			});
			resLoader.LoadAsync();
		}

		private VideoPlayer CreateVideoPlayer(string name, VideoClip clip)
		{
			var player = new GameObject(name).AddComponent<VideoPlayer>();
			player.playOnAwake = false;
			player.clip = clip;
			player.renderMode = VideoRenderMode.CameraFarPlane;
			player.targetCamera = Camera.main;
			return player;
		}

		protected override void OnOpen(IUIData uiData = null)
		{
			Play(tokenSource.Token).Forget();
		}
		
		protected override void OnShow()
		{
			this.GetModel<IRunTimeDataModel>().GameStatus.Register(StateChanged);
			this.GetModel<IRunTimeDataModel>().WantoSpace.Register(NextScene);
		}
		
		protected override void OnHide()
		{
			this.GetModel<IRunTimeDataModel>().GameStatus.UnRegister(StateChanged);
			this.GetModel<IRunTimeDataModel>().WantoSpace.UnRegister(NextScene);
		}
		
		protected override void OnClose()
		{
			tokenSource.Cancel();
			resLoader.Recycle2Cache();
			resLoader = null;
			if (prologue != null && prologue.targetTexture != null) prologue.targetTexture.Release();
			if (intro != null && intro.targetTexture != null) intro.targetTexture.Release();
		}
		
		public async UniTask Play(CancellationToken token)
		{
			await anim.WaitAnimationEnd("FadeIn", 0, token);

			nowVideo = prologue;
			prologue.Play();
			await WaitVideoEnd(prologue, token);

			nowVideo = intro;
			intro.Play();
			await WaitVideoEnd(intro, token);
			
			NextScene(true);
		}

		private static async UniTask WaitVideoEnd(VideoPlayer player, CancellationToken token)
		{
			// 不用 loopPointReached：它在这个项目里没有按预期触发。
			// 注意 Pause() 也会让 isPlaying 变 false（此时 isPaused 为 true），
			// 所以必须同时排除 isPaused，否则暂停（比如按 Esc 打开 PausePanel）会被误判成"播放完毕"。
			await UniTask.WaitUntil(() => player.isPlaying, cancellationToken: token);
			await UniTask.WaitUntil(() => (ulong)player.frame >= player.frameCount - 1, cancellationToken: token);
			player.Stop();
		}

		public void NextScene(bool value)
		{
			if (value)
			{
				nowVideo = null;
				resLoader.LoadSceneSync("SampleScene");
				CloseSelf();
			}
		}
		
		private void StateChanged(GameState state)
		{
			if (state == GameState.Paused)
			{
				nowVideo?.Pause();
			}
			else if (state == GameState.Playing)
			{
				nowVideo?.Play();
			}
			else if (state == GameState.Menu)
			{
				nowVideo?.Stop();
			}
		}

	}
}
