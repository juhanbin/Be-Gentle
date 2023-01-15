using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public float Jump_Power = 250f;
    public float Speed = 3.0f;
    float Axis;
    int jumpCount;
    bool isGround;
    public int spling_power=10;
    public int banana_power = 400;
    public int Accele_power = 400;
    Rigidbody rigid;
    SpriteRenderer rend;
    public int count_A = 0;
    public int count_D = 0;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rend = GetComponent<SpriteRenderer>();
        jumpCount = 1;
        isGround = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGround)
            {
                jumpCount = 1;
                if (jumpCount == 1)
                {
                    Jump();
                    isGround = false;
                    jumpCount = 0;
                }
            }
        }
        
        Axis = Input.GetAxisRaw("Horizontal");
        if (Axis < 0)
        {
            rend.flipX = true;
        }
        else if(Axis > 0)
        {
            rend.flipX = false;
        }
        float fallSpeed = rigid.velocity.y;
        if (isGround)
        {
            Vector3 velocity = new Vector3(Axis, 0, 0);
            velocity *= Speed;
            velocity.y = fallSpeed;
            rigid.velocity = velocity;
        }
        if(count_D>=5)
        {
            CancelInvoke("WakeUp_D");
        }
        if(count_A>=5)
        {
            CancelInvoke("WakeUp_A");
        }
    }

void Jump()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(0,Jump_Power,0));
    }
    
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "ground")
        {
            isGround = true;    //Ground에 닿으면 isGround는 true
            jumpCount = 1;          //Ground에 닿으면 점프횟수가 1로 초기화됨
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Spling")
        {
            rigid.AddForce(Vector3.up * spling_power, ForceMode.Impulse);
            Debug.Log("Spling");
        }
        else if (collision.transform.tag == "Drink")
        {
            //Speed = 15;
            //rigid.addForce(Vector3.right * Accele_power, ForceMode.Impulse);
            count_D = 0;
            InvokeRepeating("WakeUp_D", 1f, 10f);
        }
        else if (collision.transform.tag == "Banana")
        {
            if(rend.flipX == true)
                rigid.AddForce(Vector3.left * banana_power, ForceMode.Impulse);
            else if(rend.flipX == false)
                rigid.AddForce(Vector3.right * banana_power, ForceMode.Impulse);
        }
        else if (collision.transform.tag == "Accele")
        {
            count_A = 0;
            InvokeRepeating("WakeUp_A", 1f, 2f);
        }
        else if (collision.transform.tag == "Flag")
        {
            Debug.Log("Flag");
        }
    }
    void WakeUp_D()
    {
        Speed = 10.0f;
        Jump_Power = 500.0f;
        count_D += 1;
    }
    void WakeUp_A()
    {
        Speed = 7.0f;
        count_A += 1;
    }
}
