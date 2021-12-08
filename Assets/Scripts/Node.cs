using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    //just a list of the nodes this node is connected to. 
    public List<GameObject> NextNodes = new List<GameObject>();
}
