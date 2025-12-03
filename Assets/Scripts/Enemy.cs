using UnityEngine;
using System.Collections;

public enum EnmyS {
    idle,
    move,
    fall,
    alert,
    windup,
    attack,
    winddown,
    hitstun
}

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int health = 1;
    //[SerializeField] protected float hitstunFor = 0;
    [SerializeField] protected EnmyS state = EnmyS.idle;
    protected EnmyS prevState = EnmyS.idle;

    public bool isKnockedBack = false;

    public int DecrementHealth(int value, float hitstunFor = 0.0f, bool ignoreHitstun = false){
        if (!ignoreHitstun && state == EnmyS.hitstun)
            return health;
        health -= value;
        prevState = state;
        state = EnmyS.hitstun;
        StartCoroutine(DoStun(hitstunFor));
        if (health <= 0)
            Destroy(gameObject);
        return health;
    }

    IEnumerator DoStun(float hitstunFor){
        yield return new WaitForSeconds(hitstunFor);
        state = prevState;
        if (state == EnmyS.hitstun)
            state = EnmyS.fall;
    }
}
