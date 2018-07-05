using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestLogController : MonoBehaviour
{    
    public Quest[] allQuests;
    public List<Quest> activeQuests = new List<Quest>();

    public Button questlogCloseButton;
    public Button questboardCloseButton;
    Transform activeQuestsSpace;
    Transform questboardQuestsSpace;
    public GameObject quest;
    public GameObject questCompletedDisplay;

    public static QuestLogController QL;

    void Start()
    {
        QL = this;

        for (int i = 0; i < allQuests.Length; i++)
        {
            allQuests[i].questLogId = i;
        }

        activeQuestsSpace = GameManager.instance.questLog.transform.Find("QuestsSpace");
        questboardQuestsSpace = GameManager.instance.questboard.transform.Find("QuestsSpace");

        questlogCloseButton = activeQuestsSpace.parent.Find("Exit").GetComponentInChildren<Button>();
        questboardCloseButton = questboardQuestsSpace.parent.Find("Exit").GetComponentInChildren<Button>();

        if(questlogCloseButton != null)
            questlogCloseButton.onClick.AddListener(CloseQuestLog);
        if(questboardCloseButton != null)
            questboardCloseButton.onClick.AddListener(CloseQuestboard);
    }

    void Update()
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if(activeQuests[i].rewardCollected)
            {
                activeQuests[i].rewardCollected = false;
                activeQuests.RemoveAt(i);
                i--;
                continue;
            }
        }
    }

    public void AddEnemyKill(string _name)
    {
        if (activeQuests.Count <= 0)
            return;

        for (int i = 0; i < activeQuests.Count; i++)
        {
            for (int j = 0; j < activeQuests[i].requirements.Length; j++)
            {
                Quest.Requirements reqs = activeQuests[i].requirements[j];

                if (_name == reqs.enemyName)
                {
                    activeQuests[i].AddEnemyKill(_name);
                }
            }
        }
    }

    public void ActivateQuest(int id)
    {
        if (id >= allQuests.Length)
            return;

        Quest newActiveQuest = new Quest();
        newActiveQuest= allQuests[id];

        for (int i = 0; i < newActiveQuest.requirements.Length; i++)
        {
            newActiveQuest.requirements[i].killCount = 0; 
        }

        newActiveQuest.questState = Quest.State.active;

        activeQuests.Add(newActiveQuest);

        GameObject clone = Instantiate(quest, activeQuestsSpace);
        clone.GetComponent<QuestController>().quest = activeQuests[activeQuests.Count - 1];
    }

    public void ResetQuests()
    {
        activeQuests.Clear();

        for (int i = 0; i < allQuests.Length; i++)
        {
            allQuests[i].questState = Quest.State.available;

            for (int j = 0; j < allQuests[i].requirements.Length; j++)
            {
                allQuests[i].requirements[j].killCount = 0;
                allQuests[i].rewardCollected = false;
            }
        }

        foreach (Transform child in activeQuestsSpace)
        {
            Destroy(child.gameObject);
        }
    }

    public void DisplayQuestCompleted()
    {
        if(questCompletedDisplay != null)
        {
            GameObject clone = Instantiate(questCompletedDisplay, GameManager.player.transform.position, Quaternion.identity);

            clone.GetComponent<Animator>().SetTrigger("Display");
            clone.transform.SetParent(GameObject.Find("DamageNumbers").transform);
            Destroy(clone, 1f);
        }
    }

    public void CloseQuestboard()
    {
        GameManager.instance.OpenQuestboard();
    }

    public void CloseQuestLog()
    {
        GameManager.instance.OpenQuestLog();
    }
}

[System.Serializable]
public class Quest
{
    public string name;
    [HideInInspector]
    public int questLogId;

    public enum State { available, active, completed };
    public State questState;

    [HideInInspector]
    public bool rewardCollected = false;
    public int reward;
    /* TO DO LATER: ITEM REWARDS */

    public enum RequirementsType { allRequierementsNeeded, oneRequirementNeeded, oneKillCountDifferentEnemies };
    public RequirementsType requirementsType;
    public Requirements[] requirements;

    [System.Serializable]
    public class Requirements
    {
        public string enemyName;
        public int requiredKills;
        public int killCount;
    }

    public void Complete()
    {
        if (questState == State.active)
        {            
            questState = State.completed;
            QuestLogController.QL.DisplayQuestCompleted();
        }
    }

    public void CollectReward()
    {
        CurrencyController.CC.AddCurrency(reward);
        rewardCollected = true;
        questState = State.available;
    }

    public void AddEnemyKill(string _name)
    {
        if (questState != State.active)
            return;

        int requirementsCompleted = 0;
        int requiredKills = 0;
        int completedKills = 0;

        for (int i = 0; i < requirements.Length; i++)
        {
            if (requirements[i].enemyName == _name)
            {
                requirements[i].killCount++;

                if (requirements[i].killCount >= requirements[i].requiredKills)
                {
                    requirementsCompleted++;
                }

                if (requirementsType == RequirementsType.oneKillCountDifferentEnemies)
                {
                    completedKills = requirements[i].killCount;
                    requiredKills += requirements[i].requiredKills;
                }
            }
        }

        switch (requirementsType)
        {
            case RequirementsType.allRequierementsNeeded:
                {
                    if (requirementsCompleted == requirements.Length)
                        Complete();
                    break;
                }
            case RequirementsType.oneRequirementNeeded:
                {
                    if (requirementsCompleted > 0)
                        Complete();
                    break;
                }
            case RequirementsType.oneKillCountDifferentEnemies:
                {
                    if (completedKills >= requiredKills / requirements.Length)
                        Complete();
                    break;
                }
        }
    }
}

