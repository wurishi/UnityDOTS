using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

class EntityObject01 : Attribute
{
    public EntityObject01(string name = "Hello") {
        Debug.Log("Create EntityObject01:" + name);
    }
}

class NewEntityObject01 : Attribute
{
    public NewEntityObject01(Type t)
    {
        Activator.CreateInstance(t);
    }
}

[EntityObject01("MyEntity")]
[NewEntityObject01(typeof(EntityObject01))]
class GameEntity01
{
}

public class Script01 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach(var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].GetCustomAttribute<EntityObject01>() != null)
                {
                    Debug.Log(types[i].Name);
                }
            }
        }
        new GameEntity01();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
