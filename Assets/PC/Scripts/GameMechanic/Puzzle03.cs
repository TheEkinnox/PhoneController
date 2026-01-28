using System.Collections.Generic;
using UnityEngine;

public class Puzzle03 : MonoBehaviour
{
    private Camera _cam;
    [SerializeField] private List<GameObject> books = new List<GameObject>();
    [SerializeField] private List<GameObject> bookPlace = new List<GameObject>();
    [SerializeField] private Transform hand;
    private bool _emptyHand = true;
    [SerializeField] private GameObject door;
    private int _doorCount = 0;
    private GameObject _hitObj;
    [SerializeField] private Animator doorOpen;
    private bool _launchFinal = false;
    
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

                if (_emptyHand && books.Contains(hitObj))
                {
                    Rigidbody rb = hitObj.GetComponent<Rigidbody>();
                    if (!rb) return;

                    _hitObj = hitObj;
                    rb.isKinematic = true;
                    _emptyHand = false;

                    Debug.Log("Book grabbed");
                }

                else if (!_emptyHand && hitObj == door)
                {
                    int index = books.IndexOf(_hitObj);

                    if (index < 0 || index >= bookPlace.Count)
                    {
                        Debug.LogWarning("No matching book place found!");
                        return;
                    }

                    Transform targetPlace = bookPlace[index].transform;

                    _hitObj.transform.position = targetPlace.position;
                    _hitObj.transform.rotation = targetPlace.rotation;

                    Rigidbody rb = _hitObj.GetComponent<Rigidbody>();
                    if (rb) rb.isKinematic = true;

                    books.RemoveAt(index);
                    bookPlace.RemoveAt(index);

                    _hitObj = null;
                    _emptyHand = true;
                    _doorCount++;

                    Debug.Log("Book placed in correct slot");
                }
            }
        }
        if (!_emptyHand && _hitObj != null)
        {
            _hitObj.transform.position = hand.position;
            _hitObj.transform.rotation = hand.rotation;
        }

        if (_doorCount == 3 && !_launchFinal)
        {
            doorOpen.enabled = true;
            _launchFinal = true;
        }
    }
}

