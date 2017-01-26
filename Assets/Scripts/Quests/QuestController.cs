using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestController : MonoBehaviour
{
    public Quest quest;
    public Text text;
    bool onQuestboard;

    void Update()
    {
        string requirements = "\n\nRequirements:";        

        for (int i = 0; i < quest.requirements.Length; i++)
        {
            requirements += "\n" + quest.requirements[i].enemyName + "s: " + quest.requirements[i].killCount + "/" + quest.requirements[i].requiredKills;
        }


        if (quest.questState == Quest.State.available)
            requirements = "\n\nClick to choose";
        else if (quest.questState == Quest.State.completed)
            requirements = "\n\n- QUEST COMPLETED! -\nClick to collect rewards";        


        text.text = "Quest #" + quest.questLogId + "\n[" + quest.questState.ToString().ToUpper() + "]\n\n" + quest.name + "\nReward: " + quest.reward + "$"  + requirements;

       
        Button button = GetComponentInChildren<Button>();
        button.onClick.RemoveAllListeners();

        if (quest.questState == Quest.State.available)
        {
            button.onClick.AddListener(Choose);
        }
        else if (quest.questState == Quest.State.completed)
        {
            button.onClick.AddListener(CollectReward);
        }        
    }

    void Choose()
    {
        QuestboardController.QBC.ClearQuestboard();
        QuestLogController.QL.ActivateQuest(quest.questLogId);
    }

    void CollectReward()
    {
        quest.CollectReward();
        Destroy(gameObject);
    }
}
