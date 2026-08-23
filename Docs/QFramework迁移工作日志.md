# QFramework 架构迁移工作日志

> 记录本项目从手写 `Singleton<T>`/`MonoSingleton<T>` + 一堆独立 `XxxManager` 的架构，
> 迁移到 QFramework（核心 Architecture + ResKit/UIKit/AudioKit/PoolKit/FSMKit 全套 Toolkits）
> 的完整过程。按执行顺序分阶段记录，每阶段都保证项目可编译。
>
> 完整的原始迁移方案见 `Docs/`（本文件）配套的架构说明 `项目架构说明.md`；
> 更细的分阶段设计笔记保留在各源文件顶部的 XML 注释里，本日志做汇总梳理。

## 背景与目标

项目原有架构：`Singleton<T>`/`MonoSingleton<T>` 单例基类 + 一堆独立的 `XxxManager`
（Event/Mono/Pool/Resource/UI/Audio/Scenes/Input/Game），外加两套手写有限状态机
（Player 的 `PlayerFSM`+`IState`、Enemy 的 `EnemyFSM` 及其子类）。问题：
- 状态类直接调用 `AudioManager.Instance`、`EventManager.Instance` 等，层次耦合严重；
- 没有统一的数据/命令/事件规范；
- UI 面板绑定靠反射查找子物体名字；
- 资源加载是裸 `Resources.Load`；
- 可维护性、可测试性弱。

目标（用户明确要求，两轮确认后拍板）：
1. 框架选型：**QFramework**（liangxiegame）。
2. 范围：**全部核心系统**（Event/Resource/UI/Scene/Audio/Pool/Input 等 manager + Player/Enemy 状态机）。
3. 深度：**全套 Toolkits**（ResKit + UIKit + AudioKit + PoolKit + FSMKit），而非只用核心架构包一层皮。

约束（贯穿全程）：
- 每个阶段结束都必须保持项目可编译；
- `InputManager`、`GameManager` 必须保留为 `MonoBehaviour`（`MainMenu.unity` 里手工挂载并配置了序列化引用，不能改成纯 C# 类）；
- 遇到框架硬性限制或"方案与现实冲突"时停下来找用户确认，而不是默默扩大/缩小范围；
- 因采用框架语义而产生的行为变化（即使很小）必须在报告里披露，不能悄悄改掉。

## Phase 0：铺底

- 用户手动把 `QFramework.unitypackage`（核心框架 + Toolkits）导入到 `Assets/QFramework`
  （此步骤需要 Unity Editor 图形界面操作 `.unitypackage` 导入，无法用命令行完成，由用户本地完成）。
- 创建 `GameArchitecture : Architecture<GameArchitecture>` 空壳（`Assets/Scripts/Architecture/GameArchitecture.cs`），
  作为全项目唯一的架构根节点，接入空场景验证编译通过。

## Phase 1：基础设施（Event / Mono 驱动 / Pool / Resource）

被其余系统依赖的最基础模块先行替换：

- **Event**：`EventManager`（字符串键事件）→ QFramework 内置 `TypeEventSystem`。
  字符串事件名改成强类型事件结构体，定义在 `Assets/Scripts/Architecture/Events/GameEvents.cs`：
  `PausedEvent`/`ContinueEvent`/`PlayerDeadEvent`/`GruzWakeUpEvent`/`GruzDeadEvent`/`GruzAllDeadEvent`/`GruzBurstEvent`。
  发送用 `this.SendEvent<T>()`，监听用 `this.RegisterEvent<T>(...).UnRegisterWhenGameObjectDestroyed(gameObject)`。
  旧版 `"LoadProgress"`/`"LoadSceneProgress"` 字符串事件确认全项目无任何监听者，属于死代码，未迁移。
- **Mono 驱动**：`MonoManager`（全局 Update/协程分发）→ `IMonoUtility`/`MonoUtility`
  （`Assets/Scripts/Architecture/Utility/MonoUtility.cs`），内部仍复用原 `MonoController`
  组件（`Assets/Scripts/Architecture/Utility/MonoController.cs`）挂在常驻 GameObject 上驱动，
  对外通过 `this.GetUtility<IMonoUtility>()` 使用。
