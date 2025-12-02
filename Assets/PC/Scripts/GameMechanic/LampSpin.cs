using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LampSpin : MonoBehaviour
{
    public enum SpinAxis
    {
        X,
        Y,
        Z
    }

    [SerializeField] private float speed;
    [SerializeField] private SpinAxis axis;
    [SerializeField] private bool spinDirection;


    void Update()
    {
        float direction = 1f;
        if (!spinDirection)
            direction *= -1f;

        Vector3 rotation = Vector3.zero;

        switch (axis)
        {
            case SpinAxis.X:
                rotation = new Vector3(speed * direction * Time.deltaTime, 0f, 0f);
                break;
            case SpinAxis.Y:
                rotation = new Vector3(0f, speed * direction * Time.deltaTime, 0f);
                break;
            case SpinAxis.Z:
                rotation = new Vector3(0f, 0f, speed * direction * Time.deltaTime);
                break;
        }

        gameObject.transform.Rotate(rotation);
    }
}