using UnityEngine;
using System.Collections;

public class MoveToPoint : MonoBehaviour
{

    // Use this for initialization
    public Transform target;
    private bool onItsWay = false;
    private NavMeshAgent agent;
    private int visited;

    void Start()
    {
        target = GameObject.Find("Zombie").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //Updates destination every frame. Now this agent is walking via NavMesh (parameters via NavAgentComponent)
        agent.SetDestination(target.position);
    }
}
