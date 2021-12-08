using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DebuggingTools : MonoBehaviour
{
    Node[] allNodes;

    // Start is called before the first frame update
    void Start()
    {
        //get all the nodes in the scene.
        allNodes = FindObjectsOfType<Node>();
    }

    // Update is called once per frame
    void Update()
    {
        //draw a ray to each of the next nodes.
        foreach (var node in allNodes)
        {
            foreach (var nextNode in node.NextNodes)
            {
                if (nextNode != null)
                    Debug.DrawRay(node.transform.position + new Vector3(0, .5f, 0), (nextNode.transform.position - node.transform.position) * .25f);
            }
        }
    }
}
