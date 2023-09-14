using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class UIScript : MonoBehaviour
{
    public TMP_InputField inputSources;
    public TMP_InputField inputVehicles;
    public TMP_InputField inputConstraints;
    public TMP_InputField inputConnect;
    public TMP_InputField inputEnvSize;
    public TMP_InputField inputFunction;
    public TMP_InputField inputConstraintSize;
    public GameObject vehicle0;
    public GameObject source0;
    public GameObject constraint0;

    private void Start()
    {
        RecordEnvironment();
    }

    public void RunSimulation()
    {
        ResetEnvironment();
        int sourcenum = int.Parse(inputSources.text.ToString());
        int vehiclenum = int.Parse(inputVehicles.text.ToString());
        int constraintnum = int.Parse(inputConstraints.text.ToString());
        int conntype = int.Parse(inputConnect.text.ToString());
        float sizeEnv = float.Parse(inputEnvSize.text.ToString());
        float constrsize = float.Parse(inputConstraintSize.text.ToString());
        string functionstr = inputFunction.text.ToString();
        string[] funelements = functionstr.Split(' ');
        string funtype = funelements[0];
        float x0 = float.Parse(funelements[1]);
        float y0 = float.Parse(funelements[2]);
        float slope_up = float.Parse(funelements[3]);
        float slope_down = float.Parse(funelements[4]);
        //Function fun = new Function(type, slope_up, slope_down, x0, y0);
        Camera.main.transform.position = new Vector3(sizeEnv / 2, 76, sizeEnv / 2);
        Camera.main.orthographicSize = sizeEnv+10;
        //create that configuration
        SensorController sens0 = vehicle0.transform.root.Find("sensors/sensor0").GetComponent<SensorController>();
        sens0.connection = conntype;
        //sens0.function = fun;
        sens0.functionType = funtype;
        sens0.functionX0 = x0;
        sens0.functionY0 = y0;
        sens0.functionSlopeUp = slope_up;
        sens0.functionSlopeDown = slope_down;
        CarController carcont = vehicle0.transform.root.Find("vehicle").GetComponent<CarController>();
        carcont.scale_intensity *= (sizeEnv / 10f) * (sizeEnv / 10f);//to compensate for the larger environment
        for(int i=0; i<vehiclenum; i++)
        {
            //create a random vehicle
            GameObject vehicle = Instantiate(vehicle0, new Vector3(Random.Range(0f, sizeEnv), vehicle0.transform.position.y, Random.Range(0f, sizeEnv)), 
                Quaternion.Euler(vehicle0.transform.rotation.eulerAngles.x,Random.Range(0f, 360f), vehicle0.transform.rotation.eulerAngles.z));
            vehicle.transform.root.Find("vehicle/NumText").GetComponent<TMP_Text>().text = i.ToString();
            //Debug.Log(vehicle.transform.root.Find("sensors/sensor0").GetComponent<SensorController>().functionY0);
            vehicle.SetActive(true);
        }
        for(int i=0; i<sourcenum; i++)
        {
            //create a random source
            GameObject source = Instantiate(source0, new Vector3(Random.Range(0f, sizeEnv), source0.transform.position.y, Random.Range(0f, sizeEnv)), Quaternion.identity);
            source.SetActive(true);
        }
        for (int i = 0; i < constraintnum; i++)
        {
            //create a random constraint
            GameObject constraint = Instantiate(constraint0, new Vector3(Random.Range(0f, sizeEnv), constraint0.transform.position.y, Random.Range(0f, sizeEnv)),
                Quaternion.Euler(constraint0.transform.rotation.eulerAngles.x, Random.Range(0f, 360f), constraint0.transform.rotation.eulerAngles.z));
            constraint.transform.localScale *= constrsize;
            ConstraintController constcont = constraint.GetComponent<ConstraintController>();
            //constcont.speed *= constrsize;
            constcont.radius *= constrsize;
            constraint.SetActive(true);
        }
    }

    void RecordEnvironment()
    {
        vehicle0.SetActive(false);
        source0.SetActive(false);
        constraint0.SetActive(false);
    }

    void ResetEnvironment()
    {
        GameObject[] vehicles = GameObject.FindGameObjectsWithTag("vehicle");
        GameObject[] sources = GameObject.FindGameObjectsWithTag("source");
        GameObject[] constraints = GameObject.FindGameObjectsWithTag("constraint");
        foreach (var vehicle in vehicles) { if (vehicle != null && vehicle != vehicle0) { Destroy(vehicle.transform.root.gameObject); } }
        foreach (var source in sources) { if (source != null && source != source0) { Destroy(source.transform.root.gameObject); } }
        foreach (var constraint in constraints) { if (constraint != null && constraint != constraint0) { Destroy(constraint.transform.root.gameObject); } }
    }
}
