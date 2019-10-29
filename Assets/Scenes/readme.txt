建议使用unity2018.3打开，
打开SampleScene.unity场景
点击运行按钮
点击Find Example
观察结果


1 作业需要完善以下函数的实现
	c#（函数在PathFind.cs line76）
		public bool Find(Vector2 begin, Vector2 end, List<Vector2> lstPath)
	c++（函数在PathFindCplusplus.cpp line54）
		__declspec(dllexport) bool __cdecl FindCPP(Vector2& start, Vector2& end, Vector2* outPath,int* pathCount)

2 TestFind只是演示用的函数，内部并没有实现完整的搜索算法，
3 DrawButton 显示按钮
4 DrawDebug 显示地图和路径
5 isWalkable 判断某个点是否可以行走
6 LoadData 加载寻路地图（一张tif图片）

备注
使用c++的同学也需要安装unity2018.3，
打开PathFindCplusplus.sln，修改代码之后编译生成X64版本（debug和release均可） 代码会自动拷贝到unity目录下，
运行SampleScene.unity 点击FindCPP 即可执行C++版本代码


