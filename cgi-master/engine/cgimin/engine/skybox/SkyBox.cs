﻿using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using cgimin.engine.object3d;
using cgimin.engine.material.simpletexture;
using cgimin.engine.texture;
using cgimin.engine.camera;

namespace cgimin.engine.skybox
{
    public class SkyBox
    {

        private int frontID;
        private int backID;
        private int leftID;
        private int rightID;
        private int upID;
        private int downID;

        private BaseObject3D frontSide;
        private BaseObject3D backSide;
        private BaseObject3D leftSide;
        private BaseObject3D rightSide;
        private BaseObject3D upSide;
        private BaseObject3D downSide;

        private static SimpleTextureMaterial skyboxTextureMaterial;

        private Camera cam;



        public SkyBox(String front, String back, String left, String right, String up, String down, Camera cam)
        {
            this.cam = cam;
            skyboxTextureMaterial = new SimpleTextureMaterial(cam);

            frontID = TextureManager.LoadTexture(front, true);
            backID = TextureManager.LoadTexture(back, true);
            leftID = TextureManager.LoadTexture(left, true);
            rightID = TextureManager.LoadTexture(right, true);
            upID = TextureManager.LoadTexture(up, true);
            downID = TextureManager.LoadTexture(down, true);

            float s = 200.0f;

            frontSide = new BaseObject3D();
            frontSide.addTriangle(new Vector3(-s, -s, -s), new Vector3(s, -s, -s), new Vector3(s, s, -s), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0));
            frontSide.addTriangle(new Vector3(-s, -s, -s), new Vector3(s, s, -s), new Vector3(-s, s, -s), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 0));
            frontSide.CreateVAO();

            backSide = new BaseObject3D();
            backSide.addTriangle(new Vector3(-s, -s, s), new Vector3(s, -s, s), new Vector3(s, s, s), new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 0));
            backSide.addTriangle(new Vector3(-s, -s, s), new Vector3(s, s, s), new Vector3(-s, s, s), new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0));
            backSide.CreateVAO();

            rightSide = new BaseObject3D();
            rightSide.addTriangle(new Vector3(-s, -s, -s), new Vector3(-s, s, -s), new Vector3(-s, s, s), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0));
            rightSide.addTriangle(new Vector3(-s, -s, -s), new Vector3(-s, s, s), new Vector3(-s, -s, s), new Vector2(1, 1), new Vector2(0, 0), new Vector2(0, 1));
            rightSide.CreateVAO();

            leftSide = new BaseObject3D();
            leftSide.addTriangle(new Vector3(s, -s, -s), new Vector3(s, s, -s), new Vector3(s, s, s), new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0));
            leftSide.addTriangle(new Vector3(s, -s, -s), new Vector3(s, s, s), new Vector3(s, -s, s), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1));
            leftSide.CreateVAO();

            upSide = new BaseObject3D();
            upSide.addTriangle(new Vector3(-s, s, -s), new Vector3(s, s, -s), new Vector3(s, s, s), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1));
            upSide.addTriangle(new Vector3(-s, s, -s), new Vector3(s, s, s), new Vector3(-s, s, s), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1));
            upSide.CreateVAO();

            downSide = new BaseObject3D();
            downSide.addTriangle(new Vector3(-s, -s, -s), new Vector3(s, -s, -s), new Vector3(s, -s, s), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0));
            downSide.addTriangle(new Vector3(-s, -s, -s), new Vector3(s, -s, s), new Vector3(-s, -s, s), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 0));
            downSide.CreateVAO();

        }

        public void Draw()
        {

            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);

            GL.ActiveTexture(TextureUnit.Texture0);

            skyboxTextureMaterial.Draw(frontSide, frontID);
            skyboxTextureMaterial.Draw(backSide, backID);
            skyboxTextureMaterial.Draw(leftSide, leftID);
            skyboxTextureMaterial.Draw(rightSide, rightID);
            skyboxTextureMaterial.Draw(upSide, upID);
            skyboxTextureMaterial.Draw(downSide, downID);

            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);

        }

    }
}
