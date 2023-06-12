using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Attack : MonoBehaviour {
    public float timer=0f;
    public float lifetime = 10.0f;
    public Transform spawnpoint;
    public CharacterInputController player;
    public bool canfire;
    public ParticleSystem particle;
    public AudioSource audio;
    public Collider collider;
    public Vector3 velocity;
    public Text bonuspoints;

    // Update is called once per frame
    void Update()
    {//double tap function
       if( Input.GetKeyDown(KeyCode.DownArrow))
        {
            
            particle.Play(true);
            canfire = true;
            audio.Play();
            collider.enabled = true;
        }
        if (Input.touchCount == 1)
        {

            
            if (Input.GetTouch(0).tapCount == 2)
            {
                    
                
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {   particle.Play(true);
                    canfire = true;
                    audio.Play();
                    collider.enabled = true;
                }
                
               
            
        }
        } 
      {
            if (canfire )
            {

                

                transform.position += velocity * Time.deltaTime;
                timer += Time.deltaTime;
               
              
            }
        }
        if (timer > lifetime)
        {


            transform.position = spawnpoint.transform.position;
           
            timer = 0f;
           canfire = false;
            particle.Stop();
            collider.enabled = false;
        }
    }
   
    protected void OnTriggerEnter(Collider c)
    {
      
        if (c.gameObject.layer == 9)
        {
            particle.Stop();
            audio.Stop();
            canfire = false;
            transform.position = spawnpoint.transform.position;
           
            
             Obstacle ob = c.gameObject.GetComponent<Obstacle>();

            if (ob != null && ob.type==1)
            {c.enabled = false;
                ob.Impacted();
                player.trackManager.hitcount+=1;
                player.trackManager.AddScore(200);
                bonuspoints.gameObject.SetActive(true);
                Invoke("off", 1f);
            }
            collider.enabled = false;
        }

    }
    protected void off()
    {
        bonuspoints.gameObject.SetActive(false);
    }
    }
