using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TM
{
    public class PlayerLocomotion : MonoBehaviour
    {
        PlayerManager playerManager;
        Transform cameraObject;
        InputHandler inputHandler;
        public Vector3 moveDirection;
        PlayerStats playerStats;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Ground & Air Detection Stats")]
        [SerializeField]
        float groundDetectionRayStartPoint = 0.4f;
        [SerializeField]
        float minimumDistanceNeededToBeginFall = 1f;
        [SerializeField]
        float groundDirectionRayDistance = 0f;
        LayerMask ignoreForGroundCheck;
        public Vector3 wallCheck = new Vector3(0.2f, 0.2f, 0.2f);
        public float fallThreshold = 3f;
        public float fallDamageMultiplier = 2f;

        [Header("Movement Stats")]
        [SerializeField]
        float movementSpeed = 5f;
        [SerializeField]
        float sprintSpeed = 7f;
        [SerializeField]
        float rollSpeed = 2.25f;
        [SerializeField]
        float backStepSpeed = 2.25f;
        [SerializeField]
        float rotationSpeed = 10f;
        [SerializeField]
        float fallingSpeed = 8f;

        void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();
            playerStats = GetComponent<PlayerStats>();

            playerManager.isGrounded = true;
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
        }

        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;

        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;

            targetDir.Normalize();
            targetDir.y = 0;

            if(targetDir == Vector3.zero)
                targetDir = myTransform.forward;

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;

        }

        public void HandleMovement(float delta)
        {
            // disallow movement during animations which should lock the player
            if(playerManager.isInteracting || playerManager.isInAir)
            {
                moveDirection = Vector3.zero;
                return;
            } else {
                if (!animatorHandler.anim.GetBool("isInteracting") || playerManager.isInAir) 
                {
                    ResumeMovement();
                }
            }


            // find the camera's position on a horizontal plane, relative to player
            Vector3 horizontalCamPosition = new Vector3(cameraObject.position.x, myTransform.position.y, cameraObject.position.z);

            // find direction, horizontally, from cam to player
            Vector3 horizontalDirection = (myTransform.position - horizontalCamPosition).normalized;

            // determine forward/backward and left/right desired movement from input
            Vector3 forwardMovement = horizontalDirection * inputHandler.vertical;
            Vector3 horizontalMovement = cameraObject.right * inputHandler.horizontal;

            // combine these values to get a horizontal movement direction
            moveDirection = Vector3.ClampMagnitude(forwardMovement + horizontalMovement, 1);
            float speed = movementSpeed;
            if (inputHandler.rollFlag) {
                animatorHandler.StopRotation();
                //player will move at rollSpeed until roll animation completes.
                if(inputHandler.moveAmount > 0)
                {
                    moveDirection = myTransform.forward * rollSpeed;
                    animatorHandler.PlayTargetAnimation("RollForward", true, false);
                }
                //player will move at backStepSpeed until backStep animation completes.
                else
                {
                    moveDirection = -myTransform.forward * backStepSpeed;
                    animatorHandler.PlayTargetAnimation("BackStep", true, false);
                }
            } else {
                HandleRotation(delta);
            }
            if (inputHandler.sprintFlag && inputHandler.moveAmount > 0.5)
            {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
                moveDirection *= speed;
            }
            else
            {
                if (inputHandler.moveAmount < 0.5)
                {
                    moveDirection *= movementSpeed;
                    playerManager.isSprinting = false;
                }
                else
                {
                    moveDirection *= speed;
                    playerManager.isSprinting = false;
                }
            }

            // project the player's velocity along the slope of movement
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);
        }

        private Vector3 initialFallPosition;
        public void HandleFalling(float delta, Vector3 moveDirection)
        {
            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;

            // if raycast hits something directly in front of you, your move direction is zero, regardless of what your controller does:
            if (Physics.Raycast(origin, myTransform.forward, out hit, 0.2f))
            {
                moveDirection = Vector3.zero;
            }
            if (playerManager.isInAir)
            {
                // no ground directly below you... but you might be on an edge?
                playerManager.isGrounded = false;
                rigidbody.AddForce(-Vector3.up * fallingSpeed);
                rigidbody.useGravity = true;

                // check if player is already falling through the air, or slipping while on an edge
                if(rigidbody.velocity.y < .05f)
                {
                    playerManager.isOnEdge = SlipChecker();
                }
                else
                // player is falling
                {
                    playerManager.isOnEdge = false;
                    initialFallPosition = myTransform.position;
                }
            } else {
                initialFallPosition = myTransform.position;
            }

            Vector3 dir = moveDirection;
            dir.Normalize();
            origin = origin + dir * groundDirectionRayDistance;

            targetPosition = myTransform.position;

            // detect the ground
            Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);
            if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
            {
                normalVector = hit.normal;
                Vector3 targPos = hit.point;
                playerManager.isGrounded = true;
                targetPosition.y = targPos.y;

                if (playerManager.isInAir)
                {
                    // player has landed after falling

                    float distanceFallen = initialFallPosition.y - myTransform.position.y;
                    if (distanceFallen > fallThreshold)
                    {
                        if (playerStats != null)
                        {
                            int fallDamage = CalculateFallDamage(distanceFallen);
                            playerStats.TakeDamage(fallDamage);

                        }
                        if (playerStats.currentHealth > 0)
                        {
                            animatorHandler.LandHard();
                        }
                        ResumeMovement();
                    }
                    else
                    {
                        animatorHandler.Land();
                        ResumeMovement();
                    }

                    playerManager.isInAir = false;
                }
            }
            else
            {
                // no ground detected!
                if(playerManager.isGrounded)
                {
                    playerManager.isGrounded = false;
                }

                if(playerManager.isInAir == false)
                {
                    animatorHandler.StartFalling();
                    rigidbody.AddForce(-Vector3.up * fallingSpeed);
                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (movementSpeed / 10);
                    playerManager.isInAir = true;
                    moveDirection = Vector3.zero;
                }
            }
            if(playerManager.isGrounded) 
            {
                if(playerManager.isInteracting || inputHandler.moveAmount > 0)
                {
                    myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime * 10);
                }
                else
                {
                    myTransform.position = targetPosition;
                }
            }
        }

        public int CalculateFallDamage(float fallDistance)
        {
            // Ensure fall distance is greater than the threshold
            if (fallDistance <= fallThreshold)
            {
                return 0; // No fall damage if fall distance is less than or equal to the threshold
            }

            // Calculate fall damage using the quadratic formula
            int fallDamage = Mathf.RoundToInt((fallDistance - fallThreshold) * (fallDistance - fallThreshold) * fallDamageMultiplier);

            return fallDamage;
        }

        public bool SlipChecker()
        {
            RaycastHit hit;
            Vector3 raySpawnPosition = myTransform.position + Vector3.up * wallCheck.y;

            Vector3 forward = myTransform.forward * wallCheck.x;
            Vector3 backward = -myTransform.forward * wallCheck.x;
            Vector3 right = myTransform.right * wallCheck.x;
            Vector3 left = -myTransform.right * wallCheck.x;

            float dis = wallCheck.x;

            Debug.DrawRay(raySpawnPosition + backward, forward, Color.blue, 0.1f, false);
            Debug.DrawRay(raySpawnPosition + forward, backward, Color.blue, 0.1f, false);
            Debug.DrawRay(raySpawnPosition + left, right, Color.blue, 0.1f, false);
            Debug.DrawRay(raySpawnPosition + right, left, Color.blue, 0.1f, false);

            if (Physics.Raycast(raySpawnPosition + backward, forward, out hit, wallCheck.x))
            {
                SlipMove(hit.normal);
            }
            else if (Physics.Raycast(raySpawnPosition + forward, backward, out hit, wallCheck.x))
            {
                SlipMove(hit.normal);
            } 
            else if (Physics.Raycast(raySpawnPosition + left, right, out hit, wallCheck.x))
            {
                SlipMove(hit.normal);
            } 
            else if (Physics.Raycast(raySpawnPosition + right, left, out hit, wallCheck.x))
            {
                SlipMove(hit.normal);
            } 
            else if (rigidbody.velocity.y < .05f)
            {
                SlipMove(myTransform.forward / 4);
            }
            
            else {
                return false;
            }
            return true;
        }

        void SlipMove(Vector3 slipDirection)
        {
            slipDirection.y = 0;
            rigidbody.MovePosition(rigidbody.position + (slipDirection / 20));
        }

        public IEnumerator MoveOverSpeed (Rigidbody objectToMove, Vector3 end, float speed){
            // speed should be 1 unit per second
            while (objectToMove.transform.position != end)
            {
                objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, speed * Time.deltaTime);
                yield return new WaitForEndOfFrame ();
            }
        }
        public IEnumerator MoveOverSeconds (Rigidbody objectToMove, Vector3 end, float seconds)
        {
            float elapsedTime = 0;
            Vector3 startingPos = objectToMove.transform.position;
            Vector3 nextPos = objectToMove.transform.position;
            while (elapsedTime < seconds)
            {
                nextPos = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
                Vector3 spd = (nextPos - startingPos).normalized * 13f;
                objectToMove.velocity = spd;
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        public void StopMovement()
        {
            rigidbody.isKinematic = true;
            return;
        }

        public void ResumeMovement()
        {
            if (!playerManager.isInteracting) {
                rigidbody.isKinematic = false;
                return;
            }
        }

        public void Respawn(Vector3 respawnPosition, Quaternion respawnRotation)
        {
             myTransform.position = respawnPosition;
             myTransform.rotation = respawnRotation;
        }


        #endregion

    }
}
