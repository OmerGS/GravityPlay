using GravityPlay.Physics;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace GravityPlay.Rendering
{
    public class GamePanel : Panel
    {
        public PhysicsWorld World;
        public GamePanel() { this.DoubleBuffered = true; }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (World == null) return;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach (var b in World.Bodies)
                b.Draw(e.Graphics);

            // HUD
            string hud = $"FPS:{_lastMeasuredFps}  Bodies:{World.Bodies.Count}  g:{Config.Gravity:0.00}  tScale:{Config.TimeScale:0.00}";
            e.Graphics.DrawString(hud, SystemFonts.DialogFont, Brushes.Lime, 6, 6);
        }

        private int _frames;
        private int _lastMeasuredFps;
        private Stopwatch _fpsWatch = Stopwatch.StartNew();
        public void CountFrame()
        {
            _frames++;
            if (_fpsWatch.ElapsedMilliseconds >= 1000)
            {
                _lastMeasuredFps = _frames;
                _frames = 0;
                _fpsWatch.Restart();
            }
        }
    }
}
