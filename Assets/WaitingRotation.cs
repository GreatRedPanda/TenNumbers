using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingRotation : MonoBehaviour
{
    [SerializeField]
    float speed = 5f;
    Transform child;
    // Start is called before the first frame update
    void Start()
    {
        child = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        child.Rotate(Vector3.forward, Time.deltaTime*speed);
    }
}
