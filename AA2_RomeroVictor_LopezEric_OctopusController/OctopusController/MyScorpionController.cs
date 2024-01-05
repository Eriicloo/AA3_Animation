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
        MyTentacleController _tail;
        

        Vector3[] axis;
        Vector3[] startingOffsets;

        float stopThreshold;
        float learningRate;
        float[] solutionsTail;
        //TAIL

        float magnusEffectDirection;
        float shootingStrength;

        //LEGS
        Transform[] legTargets;
        Transform[] legFutureBases;
        MyTentacleController[] _legs = new MyTentacleController[6];

        List<List<float>> distance = new List<List<float>>();
        List<List<Vector3>> tempJoints = new List<List<Vector3>>();

        bool canWalk = false;
        float maxLegDistance;

        bool[] canMoveLeg;
        float[] legLerpTParam;
        Vector3[] lerpInitialPosition;
        Vector3[] lerpFinalPosition;
        //LEGS


        #region public
        public void InitLegs(Transform[] LegRoots, Transform[] LegFutureBases, Transform[] LegTargets)
        {
            _legs = new MyTentacleController[LegRoots.Length];

            legTargets = LegTargets;
            legFutureBases = LegFutureBases;

            maxLegDistance = 0.5f;

            canMoveLeg = new bool[LegRoots.Length];
            legLerpTParam = new float[LegRoots.Length];
            lerpInitialPosition = new Vector3[LegRoots.Length];
            lerpFinalPosition = new Vector3[LegRoots.Length];

            magnusEffectDirection = 0.0f;
            shootingStrength = 0.0f;

            //Legs init
            for (int i = 0; i < LegRoots.Length; i++)
            {
                _legs[i] = new MyTentacleController();
                _legs[i].LoadTentacleJoints(LegRoots[i], TentacleMode.LEG);
                //TODO: initialize anything needed for the FABRIK implementation

                List<float> distanceToAdd = new List<float>();
                List<Vector3> tempJointsToAdd = new List<Vector3>();

                canMoveLeg[i] = false;
                legLerpTParam[i] = 0.0f;

                for (int j = 0; j < _legs[i].Bones.Length; j++)
                {
                    if ((j + 1) < _legs[i].Bones.Length)
                    {
                        distanceToAdd.Add(Vector3.Distance(_legs[i].Bones[j].position, _legs[i].Bones[j + 1].position));
                    }

                    tempJointsToAdd.Add(_legs[i].Bones[j].position);
                }

                distance.Add(distanceToAdd);
                tempJoints.Add(tempJointsToAdd);
            }

        }

        public void InitTail(Transform TailBase)
        {
            _tail = new MyTentacleController();
            _tail.LoadTentacleJoints(TailBase, TentacleMode.TAIL);

            //TODO: Initialize anything needed for the Gradient Descent implementation
            axis = new Vector3[_tail.Bones.Length];
            stopThreshold = 0.25f;
            learningRate = 0.1f;
            startingOffsets = new Vector3[_tail.Bones.Length];
            solutionsTail = new float[_tail.Bones.Length];

            for (int i = 0; i < _tail.Bones.Length; i++)
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
            canWalk = true;
        }

        public void UpdateSliderValues(float magnusEffect, float strength)
        {
            magnusEffectDirection = magnusEffect;
            shootingStrength = strength;
        }

        //TODO: create the apropiate animations and update the IK from the legs and tail

        public void UpdateIK()
        {
            if (DistanceFromTarget(tailTarget.position, solutionsTail) < 1.0f)
            {
                updateTail();
            }

            if (canWalk)
            {
                updateLegPos();
            }

            LerpLegs();
        }
        #endregion


        #region private
        //TODO: Implement the leg base animations and logic
        private void updateLegPos()
        {
            //check for the distance to the futureBase, then if it's too far away start moving the leg towards the future base position
            updateLegs();

            for (int i = 0; i < _legs.Length; i++)
            {
                if (Vector3.Distance(_legs[i].Bones[0].position, legFutureBases[i].position) > maxLegDistance && !canMoveLeg[i])
                {
                    MoveLegBase(i);
                }
            }

        }
        //TODO: implement Gradient Descent method to move tail if necessary
        private void updateTail()
        {
            if (DistanceFromTarget(tailTarget.position, solutionsTail) > stopThreshold)
            {
                TargetApproach(new Vector3((magnusEffectDirection * -0.5f), 0f, 0f) + tailTarget.position);
            }
        }
        //TODO: implement fabrik method to move legs 
        private void updateLegs()
        {
            for (int legI = 0; legI < _legs.Length; legI++)
            {
                bool done = false;

                for (int j = 0; j < _legs[legI].Bones.Length; j++)
                {
                    tempJoints[legI][j] = _legs[legI].Bones[j].position;
                }

                if (!done)
                {
                    if (Vector3.Distance(tempJoints[legI][0], legTargets[legI].position) > distance[legI].Sum())
                    {
                        for (int jointI = 1; jointI < tempJoints[legI].Count - 1; jointI++)
                        {
                            float lambda = distance[legI][jointI] / Vector3.Magnitude(legTargets[legI].position - tempJoints[legI][jointI]);

                            tempJoints[legI][jointI] = (1 - lambda) * tempJoints[legI][jointI] + lambda * legTargets[legI].position;
                        }

                        done = true;
                    }
                    else
                    {
                        while (Vector3.Distance(tempJoints[legI][tempJoints[legI].Count - 1], legTargets[legI].position) > 0.1f)
                        {
                            //forward reaching
                            tempJoints[legI][tempJoints[legI].Count - 1] = legTargets[legI].position;

                            for (int jointI = tempJoints[legI].Count - 2; jointI >= 0; jointI--)
                            {
                                float lambda = distance[legI][jointI] / Vector3.Magnitude(tempJoints[legI][jointI + 1] - tempJoints[legI][jointI]);

                                tempJoints[legI][jointI] = (1 - lambda) * tempJoints[legI][jointI + 1] + lambda * tempJoints[legI][jointI];
                            }

                            //backward reaching
                            tempJoints[legI][0] = _legs[legI].Bones[0].position;

                            for (int jointI = 1; jointI < tempJoints[legI].Count - 1; jointI++)
                            {
                                float lambda = distance[legI][jointI - 1] / Vector3.Magnitude(tempJoints[legI][jointI - 1] - tempJoints[legI][jointI]);

                                tempJoints[legI][jointI] = (1 - lambda) * tempJoints[legI][jointI - 1] + lambda * tempJoints[legI][jointI];
                            }
                        }

                        done = true;
                    }

                    for (int j = 0; j <= _legs[legI].Bones.Length - 2; j++)
                    {
                        Vector3 crossProd = Vector3.Cross(Vector3.Normalize(_legs[legI].Bones[j + 1].position - _legs[legI].Bones[j].position), Vector3.Normalize(tempJoints[legI][j + 1] - tempJoints[legI][j]));
                        float dotProd = Vector3.Dot(Vector3.Normalize(_legs[legI].Bones[j + 1].position - _legs[legI].Bones[j].position), Vector3.Normalize(tempJoints[legI][j + 1] - tempJoints[legI][j]));

                        _legs[legI].Bones[j].Rotate(crossProd, Mathf.Acos(dotProd) * Mathf.Rad2Deg, Space.World);
                    }
                }
            }
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

        private float GradientCalculation(Vector3 target, float[] solutions, int i, float delta)
        {
            solutions[i] += delta;
            float distance1 = DistanceFromTarget(target, solutions);
            solutions[i] -= delta;

            float distance2 = DistanceFromTarget(target, solutions);

            return (distance1 - distance2) / delta;
        }

        private float NewError(int i)
        {
            return Vector3.Dot(new Vector3(0.0f, -1.0f, 0.0f),
                _tail.Bones[_tail.Bones.Length - 1].forward - _tail.Bones[_tail.Bones.Length - 2].forward);
        }

        private void TargetApproach(Vector3 target)
        {
            for (int i = 0; i < solutionsTail.Length; i++)
            {
                float gradient = GradientCalculation(target, solutionsTail, i, learningRate);

                solutionsTail[i] -= 100.0f * shootingStrength * gradient + NewError(i) / 20.0f;

                _tail.Bones[i].localRotation = Quaternion.Euler(solutionsTail[i] * axis[i]);

                if (DistanceFromTarget(target, solutionsTail) < 0.25f)
                {
                    return;
                }
            }
        }

        private void MoveLegBase(int i)
        {
            canMoveLeg[i] = true;

            legLerpTParam[i] = 0.0f;
            lerpInitialPosition[i] = _legs[i].Bones[0].position;
            lerpFinalPosition[i] = legFutureBases[i].position;
        }

        void LerpLegs()
        {
            for (int i = 0; i < _legs.Length; i++)
            {
                if (canMoveLeg[i])
                {
                    if (legLerpTParam[i] >= 1.0f)
                    {
                        canMoveLeg[i] = false;
                    }
                    else
                    {
                        _legs[i].Bones[0].position = lerpInitialPosition[i] + ((lerpFinalPosition[i] - lerpInitialPosition[i]) * (legLerpTParam[i]));
                        _legs[i].Bones[0].position += Vector3.up * Mathf.Pow(Mathf.Sin(legLerpTParam[i] * Mathf.PI), 2.0f) * 0.5f;

                        legLerpTParam[i] = legLerpTParam[i] + (Time.deltaTime * 7.5f);
                    }
                }
            }
        }


        #endregion
    }
}
