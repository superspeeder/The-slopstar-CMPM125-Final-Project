using UnityEngine;

public class ArmCycler : MonoBehaviour
{
    [SerializeField] private ElementArmManager armManager;

    [Header("Arm Prefabs (order: Fire, Water, Air, Earth, Lightning)")]
    [SerializeField] private GameObject[] armPrefabs;

    [Header("Hand Sockets")]
    [SerializeField] private Transform leftHandSocket;   // Assign HandSocket1
    [SerializeField] private Transform rightHandSocket;  // Assign HandSocket2

    private int currentIndex = 1; // Start at 1 to skip None
    private ElementType[] elementTypes;

    void Awake()
    {
        elementTypes = (ElementType[])System.Enum.GetValues(typeof(ElementType));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            CycleArms();
        }
    }

    void CycleArms()
    {
        currentIndex++;
        if (currentIndex >= elementTypes.Length)
            currentIndex = 1;

        var newType = elementTypes[currentIndex];
        int prefabIndex = currentIndex - 1; // armPrefabs[0]=Fire, [1]=Water, [2]=Air, [3]=Earth, [4]=Lightning

        // Destroy current children in sockets
        if (leftHandSocket.childCount > 0)
            Destroy(leftHandSocket.GetChild(0).gameObject);
        if (rightHandSocket.childCount > 0)
            Destroy(rightHandSocket.GetChild(0).gameObject);

        // Instantiate new arms
        if (prefabIndex >= 0 && prefabIndex < armPrefabs.Length)
        {
            var leftArmObj = Instantiate(armPrefabs[prefabIndex], leftHandSocket);
            leftArmObj.transform.localPosition = Vector3.zero;
            leftArmObj.transform.localRotation = Quaternion.identity;

            var rightArmObj = Instantiate(armPrefabs[prefabIndex], rightHandSocket);
            rightArmObj.transform.localPosition = Vector3.zero;
            rightArmObj.transform.localRotation = Quaternion.identity;

            // Notify manager
            if (armManager != null)
            {
                armManager.SetLeftArm(leftArmObj.GetComponent<ElementArm>());
                armManager.SetRightArm(rightArmObj.GetComponent<ElementArm>());
            }
        }

        Debug.Log($"[ArmCycler] Arms set to {newType}");
    }
}