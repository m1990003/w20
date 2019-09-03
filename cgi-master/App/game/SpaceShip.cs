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
    class SpaceShip : Object
    {

        public SpaceShip(Camera cam, float size, Color color) : base(cam)
        {

            BaseMaterial.MaterialSettings materialSettings = new BaseMaterial.MaterialSettings();

            switch (color)
            {
                case Color.RED:
                    materialSettings.colorTexture = TextureManager.LoadTexture("ship/albedo_red.png");
                    Obj = new ObjLoaderObject3D("ship_min90.obj", size);
                    break;
                case Color.GREEN:
                    materialSettings.colorTexture = TextureManager.LoadTexture("ship/albedo_green.png");
                    Obj = new ObjLoaderObject3D("ship_min180.obj", size);
                    break;
                case Color.BLUE:
                    materialSettings.colorTexture = TextureManager.LoadTexture("ship/albedo_blue.png");
                    Obj = new ObjLoaderObject3D("ship.obj", size);
                    break;
                case Color.STANDARD:
                    materialSettings.colorTexture = TextureManager.LoadTexture("ship/albedo.png");
                    Obj = new ObjLoaderObject3D("ship_min270.obj", size);
                    break;
            }

            materialSettings.normalTexture = TextureManager.LoadTexture("ship/normal.png");
            materialSettings.shininess = 10.0f;
            MaterialSettings = materialSettings;

            Material = new NormalMappingMaterial(cam);

        }

        public enum Color
        {
            RED,
            GREEN,
            BLUE,
            STANDARD
        }
    }
}
