using System;
using System.Drawing;
namespace GravityPlay.Physics
{
    public class BoxBody : RigidBody
    {
        public float HalfSize;
        public bool IsDiamond;
        public Color Color = Color.OrangeRed;

        public BoxBody(float halfSize, float mass, float friction, float restitution, bool asDiamond = false) : base(mass, friction, restitution)
        {
            HalfSize = halfSize;
            IsDiamond = asDiamond;
            ComputeInertia();
            if (IsDiamond) Angle = (float)(Math.PI / 4.0);
        }
        public override void ComputeInertia()
        {
            float a = HalfSize * 2f;
            Inertia = (1f / 6f) * Mass * a * a;
            InvInertia = 1f / Inertia;
        }
        public override void Draw(Graphics g)
        {
            var pts = new[]
            {
                new PointF(-HalfSize, -HalfSize),
                new PointF( HalfSize, -HalfSize),
                new PointF( HalfSize,  HalfSize),
                new PointF(-HalfSize,  HalfSize)
            };
            float c = (float)Math.Cos(Angle);
            float s = (float)Math.Sin(Angle);
            for (int i = 0; i < pts.Length; i++)
            {
                float x = pts[i].X, y = pts[i].Y;
                pts[i] = new PointF(Position.X + (x * c - y * s), Position.Y + (x * s + y * c));
            }
            using (var b = new SolidBrush(Color)) g.FillPolygon(b, pts);
            g.DrawPolygon(Pens.White, pts);
        }
        public override RectangleF GetAABB()
        {
            float r = HalfSize * 1.41421356f;
            return new RectangleF(Position.X - r, Position.Y - r, r * 2, r * 2);
        }
        public override float EffectiveRadiusY => HalfSize * 1.41421356f;
    }
}
