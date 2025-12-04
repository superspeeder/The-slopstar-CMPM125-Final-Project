using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCDialog : MonoBehaviour
{

    private GameObject _player;
    private GameObject _text;
    private InputAction _talkAction;
    private PlayerInput _playerInput;

    private bool isIn = false;
    
    void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _playerInput = _player.GetComponent<PlayerInput>();
        _talkAction = _playerInput.actions["Act"];
        _text = transform.Find("Canvas").gameObject;
        _text.SetActive(false);
    }

    void Update()
    {
        if (!isIn) return;
        float actInput = _talkAction.ReadValue<float>();
        if (actInput > 0)
        {
            _text.SetActive(true);
        }
    }

    private void OnTriggerEnter2D()
    {
        isIn = true;
    }

    private void OnTriggerExit2D()
    {
        isIn = false;
        _text.SetActive(false);
    }
}
