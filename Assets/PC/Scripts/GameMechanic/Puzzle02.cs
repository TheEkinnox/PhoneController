using System.Collections.Generic;
using UnityEngine;

public class Puzzle02 : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    private bool _puzzleFinished = false;
    private Camera _cam;
    [SerializeField]  private GameObject puzzle3;

    public List<GameObject> books = new List<GameObject>();

    private void Start()
    {
        _cam = Camera.main;
        foreach (GameObject obj in books)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject != targetObject || _puzzleFinished)
            return;

        foreach (GameObject book in books)
        {
            book.GetComponent<ObjectGravity>().gravityScale = 1f;
            Rigidbody rb = book.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.freezeRotation = false;
        }
        
        
        puzzle3.SetActive(true);
        _puzzleFinished = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_puzzleFinished && GameManager.Instance.powerAlreadyTriggered)
        {
            RaycastHit hit;
            
            if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, 1000f))
            {
                if (hit.collider.gameObject == targetObject)
                {
                    Rigidbody rb = hit.collider.gameObject.GetComponent<Rigidbody>();
                    rb.isKinematic = false;
                    Debug.Log(hit.collider.gameObject.name);
                }
            }
        }
    }
}