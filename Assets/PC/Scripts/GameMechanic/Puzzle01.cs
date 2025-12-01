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
        {
            if (hit.collider.CompareTag("SolarPanel"))
            {
                Debug.Log("Nice Cock");
            }
            else
            {
                Debug.Log("hunhun");
            }
        }

        // Draw visible line in Scene view
        Debug.DrawLine(objectA.position, objectB.position, Color.red);
    }
}
