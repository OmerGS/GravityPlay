using GravityPlay.Maths;
using System;

namespace GravityPlay.Physics
{
    public static class Collision
    {
        public static bool CircleToCircle(CircleBody A, CircleBody B, out Vec2 normal, out float penetration)
        {
            Vec2 d = B.Position - A.Position;
            float r = A.Radius + B.Radius;
            float dist = d.Length();

            if (dist <= 1e-6f || dist >= r)
            {
                normal = new Vec2(0, 0);
                penetration = 0;
                return false;
            }

            normal = d / dist;
            penetration = r - dist;
            return true;
        }

        public static bool CircleToBox(CircleBody circle, BoxBody box, out Vec2 normal, out float penetration)
        {
            Vec2 localCircle = circle.Position - box.Position;
            float c = (float)Math.Cos(-box.Angle);
            float s = (float)Math.Sin(-box.Angle);
            Vec2 circleRot = new Vec2(
                localCircle.X * c - localCircle.Y * s,
                localCircle.X * s + localCircle.Y * c
            );

            // Clamp du point sur le rectangle
            float dx = Math.Max(-box.HalfSize, Math.Min(box.HalfSize, circleRot.X));
            float dy = Math.Max(-box.HalfSize, Math.Min(box.HalfSize, circleRot.Y));
            Vec2 closest = new Vec2(dx, dy);

            Vec2 diff = circleRot - closest;
            float distSqr = diff.X * diff.X + diff.Y * diff.Y;


            if (distSqr > circle.Radius * circle.Radius)
            {
                normal = new Vec2(0, 0);
                penetration = 0;
                return false;
            }

            float dist = (float)Math.Sqrt(distSqr);
            penetration = circle.Radius - dist;
            normal = dist != 0 ? diff / dist : new Vec2(1, 0);

            float cosA = (float)Math.Cos(box.Angle);
            float sinA = (float)Math.Sin(box.Angle);
            normal = new Vec2(
                normal.X * cosA - normal.Y * sinA,
                normal.X * sinA + normal.Y * cosA
            );

            return true;
        }
    }
}