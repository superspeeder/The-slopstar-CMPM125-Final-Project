using UnityEngine;
using System.Collections;

public class SimpleEnemy : Enemy
{
    [SerializeField] protected float walkSp = 1;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

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
        while (true)
        {
            // Stopping enemy AI movement during knockback (This is for the ChargeMovement ability)
            if (isKnockedBack)
            {
                animator.SetBool("isWalking", false);
                yield return null;   // ensure coroutine yields
                continue;
            }

            switch (state)
            {
                case EnmyS.move:
                    animator.SetBool("isWalking", true);

                    rb.linearVelocity = (Random.value > 0.5f ? Vector2.right : Vector2.left) * walkSp / 2;
                    spriteRenderer.flipX = rb.linearVelocityX < 0;

                    timerDone = false;
                    StartCoroutine(Timer(Random.Range(0.5f, 1.0f)));

                    // ALWAYS yields
                    yield return new WaitUntil(() => timerDone || player);

                    if (player)
                        state = EnmyS.alert;
                    else
                        state = EnmyS.idle;

                    break;

                case EnmyS.idle:
                    animator.SetBool("isWalking", false);
                    rb.linearVelocity = Vector2.zero;

                    timerDone = false;
                    StartCoroutine(Timer(Random.Range(0.5f, 1.0f)));

                    yield return new WaitUntil(() => timerDone || player);

                    if (player)
                        state = EnmyS.alert;
                    else
                        state = EnmyS.idle;

                    break;

                case EnmyS.alert:

                    if (!player)
                    {
                        // DETECTED CRASH FIX — must yield AND continue
                        yield return new WaitForSeconds(0.017f);
                        continue;
                    }

                    animator.SetBool("isWalking", true);

                    rb.linearVelocity = Mathf.Sign(player.transform.position.x - transform.position.x) * Vector2.right;
                    spriteRenderer.flipX = rb.linearVelocityX < 0;

                    yield return new WaitForSeconds(0.017f);

                    // stays in alert if still seeing player
                    if (!player)
                        state = EnmyS.idle;

                    break;
            }

            // Making sure to yield every loop just incase to STOP THESE CRASHES
            yield return null;
        }
    }

    IEnumerator Timer(float time)
    {
        timerDone = false;
        yield return new WaitForSeconds(time);
        timerDone = true;
    }

    void OnTriggerStay2D(Collider2D c)
    {
        player = (c.gameObject.GetComponent<PlayerController>() == null) ? null : c.gameObject;
    }

    void OnTriggerExit2D(Collider2D c)
    {
        if (c.gameObject.GetComponent<PlayerController>() != null)
            player = null;
    }
}
