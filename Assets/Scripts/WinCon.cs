using UnityEngine;

public class WinCon : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other){
        if (other.gameObject.GetComponent<PlayerController>())
            SceneManager.LoadScene("YouWin!");
    }
}
