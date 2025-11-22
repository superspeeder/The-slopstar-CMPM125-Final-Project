using UnityEngine;

public class BulletAttributes : MonoBehaviour
{

    // 1 is right, 0 is static, -1 is left
    [SerializeField] private int direction;
    [SerializeField] private float speed;
    [SerializeField] private Collider collider;
    [SerializeField] public Transform spawnPoint;
    [SerializeField] public float duration;
    [SerializeField] public Transform enemyLocation;
    [SerializeField] public float maxRange;
    [SerializeField] public float distanceRan;
    [SerializeField] public float playerSpeedMult = 1f;
    [SerializeField] private float maxSpeedMult = 3f;
    [SerializeField] private bool physicsEnabled;
    [SerializeField] private Image bulletImage;
    private float timeRunning = 0f;
    private Rigidbody2D rb;


    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MovementUpdate();
    }

    void InstantiateBoolet(int direction, float speed, Collider collider, Transform spawnPoint, float duration, Transform enemyLocation, float maxRange, Image bulletImage)
    {
        direction = direction;
        speed = speed;
        collider = collider;
        spawnPoint = spawnPoint;
        duration = duration;
        enemyLocation = enemyLocation;
        maxRange = maxRange;
        bulletImage = bulletImage;
    }

    void MovementUpdate()
    {
        velocity = new Vector2(speed * direction);
        if(rb.velocity.magnitude < speed)
        {
            rb.AddForce(velocity);
        }
    }    
}
