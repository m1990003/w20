using cgimin.engine.object3d;
using cgimin.engine.texture;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.GUI
{
    public class UI
    {
        private int titleTexture;


        // Different grids
        private UIParent introGrid;
        private UIParent mainGrid;
        private UIParent gameGrid;
        private UIParent gameMenuGrid;
        private UIParent gameOverGrid;


        private int gw_width;
        private int gw_height;

        public UI(int gw_width, int gw_height)
        {

            this.gw_width = gw_width;
            this.gw_height = gw_height;

            this.CreateIntroWindow();
            this.CreateMainWindow();
            this.CreateGameWindow();
            this.CreateGameMenuWindow();
            this.CreateGameOverWindow();

        }

        public UIParent IntroWindow { get => introGrid; set => introGrid = value; }
        public UIParent MainWindow { get => mainGrid; set => mainGrid = value; }
        public UIParent GameWindow { get => gameGrid; set => gameGrid = value; }
        public UIParent GameMenuWindow { get => gameMenuGrid; set => gameMenuGrid = value; }
        public UIParent GameOverWindow { get => gameOverGrid; set => gameOverGrid = value; }


        public void CreateIntroWindow()
        {
            introGrid = new Grid();
            introGrid.Position_x = 0;
            introGrid.Position_y = 0;
            introGrid.Width = gw_width;
            introGrid.Height = gw_height;
            introGrid.Texture = TextureManager.LoadTexture("ui/simpleWhiteTexture.png");
            introGrid.Color = new Color4(255, 255, 255, 255);


            introGrid.Children.Add(new Panel()
            {
                Position_x = 0,
                Position_y = 0,
                Width = gw_width,
                Height = gw_height,
                Texture = TextureManager.LoadTexture("ui/introBackground.png"),
                Color = new Color4(255, 255, 255, 0),
            });


            introGrid.Children.Add(new Text()
            {
                Position_x = 0,
                Position_y = -gw_height / 2 + 48,
                Width = 180,
                Height = 48,
                TextSize = 24,
                FontStyle = FontStyle.Regular,
                Content = "Press Button",
                Color = new Color4(255, 255, 255, 0),
            });
        }

        public void CreateMainWindow()
        {

            mainGrid = new Grid();
            mainGrid.Position_x = 0;
            mainGrid.Position_y = 0;
            mainGrid.Width = gw_width;
            mainGrid.Height = gw_height;

            mainGrid.Texture = TextureManager.LoadTexture("ui/transparentBackground.png");

            //0
            mainGrid.Children.Add(new Text()
            {
                Position_x = 0,
                Position_y = (int)(gw_height * 0.46f),
                Width = gw_width,
                Height = gw_height,
                TextSize = 60,
                FontStyle = FontStyle.Regular,
                Content = "S    P    A    C    E",
            });

            //1
            mainGrid.Children.Add(new Text()
            {
                Position_x = 0,
                Position_y = (int)(gw_height * 0.37f),
                Width = gw_width,
                Height = gw_height,
                TextSize = 90,
                FontStyle = FontStyle.Regular,
                Content = "P     O     N     G"
            });

            //2
            mainGrid.Children.Add(new Text()
            {
                Position_x = 0,
                Position_y = (int)(gw_height * 0.25f),
                Width = gw_width,
                Height = gw_height,
                TextSize = 140,
                FontStyle = FontStyle.Bold,
                Content = "3 D"
            });

            //3
            Panel mainPanel = new Panel();
            mainPanel.Position_x = 0;
            mainPanel.Position_y = (int)(gw_height * -0.16f);
            mainPanel.Width = (int)(gw_width * 0.8f);
            mainPanel.Height = (int)(gw_height * 0.46f);
            mainPanel.Texture = TextureManager.LoadTexture("ui/rectanglePanel.png");
            mainGrid.Children.Add(mainPanel);


            Button button = new Button();


            button.Position_x = 0;
            button.Position_y = (int)(gw_height * -0.02f); ;
            button.Width = (int)(gw_width * 0.43f);
            button.Height = 200;

            Text t = new Text()
            {
                Position_x = -160,
                Position_y = (int)(gw_height * -0.02f) + 10,
                Width = (int)(gw_width * 0.47f) / 2,
                Height = 200,
                TypeSelected = Text.Type.SEGOEMDL2,
                TextSize = 40,
                FontStyle = FontStyle.Regular,
                Content = "\xE768",
            };

            button.Children.Add(t);

            button.Children.Add(new Text()
            {
                Position_x = t.ContentSize,
                Position_y = (int)(gw_height * -0.02f) + 20,
                Width = (int)(gw_width * 0.47f) / 2,
                Height = 200,
                TextSize = 40,
                FontStyle = FontStyle.Regular,
                Content = "Start Game",
            });

            // 3 - 0
            mainPanel.Children.Add(button);

            button = new Button();

            button.Position_x = 0;
            button.Position_y = (int)(gw_height * -0.22f);
            button.Width = (int)(gw_width * 0.43f);
            button.Height = 200;

            t = new Text()
            {
                Position_x = -160,
                Position_y = (int)(gw_height * -0.22f) + 10,
                Width = (int)(gw_width * 0.47f) / 2,
                Height = 200,
                TypeSelected = Text.Type.SEGOEMDL2,
                TextSize = 40,
                FontStyle = FontStyle.Regular,
                Content = "\xE80F",
            };

            button.Children.Add(t);

            button.Children.Add(new Text()
            {
                Position_x = t.ContentSize,
                Position_y = (int)(gw_height * -0.22f) + 20,
                Width = (int)(gw_width * 0.47f) / 2,
                Height = 200,
                TextSize = 40,
                FontStyle = FontStyle.Regular,
                Content = "Exit Game",
            });

            // 3 - 1
            mainPanel.Children.Add(button);
        }




        public void CreateGameWindow()
        {
            gameGrid = new Grid();
            gameGrid.Position_x = 0;
            gameGrid.Position_y = 0;
            gameGrid.Width = gw_width;
            gameGrid.Height = gw_height;
            gameGrid.Color = new Color4(255, 255, 255, 0);


            Text startCounter = new Text()
            {
                Position_x = 0,
                Position_y = 192 / 2,
                Width = 1920 / 2,
                Height = 1080 / 2,
                Color = new Color4(0, 0, 255, 255),
                TextSize = 192,
                FontStyle = FontStyle.Regular,
                Content = "5",
            };


            gameGrid.Children.Add(startCounter);

            //0
            Panel timePanel = new Panel();
            timePanel.Position_x = gw_width / 2 - (int)(gw_width * 0.18f) / 2;
            timePanel.Position_y = (int)gw_height / 2 - (int)(gw_height * 0.13f) / 2;
            timePanel.Width = (int)(gw_width * 0.18f);
            timePanel.Height = (int)(gw_height * 0.13f);
            timePanel.Texture = TextureManager.LoadTexture("ui/rectanglePanel.png");
            timePanel.Color = new Color4(255, 255, 255, 255);

            Panel backgroundPanel = new Panel();
            backgroundPanel.Position_x = gw_width / 2 - (int)(gw_width * 0.18f) / 2;
            backgroundPanel.Position_y = (int)gw_height / 2 - (int)(gw_height * 0.13f) / 2;
            backgroundPanel.Width = (int)(gw_width * 0.18f) - 5;
            backgroundPanel.Height = (int)(gw_height * 0.13f) - 5;
            backgroundPanel.Texture = TextureManager.LoadTexture("ui/transparentBackground.png");


            timePanel.Children.Add(backgroundPanel);

            gameGrid.Children.Add(timePanel);


            Text str = new Text()
            {
                Position_x = 0,
                Position_y = 0,
                Width = 600,
                Height = 48,
                Color = new Color4(255, 255, 255, 255),
                TextSize = 24,
                FontStyle = FontStyle.Regular,
                Content = DateTime.Now.ToString(),
            };

            str.Position_x = gw_width / 2 - str.ContentSize / 2;
            str.Position_y = gw_height / 2 - str.TextSize / 2;

            timePanel.Children.Add(str);

            Text icon = new Text()
            {
                Position_x = 0,
                Position_y = 0,
                Width = 600,
                Height = 48,
                TypeSelected = Text.Type.SEGOEMDL2,
                Color = new Color4(255, 255, 255, 255),
                TextSize = 24,
                FontStyle = FontStyle.Regular,
                Content = "\xE909",
            };

            icon.Position_x = gw_width / 2 - icon.ContentSize / 2 - str.ContentSize;
            icon.Position_y = gw_height / 2 - icon.TextSize / 2 - 10;

            timePanel.Children.Add(icon);




            str = new Text()
            {
                Position_x = 0,
                Position_y = 0,
                Width = 600,
                Height = 48,
                Color = new Color4(0, 0, 255, 255),
                TextSize = 24,
                FontStyle = FontStyle.Regular,
                Content = "00:00:00.0000000",
            };

            str.Position_x = gw_width / 2 - str.ContentSize / 2;
            str.Position_y = gw_height / 2 - str.TextSize / 2 - 48;

            timePanel.Children.Add(str);

            icon = new Text()
            {
                Position_x = 0,
                Position_y = 0,
                Width = 600,
                Height = 48,
                TypeSelected = Text.Type.SEGOEMDL2,
                Color = new Color4(0, 0, 255, 255),
                TextSize = 24,
                FontStyle = FontStyle.Regular,
                Content = "\xE916",
            };

            icon.Position_x = gw_width / 2 - icon.ContentSize / 2 - str.ContentSize;
            icon.Position_y = gw_height / 2 - icon.TextSize / 2 - 10 - 48;

            timePanel.Children.Add(icon);







            //0
            Panel mapPanel = new Panel();
            mapPanel.Position_x = gw_width / 2 - (int)(gw_width * 0.25f) / 2;
            mapPanel.Position_y = (int)(gw_height * 0.4f) / 2 - (int)gw_height / 2;
            mapPanel.Width = (int)(gw_width * 0.25f);
            mapPanel.Height = (int)(gw_height * 0.4f);
            mapPanel.Texture = TextureManager.LoadTexture("ui/diamondPanel.png");
            mapPanel.Color = new Color4(255, 255, 255, 255);


            icon = new Text()
            {
                Position_x = gw_width / 2 - (int)(gw_width * 0.25f) / 2,
                Position_y = (int)(gw_height * 0.7f) / 2 - (int)gw_height / 2,
                Width = 100,
                Height = 48,
                TypeSelected = Text.Type.SEGOEMDL2,
                Color = new Color4(0, 0, 255, 255),
                TextSize = 24,
                FontStyle = FontStyle.Regular,
                Content = "\xE774",
            };

            mapPanel.Children.Add(icon);
            gameGrid.Children.Add(mapPanel);

        }

        private void CreateGameMenuWindow()
        {

            gameMenuGrid = new Grid();
            gameMenuGrid.Position_x = 0;
            gameMenuGrid.Position_y = 0;
            gameMenuGrid.Width = gw_width;
            gameMenuGrid.Height = gw_height;

            gameMenuGrid.Texture = TextureManager.LoadTexture("ui/transparentBackground.png");

            //0
            gameMenuGrid.Children.Add(new Text()
            {
                Position_x = 0,
                Position_y = (int)(gw_height * 0.46f),
                Width = gw_width,
                Height = gw_height,
                TextSize = 60,
                FontStyle = FontStyle.Regular,
                Content = "S    P    A    C    E",
            });

            //1
            gameMenuGrid.Children.Add(new Text()
            {
                Position_x = 0,
                Position_y = (int)(gw_height * 0.37f),
                Width = gw_width,
                Height = gw_height,
                TextSize = 90,
                FontStyle = FontStyle.Regular,
                Content = "P     O     N     G"
            });

            //2
            gameMenuGrid.Children.Add(new Text()
            {
                Position_x = 0,
                Position_y = (int)(gw_height * 0.25f),
                Width = gw_width,
                Height = gw_height,
                TextSize = 140,
                FontStyle = FontStyle.Bold,
                Content = "3 D"
            });

            //3
            Panel mainPanel = new Panel();
            mainPanel.Position_x = 0;
            mainPanel.Position_y = (int)(gw_height * -0.16f);
            mainPanel.Width = (int)(gw_width * 0.8f);
            mainPanel.Height = (int)(gw_height * 0.46f);
            mainPanel.Texture = TextureManager.LoadTexture("ui/rectanglePanel.png");
            gameMenuGrid.Children.Add(mainPanel);


            Button button = new Button();

            button.Position_x = 0;
            button.Position_y = (int)(gw_height * -0.02f); ;
            button.Width = (int)(gw_width * 0.43f);
            button.Height = 200;

            Text t = new Text()
            {
                Position_x = -160,
                Position_y = (int)(gw_height * -0.02f) + 10,
                Width = (int)(gw_width * 0.47f) / 2,
                Height = 200,
                TypeSelected = Text.Type.SEGOEMDL2,
                TextSize = 40,
                FontStyle = FontStyle.Regular,
                Content = "\xE768",
            };

            button.Children.Add(t);

            button.Children.Add(new Text()
            {
                Position_x = t.ContentSize,
                Position_y = (int)(gw_height * -0.02f) + 20,
                Width = (int)(gw_width * 0.47f) / 2,
                Height = 200,
                TextSize = 40,
                FontStyle = FontStyle.Regular,
                Content = "Resume Game",
            });

            // 3 - 0
            mainPanel.Children.Add(button);


            button = new Button();

            button.Position_x = 0;
            button.Position_y = (int)(gw_height * -0.17f); ;
            button.Width = (int)(gw_width * 0.43f);
            button.Height = 200;

            t = new Text()
            {
                Position_x = -160,
                Position_y = (int)(gw_height * -0.17f) + 10,
                Width = (int)(gw_width * 0.47f) / 2,
                Height = 200,
                TypeSelected = Text.Type.SEGOEMDL2,
                TextSize = 40,
                FontStyle = FontStyle.Regular,
                Content = "\xE71A",
            };

            button.Children.Add(t);

            button.Children.Add(new Text()
            {
                Position_x = t.ContentSize,
                Position_y = (int)(gw_height * -0.17f) + 20,
                Width = (int)(gw_width * 0.47f) / 2,
                Height = 200,
                TextSize = 40,
                FontStyle = FontStyle.Regular,
                Content = "End Game",
            });

            // 3 - 0
            mainPanel.Children.Add(button);


            button = new Button();
            button.Position_x = 0;
            button.Position_y = (int)(gw_height * -0.32f);
            button.Width = (int)(gw_width * 0.43f);
            button.Height = 200;

            t = new Text()
            {
                Position_x = -160,
                Position_y = (int)(gw_height * -0.32f) + 10,
                Width = (int)(gw_width * 0.47f) / 2,
                Height = 200,
                TypeSelected = Text.Type.SEGOEMDL2,
                TextSize = 40,
                FontStyle = FontStyle.Regular,
                Content = "\xE80F",
            };

            button.Children.Add(t);

            button.Children.Add(new Text()
            {
                Position_x = t.ContentSize,
                Position_y = (int)(gw_height * -0.32f) + 20,
                Width = (int)(gw_width * 0.47f) / 2,
                Height = 200,
                TextSize = 40,
                FontStyle = FontStyle.Regular,
                Content = "Exit Game",
            });

            // 3 - 1
            mainPanel.Children.Add(button);
        }


        private void CreateGameOverWindow()
        {


            gameOverGrid = new Grid();
            gameOverGrid.Position_x = 0;
            gameOverGrid.Position_y = 0;
            gameOverGrid.Width = gw_width;
            gameOverGrid.Height = gw_height;

            gameOverGrid.Texture = TextureManager.LoadTexture("ui/transparentBackground.png");

            //0
            gameOverGrid.Children.Add(new Text()
            {
                Position_x = 0,
                Position_y = (int)(gw_height * 0.46f),
                Width = gw_width,
                Height = gw_height,
                TextSize = 60,
                FontStyle = FontStyle.Regular,
                Content = "G    A    M    E",
            });

            //1
            gameOverGrid.Children.Add(new Text()
            {
                Position_x = 0,
                Position_y = (int)(gw_height * 0.37f),
                Width = gw_width,
                Height = gw_height,
                TextSize = 90,
                FontStyle = FontStyle.Regular,
                Content = "O     V     E     R"
            });

            //2
            Text gameTime = new Text()
            {
                Position_x = 48,
                Position_y = (int)(gw_height * 0.18f),
                Width = 600,
                Height = 96,
                Color = new Color4(0, 0, 255, 255),
                TextSize = 48,
                FontStyle = FontStyle.Regular,
                Content = "00:00:00.0000000",
            };

            gameOverGrid.Children.Add(gameTime);

            //3
            Text icon = new Text()
            {
                Position_x = 48,
                Position_y = (int)(gw_height * 0.18f),
                Width = 600,
                Height = 96,
                TypeSelected = Text.Type.SEGOEMDL2,
                Color = new Color4(0, 0, 255, 255),
                TextSize = 48,
                FontStyle = FontStyle.Regular,
                Content = "\xE916",
            };

            icon.Position_x += -icon.ContentSize / 2 - gameTime.ContentSize / 2;
            icon.Position_y -= 10;

            gameOverGrid.Children.Add(icon);


            //4
            Panel mainPanel = new Panel();
            mainPanel.Position_x = 0;
            mainPanel.Position_y = (int)(gw_height * -0.16f);
            mainPanel.Width = (int)(gw_width * 0.8f);
            mainPanel.Height = (int)(gw_height * 0.46f);
            mainPanel.Color = new Color4(255, 255, 255, 255);
            mainPanel.Texture = TextureManager.LoadTexture("ui/rectanglePanel.png");
            gameOverGrid.Children.Add(mainPanel);


            Button button = new Button();

            button.Position_x = 0;
            button.Position_y = (int)(gw_height * -0.02f); ;
            button.Width = (int)(gw_width * 0.43f);
            button.Height = 200;

            Text t = new Text()
            {
                Position_x = -160,
                Position_y = (int)(gw_height * -0.02f) + 10,
                Width = (int)(gw_width * 0.47f) / 2,
                Height = 200,
                TypeSelected = Text.Type.SEGOEMDL2,
                TextSize = 40,
                FontStyle = FontStyle.Regular,
                Content = "\xE777",
            };

            button.Children.Add(t);

            button.Children.Add(new Text()
            {
                Position_x = t.ContentSize,
                Position_y = (int)(gw_height * -0.02f) + 20,
                Width = (int)(gw_width * 0.47f) / 2,
                Height = 200,
                TextSize = 40,
                FontStyle = FontStyle.Regular,
                Content = "Restart Game",
            });

            // 3 - 0
            mainPanel.Children.Add(button);

            button = new Button();
            button.Position_x = 0;
            button.Position_y = (int)(gw_height * -0.17f); ;
            button.Width = (int)(gw_width * 0.43f);
            button.Height = 200;

            t = new Text()
            {
                Position_x = -160,
                Position_y = (int)(gw_height * -0.17f) + 10,
                Width = (int)(gw_width * 0.47f) / 2,
                Height = 200,
                TypeSelected = Text.Type.SEGOEMDL2,
                TextSize = 40,
                FontStyle = FontStyle.Regular,
                Content = "\xE830",
            };

            button.Children.Add(t);

            button.Children.Add(new Text()
            {
                Position_x = t.ContentSize,
                Position_y = (int)(gw_height * -0.17f) + 20,
                Width = (int)(gw_width * 0.47f) / 2,
                Height = 200,
                TextSize = 40,
                FontStyle = FontStyle.Regular,
                Content = "Main Menu",
            });

            // 3 - 0
            mainPanel.Children.Add(button);

            button = new Button();
            button.Position_x = 0;
            button.Position_y = (int)(gw_height * -0.32f);
            button.Width = (int)(gw_width * 0.43f);
            button.Height = 200;

            t = new Text()
            {
                Position_x = -160,
                Position_y = (int)(gw_height * -0.32f) + 10,
                Width = (int)(gw_width * 0.47f) / 2,
                Height = 200,
                TypeSelected = Text.Type.SEGOEMDL2,
                TextSize = 40,
                FontStyle = FontStyle.Regular,
                Content = "\xE80F",
            };

            button.Children.Add(t);

            button.Children.Add(new Text()
            {
                Position_x = t.ContentSize,
                Position_y = (int)(gw_height * -0.32f) + 20,
                Width = (int)(gw_width * 0.47f) / 2,
                Height = 200,
                TextSize = 40,
                FontStyle = FontStyle.Regular,
                Content = "Exit Game",
            });

            // 3 - 1
            mainPanel.Children.Add(button);
        }

        // Create introGrid, mainGrid, gameGrid, gameMenuGrid or gameOverGrid
        public void Draw(UIParent uie)
        {
            uie.Draw();
        }

    }
}






