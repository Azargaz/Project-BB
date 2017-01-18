using UnityEngine;

public class PassiveTreeController : MonoBehaviour
{
    public PassiveBranch[] passiveBranches;

    [Header("GameObjects")]
    public GameObject passiveBranchGO;
    public GameObject passiveGO;
    public GameObject lineRendererGO;

    [System.Serializable]
    public class PassiveBranch
    {
        public string Name;
        public enum BranchType { Strength, Dexterity, Magic, Other };
        public BranchType branchType;
        public PassiveStats[] passives;
    }

    void Awake()
    {
        for (int i = 0; i < passiveBranches.Length; i++)
        {            
            passiveBranches[i].Name = passiveBranches[i].branchType.ToString();

            GameObject branch = Instantiate(passiveBranchGO, transform);            

            /* Set branch's name to Branch + ID + Branch's Name (in format "B# [Name]")*/
            branch.name = "B" + i + " [" + passiveBranches[i].Name + "]";

            for (int j = 0; j < passiveBranches[i].passives.Length; j++)
            {
                GameObject passive = Instantiate(passiveGO, branch.transform);                

                /* Get PassiveStats made in Inspector and set it's IDs and requiredPassivePassiveObject */
                PassiveStats stats = passiveBranches[i].passives[j];

                stats.branchID = i;
                stats.ID = j;
                stats.requiredPassivesPassiveObject = new Passive[stats.requiredPassives.Length];
                //////////////////////////////////

                /* Get Passive object and set it's properties */
                Passive passiveObject = passive.GetComponent<Passive>();

                passiveObject.passive = stats;                
                passiveObject.lineRenderer = lineRendererGO;
                passiveObject.passiveController = this;
                //////////////////////////////////

                /* Set passive's name to Passive + ID + Passive's Name (in format "P# [Name]")*/
                passive.name = "P" + stats.ID + " [" + passiveBranches[i].passives[j].Name + "]";                
            }
        }
    }

    int countStr;
    int countDex;
    int countMag;

    void Update()
    {
        countStr = 0;
        countDex = 0;
        countMag = 0;

        for (int i = 0; i < passiveBranches.Length; i++)
        {
            for (int j = 0; j < passiveBranches[i].passives.Length; j++)
            {
                if(passiveBranches[i].passives[j].state == PassiveStats.State.Active)
                {
                    switch (passiveBranches[i].branchType)
                    {
                        case PassiveBranch.BranchType.Strength:
                            {
                                countStr++;
                                break;
                            }
                        case PassiveBranch.BranchType.Dexterity:
                            {
                                countDex++;
                                break;
                            }
                        case PassiveBranch.BranchType.Magic:
                            {
                                countMag++;
                                break;
                            }
                    }
                }
            }
        }

        WeaponController.wc.UpdateDamageBonus(WeaponController.DmgBonus.Type.Strength, countStr);
        WeaponController.wc.UpdateDamageBonus(WeaponController.DmgBonus.Type.Dexterity, countDex);
        WeaponController.wc.UpdateDamageBonus(WeaponController.DmgBonus.Type.Magic, countMag);
    }
}

