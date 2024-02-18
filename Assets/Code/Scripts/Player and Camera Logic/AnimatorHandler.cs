using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TM
{
    public class AnimatorHandler : MonoBehaviour
    {
        PlayerManager playerManager;
        public Animator anim;
        InputHandler inputHandler;
        PlayerLocomotion playerLocomotion;
        int vertical;
        int horizontal;
        public bool canRotate;

        public void Initialize()
        {
            anim = GetComponent<Animator>();
            playerManager = GetComponentInParent<PlayerManager>();
            inputHandler = GetComponentInParent<InputHandler>();
            playerLocomotion = GetComponentInParent<PlayerLocomotion>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
        {
            // clamp vertical movement values:
            #region Vertical
            float v = 0;

            if (verticalMovement > 0 && verticalMovement < 0.55f)
            {
                v = 0.5f;
            }
            else if (verticalMovement > 0.55f)
            {
                v = 1;
            }
            if (verticalMovement < 0 && verticalMovement > -0.55f)
            {
                v = -0.5f;
            }
            else if (verticalMovement < -0.55f)
            {
                v = -1;
            }
            #endregion

            // clamp horizontal movement values:
            #region Horizontal
            float h = 0;

            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                h = 0.5f;
            }
            else if (horizontalMovement > 0.55f)
            {
                h = 1;
            }
            if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                h = -0.5f;
            }
            else if (horizontalMovement < -0.55f)
            {
                h = -1;
            }
            #endregion

            // increase the speed of the roll animation:
            if(inputHandler.rollFlag)
            {
                anim.speed = 1.5f;
            }
            // movement speed increases when sprinting:
            if (isSprinting)
            {
                v = 2;
                h = horizontalMovement;
            }
            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        public void PlayTargetAnimation(string targetAnim, bool isInteracting, bool lockInPlace)
        {
            if (lockInPlace)
            {
                playerLocomotion.StopMovement();
            }
            if (!playerManager.isInteracting)
            {
                anim.SetBool("isInteracting", isInteracting);
                anim.applyRootMotion = isInteracting;
                anim.CrossFade(targetAnim, 0.2f);
            }
        }

        public void StartFalling()
        {
            anim.SetBool("isInteracting", true);
            anim.SetBool("isFalling", true);
            anim.SetBool("hardFall", false);
        }

        public void Land()
        {
            playerLocomotion.StopMovement();
            anim.SetBool("isFalling", false);
            playerLocomotion.ResumeMovement();
        }

        public void LandHard()
        {
            anim.SetBool("isInteracting", true);
            playerLocomotion.StopMovement();
            anim.SetBool("hardFall", true);
            anim.SetBool("isFalling", false);
            playerLocomotion.ResumeMovement();
        }

        public void Die()
        {
            playerLocomotion.StopMovement();
            anim.SetBool("isDead", true);
            anim.CrossFade("Death", 0f);
        }

        public void Revive()
        {
            anim.SetBool("isDead", false);
            anim.SetBool("isFalling", false);
            anim.CrossFade("Empty", 0f);
            playerLocomotion.ResumeMovement();
        }

        public void CancelAnimation()
        {
            anim.enabled = false;
        }

        public void ResumeAnimation()
        {
            anim.enabled = true;
        }

        public void CanRotate()
        {
            canRotate = true;
        }

        public void StopRotation()
        {
            canRotate = false;
        }

        private void OnAnimatorMove()
        {
            if (playerManager.isInteracting == false)
                return;

            float delta = Time.deltaTime;
            playerLocomotion.rigidbody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
        }
    }
}
