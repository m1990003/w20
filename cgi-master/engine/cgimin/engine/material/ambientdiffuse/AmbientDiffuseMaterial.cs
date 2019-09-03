﻿using System;
using cgimin.engine.camera;
using cgimin.engine.object3d;
using cgimin.engine.light;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace cgimin.engine.material.ambientdiffuse
{
    public class AmbientDiffuseMaterial : BaseMaterial
    {
        private int modelMatrixLocation;
        private int modelviewProjectionMatrixLocation;

        private int lightDirectionLocation;
        private int lightAmbientLocation;
        private int lightDiffuseLocation;

        private Camera cam;

        public AmbientDiffuseMaterial(Camera cam)
        {

            this.cam = cam;


            // shader-programm is loaded
            CreateShaderProgram("cgimin/engine/material/ambientdiffuse/AmbientDiffuse_VS.glsl",
                                "cgimin/engine/material/ambientdiffuse/AmbientDiffuse_FS.glsl");

            // GL.BindAttribLocation, defines which index of the data-structure is assigned to which "in" parameter 
            GL.BindAttribLocation(Program, 0, "in_position");
            GL.BindAttribLocation(Program, 1, "in_normal");
            GL.BindAttribLocation(Program, 2, "in_uv");

            // ...has to be done before the final "link" of the shader-program
            GL.LinkProgram(Program);

            // the location of the "uniform"-paramter "modelview_projection_matrix" on the shader is saved to modelviewProjectionMatrixLocation
            modelviewProjectionMatrixLocation = GL.GetUniformLocation(Program, "modelview_projection_matrix");

            // the location of the "uniform"-paramter for the model matrix on the shader is saved to modelviewMatrixLocation
            modelMatrixLocation = GL.GetUniformLocation(Program, "model_matrix");

            /// the location of the "uniform"-paramters of the light parameters
            lightDirectionLocation = GL.GetUniformLocation(Program, "light_direction");
            lightAmbientLocation = GL.GetUniformLocation(Program, "light_ambient_color");
            lightDiffuseLocation = GL.GetUniformLocation(Program, "light_diffuse_color");

        }

        public void Draw(BaseObject3D object3d, int textureID)
        {
         //   GL.Enable(EnableCap.Texture2D);

            // set the texture
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            // using the Vertex-Array-Object of out object
            GL.BindVertexArray(object3d.Vao);

            // using our shader
            GL.UseProgram(Program);

            // The matrix which we give as "modelview_projection_matrix" is assembled:
            // object-transformation * camera-transformation * perspective projection of the camera
            // on the shader each vertex-position is multiplied by this matrix. The result is the final position on the screen
            Matrix4 modelViewProjection = object3d.Transformation * cam.Transformation * cam.PerspectiveProjection;

            // modelViewProjection is passed to the shader
            GL.UniformMatrix4(modelviewProjectionMatrixLocation, false, ref modelViewProjection);

            // The model matrix (just the transformation of the object) is also given to the shader. We want to multiply our normals with this matrix, to have them in world space.
            Matrix4 model = object3d.Transformation;
            GL.UniformMatrix4(modelMatrixLocation, false, ref model);

            // Die Licht Parameter werden übergeben
            GL.Uniform3(lightDirectionLocation, Light.lightDirection);
            GL.Uniform4(lightAmbientLocation, Light.lightAmbient);
            GL.Uniform4(lightDiffuseLocation, Light.lightDiffuse);

            // the object is drawn
            GL.DrawElements(PrimitiveType.Triangles, object3d.Indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.BindVertexArray(0);
        }


        // implementatin for octree drawing logic
        public override void DrawWithSettings(BaseObject3D object3d, MaterialSettings settings)
        {
            Draw(object3d, settings.colorTexture);
        }


    }
}
