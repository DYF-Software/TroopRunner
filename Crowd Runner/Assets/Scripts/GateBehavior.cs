using UnityEngine;
using TMPro;

public class GateBehavior : MonoBehaviour
{
    public GateOperation operation;
    public int value;

    [Header("Effect Settings")]
    [SerializeField] private GameObject gateEffectPrefab;

    private TextMeshPro labelText;

    private void Awake()
    {
        labelText = GetComponent<TextMeshPro>();

        if (labelText != null)
        {
            Vector3 currentRotation = labelText.transform.localEulerAngles;
            labelText.transform.localEulerAngles = new Vector3(90f, currentRotation.y, currentRotation.z);
        }
    }

    public void Initialize(GateOperation op, int val)
    {
        operation = op;
        value = val;
        UpdateLabel();
    }

    public void UpdateLabel()
    {
        if (labelText == null) return;

        string symbol = operation switch
        {
            GateOperation.Add => "+",
            GateOperation.Subtract => "-",
            GateOperation.Multiply => "×",
            GateOperation.Divide => "÷",
            _ => "?"
        };

        labelText.text = symbol + value.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerGroup"))
        {
            TroopSpawner spawner = other.GetComponent<TroopSpawner>();
            if (spawner != null)
            {
                spawner.ApplyGateEffect(operation, value);

                if (gateEffectPrefab != null)
                {
                    Instantiate(gateEffectPrefab, transform.position, Quaternion.identity);
                    Debug.Log("Gate effect spawned.");
                }
            }

            if (labelText != null)
                labelText.gameObject.SetActive(false);
        }
    }
}
