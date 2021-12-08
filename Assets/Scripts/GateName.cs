using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GateName : MonoBehaviour
{
    TextMeshPro GateNameText;

    // Start is called before the first frame update
    void Start()
    {
        //get the name text component.
        GateNameText = gameObject.transform.Find("FloatingText/ProcessingTime").GetComponent<TextMeshPro>();
        //set the name.
        GateNameText.text = gameObject.name;
    }
}
