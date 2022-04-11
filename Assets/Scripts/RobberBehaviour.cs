using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : MonoBehaviour
{
    BehaviourTree tree;
    public GameObject diamond;
    public GameObject car;
    public GameObject backdoor;
    public GameObject frontdoor;
    NavMeshAgent agent;
    public enum ActionState { IDLE, WORKING };
    ActionState state = ActionState.IDLE;
    Node.Status treeStatus = Node.Status.RUNNING;

   void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();

        tree = new BehaviourTree();
        Sequence steal = new Sequence("Steal something");
        Leaf goToDiamond = new Leaf("Go to diamond", GoToDiamond);
        Leaf goToCar = new Leaf("Go to car", GoToCar);
        Leaf goToBackDoor = new Leaf("Go to Backdoor", GoToBackDoor);
        Leaf goToFrontDoor = new Leaf("Go to Frontdoor", GoToFrontDoor);
        
        Selector opendoor = new Selector("Open Door");
        opendoor.AddChild(goToFrontDoor);
        opendoor.AddChild(goToBackDoor);
        // opendoor.AddChild(goToFrontDoor);

        steal.AddChild(opendoor);
        steal.AddChild(goToDiamond);
        steal.AddChild(goToCar);
        tree.AddChild(steal);
    }

    public Node.Status GoToDiamond()
    {
        Node.Status s = GoToLocation(diamond.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            diamond.transform.parent = this.gameObject.transform;
        }
        return s;
    }
    public Node.Status GoToCar()
    {
        return GoToLocation(car.transform.position);

    }
    public Node.Status GoToBackDoor()
    {
        return GoToDoor(backdoor);
    }
    public Node.Status GoToFrontDoor()
    {
        return GoToDoor(frontdoor);
    }

    public Node.Status GoToDoor(GameObject door)
    {
        Node.Status s = GoToLocation(door.transform.position);
        if(s == Node.Status.SUCCESS)
        {
            if (door.GetComponent<Lock>().isLocked)
            {
                door.SetActive(false);
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        else return s;
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
