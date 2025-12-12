using UnityEngine;

public class RotationAlongPhone : MonoBehaviour
{
    void Update()
    {
        transform.rotation = GameManager.Instance.phoneRotation;
    }
}
