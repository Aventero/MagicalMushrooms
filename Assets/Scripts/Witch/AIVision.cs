using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class AIVision : MonoBehaviour
{
    public GameObject ViewCone;
    public ScriptableRendererFeature ScriptableRenderer;
    public Material BlitMaterial;

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

    private AIStateManager aiStateManager;
    public Slider Slider;

    // Start is called before the first frame update
    void Start()
    {
        aiStateManager = GetComponent<AIStateManager>();
        Player = aiStateManager.Player;
        currentWatchTarget = aiStateManager.WatchPoints[0];
        smoothingPosition = currentWatchTarget.position;
        ScriptableRenderer.SetActive(false);
    }

    void FixedUpdate()
    {
        PlayerIsVisible = PlayerVisible();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void WatchSpot()
    {
        Debug.DrawLine(ViewCone.transform.position, currentWatchTarget.position, new Color(0.1f, 0.1f, 0.1f));
        smoothingPosition = Vector3.SmoothDamp(smoothingPosition, currentWatchTarget.position, ref SmoothVelocity, SmoothTime);
        Vector3 relativeSmoothingPosition = smoothingPosition - ViewCone.transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativeSmoothingPosition, ViewCone.transform.up);
        ViewCone.transform.rotation = rotation;
    }

    public void Watch(Transform point)
    {
        currentWatchTarget = point;
    }

    private bool PlayerVisible()
    {
        if (!StateManager.Instance.WitchConeOnPlayer)
            return false;

        if (Physics.Linecast(ViewCone.transform.position, Player.transform.position, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.CompareTag("Player"))
            {
                Debug.DrawLine(ViewCone.transform.position, Player.transform.position, Color.magenta);
                return true;
            }
        }

        return false;
    }

    public bool HasJustFoundPlayer()
    {
        if (PlayerIsVisible && !IsHuntingPlayer)
        {
            alertTimer += Time.deltaTime;
            if (alertTimer >= AlertTime)
            {
                Slider.value = Slider.maxValue;
                IsHuntingPlayer = true;
                ScriptableRenderer.SetActive(true);
                StartCoroutine(LerpBlit(0.2f, 2f, true));
                return true;
            }
        }

        return false;
    }

    public bool HasJustLostPlayer()
    {
        if (!PlayerIsVisible && IsHuntingPlayer)
        {
            losingTimer += Time.deltaTime;
            if (losingTimer >= LosingTime)
            {
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
