using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    private static QuestManager _instance;

    // Singleton
    public static QuestManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<QuestManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    _instance = obj.AddComponent<QuestManager>();
                }
            }
            return _instance;
        }
    }

    private List<Quest> Quests = new List<Quest>();
    private Quest ActiveQuest;
    public Image QuestImage;
    public Image ArrowIcon;

    public List<Quest> ActiveQuests { get; private set; } = new List<Quest>();
    public List<Quest> CompletedQuests { get; private set; } = new List<Quest>();
    public RectTransform TrackingArea;
    private Camera mainCamera;

    private void Awake()
    {
        // Singleton per scene
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
        {
            _instance = this;
            mainCamera = Camera.main;
        }
    }

    public void AddQuest(Quest quest)
    {
        Quests.Add(quest);

        if (Quests.Count == 1)
            ActiveQuest = quest;
    }

    public void Update()
    {
        SetQuestIconPositionOnScreen();
    }


    public void MarkQuestAsCompleted()
    {
        ActiveQuest.MarkAsCompleted();
    }

    private void UpdateCurrentQuestIcon()
    {
        if (ActiveQuest == null)
            return;

        Vector3 directionToQuest = (ActiveQuest.transform.position - Camera.main.transform.position).normalized;

        // Get the angle on the horizontal plane (to determine left or right)
        float horizontalAngle = Vector3.SignedAngle(Camera.main.transform.forward, directionToQuest, Camera.main.transform.up);

        // Get the angle on the vertical plane (to determine up or down)
        float verticalAngle = Vector3.SignedAngle(Camera.main.transform.forward, directionToQuest, Camera.main.transform.right);

        // Convert angles to a range between -1 and 1
        float normalizedHorizontal = Mathf.Sin(horizontalAngle * Mathf.Deg2Rad);
        float normalizedVertical = Mathf.Sin(verticalAngle * Mathf.Deg2Rad);

        // Determine the position on the boundary of the circle
        Vector2 positionOnCircleBoundary;
        float magnitude = Mathf.Sqrt(normalizedHorizontal * normalizedHorizontal + normalizedVertical * normalizedVertical);
        if (magnitude > 1)
        {
            positionOnCircleBoundary = new Vector2(normalizedHorizontal, normalizedVertical) / magnitude;
        }
        else
        {
            positionOnCircleBoundary = new Vector2(normalizedHorizontal, normalizedVertical);
        }

        // Convert to screen space
        Vector3 screenPosition = new Vector3(
            positionOnCircleBoundary.x * Screen.width * 0.5f + Screen.width * 0.5f,
            positionOnCircleBoundary.y * Screen.height * 0.5f + Screen.height * 0.5f,
            0
        );

        // Update the icon position
        QuestImage.rectTransform.position = screenPosition;
    }

    private void SetQuestIconPositionOnScreen()
    {
        // Convert the world position of the quest to viewport position.
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(ActiveQuest.transform.position);

        if (viewportPosition.z > 0f && viewportPosition.z < 3f)
            QuestImage.color = new Color(QuestImage.color.r, QuestImage.color.g, QuestImage.color.b, viewportPosition.z);
        if (viewportPosition.z < 0f && viewportPosition.z > -3f)
            ArrowIcon.color = new Color(ArrowIcon.color.r, ArrowIcon.color.g, ArrowIcon.color.b, Mathf.Abs(viewportPosition.z));

        // Check if the quest is behind the camera.
        if (viewportPosition.z < 0)
        {


            Vector2 directionFromCenterToQuest = new Vector2(0.5f - viewportPosition.x, 0.5f - viewportPosition.y).normalized;

            // This clamps the arrow to the bottom instead of the top
            //if (directionFromCenterToQuest.y > 0)
            //    directionFromCenterToQuest.y = -directionFromCenterToQuest.y;

            // Calculate the angle to rotate the arrow (Arrow pointing Up)
            float angle = Mathf.Atan2(directionFromCenterToQuest.y, directionFromCenterToQuest.x) * Mathf.Rad2Deg - 90f;

            ArrowIcon.rectTransform.rotation = Quaternion.Euler(0, 0, angle); // Apply the rotation

            // Calculate arrow's position on the boundary of the TrackingArea
            float boundaryFactor = Mathf.Min(
                TrackingArea.rect.width * 0.5f / Mathf.Abs(directionFromCenterToQuest.x),
                TrackingArea.rect.height * 0.5f / Mathf.Abs(directionFromCenterToQuest.y)
            );

            float arrowX = directionFromCenterToQuest.x * boundaryFactor;
            float arrowY = directionFromCenterToQuest.y * boundaryFactor;

            ArrowIcon.rectTransform.anchoredPosition = new Vector2(arrowX, arrowY);

            ArrowIcon.enabled = true;
            QuestImage.enabled = false;
            return;
        }
        else
        {
            ArrowIcon.enabled = false;
            QuestImage.enabled = true;
        }

        // Calculate the x and y position for the icon based on the tracking area size.
        float iconX = (viewportPosition.x - 0.5f) * TrackingArea.rect.width;
        float iconY = (viewportPosition.y - 0.5f) * TrackingArea.rect.height;

        // Check if the icon position is outside the boundary of the TrackingArea
        if (Mathf.Abs(iconX) > TrackingArea.rect.width * 0.5f || Mathf.Abs(iconY) > TrackingArea.rect.height * 0.5f)
        {
            Vector2 directionFromCenter = new Vector2(iconX, iconY).normalized;
            float boundaryFactor = Mathf.Min(
                TrackingArea.rect.width * 0.5f / Mathf.Abs(directionFromCenter.x),
                TrackingArea.rect.height * 0.5f / Mathf.Abs(directionFromCenter.y)
            );

            iconX = directionFromCenter.x * boundaryFactor;
            iconY = directionFromCenter.y * boundaryFactor;
        }

        QuestImage.rectTransform.anchoredPosition = new Vector2(iconX, iconY);
    }

}
