using UnityEngine;

public class ServerGyroscopeRotation : NetworkedBehaviour
{
    //NEEDED: convert with the data you want
    private Transform _target;

    private void Awake()
    {
        _target = transform;
    }

    protected override void HandleMessage(NetworkedObject data) // Where the magic happens
    {
        if (!_target)
            return;

        if (!data.Is<GyroData>()) //NEEDED: Data receiver
            return;

        GameManager.Instance.phoneRotation = data.GetData<GyroData>().value;
    }
}