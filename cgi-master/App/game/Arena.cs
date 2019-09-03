using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cgimin.engine.camera;
using cgimin.engine.material;
using cgimin.engine.material.ambientdiffuse;
using cgimin.engine.material.simpletexture;
using cgimin.engine.object3d;
using cgimin.engine.texture;
using OpenTK;
using OpenTK.Input;

namespace App.Game
{
    class Arena : Object
    {

        public Arena(Camera cam) : base(cam)
        {

            Obj = new ObjLoaderObject3D("Background.obj");

            BaseMaterial.MaterialSettings materialSettings = new BaseMaterial.MaterialSettings();

            materialSettings.colorTexture = TextureManager.LoadTexture("arena.png");
            MaterialSettings = materialSettings;

            Obj.Transformation *= Matrix4.CreateScale(-.5f, .5f, .5f); // Invert on x-axis to be transparent for camera
            Material = new SimpleTextureMaterial(cam);
        }
    }
}
