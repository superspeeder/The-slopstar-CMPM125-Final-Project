using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [Header("Core References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private ElementArmManager armManager;

    [Header("Projectile Pool")]
    [SerializeField] private ProjectilePool projectilePool;

    [Header("Non-Projectile Ability Prefabs")]
    [SerializeField] private GameObject steamCloudPrefab;
    [SerializeField] private GameObject freezeFieldPrefab;
    [SerializeField] private GameObject swampFieldPrefab;
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private GameObject flameConePrefab;
    [SerializeField] private GameObject lavaSprayPrefab;
    [SerializeField] private GameObject chargePrefab;

    [Header("Platforming Abilities")]
    [SerializeField] private GameObject updraftPrefab;
    [SerializeField] private float updraftDuration = 3f;
    [SerializeField] private GameObject airPlatformPrefab;
    [SerializeField] private GameObject earthRampPrefab;


    private float[,] comboNextReadyTime;
    private GameObject activeUpdraftInstance;

    private void Awake()
    {
        int count = System.Enum.GetValues(typeof(ElementType)).Length;
        comboNextReadyTime = new float[count, count];
    }

    public void TryExecuteCombo(int armA, int armB)
    {
        if (!player || !armManager)
            return;

        if (armA == armB)
            return;

        ElementType e1 = armManager.GetElementOfArm(armA);
        ElementType e2 = armManager.GetElementOfArm(armB);

        if (e1 == ElementType.None || e2 == ElementType.None)
            return;

        if ((int)e1 > (int)e2)
        {
            var temp = e1;
            e1 = e2;
            e2 = temp;
        }

        int i = (int)e1;
        int j = (int)e2;

        if (Time.time < comboNextReadyTime[i, j])
            return;

        float cooldown = GetComboCooldown(e1, e2);
        comboNextReadyTime[i, j] = Time.time + cooldown;

        ExecuteCombo(e1, e2);
    }


    private float GetComboCooldown(ElementType e1, ElementType e2)
    {
        if (e1 == e2)
            return 5f;
        return 0f;
    }

    private void ExecuteCombo(ElementType e1, ElementType e2)
    {
        if (e1 == e2)
        {
            switch (e1)
            {
                case ElementType.Fire:
                    ExecuteFireFire();
                    break;
                case ElementType.Water:
                    ExecuteWaterWater();
                    break;
                case ElementType.Air:
                    ExecuteAirAir();
                    break;
                case ElementType.Earth:
                    ExecuteEarthEarth();
                    break;
                case ElementType.Lightning:
                    ExecuteLightningLightning();
                    break;
            }
        }
        else
        {
            ExecuteMixedCombo(e1, e2);
        }
    }

    private void ExecuteMixedCombo(ElementType a, ElementType b) // So that we have 10 attacks and not 25
    {
        if ((int)a > (int)b)
        {
            var t = a;
            a = b;
            b = t;
        }

        Vector3 spawnPos = player.transform.position + new Vector3(player.direction, 0, 0);
        Transform nearestEnemy = FindNearestEnemy();

        /*
            Blinding fog / steam idea that makes enemies blind
            Static projectile <= speed
            Spawns on enemy if one is in range
            Lasts 3 seconds <= duration
            Disables enemy perception for duration
        */
        if (a == ElementType.Fire && b == ElementType.Water)
        {
            BulletAttributes proj = projectilePool.GetProjectile();
            proj.transform.position = spawnPos;
            AchievementState.GiveAchievement(Achievement.UseWaterFire);

            proj.Init(
                speed: 12f,
                maxRange: 14f,
                direction: player.direction,
                homing: false,
                target: null,
                turnSpeed: 0f
            );

            proj.GetComponent<AoEEffectSpawner>().Setup(steamCloudPrefab);
            return;
        }

        /* 
            Fire and Lightning
            Explosion in front that sends enemies away
            Moving projectile
            Moves quickly
            Applies force in the same direction as the bolt travelled
        */
        if (a == ElementType.Fire && b == ElementType.Lightning)
        {
            return;
        }

        /*
            Fire and air
            Shorter range fire cone that does damage over time
            Flamethrower-esc cone, damages enemies in front
            Pretty much melee
            Lasts as long as the player activates it
        */
        if (a == ElementType.Fire && b == ElementType.Air)
        {
            var cone = Instantiate(flameConePrefab).GetComponent<FlameCone>();
            AchievementState.GiveAchievement(Achievement.UseAirFire);
            cone.player = player.transform;
            cone.direction = player.direction;
            return;
        }

        /*
            Fire and Earth
            Short range, flamethrower that leaves lava on surfaces
            Longer duration, 10s
        */
        if (a == ElementType.Fire && b == ElementType.Earth)
        {
            var spray = Instantiate(lavaSprayPrefab).GetComponent<LavaSprayCone>();
            AchievementState.GiveAchievement(Achievement.UseFireEarth);
            spray.player = player.transform;
            spray.direction = player.direction;
            return;
        }

        /*
            Water and Lightning 
            Water making an ability being electrified, damage buff
            A projectile of water must hit the enemy, when it does a bolt (the actual damage dealing component) “charges”(?) and automatically targets enemy from above, passing through terrain for a guaranteed hit
        */
        if (a == ElementType.Water && b == ElementType.Lightning)
        {
            if (nearestEnemy == null)
                return;

            BulletAttributes proj = projectilePool.GetProjectile();
            proj.transform.position = spawnPos;
            AchievementState.GiveAchievement(Achievement.UseLightningWater);

            proj.Init(
                speed: 10f,
                maxRange: 20f,
                direction: player.direction,
                homing: true,
                target: nearestEnemy,
                turnSpeed: 8f
            );
            return;
        }

        /* 
            Stasis effect (frozen in ice), which disables the enemy but renders them immune to damage for the duration
            Attacking before the effect ends does not damage the enemy, but ends the effect early
        */
        if (a == ElementType.Water && b == ElementType.Air)
        {
            BulletAttributes proj = projectilePool.GetProjectile();
            proj.transform.position = spawnPos;
            AchievementState.GiveAchievement(Achievement.UseAirWater);

            proj.Init(
                speed: 10f,
                maxRange: 14f,
                direction: player.direction,
                homing: false,
                target: null,
                turnSpeed: 0f
            );

            proj.GetComponent<AoEEffectSpawner>().Setup(freezeFieldPrefab);
            return;
        }

        // Swamp roots that grow out and stop enemies, slows movement and attacks
        if (a == ElementType.Water && b == ElementType.Earth)
        {
            BulletAttributes proj = projectilePool.GetProjectile();
            proj.transform.position = spawnPos;

            AchievementState.GiveAchievement(Achievement.UseWaterEarth);
            
            proj.Init(
                speed: 8f,
                maxRange: 14f,
                direction: player.direction,
                homing: false,
                target: null,
                turnSpeed: 0f
            );

            proj.GetComponent<AoEEffectSpawner>().Setup(swampFieldPrefab);

            return;
        }

        // Longer range plinking mud ball that auto aims to the enemies
        if (a == ElementType.Air && b == ElementType.Earth)
        {
            if (nearestEnemy == null)
                return;

            BulletAttributes proj = projectilePool.GetProjectile();
            proj.transform.position = spawnPos;

            AchievementState.GiveAchievement(Achievement.UseAirEarth);

            proj.Init(
                speed: 7f,
                maxRange: 12f,
                direction: player.direction,
                homing: true,
                target: nearestEnemy,
                turnSpeed: 5f
            );
            return;
        }

        // Charging ability that has you running into enemies to bash through them
        // Duration here is for the time it takes to ramp up
        if (a == ElementType.Air && b == ElementType.Lightning)
        {
            var charge = Instantiate(chargePrefab).GetComponent<ChargeMovement>();
            AchievementState.GiveAchievement(Achievement.UseLightningAir);

            charge.Init(player);
            return;
        }

        /*
            Projectile launched out in front of you that black holes / magnetizes bullets to it
            Once projectile absorbed, sends it back towards enemy through walls
        */
        if (a == ElementType.Earth && b == ElementType.Lightning)
        {
            Vector3 offset = new Vector3(player.direction * 5f, 0f, 0f);
            Vector3 spawnPosition = player.transform.position + offset;

            Instantiate(blackHolePrefab, spawnPosition, Quaternion.identity);
            AchievementState.GiveAchievement(Achievement.UseLightningEarth);

            return;
        }
    }

    private void ExecuteFireFire()
    {
        if (!updraftPrefab || !player)
            return;

        Vector3 spawnPos = player.transform.position + new Vector3(player.direction, 0, 0);

        if (activeUpdraftInstance == null)
            activeUpdraftInstance = Instantiate(updraftPrefab, spawnPos, Quaternion.identity);
        else
        {
            activeUpdraftInstance.transform.position = spawnPos;
            activeUpdraftInstance.SetActive(true);
        }

        CancelInvoke(nameof(DisableUpdraft));
        Invoke(nameof(DisableUpdraft), updraftDuration);
        AchievementState.GiveAchievement(Achievement.UseFireFire);
    }

    private void DisableUpdraft()
    {
        if (activeUpdraftInstance)
        {
            activeUpdraftInstance.SetActive(false);
        }
    }

    private void ExecuteWaterWater() {/* Please Xavier we need the map ;_; */}
    private void ExecuteAirAir()
    {
        var pos = player.transform.position;
        var groundCheck = Physics2D.Linecast(pos + new Vector3(-0.3f, -1.05f, 0), pos + new Vector3(0.3f, -1.05f, 0));
        bool isGrounded = groundCheck && groundCheck.transform.CompareTag("Wall");

        if (isGrounded)
        {
            return;
        }
        Vector2 spawnPos = new Vector2(
            pos.x + 2 * player.direction,
            pos.y - 2f
        );

        Instantiate(airPlatformPrefab, spawnPos, Quaternion.identity);
        AchievementState.GiveAchievement(Achievement.UseAirAir);
    }

    private void ExecuteEarthEarth()
    {
        var pos = player.transform.position;
        var groundCheck = Physics2D.Linecast(pos + new Vector3(-0.3f, -1.05f, 0), pos + new Vector3(0.3f, -1.05f, 0));
        bool isGrounded = groundCheck && groundCheck.transform.CompareTag("Wall");

        if (!isGrounded)
        {
            return;
        }

        Vector2 spawnPos = new Vector2(pos.x + 5 * player.direction, pos.y - 3f);

        Instantiate(earthRampPrefab, spawnPos, Quaternion.identity);
        AchievementState.GiveAchievement(Achievement.UseEarthEarth);
    }

    private void ExecuteLightningLightning()
    {
        player.ApplySpeedBoost(4f, 3f);
        AchievementState.GiveAchievement(Achievement.UseLightningLightning);
    }

    private Transform FindNearestEnemy()
    {
        // placeholder, still need enemies :(
        return null;
    }
}