- **Pool**：`PoolManager`/`PoolData` → `IObjectPoolSystem`/`ObjectPoolSystem`
  （`Assets/Scripts/Architecture/System/ObjectPoolSystem.cs`）。技术决策：QFramework PoolKit 的
  `SafeObjectPool<T>` 是"每个类型 T 一个单例池"，不适合"按 prefab 路径动态开池"的场景，
  因此保留原来按路径分池的 `Queue<GameObject>` 结构（语义与旧 `PoolData` 完全一致），
  只是把生命周期从 `MonoSingleton` 迁移到 `AbstractSystem`，资源加载改走新的 `IResSystem`。
  对外方法名 `Get`/`Push`/`ClearPool` 刻意保持不变。
- **Resource**：`ResourceManager` → `IResSystem`/`ResSystem`（`Assets/Scripts/Architecture/System/ResSystem.cs`），
  底层用 ResKit 的 `ResLoader.Allocate()` + `LoadSync<T>`/`Add2Load<T>`。
  **关键实现决策**：为了规避 ResKit/UIKit 官方默认工作流要求的 AssetBundle 标记
  （需要把 `Resources/` 下大量资源重新组织并打 AB 标记，风险和工作量都很高），
  所有资源路径统一在内部自动加上 ResKit 支持的 `"resources://"` 前缀，直接命中
  `ResourcesResCreator`，资源目录结构、打包方式完全不变。`Load<T>`/`LoadAsync<T>`
  的调用方式和旧 `ResourceManager.Load`/`LoadAsync` 保持一致（`T` 为 `GameObject` 时自动
  `Instantiate`）。ResKit 的 Simulation Mode（开发期模拟资源加载）默认开启，作为兜底不需要额外配置。

## Phase 2：横向系统（Audio / Scene / Input / GameState）

- **Audio**：`AudioManager` 内部实现改为混合方案（**已与用户确认，非遗留降级**）：
  - 按名字播放的全局/2D 一次性音效与背景音乐 → 迁移到 AudioKit（`AudioKit.PlayMusic`/`PlaySound`/
    `PauseMusic`/`ResumeMusic`/`StopMusic`），因为这部分本来不需要 3D 定位，也不依赖 Mixer 分组特殊处理；
  - 敌人身上的 3D 定位音效（`AudioController.cs`，`spatialBlend=1`）以及基于 `AudioMixer`
    Group（`"Global"`/`"Music"`/`"Sound"`）做音量控制、直接操作 Inspector 已配置好的
    `AudioSource`（`PlayAudio`/`PauseSound`/`UnPauseSound`/`StopSound`）这几类功能 → 继续保留手写实现，
    不迁移到 AudioKit，因为 AudioKit 内部 `AudioSourceProxy`/`AudioSource` 均为 `internal`，
    无法暴露 3D 定位参数，也没有路由到指定 `AudioMixer Group` 的钩子；
  - `SetSoundVolume` 同时驱动旧 `AudioMixer` `"Sound"` 分组音量与 `AudioKit.Settings.SoundVolume`，
    保证同一个音量滑条对两套播放路径都生效。
  - 对外方法名（`PlayBackgroundMusic`/`PlaySound`/`PlaySoundWithComplete`/... ）全部保持不变。
- **Scene**：`ScenesManager` → `ISceneSystem`/`SceneSystem`（`Assets/Scripts/Architecture/System/SceneSystem.cs`）。
  调研发现旧 `ScenesManager` 全项目无任何调用方、也未被任何 UnityEvent 引用，属于"预留但从未接入"
  的模块，因此直接迁移为不挂节点的 `AbstractSystem`（不像 `InputManager`/`GameManager` 那样保留
  `MonoBehaviour` 形态）。`LoadSceneAsync` 内部通过 `this.GetUtility<IMonoUtility>().StartCoroutine(...)`
  驱动协程。旧版逐帧广播加载进度的行为（`LoadSceneProgress` 事件）确认无监听者，未复刻。
- **Input**：`InputManager` 保留 `MonoBehaviour` 形态（场景手工挂载约束），去掉
  `MonoSingleton<InputManager>`，改为实现 `IController` + 新增的 `IInputUtility`
  （`Assets/Scripts/Input/IInputUtility.cs`），在自己的 `Awake()` 里
  `this.GetArchitecture().RegisterUtility<IInputUtility>(this);` 完成自注册。
  其余脚本通过 `this.GetUtility<IInputUtility>()` 取用 `attack`/`jump`/`pause`/`back` 等
  `InputAction`，替代原来的 `InputManager.Instance`。
- **GameState**：`GameManager` 内部状态改成 `IGameStateModel`/`GameStateModel`
  （`Assets/Scripts/Architecture/Model/GameStateModel.cs`，`BindableProperty<GameState>`），
  暂停/继续逻辑改成 `PauseGameCommand`/`ResumeGameCommand`
  （`Assets/Scripts/Architecture/Command/GameStateCommands.cs`）。`GameManager.gameState`
  属性直接代理到 `IGameStateModel.State.Value`，外部调用方式不变。

