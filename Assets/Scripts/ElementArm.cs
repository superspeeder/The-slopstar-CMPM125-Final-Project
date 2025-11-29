using UnityEngine;

public enum ElementType
{
    None = 0,
    Fire = 1,
    Water = 2,
    Air = 3,
    Earth = 4,
    Lightning = 5
}

public class ElementArm : MonoBehaviour
{
    public ElementType elementType = ElementType.None;
}
