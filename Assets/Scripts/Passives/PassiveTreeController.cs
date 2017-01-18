using UnityEngine;
using System.Collections.Generic;

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
        public PassiveStats[] passives;
    }

    void Awake()
    {
        for (int i = 0; i < passiveBranches.Length; i++)
        {
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
}

