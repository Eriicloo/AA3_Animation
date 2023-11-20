using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace OctopusController
{
  
    public class MyScorpionController
    {
        //TAIL
        Transform tailTarget;
        Transform tailEndEffector;
        MyTentacleController _tail;
        float animationRange;
        //TAIL

        Vector3[] axis;
        Vector3[] startingOffsets;

        float stopThreshold;
        float learningRate;
        float[] solutionsTail;


        //LEGS
        Transform[] legTargets;
        Transform[] legFutureBases;
        MyTentacleController[] _legs = new MyTentacleController[6];
        //LEGS


        #region public
        public void InitLegs(Transform[] LegRoots,Transform[] LegFutureBases, Transform[] LegTargets)
        {
            _legs = new MyTentacleController[LegRoots.Length];

            //Legs init
            for (int i = 0; i < LegRoots.Length; i++)
            {
                _legs[i] = new MyTentacleController();
                _legs[i].LoadTentacleJoints(LegRoots[i], TentacleMode.LEG);
                //TODO: initialize anything needed for the FABRIK implementation
            }

        }

        public void InitTail(Transform TailBase)
        {
            _tail = new MyTentacleController();
            _tail.LoadTentacleJoints(TailBase, TentacleMode.TAIL);

            //TODO: Initialize anything needed for the Gradient Descent implementation
            axis = new Vector3[_tail.Bones.Length];
            stopThreshold = 0.5f;
            learningRate = 0.2f;
            startingOffsets = new Vector3[_tail.Bones.Length];
            solutionsTail = new float[_tail.Bones.Length];

            for (int i = 0;i < _tail.Bones.Length;i++) 
            {
                if (i == 0)
                {
                    axis[i] = Vector3.up;
                    solutionsTail[i] = _tail.Bones[i].localRotation.eulerAngles.y;
                }
                else 
                {
                    axis[i] = Vector3.right;
                    solutionsTail[i] = _tail.Bones[i].localRotation.eulerAngles.x;
                }
            }

            for (int i = 0; i < _tail.Bones.Length - 1; i++)
            {
                startingOffsets[i] = Quaternion.Inverse(_tail.Bones[i].rotation) * (_tail.Bones[i + 1].position - _tail.Bones[i].position);
            }


        }

        //TODO: Check when to start the animation towards target and implement Gradient Descent method to move the joints.
        public void NotifyTailTarget(Transform target)
        {
            tailTarget = target;
        }

        //TODO: Notifies the start of the walking animation
        public void NotifyStartWalk()
        {

        }

        //TODO: create the apropiate animations and update the IK from the legs and tail

        public void UpdateIK()
        {
 
        }
        #endregion


        #region private
        //TODO: Implement the leg base animations and logic
        private void updateLegPos()
        {
            //check for the distance to the futureBase, then if it's too far away start moving the leg towards the future base position
            //
        }
        //TODO: implement Gradient Descent method to move tail if necessary
        private void updateTail()
        {

        }
        //TODO: implement fabrik method to move legs 
        private void updateLegs()
        {

        }

        private Vector3 ForwardKinematics(float[] solutions) 
        {
            Vector3 lastPosition = _tail.Bones[0].position;
            Quaternion rotation = _tail.Bones[0].parent.rotation;

            for (int i = 1; i < _tail.Bones.Length; i++)
            {
                rotation *= Quaternion.AngleAxis(solutions[i - 1], axis[i - 1]);
                Vector3 nextPosition = lastPosition + rotation * startingOffsets[i - 1];
                lastPosition = nextPosition;
            }

            return lastPosition;
        }

        private float DistanceFromTarget(Vector3 target, float[] solutions) 
        {
            Vector3 point = ForwardKinematics(solutions);
            return Vector3.Distance(point, target);
        }


        #endregion
    }
}
