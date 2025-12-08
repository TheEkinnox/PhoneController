using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Character : MonoBehaviour
{
    private CharacterController characterController;
    public float speed = 5f;
    public float gravity = -9.8f;
    Vector3 velocity;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 move =  new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        
        characterController.Move(move * (speed * Time.deltaTime));
        velocity.y += gravity * Time.deltaTime;
        
        characterController.Move(velocity * Time.deltaTime);
    }
}
