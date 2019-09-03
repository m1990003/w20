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
using static cgimin.engine.material.BaseMaterial;

namespace App.Game
{
    class Backgroundstation : Object
    {

        public Backgroundstation(Camera cam) : base(cam)
        {
            Obj = new ObjLoaderObject3D("station.obj", 50.0f);

            BaseMaterial.MaterialSettings materialSettings = new BaseMaterial.MaterialSettings();
            materialSettings.colorTexture = TextureManager.LoadTexture("station/diffuse.png");
            materialSettings.normalTexture = TextureManager.LoadTexture("station/normal.png");
            materialSettings.shininess = 10.0f;
            MaterialSettings = materialSettings;

            Material = new NormalMappingMaterial(cam);

            Obj.Transformation *= Matrix4.CreateTranslation(160f, 160f, 160f);

        }
    }
}
