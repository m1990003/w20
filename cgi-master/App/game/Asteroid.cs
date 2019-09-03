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
using static cgimin.engine.material.BaseMaterial;

namespace App.Game
{
    class Asteroid : Object
    {

        public Asteroid(Camera cam, float size) : base(cam)
        {
            Obj = new ObjLoaderObject3D("asteroid.obj", size);

            BaseMaterial.MaterialSettings materialSettings = new BaseMaterial.MaterialSettings();
            materialSettings.colorTexture = TextureManager.LoadTexture("asteroid.jpg");
            MaterialSettings = materialSettings;

            Material = new AmbientDiffuseMaterial(cam);

        }
    }
}
