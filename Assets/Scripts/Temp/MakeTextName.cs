using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MakeTextName : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TMP_Text>().text = transform.root.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
