using cgimin.engine.camera;
using cgimin.engine.helpers;
using OpenTK;
using System;
using System.Collections.Generic;

namespace Engine.cgimin.engine.octree
{
    public class Octree
    {
        private static int drawCountStatistic;

        private static int maxIterationDepth = 5;

        internal List<Octree> children;

        internal Vector3 bMin;
        internal Vector3 bMax;

        internal Vector3 mid;
        internal float midRadius;

        public List<OctreeEntity> entities;

        private int iteration;

        private static List<OctreeEntity> transparentList = new List<OctreeEntity>();

        private Camera cam;

        public Octree(Vector3 boundsMin, Vector3 boundsMax, Camera cam, int iterationDepth = 1)
        {

            this.cam = cam;

            iteration = iterationDepth;

            bMin = boundsMin;
            bMax = boundsMax;

            mid = (bMin + bMax) / 2;
            midRadius = (mid - bMax).Length;

            children = new List<Octree>();
            for (int i = 0; i < 8; i++) children.Add(null);
        }


        public void AddEntity(OctreeEntity entity)
        {

            if (iteration == 1)
            {
                if (entities == null) entities = new List<OctreeEntity>();
                entities.Add(entity);
            }

            // extracting position from object transform matrix
            Vector3 pos = new Vector3(entity.Transform.M41, entity.Transform.M42, entity.Transform.M43);
            float radius = entity.Object3d.radius;

            if (Helpers.SphereAARectangleIntersect(pos, radius, bMin, bMax))
            {

                if (iteration == maxIterationDepth)
                {
                    // only add entity when at max iteration depth
                    if (entities == null) entities = new List<OctreeEntity>();
                    entities.Add(entity);
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                    {
                        // i is the index of the sub-bounding box
                        Vector3 dif = (bMax - bMin) / 2;
                        Vector3 bMinSub = bMin;
                        Vector3 bMaxSub = (bMin + bMax) / 2;

                        if (i % 2 == 1) { bMinSub.X += dif.X; bMaxSub.X += dif.X; }
                        if ((i / 2) % 2 == 1) { bMinSub.Y += dif.Y; bMaxSub.Y += dif.Y; }
                        if (i >= 4) { bMinSub.Z += dif.Z; bMaxSub.Z += dif.Z; }

                        // if object is inside the sub-boundary..
                        if (Helpers.SphereAARectangleIntersect(pos, radius, bMinSub, bMaxSub))
                        {
                            // create a new octree for it and add
                            if (children[i] == null) children[i] = new Octree(bMinSub, bMaxSub, this.cam, iteration + 1);
                            children[i].AddEntity(entity);
                        }
                    }
                }
            }
        }


        public void Draw()
        {

            if (iteration == 1)
            {
                transparentList.Clear();

                drawCountStatistic = 0;
                int len = entities.Count;
                for (int i = 0; i < len; i++) entities[i].drawn = false;
            }

            if (iteration == maxIterationDepth)
            {
                int len = entities.Count;
                for (int i = 0; i < len; i++)
                {
                    if (entities[i].drawn == false)
                    {
                        entities[i].drawn = true;

                        if (!entities[i].Material.isTransparent)
                        {
                            entities[i].Object3d.Transformation = entities[i].Transform;
                            entities[i].Material.DrawWithSettings(entities[i].Object3d, entities[i].MaterialSetting);
                        }
                        else
                        {
                            transparentList.Add(entities[i]);
                        }

                        drawCountStatistic++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    if (children[i] != null && cam.SphereIsInFrustum(children[i].mid, children[i].midRadius))
                    {
                        children[i].Draw();
                    }
                }
            }

            if (iteration == 1)
            {

                // Alle transparentren Objekte zeichnen
                foreach (OctreeEntity transEntity in transparentList)
                {
                    transEntity.distToCam = (new Vector3(transEntity.Transform.M41, transEntity.Transform.M42, transEntity.Transform.M43) - cam.Position).Length;
                }

                // Sortiert die Liste nach 'distToCam'
                transparentList.Sort((x, y) => y.distToCam.CompareTo(x.distToCam));

                foreach (OctreeEntity transEntity in transparentList)
                {
                    transEntity.Object3d.Transformation = transEntity.Transform;
                    transEntity.Material.DrawWithSettings(transEntity.Object3d, transEntity.MaterialSetting);
                }

            }
            // Transformation.M41, Transformation.M42, Transformation.M43

        }


    }
}
