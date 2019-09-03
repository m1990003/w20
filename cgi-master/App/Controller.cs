using cgimin.engine.camera;
using cgimin.engine.skybox;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using System.Drawing;

using App.GUI;
using App.Game;
using OpenTK.Graphics;
using Engine.cgimin.engine.octree;

namespace App.Main
{
    class Controller
    {
        // window
        private int gw_width;
        private int gw_height;

        // objects
        private App.Game.Arena arena;
        private App.Game.Asteroid asteroid1;
        private App.Game.Asteroid asteroid2;
        private App.Game.Asteroid asteroid3;
        private App.Game.Asteroid asteroid4;
        private App.Game.SpaceShip spaceShip1;
        private App.Game.SpaceShip spaceShip2;
        private App.Game.SpaceShip spaceShip3;
        private App.Game.SpaceShip spaceShip4;
        private App.Game.Slider slider;
        private App.Game.Backgroundstation backgroundstation;
        private App.Game.Pong pong;
        private SkyBox skybox;

        // map objects
        private App.Game.Map map;
        private App.Game.Axis x_Axis;
        private App.Game.Axis y_Axis;
        private App.Game.Axis z_Axis;
        private App.Game.MapPong map_Pong;


        private int selectedIndex = 0;
        private Random random = new Random();
        private Matrix4 shipPosition;

        private Octree octree;


        // program states        
        private enum State
        {
            SPLASHSCREEN,
            MAINBEFORE,
            GAMEREADY,
            GAME,
            MAINAFTER,
            GAMEOVER
        }

        // Init state
        private State state = State.SPLASHSCREEN;

        public void setGameover()
        {
            state = State.GAMEOVER;
        }

        // Times
        private Stopwatch watch = new Stopwatch();
        private DateTime systemTime = new DateTime();
        private Stopwatch gameTime = new Stopwatch();


        private UI userInterface;
        private App.Main.Program program;
        private Camera mainCam = new Camera();
        private Camera uiCam = new Camera();

        public Controller(int gw_width, int gw_height, Program program, Camera mainCam, Camera uiCam)
        {
            watch.Start();
            this.gw_height = gw_height;
            this.gw_width = gw_width;
            this.program = program;
            this.mainCam = mainCam;
            this.uiCam = uiCam;

        }


        public void AnimateIntro()
        {
            if (state == State.MAINBEFORE)
            {
                moveCamera();

                // Rotation of Skydome
                arena.Obj.Transformation *= Matrix4.CreateRotationY(-0.001f);
                slider.Obj.Transformation *= Matrix4.CreateRotationY(-0.01f);

                pong.AnimateRotation();

                map.Obj.Transformation *= Matrix4.CreateRotationY(-0.001f);
                int noRotation = 0;
                foreach (var roid in octree.entities)
                {
                    if (noRotation != 0)
                    {
                        roid.Transform *= Matrix4.CreateRotationY(-0.001f);
                    }
                    else
                    {
                        noRotation++;
                    }
                }
            }
        }

        public void AnimateSplashScreen()
        {
            if (state == State.SPLASHSCREEN)
            {
                Panel t = (Panel)userInterface.IntroWindow.Children[0];

                t.Color = new Color4(255, 255, 255,
                  t.Color.A <= 255 ? t.Color.A + 0.005f : 0);

                userInterface.IntroWindow.Children[0] = t;

                if (watch.ElapsedMilliseconds > 7000)
                {
                    Text text = (Text)userInterface.IntroWindow.Children[1];

                    text.Color = new Color4(255, 255, 255, 255);

                    userInterface.IntroWindow.Children[1] = text;
                }
            }
        }

