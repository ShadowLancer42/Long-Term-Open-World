using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public int attackDamage = 3;


    public float lookRadius = 10f;



    Transform target;
    GameObject player;

    NavMeshAgent agent;


    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance.player;
        
        target = player.transform;

        agent = GetComponent<NavMeshAgent>();
    }




    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if(distance <= lookRadius)
        {
            agent.SetDestination(target.position);

            if (distance <= agent.stoppingDistance + 3f)
            {
                //Attack the target
                FaceTarget();
            }
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    //Damage func
    /*
    private void OnTriggerStay(Collider other)
    {
        while (other.tag == "player")
        {
            StartCoroutine(attackPlayer());
            player.SendMessage(die);
        }
    }
    */

    IEnumerator attackPlayer()
    {
        Debug.Log("yes");
        yield return new WaitForSeconds(3);
        Debug.Log("yes 2");

    }

}
