using UnityEngine;
using System.Collections;

public class SimpleEnemy : Enemy
{
    [SerializeField] protected float walkSp = 1;
    Rigidbody2D rb;
    bool timerDone = false;
    GameObject player = null;
    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        StartCoroutine(CustomUpdate());
    }

    IEnumerator CustomUpdate()
    {
        while (true){
            switch (state){
                case EnmyS.move:
                    rb.linearVelocity = (Random.value > 0.5 ? Vector2.right : Vector2.left) * walkSp / 2;
                    timerDone = false;
                    StartCoroutine(Timer(Random.Range(0.5f,1.0f)));
                    yield return new WaitUntil(() => (timerDone || player));
                    if (player)
                        state = EnmyS.alert;
                    else if (timerDone)
                        state = EnmyS.idle;
                break;
                case EnmyS.idle:
                    rb.linearVelocity = Vector2.zero;
                    timerDone = false;
                    StartCoroutine(Timer(Random.Range(0.5f,1.0f)));
                    yield return new WaitUntil(() => (timerDone || player));
                    if (player)
                        state = EnmyS.alert;
                    else if (timerDone)
                        state = EnmyS.idle;
                break;
                case EnmyS.alert:
                    if (!player){
                        yield return new WaitForSeconds(0.017f);
                        break;
                    }
                    rb.linearVelocity = Mathf.Sign(player.transform.position.x - transform.position.x) * Vector2.right;
                    yield return new WaitForSeconds(0.017f);
                    if (player)
                        state = EnmyS.alert;
                    else if (timerDone)
                        state = EnmyS.idle;
                break;
            }
        }
    }

    IEnumerator Timer(float time){
        timerDone = false;
        yield return new WaitForSeconds(time);
        timerDone = true;
    }

    void OnTriggerStay2D(Collider2D c){
        player = (c.gameObject.GetComponent<PlayerController>() == null) ? null : c.gameObject;
    }

    void OnTriggerExit2D(Collider2D c){
        if (c.gameObject.GetComponent<PlayerController>() != null)
            player = null;
    }
}
