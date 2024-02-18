using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TM
{
    public class HealPlayer : MonoBehaviour
    {
        public int health = 7;
        private float hitLast = 0;
        private float hitDelay = (float).4;
        private void OnTriggerStay(Collider other)
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                if (Time.time - hitLast < hitDelay)
                {
                    return;
                }
                hitLast = Time.time;
                playerStats.Heal(health);
            }
        }
    }
}
