using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;


namespace cgimin.engine.camera
{
    public class Camera
    {
        // enumeration for 6 clipping-planes
        enum planeEnum : int
        {
            NEAR_PLANE = 0,
            FAR_PLANE = 1,
            LEFT_PLANE = 2,
            RIGHT_PLANE = 3,
            TOP_PLANE = 4,
            BOTTOM_PLANE = 5
        };

        // Struct for a plane (hnf)
        struct Plane
        {
            public float d;
            public Vector3 normal;
        }

        // frustum clipping-planes
        private List<Plane> planes;

        // Matrix for the transformation
        private Matrix4 transformation;

        // ... and the petrspective projection
        private Matrix4 perspectiveProjection;

        // position for the camera is saved
        private Vector3 position;

        // our implementation
        private Vector3 direction;
        private Vector3 cam_up = Vector3.UnitY;

        // our implementation
        private Vector3 distancePosition;
        private Vector3 newPosition;

        // our implementation
        private Vector3 distanceDirection;
        private Vector3 newDirection;


        // for the control of the fly-cam
        private float xRotation;
        private float yRotation;

      // gui projection matrix
        public Matrix4 GuiProjection { get; private set; }

        private Vector3 oldDirection;
        private Vector3 oldPosition;


        // our implementation
        public enum MoveType
        {
            Linear,
            Nonlinear
        }
        // our implementation
        private MoveType cMove;
        private MoveType cRotate;

        // our implementation
        private float x_Step = 0.05f;
        private float y_Step = 0.05f;
        private float z_Step = 0.05f;
        public bool isMoving()
        {
            if (this.DistancePosition.Length > 0.05f)
            {
                return true;

            }
            return false;
        }


        // our implementation
        private const float moveSpeed = 0.25f;

        public  void Init()
        {
            planes = new List<Plane>();
            for (int i = 0; i < 6; i++) planes.Add(new Plane());

            perspectiveProjection = Matrix4.Identity;
            transformation = Matrix4.Identity;
            xRotation = 0;
            yRotation = 0;
            position = Vector3.Zero;


            // create default orthographic
            Matrix4 ddProjection = new Matrix4();
            Matrix4.CreateOrthographic(1920, 1080, -1, 1, out ddProjection);
            GuiProjection = ddProjection;


        }


        // width, height = size of screen in pixeln, fov = "field of view", der opening-angle for the camera lense
        public void SetWidthHeightFov(int width, int height, float fov)
        {
            float aspectRatio = width / (float)height;
            Matrix4.CreatePerspectiveFieldOfView((float)(fov * Math.PI / 180.0f), aspectRatio, 0.01f, 500, out perspectiveProjection);
                 // Set orthographic projection that width of screen stays 1920, but height depends on aspect-ratio
            Matrix4 ddProjection = new Matrix4();
            Matrix4.CreateOrthographic(1920, 1920.0f * height / width, -1, 1, out ddProjection);
            GuiProjection = ddProjection;

}


        // generation of the camera-transformation using LookAt
        // position of the camera-"eye", look-at poinmt, "up" direction of camera
        public  void SetLookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            position = eye;
            direction = target;
            cam_up = up;
            setLookAt();
        }

        public void setLookAt()
        {
            // our implementation
            transformation = Matrix4.LookAt(position, direction, cam_up);



            //	  transformation = Matrix4.LookAt(eye, target, up);
            CreateViewFrustumPlanes(transformation * perspectiveProjection);

        }

