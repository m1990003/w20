using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using cgimin.engine.material.ambientdiffuse;
using cgimin.engine.material.simpletexture;
using cgimin.engine.object3d;
using cgimin.engine.texture;
using OpenTK;
using OpenTK.Input;
using cgimin.engine.camera;
using System.Diagnostics;
using cgimin.engine.material;

namespace App.Game
{
    class Slider : Object
    {

        private Vector3 v;
        private float u, d, l, r = 0;

        private float accelerationLR = 0.0000f;
        private float accelerationUD = 0.0000f;
        private float maxAcceleration = 0.02f;
        private float sliderSpeedLR = 0.0001f;
        private float sliderSpeedUD = 0.0001f;

        private double distanceLR = 0f;
        private double distanceUD = 0f;
        private bool endMoveLR = true;
        private bool endMoveUD = true;

        public enum Directions
        {
            NONE,
            LEFT,
            RIGHT,
            UP,
            DOWN
        }

        private Directions directionLR;     // Left-Right
        private Directions directionUD;     // Up-Down

        private Directions Direction { get => Direction; set => Direction = value; }

        private Stopwatch watch = new Stopwatch();

        private Vector3 sliderPosition, camPosition;


        private Matrix4 camMatrix;
        private float distanceCamSlider = 1.75f;
        private float camSpeed = 0.1f;
        private Vector3 camUP;

        private Quaternion q;
        private Camera mainCam;

        public Vector3 getPos()
        {
            return this.Obj.Transformation.ExtractTranslation();
        }

        public Slider(Camera mainCam) : base(mainCam)
        {
            Obj = new ObjLoaderObject3D("slider_v4.obj", 0.8f);

            this.mainCam = mainCam;

            BaseMaterial.MaterialSettings materialSettings = new BaseMaterial.MaterialSettings();
            materialSettings.colorTexture = TextureManager.LoadTexture("galvanized-metal-texture.jpg");
            materialSettings.shininess = 10.0f;
            MaterialSettings = materialSettings;
            Material = new AmbientDiffuseSpecularMaterial(mainCam);

            Obj.Transformation = Matrix4.CreateTranslation(47f, 0f, 0f);

        }

        public Matrix4 CamMatrix { get => camMatrix; set => camMatrix = value; }
        public float DistanceCamSlider { get => distanceCamSlider; set => distanceCamSlider = value; }
        public Vector3 SliderPosition { get => sliderPosition; set => sliderPosition = value; }

        public void initMove(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            Key key = e.Key;

            // Block directionLR
            switch (key)
            {
                case Key.Left:
                case Key.A:
                    directionLR = Directions.LEFT;
                    mainCam.SetSpeed(camSpeed, camSpeed, camSpeed);
                    endMoveLR = false;
                    distanceLR = 5f;
                    break;

                case Key.Right:
                case Key.D:
                    directionLR = Directions.RIGHT;
                    mainCam.SetSpeed(camSpeed, camSpeed, camSpeed);
                    endMoveLR = false;
                    distanceLR = 5f;
                    break;
            }

            // Block directionUD
            switch (key)
            {
                case Key.Up:
                case Key.W:
                    directionUD = Directions.UP;
                    mainCam.SetSpeed(camSpeed, camSpeed, camSpeed);
                    endMoveUD = false;
                    distanceUD = 5f;
                    break;

                case Key.Down:
                case Key.S:
                    directionUD = Directions.DOWN;
                    mainCam.SetSpeed(camSpeed, camSpeed, camSpeed);
                    endMoveUD = false;
                    distanceUD = 5f;
                    break;

                // Cam perspective
                case Key.C:
                    float oldDistance = distanceCamSlider;
                    distanceCamSlider = distanceCamSlider == 1.75f ? 1.45f : distanceCamSlider == 1.45f ? 1.2f : 1.75f;
                    mainCam.SetLookAt(mainCam.Position / oldDistance * distanceCamSlider, mainCam.Direction, mainCam.Cam_up);
                    break;
            }
        }


