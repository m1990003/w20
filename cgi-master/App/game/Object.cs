using cgimin.engine.camera;
using cgimin.engine.material;
using cgimin.engine.material.ambientdiffuse;
using cgimin.engine.material.cubereflectionnormal;
using cgimin.engine.material.normalmapping;
using cgimin.engine.material.normalmappingcubespecular;
using cgimin.engine.material.simplereflection;
using cgimin.engine.material.simpletexture;
using cgimin.engine.material.wobble1;
using cgimin.engine.material.zbuffershader;
using cgimin.engine.object3d;
using cgimin.engine.texture;
using Engine.cgimin.engine.material.simpleblend;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Game
{
    public abstract class Object
    {
        // obj
        private ObjLoaderObject3D obj;

        private BaseMaterial material;
        private BaseMaterial.MaterialSettings materialSettings;

        private Camera cam;

        public Object(Camera cam)
        {
            this.cam = cam;

            material = new AmbientDiffuseMaterial(cam);
        }

        // Getter & Setter
        public ObjLoaderObject3D Obj { get => obj; set => obj = value; }

        // Getter & Setter
        public BaseMaterial Material { get => material; set => material = value; }
        public BaseMaterial.MaterialSettings MaterialSettings { get => materialSettings; set => materialSettings = value; }

        public Camera Cam { get => cam; set => cam = value; }

        public void Draw()
        {
            material.DrawWithSettings(obj, materialSettings);
        }

    }


    public class BackgroundSphere : Object
    {

        public BackgroundSphere(Camera cam) : base(cam)
        {
            Obj = new ObjLoaderObject3D("Background.obj");

            BaseMaterial.MaterialSettings materialSettings = new BaseMaterial.MaterialSettings();
            materialSettings.colorTexture = TextureManager.LoadTexture("background.jpg");

            MaterialSettings = materialSettings;
            Material = new SimpleTextureMaterial(Cam);


            // Scale objects
            Obj.Transformation *= Matrix4.CreateScale(-2.0f, 2.0f, 2.0f); // Invert on x-axis so that all of the faces point inward

        }
    }
}
