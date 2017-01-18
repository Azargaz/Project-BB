using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class Passive : MonoBehaviour
{
    public PassiveStats passive = new PassiveStats();

    [HideInInspector]
    public Player playerController;
    [HideInInspector]
    public PlayerCreature playerCreature;
    [HideInInspector]
    public PassiveTreeController passiveController;

    [HideInInspector]
    public Button button;
    [HideInInspector]
    public Text text;
    [HideInInspector]
    public Image img;
    [HideInInspector]
    public GameObject lineRenderer;
    [HideInInspector]
    public LineRenderer[] line;
    [HideInInspector]
    public List<Vector3[]> linePositions = new List<Vector3[]>();

    void Start()
    {
        passive.activationMethods = new PassiveStats.Activates[] { Dash, Test };
        GameManager.passives.Add(this);

        #region Required Passives

        passive.monobehaviourPassive = this;

        if (passive.requiredPassivesPassiveObject.Length > 0)
        {
            for (int r = 0; r < passive.requiredPassivesPassiveObject.Length; r++)
            {
                PassiveStats.RequiredPassives requiredPassive = passive.requiredPassives[r];
                passive.requiredPassivesPassiveObject[r] = passiveController.passiveBranches[requiredPassive.branchID].passives[requiredPassive.passiveID].monobehaviourPassive;
            }
        }

        #endregion

        #region Lines

        line = new LineRenderer[passive.requiredPassives.Length];

        if (passive.requiredPassives.Length > 0 && lineRenderer != null)
        {
            for (int i = 0; i < passive.requiredPassives.Length; i++)
            {
                line[i] = Instantiate(lineRenderer, transform).GetComponent<LineRenderer>();
                line[i].numPositions = 2;
                linePositions.Add(new Vector3[2]);
            }
        }

        #endregion

        #region GETs

        button = GetComponentInChildren<Button>();
        text = GetComponentInChildren<Text>();
        img = button.GetComponent<Image>();
        img.sprite = passive.sprite;
        button.onClick.AddListener(passive.Activate);

        if (button == null)
            Debug.LogError("No button element in children.");
        if (text == null)
            Debug.LogError("No text element in children.");
        if (img == null)
            Debug.LogError("No image element in button.");

        #endregion

        playerController = GameManager.player.GetComponent<Player>();
        playerCreature = GameManager.player.GetComponent<PlayerCreature>();
    }

    void Update()
    {
        if (playerController == null)
            playerController = GameManager.player.GetComponent<Player>();

        #region Lines && Required passives

        if (passive.requiredPassivesPassiveObject.Length > 0)
        {
            for (int i = 0; i < passive.requiredPassivesPassiveObject.Length; i++)
            {
                if (line.Length > 0 && linePositions.Count > 0 && lineRenderer != null)
                {
                    Vector3 rPassivePos = Camera.main.ScreenToWorldPoint(passive.requiredPassivesPassiveObject[i].transform.position);
                    linePositions[i][1] = new Vector3(rPassivePos.x, rPassivePos.y, -10);
                }

                if (passive.requiredPassivesPassiveObject[i].passive.state != PassiveStats.State.Active)
                {
                    passive.state = PassiveStats.State.Disabled;
                }
                else if (passive.requiredPassivesPassiveObject[i].passive.state == PassiveStats.State.Active && passive.state != PassiveStats.State.Active)
                {
                    passive.state = PassiveStats.State.Enabled;
                }
            }
        }

        if (line.Length > 0 && linePositions.Count > 0 && lineRenderer != null)
        {
            for (int i = 0; i < line.Length; i++)
            {
                Vector3 passivePos = Camera.main.ScreenToWorldPoint(transform.position);
                linePositions[i][0] = new Vector3(passivePos.x, passivePos.y, -10);
                line[i].SetPositions(linePositions[i]);
            }
        }

        #endregion

        if (button != null)
        {
            switch (passive.state)
            {
                case PassiveStats.State.Active:
                    {
                        button.transition = Selectable.Transition.None;
                        button.GetComponent<Image>().color = button.colors.highlightedColor;
                        break;
                    }
                case PassiveStats.State.Enabled:
                    {
                        button.interactable = true;
                        button.transition = Selectable.Transition.ColorTint;
                        button.GetComponent<Image>().color = button.colors.normalColor;
                        break;
                    }
                case PassiveStats.State.Disabled:
                    {
                        button.interactable = false;
                        button.GetComponent<Image>().color = button.colors.disabledColor;
                        break;
                    }
            }
        }
        else
        {
            button = GetComponentInChildren<Button>();
        }

        if (text != null)
        {
            text.text = passive.state == PassiveStats.State.Active ? passive.Name : passive.Name + "\nCost: " + passive.cost;
        }
        else
        {
            text = GetComponentInChildren<Text>();
        }
    }

    #region Passive Actiavtes()

    void Dash()
    {
        Debug.Log("Dash");
    }

    void Test()
    {
        Debug.Log("Test");
    }

    #endregion
}

[Serializable]
public class PassiveStats
{
    public string Name;
    [HideInInspector]
    public int branchID;
    [HideInInspector]
    public int ID;
    public int cost;

    /* State in tree:
    Enabled = can be picked
    Active = has been picked
    Disabled = can't be picked */
    public enum State { Enabled, Active, Disabled };
    public State state;

    /* Activate() method for this passive */
    public enum Passives { Dash, Test };
    public Passives passive;
    
    [Serializable]
    public class RequiredPassives
    {
        public int branchID;
        public int passiveID;
    }
    
    public RequiredPassives[] requiredPassives;
    [HideInInspector]
    public Passive[] requiredPassivesPassiveObject;
    [HideInInspector]
    public Passive monobehaviourPassive;
    public Sprite sprite;

    public delegate void Activates();
    public Activates[] activationMethods;    

    public void Activate()
    {
        if (state != State.Enabled)
            return;

        if (CurrencyController.CC.CheckCurrency() < cost)
        {
            Debug.Log("Not enough $ for [" + Name + "]");
            return;
        }

        CurrencyController.CC.RemoveCurrency(cost);

        state = State.Active;
        activationMethods[(int)passive].Invoke();
    }

    public void Reset()
    {
        state = State.Enabled;
    }
}