## Phase 3：UI

- `BasePanel` 改为继承 QFramework UIKit 的 `UIPanel` 基类，获得 `Init`/`Open`/`Show`/`Hide`
  标准生命周期，`OnShow`/`OnHide` 钩子转发到原有的 `ShowMe()`/`HideMe()` 虚方法，
  子类（`MainMenuPanel`/`PausePanel`/`OptionMenuPanel`/`VolumeMenuPanel`/`BossPanel`/
  `KnightPanel`/`OpeningPanel`/`ScreenMask` 等）零改动。
- **技术决策**：UIKit 自带的 `UIKit.OpenPanel`/`UIRoot`/UIKit 层级模型（`UILevel` 只有 3 层）
  与项目现有的 4 层（`Bot`/`Mid`/`Top`/`System`）+ 自定义面板栈（"返回上一个面板"）模型不兼容，
  且需要一个项目里没有的 `UIRoot` 预制体，强行套用收益低、风险高。因此**只借用 UIKit 的
  `UIPanel` 基类**，继续沿用项目自己的 `UIManager` 做加载/挂载/销毁/层级/回退栈管理。
  `UIManager` 本身在 Phase 6 之前仍然是 `MonoSingleton<UIManager>`。
- `UIManager` 对外接口保持 `ShowPanel<T>`/`HidePanel`/`ClosePanel`/`BackToLast`/`ClearPanel`/
  `GetPanel<T>`/`GetLayer` 不变。

## Phase 4：Player 状态机

- `IState` 接口 + Player 各状态类（`Assets/Scripts/Player/Test/States.cs`）迁移到 FSMKit：
  逐个状态类改写为 `AbstractState<States, PlayerController>`，构造函数签名统一为
  `(FSM<States> fsm, PlayerController target)`，原来的方法体搬进
  `OnCondition`/`OnEnter`/`OnUpdate`/`OnFixedUpdate`/`OnExit` 对应覆写方法，
  `manager.TransitionState(x)` 换成 `FSM.ChangeState(x)`。
- `PlayerFSM` 类改名为 `PlayerController : MonoBehaviour, IController`
  （文件仍是 `Player/Test/PlayerFSM.cs`，类名重命名对场景引用无影响，因为 Unity 按脚本 GUID
  而非类名解析组件引用，且该文件是文件内唯一的 MonoBehaviour 类）。
  持有 `FSM<States> FSM`，驱动 Move/SlashAndDetect/Grounding/Dead 等生命周期相关逻辑。
- 需要用户在 Editor 里实际 Play 测试手感（迁移过程本身无法跑 Play Mode）。

## Phase 5：Enemy 状态机

- `EnemyFSM` 基类改名为 `EnemyController : MonoBehaviour, IController`
  （文件仍是 `Enemy/EnemyFSM.cs`，重命名安全性理由同 Phase 4：无场景/预制体直接以
  `EnemyFSM` 类型序列化组件，挂在 GameObject 上的始终是具体子类）。
  `TransitionState`/`Hurt`/`SpawnCoins` 等公开 API 保持不变，子类
  （`Crawler`/`Gruz`/`Vengefly`/`GruzMother`）无需感知内部实现变化。
  各子类的状态类同样迁移为 FSMKit 的 `AbstractState<States, EnemyController>` 模式。
- **本阶段修复的时序 bug**（对 Phase 6 的设计有直接指导意义）：发现并修复了一处
  `AnimationController` 空引用问题，根因是跨组件依赖被放在了 `Awake()` 里，
  而 Unity 只保证"全场景 `Awake()` 完成"发生在"任何 `Start()` 之前"，不保证
  `Awake()` 之间的相对顺序。修复方式是把跨组件/跨系统依赖延后到 `Start()`
  或更晚（事件回调、协程）执行。这条时序安全规则在 Phase 6 设计三个 Manager
  改造方案时被重新应用（见下）。

## Phase 6：收尾——彻底改造 UIManager / AudioManager / GameManager

### 调研结论

