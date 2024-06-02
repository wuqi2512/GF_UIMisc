# 简介
1. GameFramework框架中的UI模块
2. 安全区适配、黑边适配，以及一些其他
3. [UI 自动绑定](https://github.com/CatImmortal/ComponentAutoBindTool)
4. 2048 demo

# UI 模块
UIManager:
- 使用ObjectPool，管理所有ui实例
	- Spawn，Unspawn，Register
	- API: SetLock, SetPriority
- 使用UIGroup，管理ui的层级
	- API: HasUIGrop, AddUIGroup, GetUIGroup
- Open/Close/RefocusUIForm
	- 异步加载
		- 缓存正在加载的UI
		- 关闭正在加载的UI
	- 分配并维护UIForm的SerialId
	- 发送相关事件，OpenUISuccess, OpenUIFailure, CloseUIComplete等
	- Helper, Instantiate/Create/ReleaseUIForm
- OnUpdate：轮询更新UIGroup

UIGroup:
- ui 栈, 维护 UI 的层级和生命周期
- 在内部使用OpenUIInfo记录UIForm的covered和paused状态
- UIGroup的Depth和Pause状态
- OnUpdate：轮询更新 UIForm
- Refresh：更新组内所有 UI 的深度和暂停状态，并调用 UIForm 的一些生命周期函数（调用过程中可能会改变状态，如 pause、增删 UIForm，要注意调用前缓存好状态）
- Add/Remove/Refocus/HasUIForm，SetDepth

UIForm:
- SerialId, AssetName, UIGroup, DepthInGroup, PausedCoveredUIForm, UIFormLogic
	- 如果是创建的实例，调用GetComponent获取UIFormLogic
	- 除了UIFormLogic，其余都是初始化时传入的参数
- 调用UIFormLogic对应生命周期函数

UIFormLogic:
- 生命周期: Init, Recycle, Open, Close, Pause, Resume, Cover, Reveal, Refocus, Update, DepthChanged
- Available, Visible, UIForm, OriginLayer
	- UGuiForm
		- CanvasGroup