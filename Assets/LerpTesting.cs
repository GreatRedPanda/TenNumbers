using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTesting : MonoBehaviour
{

    public GameObject Prefab;
    public float Speed;

 
    void Start()
    {
       
       
    }

    // Update is called once per frame
    void Update()
    {
        // transform.position = Vector2.SmoothDamp(transform.position, Destination.transform.position, ref vel, Time.deltaTime * Speed, MaxSpeed);
        //transform.position = Vector2.Lerp(transform.position, Destination.transform.position, Time.deltaTime * Speed);
        if (Input.GetButtonDown("Fire1"))
        {

            Shoot( Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

    }

    public void Shoot(Vector3 destination)
    {
        GameObject arrow = Instantiate(Prefab, transform.position, Quaternion.identity);

        Rigidbody2D r;
        r = arrow.GetComponent<Rigidbody2D>();
        r.AddForce(( -transform.position+ destination ) * Speed);

       // Debug.Log((-transform.position - destination) * Speed);
        Vector3 dir = destination;
      //  Debug.Log(dir);
       // dir.Normalize();
        //Debug.Log(dir);
        arrow.transform.Rotate(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        Destroy(arrow, 3);
    }
}
