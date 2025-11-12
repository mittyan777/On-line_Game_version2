using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class random_collar : MonoBehaviour
{
    int collar;
    Material material;
    [SerializeField]Material []Reference_material;
    // Start is called before the first frame update
    void Start()
    {
        collar = Random.Range(1, 5);
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if(collar == 1)
        {
            gameObject.tag = "red";
            material.color = Reference_material[0].color;
        }
        else if(collar == 2) 
        {
            gameObject.tag = "blue";
            material.color = Reference_material[1].color;
        }
        else if(collar == 3)
        {
            gameObject.tag = "purple";
            material.color = Reference_material[2].color;
        }
        else if(collar == 4)
        {
            gameObject.tag = "white";
            material.color = Reference_material[3].color;
        }
    }
}
