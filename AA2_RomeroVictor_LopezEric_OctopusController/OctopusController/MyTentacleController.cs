using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;




namespace OctopusController
{

    
    internal class MyTentacleController

    //MAINTAIN THIS CLASS AS INTERNAL
    {

        TentacleMode tentacleMode;
        Transform[] _bones;
        Transform _endEffectorSphere;

        public Transform[] Bones { get => _bones; }
        public Transform EndEffectorSphere { get => _endEffectorSphere; set => _endEffectorSphere = value; }

        //Exercise 1.
        public Transform[] LoadTentacleJoints(Transform root, TentacleMode mode)
        {
            //TODO: add here whatever is needed to find the bones forming the tentacle for all modes
            //you may want to use a list, and then convert it to an array and save it into _bones
            tentacleMode = mode;

            switch (tentacleMode){
                case TentacleMode.LEG:
                    //TODO: in _endEffectorsphere you keep a reference to the base of the leg
                    LegBones(root);

                    break;
                case TentacleMode.TAIL:
                    //TODO: in _endEffectorsphere you keep a reference to the red sphere
                    TailBones(root);

                    break;
                case TentacleMode.TENTACLE:
                    //TODO: in _endEffectorphere you  keep a reference to the sphere with a collider attached to the endEffector
                    TentacleBones(root);

                    break;
            }
            return Bones;
        }

        private void LegBones(Transform root) 
        {
            Transform bone = root.GetChild(0);

            List<Transform> tempBones = new List<Transform>();

            while (bone.childCount > 0)
            {
                tempBones.Add(bone);
                bone = bone.GetChild(1);
            }
            tempBones.Add(bone);

            _bones = tempBones.ToArray();
            _endEffectorSphere = tempBones[_bones.Length - 1];
        }

        private void TailBones(Transform root) 
        {
            Transform bone = root;

            List<Transform> tempBones = new List<Transform>();

            while (bone.childCount > 0)
            {
                tempBones.Add(bone);
                bone = bone.GetChild(1);
            }
            tempBones.Add(bone);

            _bones = tempBones.ToArray();
            _endEffectorSphere = tempBones[_bones.Length - 1];
        }

        private void TentacleBones(Transform root)
        {
            Transform bone = root.GetChild(0).GetChild(0);

            List<Transform> tempBones = new List<Transform>();

            while (bone.childCount > 0)
            {
                tempBones.Add(bone);
                bone = bone.GetChild(0);
            }

            _bones = tempBones.ToArray();
            _endEffectorSphere = bone;
        }
    }
}
