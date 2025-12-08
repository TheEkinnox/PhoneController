using System;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle02 : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    private bool _puzzleFinished = false;

    public List<GameObject> books = new List<GameObject>();

    private void Start()
    {
        foreach (GameObject obj in books)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == targetObject && !_puzzleFinished)
        {
            foreach (GameObject book in books)
            {
                book.GetComponent<ObjectGravity>().gravityActive = true;
                Rigidbody rb = book.GetComponent<Rigidbody>();
                rb.isKinematic = false;
            }

            _puzzleFinished = true;
        }
    }
}