﻿using System;
using cgimin.engine.camera;
using cgimin.engine.object3d;
using cgimin.engine.light;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace cgimin.engine.material.normalmapping
{
    public class NormalMappingMaterial : BaseMaterial
    {
        private int modelMatrixLocation;
        private int modelviewProjectionMatrixLocation;

        private int lightDirectionLocation;
        private int lightAmbientLocation;
        private int lightDiffuseLocation;
        private int lightSpecularLocation;
        private int cameraPositionLocation;
        private int materialShininessLocation;

        private int colorTextureLocation;
        private int normalTextureLocation;

        private Camera cam;

        public NormalMappingMaterial(Camera cam)
        {

            this.cam = cam;


            // Shader-Programm wird aus den externen Files generiert...
            CreateShaderProgram("cgimin/engine/material/normalmapping/NormalMapping_VS.glsl",
                                "cgimin/engine/material/normalmapping/NormalMapping_FS.glsl");

            // GL.BindAttribLocation, gibt an welcher Index in unserer Datenstruktur welchem "in" Parameter auf unserem Shader zugeordnet wird
            // folgende Befehle müssen aufgerufen werden...
            GL.BindAttribLocation(Program, 0, "in_position");
            GL.BindAttribLocation(Program, 1, "in_normal");
            GL.BindAttribLocation(Program, 2, "in_uv");
            GL.BindAttribLocation(Program, 3, "in_tangent");
            GL.BindAttribLocation(Program, 4, "in_bitangent");

            // ...bevor das Shader-Programm "gelinkt" wird.
            GL.LinkProgram(Program);

            // Die Stelle an der im Shader der per "uniform" der Input-Paremeter "modelview_projection_matrix" definiert wird, wird ermittelt.
            modelviewProjectionMatrixLocation = GL.GetUniformLocation(Program, "modelview_projection_matrix");

            // Die Stelle für die den "model_matrix" - Parameter wird ermittelt.
            modelMatrixLocation = GL.GetUniformLocation(Program, "model_matrix");

            // Die Stelle fur den "specular_shininess" - Parameter
            materialShininessLocation = GL.GetUniformLocation(Program, "specular_shininess");

            // Die Stellen im Fragemant-Shader für Licht-parameter ermitteln.
            lightDirectionLocation = GL.GetUniformLocation(Program, "light_direction");
            lightAmbientLocation = GL.GetUniformLocation(Program, "light_ambient_color");
            lightDiffuseLocation = GL.GetUniformLocation(Program, "light_diffuse_color");
            lightSpecularLocation = GL.GetUniformLocation(Program, "light_specular_color");
            cameraPositionLocation = GL.GetUniformLocation(Program, "camera_position");
            colorTextureLocation = GL.GetUniformLocation(Program, "color_texture");
            normalTextureLocation = GL.GetUniformLocation(Program, "normalmap_texture");
        }



        public void Draw(BaseObject3D object3d, int textureID, int normalTextureID, float shininess)
        {

            // Das Vertex-Array-Objekt unseres Objekts wird benutzt
            GL.BindVertexArray(object3d.Vao);

            // Unser Shader Programm wird benutzt
            GL.UseProgram(Program);

            // Farb-Textur wird "gebunden"
            GL.Uniform1(colorTextureLocation, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            // Normalmap-Textur wird "gebunden"
            GL.Uniform1(normalTextureLocation, 1);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, normalTextureID);

            // Die Matrix, welche wir als "modelview_projection_matrix" übergeben, wird zusammengebaut:
            // Objekt-Transformation * Kamera-Transformation * Perspektivische Projektion der kamera.
            // Auf dem Shader wird jede Vertex-Position mit dieser Matrix multipliziert. Resultat ist die Position auf dem Screen.
            Matrix4 modelViewProjection = object3d.Transformation * cam.Transformation * cam.PerspectiveProjection;

            // Die ModelViewProjection Matrix wird dem Shader als Parameter übergeben
            GL.UniformMatrix4(modelviewProjectionMatrixLocation, false, ref modelViewProjection);

            // Die Model-Matrix wird dem Shader übergeben, zur transformation der Normalen
            // und der Berechnung des Winkels Betrachter / Objektpunkt 
            Matrix4 model = object3d.Transformation;
            GL.UniformMatrix4(modelMatrixLocation, false, ref model);

            // Die Licht Parameter werden übergeben
            GL.Uniform3(lightDirectionLocation, Light.lightDirection);
            GL.Uniform4(lightAmbientLocation, Light.lightAmbient);
            GL.Uniform4(lightDiffuseLocation, Light.lightDiffuse);
            GL.Uniform4(lightSpecularLocation, Light.lightSpecular);

            // Shininess
            GL.Uniform1(materialShininessLocation, shininess);

            // Positions Parameter
            GL.Uniform4(cameraPositionLocation, new Vector4(cam.Position.X, cam.Position.Y, cam.Position.Z, 1));

            // Das Objekt wird gezeichnet
            GL.DrawElements(PrimitiveType.Triangles, object3d.Indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Active Textur wieder auf 0, um andere Materialien nicht durcheinander zu bringen
            GL.ActiveTexture(TextureUnit.Texture0);

            GL.BindVertexArray(0);
        }


        // implementatin for octree drawing logic
        public override void DrawWithSettings(BaseObject3D object3d, MaterialSettings settings)
        {
            Draw(object3d, settings.colorTexture, settings.normalTexture, settings.shininess);
        }


    }
}
