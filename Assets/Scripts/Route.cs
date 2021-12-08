using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Route : MonoBehaviour
{
    Node[] allNodes;

    /// <summary>
    /// Dijkstra node information.
    /// </summary>
    class DijkstraNode
    {
        //the current game object.
        public GameObject NodeGameObject;
        //the previous game object
        public DijkstraNode PreviousDJNode;
        //reference to the node script on the current game object.
        public Node Node;
        //distance value, includes time if the agent is impatient.
        public float Distance = Mathf.Infinity;
    }

    // Start is called before the first frame update
    void Start()
    {
        //find all the nodes in the scene.
        allNodes = FindObjectsOfType<Node>();
    }

    /// <summary>
    /// Get a route for an agent.
    /// </summary>
    /// <param name="start">The starting GameObject.</param>
    /// <param name="destination">The target GameObject.</param>
    /// <param name="fastest">Optional. Pass nothing for the shortest route, true for the fastest.</param>
    /// <returns></returns>
    public Queue<GameObject> GetRoute(GameObject start, GameObject destination, bool fastest = false)
    {
        //establish the unvisited list.
        List<DijkstraNode> unvisited = new List<DijkstraNode>();

        //make each node into a Dijkstra Node.
        foreach (var node in allNodes)
        {
            if (node.gameObject == start)
                unvisited.Add(new DijkstraNode { NodeGameObject = node.gameObject, Node = node, Distance = 0f });
            else
                unvisited.Add(new DijkstraNode { NodeGameObject = node.gameObject, Node = node });
        }

        //empty visited list.
        List<DijkstraNode> visited = new List<DijkstraNode>();

        //set the current node to the starting node.
        var current = unvisited.Find(n => n.NodeGameObject == start);

        //loop until the visited list contains the destination.
        while (visited.Find(n => n.NodeGameObject == destination) == null)
        {
            //skip this if the node does not connect to another node.
            if (current.Node.NextNodes != null)
            {
                //loop through the neighbors.
                foreach (var node in current.Node.NextNodes)
                {
                    //find this neighbor in the unvisited list.
                    var neighbor = unvisited.Find(n => n.NodeGameObject == node);
                    //the node has already been visited, skip to the next loop.
                    if (neighbor == null)
                        continue;
                    //set the distance to the neighbor. 
                    var distanceToNeighbor = current.Distance +
                        Vector3.Distance(current.NodeGameObject.transform.position, neighbor.NodeGameObject.transform.position);
                    //if the agent is impatient, include the line time in the distance.
                    if (fastest && neighbor.NodeGameObject.tag.Equals("SecurityNode"))
                        distanceToNeighbor += neighbor.NodeGameObject.GetComponent<ProcessPeople>().LineTime;

                    //check if this distance is less the then distance set before, if it is, change it to this one.
                    if (distanceToNeighbor < neighbor.Distance)
                    {
                        neighbor.Distance = distanceToNeighbor;
                        //set the previous node to the current node.
                        neighbor.PreviousDJNode = current;
                    }
                }
            }

            //remove this node from unvisited and add it to visited.
            unvisited.Remove(current);
            visited.Add(current);

            //the destination has been found or it cannot be found.
            if (visited.Find(n => n.NodeGameObject == destination) != null ||
                unvisited.Min(dj => dj.Distance) == Mathf.Infinity)
                break;

            //find the next node with the smallest distance score.
            current = unvisited.Find(dj => dj.Distance == unvisited.Min(n => n.Distance));
        }


        Queue<GameObject> path = new Queue<GameObject>();
        List<GameObject> reversedPath = new List<GameObject>();

        //set current to the destination.
        current = visited.Find(dj => dj.NodeGameObject == destination);

        //loop until the starting point is found.
        while (current.NodeGameObject != start)
        {
            reversedPath.Add(current.NodeGameObject);
            current = current.PreviousDJNode;
        }

        //reverse the path.
        reversedPath.Reverse();

        //add each node to the queue.
        foreach (var node in reversedPath)
        {
            path.Enqueue(node);
        }

        //return the path.
        return path;
    }
}
