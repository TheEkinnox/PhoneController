using UnityEngine;

public class Puzzle01 : MonoBehaviour
{
    [SerializeField] private Transform objectA;
    [SerializeField] private Transform objectB;

    void Update()
    {
        Vector3 direction = objectB.position - objectA.position;
        
        float distance = direction.magnitude;
        
        Vector3 dirNormalized = direction.normalized;
        
        if (Physics.Raycast(objectA.position, dirNormalized, out RaycastHit hit, distance))
            if (hit.collider.CompareTag("SolarPanel") && !GameManager.Instance.powerEnabled)
                GameManager.Instance.powerEnabledAction = true;
            
        
        #if UNITY_EDITOR
        Debug.DrawLine(objectA.position, objectB.position, Color.red);
        #endif
    }
}