        public void move()
        {
            if (isMovingLR() && isMovingUD())
            {
                distanceLR += endMoveLR ? -0.8f : 0;
                distanceUD += endMoveUD ? -0.8f : 0;
                sliderSpeedLR = 0.0003f;
                sliderSpeedUD = 0.0003f;
            }
            else if (isMovingLR())
            {
                distanceLR += endMoveLR ? -0.1f : 0;
                sliderSpeedLR = 0.0001f;
                sliderSpeedUD = 0.0001f;
            }

            else if (isMovingUD())
            {
                distanceUD += endMoveUD ? -0.1f : 0;
                sliderSpeedLR = 0.0001f;
                sliderSpeedUD = 0.0001f;
            }

            if (isMovingLR())
            {

                accelerationLR += accelerationLR < maxAcceleration && !endMoveLR ? 0.001f : endMoveLR && accelerationLR > 0 ? -0.001f : 0;

                switch (directionLR)
                {
                    case Directions.LEFT:

                        sliderPosition = this.Obj.Transformation.ExtractTranslation();
                        camPosition = camMatrix.ExtractTranslation();

                        if (l == 0 && r == 0)
                        {
                            accelerationLR = 0.0000f;

                            if (v.Length == 0)
                                v = new Vector3(1, 0, 0);

                            v.Normalize();


                            Vector3 left = Vector3.Cross(sliderPosition, v);
                            v = left;
                            v = calcRotationV(v, sliderSpeedLR, accelerationLR);



                            l = (float)(Math.Cos((sliderSpeedLR + accelerationLR) / 2) * 180.0 / 3.141593);
                            l = -l;

                            q = new Quaternion(v, l);
                            q.Normalize();

                            u = 0;
                            d = 0;
                        }

                        else if (l == 0 && r != 0)
                        {
                            accelerationLR = 0.0000f;
                            r = 0;

                            v.Normalize();
                            v = calcRotationV(v, sliderSpeedLR, accelerationLR);

                            l = (float)(Math.Cos((sliderSpeedLR + accelerationLR) / 2) * 180.0 / 3.141593);
                            l = -l;
                            q = new Quaternion(v, l);
                            q.Normalize();


                        }
                        else
                        {
                            v.Normalize();
                            v = calcRotationV(v, sliderSpeedLR, accelerationLR);

                            l = (float)(Math.Cos((sliderSpeedLR + accelerationLR) / 2) * 180.0 / 3.141593);
                            l = -l;
                            q = new Quaternion(v, l);
                            q.Normalize();

                        }

                        // Rotate slider around quaternion
                        this.Obj.Transformation *= Matrix4.CreateFromQuaternion(q);
                        camMatrix *= Matrix4.CreateFromQuaternion(q);

                        // Camera setup
                        setCamPosition(directionLR);

                        break;



                    case Directions.RIGHT:

                        sliderPosition = this.Obj.Transformation.ExtractTranslation();
                        camPosition = camMatrix.ExtractTranslation();

                        if (r == 0 && l == 0)
                        {
                            accelerationLR = 0.0000f;

                            if (v.Length == 0)
                                v = new Vector3(1, 0, 0);
                            v.Normalize();
                            Vector3 left = Vector3.Cross(sliderPosition, v);
                            v = left;

                            v = calcRotationV(v, sliderSpeedLR, accelerationLR);
                            r = (float)(Math.Cos((sliderSpeedLR + accelerationLR) / 2) * 180.0 / 3.141593);
                            q = new Quaternion(v, r);
                            q.Normalize();

                            u = 0;
                            d = 0;
                        }

                        else if (r == 0 && l != 0)
                        {
                            l = 0;
                            accelerationLR = 0.0000f;
                            v.Normalize();


                            v = calcRotationV(v, sliderSpeedLR, accelerationLR);

                            r = (float)(Math.Cos((sliderSpeedLR + accelerationLR) / 2) * 180.0 / 3.141593);
                            q = new Quaternion(v, r);
                            q.Normalize();


                        }
                        else
                        {
                            v.Normalize();
                            v = calcRotationV(v, sliderSpeedLR, accelerationLR);
                            r = (float)(Math.Cos((sliderSpeedLR + accelerationLR) / 2) * 180.0 / 3.141593);
                            q = new Quaternion(v, r);
                            q.Normalize();

                        }

                        this.Obj.Transformation *= Matrix4.CreateFromQuaternion(q);
                        camMatrix *= Matrix4.CreateFromQuaternion(q);
                        // Camera setup
                        setCamPosition(directionLR);
                        break;

                }
            }

            if (isMovingUD())
            {
                accelerationUD += accelerationUD < maxAcceleration && !endMoveUD ? 0.001f : endMoveUD && accelerationUD > 0 ? -0.001f : 0;

                switch (directionUD)
                {
                    case Directions.UP:

                        sliderPosition = this.Obj.Transformation.ExtractTranslation();
                        camPosition = camMatrix.ExtractTranslation();

                        if (u == 0 && d == 0)
                        {
                            accelerationUD = 0.0000f;

                            if (v.Length == 0)

                                v = new Vector3(0, -1, 0);
                            v.Normalize();

                            // Cross product
                            Vector3 left = Vector3.Cross(sliderPosition, v);

                            v = left;

                            if (l != 0 || r != 0)
                                v = -v;

                            v = calcRotationV(v, sliderSpeedUD, accelerationUD);

                            u = (float)(Math.Cos((sliderSpeedUD + accelerationUD) / 2) * 180.0 / 3.141593);

                            u = -u;

                            q = new Quaternion(v, u);

                            q.Normalize();


                            l = 0;
                            r = 0;
                        }


                        else if (u == 0 && d != 0)
                        {
                            accelerationUD = 0.0000f;
                            d = 0;

                            v.Normalize();


                            v = calcRotationV(v, sliderSpeedUD, accelerationUD);


                            u = (float)(Math.Cos((sliderSpeedUD + accelerationUD) / 2) * 180.0 / 3.141593);
                            u = -u;

                            q = new Quaternion(v, u);



                            q.Normalize();


                        }
                        else
                        {
                            v.Normalize();



                            v = calcRotationV(v, sliderSpeedUD, accelerationUD);


                            u = (float)(Math.Cos((sliderSpeedUD + accelerationUD) / 2) * 180.0 / 3.141593);
                            u = -u;


                            q = new Quaternion(v, u);



                            q.Normalize();

                        }

                        this.Obj.Transformation *= Matrix4.CreateFromQuaternion(q);
                        camMatrix *= Matrix4.CreateFromQuaternion(q);

                        // Camera setup
                        setCamPosition(directionUD);

                        break;



                    case Directions.DOWN:

                        sliderPosition = this.Obj.Transformation.ExtractTranslation();
                        camPosition = camMatrix.ExtractTranslation();

                        if (d == 0 && u == 0)
                        {

                            accelerationUD = 0.0000f;

                            if (v.Length == 0)

                                v = new Vector3(0, -1, 0);

                            v.Normalize();
                            Vector3 left = Vector3.Cross(sliderPosition, v);
                            v = left;

                            if (l != 0 || r != 0)

                                v = -v;

                            v = calcRotationV(v, sliderSpeedUD, accelerationUD);

                            d = (float)(Math.Cos((sliderSpeedUD + accelerationUD) / 2) * 180.0 / 3.141593);

                            q = new Quaternion(v, d);

                            q.Normalize();

                            l = 0;
                            r = 0;


                        }

                        else if (d == 0 && u != 0)
                        {

                            accelerationUD = 0.0000f;
                            u = 0;

                            v.Normalize();

                            v = calcRotationV(v, sliderSpeedUD, accelerationUD);

                            d = (float)(Math.Cos((sliderSpeedUD + accelerationUD) / 2) * 180.0 / 3.141593);

                            q = new Quaternion(v, d);

                            q.Normalize();


                        }
                        else
                        {
                            v.Normalize();

                            v = calcRotationV(v, sliderSpeedUD, accelerationUD);

                            d = (float)(Math.Cos((sliderSpeedUD + accelerationUD) / 2) * 180.0 / 3.141593);

                            q = new Quaternion(v, d);

                            q.Normalize();

                        }

                        this.Obj.Transformation *= Matrix4.CreateFromQuaternion(q);
                        camMatrix *= Matrix4.CreateFromQuaternion(q);

                        // Camera setup
                        setCamPosition(directionUD);

                        break;
                }
            }
        }

