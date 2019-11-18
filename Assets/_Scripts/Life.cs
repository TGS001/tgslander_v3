using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Life : MonoBehaviour
{
    public class Damage
    {
        public float amount;
        public float multiplier;
        public Vector2 normal;

        public float finalAmount
        {
            get
            {
                return (amount * multiplier);
            }
        }

        public Damage(float amount, float multiplier, Vector2 offset)
        {
            this.amount = amount;
            this.multiplier = multiplier;
            this.normal = offset;
        }
    };
    public bool canRevive = false;
    public float hitpoints;
    public float maxHitpoints;
    public Slider displaySlider;
    public Animator displayAnimator;

    public delegate void AfflictDelegate(ref Damage damage);
    public delegate void DeathDelegate();
    [SerializeField]
    private List<AfflictDelegate> afflictionListeners = new List<AfflictDelegate>();
    [SerializeField]
    private List<DeathDelegate> deathListeners = new List<DeathDelegate>();

    public float percent
    {
        get
        {
            return (float)hitpoints / maxHitpoints;
        }
        set
        {
            float nhp = Mathf.Clamp((float)maxHitpoints * value, 0, maxHitpoints);
            Afflict(nhp - hitpoints);
        }
    }

    public void Register(AfflictDelegate affliction)
    {
        afflictionListeners.Add(affliction);
    }

    public void Register(DeathDelegate death)
    {
        deathListeners.Add(death);
    }

    public void Unregister(AfflictDelegate affliction)
    {
        afflictionListeners.Remove(affliction);
    }

    public void Unregister(DeathDelegate death)
    {
        deathListeners.Remove(death);
    }

    public void Afflict(float amount, float multiplier = 1)
    {
        Afflict(amount, multiplier, Vector2.zero);
    }

    public void Afflict(float amount, float multiplier, Vector2 normal)
    {
        if ((Dead() && !canRevive) || PlaySessionControl.Invulnerable())
        {
            return;
        }

        Damage damage = new Life.Damage(amount, multiplier, normal);
        if (damage.finalAmount > 0) {
            foreach (AfflictDelegate a in afflictionListeners) {
                a(ref damage);
            }
        }
        bool wasDead = Dead();

        if (damage.finalAmount > 0) {
            Trigger.DamageTaken(gameObject);
        }

        hitpoints = Mathf.Max(Mathf.Min(hitpoints - damage.finalAmount, maxHitpoints), 0);

        if (displaySlider != null)
        {
            displaySlider.value = Percent();
        }

        if (displayAnimator != null) {
            displayAnimator.SetFloat("damagePercent", 1 - percent);
        }

        if (!wasDead && hitpoints == 0)
        {
            BroadcastMessage("OnMortalDie", SendMessageOptions.DontRequireReceiver);
            DeathDelegate[] dd = deathListeners.ToArray();
            foreach (DeathDelegate d in dd)
            {
                d();
            }
        }
    }

    public bool Dead()
    {
        return hitpoints == 0;
    }

    public bool Unharmed()
    {
        return hitpoints == maxHitpoints;
    }

    public float Percent()
    {
        return (float)hitpoints / (float)maxHitpoints;
    }

    public static bool DoDamage(GameObject target, float amount)
    {
        Life m = target.GetComponentInParent<Life>();
        if (m != null)
        {
            m.Afflict(amount);
            return true;
        }
        return false;
    }

    public static bool DoDamage(GameObject target, float amount, Vector2 normal) {
        Life m = target.GetComponentInParent<Life>();
        if (m != null) {
            m.Afflict(amount, 1, normal);
            return true;
        }
        return false;
    }
}
