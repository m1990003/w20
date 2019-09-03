using cgimin.engine.camera;
using cgimin.engine.material;
using cgimin.engine.material.ambientdiffuse;
using cgimin.engine.material.simpletexture;
using cgimin.engine.object3d;
using cgimin.engine.texture;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace App.Game
{
    class Map : Object
    {


        public Map(Camera cam) : base(cam)
        {
            Obj = new ObjLoaderObject3D("Background.obj");
            BaseMaterial.MaterialSettings materialSettings = new BaseMaterial.MaterialSettings();

            materialSettings.colorTexture = TextureManager.LoadTexture("ui/simpleBlueTexture.png");
            AmbientDiffuseMaterial material = new AmbientDiffuseMaterial(cam);

            MaterialSettings = materialSettings;


            Obj.Transformation *= Matrix4.CreateScale(-0.05f, 0.05f, 0.05f); // Invert on x-axis to be transparent for camera
            Obj.Transformation *= Matrix4.CreateTranslation(new Vector3(0, 0, 0));


        }

    }


    class Axis : Object
    {

        public Axis(Camera cam) : base(cam)
        {

            Obj = new ObjLoaderObject3D("Axis.obj");
            BaseMaterial.MaterialSettings materialSettings = new BaseMaterial.MaterialSettings();

            materialSettings.colorTexture = TextureManager.LoadTexture("ui/simpleWhiteTexture.png");
            MaterialSettings = materialSettings;

            Material = new AmbientDiffuseMaterial(cam);

            Obj.Transformation *= Matrix4.CreateScale(2.5f, 5f, 5f);
            Obj.Transformation *= Matrix4.CreateTranslation(new Vector3(0, 0, 0));
        }

    }

    class MapPong : Object
    {

        public MapPong(Camera cam) : base(cam)
        {
            Obj = new ObjLoaderObject3D("Background.obj", 0.005f);

            BaseMaterial.MaterialSettings materialSettings = new BaseMaterial.MaterialSettings();

            materialSettings.colorTexture = TextureManager.LoadTexture("ui/simpleRedTexture.png");
            MaterialSettings = materialSettings;

            Material = new AmbientDiffuseMaterial(cam);

            Obj.Transformation *= Matrix4.CreateTranslation(new Vector3(0, 0, 0));
            Obj.Transformation *= Matrix4.CreateTranslation(new Vector3(20f, 10f, 10f) * 0.1f);


        }
    }

}