using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Puzzle01 : MonoBehaviour
{
    
    [SerializeField] private Transform objectA;
    [SerializeField] private Transform objectB;
    
    void Update()
    {
        Vector3 direction = objectB.position - objectA.position;
        float distance = direction.magnitude;
        Vector3 dirNormalized = direction.normalized;
        bool isCharging = false;

        if (Physics.Raycast(objectA.position, dirNormalized, out RaycastHit hit, distance))
        {
            if (hit.collider.CompareTag("SolarPanel"))
                isCharging = true;
        }

        if (!GameManager.Instance.powerAlreadyTriggered)
        {
            if (isCharging)
                GameManager.Instance.currentChargeTime += Time.deltaTime;
            else
                GameManager.Instance.currentChargeTime -= Time.deltaTime;

            GameManager.Instance.currentChargeTime = Mathf.Clamp(GameManager.Instance.currentChargeTime, 0f, GameManager.Instance.chargeTime
            );

            if (GameManager.Instance.currentChargeTime >= GameManager.Instance.chargeTime)
            {
                GameManager.Instance.currentChargeTime = GameManager.Instance.chargeTime;
                GameManager.Instance.TriggerPower();
            }
        }
            
        

#if UNITY_EDITOR
        Debug.DrawLine(objectA.position, objectB.position, Color.red);
#endif
    }
}
