using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionStaff : MonoBehaviour
{
    public GameObject explosion;
    public Transform firePoint;
    public float explosionTargetMaxDistance;
    public float linesDelay;
    public LayerMask mask;
    LineRenderer line;
    Player player;
    LivingCreature playerStats;
    Vector3[] linePositions = new Vector3[2];

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        player = transform.root.GetComponent<Player>();
        playerStats = transform.root.GetComponent<PlayerCreature>();
    }
	
    void AnimationFindExplosionTarget()
    {
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, Vector2.right * player.facing, explosionTargetMaxDistance, mask);

        if(hit)
        {            
            linePositions[0] = new Vector3(firePoint.position.x, firePoint.position.y, -9);
            linePositions[1] = new Vector3(hit.point.x, hit.point.y, -9);
            line.sortingLayerName = "Player";
            line.SetPositions(linePositions);

            if(explosion != null)
            {
                GameObject clone = Instantiate(explosion, hit.point, Quaternion.identity);
                clone.GetComponent<DamageLivingCreature>().creature = playerStats;
                Destroy(clone, 1f);
            }

            StartCoroutine(TurnOffLineRenderer(linesDelay));            
        }
    }

    IEnumerator TurnOffLineRenderer(float delay)
    {
        yield return new WaitForSeconds(delay);

        linePositions = new Vector3[2];
        line.SetPositions(linePositions);
    }

}
