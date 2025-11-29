using UnityEngine;

public class ArmEquip : MonoBehaviour
{
    [SerializeField] private ElementArmManager armManager;

    [Header("Default Elements For 4 Logical Arms (I, J, K, L)")]
    [SerializeField] private ElementType[] defaultElements = new ElementType[4];

    private void Start()
    {
        if (armManager == null)
        {
            return;
        }

        for (int i = 0; i < defaultElements.Length; i++)
        {
            armManager.SetArmElement(i, defaultElements[i]);
        }
    }
}
