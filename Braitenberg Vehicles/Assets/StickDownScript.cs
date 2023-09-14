using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickDownScript : MonoBehaviour
{
    private Transform vtrans;
    // Start is called before the first frame update
    void Start()
    {
        vtrans = transform.root.Find("vehicle/Cube");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(vtrans.position.x, 1.15f, vtrans.position.z);
        //Debug.Log(vtrans.position);
    }
}
