using UnityEngine;
using UnityEngine.InputSystem;

public class ElementArmManager : MonoBehaviour
{
    [Header("Generic")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInput playerInput;

    [Header("Sockets")]
    [SerializeField] private Transform leftArmSocket;
    [SerializeField] private Transform rightArmSocket;

    [Header("Current Arms (optional, for reference)")]
    [SerializeField] private ElementArm leftArm;
    [SerializeField] private ElementArm rightArm;

    [Header("Updraft Ability")]
    [SerializeField] private GameObject updraftPrefab;
    [SerializeField] private Transform updraftSpawnPoint; // where the updraft appears
    [SerializeField] private float updraftDuration = 3f;
    [SerializeField] private float updraftCooldown = 5f;
    
    [Header("Earth Platformn")] // Platformn my beloved
    [SerializeField] private GameObject earthRampPrefab;
    [SerializeField] private Transform earthRampSpawnPoint;

    [Header("Air Platform")]
    [SerializeField] private GameObject airPlatformPrefab;

    [Header("Lightning Speed Boost")]
    [SerializeField] private float lightningSpeedMultiplier = 2f;
    [SerializeField] private float lightningBoostDuration = 3f;

    [Header("Water Pool")]
    [SerializeField] private GameObject bucket;

    private float _nextUpdraftTime;
    private InputAction _actAction;

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Start()
    {
        _actAction = playerInput.actions["Act"];
    }

    void Update()
    {
        if (_actAction.triggered)
        {
            if (mainCam == null)
                mainCam = Camera.main;

            Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorld.z = 0f;

            // Debug so you can see what’s happening
            // Debug.Log($"[Arms] L={leftArm?.elementType}, R={rightArm?.elementType}");

            if (BothArmsAreLightning())
            {
                // Lightning: speed boost instead of spawning something
                if (playerController != null)
                {
                    playerController.ApplySpeedBoost(lightningSpeedMultiplier, lightningBoostDuration);
                    // Debug.Log("[Ability] Lightning combo → Speed boost");
                }
            }
            else if (BothArmsAreAir())
            {
                SpawnAirPlatform(mouseWorld);
            }
            else if (BothArmsAreEarth())
            {
                SpawnEarthRamp(mouseWorld);
            }
            else if (BothArmsAreFire())
            {
                TrySpawnUpdraft(mouseWorld);
            }
            else if (BothArmsAreWater())
            {
                spawnWaterPool(mouseWorld);
            }
        }
    }




    public void SetLeftArm(ElementArm arm)
    {
        leftArm = arm;
    }

    public void SetRightArm(ElementArm arm)
    {
        rightArm = arm;
    }

    private bool BothArmsAreFire()
    {
        if (leftArm == null || rightArm == null)
        {
            Debug.Log("[Combo] Fire check failed: missing arm ref");
            return false;
        }

        bool result = leftArm.elementType == ElementType.Fire &&
                      rightArm.elementType == ElementType.Fire;

        Debug.Log($"[Combo] Fire? {result} (L={leftArm.elementType}, R={rightArm.elementType})");
        return result;
    }

    private bool BothArmsAreEarth()
    {
        if (leftArm == null || rightArm == null)
        {
            Debug.Log("[Combo] Earth check failed: missing arm ref");
            return false;
        }

        bool result = leftArm.elementType == ElementType.Earth &&
                      rightArm.elementType == ElementType.Earth;

        Debug.Log($"[Combo] Earth? {result} (L={leftArm.elementType}, R={rightArm.elementType})");
        return result;
    }

    private bool BothArmsAreAir()
    {
        if (leftArm == null || rightArm == null)
            return false;

        return leftArm.elementType == ElementType.Air &&
               rightArm.elementType == ElementType.Air;
    }


    private bool BothArmsAreLightning()
    {
        if (leftArm == null || rightArm == null)
            return false;

        return leftArm.elementType == ElementType.Lightning &&
               rightArm.elementType == ElementType.Lightning;
    }

        private bool BothArmsAreWater()
    {
        if (leftArm == null || rightArm == null)
        {
            Debug.Log("[Combo] Water check failed: missing arm ref");
            return false;
        }

        bool result = leftArm.elementType == ElementType.Water && rightArm.elementType == ElementType.Water;
        return result;
    }

    private void TrySpawnUpdraft(Vector3 spawnPos)
    {
        if (updraftPrefab == null)
        {
            Debug.LogWarning("[ElementArmManager] Missing updraftPrefab");
            return;
        }

        Instantiate(updraftPrefab, spawnPos, Quaternion.identity);
    }


    private void SpawnEarthRamp(Vector3 spawnPos)
    {
        if (earthRampPrefab == null)
        {
            Debug.LogWarning("[ElementArmManager] Missing earthRampPrefab");
            return;
        }

        Instantiate(earthRampPrefab, spawnPos, Quaternion.identity);
    }

    private void SpawnAirPlatform(Vector3 spawnPos)
    {
        if (airPlatformPrefab == null)
        {
            Debug.LogWarning("[ElementArmManager] Missing airPlatformPrefab");
            return;
        }

        Instantiate(airPlatformPrefab, spawnPos, Quaternion.identity);
    }

    private void spawnWaterPool(Vector3 playerPos)
    {
        Debug.Log("[Combo] Water combo activated at " + playerPos);
        
        if(bucket == null)
        {
            Debug.LogError("Dear God...");
            Debug.LogError("There's more.");
            Debug.LogError("No...");
            return;
        }

        Transform closestPool = null;
        float closestDistance = float.MaxValue;
        foreach (Transform child in bucket.transform)
        {
            float distance = Vector3.Distance(playerPos, child.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPool = child;
            }
        }

        if(closestPool != null && closestDistance <= 5f) // Range is just a number
        {
            closestPool.gameObject.SetActive(true);
            Debug.Log("Water pool activated at " + closestPool.position);
        }
        else
        {
            Debug.Log("lmao get dehydrated nerd");
        }
    }
}
