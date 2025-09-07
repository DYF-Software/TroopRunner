using UnityEngine;
using UnityEngine.UI;

public class PlayerGroupController : MonoBehaviour
{
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private Slider movementSlider;

    private float zPos;
    private float xTarget;
    private Transform playerGroup;

    private void Start()
    {
        zPos = transform.position.z;
        xTarget = transform.position.x;

        playerGroup = GameObject.FindGameObjectWithTag("PlayerGroup")?.transform;

        if (movementSlider == null)
            Debug.LogWarning("Movement slider is not assigned.");
        if (playerGroup == null)
            Debug.LogError("No object with tag 'PlayerGroup' found!");
    }

    void Update()
    {
        HandleInput();
        MoveForwardAndSlide();
        UpdateSliderHandle();
    }

    void HandleInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0))
        {
            float percent = Input.mousePosition.x / Screen.width;
            xTarget = Mathf.Lerp(-gameSettings.scrollRange, gameSettings.scrollRange, percent);
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float percent = touch.position.x / Screen.width;
            xTarget = Mathf.Lerp(-gameSettings.scrollRange, gameSettings.scrollRange, percent);
        }
#endif
    }

    void MoveForwardAndSlide()
    {
        int troopCount = GetComponent<TroopSpawner>().TroopCount;
        float groupHalfWidth = Mathf.Sqrt(troopCount) * gameSettings.spacingPerTroop;

        float minX = -gameSettings.laneHalfWidth + groupHalfWidth;
        float maxX = gameSettings.laneHalfWidth - groupHalfWidth;

        float clampedX = Mathf.Clamp(xTarget, minX, maxX);
        Vector3 tentativeTarget = new Vector3(clampedX, transform.position.y, transform.position.z); 

        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(tentativeTarget, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
        {
            transform.position = Vector3.Lerp(transform.position, hit.position, Time.deltaTime * gameSettings.lateralLerpSpeed);
        }
    }

    void UpdateSliderHandle()
    {
        if (movementSlider == null || playerGroup == null) return;

        float range = gameSettings.scrollRange;
        float normalizedX = Mathf.InverseLerp(-range, range, playerGroup.position.x);
        movementSlider.value = normalizedX;
    }




}
