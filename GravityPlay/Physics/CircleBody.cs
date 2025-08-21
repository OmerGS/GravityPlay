using GravityPlay.Maths;
using System;
using System.Drawing;

namespace GravityPlay.Physics
{
    public class CircleBody : RigidBody
    {
        public float Radius;
        public Color Color = Color.DeepSkyBlue;

        public CircleBody(float radius, float mass, float friction, float restitution) : base(mass, friction, restitution)
        {
            Radius = radius;
            ComputeInertia();
        }
        public override void ComputeInertia()
        {
            // Solid disk: I = 1/2 m r^2
            Inertia = 0.5f * Mass * Radius * Radius;
            InvInertia = 1f / Inertia;
        }
        public override void Draw(Graphics g)
        {
            float d = Radius * 2f;
            g.FillEllipse(new SolidBrush(Color), Position.X - Radius, Position.Y - Radius, d, d);
            // draw a small line to show rotation
            var dir = new Vec2((float)Math.Cos(Angle), (float)Math.Sin(Angle));
            g.DrawLine(Pens.White, Position.X, Position.Y, Position.X + dir.X * Radius, Position.Y + dir.Y * Radius);
        }
        public override RectangleF GetAABB() => new RectangleF(Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);
        public override float EffectiveRadiusY => Radius;
    }
}