对照模块映射表逐项核对现状后发现：
- `Assets/Scripts/Singleton/BaseManager.cs`（`BaseManager<T>`，非 Mono 单例）：全项目 0 处引用，纯死代码。
- `Event`/`Mono`/`Pool`/`Res`/`Scenes` 五个目录：Phase 1/2 迁移后已清空，只剩空文件夹 + `.meta`，纯遗留残留。
- `Assets/Scripts/Singleton/MonoSingleton.cs`：**并未废弃**——`UIManager`、`AudioManager`、
  `GameManager` 三个类当时仍然 `: MonoSingleton<T>`，全项目有 **74 处** `XxxManager.Instance.xxx()`
  调用依赖这个模式。
- 场景挂载核实（按 script GUID 核对 `MainMenu.unity`）：只有 `InputManager`、`GameManager`
  是手工挂在场景对象上的；`AudioManager`、`UIManager` 没有出现在任何场景/prefab 里，
  完全依赖 `MonoSingleton.Instance` 的"首次访问时自动创建 GameObject"机制。
- 74 处 `.Instance` 调用点全部发生在 `Start()`、事件回调（按钮点击、`.performed +=`、
  `OnTriggerEnter2D`）或协程回调里，**没有一处发生在 `Awake()`**——这一点决定了改造在时序上是安全的。

用户就 Phase 6 范围做出选择：从"简单删除死代码"升级为**"彻底改造三个 Manager"**——
完全去掉 `MonoSingleton<T>`，74 处调用点全部改为 `GetUtility<T>()` 模式。

### 实施内容

1. **删除死代码**：`Assets/Scripts/Singleton/BaseManager.cs`，以及已清空的 `Event`/`Mono`/
   `Pool`/`Res`/`Scenes` 空文件夹。
2. **`AudioManager`**：新增 `IAudioUtility` 接口（16 个方法，签名与原 public 方法一致），
   类改为 `AudioManager : MonoBehaviour, IController, IAudioUtility`。
3. **`UIManager`**：新增 `IUIUtility` 接口（`GetLayer`/`ShowPanel<T>`/`HidePanel`/`ClosePanel`/
   `BackToLast`/`ClearPanel`/`GetPanel<T>`），类改为
   `UIManager : MonoBehaviour, IController, IUIUtility`；静态方法 `AddCustomEventListener`
   不经过 `.Instance`，原样保留。
4. **`GameManager`**：新增 `IGameUtility` 接口（`GameState gameState { get; set; }`），
   类改为 `GameManager : MonoBehaviour, IController, IGameUtility`，新增 `Awake()`
   自注册：`this.GetArchitecture().RegisterUtility<IGameUtility>(this);`（原来靠
   `MonoSingleton.Awake()` 做的 `DontDestroyOnLoad` 也一并挪到这里显式调用）。
5. **创建/注册方式**：
   - `GameManager` 本来就是场景手工挂载对象，用法与 `InputManager` 同款（自注册）。
   - `UIManager`/`AudioManager` 原来靠 `MonoSingleton` 懒创建，改造后在
     `GameArchitecture.Init()` 里主动创建（写法参考已有的 `MonoUtility` 模式）：
     ```csharp
     var uiManagerGo = new GameObject("[UIManager]");
     Object.DontDestroyOnLoad(uiManagerGo);
     this.RegisterUtility<IUIUtility>(uiManagerGo.AddComponent<UIManager>());
     ```
     且必须放在 `this.RegisterSystem<IResSystem>(new ResSystem());` **之后**，
     因为两者的 `Awake()` 会同步调用 `this.GetSystem<IResSystem>()` 加载 Canvas/Cursor/AudioMixer，
     `AddComponent<T>()` 会立即同步触发该组件的 `Awake()`。
   - **行为变化（已披露）**：`UIManager`/`AudioManager` 的创建时机从"第一次被谁访问
     `.Instance` 时"变成"`GameArchitecture` 第一次被访问时立刻创建"。实际发生时间点基本不变
     （游戏刚启动、`InputManager.Awake()` 就会触发 `GameArchitecture.Interface` 首次访问），
     没有可感知的行为差异，但严格意义上是个时机变化。
6. **补充 `IController`** 到之前不是 `IController`、但调用了 `.Instance` 的类：
   `UI/BasePanel.cs`（连带其所有子类）、`Other/Cave.cs`、`Other/MainMenu.cs`、
   `EventTrigger/Room.cs`、`EventTrigger/GameOver.cs`、`EventTrigger/FirstLand.cs`、
   `UI/ButtonController.cs`、`Props/Breakable.cs`（连带子类 `BreakableWall`）。
   **过程中发现并修正的一处计划假设错误**：`Enemy/EnemyFSM.cs` 里的 `EnemyController`
   基类当时实际上还**不是** `IController`（此前的记录误以为它已经是），必须补上
   `IController` + `GetArchitecture()`，否则其子类 `Gruz`/`Vengefly`/`GruzMother` 里新写的
   `GetUtility<IAudioUtility>()` 调用无法通过编译。这是保证既有改动可编译的必要修正，不属于新范围决策。
