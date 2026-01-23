using System;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Vector3 GravityDirection { get; private set; } = Vector3.down;
    public float gravityStrength = 20f;

    public bool powerAlreadyTriggered = false;
    public float currentChargeTime;
    public float chargeTime;
    public Quaternion phoneRotation;
    public bool gameStarted = false;
    [SerializeField] private GameObject menu;

    public void TriggerPower()
    {
        if (powerAlreadyTriggered)
            return;

        powerAlreadyTriggered = true;
        PowerTrigger?.Invoke();
    }

    public void Update()
    {
        if (!gameStarted)
            GravityDirection = Physics.gravity;
        else
            GravityDirection = phoneRotation * Vector3.down;
    }

    public void StartGame()
    {
        GameStarted?.Invoke();
        gameStarted = true; 
        Cursor.lockState = CursorLockMode.Locked;
        menu.SetActive(false);
    }
    

    public void LeaveGame() => Application.Quit();
    

    public event Action PowerTrigger;
    public event Action GameStarted;
}