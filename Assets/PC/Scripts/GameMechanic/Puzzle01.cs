using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Puzzle01 : MonoBehaviour
{
    
    [SerializeField] private Transform objectA;
    [SerializeField] private Transform objectB;
    [SerializeField] private LayerMask raycastMask;
    
    void Update()
    {
        Vector3 direction = objectB.position - objectA.position;
        float distance = direction.magnitude;
        Vector3 dirNormalized = direction.normalized;
        bool isCharging = false;
        GameManager gm = GameManager.Instance;

        if (Physics.Raycast(objectA.position, dirNormalized, out RaycastHit hit, distance))
        {
            if (hit.collider.CompareTag("SolarPanel"))
                isCharging = true;
        }

        if (!gm.powerAlreadyTriggered)
        {
            if (isCharging)
                gm.currentChargeTime += Time.deltaTime;
            else
                gm.currentChargeTime -= Time.deltaTime;

            gm.currentChargeTime = Mathf.Clamp(gm.currentChargeTime, 0f, gm.chargeTime);

            if (gm.currentChargeTime >= gm.chargeTime)
            {
                gm.currentChargeTime = gm.chargeTime;
                gm.TriggerPower();
            }
        }
            
        

#if UNITY_EDITOR
        Debug.DrawLine(objectA.position, objectB.position, Color.red);
#endif
    }
}
