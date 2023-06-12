using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEffect : MonoBehaviour
{
    public Animation anim;
    public AudioSource audio;
    

    protected void OnTriggerEnter(Collider c)
    {

        if (c.gameObject.layer == 9)
        {
            Obstacle ob = c.gameObject.GetComponent<Obstacle>();

            if (ob != null)
            {
                c.enabled = false;
                ob.Impacted();
                Destroy(c.gameObject);
                //anim.Play();
                //audio.Play();
            }
            
        }
    }
}
