using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] int numberOfAgents = 300;
    [SerializeField] float spawnSpeed = .1f;
    [SerializeField] GameObject agent;
    [SerializeField] Color GizmosColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
    [SerializeField] List<GameObject> ticketingLocations;

    float timer = 0;
    int count = 0;

    // Update is called once per frame
    void Update()
    {
        //there are still agents that need to be spawned.
        if (count <= numberOfAgents)
            timer += Time.deltaTime;

        //spawn an agent and reset the timer.
        if (timer >= spawnSpeed)
        {
            count++;
            SpawnAgent();
            timer = 0f;
        }
    }

    void OnDrawGizmos()
    {
        //used for editor to visualize the spawn area.
        Gizmos.color = GizmosColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }

    public void SpawnAgent()
    {
        //set the spawn location somewhere in the spawn area.
        Vector3 origin = transform.position;
        Vector3 range = transform.localScale / 2.0f;
        Vector3 randomRange = new Vector3(Random.Range(-range.x, range.x),
                                          1,
                                          Random.Range(-range.z, range.z));
        Vector3 randomCoordinate = origin + randomRange;

        //spawn the agent.
        var spawnedAgent = Instantiate(agent, randomCoordinate, Quaternion.identity);
        //get its person script.
        var person = spawnedAgent.GetComponent<Person>();

        //ticketing is empty.
        GameObject ticketing = null;

        //the agent is impatient, select the ticketing counter with the shortest line.
        if (person.isImpatient)
        {
            foreach (var ticket in ticketingLocations)
            {
                //set the ticketing counter to this one. 
                if (ticketing == null)
                    ticketing = ticket;
                //find the shortest line time.
                else if (ticket.GetComponent<ProcessPeople>().LineTime < ticketing.GetComponent<ProcessPeople>().LineTime)
                    ticketing = ticket;
            }
        }
        //the agent does not care about time, set a random ticketing counter.
        else
            ticketing = ticketingLocations[Random.Range(0, ticketingLocations.Count)];

        //set the target.
        person.SetTarget(ticketing);
    }
}