        public void AnimateStartGame()
        {
            if (state == State.GAMEREADY || state == State.GAME)
            {
                Text gameTimeText;

                // 1x
                if (state == State.GAMEREADY && gameTime.ElapsedMilliseconds <= 0)
                    moveCamera();

                if (systemTime.Second != DateTime.Now.Second)
                {

                    if (this.systemTime.ToString().Contains(':'))
                    {
                        this.systemTime = DateTime.Now;
                    }

                    UIParent uie = (UIParent)userInterface.GameWindow.Children[1];
                    Text systemDatetime = (Text)uie.Children[1];
                    systemDatetime.Content = systemDatetime.Content.ToString().Contains(':') ? systemTime.ToString().Replace(':', ' ') : systemTime.ToString();
                }

                // gameTime
                if (state == State.GAME)
                {
                    UIParent uie = (UIParent)userInterface.GameWindow.Children[1];
                    gameTimeText = (Text)uie.Children[3];
                    gameTimeText.Content = gameTime.Elapsed.ToString();
                }


                if (watch.ElapsedMilliseconds >= 6000 && state == State.GAMEREADY && mainCam.isMoving() == false)
                {
                    state = State.GAME;

                    gameTimeText = (Text)userInterface.GameWindow.Children[0];

                    gameTimeText.Content = " ";

                    gameTime.Start();
                }

                gameTimeText = (Text)userInterface.GameWindow.Children[0];


                if (watch.ElapsedMilliseconds <= 6000 && state == State.GAMEREADY)
                {
                    if (6000 - watch.ElapsedMilliseconds >= 1000)
                        gameTimeText.Content = (6000 - watch.ElapsedMilliseconds).ToString().Substring(0, 1);
                    else
                        gameTimeText.Content = "0";
                }

                // Always in background
                arena.Obj.Transformation *= Matrix4.CreateRotationY(-0.001f);
                int noRotation = 0;
                foreach (var roid in octree.entities)
                {
                    if (noRotation != 0)
                    {
                        roid.Transform *= Matrix4.CreateRotationY(-0.001f);
                    }
                    else
                    {
                        noRotation++;
                    }
                }

            }

        }


        internal void UpdateAnimations()
        {
            AnimateSplashScreen();
            AnimateIntro();
            AnimateStartGame();
        }


        public void Draw3DUI()
        {
            switch (state)
            {
                case State.MAINBEFORE:
                    break;
                case State.SPLASHSCREEN:
                    break;
                case State.GAMEREADY:
                case State.GAME:
                    map.Draw();
                    x_Axis.Draw();
                    y_Axis.Draw();
                    z_Axis.Draw();
                    map_Pong.Draw();
                    break;
            }
        }


        public void Update3DUI()
        {
            switch (state)
            {
                case State.MAINBEFORE:
                    break;
                case State.SPLASHSCREEN:
                    break;
                default:
                    // Update map cam position by slider position!!!
                    uiCam.SetLookAt((slider.SliderPosition * slider.DistanceCamSlider).Normalized() * 10f, new Vector3(0, 0, 0), mainCam.Cam_up);

                    // Rotate map arena
                    map.Obj.Transformation *= Matrix4.CreateRotationY(-0.001f);

                    // Update map pong position
                    map_Pong.Obj.Transformation =
                        Matrix4.CreateTranslation(pong.Obj.Transformation.ExtractTranslation() * 0.1f);
                    break;
            }

        }

        public void Draw3D()
        {
            skybox.Draw();

            // game objects
            slider.Draw();

            // Octree objects
            octree.Draw();
            pong.Draw();
        }

        public void Draw3DTransparent()
        {
            arena.Draw();
        }

        public void Update3D()
        {
            if (state == State.GAME)
            {
                slider.move();
                pong.move(slider.getPos());
                if (pong.GetGameStatus())
                {
                    state = State.GAMEOVER;
                    Text t = (Text)userInterface.GameOverWindow.Children[2];
                    t.Content = gameTime.Elapsed.ToString();
                }
            }
        }

        // OnUpdateFrame()
        private void moveCamera()
        {
            if (mainCam.isMoving())
                mainCam.Move();
            mainCam.DirectionTo();
        }

