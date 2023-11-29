using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private float initialWeight;

    [Header("Attack Zone")]
    public GameObject AttackZone;
    public float AttackTime = 1f;
    public float EndTime = 1f;
    private Vector3 initialScale;
    private float InitialScaling = 0.1f;
    private float MaxScaling = 15f;

    [Header("Actual Attack")]
    public GameObject AttackObject;
    public GameObject RunSign;
    private TMP_Text runText;
    public GameObject AttackSign;
    public float ProjectileTravelTime;


    [Header("Line Renderer")]
    [SerializeField] private LineRenderer attackLineRenderer;
    [SerializeField] private Transform witchShootOrigin; // The transform component of the witch
    private Vector3 initialPlayerPosition;
    GameObject magicProjectile;

    public void InitState(AIStateManager stateManager)
    {
        this.stateManager = stateManager;
        initialScale = AttackZone.transform.localScale;
    }

    public void EnterState()
    {
        AttackZone.SetActive(true);
        stateManager.Movement.SetWalkPoint(stateManager.Player.position);
        stateManager.DangerOverlay.SetState(DangerState.Attack);
        stateManager.ToggleWitchLocator(true);
        StartCoroutine(ScaleZone());
        initialWeight = RightHandAimConstraint.weight;
        stateManager.WarnPulse.StartPulse();
        initialPlayerPosition = stateManager.Player.position;
        attackLineRenderer.enabled = true;
        RunSign.SetActive(true);
        runText = RunSign.GetComponentInChildren<TMP_Text>();
        runText.SetText("Run!");
    }

    public void ExitState()
    {
        StopAllCoroutines();
        if (magicProjectile != null)
            Destroy(magicProjectile);

        stateManager.ToggleWitchLocator(false);
        AttackZone.SetActive(false);
        stateManager.WarnPulse.StopPulse();
        attackLineRenderer.enabled = false;
        RunSign.SetActive(false);
    }

    public void UpdateState()
    {
        if (!AudioManager.Instance.IsPlaying("heartBeatFastest"))
            AudioManager.Instance.Play("heartBeatFastest");


        stateManager.Watch(stateManager.Player.position);
        attackLineRenderer.SetPosition(0, witchShootOrigin.position);
        attackLineRenderer.SetPosition(1, initialPlayerPosition);
    }

    private IEnumerator ScaleZone()
    {

        magicProjectile = Instantiate(AttackObject, AttackZone.transform.position, AttackZone.transform.localRotation);
        TrailRenderer trailRenderer = magicProjectile.GetComponent<TrailRenderer>();

        float attackTimeDelta = 0f;
        float rigTimeDelta = 0f;

        while (attackTimeDelta <= AttackTime)
        {
            attackTimeDelta += Time.deltaTime;
            rigTimeDelta += Time.deltaTime;

            // Rig
            RightHandAimConstraint.weight = Mathf.Lerp(initialWeight, 1f, rigTimeDelta / WeightTime);

            // Zone
            float scaleValue = Mathf.Lerp(InitialScaling, MaxScaling, attackTimeDelta / AttackTime);
            AttackZone.transform.localScale = new Vector3(scaleValue, initialScale.y, scaleValue);
            AttackZone.transform.position = initialPlayerPosition;

            // Projectile
            float magicProjectileScale = scaleValue / 3f;
            magicProjectile.transform.position = witchShootOrigin.position;
            magicProjectile.transform.localScale = new Vector3(magicProjectileScale, magicProjectileScale, magicProjectileScale);
            trailRenderer.startWidth = magicProjectileScale;
            yield return null;
        }

        // Shoot Projectile towards the player direction
        StartCoroutine(ShootMagicProjectile());
    }

    private IEnumerator ShootMagicProjectile()
    {
        // Assuming the projectile won't live forever, we'll add a limit to its lifetime.
        MagicProjectile projectile = magicProjectile.GetComponent<MagicProjectile>();

        float delta = 0;
        while (delta < ProjectileTravelTime)
        {
            // Check for collisions with player
            if (projectile.CollidedWithPlayer)
            {
                runText.SetText("Caught!");
                AudioManager.Instance.UpdateVolume("heartBeatFastest", 0.2f);
                TrailRenderer trailRenderer = magicProjectile.GetComponent<TrailRenderer>();
                trailRenderer.enabled = false;
                attackLineRenderer.enabled = false;
                StartCoroutine(ScaleUpProjectile());
                yield break;
            }

            // Move the projectile.
            magicProjectile.transform.position = Vector3.Lerp(witchShootOrigin.position, initialPlayerPosition, delta / ProjectileTravelTime);
            delta += Time.deltaTime;
            yield return null;
        }

        runText.SetText("Escaped!");
        delta = 0;
        float scaleDownTime = 1f;
        Vector3 initialScale = magicProjectile.transform.localScale;
        while (delta < scaleDownTime)
        {
            magicProjectile.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, delta / scaleDownTime);
            delta += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(ScaleDownZone());
        attackLineRenderer.enabled = false;

        // Nothing hit!
        AudioManager.Instance.UpdateVolume("heartBeatFastest", 0.02f);
        float deltaTime = 0;
        while (deltaTime <= EndTime)
        {
            // Move arm back down
            deltaTime += Time.deltaTime;
            RightHandAimConstraint.weight = Mathf.Lerp(1f, initialWeight, deltaTime / 1f);
            yield return null;
        }

        stateManager.TransitionToState(AIStates.Idle);
        Destroy(magicProjectile);
        magicProjectile = null;
    }

    IEnumerator ScaleUpProjectile()
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

        StartCoroutine(KeepPlayerAndMove());
        StartCoroutine(ScaleDownZone());
    }

    IEnumerator KeepPlayerAndMove()
    {
        float deltaTime = 0;
        float maxTime = 3f;
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
        while (deltaTime <= maxTime / 2f)
        {
            deltaTime += Time.deltaTime;
            RightHandAimConstraint.weight = Mathf.Lerp(1f, initialWeight, deltaTime / maxTime);
            stateManager.Player.transform.position = magicProjectile.transform.position;
            yield return null;
        }
        StateManager.Instance.PlayerWasCaughtEvent.Invoke();

        // Hold player.
        deltaTime = 0;
        while (deltaTime <= maxTime / 2f)
        {
            deltaTime += Time.deltaTime;
            RightHandAimConstraint.weight = Mathf.Lerp(1f, initialWeight, deltaTime / maxTime);
            stateManager.Player.transform.position = magicProjectile.transform.position;
            yield return null;
        }
        Destroy(magicProjectile);
        magicProjectile = null;
        stateManager.TransitionToState(AIStates.IgnorePlayerIdle);
    }


    IEnumerator ScaleDownZone()
    {
        float scale = AttackZone.transform.localScale.x;
        float deltaTime = 0f;
        float maxTime = EndTime / 2f;
        while (deltaTime <= maxTime)
        {
            deltaTime += Time.deltaTime;
            
            // Scale the Zone and Projectile
            float scaleValue = Mathf.Lerp(scale, 0, deltaTime / maxTime);
            AttackZone.transform.localScale = new Vector3(scaleValue, initialScale.y, scaleValue);
            yield return null;
        }
    }
}
