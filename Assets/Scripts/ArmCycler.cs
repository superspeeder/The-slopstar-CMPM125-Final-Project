using System;
using UnityEngine;

public class ArmCycler : MonoBehaviour
{
    [SerializeField] private ElementArmManager armManager;

    [Header("Logical Arm Index (0-3)")]
    [SerializeField] private int armIndex = 0;

    private int currentElementIndex = 1;
    private ElementType[] elementTypes;

    public static ArmCycler instance;
    

    private void Awake()
    {
        elementTypes = (ElementType[])System.Enum.GetValues(typeof(ElementType));
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            armIndex = 0;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            armIndex = 1;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            armIndex = 2;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            armIndex = 3;
        }
        
        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     CycleArmElement();
        // }
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

    public int GetActiveArmIndex() {
        return armIndex;
    }

    public void SetActiveArmElement(ElementType element) {
        Debug.Log($"Change arm {armIndex} to element {element}");
        armManager.SetArmElement(armIndex, element);
    }

    public ElementType GetActiveArmElement() {
        return armManager.GetElementOfArm(armIndex);
    }
}
