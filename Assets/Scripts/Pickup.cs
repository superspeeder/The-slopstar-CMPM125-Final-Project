using System;
using UnityEngine;

public class Pickup : MonoBehaviour {
    public ElementType elementType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        switch (elementType) {
            case ElementType.Earth:
                GetComponent<SpriteRenderer>().color = Color.forestGreen;
                break;
            case ElementType.Fire:
                GetComponent<SpriteRenderer>().color = Color.softRed;
                break;
            case ElementType.Water:
                GetComponent<SpriteRenderer>().color = Color.dodgerBlue;
                break;
            case ElementType.Air:
                GetComponent<SpriteRenderer>().color = Color.lightBlue;
                break;
            case ElementType.Lightning:
                GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
        }
    }

    // Update is called once per frame
    void Update() {
    }

    public void SetElement(ElementType element) {
        elementType = element;
        if (element == ElementType.None) {
            Destroy(gameObject);
        }
        else {
            switch (element) {
                case ElementType.Earth:
                    GetComponent<SpriteRenderer>().color = Color.forestGreen;
                    break;
                case ElementType.Fire:
                    GetComponent<SpriteRenderer>().color = Color.softRed;
                    break;
                case ElementType.Water:
                    GetComponent<SpriteRenderer>().color = Color.dodgerBlue;
                    break;
                case ElementType.Air:
                    GetComponent<SpriteRenderer>().color = Color.lightBlue;
                    break;
                case ElementType.Lightning:
                    GetComponent<SpriteRenderer>().color = Color.yellow;
                    break;
            }
        }
    }
}