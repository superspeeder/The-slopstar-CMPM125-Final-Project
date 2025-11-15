using UnityEngine;

public enum ElementType
{
    None,
    Fire,
    Water,
    Air,
    Earth,
    Lightning
}

public class ElementArm : MonoBehaviour
{
    public ElementType elementType = ElementType.None;
}
