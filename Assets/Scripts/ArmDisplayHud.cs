using UnityEngine;
using UnityEngine.UI;

public class ArmDisplayHud : MonoBehaviour {
    public Image armI;
    public Image armJ;
    public Image armK;
    public Image armL;
    public RectTransform selector;
    public ElementArmManager armManager;
    public ArmCycler armCycler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        setColorFromElement(armManager.GetElementOfArm(0), armI);
        setColorFromElement(armManager.GetElementOfArm(1), armJ);
        setColorFromElement(armManager.GetElementOfArm(2), armK);
        setColorFromElement(armManager.GetElementOfArm(3), armL);

        selector.anchoredPosition = armCycler.GetActiveArmIndex() switch {
            0 => armI.rectTransform.anchoredPosition,
            1 => armJ.rectTransform.anchoredPosition,
            2 => armK.rectTransform.anchoredPosition,
            3 => armL.rectTransform.anchoredPosition,
            _ => selector.anchoredPosition
        };
    }

    private static void setColorFromElement(ElementType element, Image image) {
        image.color = elementColor(element);
    }

    private static Color elementColor(ElementType element) {
        return element switch {
            ElementType.Earth => Color.forestGreen,
            ElementType.Fire => Color.softRed,
            ElementType.Water => Color.dodgerBlue,
            ElementType.Air => Color.lightBlue,
            ElementType.Lightning => Color.yellow,
            _ => Color.white
        };
    }
}