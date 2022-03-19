using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : MonoBehaviour
{
    BehaviourTree tree;
    public GameObject diamond;
    public GameObject car;
    NavMeshAgent agent;
    public enum ActionState { IDLE, WORKING };
    ActionState state = ActionState.IDLE;
    Node.Status treeStatus = Node.Status.RUNNING;

   void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();

        tree = new BehaviourTree();
        Sequence steal = new Sequence("Steal something");
        Node goToDiamond = new Leaf("Go to diamond", GoToDiamond);
        Node goToCar = new Leaf("Go to car", GoToCar);

        steal.AddChild(goToDiamond);
        steal.AddChild(goToCar);
        tree.AddChild(steal);
    }

    public Node.Status GoToDiamond()
    {
        return GoToLocation(diamond.transform.position);
    }

    public Node.Status GoToCar()
    {
        return GoToLocation(car.transform.position);

    }

    Node.Status GoToLocation(Vector3 destination)
    {
        float distanceToTarget = Vector3.Distance(destination, this.transform.position);
        if (state == ActionState.IDLE)
        {
            agent.SetDestination(destination);
            state = ActionState.WORKING;
        } 
        else if (Vector3.Distance(agent.pathEndPosition, destination) >= 2)
        {
            state = ActionState.IDLE;
            return Node.Status.FAILURE;
        }
        else if (distanceToTarget < 2)
        {
            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }

    void Update()
    {
        tree.Process();
    }
}
