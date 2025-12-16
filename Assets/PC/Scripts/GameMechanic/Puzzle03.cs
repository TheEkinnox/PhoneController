using System.Collections.Generic;
using UnityEngine;

public class Puzzle03 : MonoBehaviour
{
    private Camera _cam;
    [SerializeField] private List<GameObject> books = new List<GameObject>();
    [SerializeField] private Transform hand;
    private bool _emptyHand = true;
    [SerializeField] private GameObject door;
    private int _doorCount = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            
            if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, 1000f))
            {
                GameObject hitObj = hit.collider.gameObject;
                
                if (books.Contains(hitObj) && _emptyHand)
                {
                    Rigidbody rb = hit.collider.gameObject.GetComponent<Rigidbody>();
                    rb.isKinematic = false;
                    hitObj.transform.position = hand.position;
                    _emptyHand = false;
                }

                if (hit.collider.gameObject == door && !_emptyHand)
                {
                    hitObj.transform.position = door.transform.position;
                    Rigidbody rb = hit.collider.gameObject.GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                    _emptyHand = true;
                    _doorCount++;
                }
            }
        }
    }
}
