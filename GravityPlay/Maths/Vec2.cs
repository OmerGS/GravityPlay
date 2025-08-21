using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GravityPlay.Maths
{
    public struct Vec2
    {
        public float X, Y;
        public Vec2(float x, float y) { X = x; Y = y; }
        public static Vec2 operator +(Vec2 a, Vec2 b) => new Vec2(a.X + b.X, a.Y + b.Y);
        public static Vec2 operator -(Vec2 a, Vec2 b) => new Vec2(a.X - b.X, a.Y - b.Y);
        public static Vec2 operator *(Vec2 a, float s) => new Vec2(a.X * s, a.Y * s);
        public static Vec2 operator /(Vec2 a, float s) => new Vec2(a.X / s, a.Y / s);
        public float Dot(Vec2 b) => X * b.X + Y * b.Y;
        public float Length() => (float)Math.Sqrt(X * X + Y * Y);
        public Vec2 Normalized()
        {
            float len = Length();
            if (len < 1e-6f) return new Vec2(0, 0);
            return this / len;
        }
        public static Vec2 Zero => new Vec2(0, 0);
        public override string ToString() => $"({X:0.00},{Y:0.00})";
    }
}
