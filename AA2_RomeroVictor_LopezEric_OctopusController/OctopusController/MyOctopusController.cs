using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace OctopusController
{
    public enum TentacleMode { LEG, TAIL, TENTACLE };

    public class MyOctopusController
    {

        MyTentacleController[] _tentacles = new MyTentacleController[4];

        Transform _currentRegion;
        Transform _target;

        Transform[] _randomTargets;// = new Transform[4];


        //Save the target's position to modify
        Vector3[] targetPosition;

        //Max attempts until system collapses
        int maxAttempts = 10;
        //Attempts the system is at
        int[] currentAttempts;

        bool ballShot;

        private Dictionary<Transform, int> regionToTentacleIndex;

        int tentacleToTargetIndex = -1;

        //Target duration and timer
        float duration = 3f;
        float timer = 0f;

        bool targetReached = false;

        //Range we assume the target will be reached
        float epsilon = 0.1f;

        float sin;
        float cos;
        float theta;

        float _twistMin, _twistMax;
        float _swingMin, _swingMax;

        #region public methods
        //DO NOT CHANGE THE PUBLIC METHODS!!

        public float TwistMin { set => _twistMin = value; }
        public float TwistMax { set => _twistMax = value; }
        public float SwingMin { set => _swingMin = value; }
        public float SwingMax { set => _swingMax = value; }


        public void TestLogging(string objectName)
        {


            Debug.Log("hello, I am initializing my Octopus Controller in object " + objectName);


        }

        public void Init(Transform[] tentacleRoots, Transform[] randomTargets)
        {
            _randomTargets = randomTargets;

            _tentacles = new MyTentacleController[tentacleRoots.Length];
            targetPosition = new Vector3[tentacleRoots.Length];
            currentAttempts = new int[tentacleRoots.Length];
            regionToTentacleIndex = new Dictionary<Transform, int>();

            // foreach (Transform t in tentacleRoots)
            for (int i = 0; i < tentacleRoots.Length; i++)
            {

                _tentacles[i] = new MyTentacleController();
                _tentacles[i].LoadTentacleJoints(tentacleRoots[i], TentacleMode.TENTACLE);

                //TODO: initialize any variables needed in ccd

                targetPosition[i] = randomTargets[i].position;
                currentAttempts[i] = 0;

                //TODO: use the regions however you need to make sure each tentacle stays in its region
                regionToTentacleIndex.Add(randomTargets[i].parent, i);
            }

            tentacleToTargetIndex = -1;
            ballShot = false;
            timer = 0f;


        }


        public void NotifyTarget(Transform target, Transform region)
        {
            if (!ballShot || timer >= duration)
            {
                return;
            }

            _currentRegion = region;
            _target = target;

            if (regionToTentacleIndex.ContainsKey(region))
            {
                tentacleToTargetIndex = regionToTentacleIndex[region];
            }
        }

        public void NotifyShoot()
        {
            //TODO. what happens here?
            Debug.Log("Shoot");
            ballShot = true;
        }


        public void UpdateTentacles()
        {
            //TODO: implement logic for the correct tentacle arm to stop the ball and implement CCD method
            update_ccd();

            if (ballShot)
            {
                if (timer < duration)
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    tentacleToTargetIndex = -1;
                }
            }
        }




        #endregion


        #region private and internal methods
        //todo: add here anything that you need

        void update_ccd()
        {
            for (int i = 0; i < _tentacles.Length; ++i)
            {
                Transform[] bones = _tentacles[i].Bones;
                Transform target;

                if (ballShot && i == tentacleToTargetIndex)
                {
                    target = _target;
                }
                else
                {
                    target = _randomTargets[i];
                }

                targetReached = false;


                if (!targetReached)
                {
                    if (currentAttempts[i] <= maxAttempts)
                    {
                        // loop backwards through all joints starting from the second to last
                        for (int j = bones.Length - 2; j >= 0; j--)
                        {
                            //Vector from a particular joint to endEffector
                            Vector3 vector1 = (bones[bones.Length - 1].transform.position - bones[i].transform.position).normalized;

                            //Vector from a particular joint to target
                            Vector3 vector2 = (target.position - bones[i].transform.position).normalized;


                            if (vector1.magnitude * vector2.magnitude <= 0.001f)
                            {
                                sin = 0f;
                                cos = 1f;
                            }
                            else
                            {
                                //In order to find the components we use the cross & dot product
                                Vector3 cross = Vector3.Cross(vector1, vector2);
                                sin = cross.magnitude;
                                float dot = Vector3.Dot(vector1, vector2);
                                cos = dot;
                            }


                            Vector3 rotationAxis = Vector3.Cross(vector1, vector2).normalized;

                            //Angle between vector 1 and 2 to clamp them
                            theta = Mathf.Acos(Mathf.Clamp(cos, -1f, 1f));

                            if (sin < 0f)
                            {
                                theta = -theta;
                            }


                            theta = theta * Mathf.Rad2Deg;

                            //Rotate a particular joint with the axis by theta degrees
                            bones[i].transform.rotation = Quaternion.AngleAxis(theta, rotationAxis) * bones[i].transform.rotation;

                            ++currentAttempts[i];

                        }
                    }

                    Vector3 diffEndEffectorToTarget = bones[bones.Length - 1].transform.position - target.position;

                    //Range of the target is reacheable, if not, the process starts again until it's found
                    if (diffEndEffectorToTarget.magnitude < epsilon)
                    {
                        targetReached = true;
                    }
                    else
                    {
                        targetReached = false;
                    }

                    if (target.position != targetPosition[i])
                    {
                        currentAttempts[i] = 0;
                        targetPosition[i] = target.position;
                    }

                }
                _tentacles[i].EndEffectorSphere = bones[bones.Length - 1];
            }
        }




        #endregion






    }
}
