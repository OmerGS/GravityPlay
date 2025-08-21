using GravityPlay.Maths;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GravityPlay.Physics
{
    public class PhysicsWorld
    {
        public readonly List<RigidBody> Bodies = new List<RigidBody>();
        public RectangleF Bounds; // world bounds in pixels (screen size)

        public PhysicsWorld(RectangleF bounds) { Bounds = bounds; }

        public void Add(RigidBody b) => Bodies.Add(b);

        public void Step(float dt)
        {
            float ts = Config.TimeScale;
            dt *= Math.Max(0.001f, ts);

            // Apply forces
            foreach (var b in Bodies)
            {
                if (b.IsSleeping) continue;
                b.Velocity.Y += Config.Gravity * dt;
                b.Velocity *= (1f - Config.AirDrag * dt);
                b.AngularVelocity *= (1f - 0.5f * Config.AirDrag * dt);
            }

            // Integrate positions
            foreach (var b in Bodies)
            {
                if (b.IsSleeping) continue;
                b.Position += b.Velocity * dt;
                b.Angle += b.AngularVelocity * dt;
            }

            // World bounds collision
            foreach (var b in Bodies)
            {
                var aabb = b.GetAABB();

                if (aabb.Left < Bounds.Left)
                {
                    float dx = Bounds.Left - aabb.Left;
                    b.Position.X += dx;
                    b.Velocity.X = -b.Velocity.X * Math.Max(0f, Math.Min(1f, b.Restitution * Config.Restitution));
                    b.Velocity.Y *= (1f - Config.WallFriction);
                }
                if (aabb.Right > Bounds.Right)
                {
                    float dx = aabb.Right - Bounds.Right;
                    b.Position.X -= dx;
                    b.Velocity.X = -b.Velocity.X * Math.Max(0f, Math.Min(1f, b.Restitution * Config.Restitution));
                    b.Velocity.Y *= (1f - Config.WallFriction);
                }
                if (aabb.Top < Bounds.Top)
                {
                    float dy = Bounds.Top - aabb.Top;
                    b.Position.Y += dy;
                    b.Velocity.Y = -b.Velocity.Y * Math.Max(0f, Math.Min(1f, b.Restitution * Config.Restitution));
                    b.Velocity.X *= (1f - Config.WallFriction);
                }
                if (aabb.Bottom > Bounds.Bottom)
                {
                    float dy = aabb.Bottom - Bounds.Bottom;
                    b.Position.Y -= dy;
                    b.Velocity.Y = -b.Velocity.Y * Math.Max(0f, Math.Min(1f, b.Restitution * Config.Restitution));
                    ApplyGroundRollingFriction(b, dt);
                }
            }

            // Inter-shape collisions
            if (Config.EnableInterShapeCollisions)
            {
                for (int i = 0; i < Bodies.Count; i++)
                {
                    for (int j = i + 1; j < Bodies.Count; j++)
                    {
                        var A = Bodies[i];
                        var B = Bodies[j];

                        if (A is CircleBody c1 && B is CircleBody c2)
                            ResolveCircleCircle(c1, c2);
                        else if (A is BoxBody b1 && B is BoxBody b2)
                            ResolveBoxBox(b1, b2);
                        else if (A is CircleBody c3 && B is BoxBody b3)
                            ResolveCircleBox(c3, b3);
                        else if (A is BoxBody b4 && B is CircleBody c4)
                            ResolveCircleBox(c4, b4);
                    }
                }
            }
        }

        private void ApplyGroundRollingFriction(RigidBody b, float dt)
        {
            float r = b.EffectiveRadiusY;
            if (r <= 0) return;

            float v_tangent = b.Velocity.X - b.AngularVelocity * r;

            float k = Math.Max(0f, Math.Min(1f, b.Friction));
            float impulse = -v_tangent * k;

            b.Velocity.X += impulse * b.InvMass;
            b.AngularVelocity -= impulse * r * b.InvInertia;

            b.Velocity.X *= (1f - 0.2f * k * dt);
            b.AngularVelocity *= (1f - 0.1f * k * dt);
        }

        private void ResolveCircleCircle(CircleBody A, CircleBody B)
        {
            Vec2 d = B.Position - A.Position;
            float dist = d.Length();
            float r = A.Radius + B.Radius;
            if (dist <= 1e-6f || dist >= r) return;

            Vec2 n = d / dist;
            float penetration = r - dist;
            float totalInvMass = A.InvMass + B.InvMass;
            if (totalInvMass <= 0) return;
            Vec2 correction = n * (penetration / totalInvMass * 0.8f);
            A.Position -= correction * A.InvMass;
            B.Position += correction * B.InvMass;

            Vec2 rv = B.Velocity - A.Velocity;
            float velAlongNormal = rv.Dot(n);
            if (velAlongNormal > 0) return;

            float e = Math.Max(0f, Math.Min(1f, (A.Restitution + B.Restitution) * 0.5f * Config.Restitution));
            float j = -(1 + e) * velAlongNormal / totalInvMass;

            Vec2 impulse = n * j;
            A.Velocity -= impulse * A.InvMass;
            B.Velocity += impulse * B.InvMass;

            Vec2 t = new Vec2(-n.Y, n.X);
            float vt = rv.Dot(t);
            float mu = (A.Friction + B.Friction) * 0.5f;
            float jt = -vt / totalInvMass;
            jt = Math.Max(-j * mu, Math.Min(j * mu, jt));
            Vec2 frictionImpulse = t * jt;
            A.Velocity -= frictionImpulse * A.InvMass;
            B.Velocity += frictionImpulse * B.InvMass;
        }

        private void ResolveBoxBox(BoxBody A, BoxBody B)
        {
            // Approximatif : collision circulaire avec rayon effectif
            Vec2 d = B.Position - A.Position;
            float r = A.EffectiveRadiusY + B.EffectiveRadiusY;
            float dist = d.Length();
            if (dist <= 1e-6f || dist >= r) return;

            Vec2 n = d / dist;
            float penetration = r - dist;
            float totalInvMass = A.InvMass + B.InvMass;
            if (totalInvMass <= 0) return;

            Vec2 correction = n * (penetration / totalInvMass * 0.8f);
            A.Position -= correction * A.InvMass;
            B.Position += correction * B.InvMass;

            Vec2 rv = B.Velocity - A.Velocity;
            float velAlongNormal = rv.Dot(n);
            if (velAlongNormal > 0) return;

            float e = Math.Max(A.Restitution, B.Restitution);
            float j = -(1 + e) * velAlongNormal / totalInvMass;
            Vec2 impulse = n * j;
            A.Velocity -= impulse * A.InvMass;
            B.Velocity += impulse * B.InvMass;
        }

        private void ResolveCircleBox(CircleBody circle, BoxBody box)
        {
            Vec2 normal;
            float penetration;
            if (!Collision.CircleToBox(circle, box, out normal, out penetration)) return;

            float totalInvMass = circle.InvMass + box.InvMass;
            if (totalInvMass <= 0) return;

            Vec2 correction = normal * (penetration / totalInvMass * 0.8f);
            circle.Position += correction * circle.InvMass;
            box.Position -= correction * box.InvMass;

            Vec2 rv = box.Velocity - circle.Velocity;
            float velAlongNormal = rv.Dot(normal);
            if (velAlongNormal > 0) return;

            float e = Math.Max(circle.Restitution, box.Restitution);
            float j = -(1 + e) * velAlongNormal / totalInvMass;
            Vec2 impulse = normal * j;
            circle.Velocity -= impulse * circle.InvMass;
            box.Velocity += impulse * box.InvMass;
        }
    }
}