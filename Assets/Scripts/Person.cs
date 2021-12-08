using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public float speed = 5f;
    public int impatientThreshold = 20;
    public GameObject Destination;
    public Queue<GameObject> Path = new Queue<GameObject>();
    public bool isImpatient = false;

    GameObject target;
    [SerializeField] Material impatientMat;

    // Start is called before the first frame update
    void Start()
    {
        //find out if the person is impatient. True if less than the threshold, false otherwise.
        isImpatient = Random.Range(0, 100) < impatientThreshold ? true : false;
        if (isImpatient)
            gameObject.GetComponent<Renderer>().material = impatientMat;
    }

    // Update is called once per frame
    void Update()
    {
        //move the agent as long as they have somewhere to go.
        if (target != null)
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position + Vector3.up, speed * Time.deltaTime);
    }

    public void NextNode()
    {
        //enable the renderer.
        GetComponent<MeshRenderer>().enabled = true;
        //set the next target.
        target = Path.Dequeue();
    }

    private void OnTriggerEnter(Collider other)
    {
        //the agent has reached their target.
        if (other.gameObject == target)
        {
            //this is a security node or ticketing node, join the queue and disable the mesh renderer.
            if (other.tag.Equals("SecurityNode") || other.tag.Equals("TicketingNode"))
            {
                other.GetComponent<ProcessPeople>().EnterQueue(gameObject);
                GetComponent<MeshRenderer>().enabled = false;
                return;
            }

            //we've reached the destination, destroy the gameobject.
            if (other.tag.Equals("GateNode"))
            {
                Destroy(gameObject);
                return;
            }

            //set the next target.
            target = Path.Dequeue();
        }
    }

    /// <summary>
    /// Set the agent's next target.
    /// </summary>
    /// <param name="newTarget">Target GameObject.</param>
    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }
}
