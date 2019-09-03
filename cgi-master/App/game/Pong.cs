using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cgimin.engine.camera;
using cgimin.engine.material;
using cgimin.engine.material.normalmapping;
using cgimin.engine.object3d;
using cgimin.engine.texture;
using OpenTK;
using OpenTK.Input;

namespace App.Game
{
    class Pong : Object
    {

        private float distance;
        private float sliderToPongDistance;
        private Random random = new Random();
        private Vector3 pongPosition; // Pong Vector Position
        private Vector3 oldPos;
        private Vector3 mittelPunkt = new Vector3(0, 0, 0);
        private Vector3 direction = new Vector3(0, 0.08875f, 0.08875f);
        private Vector3 kugelSchnittPunkt;
        private bool dirCalc = false;
        private bool gameStart = true;
        private bool gameOver = false; // Controller.state = State.GAMEOVER;
        private bool DEBUG = false;

        public Vector3 PongPosition { get => pongPosition; set => pongPosition = value; }

        public Pong(Camera cam) : base(cam)
        {
            Obj = new ObjLoaderObject3D("Background.obj", 0.05f);

            BaseMaterial.MaterialSettings materialSettings = new BaseMaterial.MaterialSettings();
            materialSettings.colorTexture = TextureManager.LoadTexture("green.jpg");
            materialSettings.normalTexture = TextureManager.LoadTexture("green_NRM.jpg");
            materialSettings.shininess = 10.0f;
            MaterialSettings = materialSettings;

            Obj.Transformation *= Matrix4.CreateTranslation(20f, 10f, 10f);

            Material = new NormalMappingMaterial(cam);

        }


        public bool GetGameStatus()
        {
            return gameOver;
        }

        public void move(Vector3 sliderPosition)
        {
            this.Obj.Transformation *= Matrix4.CreateTranslation(direction);
            pongPosition = this.Obj.Transformation.ExtractTranslation();

            distance = Distance(pongPosition, mittelPunkt);
            if (distance > 50.0f - 8.0f)
            {
                dirCalc = true;
            }
            else
            {
                oldPos = pongPosition;
            }

            if (dirCalc)
            {
                float d = 50.0f - Distance(oldPos, new Vector3(0, 0, 0));
                kugelSchnittPunkt = KugelSchnittpunkt(oldPos, d);
                Vector3 v = pongPosition - oldPos;
                direction = Direction(kugelSchnittPunkt, v);
               
                if (DEBUG)
                {
                    sliderToPongDistance = 0.0f;
                }
                else
                {
                    sliderToPongDistance = Distance(sliderPosition, kugelSchnittPunkt);
                }
                if (sliderToPongDistance <= 10.0f)
                {
                    if (gameStart)
                    {
                        direction = direction * 2;
                        gameStart = false;
                    }
                    this.Obj.Transformation *= Matrix4.CreateTranslation(direction);
                }
                else
                {
                    gameOver = true;
                }


                dirCalc = false;
            }
        }


        internal void AnimateRotation()
        {
            this.Obj.Transformation *= Matrix4.CreateFromAxisAngle(this.Obj.Transformation.ExtractScale(), 0.005f);
            this.Obj.Transformation *= Matrix4.CreateFromAxisAngle(this.Obj.Transformation.ExtractTranslation(), 0.005f);

        }

        private Vector3 Direction(Vector3 n, Vector3 v)
        {

            Vector3 vr = new Vector3();
            n = n * -1.0f;
            vr = v - (2 * (Vector3.Dot(v, n) / Vector3.Dot(n, n)) * n);

            return vr;
        }

        private Vector3 KugelSchnittpunkt(Vector3 p, float d)
        {
            Vector3 schnittPunkt;

            schnittPunkt = Vector3.Multiply(p, (50.0f / Distance(p, new Vector3(0, 0, 0))));

            return schnittPunkt;
        }

        private float Distance(Vector3 p, Vector3 q)
        {
            float distance;

            distance = (float)Math.Sqrt(Math.Pow((q.X - p.X), 2) + Math.Pow((q.Y - p.Y), 2) + Math.Pow((q.Z - p.Z), 2));

            return distance;
        }
    }
}