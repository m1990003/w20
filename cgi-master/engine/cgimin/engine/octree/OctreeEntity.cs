﻿using cgimin.engine.material;
using cgimin.engine.object3d;
using OpenTK;
using static cgimin.engine.material.BaseMaterial;

namespace Engine.cgimin.engine.octree
{
    public class OctreeEntity
    {

        public BaseObject3D Object3d;
        public BaseMaterial Material;
        public MaterialSettings MaterialSetting;
        public Matrix4 Transform;

        public bool drawn;
        public float distToCam;

        public OctreeEntity(BaseObject3D object3d, BaseMaterial material, MaterialSettings materialSetting, Matrix4 transform)
        {
            Object3d = object3d;
            Material = material;
            MaterialSetting = materialSetting;
            Transform = transform;
            drawn = false;
        }

    }
}