        // Steering the fly-cam
        public void UpdateFlyCamera(bool rotLeft, bool rotRight, bool moveForward, bool moveBack, bool moveUp = false, bool moveDown = false, bool tiltFoward = false, bool tiltBackward = false)
        {
            if (rotLeft) yRotation -= 0.025f;
            if (rotRight) yRotation += 0.025f;

            if (moveForward) position -= new Vector3(transformation.Column2.X, transformation.Column2.Y, transformation.Column2.Z) * 0.08f;
            if (moveBack) position += new Vector3(transformation.Column2.X, transformation.Column2.Y, transformation.Column2.Z) * 0.08f;

            if (moveUp) position += new Vector3(transformation.Column1.X, transformation.Column1.Y, transformation.Column1.Z) * 0.08f;
            if (moveDown) position -= new Vector3(transformation.Column1.X, transformation.Column1.Y, transformation.Column1.Z) * 0.08f;

            if (tiltFoward) xRotation += 0.02f;
            if (tiltBackward) xRotation -= 0.02f;

            transformation = Matrix4.Identity;
            transformation *= Matrix4.CreateTranslation(-position.X, -position.Y, -position.Z);
            transformation *= Matrix4.CreateRotationX(xRotation);
            transformation *= Matrix4.CreateRotationY(yRotation);

            CreateViewFrustumPlanes(transformation * perspectiveProjection);
        }

        // calculate 6 clipping planes of the view frustum
        private void CreateViewFrustumPlanes(Matrix4 mat)
        {
            // left
            Plane plane = new Plane();
            plane.normal.X = mat.M14 + mat.M11;
            plane.normal.Y = mat.M24 + mat.M21;
            plane.normal.Z = mat.M34 + mat.M31;
            plane.d = mat.M44 + mat.M41;
            planes[(int)planeEnum.LEFT_PLANE] = plane;

            // right
            plane = new Plane();
            plane.normal.X = mat.M14 - mat.M11;
            plane.normal.Y = mat.M24 - mat.M21;
            plane.normal.Z = mat.M34 - mat.M31;
            plane.d = mat.M44 - mat.M41;
            planes[(int)planeEnum.RIGHT_PLANE] = plane;

            // bottom
            plane = new Plane();
            plane.normal.X = mat.M14 + mat.M12;
            plane.normal.Y = mat.M24 + mat.M22;
            plane.normal.Z = mat.M34 + mat.M32;
            plane.d = mat.M44 + mat.M42;
            planes[(int)planeEnum.BOTTOM_PLANE] = plane;

            // top
            plane = new Plane();
            plane.normal.X = mat.M14 - mat.M12;
            plane.normal.Y = mat.M24 - mat.M22;
            plane.normal.Z = mat.M34 - mat.M32;
            plane.d = mat.M44 - mat.M42;
            planes[(int)planeEnum.TOP_PLANE] = plane;

            // near
            plane = new Plane();
            plane.normal.X = mat.M14 + mat.M13;
            plane.normal.Y = mat.M24 + mat.M23;
            plane.normal.Z = mat.M34 + mat.M33;
            plane.d = mat.M44 + mat.M43;
            planes[(int)planeEnum.NEAR_PLANE] = plane;

            // far
            plane = new Plane();
            plane.normal.X = mat.M14 - mat.M13;
            plane.normal.Y = mat.M24 - mat.M23;
            plane.normal.Z = mat.M34 - mat.M33;
            plane.d = mat.M44 - mat.M43;
            planes[(int)planeEnum.FAR_PLANE] = plane;

            // normalize
            for (int i = 0; i < 6; i++)
            {
                plane = planes[i];

                float length = plane.normal.Length;
                plane.normal.X = plane.normal.X / length;
                plane.normal.Y = plane.normal.Y / length;
                plane.normal.Z = plane.normal.Z / length;
                plane.d = plane.d / length;

                planes[i] = plane;
            }
        }


        // returns the signed distance froma point to frustum clipping plane
        private float signedDistanceToPoint(int planeID, Vector3 pt)
        {
            return Vector3.Dot(planes[planeID].normal, pt) + planes[planeID].d;
        }


        // is sphere inside or overlapping the view frustum?
        public bool SphereIsInFrustum(Vector3 center, float radius)
        {
            for (int i = 0; i < 6; i++)
            {
                if (signedDistanceToPoint(i, center) < -radius)
                {
                    return false;
                }
            }
            return true;
        }

