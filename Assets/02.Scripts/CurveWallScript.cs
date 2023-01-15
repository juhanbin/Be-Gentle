using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveWallScript : MonoBehaviour
{
    public GameObject player;
    public GameObject bezier;

    //public float playerPosX;
    public Vector3 bezierPosMin;
    public Vector3 bezierPosMax;

    float testV;

    Test2 func;
    PlayerScript PS;

    // Start is called before the first frame update
    void Start()
    {
        func = GameObject.Find("BezierCuve1").GetComponent<Test2>();
        //bezierPosMin = GameObject.Find("BezierCuve1").GetComponent<Test2>().BezierTest(0);
        bezierPosMin = func.BezierTest(0);
        //bezierPosMax = GameObject.Find("BezierCuve1").GetComponent<Test2>().BezierTest(1);
        bezierPosMin = func.BezierTest(1);

        Debug.Log("p1: " + bezierPosMin);
        Debug.Log("p3: " + bezierPosMax);
    }

    // Update is called once per frame
    void Update()
    {
        //playerPosX = player.transform.position.x;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        PS = obj.GetComponent<PlayerScript>();
        //obj.transform.position += new Vector3(0, 0.5f, 0);

        //Debug.Log(transform.position);
        Debug.Log(obj.transform.position);
        //Debug.Log("0" + func.BezierTest(0.2f).x);
        //Debug.Log("1" + func.BezierTest(0.33f).x);
        //Debug.Log("2" + func.BezierTest(0.4f).x);
        //Debug.Log("3" + func.BezierTest(0.7f).x);
        //Debug.Log("4" + func.BezierTest(0.9f).x);

        if (transform.position.x - 9.75f < obj.transform.position.x && obj.transform.position.x < func.BezierTest(0.2f).x)  //3.3
        {
            Debug.Log("값: [거의 직선]");
            PS.v3 = (obj.transform.position - func.BezierTest(0));
        }
        else if (obj.transform.position.x < func.BezierTest(0.33f).x)              //5
        {
            Debug.Log("값: [약간 위로]");
            PS.v3 = (obj.transform.position - func.BezierTest(0.2f));
        }
        else if (obj.transform.position.x < func.BezierTest(0.4f).x)              //6
        {
            Debug.Log("값: [거의 대각선]");
            PS.v3 = (obj.transform.position - func.BezierTest(0.33f));
        }
        else if (obj.transform.position.x < func.BezierTest(0.7f).x)              //8.5
        {
            Debug.Log("값: [약간 수직]");
            PS.v3 = (obj.transform.position - func.BezierTest(0.4f));
        }
        else if (obj.transform.position.x < func.BezierTest(0.9f).x)              //9.5
        {
            Debug.Log("값: [거의 수직]");
            PS.v3 = (obj.transform.position - func.BezierTest(0.7f));
        }
        else {
            //Debug.Log("범위초과");
            //Debug.Log(obj.transform.position + " " + PS.v3);
        }
            
    }
}
