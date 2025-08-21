using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GravityPlay
{
    public static class Config
    {
        // Gravity presets (m/s^2)
        public const float GRAVITY_MOON = 1.62f;
        public const float GRAVITY_MARS = 3.71f;
        public const float GRAVITY_EARTH = 9.81f;

        // Active physics parameters
        public static float Gravity = GRAVITY_EARTH;
        public static float TimeScale = 25f;        // Time dilatation
        public static float AirDrag = 0.02f;

        // Wall interaction
        public static float Restitution = 1.00f;     // bounciness 0..1
        public static float WallFriction = 0.8f;    // loss on wall hit

        // Global toggles
        public static bool EnableInterShapeCollisions = true; // Collision between shapes
        public static bool UltraSmooth = true;                  // Idle loop vs. Timer

        // Rendering / timing
        public static int TargetRenderFps = 120;     // 1, 30, 60, 120, 850 (best-effort)
        public static int FixedPhysicsHz = 240;      // fixed step for stability

        // Spawn defaults
        public static float DefaultMass = 0.5f;      // kg
        public static float DefaultFriction = 0.02f; // rolling/kinetic approx 0..1
        public static float DefaultSize = 50f;       // radius for circles, half-size for squares

        // Random spawn
        public static Random RNG = new Random();
    }
}
