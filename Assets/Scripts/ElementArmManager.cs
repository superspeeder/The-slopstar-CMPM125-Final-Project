using UnityEngine;

public class ElementArmManager : MonoBehaviour
{
    [Header("Arm Elements")]
    public ElementType[] arms = new ElementType[4];
    // arms[0] == I
    // arms[1] == J
    // arms[2] == K
    // arms[3] == L

    [Header("Combo Settings")]
    public float comboWindow = 0.25f;

    [Header("Arm Cycler Integration")]
    [SerializeField] private int testArmIndex = 0; 
    private int currentElementIndex = 1; 
    private ElementType[] elementTypes;
    private AttackManager attackManager;
    private int firstArmIndex = -1;
    private float comboTimer = 0f;
    private bool waitingForSecond = false;
    public static ElementArmManager instance;

    private void Awake() {
        instance = this;
        elementTypes = (ElementType[])System.Enum.GetValues(typeof(ElementType));
        attackManager = GetComponent<AttackManager>();
    }

    void Update()
    {
        HandleComboTimer();
        HandleArmInput();
        HandleTestingInput();
    }

    private void HandleArmInput()
    {
        if (Input.GetKeyDown(KeyCode.I)) RegisterArmPress(0);
        if (Input.GetKeyDown(KeyCode.J)) RegisterArmPress(1);
        if (Input.GetKeyDown(KeyCode.K)) RegisterArmPress(2);
        if (Input.GetKeyDown(KeyCode.L)) RegisterArmPress(3);
    }

    private void RegisterArmPress(int armIndex)
    {
        if (!waitingForSecond)
        {
            waitingForSecond = true;
            comboTimer = comboWindow;
            firstArmIndex = armIndex;
            return;
        }

        if (armIndex == firstArmIndex)
        {
            waitingForSecond = false;
            firstArmIndex = -1;
            return;
        }

        waitingForSecond = false;

        ElementType e1 = GetElementOfArm(firstArmIndex);
        ElementType e2 = GetElementOfArm(armIndex);

        if (e1 == ElementType.None || e2 == ElementType.None)
        {
            firstArmIndex = -1;
            return;
        }

        attackManager.TryExecuteCombo(firstArmIndex, armIndex);

        firstArmIndex = -1;
    }

    private void HandleComboTimer()
    {
        if (!waitingForSecond)
            return;

        comboTimer -= Time.deltaTime;

        if (comboTimer <= 0f)
        {
            waitingForSecond = false;
            firstArmIndex = -1;
        }
    }

    public ElementType GetElementOfArm(int index)
    {
        if (index < 0 || index >= arms.Length)
            return ElementType.None;

        return arms[index];
    }

    public void SetArmElement(int index, ElementType element)
    {
        if (index < 0 || index >= arms.Length)
            return;

        arms[index] = element;
    }
    
    private void HandleTestingInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            CycleArmElement();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            testArmIndex = (testArmIndex + 1) % 4;
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
        SetArmElement(testArmIndex, newType);
    }
}
