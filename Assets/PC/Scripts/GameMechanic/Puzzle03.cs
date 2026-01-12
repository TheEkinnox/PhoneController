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
    private GameObject _hitObj;
    
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

                // ===== GRAB =====
                if (_emptyHand && books.Contains(hitObj))
                {
                    Rigidbody rb = hitObj.GetComponent<Rigidbody>();
                    if (!rb) return;

                    _hitObj = hitObj;
                    rb.isKinematic = true;
                    _emptyHand = false;

                    Debug.Log("Book grabbed");
                }

                // ===== PLACE =====
                else if (!_emptyHand && hitObj == door)
                {
                    _hitObj.transform.position = door.transform.position;

                    Rigidbody rb = _hitObj.GetComponent<Rigidbody>();
                    if (rb) rb.isKinematic = true;

                    books.Remove(_hitObj);
                    _hitObj = null;
                    _emptyHand = true;
                    _doorCount++;

                    Debug.Log("Book placed");
                }
            }
        }

        // ===== FOLLOW HAND =====
        if (!_emptyHand && _hitObj != null)
        {
            _hitObj.transform.position = hand.position;
            _hitObj.transform.rotation = hand.rotation;
        }

        if (_doorCount == 3)
        {
            Debug.Log("finished");
        }
    }
}

