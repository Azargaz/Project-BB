using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestboardController : MonoBehaviour
{
    GameObject player;
    public GameObject prompt;
    public GameObject quest;
    public bool questsRolled;
    List<GameObject> quests = new List<GameObject>();
    public int numberOfQuests = 3;

    public static QuestboardController QBC;

    void Start()
    {
        QBC = this;
    }

    void RollQuests()
    {
        List<int> availableIds = new List<int>();

        for (int i = 0; i < QuestLogController.QL.allQuests.Length; i++)
        {
            if (!QuestLogController.QL.activeQuests.Contains(QuestLogController.QL.allQuests[i]))
            {
                availableIds.Add(QuestLogController.QL.allQuests[i].questLogId);
            }
        }

        for (int i = 0; i < numberOfQuests; i++)
        {
            if (availableIds.Count <= 0)
                break;

            int questLogId = 0;
            int randomId = Random.Range(0, availableIds.Count);
            questLogId = availableIds[randomId];
            availableIds.RemoveAt(randomId);

            GameObject clone = Instantiate(quest, GameManager.instance.questboard.transform.FindChild("QuestsSpace"));
            clone.GetComponent<QuestController>().quest = QuestLogController.QL.allQuests[questLogId];
            quests.Add(clone);
        }
    }

    void Update()
    {
        if (player != null && !GameManager.instance.pause)
        {
            if (Input.GetButtonDown("Interact") && !player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack_player"))
            {
                OpenQuestboard();

                if(!questsRolled)
                {
                    RollQuests();
                    questsRolled = true;
                }
            }
        }
    }

    void OpenQuestboard()
    {
        GameManager.instance.OpenQuestboard();
    }

    public void ClearQuestboard()
    {
        for (int i = 0; i < quests.Count; i++)
        {
            Destroy(quests[i]);
        }

        prompt.SetActive(false);
        Destroy(this);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 8)
        {
            player = other.gameObject;
            prompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 8)
        {
            player = null;
            prompt.SetActive(false);
        }
    }
}
