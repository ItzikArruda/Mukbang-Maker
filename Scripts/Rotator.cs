using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// namespace Snake3D.Visuals
// {
    public class Rotator : MonoBehaviour
    {
        public Vector3 Eulers;
        public float Speed;
        public float Rot;
        Vector3 BaseRot;

        private void Start() {
            BaseRot = transform.eulerAngles;
        }

        // Update is called once per frame
        void Update()
        {
            if(Speed != 0f) { Rot += Speed * 90f * Time.deltaTime; }
            if(Rot > 360f || Rot < -360f) { Rot = 0f; }
            transform.eulerAngles = BaseRot + (Eulers * Rot);
        }

        public void Freeze()
        {
            Speed = 0f;
            Rot = 0f;
            Update();
        }
    }
// }