        internal void HandleInput(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            UIParent uie;
            Key key = e.Key;

            switch (state)
            {

                case State.SPLASHSCREEN:

                    if (watch.ElapsedMilliseconds > 7000)
                    {
                        watch.Stop();

                        state = State.MAINBEFORE;

                        mainCam.Init_Move(new Vector3(2, 0, 80), Camera.MoveType.Linear);
                        mainCam.Init_DirectionTo(new Vector3(0, 0, 0), Camera.MoveType.Linear);
                        watch.Reset();
                    }
                    break;


                case State.MAINBEFORE:

                    uie = (UIParent)userInterface.MainWindow.Children[3];

                    switch (key)
                    {
                        case Key.Up:

                            selectedIndex = selectedIndex == 0 ? uie.Children.Count - 1 : --selectedIndex;

                            for (int i = 0; i < uie.Children.Count; i++)
                            {

                                UIParent button = (UIParent)uie.Children[i];
                                button.Color = i == selectedIndex ? Color4.Blue : Color4.White;
                                Text text = (Text)button.Children[0];
                                text.Color = i == selectedIndex ? Color4.Blue : Color4.White;
                                text = (Text)button.Children[1];
                                text.Color = i == selectedIndex ? Color4.Blue : Color4.White;
                            }
                            break;

                        case Key.Down:
                            selectedIndex = selectedIndex == uie.Children.Count - 1 ? 0 : ++selectedIndex;
                            for (int i = 0; i < uie.Children.Count; i++)
                            {
                                UIParent button = (UIParent)uie.Children[i];
                                button.Color = i == selectedIndex ? Color4.Blue : Color4.White;
                                Text text = (Text)button.Children[0];
                                text.Color = i == selectedIndex ? Color4.Blue : Color4.White;
                                text = (Text)button.Children[1];
                                text.Color = i == selectedIndex ? Color4.Blue : Color4.White;

                            }
                            break;


                        case Key.Enter:

                            switch (selectedIndex)
                            {
                                case 0:
                                    state = State.GAMEREADY;
                                    mainCam.SetSpeed(0.3f, 0.3f, 0.3f);

                                    slider.SliderPosition = slider.Obj.Transformation.ExtractTranslation();
                                    slider.CamMatrix = slider.Obj.Transformation;

                                    Vector3 camPosition = slider.CamMatrix.ExtractTranslation();
                                    camPosition =
                                        new Vector3(camPosition.X, camPosition.Y + 10, camPosition.Z);

                                    slider.CamMatrix = Matrix4.CreateTranslation(camPosition);

                                    mainCam.Init_Move(camPosition * slider.DistanceCamSlider, Camera.MoveType.Linear);
                                    mainCam.Init_DirectionTo(new Vector3(0, 0, 0), Camera.MoveType.Linear);

                                    watch.Reset();
                                    watch.Start();

                                    selectedIndex = 0;
                                    break;
                                case 1:
                                    program.Exit();
                                    break;
                            }
                            break;
                    }
                    break;


                case State.GAMEREADY:
                case State.GAME:
                    switch (key)
                    {
                        case Key.Escape:
                            gameTime.Stop();
                            state = State.MAINAFTER;
                            break;
                        default:

                            if (!mainCam.isMoving() && state == State.GAME)
                                slider.initMove(e);
                            break;
                    }
                    break;

                case State.MAINAFTER:

                    uie = (UIParent)userInterface.GameMenuWindow.Children[3];
                    switch (key)
                    {
                        case Key.Up:

                            selectedIndex = selectedIndex == 0 ? uie.Children.Count - 1 : --selectedIndex;

                            for (int i = 0; i < uie.Children.Count; i++)
                            {

                                UIParent button = (UIParent)uie.Children[i];
                                button.Color = i == selectedIndex ? Color4.Blue : Color4.White;
                                Text text = (Text)button.Children[0];
                                text.Color = i == selectedIndex ? Color4.Blue : Color4.White;
                                text = (Text)button.Children[1];
                                text.Color = i == selectedIndex ? Color4.Blue : Color4.White;
                            }
                            break;

                        case Key.Down:
                            selectedIndex = selectedIndex == uie.Children.Count - 1 ? 0 : ++selectedIndex;
                            for (int i = 0; i < uie.Children.Count; i++)
                            {
                                UIParent button = (UIParent)uie.Children[i];
                                button.Color = i == selectedIndex ? Color4.Blue : Color4.White;
                                Text text = (Text)button.Children[0];
                                text.Color = i == selectedIndex ? Color4.Blue : Color4.White;
                                text = (Text)button.Children[1];
                                text.Color = i == selectedIndex ? Color4.Blue : Color4.White;

                            }
                            break;

                        case Key.Enter:

                            switch (selectedIndex)
                            {
                                case 0:
                                    // Resume game
                                    state = State.GAMEREADY;
                                    watch.Restart();
                                    selectedIndex = 0;
                                    break;

                                case 1:

                                    // Reset
                                    program.ReUnload();
                                    program.Reload();
                                    selectedIndex = 0;

                                    // End game
                                    state = State.SPLASHSCREEN;
                                    break;
                                case 2:
                                    // Exit game
                                    program.Exit();
                                    break;
                            }
                            break;

                        case Key.Escape:
                            // Resume game
                            state = State.GAMEREADY;
                            watch.Restart();
                            selectedIndex = 0;
                            break;
                    }
                    break;


                case State.GAMEOVER:

                    uie = (UIParent)userInterface.GameOverWindow.Children[4];
                    switch (key)
                    {
                        case Key.Up:

                            selectedIndex = selectedIndex == 0 ? uie.Children.Count - 1 : --selectedIndex;

                            for (int i = 0; i < uie.Children.Count; i++)
                            {
                                UIParent button = (UIParent)uie.Children[i];
                                button.Color = i == selectedIndex ? Color4.Blue : Color4.White;
                                Text text = (Text)button.Children[0];
                                text.Color = i == selectedIndex ? Color4.Blue : Color4.White;
                                text = (Text)button.Children[1];
                                text.Color = i == selectedIndex ? Color4.Blue : Color4.White;
                            }
                            break;

                        case Key.Down:
                            selectedIndex = selectedIndex == uie.Children.Count - 1 ? 0 : ++selectedIndex;
                            for (int i = 0; i < uie.Children.Count; i++)
                            {
                                UIParent button = (UIParent)uie.Children[i];
                                button.Color = i == selectedIndex ? Color4.Blue : Color4.White;
                                Text text = (Text)button.Children[0];
                                text.Color = i == selectedIndex ? Color4.Blue : Color4.White;
                                text = (Text)button.Children[1];
                                text.Color = i == selectedIndex ? Color4.Blue : Color4.White;

                            }
                            break;

                        case Key.Enter:

                            switch (selectedIndex)
                            {
                                case 0:
                                    // Restart game

                                    // Reset
                                    program.ReUnload();
                                    program.Reload();
                                    selectedIndex = 0;


                                    state = State.GAMEREADY;
                                    mainCam.SetSpeed(0.3f, 0.3f, 0.3f);


                                    slider.SliderPosition = slider.Obj.Transformation.ExtractTranslation();
                                    slider.CamMatrix = slider.Obj.Transformation;

                                    Vector3 camPosition = slider.CamMatrix.ExtractTranslation();
                                    camPosition =
                                        new Vector3(camPosition.X, camPosition.Y + 10, camPosition.Z);

                                    slider.CamMatrix = Matrix4.CreateTranslation(camPosition);

                                    mainCam.Init_Move(camPosition * slider.DistanceCamSlider, Camera.MoveType.Linear);
                                    mainCam.Init_DirectionTo(new Vector3(0, 0, 0), Camera.MoveType.Linear);

                                    selectedIndex = 0;
                                    break;

                                case 1:

                                    // Reset
                                    program.ReUnload();
                                    program.Reload();
                                    selectedIndex = 0;

                                    // End game
                                    state = State.SPLASHSCREEN;
                                    break;
                                case 2:
                                    // Exit game
                                    program.Exit();
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }

        // Outside draw
        internal void HandleInput_KeyUp(KeyboardKeyEventArgs e)
        {
            switch (state)
            {
                case State.GAME:
                    switch (e.Key)
                    {
                        case Key.Left:
                        case Key.Right:
                        case Key.A:
                        case Key.D:
                            slider.EndMoveLR();
                            break;
                    }
                    switch (e.Key)
                    {
                        case Key.Up:
                        case Key.Down:
                        case Key.W:
                        case Key.S:
                            slider.EndMoveUD();
                            break;
                    }
                    break;
            }
        }

        // Only draw2D
        internal void CreateUI()
        {
            // draw menu
            switch (state)
            {
                case State.SPLASHSCREEN:
                    userInterface.Draw(userInterface.IntroWindow);
                    break;
                case State.MAINBEFORE:
                    userInterface.Draw(userInterface.MainWindow);
                    break;
                case State.GAMEREADY:
                case State.GAME:
                    userInterface.Draw(userInterface.GameWindow);
                    break;
                case State.MAINAFTER:
                    userInterface.Draw(userInterface.GameMenuWindow);
                    break;
                case State.GAMEOVER:
                    userInterface.Draw(userInterface.GameOverWindow);
                    break;
            }
        }


        internal void Load()
        {

            // Initialize camera perspective 
            // TopView
            mainCam.Init();
            mainCam.SetSpeed(0.05f, 0.05f, 0.05f);
            mainCam.SetLookAt(new Vector3(2, 0, 0), new Vector3(0, 0, 0), Vector3.UnitY);

            mainCam.SetWidthHeightFov(gw_width, gw_height, 100);
            uiCam.Init();
            uiCam.SetWidthHeightFov(gw_width, gw_height, 100);

            // Loading objects
            this.LoadObjects();

            // Initialize camera perspective 
            // TopView

            uiCam.SetLookAt(new Vector3(2, 0, 0), new Vector3(0, 0, 0), Vector3.UnitY);


            userInterface = new UI(gw_width, gw_height);
            gameTime.Reset();
            watch.Reset();
            watch.Start();

        }


        internal void LoadObjects()
        {

            // Init Octree
            octree = new Octree(new Vector3(-500, -500, -500), new Vector3(500, 500, 500), mainCam);

            // Init Skybox
            skybox = new SkyBox("earth_skybox/front.png", "earth_skybox/back.png", "earth_skybox/left.png", "earth_skybox/right.png", "earth_skybox/down.png", "earth_skybox/up.png", mainCam);

            arena = new Arena(mainCam);
            asteroid1 = new Asteroid(mainCam, 0.01f);
            asteroid2 = new Asteroid(mainCam, 0.023f);
            asteroid3 = new Asteroid(mainCam, 0.03f);
            asteroid4 = new Asteroid(mainCam, 0.04f);
            spaceShip1 = new SpaceShip(mainCam, 10.0f, SpaceShip.Color.STANDARD);
            spaceShip2 = new SpaceShip(mainCam, 25.0f, SpaceShip.Color.RED);
            spaceShip3 = new SpaceShip(mainCam, 18.0f, SpaceShip.Color.GREEN);
            spaceShip4 = new SpaceShip(mainCam, 15.0f, SpaceShip.Color.BLUE);
            slider = new Slider(this.mainCam);
            backgroundstation = new Backgroundstation(mainCam);
            pong = new Pong(mainCam);


            map = new Map(uiCam);
            x_Axis = new Axis(uiCam);
            y_Axis = new Axis(uiCam);
            z_Axis = new Axis(uiCam);
            map_Pong = new MapPong(uiCam);


            // Update
            y_Axis.Obj.Transformation *= Matrix4.CreateRotationZ((float)Math.PI / 2);
            z_Axis.Obj.Transformation *= Matrix4.CreateRotationY((float)Math.PI / 2);


            // Add octree entities --> background objects
            octree.AddEntity(new OctreeEntity(backgroundstation.Obj, backgroundstation.Material, backgroundstation.MaterialSettings, backgroundstation.Obj.Transformation));

            Matrix4 asteroidPosition = Matrix4.CreateTranslation(-60.0f, -60.0f, -60.0f);
            int counter;
            int roidCounter = 0;

            for (int i = 0; i < 30; i++) // Add 30 asteroid in a random pattern without overlapping
            {
                while (true)
                {
                    counter = -1;
                    foreach (var roid in octree.entities)
                    {
                        if (Vector3.Distance(roid.Transform.ExtractTranslation(), asteroidPosition.ExtractTranslation()) > 15.0f)
                        {
                            counter++;
                        }
                    }
                    if (counter < i)
                    {
                        asteroidPosition = Matrix4.CreateTranslation(-100.0f + random.Next(-30, 30), -100.0f + random.Next(-30, 30), -100.0f + random.Next(-30, 30));
                    }
                    else
                    {
                        break;
                    }

                }

                RoidSwitch(roidCounter, octree, asteroidPosition);
                if (roidCounter < 3)
                {
                    roidCounter++;
                }
                else
                {
                    roidCounter = 0;
                }
            }

            RoidCopy(octree, 50.0f, 230.0f, 400.0f);
            RoidCopy(octree, 230, 400.0f, 230.0f);
            RoidCopy(octree, 230, 50.0f, 230.0f);
            RoidCopy(octree, -230, 50.0f, -230.0f);

            shipPosition = Matrix4.CreateTranslation(-100.0f, 50.0f, -100.0f);
            octree.AddEntity(new OctreeEntity(spaceShip1.Obj, spaceShip1.Material, spaceShip1.MaterialSettings, shipPosition));
            shipPosition = Matrix4.CreateTranslation(0.0f, 0.0f, 100.0f);
            octree.AddEntity(new OctreeEntity(spaceShip2.Obj, spaceShip2.Material, spaceShip2.MaterialSettings, shipPosition));
            shipPosition = Matrix4.CreateTranslation(-100.0f, 50.0f, 0.0f);
            octree.AddEntity(new OctreeEntity(spaceShip3.Obj, spaceShip3.Material, spaceShip3.MaterialSettings, shipPosition));
            shipPosition = Matrix4.CreateTranslation(100.0f, 50.0f, -100.0f);
            octree.AddEntity(new OctreeEntity(spaceShip4.Obj, spaceShip4.Material, spaceShip4.MaterialSettings, shipPosition));
        }

        internal void Unload()
        {
            UnloadObjects();
        }

        private void RoidCopy(Octree octree, float x, float y, float z) // Handle another asteroid cluster by copiing the random cluster and adding x, y, z values
        {
            int roidCounter = 0;
            Matrix4 asteroidPosition = new Matrix4();

            Vector3 roid_vec = new Vector3();


            for (int roidCntr = 1; roidCntr <= 29; roidCntr++)
            {
                roid_vec = octree.entities[roidCntr].Transform.ExtractTranslation();
                asteroidPosition = Matrix4.CreateTranslation(roid_vec.X + x, roid_vec.Y + y, roid_vec.Z + z);
                RoidSwitch(roidCounter, octree, asteroidPosition);
                if (roidCounter < 3)
                {
                    roidCounter++;
                } else
                {
                    roidCounter = 0;
                }

            }
        }

        private void RoidSwitch(int roidCounter, Octree octree, Matrix4 asteroidPosition) // Switch between the four asteroid types and add them to the octree
        {
            switch (roidCounter)
            {
                case 0:
                    octree.AddEntity(new OctreeEntity(asteroid1.Obj, asteroid1.Material, asteroid1.MaterialSettings, asteroidPosition));
                    roidCounter++;
                    break;
                case 1:
                    octree.AddEntity(new OctreeEntity(asteroid2.Obj, asteroid2.Material, asteroid2.MaterialSettings, asteroidPosition));
                    roidCounter++;
                    break;
                case 2:
                    octree.AddEntity(new OctreeEntity(asteroid3.Obj, asteroid3.Material, asteroid3.MaterialSettings, asteroidPosition));
                    roidCounter++;
                    break;
                case 3:
                    octree.AddEntity(new OctreeEntity(asteroid4.Obj, asteroid4.Material, asteroid4.MaterialSettings, asteroidPosition));
                    roidCounter = 0;
                    break;
            }
        }

        internal void UnloadObjects()
        {

            userInterface = null;
            octree = null;

            arena.Obj.UnLoad();
            slider.Obj.UnLoad();
            backgroundstation.Obj.UnLoad();


            map.Obj.UnLoad();
            y_Axis.Obj.UnLoad();
            z_Axis.Obj.UnLoad();
            x_Axis.Obj.UnLoad();
            map_Pong.Obj.UnLoad();



            GC.Collect();
            GC.WaitForPendingFinalizers();

        }
    }

}