using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class AIVision : MonoBehaviour
{
    public GameObject ViewCone;
    public ScriptableRendererFeature ScriptableRenderer;
    public Material BlitMaterial;
    public GameObject CurrentWatchTarget;
    public GameObject RaycastPoint;

    // Player watching
    private Transform Player;
    bool IsHuntingPlayer = false;
    bool IsWatchingLastSeenSpot = false;
    bool PlayerIsVisible = false;
    public float AlertTime = 0.5f;
    private float alertTimer = 0f;
    public float LosingTime = 1.0f;
    private float losingTimer = 0f;

    // Point Watching
    public Transform currentWatchTarget { get; set; }
    private Vector3 SmoothVelocity = Vector3.zero;
    private Vector3 smoothingPosition;
    public float SmoothTime = 0.3f;
    public float HuntSmoothTime = 0.1f;
    private float currentSmoothTime = 0f;

    private AIStateManager aiStateManager;
    public Slider Slider;

    public UnityAction HasLostPlayer;
    public UnityAction HasFoundPlayer;

    // Start is called before the first frame update
    void Start()
    {
        aiStateManager = GetComponent<AIStateManager>();
        Player = aiStateManager.Player;
        Watch(aiStateManager.WatchPoints[0]);
        RelaxedWatching();
        smoothingPosition = currentWatchTarget.position;
        ScriptableRenderer.SetActive(false);
    }

    void FixedUpdate()
    {
        PlayerIsVisible = PlayerVisible();
    }

    public void WatchSpot()
    {
        Debug.DrawLine(ViewCone.transform.position, currentWatchTarget.position, new Color(0.1f, 0.1f, 0.1f));
        smoothingPosition = Vector3.SmoothDamp(smoothingPosition, currentWatchTarget.position, ref SmoothVelocity, currentSmoothTime);
        Vector3 relativeSmoothingPosition = smoothingPosition - ViewCone.transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativeSmoothingPosition, ViewCone.transform.up);
        ViewCone.transform.rotation = rotation;
        CurrentWatchTarget.transform.position = smoothingPosition;

        HasJustFoundPlayer();
        HasJustLostPlayer();
    }

    public void PlayerWatching()
    {
        currentSmoothTime = HuntSmoothTime;
    }

    public void RelaxedWatching()
    {
        currentSmoothTime = SmoothTime;
    }

    public void Watch(Transform point)
    {
        currentWatchTarget = point;
    }

    private bool PlayerVisible()
    {
        if (!StateManager.Instance.WitchConeOnPlayer)
            return false;

        // Only Look at Prop and Player!
        LayerMask layerMask = LayerMask.GetMask("Prop");
        layerMask |= LayerMask.GetMask("Player");
        layerMask |= LayerMask.GetMask("Interactable");

        if (Physics.Linecast(ViewCone.transform.position, RaycastPoint.transform.position, out RaycastHit hitInfo, layerMask))
        {
            Debug.DrawLine(ViewCone.transform.position, hitInfo.transform.position, Color.green);

            if (hitInfo.transform.CompareTag("Player"))
            {
                Debug.DrawLine(ViewCone.transform.position, Player.transform.position, Color.magenta);
                return true;
            }
        }

        return false;
    }

    private bool HasJustFoundPlayer()
    {
        if (PlayerIsVisible && !IsHuntingPlayer)
        {
            alertTimer += Time.deltaTime;
            if (alertTimer >= AlertTime)
            {
                HasFoundPlayer.Invoke();
                alertTimer = 0f;
                losingTimer = 0f;
                Slider.value = Slider.maxValue;
                IsHuntingPlayer = true;
                ScriptableRenderer.SetActive(true);
                StartCoroutine(LerpBlit(0.2f, 2f, true));
                return true;
            }
        }

        return false;
    }

    private bool HasJustLostPlayer()
    {
        if (!PlayerIsVisible && IsHuntingPlayer)
        {
            losingTimer += Time.deltaTime;
            if (losingTimer >= LosingTime)
            {
                HasLostPlayer.Invoke();
                alertTimer = 0f;
                losingTimer = 0f;
                IsHuntingPlayer = false;
                StartCoroutine(LerpBlit(0f, 2f, false));
                return true;
            }
        }

        return false;
    }

    IEnumerator LerpBlit(float toLerpTo, float time, bool activate)
    {
        // Activate if necessary
        if (activate)
            ScriptableRenderer.SetActive(true);

        // Lerp the Transparency to b
        float a = BlitMaterial.GetFloat("_Transparency");
        float delta = 0f;
        while (delta < time)
        {
            delta += Time.deltaTime;
            float val = Mathf.Lerp(a, toLerpTo, delta / time);
            BlitMaterial.SetFloat("_Transparency", val);
            yield return null;
        }

        // Deactivate if necessary
        if (!activate)
            ScriptableRenderer.SetActive(false);
    }

    private void OnDisable()
    {
        ScriptableRenderer.SetActive(false);
        BlitMaterial.SetFloat("_Transparency", 0);
    }
}
