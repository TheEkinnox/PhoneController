using System;
using UnityEngine;

[Serializable]
public struct NetworkedObject
{
    [SerializeField] private int _type;
    [SerializeField] private string _data;

    public NetworkedObject(string data)
    {
        _type = 0;
        _data = null;
        this = JsonUtility.FromJson<NetworkedObject>(data);
    }

    public bool Is<T>()
    {
        return _type == (typeof(T).FullName?.GetHashCode() ?? 0);
    }

    public T GetData<T>()
    {
        Debug.Assert(Is<T>());
        return JsonUtility.FromJson<T>(_data);
    }

    public NetworkedObject SetData<T>(T data)
    {
        _type = typeof(T).FullName?.GetHashCode() ?? 0;
        _data = JsonUtility.ToJson(data);
        return this;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }

    public static string MakePayload<T>(T data)
    {
        return new NetworkedObject().SetData(data).ToString();
    }
}