     // set transformation Matrix manually from outside
        public void SetTransformMatrix(Matrix4 transform)
        {
            transformation = transform;
        }

        // Getter
        public Vector3 Position
        {
            get { return position; }
        }


        public Matrix4 Transformation
        {
            get { return transformation; }
        }


        public Matrix4 PerspectiveProjection
        {
            get { return perspectiveProjection; }
        }

        // our implementation
        public void SetSpeed(float x, float y, float z)
        {
            x_Step = x;
            y_Step = y;
            z_Step = z;
        }
        // our implementation
        public Vector3 DistancePosition { get => distancePosition; set => distancePosition = value; }
        public MoveType CMove { get => cMove; set => cMove = value; }
        // our implementation
        public MoveType CRotate { get => cRotate; set => cRotate = value; }
        public Vector3 OldDirection { get => oldDirection; set => oldDirection = value; }
        public Vector3 OldPosition { get => oldPosition; set => oldPosition = value; }
        public Vector3 Direction { get => direction; set => direction = value; }
        public Vector3 NewDirection { get => newDirection; set => newDirection = value; }
        public Vector3 NewPosition { get => newPosition; set => newPosition = value; }
        public Matrix4 PerspectiveProjection1 { get => perspectiveProjection; set => perspectiveProjection = value; }
        public Vector3 Cam_up { get => cam_up; set => cam_up = value; }

        // our implementation
        public void Init_Move(Vector3 newPos, Enum moveType)
        {
            newPosition = newPos;
            cMove = (MoveType)moveType;

            distancePosition.X = Math.Abs(newPosition.X - position.X);
            distancePosition.Y = Math.Abs(newPosition.Y - position.Y);
            distancePosition.Z = Math.Abs(newPosition.Z - position.Z);

        }
        // our implementation
        // Linear camera move
        public void Move()
        {
            switch (cMove)
            {
                case MoveType.Linear:

                    position.Z += (distancePosition.Z != 0) ? (newPosition.Z < position.Z ? -z_Step : z_Step) : 0;
                    position.Y += (distancePosition.Y != 0) ? (newPosition.Y < position.Y ? -y_Step : y_Step) : 0;
                    position.X += (distancePosition.X != 0) ? (newPosition.X < position.X ? -x_Step : x_Step) : 0;

                    break;
                case MoveType.Nonlinear:
                    break;
            }

            distancePosition.X = (float)Math.Truncate((double)(newPosition.X - position.X));
            distancePosition.Y = (float)Math.Truncate((double)(newPosition.Y - position.Y));
            distancePosition.Z = (float)Math.Truncate((double)(newPosition.Z - position.Z));

            setLookAt();

        }
        // our implementation
        public void Init_DirectionTo(Vector3 newTar, Enum moveType)
        {
            newDirection = newTar;
            cMove = (MoveType)moveType;

            distanceDirection.X = Math.Abs(newDirection.X - direction.X);
            distanceDirection.Y = Math.Abs(newDirection.Y - direction.Y);
            distanceDirection.Z = Math.Abs(newDirection.Z - direction.Z);

        }

        // our implementation
        public void DirectionTo()
        {
            switch (cRotate)
            {
                case MoveType.Linear:


                    direction.Z += (distanceDirection.Z != 0) ? (newDirection.Z < direction.Z ? -z_Step : z_Step) : 0;
                    direction.Y += (distanceDirection.Y != 0) ? (newDirection.Y < direction.Y ? -y_Step : y_Step) : 0;
                    direction.X += (distanceDirection.X != 0) ? (newDirection.X < direction.X ? -x_Step : x_Step) : 0;

                    break;
                case MoveType.Nonlinear:
                    break;
            }



            distanceDirection.X = (float)Math.Truncate((double)(newDirection.X - direction.X));
            distanceDirection.Y = (float)Math.Truncate((double)(newDirection.Y - direction.Y));
            distanceDirection.Z = (float)Math.Truncate((double)(newDirection.Z - direction.Z));

            setLookAt();

        }

    }
}
