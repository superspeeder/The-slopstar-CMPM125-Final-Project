using UnityEngine;

public class ArmEquip : MonoBehaviour
{
    [SerializeField] private ElementArmManager armManager;

    [SerializeField] private Transform leftArmSocket;
    [SerializeField] private Transform rightArmSocket;

    [SerializeField] private GameObject leftArmPrefab;   // Fire hand, etc.
    [SerializeField] private GameObject rightArmPrefab;  // Fire hand, etc.

    private GameObject _leftArmInstance;
    private GameObject _rightArmInstance;

    public static ArmEquip instance;

    private void Start() {
        instance = this;
        EquipDefaultArms();
    }

    private void EquipDefaultArms()
    {
        // Left
        if (_leftArmInstance != null) Destroy(_leftArmInstance);
        _leftArmInstance = Instantiate(leftArmPrefab, leftArmSocket);
        _leftArmInstance.transform.localPosition = Vector3.zero;
        _leftArmInstance.transform.localRotation = Quaternion.identity;

        // Right
        if (_rightArmInstance != null) Destroy(_rightArmInstance);
        _rightArmInstance = Instantiate(rightArmPrefab, rightArmSocket);
        _rightArmInstance.transform.localPosition = Vector3.zero;
        _rightArmInstance.transform.localRotation = Quaternion.identity;

        // Notify the manager
        if (armManager != null)
        {
            armManager.SetLeftArm(_leftArmInstance.GetComponent<ElementArm>());
            armManager.SetRightArm(_rightArmInstance.GetComponent<ElementArm>());
        }
    }
}
