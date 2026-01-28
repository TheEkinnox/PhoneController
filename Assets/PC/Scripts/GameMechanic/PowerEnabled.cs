using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerEnabled : MonoBehaviour
{
    [SerializeField] private List<GameObject> emissiveObjects = new();
    [SerializeField] private List<GameObject> spinObjects = new();
    [SerializeField] private List<TextMeshPro> timers = new();
    [SerializeField] private Light mainLight;
    private float _mainLightValue;

    private readonly List<Material> _materials = new();
    private readonly List<ObjectSpin> _objectSpins = new();

    private void Awake()
    {
        foreach (GameObject obj in emissiveObjects)
        {
            if (!obj) continue;

            Renderer renderer = obj.GetComponent<Renderer>();
            if (!renderer) continue;

            _materials.AddRange(renderer.materials);
        }

        foreach (GameObject obj in spinObjects)
        {
            if (!obj) continue;

            ObjectSpin spin = obj.GetComponent<ObjectSpin>();
            if (spin)
                _objectSpins.Add(spin);
        }
        _mainLightValue = mainLight.intensity;
        mainLight.intensity = 0f;
    }

    private void Update()
    {
        if (GameManager.Instance.powerAlreadyTriggered)
            return;
        
        float emissiveValue = Mathf.InverseLerp(
            0f,
            GameManager.Instance.chargeTime,
            GameManager.Instance.currentChargeTime
        );

        foreach (Material mat in _materials)
        {
            if (mat)
                mat.SetFloat("_EmissiveOn", emissiveValue);
        }
        
                
        foreach (TextMeshPro mesh in timers)
        {
            if (!mesh) continue;
            mesh.alpha = emissiveValue;
        }
        
        foreach (ObjectSpin spin in _objectSpins)
        {
            if (!spin) continue;
            spin.powerMultiplier = emissiveValue;
        }
        
        mainLight.intensity = emissiveValue * _mainLightValue;
    }
    
}