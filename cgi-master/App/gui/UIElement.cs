using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using cgimin.engine.object3d;
using cgimin.engine.texture;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace App.GUI
{
    public abstract class UIElement
    {
        // private attributes
        protected Color4 color = System.Drawing.Color.Empty;
        protected float position_x, position_y, width, height;
        protected int texture;



        // public methods
        public int Texture { get => texture; set => texture = value; }
        public float Position_x { get; set; }
        public float Position_y { get; set; }
        public virtual float Width { get => width; set => width = value; }
        public virtual float Height { get => height; set => height = value; }
        public Color4 Color { get => color; set => color = value; }


        public virtual void Draw()
        {

            GL.Enable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, this.Texture);

            GL.Begin(BeginMode.Quads);

            if (this.Color != System.Drawing.Color.Empty || this.Color != new Color4(0, 0, 0, 0))
                GL.Color4(Color.R, Color.G, Color.B, Color.A);


            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(this.Position_x - this.Width / 2, this.Position_y - this.Height / 2); // bottom-left
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(this.Position_x + this.Width / 2, this.Position_y - this.Height / 2); // bottom-right
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(this.Position_x + this.Width / 2, this.Position_y + this.Height / 2); // top-right
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(this.Position_x - this.Width / 2, this.Position_y + this.Height / 2); // top-left

            GL.Disable(EnableCap.Texture2D);
            GL.End();
            GL.PopMatrix();
            GL.PopAttrib();
        }
    }

    public abstract class UIParent : UIElement
    {
        private List<UIElement> children = new List<UIElement>();
        public List<UIElement> Children
        { get => children; set => children = value; }

        public override void Draw()
        {

            base.Draw();
            if (children != null)
                foreach (UIElement uie in children)
                    uie.Draw();
        }
    }

    class Grid : UIParent
    {
        public Grid() { }
    }

    class Panel : UIParent
    {
        private enum Type
        {
            STAR,
            RECTANGLE,
            CIRCLE,
        }

        private Type typeSelected;

        private Type TypeSelected { get => typeSelected; set => typeSelected = value; }

        public Panel() { }

        public Panel(Enum type)
        {
            this.typeSelected = (Type)type;
            this.Color = Color4.White;
        }
    }

    class Button : UIParent
    {
        public Button()
        {

            this.Texture = TextureManager.LoadTexture("ui/button.png");
            this.Color = Color4.White;
        }
    }

    class Text : UIElement
    {
        private PrivateFontCollection privateFontCollection = new PrivateFontCollection();

        private FontFamily segoeUI;
        private FontFamily segoeMDL2 = new FontFamily("Segoe MDL2 Assets");

        private Font font;

        public enum Type
        {
            SEGOEUI,
            SEGOEMDL2
        }

        public Type TypeSelected { get; set; }



        private FontStyle fontStyle = FontStyle.Regular;

        private int contentSize;
        private TextRenderer textRenderer;


        private string content;
        internal Bitmap bmp;
        private int textSize;

        public Text()
        {
            privateFontCollection.AddFontFile("fonts/segoeui.ttf");
            segoeUI = privateFontCollection.Families[0];
            this.Color = Color4.White;
        }

        public string Content
        {
            get => content;
            set
            {

                content = value;

                bmp = new Bitmap((int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics graphics = Graphics.FromImage(bmp);

                contentSize = (int)graphics.MeasureString(content, font).Width;

                this.CreateText();

            }
        }

        public int TextSize
        {
            get
            {
                return textSize;
            }
            set
            {
                textSize = value;

                switch (TypeSelected)
                {

                    case Type.SEGOEMDL2:
                        font = new Font(segoeMDL2, this.textSize, fontStyle);
                        break;
                    default:
                        font = new Font(segoeUI, this.textSize, fontStyle);
                        break;
                }

            }
        }

        public override float Width
        {
            set
            {
                width = value;

                if (width != 0 && height != 0)
                    textRenderer = new TextRenderer((int)Width, (int)Height);
            }
        }

        public override float Height
        {
            set
            {
                height = value;

                if (width != 0 && height != 0)
                    textRenderer = new TextRenderer((int)width, (int)height);

            }
        }

        public FontStyle FontStyle
        {
            get => fontStyle; set
            {

                fontStyle = value;
                if (textSize > 0)
                {
                    switch (TypeSelected)
                    {

                        case Type.SEGOEMDL2:
                            font = new Font(segoeMDL2, this.textSize, fontStyle);
                            break;
                        default:
                            font = new Font(segoeUI, this.textSize, fontStyle);
                            break;
                    }
                }
            }
        }

        public int ContentSize { get => contentSize; set => contentSize = value; }

        public void CreateText()
        {
            this.texture = textRenderer.DrawString(content, font, this.Color, new PointF(width / 2 - contentSize / 2, height / 2 - TextSize / 2));
        }

    }
}
