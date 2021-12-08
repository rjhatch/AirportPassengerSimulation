using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ProcessPeople : MonoBehaviour
{
    public float LineTime { get { return line.Count * processingSpeed; } }
    public float ProcessingSpeed { get { return processingSpeed; } }
    public int QueueSize { get { return line.Count; } }

    [SerializeField] float processingSpeed = .5f;
    TextMeshPro Queue;
    TextMeshPro ProcessingTime;
    TextMeshPro TimeToCompletion;

    float timer;

    Queue<GameObject> line = new Queue<GameObject>();

    List<GameObject> destinations;

    Route router;

    // Start is called before the first frame update
    void Start()
    {
        //get the components for the UI.
        Queue = gameObject.transform.Find("FloatingText/Queue").GetComponent<TextMeshPro>();
        ProcessingTime = gameObject.transform.Find("FloatingText/ProcessingTime").GetComponent<TextMeshPro>();
        TimeToCompletion = gameObject.transform.Find("FloatingText/TimeToCompletion").GetComponent<TextMeshPro>();

        //update the UI.
        UpdateUI();

        //if this is a ticketing counter, find the destinations and the routing script.
        if (gameObject.tag.Equals("TicketingNode"))
        {
            destinations = FindObjectsOfType<Node>().Where(n => n.gameObject.tag.Equals("GateNode")).Select(n => n.gameObject).ToList();
            router = FindObjectOfType<Route>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        RunTimer();
    }

    /// <summary>
    /// Update the UI for this gameObject.
    /// </summary>
    void UpdateUI()
    {
        Queue.text = $"{QueueSize}";
        ProcessingTime.text = $"{ProcessingSpeed} / s";
        TimeToCompletion.text = $"{LineTime} s";
    }

    void RunTimer()
    {
        //if there is someone waiting, increase the timer.
        if (line.Count > 0)
            timer += Time.deltaTime;

        //the timer is greater than the processing speed.
        if (timer >= processingSpeed)
        {
            //get the next person.
            var agent = line.Dequeue();

            //if this is a ticketing node, set the destination, get their route, and set their color.
            if (gameObject.tag.Equals("TicketingNode"))
            {
                var dest = destinations[Random.Range(0, destinations.Count)];
                var person = agent.GetComponent<Person>();
                person.Path = router.GetRoute(gameObject, dest, person.isImpatient);
                person.Destination = dest;
            }

            //let the agent know they can move on.
            agent.SendMessage("NextNode");

            //update the UI.
            UpdateUI();

            //reset the timer.
            timer = 0f;
        }
    }

    public void EnterQueue(GameObject person)
    {
        //place this person in the queue
        line.Enqueue(person);
        //update the UI to reflect their precense.
        UpdateUI();
    }
}
