using System.Collections.Generic;
using UnityEngine;

public class PowerEnabled : MonoBehaviour
{
    [SerializeField] private List<GameObject> emissiveObjects = new();
    [SerializeField] private List<GameObject> spinObjects = new();

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
    }

    private void OnEnable()
    {
        GameManager.Instance.PowerTrigger += LightTurnOn;
    }

    private void OnDisable()
    {
        if (GameManager.Instance)
            GameManager.Instance.PowerTrigger -= LightTurnOn;
    }

    private void Update()
    {
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
    }

    private void LightTurnOn()
    {
        foreach (ObjectSpin spin in _objectSpins)
        {
            if (spin)
                spin.enabled = true;
        }
    }
}