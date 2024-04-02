using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class System05 : SystemBase
{
    protected override void OnUpdate()
    {
        foreach (var (a, b) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotateComponentData>>())
        //foreach (var a in SystemAPI.Query<RefRW<LocalTransform>>())
        {
            Debug.Log(a.ValueRO.Position);
            Debug.Log(b.ValueRO.rotateSpeed);
        }
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        Debug.Log("OnCreate");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Debug.Log("OnDestroy");
    }
}