7. **转换全部 74 处 `.Instance` 调用点**为 `this.GetUtility<IXxxUtility>().Method(...)`
   （主类方法体内）或 `manager.GetUtility<IXxxUtility>().Method(...)`（FSM 嵌套状态类内，
   `manager` 是已有字段），涉及约 23 个文件，包括 `UI/*Panel.cs`、`EventTrigger/*.cs`、
   `Enemy/Gruz.cs`/`Vengefly.cs`/`GruzMother.cs`（含其嵌套状态类）、
   `Player/Test/States.cs`（11 处，FSM 状态类）、`Player/Test/PlayerFSM.cs`、
   `Input/InputManager.cs`（`GameManager.Instance.gameState` 一处）等。
8. **删除 `Assets/Scripts/Singleton/MonoSingleton.cs`**（及其 `.meta`），随后确认
   `Assets/Scripts/Singleton/` 目录已空，一并删除该文件夹。
9. 全局 grep 确认 `.Instance`、`MonoSingleton`、`BaseManager` 均无残留引用
   （QFramework 自带的 `SingletonKit` 目录不算，那是框架自身代码）。

### 编译验证过程中的插曲（记录下来避免重蹈）

补齐 `using QFramework;` 时遗漏了 4 个文件（`OptionMenuPanel.cs`/`VolumeMenuPanel.cs`/
`MainMenuPanel.cs`/`BreakableWall.cs`），导致 Unity Editor.log 里出现 `CS1061`
"does not contain a definition for 'GetUtility'"报错；随后又发现同一批日志里混有更早一次
编译（`EnemyController` 补 `IController`之前）遗留的 `CS1929` 报错块，一度难以判断
"日志里的报错是不是最新一次编译的结果"。最终通过：
1. 直接读取源文件确认当前磁盘状态已经修复；
2. 用 Win32 API（`SetForegroundWindow`）把 Unity Editor 窗口拉到前台，强制触发一次
   `Reloading assemblies after forced synchronous recompile` 的同步重编译；
3. 用 `Get-Content -Skip N` 只看**新增的日志行**（而不是笼统的 `-Tail N`，避免和历史报错混在一起）；

确认最终这批新增日志里 `LogAssemblyErrors (0ms)`、零个 `error CS`，只有几条无害的
`CS0108`（方法遮蔽提示）/`CS0114`/`CS0067` warning，编译通过。

### 结果

- 全项目 74 处 `.Instance` 调用点清零，`MonoSingleton<T>` 类本身及其所在的 `Singleton/`
  文件夹已删除。
- `UIManager`、`AudioManager`、`GameManager` 与 `InputManager` 现在采用完全一致的
  "`MonoBehaviour + IController + IXxxUtility`，通过 `GetArchitecture().RegisterUtility<T>(this)`
  自注册或由 `GameArchitecture.Init()` 主动创建注册"模式，架构风格统一。

### 待用户验证事项

- Play 测试：主菜单音乐/UI 面板切换、暂停菜单（ESC）、音量滑条（Option/Volume 面板）、
  Boss 战面板触发、Room 场景"发现密室"音效——这些正是 74 处调用点覆盖到的功能面。
- **仍未闭环的旧问题**：Phase 5 修复的 FSM/`AnimationController` 空引用 bug，是否真正解决了
  用户最初反馈的"运行后全面报错"问题，用户尚未明确确认过（当时直接说"进行下一阶段"进入了
  Phase 6）。建议本次 Play 测试时一并确认。

## 后续可选事项（未在本次范围内，仅记录）

- `Geo.cs`/`Health.cs`/`GeoCollect.cs` 等道具脚本本次未改动（它们已经比较独立，未强依赖
  被替换的 manager API），如后续发现引用了已删除的 API 需要单独处理。
- `AudioManager` 里手写的 3D 定位音效/`AudioMixer` 分组路径，如果未来需要完全统一到 AudioKit，
  需要先解决 AudioKit 内部 `AudioSource`/`AudioSourceProxy` 为 `internal` 无法路由到自定义
  `AudioMixer Group`/暴露 `spatialBlend` 的限制（可能需要自定义 `IAudioLoader` 或等 AudioKit
  后续版本开放相关 API）。
