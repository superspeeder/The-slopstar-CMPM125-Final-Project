using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool open = false;
    Vector3 pos;
    float yScale;

    void Start()
    {
        yScale = transform.localScale.y;
        pos = transform.position;
        StartCoroutine(DoorLogic());
    }

    IEnumerator DoorLogic(){
        while (true){
            yield return new WaitUntil(() => open);
            while (Mathf.Abs(transform.localScale.y) > 0.05f && open){
                transform.localScale = new Vector3(transform.localScale.x,transform.localScale.y/2,transform.localScale.z);
                transform.position += transform.localScale.y * transform.up / 2;
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitUntil(() => !open);
            while (Mathf.Abs(transform.localScale.y) < yScale && !open){
                transform.localScale = new Vector3(transform.localScale.x,(transform.localScale.y + yScale)/2,transform.localScale.z);
                transform.position = (transform.position + pos) / 2;
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
