using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxThrusterDirt : MonoBehaviour
{
    public List<ParticleSystem> particleSystems;
    // Start is called before the first frame update
    void Awake()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop();
        }

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            foreach (ParticleSystem ps in particleSystems)
            {
                ps.Play();
            }
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            List<ContactPoint2D> contactPoints = new List<ContactPoint2D>();
            col.GetContacts(contactPoints);
            float highY = float.NegativeInfinity;
            foreach (ContactPoint2D pt in contactPoints)
            {
                if (pt.point.y > highY)
                {
                    highY = pt.point.y;
                }
            }
            if (highY > float.NegativeInfinity) {
                Vector3 target = new Vector3(transform.position.x, highY, transform.position.z);
                foreach (ParticleSystem ps in particleSystems)
                {
                    ps.transform.position = Vector3.Lerp(ps.transform.position, target, Time.deltaTime);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            foreach (ParticleSystem ps in particleSystems)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}
