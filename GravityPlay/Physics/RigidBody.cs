using GravityPlay.Maths;
using System;
using System.Drawing;

namespace GravityPlay.Physics
{
    public abstract class RigidBody
    {
        public Vec2 Position;
        public Vec2 Velocity;
        public float Angle;            // radians
        public float AngularVelocity;  // rad/s
        public float Mass;             // kg
        public float InvMass;          // 1/kg
        public float Inertia;          // moment of inertia
        public float InvInertia;       // 1/inertia
        public float Friction;         // 0..1 (rolling/kinetic approx)
        public float Restitution;      // 0..1 (bounciness vs walls & bodies)

        public bool IsSleeping = false; // simple sleep

        protected RigidBody(float mass, float friction, float restitution)
        {
            SetMass(mass);
            Friction = friction;
            Restitution = restitution;
        }

        public void SetMass(float m)
        {
            Mass = Math.Max(0.01f, m);
            InvMass = 1f / Mass;
        }

        public abstract void ComputeInertia();
        public abstract void Draw(Graphics g);
        public abstract RectangleF GetAABB(); // for wall collision & culling

        // Optional: per-shape contact point vs. floor for rolling friction
        public virtual float EffectiveRadiusX => 1f; // used for rolling calc (override)
        public virtual float EffectiveRadiusY => 1f;
    }
}
