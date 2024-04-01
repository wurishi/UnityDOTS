Platform:

- Unity 2023.1.6
- Entities 1.0.16
- 文档：https://docs.unity3d.com/Packages/com.unity.entities@1.0/manual/index.html

# 1. 准备

## 1.1 环境

1. 创建基于可编程渲染管线（SRP）的项目，如 URP（轻量级渲染管线），HDRP（高清渲染管线）。
2. 需要安装的额外包：
   - com.unity.entities: ECS 相关的工具包，包含了 Entity, ECS, Job System, Burst, Mathematics。
   - com.unity.enities.graph: 渲染 Entity 需要的包。
3. Packages / Entities / Documentation~ 下有对应的文档。
4. 常用包：
   - Unity Physics: ECS 下的物理引擎。
5. Edit -> Project Settings -> Editor
   - Enter Play Mode Options -> On
   - Reload Domain -> Off
   - Reload Scene -> Off
6. df

## 1.2 注解与装饰器

```csharp
class EntityObject01 : System.Attribute
{
    public EntityObject01(string name) { }
}
```

所有装饰器类需要继承 `System.Attribute`。

```csharp
[EntityObject01("MyEntity")] // 实例化了一个 EntityObject01
class GameEntity01 // 将注解实例的对象记录到 GameEntity01 对应的 Type 的 Attribute 下
```

## 1.3 托管对象与非托管对象

托管对象：垃圾回收器直接会回收的对象。

非托管对象：垃圾回收器不管，需要自己释放。

unsafe code: 通过 Edit -> Project Settings -> Player -> Other Settings -> Script Compilation -> Allow 'unsafe' Code 打开。

# 2. DOTS 的核心机制与概述

## 2.1 ECS 概述

1. ECS:

   - Entity: 实体
   - Component: 组件
   - System: 系统，算法

2. 面向数据编程：D(Data) O(Oriented) T(Technology) S(Stack)

3. ECS 代码的组织方式：

   - Entity: 包含的多个 Component 会组合在一起，使的它是连续的内存布局。EntityManager 负责高效的分配与释放相关的 Entity。
   - World: 包含所有的 Entity，每个 Entity 都有唯一不同的 entityid。
   - 每个 Entity 拥有多个 Component，它会转换为 ArcheType，来决定这些 Component 的内存排布。

4. Unity 会创建一个默认的 World，会把所有需要的 System 加入到这个 World 中。

   可以通过相关的宏来手动控制创建 World 的时机。

   - `#UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP_RUNTIME_WORLD`: Disables generation of the default runtime world.
   - `#UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP_EDITOR_WORLD`: Disables generation of the default Editor world.
   - `#UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP`: Disables generation of both default worlds.

5. Entity 包含的方法：

   - `CreateEntity`
   - `Instantiate`
   - `DestroyEntity`
   - `AddComponent`
   - `RemoveComponent`
   - `GetComponent`
   - `SetComponent`

6. Archetype 指的是由一个世界中，不同标识的实体，它们拥有相同组件组合的类型。

7. Archetype Chunk 每个 Chunk 为 16KiB 大小。

## 2.2 Job System

Job System 可以由多线程来驱动。

## 2.3 EntityQuery

用来查询组件数据

## 2.4 Burst 工具链

传统 Unity: .net代码 -> il2cpp -> cpp。

Burst: .net 代码 LLVM -> native code，使用了 ”单指令多数据集“ MMX。

# 3. DOTS 的 SubScene

## 3.1 SubScene 机制

SubScene 用来存放 ECS 模式下的内容，用来代替传统的 Scene。

可以在 SubScene 中创建 GameObject 和 MonoBehaviour，在烘培（baking）阶段会将它们转换成 ECS 的实体和组件。

## 3.2 如何创建 SubScene

1. **创建空的 SubScene**：在场景的 Hierarchy 右键 -> New Sub Scene -> Empty Scene。
2. **连带物体一起创建 SubScene**：选中想要放到 SubScene 的物体，在 Hierarchy 右键 -> New Sub Scene -> From Selection。
3. **添加已经存在的 SubScene**：在空的 GameObject 上添加组件 SubScene，并在 SubScene -> Scene Asset 里指定已经有 SubScene。

## 3.3 DOTS 开发常用的窗口

Window / Entities 

# 4. Component 概述与添加自定义 Component

## 4.1 Component 的作用与概述

1. Component 是用来存放 Entity 的数据，并让 System 读与写。
2. 组件是一个实现了 IComponentData 的 class 或 struct，struct 只能包含非托管的数据类型。要包含托管数据，就必须为 class。

## 4.2 Component 的几种类型

| Component             |            | 描述                                                         |
| --------------------- | ---------- | ------------------------------------------------------------ |
| Unmanaged components  | 非托管组件 | 最常见的组件类型，只能保存特定的某些类型的字段。             |
| Managed components    | 托管组件   | 可以保存任意类型字段。                                       |
| Shared components     | 共享组件   | 一组 entity 共用数据。                                       |
| Cleanup components    |            | 当销毁的 entity 包含了 Cleanup 组件，Unity 会删除所有非 Cleanup 组件。它常用来标识 entities 销毁时需要清理操作。 |
| Tag components        |            | 该组件不保存数据也不占用内存，方便 entity queries 过滤。     |
| Buffer components     |            | 可缩放的数组                                                 |
| Chunk components      |            |                                                              |
| Enableable components |            | 在运行时可以设置 entity 可用/禁用                            |
| Singleton components  |            | 一个世界中只有一个                                           |

## 4.3 向 SubScene 中的 GameObject 添加自定义的 ComponentData

由 Authoring 将普通的数据（MonoBehaviour 中的数据）烘培（Bake）到 Entity 中。

```csharp
public class RotateComponentAuthoring : MonoBehaviour
{
    public float speed;
}
struct RotateComponentData : IComponentData
{
    public float rotateSpeed;
}
public class Baker : Baker<RotateComponentAuthoring>
{
    
    public override void Bake(RotateComponentAuthoring authoring)
    {
        var entity = this.GetEntity(TransformUsageFlags.Dynamic);
        var data = new RotateComponentData
        {
            rotateSpeed = math.radians(authoring.speed),
        };
        AddComponent(entity, data);
    }
}
```



