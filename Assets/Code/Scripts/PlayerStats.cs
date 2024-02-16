using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TM
{
    public class PlayerStats : MonoBehaviour
    {
        public int healthLevel = 10;
        public int maxHealth = 100;
        public int currentHealth = 100;
        public float holdOnDeathScreen = 2.75f;
        private float environmentChange = 0.0f;

        public HealthBar healthBar;
        public AlphaController alphaController;
        public AudioVolumeAdjuster soundTrackAdjuster;
        public AudioVolumeAdjuster chaoticNoiseAdjuster;
        public RotateObject rotateObject;
        public PlayerLocomotion playerLocomotion;
        public CameraHandler cameraHandler;
        public Vector3 respawnLocation = new Vector3(-28.57f, 9.6f, -6.48f);
        public Quaternion respawnRotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

        AnimatorHandler animatorHandler;

        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }

        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (currentHealth > 0)
            {
                int maxDamage = Mathf.Min(damage, currentHealth);
                currentHealth = currentHealth - maxDamage;

                healthBar.SetCurrentHealth(currentHealth);
                environmentChange = (1f - ((float)currentHealth/(float)maxHealth));
                float skyBoxChange = environmentChange * .8f;
                alphaController.SetAlphaValue(skyBoxChange);
                rotateObject.ChangeRotation(new Vector3((0f * skyBoxChange), (0f * skyBoxChange), (-.25f * skyBoxChange)));
                chaoticNoiseAdjuster.AdjustVolume(environmentChange);
                soundTrackAdjuster.AdjustVolume(1 - environmentChange);
                animatorHandler.PlayTargetAnimation("TakeDamage", true, true);

                if(currentHealth <= 0)
                {
                    currentHealth = 0;
                    animatorHandler.Die();
                    // HANDLE PLAYER DEATH

                    //show death screen UI element

                    StartCoroutine(WaitSeconds(holdOnDeathScreen));
                }
            }
        }

        public void Heal(int healthRecovered)
        {
                if(currentHealth < maxHealth) {
                    int maxHeal = Mathf.Min(healthRecovered, (maxHealth - currentHealth));
                    currentHealth = currentHealth + maxHeal;
                    healthBar.SetCurrentHealth(currentHealth);
                    environmentChange = (1f - ((float)currentHealth/(float)maxHealth));
                    float skyBoxChange = environmentChange * .8f;
                    alphaController.SetAlphaValue(skyBoxChange);
                    rotateObject.ChangeRotation(new Vector3((0f * skyBoxChange), (0f * skyBoxChange), (-.25f * skyBoxChange)));
                    chaoticNoiseAdjuster.AdjustVolume(environmentChange);
                    soundTrackAdjuster.AdjustVolume(1 - environmentChange);
                }
        }

        IEnumerator WaitSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Respawn();
        }

        public void Respawn()
        {
            Heal(maxHealth - currentHealth);
            animatorHandler.Revive();
            playerLocomotion.Respawn(respawnLocation, respawnRotation);
            cameraHandler.lookAngle = respawnRotation.eulerAngles.y;
            cameraHandler.pivotAngle = respawnRotation.eulerAngles.x;
        }

    }
}
