using System.IO;
using UnityEngine;

namespace Muco
{
    [System.Serializable]
    public class FloatForce
    {
        [SerializeField] private float value, force;
        [SerializeField] private float speed, damp;

        public FloatForce(int fps) { step = 1f / fps; }
        public FloatForce(float speed, float damp)
        {
            this.speed = speed;
            this.damp = damp;
        }

        private readonly float step;
        private float time;

        public FloatForce SetValue(float value) { this.value = value; return this; }
        public FloatForce SetForce(float force) { this.force = force; return this; }
        public FloatForce SetSpeed(float speed) { this.speed = speed; return this; }
        public FloatForce SetDamp(float damp) { this.damp = damp; return this; }

        public FloatForce AddForce(float force) { this.force += force; return this; }

        public float Update(float target, float dt)
        {
            if (step < .00001f)
                StepUpdate(target, dt);
            else
            {
                if (time > 0)
                {
                    StepUpdate(target, time);
                    dt -= time;
                }

                while (dt > step)
                {
                    StepUpdate(target, step);
                    dt -= step;
                }

                if (dt > 0)
                {
                    StepUpdate(target, dt);
                    time = step - dt;
                }
                else
                    time = 0;
            }

            return value;
        }

        private void StepUpdate(float target, float dt)
        {
            force *= 1f - damp * dt;

            force += (target - value) * dt;

            value += force * speed * dt;
        }

        public float Value { get { return value; } }

        public float GetSpeed { get { return speed; } }


        public void Save(BinaryWriter w)
        {
            w.Write(value);
            w.Write(force);
        }

        public void Load(BinaryReader r)
        {
            value = r.ReadSingle();
            force = r.ReadSingle();
        }
    }

    [System.Serializable]
    public class Vector3Force
    {
        [SerializeField] private Vector3 value, force;
        [SerializeField] private float speed, damp;

        public Vector3Force() { }
        public Vector3Force(int fps) { step = 1f / fps; }
        public Vector3Force(float speed, float damp)
        {
            this.speed = speed;
            this.damp = damp;
        }

        private readonly float step;
        private float time;

        public Vector3Force SetValue(Vector3 value) { this.value = value; return this; }
        public Vector3Force SetForce(Vector3 force) { this.force = force; return this; }
        public Vector3Force SetSpeed(float speed) { this.speed = speed; return this; }
        public Vector3Force SetDamp(float damp) { this.damp = damp; return this; }

        public Vector3Force AddForce(Vector3 force) { this.force += force; return this; }

        public Vector3 Update(Vector3 target, float dt)
        {
            if (step < .00001f)
                StepUpdate(target, dt);
            else
            {
                if (time > 0)
                {
                    StepUpdate(target, time);
                    dt -= time;
                }

                while (dt > step)
                {
                    StepUpdate(target, step);
                    dt -= step;
                }

                if (dt > 0)
                {
                    StepUpdate(target, dt);
                    time = step - dt;
                }
                else
                    time = 0;
            }

            return value;
        }

        private void StepUpdate(Vector3 target, float dt)
        {
            force *= Mathf.Max(0, 1f - damp * dt);

            force += (target - value) * dt;

            value += force * speed * dt;
        }

        public Vector3 Value { get { return value; } }

        public float GetSpeed { get { return speed; } }
    }

    [System.Serializable]
    public class Vector2Force
    {
        [SerializeField] private Vector2 value, force;
        [SerializeField] private float speed, damp;

        public Vector2Force() { }
        public Vector2Force(int fps) { step = 1f / fps; }
        public Vector2Force(float speed, float damp)
        {
            this.speed = speed;
            this.damp = damp;
        }

        private readonly float step;
        private float time;

        public Vector2Force SetValue(Vector2 value) { this.value = value; return this; }
        public Vector2Force SetForce(Vector2 force) { this.force = force; return this; }
        public Vector2Force SetSpeed(float speed) { this.speed = speed; return this; }
        public Vector2Force SetDamp(float damp) { this.damp = damp; return this; }

        public Vector2Force AddForce(Vector2 force) { this.force += force; return this; }

        public Vector2 Update(Vector2 target, float dt)
        {
            if (step < .00001f)
                StepUpdate(target, dt);
            else
            {
                if (time > 0)
                {
                    StepUpdate(target, time);
                    dt -= time;
                }

                while (dt > step)
                {
                    StepUpdate(target, step);
                    dt -= step;
                }

                if (dt > 0)
                {
                    StepUpdate(target, dt);
                    time = step - dt;
                }
                else
                    time = 0;
            }


            return value;
        }

        private void StepUpdate(Vector2 target, float dt)
        {
            force *= 1f - damp * dt;

            force += (target - value) * dt;

            value += force * speed * dt;
        }

        public Vector2 Value { get { return value; } }

        public float GetSpeed { get { return speed; } }
    }

    [System.Serializable]
    public class QuaternionForce
    {
        [SerializeField] private Quaternion value = Quaternion.identity, force = Quaternion.identity;
        [SerializeField] private float speed, damp;

        public QuaternionForce() { }
        public QuaternionForce(int fps) { step = 1f / fps; }
        public QuaternionForce(float speed, float damp)
        {
            this.speed = speed;
            this.damp = damp;
        }

        private readonly float step;
        private float time;

        public QuaternionForce SetValue(Quaternion value) { this.value = value; return this; }
        public QuaternionForce SetForce(Quaternion force) { this.force = force; return this; }
        public QuaternionForce SetSpeed(float speed) { this.speed = speed; return this; }
        public QuaternionForce SetDamp(float damp) { this.damp = damp; return this; }

        public QuaternionForce AddForce(Quaternion force) { this.force = force * this.force; return this; }

        public Quaternion Update(Quaternion target, float dt, bool shortest = false)
        {
            if (step < .00001f)
                StepUpdate(target, dt, shortest);
            else
            {
                if (time > 0)
                {
                    StepUpdate(target, time, shortest);
                    dt -= time;
                }

                while (dt > step)
                {
                    StepUpdate(target, step, shortest);
                    dt -= step;
                }

                if (dt > 0)
                {
                    StepUpdate(target, dt, shortest);
                    time = step - dt;
                }
                else
                    time = 0;
            }

            return value;
        }

        private void StepUpdate(Quaternion target, float dt, bool shortest)
        {
            //  Damping  //
            force = Quaternion.LerpUnclamped(Quaternion.identity, force, 1f - damp * dt);

            //  ToTarget  //
            if (shortest)
                target = Quaternion.Slerp(value, target, 1);

            Quaternion offset = target * Quaternion.Inverse(value);
            force = Quaternion.LerpUnclamped(Quaternion.identity, offset, dt) * force;

            //  AddForce  //   
            value = Quaternion.LerpUnclamped(Quaternion.identity, force, speed * dt) * value;
        }

        public Quaternion Value { get { return value; } }

        public float GetSpeed { get { return speed; } }
    }
}