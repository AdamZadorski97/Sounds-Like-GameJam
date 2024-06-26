using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG
{
    public class DoorHelper : MonoBehaviour
    {
        public AudioClip DoorOpenSound;
        public AudioClip DoorCloseSound;

        [Tooltip("Does the handle need to be turned in order to open the door from the closed position?")]
        public bool RequireHandleTurnToOpen = false;

        bool handleLocked = false;

        public Transform HandleFollower;
        public float DegreesTurned;
        public float DegreesTurnToOpen = 10f;
        public Transform DoorLockTransform;
        float initialLockPosition;

        HingeJoint hinge;
        Rigidbody rigid;
        bool playedOpenSound = false;
        bool readyToPlayCloseSound = false;

        public float AngularVelocitySnapDoor = 0.2f;
        public float angle;
        public float AngularVelocity = 0.2f;

        [Tooltip("If true the door will not respond to user input")]
        public bool DoorIsLocked = false;

        public float lockPos;

        Vector3 currentRotation;
        float moveLockAmount, rotateAngles, ratio;

        void Start()
        {
            hinge = GetComponent<HingeJoint>();
            rigid = GetComponent<Rigidbody>();

            if (DoorLockTransform)
            {
                initialLockPosition = DoorLockTransform.transform.localPosition.x;
            }
        }

        void Update()
        {
            AngularVelocity = rigid.angularVelocity.magnitude;
            currentRotation = transform.localEulerAngles;
            angle = GetDoorAngle();

            if (angle > 10 && !playedOpenSound)
            {
                VRUtils.Instance.PlaySpatialClipAt(DoorOpenSound, transform.position, 1f, 1f);
                playedOpenSound = true;
            }

            if (angle > 30)
            {
                readyToPlayCloseSound = true;
            }

            if (angle < 2 && playedOpenSound)
            {
                playedOpenSound = false;
            }

            if (angle < 1 && AngularVelocity <= AngularVelocitySnapDoor)
            {
                if (!rigid.isKinematic)
                {
                    rigid.angularVelocity = Vector3.zero;
                }
            }

            if (readyToPlayCloseSound && angle < 2)
            {
                VRUtils.Instance.PlaySpatialClipAt(DoorCloseSound, transform.position, 1f, 1f);
                readyToPlayCloseSound = false;
            }

            if (HandleFollower)
            {
                DegreesTurned = Mathf.Abs(HandleFollower.localEulerAngles.y - 270);
            }

            if (DoorLockTransform)
            {
                moveLockAmount = 0.025f;
                rotateAngles = 55;
                ratio = rotateAngles / (rotateAngles - Mathf.Clamp(DegreesTurned, 0, rotateAngles));
                lockPos = initialLockPosition - (ratio * moveLockAmount) + moveLockAmount;
                lockPos = Mathf.Clamp(lockPos, initialLockPosition - moveLockAmount, initialLockPosition);

                DoorLockTransform.transform.localPosition = new Vector3(lockPos, DoorLockTransform.transform.localPosition.y, DoorLockTransform.transform.localPosition.z);
            }

            if (RequireHandleTurnToOpen)
            {
                handleLocked = DegreesTurned < DegreesTurnToOpen;
            }

            if (angle < 0.02f && (handleLocked || DoorIsLocked))
            {
                if (rigid.collisionDetectionMode == CollisionDetectionMode.Continuous || rigid.collisionDetectionMode == CollisionDetectionMode.ContinuousDynamic)
                {
                    rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                }

                rigid.isKinematic = true;
            }
            else
            {
                if (rigid.collisionDetectionMode == CollisionDetectionMode.ContinuousSpeculative)
                {
                    rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                }

                rigid.isKinematic = false;
            }
        }

        float GetDoorAngle()
        {
            Vector3 doorForward = transform.forward;
            Vector3 parentForward = transform.parent ? transform.parent.forward : Vector3.forward;

            float angle = Vector3.SignedAngle(parentForward, doorForward, Vector3.up);
            return Mathf.Abs(angle);
        }

        public void OpenDoor()
        {
            Debug.Log("Open door");
            if (!playedOpenSound)
            {
                VRUtils.Instance.PlaySpatialClipAt(DoorOpenSound, transform.position, 1f, 1f);
                playedOpenSound = true;
            }

            hinge.useSpring = true;
            JointSpring spring = hinge.spring;
            spring.spring = 10f;  // Adjust the spring force value as needed
            spring.damper = 1f;   // Adjust the damper value as needed
            spring.targetPosition = 90;  // Adjust this value based on your door's hinge settings and orientation
            hinge.spring = spring;
            hinge.useLimits = true;

            JointLimits limits = hinge.limits;
            limits.min = 90;  // Adjust this value based on your door's hinge settings
            limits.max = -90;    // Adjust this value based on your door's hinge settings
            hinge.limits = limits;

            rigid.isKinematic = false;
        }

        public void CloseDoor()
        {
            Debug.Log("Close door");
            if (!readyToPlayCloseSound)
            {
                return;
            }

            if (!playedOpenSound)
            {
                VRUtils.Instance.PlaySpatialClipAt(DoorCloseSound, transform.position, 1f, 1f);
                readyToPlayCloseSound = false;
            }

            hinge.useSpring = true;
            JointSpring spring = hinge.spring;
            spring.spring = 10f;  // Adjust the spring force value as needed
            spring.damper = 1f;   // Adjust the damper value as needed
            spring.targetPosition = 0;  // Adjust this value based on your door's hinge settings
            hinge.spring = spring;
            hinge.useLimits = true;

            JointLimits limits = hinge.limits;
            limits.min = -90;  // Adjust this value based on your door's hinge settings
            limits.max = 0;    // Adjust this value based on your door's hinge settings
            hinge.limits = limits;

            rigid.isKinematic = true;
        }
    }
}
