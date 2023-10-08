using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AIStateRangeAttack : MonoBehaviour, IAIState
{
    public AIStates StateName => AIStates.RangeAttack;

    public AIStateManager AIStateManager { get => stateManager; }
    private AIStateManager stateManager;

    [Header("Rig")]
    public TwoBoneIKConstraint RightHandAimConstraint;
    public float WeightTime = 2f;

    [Header("Attack Zone")]
    public GameObject InteractionZoneGimble;
    public Transform witchPivotTransform;
    public float AttackTime = 3f;
    private Vector3 initialScale;
    private float InitialScale = 0.1f;
    private float MaxScale = 5f;

    [Header("Actual Attack")]
    public GameObject AttackObject;
    public float Speed;

    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
        initialScale = InteractionZoneGimble.transform.localScale;
    }

    public void EnterState()
    {
        InteractionZoneGimble.SetActive(true);
        stateManager.Movement.SetWalkPoint(stateManager.Player.position);
        StartCoroutine(ScaleZone());
    }

    public void ExitState()
    {
        InteractionZoneGimble.SetActive(false);
    }

    public void UpdateState()
    {
        stateManager.Watch(stateManager.Player.position);
    }

    private IEnumerator ScaleZone()
    {
        GameObject magicProjectile = Instantiate(AttackObject, InteractionZoneGimble.transform.position, InteractionZoneGimble.transform.localRotation);
        TrailRenderer trailRenderer = magicProjectile.GetComponent<TrailRenderer>();

        float attackTimeDelta = 0f;
        float rigTimeDelta = 0f;

        while (attackTimeDelta <= AttackTime)
        {
            attackTimeDelta += Time.deltaTime;
            rigTimeDelta += Time.deltaTime;

            RightHandAimConstraint.weight = rigTimeDelta / WeightTime;
            Tracking(magicProjectile);

            // Scale the Zone and Projectile
            float scaleValue = Mathf.Lerp(InitialScale, MaxScale, attackTimeDelta / AttackTime);
            InteractionZoneGimble.transform.localScale = new Vector3(scaleValue, initialScale.y, scaleValue);
            float magicProjectileScale = scaleValue / 2f;
            magicProjectile.transform.localScale = new Vector3(magicProjectileScale, magicProjectileScale, magicProjectileScale);
            
            // Set Projectile Trail
            trailRenderer.startWidth = magicProjectileScale;
            yield return null;
        }

        // Shoot Projectile towards the player direction
        Vector3 directionToPlayer = (stateManager.Player.position - witchPivotTransform.position).normalized;
        StartCoroutine(ShootMagic(magicProjectile, directionToPlayer));
    }

    private IEnumerator ShootMagic(GameObject magicProjectile, Vector3 direction)
    {
        // Assuming the projectile won't live forever, we'll add a limit to its lifetime.
        float maxProjectileLifetime = 5f;
        float projectileElapsedTime = 0f;
        MagicProjectile projectile = magicProjectile.GetComponent<MagicProjectile>();

        Debug.Log("Starting Attack");
        while (projectileElapsedTime < maxProjectileLifetime)
        {
            float moveDistance = Speed * Time.deltaTime;

            // Check for collisions with player
            if (projectile.CollidedWithPlayer)
            {
                TrailRenderer trailRenderer = magicProjectile.GetComponent<TrailRenderer>();
                trailRenderer.enabled = false;
                StartCoroutine(ScaleUp(magicProjectile));
                yield break;
            }

            // Move the projectile.
            magicProjectile.transform.position += direction * moveDistance;
            projectileElapsedTime += Time.deltaTime;
            yield return null;
        }

        // Nothing hit!
        stateManager.TransitionToState(AIStates.IgnorePlayerIdle);
        Destroy(magicProjectile);
    }

    IEnumerator ScaleUp(GameObject magicProjectile)
    {
        float deltaTime = 0;
        float maxTime = 0.2f;
        Vector3 startScale = magicProjectile.transform.localScale;
        while (deltaTime <= maxTime)
        {
            stateManager.Player.transform.position = magicProjectile.transform.position;

            deltaTime += Time.deltaTime;
            magicProjectile.transform.localScale = Vector3.Lerp(startScale, new Vector3(10, 10, 10), deltaTime / maxTime);
            yield return null;
        }

        StartCoroutine(KeepPlayerAndMove(magicProjectile));
    }

    IEnumerator KeepPlayerAndMove(GameObject magicProjectile)
    {
        float deltaTime = 0;
        float maxTime = 5f;
        Vector3 startPosition = magicProjectile.transform.position;
        while (deltaTime <= maxTime)
        {
            // Move player upwards with the Projectile
            deltaTime += Time.deltaTime;
            magicProjectile.transform.position = Vector3.Lerp(startPosition, new Vector3(startPosition.x, startPosition.y + 7f, startPosition.z), deltaTime / maxTime);
            stateManager.Player.transform.position = magicProjectile.transform.position;
            yield return null;
        }

        // Hold player.
        deltaTime = 0;
        while (deltaTime <= maxTime)
        {
            deltaTime += Time.deltaTime;
            stateManager.Player.transform.position = magicProjectile.transform.position;
            yield return null;
        }
        StateManager.Instance.RespawnPlayerEvent.Invoke();
        Destroy(magicProjectile);
        stateManager.TransitionToState(AIStates.IgnorePlayerIdle);
    }

    private void Tracking(GameObject magicProjectile)
    {
        InteractionZoneGimble.transform.position = stateManager.Player.position;
        magicProjectile.transform.position = witchPivotTransform.position;
    }
}
