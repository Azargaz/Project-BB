using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

[Serializable]
public class Passive : MonoBehaviour
{
    public string Name;
    [HideInInspector]
    public int id;
    public int cost;

    /* State in tree:
    Enabled = can be picked
    Active = has been picked
    Disabled = can't be picked */
    public enum State { Enabled, Active, Disabled };
    public State state;

    /* Activate() method for this passive */
    public enum Passives { test, test2 };
    public Passives passive;
    public Passive[] requiredPassive;
    public Sprite sprite;

    public delegate void Activates();
    public Activates[] activationMethods;

    Player playerController;
    public GameObject skeleton;

    Button button;
    Text text;
    Image img;
    public GameObject lineRenderer;
    LineRenderer[] line;
    List<Vector3[]> linePositions = new List<Vector3[]>();

    void Activate()
    {
        if (state != State.Enabled)
            return;

        if (CurrencyController.CC.CheckCurrency() < cost)
        {
            Debug.Log("Not enough $$$ for [" + Name + "]");
            return;
        }

        CurrencyController.CC.RemoveCurrency(cost);

        state = State.Active;
        activationMethods[(int)passive].Invoke();
    }

    void Awake()
    {
        activationMethods = new Activates[] { Dash, SpookTest } ;

        #region Lines

        line = new LineRenderer[requiredPassive.Length];

        if (requiredPassive.Length > 0 && lineRenderer != null)
        {            
            for (int i = 0; i < requiredPassive.Length; i++)
            {
                line[i] = Instantiate(lineRenderer, transform).GetComponent<LineRenderer>();
                line[i].numPositions = 2;
                linePositions.Add(new Vector3[2]);
            }
        }

        #endregion
        
        button = GetComponentInChildren<Button>();
        text = GetComponentInChildren<Text>();
        img = button.GetComponent<Image>();
        img.sprite = sprite;
        button.onClick.AddListener(Activate);

        if (button == null)
            Debug.LogError("No button element in children.");
        if (text == null)
            Debug.LogError("No text element in children.");
        if(img == null)
            Debug.LogError("No image element in button.");        
    }

    void Start()
    {
        playerController = GameManager.player.GetComponent<Player>();
    }

    void Update()
    {
        #region Lines && Required passives

        if (requiredPassive.Length > 0)
        {
            for (int i = 0; i < requiredPassive.Length; i++)
            {
                if(line.Length > 0 && linePositions.Count > 0 && lineRenderer != null)
                {
                    Vector3 rPassivePos = Camera.main.ScreenToWorldPoint(requiredPassive[i].transform.position);
                    linePositions[i][1] = new Vector3(rPassivePos.x, rPassivePos.y, -10);
                }

                if (requiredPassive[i].state != State.Active)
                {
                    state = State.Disabled;
                }
                else if(requiredPassive[i].state == State.Active && state != State.Active)
                {
                    state = State.Enabled;
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
            switch (state)
            {
                case State.Active:
                    {
                        button.transition = Selectable.Transition.None;
                        button.GetComponent<Image>().color = button.colors.highlightedColor;
                        break;
                    }
                case State.Enabled:
                    {
                        button.interactable = true;
                        button.transition = Selectable.Transition.ColorTint;
                        button.GetComponent<Image>().color = button.colors.normalColor;
                        break;
                    }
                case State.Disabled:
                    {
                        button.interactable = false;
                        button.GetComponent<Image>().color = button.colors.disabledColor;
                        break;
                    }
            }
        }

        if (text != null)
        {
            text.text = state == State.Active ? Name : Name + "\nCost: " + cost;
        }
    }

    void Dash()
    {
        playerController.canDash = true;
    }

    void SpookTest()
    {
        Debug.Log("Boo");

        int skeletons = 20;

        for (int i = 0; i < skeletons; i++)
        {
            GameObject clone = Instantiate(skeleton, playerController.transform.position, Quaternion.identity);
            LivingCreature.Statistics stats = clone.GetComponent<SummonCreature>().stats;
            stats.damage = 10;
            stats.knockbackPower = 10;
            stats.maxHealth = 50;
            stats.Initialize();
        }
    }
}


