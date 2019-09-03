using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using System.Drawing;

using cgimin.engine.object3d;
using cgimin.engine.texture;
using cgimin.engine.material.simpletexture;
using cgimin.engine.material.wobble1;
using cgimin.engine.material.simplereflection;
using cgimin.engine.camera;
using cgimin.engine.material.ambientdiffuse;
using cgimin.engine.light;



using OpenTK.Input;
using OpenTK.Graphics;
using System.Windows.Forms;
using System.Diagnostics;

namespace App.Main
{
    class Program : OpenTK.GameWindow
    {
        // Attributes
        private float angle = 0.0f;
        private int gw_width = 0;
        private int gw_height = 0;

        private Camera mainCam = new Camera();
        private Camera uiCam = new Camera();


        // Constants for scaling
        private const int SCALEX = 1000;
        private const int SCALEY = 1000;
        private const int SCALEZ = 1000;


        private GraphicsMode gm = new GraphicsMode(new ColorFormat(256, 256, 256, 0),
             32,
             8,
             16,
             new ColorFormat(256, 256, 256, 0)
             );

        private Controller controller;

        public Program()
        {
            // Call KeyboardEvent functions
            this.KeyDown += Window_KeyDown;
            this.KeyUp += Window_KeyUp;
            this.KeyPress += Window_KeyPress;

            // Init
            OpenTK.Toolkit.Init();

            this.CheckPrimaryDisplayRes();

            controller = new Controller(gw_width, gw_height, this, mainCam, uiCam);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            float aspect_ratio = Width / (float)Height;
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            controller.Update3D();
            controller.Update3DUI();
            controller.UpdateAnimations();
        }

        internal void Reload()
        {
            this.OnLoad(new EventArgs() { });
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Initialize Light
            Light.SetDirectionalLight(new Vector3(0, 0, 1.0f), new Vector4(0.1f, 0.1f, 0.1f, 0), new Vector4(1, 1, 1, 0), new Vector4(1, 1, 1, 0));

            controller.Load();
            CheckGLVersion();
        }

        private void CheckGLVersion()
        {
            // Check for necessary capabilities:
            Version version = new Version(GL.GetString(StringName.Version).Substring(0, 3));
            Version target = new Version(2, 0);
            if (version < target)
            {
                throw new NotSupportedException(String.Format(
                    "OpenGL {0} is required (you only have {1}).", target, version));
            }
        }

        private void DrawAll()
        {
            // 3D
            Make3D();
            Draw3D();

            // Transparent 3D elements
            Make3DTransparent();
            Draw3DTransparent();

            // 3D UI
            Make3DOnScreen();
            Draw3DOnScreen();

            // 2D
            Make2D();
            Draw2D();
        }

        private void Draw2D()
        {
            controller.CreateUI();
        }

        private void Draw3D()
        {
            controller.Draw3D();
        }

        private void Draw3DTransparent()
        {
            controller.Draw3DTransparent();
        }
        private void Draw3DOnScreen()
        {
            controller.Draw3DUI();
        }

        private void Make2D()
        {
            GL.Viewport(0, 0, Width, Height);

            // Disable shader programs
            GL.UseProgram(0);

            GL.Clear(ClearBufferMask.DepthBufferBit);

            GL.Disable(EnableCap.Lighting);
            GL.DepthMask(false);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc((BlendingFactor)BlendingFactorSrc.SrcAlpha, (BlendingFactor)BlendingFactorDest.OneMinusSrcAlpha);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();

            GL.LoadIdentity();

            GL.Ortho(-gw_width / 2, gw_width / 2, -gw_height / 2, gw_height / 2, -1, 1);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();

            GL.LoadIdentity();
        }



        private void Make3D()
        {
            GL.Viewport(0, 0, Width, Height);

            // Enable z-buffer
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.Enable(EnableCap.Lighting);

            // Backface culling enabled
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);

            GL.Disable(EnableCap.Blend);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
            GL.LoadIdentity();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        private void Make3DTransparent()
        {
            GL.Enable(EnableCap.Normalize);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc((BlendingFactor)BlendingFactorSrc.SrcAlpha, (BlendingFactor)BlendingFactorDest.OneMinusSrcAlpha);

        }

        private void Make3DOnScreen()
        {
            GL.Viewport((int)(Width - Width / 3.1f), 0, (int)(Width / 2.5f), (int)(Height / 2.5f));
            GL.Disable(EnableCap.Normalize);
            GL.Disable(EnableCap.Blend);
        }


        internal void ReUnload()
        {
            this.OnUnload(new EventArgs() { });
        }

        protected override void OnUnload(EventArgs e)
        {
            controller.Unload();
        }




        protected override void OnRenderFrame(FrameEventArgs e)
        {

            // FPS Counter in window title
            Title = $"Space Pong 3D @{1f / e.Time:0}FPS";

            // Let's start here
            this.DrawAll();

            GL.Flush();
            SwapBuffers();
        }



        private void Window_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F11:
                    if (WindowState != WindowState.Fullscreen)
                        WindowState = WindowState.Fullscreen;
                    else
                        WindowState = WindowState.Normal;
                    break;

                default:
                    controller.HandleInput(e);
                    break;
            }
        }

        private void Window_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                default:
                    controller.HandleInput_KeyUp(e);
                    break;
            }
        }

        private void Window_KeyPress(object sender, OpenTK.KeyPressEventArgs e)
        {

        }

        static void Main(string[] args)
        {

            using (App.Main.Program app = new App.Main.Program())
            {
                app.VSync = OpenTK.VSyncMode.On;
                app.Height = app.gw_height;
                app.Width = app.gw_width;
                app.WindowState = WindowState.Fullscreen;

                app.Icon = new Icon("../../icons/game.ico");
                app.Run(60.0, 60.0);
            }
        }

        private void CheckPrimaryDisplayRes() // Get Resolution from primary display for game window resolution
        {
            foreach (DisplayIndex index in Enum.GetValues(typeof(DisplayIndex)))
            {
                DisplayDevice device = DisplayDevice.GetDisplay(index);
                if (device == null)
                {
                    continue;
                }

                if (!device.IsPrimary) // ToDo: Error handling
                {
                    gw_width = 1280;
                    gw_height = 1024;
                }
                else
                {
                    gw_height = device.Height;
                    gw_width = device.Width;
                    if (gw_height > 1080)
                    {
                        gw_height = 1080;
                    }
                    if (gw_width > 1920)
                    {
                        gw_width = 1920;
                    }
                }
            }
        }
    }
}
