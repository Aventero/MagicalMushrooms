using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Quest UI")]
    public Image QuestImage;
    public Image ArrowIcon;
    public RectTransform TrackingArea;
    private Camera mainCamera;

    [Header("Quests")]
    private Quest VisibleQuest; 
    private List<Quest> AllQuests = new();
    public List<Quest> InCompletedQuests { get; private set; } = new List<Quest>();

    [Header("Settings")]
    public float maxQuestTrackingDistance = 100f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            mainCamera = Camera.main;
        }
    }

    public void AddQuest(Quest quest)
    {
        AllQuests.Add(quest);
        InCompletedQuests.Add(quest);
    }

    private void Update()
    {
        FindClosestQuestInRange();

        if (VisibleQuest == null)
            return;

        SetQuestIconPositionOnScreen();
    }
    private void FindClosestQuestInRange()
    {
        Quest closestQuest = null;
        float closestDistanceSqr = maxQuestTrackingDistance * maxQuestTrackingDistance;

        foreach (Quest quest in InCompletedQuests)
        {
            float distanceSqr = (quest.transform.position - mainCamera.transform.position).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestQuest = quest;
            }
        }

        if (closestQuest != null)
            VisibleQuest = closestQuest; 
        else
        {
            VisibleQuest = null;
            Debug.Log("No quest in range");
            ArrowIcon.enabled = false;
            QuestImage.enabled = false;
        }
    }

    public void RemoveQuest(Quest quest)
    {
        InCompletedQuests.Remove(quest);
        if (VisibleQuest == quest)
            VisibleQuest = null;
    }

    public void CompletedQuest(Quest quest)
    {
        quest.CompletedQuest();
    }

    private void SetQuestIconPositionOnScreen()
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(VisibleQuest.transform.position);

        float centerX = 0.5f;
        float centerY = 0.5f;

        float angleToQuest = Mathf.Atan2(viewportPosition.y - centerY, viewportPosition.x - centerX);

        // Quest is behind the player
        if (viewportPosition.z < 0) 
        {
            angleToQuest += Mathf.PI;
            float arrowX = Mathf.Cos(angleToQuest) * (TrackingArea.rect.width * 0.5f);
            float arrowY = Mathf.Sin(angleToQuest) * (TrackingArea.rect.height * 0.5f);

            ArrowIcon.rectTransform.anchoredPosition = new Vector2(arrowX, arrowY);
            ArrowIcon.rectTransform.rotation = Quaternion.Euler(0, 0, angleToQuest * Mathf.Rad2Deg - 90f);

            ArrowIcon.enabled = true;
            QuestImage.enabled = false;
            return;
        }

        float distanceFromCenter = Mathf.Sqrt((viewportPosition.x - centerX) * (viewportPosition.x - centerX) +
                                              (viewportPosition.y - centerY) * (viewportPosition.y - centerY));

        float iconX;
        float iconY;
        if (distanceFromCenter > 0.5f) 
        {
            // Quest icon outside the screen
            iconX = centerX + Mathf.Cos(angleToQuest) * (TrackingArea.rect.width * 0.5f);
            iconY = centerY + Mathf.Sin(angleToQuest) * (TrackingArea.rect.height * 0.5f);
        }
        else 
        {
            // Quest icon is inside the screen
            iconX = (viewportPosition.x - centerX) * TrackingArea.rect.width;
            iconY = (viewportPosition.y - centerY) * TrackingArea.rect.height;
        }

        QuestImage.rectTransform.anchoredPosition = new Vector2(iconX, iconY);
        QuestImage.enabled = true;
        ArrowIcon.enabled = false;
    }
}
