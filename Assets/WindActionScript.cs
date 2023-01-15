using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindActionScript : MonoBehaviour
{
    //GameObject player;

    Vector3 movePosX;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //player.transform.position.x + 1;
        //player.transform.Translate(Vector3.right * 1 * Time.deltaTime);

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("¹Ù¶÷");
            Debug.Log(collision.gameObject);
            //movePosX = collision.gameObject.transform.position;
            //movePosX.x += 1.0f;
            collision.gameObject.GetComponent<Rigidbody2D>().drag = 0;
            collision.gameObject.transform.Translate(Vector3.right * 10 * Time.deltaTime);
            //collision.gameObject.transform.position = movePosX;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<Rigidbody2D>().drag = 1;
        }
    }
}
