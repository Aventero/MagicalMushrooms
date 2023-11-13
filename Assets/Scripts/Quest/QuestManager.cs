using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using static Quest;
using static UnityEngine.EventSystems.EventTrigger;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Quest UI")]
    public Image QuestImage;
    public Image CollectibleImage;
    public Image QuestArrowIcon; 
    public Image CollectibleArrowIcon; 
    public RectTransform TrackingArea;
    private Camera mainCamera;

    [Header("Quests")]
    private Quest VisibleQuest; // Closest 'ReachDestination' quest
    private Quest VisibleCollectible; // Closest 'Collectible' quest
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
        FindClosestQuestsInRange();

        if (VisibleQuest != null)
            SetQuestIconPositionOnScreen(VisibleQuest, QuestImage, QuestArrowIcon);
        else
            QuestImage.enabled = false;

        if (VisibleCollectible != null)
            SetQuestIconPositionOnScreen(VisibleCollectible, CollectibleImage, CollectibleArrowIcon);
        else
            CollectibleImage.enabled = false;
    }

    private void FindClosestQuestsInRange()
    {
        VisibleQuest = FindClosestQuestByType(QuestType.ReachDestination);
        VisibleCollectible = FindClosestQuestByType(QuestType.Collect);
    }

    private Quest FindClosestQuestByType(QuestType questType)
    {
        Quest closestQuest = null;
        float closestDistanceSqr = maxQuestTrackingDistance * maxQuestTrackingDistance;

        foreach (Quest quest in InCompletedQuests)
        {
            if (quest.Type != questType)
                continue;

            float distanceSqr = (quest.transform.position - mainCamera.transform.position).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestQuest = quest;
            }
        }

        return closestQuest;
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

    private void SetQuestIconPositionOnScreen(Quest quest, Image icon, Image arrowIcon)
    {
        UpdateIconPosition(icon, quest.transform.position, arrowIcon);
    }

    private void UpdateIconPosition(Image icon, Vector3 position, Image correspondingArrowIcon)
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(position);

        float centerX = 0.5f;
        float centerY = 0.5f;

        float angleToQuest = Mathf.Atan2(viewportPosition.y - centerY, viewportPosition.x - centerX);

        // Quest is behind the player
        if (viewportPosition.z < 0)
        {
            angleToQuest += Mathf.PI;
            float arrowX = Mathf.Cos(angleToQuest) * (TrackingArea.rect.width * 0.5f);
            float arrowY = Mathf.Sin(angleToQuest) * (TrackingArea.rect.height * 0.5f);

            correspondingArrowIcon.rectTransform.anchoredPosition = new Vector2(arrowX, arrowY);
            correspondingArrowIcon.rectTransform.rotation = Quaternion.Euler(0, 0, angleToQuest * Mathf.Rad2Deg - 90f);

            correspondingArrowIcon.enabled = true;
            icon.enabled = false;
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

        icon.rectTransform.anchoredPosition = new Vector2(iconX, iconY);
        icon.enabled = true;
        correspondingArrowIcon.enabled = false;
    }
}
