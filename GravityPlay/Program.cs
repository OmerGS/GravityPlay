using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GravityPlay.Maths;
using GravityPlay.Physics;
using GravityPlay.Rendering;

namespace GravityPlay {
    public class Form1 : Form
    {
        private GamePanel _view;
        private PhysicsWorld _world;
        private Stopwatch _loopWatch = new Stopwatch();
        private double _accumulator = 0.0;
        private double _fixedDt;
        private bool _spawningAwaitDirection = false;
        private Vec2 _pendingSpawnPos;

        public Form1()
        {
            Text = "Gravity Play";
            Width = 1000; Height = 600;
            StartPosition = FormStartPosition.CenterScreen;

            _view = new GamePanel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(20, 20, 28) };
            Controls.Add(_view);

            Resize += (s, e) => RebuildWorldBounds();
            Shown += (s, e) => StartGameLoop();
            KeyDown += OnKeyDown;
            _view.MouseDown += OnMouseDown;

            RebuildWorldBounds();

            SpawnRandomCircle();
            SpawnRandomBox(false);
            SpawnRandomBox(true);
        }

        private void RebuildWorldBounds()
        {
            var rect = _view.ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0) return;
            _world = new PhysicsWorld(new RectangleF(0, 0, rect.Width, rect.Height));
            _view.World = _world;
        }

        private void StartGameLoop()
        {
            _fixedDt = 1.0 / Math.Max(30, Config.FixedPhysicsHz);
            _loopWatch.Start();
            Application.Idle += GameLoopOnIdle;
        }

        private void GameLoopOnIdle(object sender, EventArgs e)
        {
            while (AppStillIdle)
            {
                double elapsed = _loopWatch.Elapsed.TotalSeconds;
                _loopWatch.Restart();
                _accumulator += elapsed;

                if (_accumulator > 0.5) _accumulator = 0.5;

                while (_accumulator >= _fixedDt)
                {
                    _world.Step((float)_fixedDt);
                    _accumulator -= _fixedDt;
                }

                _view.Invalidate();
                _view.Update();
                _view.CountFrame();

                int target = Math.Max(1, Config.TargetRenderFps);
                int sleep = (int)Math.Max(0, (1000.0 / target) - 0.25);
                if (sleep > 0) System.Threading.Thread.Sleep(sleep);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeMessage { public IntPtr handle; public uint msg; public IntPtr wParam; public IntPtr lParam; public uint time; public System.Drawing.Point p; }
        [DllImport("user32.dll")]
        private static extern bool PeekMessage(out NativeMessage lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);
        private bool AppStillIdle => !PeekMessage(out _, IntPtr.Zero, 0, 0, 0);

        // Input / Controls
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            var pos = new Vec2(e.X, e.Y);
            if (!_spawningAwaitDirection)
            {
                // First click: choose spawn position
                _pendingSpawnPos = pos;
                _spawningAwaitDirection = true;
            }
            else
            {
                // Second click: choose direction vector
                var dir = (pos - _pendingSpawnPos).Normalized();
                float speed = 400f; // default launch speed
                var v0 = dir * speed;

                // Spawn a circle for demo
                var c = new CircleBody(Config.DefaultSize, Config.DefaultMass, Config.DefaultFriction, Config.Restitution)
                {
                    Position = _pendingSpawnPos,
                    Velocity = v0,
                    Color = Color.FromArgb(Config.RNG.Next(40, 220), Config.RNG.Next(40, 220), Config.RNG.Next(40, 220))
                };
                _world.Add(c);
                _spawningAwaitDirection = false;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D1: Config.Gravity = Config.GRAVITY_MOON; break;
                case Keys.D2: Config.Gravity = Config.GRAVITY_MARS; break;
                case Keys.D3: Config.Gravity = Config.GRAVITY_EARTH; break;
                case Keys.G: Config.Gravity = Math.Max(0, Config.Gravity - 0.5f); break;
                case Keys.H: Config.Gravity += 0.5f; break;

                case Keys.F1: Config.TargetRenderFps = 1; break;
                case Keys.F2: Config.TargetRenderFps = 30; break;
                case Keys.F3: Config.TargetRenderFps = 60; break;
                case Keys.F4: Config.TargetRenderFps = 120; break;
                case Keys.F5: Config.TargetRenderFps = 850; break;

                case Keys.Subtract:
                case Keys.OemMinus:
                    Config.TimeScale = Math.Max(0.5f, Config.TimeScale - 1f); // slow motion more
                    break;
                case Keys.Add:
                case Keys.Oemplus:
                    Config.TimeScale = Math.Min(60.0f, Config.TimeScale + 1f);
                    break;

                case Keys.C: SpawnRandomCircle(); break;
                case Keys.S: SpawnRandomBox(false); break;
                case Keys.D: SpawnRandomBox(true); break;
                case Keys.T: Config.EnableInterShapeCollisions = !Config.EnableInterShapeCollisions; break;
            }
        }

        private void SpawnRandomCircle()
        {
            float r = Config.DefaultSize;
            var c = new CircleBody(r, Config.DefaultMass, Config.DefaultFriction, Config.Restitution)
            {
                Position = new Vec2((float)Config.RNG.Next(50, _view.Width - 50), (float)Config.RNG.Next(50, _view.Height - 200)),
                Velocity = new Vec2((float)Config.RNG.Next(-50, 50), (float)Config.RNG.Next(-20, 20)),
                Color = Color.FromArgb(Config.RNG.Next(50, 230), Config.RNG.Next(50, 230), Config.RNG.Next(50, 230))
            };
            _world.Add(c);
        }

        private void SpawnRandomBox(bool diamond)
        {
            float hs = Config.DefaultSize;
            var b = new BoxBody(hs, Config.DefaultMass * 1.2f, Config.DefaultFriction * 1.2f, Config.Restitution * 0.9f, diamond)
            {
                Position = new Vec2((float)Config.RNG.Next(80, _view.Width - 80), (float)Config.RNG.Next(80, _view.Height - 250)),
                Velocity = new Vec2((float)Config.RNG.Next(-60, 60), (float)Config.RNG.Next(-30, 30)),
                Color = diamond ? Color.MediumVioletRed : Color.Goldenrod
            };
            _world.Add(b);
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}