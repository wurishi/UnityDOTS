using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

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
        Debug.Log("∑¢…˙‘⁄±‡“Î");
        var entity = this.GetEntity(TransformUsageFlags.Dynamic);

        var data = new RotateComponentData
        {
            rotateSpeed = math.radians(authoring.speed),
        };

        AddComponent(entity, data);
    }
}