        // mainCam position update
        private void setCamPosition(Directions d)
        {
            camPosition = this.camMatrix.ExtractTranslation();

            switch (d)
            {
                case Directions.LEFT:
                case Directions.RIGHT:
                    camUP = v;
                    break;
                case Directions.UP:
                case Directions.DOWN:
                    camUP = Vector3.Cross(camPosition, v);
                    break;
            }
            mainCam.SetLookAt(camMatrix.ExtractTranslation() * distanceCamSlider, new Vector3(0, 0, 0), camUP);
        }

        private Vector3 calcRotationV(Vector3 v, float sliderSpeed, float acceleration)
        {
            v.X *= (float)(Math.Sin((sliderSpeed + acceleration) / 2) * 180.0 / 3.141593);
            v.Y *= (float)(Math.Sin((sliderSpeed + acceleration) / 2) * 180.0 / 3.141593);
            v.Z *= (float)(Math.Sin((sliderSpeed + acceleration) / 2) * 180.0 / 3.141593);
            return v;
        }

        internal void EndMoveLR()
        {
            endMoveLR = true;
        }

        internal void EndMoveUD()
        {
            endMoveUD = true;
        }

        public bool isMovingLR()
        {
            if (this.distanceLR > 0)
            {
                return true;
            }
            else
            {
                accelerationLR = 0f;
                directionLR = Directions.NONE;
                return false;
            }

        }

        public bool isMovingUD()
        {

            if (this.distanceUD > 0)
            {
                return true;
            }
            else
            {
                accelerationUD = 0f;
                directionUD = Directions.NONE;
                return false;
            }
        }
    }
}
