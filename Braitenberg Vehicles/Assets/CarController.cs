using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System;
using JetBrains.Annotations;

public class CarController : MonoBehaviour
{
    public const int numtypes = 100;
    public float scale_intensity = 100000f;
    public const float maxspeed = 500f;
    public GameObject leftMotor, rightMotor;
    private const float bounceconst = 600000f; //amount of force to apply
    private const float dampconst = 6000f;
    private const float repelpower = 60000f;
    private const float repelrad = 5f;
    private bool isBouncing = false;
    public GameObject leftsens, rightsens;
    private Rigidbody rb;
    //find sensors from gameobjects
    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        rb = GetComponent<Rigidbody>();
    }
    public void FixedUpdate()
    {
        //Console.WriteLine(sources.Length.ToString());
        float[,] intens_type = new float[numtypes,2];//hopefully initialized to 0
        GameObject[] sources = GameObject.FindGameObjectsWithTag("source");
        foreach (var source in sources)
        {
            //add the intensity to the specific type
            SourceController sc = source.GetComponent<SourceController>();
            float ldist = Vector3.Distance(source.transform.position, leftsens.transform.position);
            float rdist = Vector3.Distance(source.transform.position, rightsens.transform.position);
            float lout = (sc.source_intensity * scale_intensity) / (ldist * ldist);
            float rout = (sc.source_intensity * scale_intensity) / (rdist * rdist);
            intens_type[sc.source_type, 0] += lout;
            intens_type[sc.source_type, 1] += rout;
        }
        float speedleft = 0, speedright = 0;
        Transform sensors = transform.root.Find("sensors");
        //Debug.Log(sensors);
        foreach(Transform sensortrans in sensors)
        {
            //add to the speed motors respectively
            GameObject sensor = sensortrans.gameObject;
            SensorController sensorcont = sensor.GetComponent<SensorController>();
            var (type, conn, fun) = (sensorcont.type, sensorcont.connection, sensorcont.function);
            if(conn == 1)
            {
                speedleft += fun.evaluate(intens_type[type, 0]);
                speedright += fun.evaluate(intens_type[type, 1]);
            }
            else
            {
                speedleft += fun.evaluate(intens_type[type, 1]);
                speedright += fun.evaluate(intens_type[type, 0]);
            }
        }
        (speedleft, speedright) = Bounded(speedleft, speedright);
        if (!isBouncing) { ApplySpeed(leftMotor, speedleft); ApplySpeed(rightMotor, speedright); }
        else
        {
            //damping
            RemoveSpeed(leftMotor); RemoveSpeed(rightMotor);
            rb.AddForce(-rb.velocity * dampconst);
        }
        //Repellance between vehicles
        GameObject[] vehs = GameObject.FindGameObjectsWithTag("vehicle");
        foreach (var v in vehs)
        {
            if (v != gameObject)
            {
                v.transform.Find("vehicle").GetComponent<Rigidbody>().AddExplosionForce(repelpower, transform.position, repelrad);
            }
        }
        //Debug.Log((speedleft - GetVelocity(leftMotor)).ToString() + " " + (speedright - GetVelocity(rightMotor)).ToString());
        //Debug.Log(speedleft.ToString() + ", " + speedright.ToString());
    }

    public void ApplySpeed(GameObject motor, float speedval)
    {
        //float rpm = coef * speedval; //our "target rpm" value
        //float rps = rpm * 6f;  //multiply by 360 for degrees and divide by 60 seconds to get degrees per second
        //motor.transform.Rotate(new Vector3(0, rps, 0) * Time.deltaTime); // multiply by Time.deltaTime to get the amount of one second that we want to move in this frame.
        //motor.GetComponent<Rigidbody>().AddTorque(new Vector3(1, 0, 0) * speedval * Time.deltaTime);
        var newmotor = motor.GetComponent<HingeJoint>().motor;
        newmotor.targetVelocity = speedval;
        newmotor.force = 1000000f;
        newmotor.freeSpin = false;
        motor.GetComponent<HingeJoint>().motor = newmotor;

    }

    public void RemoveSpeed(GameObject motor)
    {
        //float rpm = coef * speedval; //our "target rpm" value
        //float rps = rpm * 6f;  //multiply by 360 for degrees and divide by 60 seconds to get degrees per second
        //motor.transform.Rotate(new Vector3(0, rps, 0) * Time.deltaTime); // multiply by Time.deltaTime to get the amount of one second that we want to move in this frame.
        //motor.GetComponent<Rigidbody>().AddTorque(new Vector3(1, 0, 0) * speedval * Time.deltaTime);
        var newmotor = motor.GetComponent<HingeJoint>().motor;
        newmotor.targetVelocity = 0;
        newmotor.force = 0;
        newmotor.freeSpin = true;
        motor.GetComponent<HingeJoint>().motor = newmotor;

    }

    public float GetVelocity(GameObject motor)
    {
        return motor.GetComponent<HingeJoint>().velocity;
    }

    public (float, float) Bounded(float leftspeed, float rightspeed)
    {
        float maxscale = Math.Max(Math.Abs(leftspeed), Math.Abs(rightspeed));
        if (maxscale > maxspeed) return (leftspeed / maxscale * maxspeed, rightspeed / maxscale * maxspeed);
        else return (leftspeed, rightspeed);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.root.gameObject.tag == "constraint")
        {
            rb.AddForce(collision.contacts[0].normal * bounceconst);
            isBouncing = true;
            Invoke("StopBounce", 0.2f);
            //Debug.Log("Bounce");
        }
    }

    void StopBounce()
    {
        isBouncing = false;
    }
}