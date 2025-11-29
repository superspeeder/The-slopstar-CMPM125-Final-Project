using UnityEngine;

public class ArmCycler : MonoBehaviour
{
    [SerializeField] private ElementArmManager armManager;

    [Header("Logical Arm Index (0-3)")]
    [SerializeField] private int armIndex = 0;

    private int currentElementIndex = 1;
    private ElementType[] elementTypes;

    private void Awake()
    {
        elementTypes = (ElementType[])System.Enum.GetValues(typeof(ElementType));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            CycleArmElement();
        }
        if(Input.GetKeyDown(KeyCode.O))
        {
            armIndex = (armIndex + 1) % 4;
        }
    }

    private void CycleArmElement()
    {
        currentElementIndex++;
        if (currentElementIndex >= elementTypes.Length)
        {
            currentElementIndex = 1;
        }

        var newType = elementTypes[currentElementIndex];

        if (armManager != null)
        {
            armManager.SetArmElement(armIndex, newType);
        }
    }
}
