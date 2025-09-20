using System;
using Shared.Utility;
using UnityEngine;

[Serializable]
public struct NetworkedObject
{
    private static readonly string sentinel = $"\x8c{nameof(NetworkedObject).GetHashCode():X}\x8c";

    [SerializeField] private int type;
    [SerializeField] private string data;

    public NetworkedObject(string data)
    {
        type = 0;
        this.data = null;

        if (data.Length > sentinel.Length && data.StartsWith(sentinel))
            this = JsonUtility.FromJson<NetworkedObject>(data[sentinel.Length..]);
    }

    private static int GetTypeId<T>()
    {
        return (typeof(T).FullName ?? typeof(T).Name).GetHashCode();
    }

    public bool Is<T>()
    {
        return type == GetTypeId<T>();
    }

    public bool IsValid()
    {
        return type != 0;
    }

    public T GetData<T>()
    {
        TrueDebug.Assert(Is<T>());
        return JsonUtility.FromJson<T>(data);
    }

    public NetworkedObject SetData<T>(T data)
    {
        type = GetTypeId<T>();
        this.data = JsonUtility.ToJson(data);
        return this;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }

    public static string MakePayload<T>(T data)
    {
        return $"{sentinel}{new NetworkedObject().SetData(data)}";
    }
}