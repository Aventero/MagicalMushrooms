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

    private void SetQuestIconPositionOnScreen()
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(ActiveQuest.transform.position);

        float centerX = 0.5f;
        float centerY = 0.5f;

        float angleToQuest = Mathf.Atan2(viewportPosition.y - centerY, viewportPosition.x - centerX);

        // Determine transparency 
        if (viewportPosition.z > 0f && viewportPosition.z < 5f)
        {
            float alpha = viewportPosition.z / 5;
            QuestImage.color = new Color(QuestImage.color.r, QuestImage.color.g, QuestImage.color.b, alpha);
        }
        if (viewportPosition.z < 0f && viewportPosition.z > -5f)
        {
            float alpha = Mathf.Abs(viewportPosition.z) / 5;
            ArrowIcon.color = new Color(ArrowIcon.color.r, ArrowIcon.color.g, ArrowIcon.color.b, alpha);
        }

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
