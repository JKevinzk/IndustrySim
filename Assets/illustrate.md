# 规则
## 网格系统  
factory IO：   2m-1大格-16小格 0.125m/小格  
industrySim：                 0.5m/小格

# 代码说明
## 物体创建
### 相关组件
#### SelectImage
| 函数 | 说明                      |
|----|-------------------------|
|Start| 对对象进行实例化并隐藏             |
|OnPointerDown| 将对象从面板中拖动出来，并传递到对象管理器（`SelectObjManager`）中。|

#### SelectObjManager
单例模式，对象全局管理器

| 函数  | 说明                              |
|--------|---------------------------------|
| Update | 每帧执行物体拖动动画                      |
|MoveCurrentPlaceObj| 让当前对象跟随鼠标移动                     |
|CreatePlaceObj| 在指定位置实例化一个对象                    |
|CheckIfPlaceSuccess| 检测是否放置成功                        |
|AttachNewObject| 将要创建的对象传递给`currentPlaceObj`公有属性 |


1. `ComponentPanel`面板中，每个窗格会挂载`SelectImage`组件。
2. `SelectImage`组件，对所有挂载了该组件的物体，进行实例化并隐藏。


对于创建物体，全部都在componentFactory里完成，通过调用ICompoonentModel里的creat完成创建，调用AddComponent完成组件添加