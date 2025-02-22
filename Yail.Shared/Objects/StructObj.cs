﻿using Yail.Shared.Abstract;
using Yail.Shared.Helpers;

namespace Yail.Shared.Objects;

public class StructObj : ValueObj, IInstantiable
{
    public StructObj()
    {
        Value = new Dictionary<string, ValueObj>();
    }
    
    public required string Name { get; set; }
    public required bool IsPublic { get; set; }

    public string Package => Name.Split("::").First();
    public string StructName => Name.Split("::").Last();

    public void Set(string variableName, ValueObj value)
    {
        var list = Get();
        
        list.TryAdd(variableName, value);

        Value = list;
    }

    public void Update(string variableName, ValueObj valueObj)
    {
        var fields = Get();
        
        if (!fields.TryGetValue(variableName, out var prevValue))
            ExceptionHelper.PrintError($"Variable '{variableName}' is not defined in struct '{Name}'");

        if (!prevValue.ValueEquals(valueObj))
            ExceptionHelper.PrintError($"Data types must be equal.");

        fields[variableName] = valueObj;
    }
    
    public Dictionary<string, ValueObj> Get()
    {
        return Value as Dictionary<string, ValueObj>;
    }

    public ValueObj Get(string propName)
    {
        if (!(Value as Dictionary<string, ValueObj>).TryGetValue(propName, out var val))
        {
            throw new Exception($"Field '{propName}' was not present in '{Name}'");
        }

        return val;
    }
}