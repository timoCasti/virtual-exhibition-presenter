using System;
using Unibas.DBIS.DynamicModelling.Models;
using UnityEngine;

namespace Unibas.DBIS.DynamicModelling
{
    public static class ModelFactory
    {
        /// <summary>
        /// Quad of sorts:
        ///
        /// c---d
        /// |   |
        /// a---b
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        public static GameObject CreateFreeformQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d,
            Material material = null)
        {
            GameObject go = new GameObject("FreeformQuad");
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
            Mesh mesh = meshFilter.mesh;
            Vector3[] vertices = new[]
            {
                a, b, c, d
            };
            mesh.vertices = vertices;

            int[] tri = new int[6];

            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;

            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;

            mesh.triangles = tri;

            /*
            Vector3[] normals = new Vector3[4];

            normals[0] = -Vector3.forward;
            normals[1] = -Vector3.forward;
            normals[2] = -Vector3.forward;
            normals[3] = -Vector3.forward;

            mesh.normals = normals;*/

            Vector2[] uv = new Vector2[4];

            /*
            float xUnit = 1;
            float yUnit = 1;

            if (width > height)
            {
                xUnit = width / height;
            }
            else
            {
                yUnit = height / width;
            }
            */

            // TODO

            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);

            mesh.uv = uv;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            if (material != null)
            {
                meshRenderer.material.CopyPropertiesFromMaterial(material);
                //meshRenderer.material.SetTextureScale("_MainTex", new Vector2(1,1));
                meshRenderer.material.name = material.name + "(Instance)";
            }
            else
            {
                meshRenderer.material = new Material(Shader.Find("Standard"));
                meshRenderer.material.color = Color.white;
            }


            return go;
        }
        
        public static GameObject CreateFreeWall(Vector3[] coordinates, Material material = null)
        {
            GameObject go = new GameObject("FreeWall");
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
            Mesh mesh = meshFilter.mesh;
            
            mesh.vertices = coordinates;

            int[] tri = new int[6];
            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;

            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;
            mesh.triangles = tri;

            Vector3[] normals = new Vector3[4];
            normals[0] = -Vector3.forward;
            normals[1] = -Vector3.forward;
            normals[2] = -Vector3.forward;
            normals[3] = -Vector3.forward;
            mesh.normals = normals;

            Vector2[] uv = new Vector2[4];

            float width = Vector3.Distance(coordinates[0],coordinates[1]);
            float height = Vector3.Distance(coordinates[0], coordinates[3]);
                        
            float xUnit = 1;
            float yUnit = 1;

            if (width > height)
            {
                xUnit = width / height;
            }
            else
            {
                yUnit = height / width;
            }

            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(xUnit, 0);
            uv[2] = new Vector2(0, yUnit);
            uv[3] = new Vector2(xUnit, yUnit);
            
            mesh.uv = uv;

            
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            if (material != null)
            {
                meshRenderer.material.CopyPropertiesFromMaterial(material);
                //meshRenderer.material.SetTextureScale("_MainTex", new Vector2(1,1));
                meshRenderer.material.name = material.name + "(Instance)";
            }
            else
            {
                meshRenderer.material = new Material(Shader.Find("Standard"));
                meshRenderer.material.color = Color.white;
            }
            
            
            //postion wall
            float a = Vector3.Angle(coordinates[0]-coordinates[1], Vector3.right);
        
            //add collider for teleportation limits
            var collider = go.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;
            
            return go;
            
            
            
            

            return go;
        }
        /*
         * Creates the "walls" for ceiling or floor out of various amounts of vertices
         *
         * gets all vertices of floor or ceiling as Vector3[]
         * A Material
         * String "Floor" or "Ceiling" necessary to face in correct direction
         */
        public static GameObject CreatePolygonalMeshes(Vector3[] vertices, Material material, String FloororCeiling)
        {
            GameObject go =new GameObject("PolygonalWall");
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
            Mesh mesh = meshFilter.mesh;

            mesh.vertices = vertices;
           
            // transform Vector3 to Vector2 with x and z only, since height not needed
            Vector2[] vector2s=new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++) {
                vector2s[i].x = vertices[i].x;
                vector2s[i].y = vertices[i].z;
            }
            // Method which calculates polygons triangluars
            Triangulator tr = new Triangulator(vector2s);
            int[] indices = tr.Triangulate();

            if (string.Equals(FloororCeiling, "Floor")) {
                System.Array.Reverse(indices);
            }
 
            // Create the Vector3 vertices  ??
            Vector3[] verticesOfpoly = new Vector3[vector2s.Length];
            for (int i=0; i<vertices.Length; i++) {
                vertices[i] = new Vector3(vector2s[i].x, vector2s[i].y, 0);
            }

            mesh.triangles = indices;
            
            //Maybe we need normals and uv
            /*Vector2[] uvs = new Vector2[vertices.Length];

            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
                Debug.Log(uvs[i]);
            }
            */
            mesh.uv = vector2s;
            
            
            
            
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            
            
            
            if (material != null)
            {
                meshRenderer.material.CopyPropertiesFromMaterial(material);
                //meshRenderer.material.SetTextureScale("_MainTex", new Vector2(1,1));
                meshRenderer.material.name = material.name + "(Instance)";
            }
            else
            {
                meshRenderer.material = new Material(Shader.Find("Standard"));
                meshRenderer.material.color = Color.white;
            }
            
            

            return go;
        }
        
        
        

        /// <summary>
        /// Creates a wall between two positions
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="height"></param>
        /// <param name="materialName"></param>
        /// <returns></returns>
        public static GameObject CreatePositionedWall(Vector3 start, Vector3 end, float height,
            string materialName = null)
        {
            float width = Vector3.Distance(start, end);
            float a = Vector3.Angle(end - start, Vector3.right);
            GameObject go = new GameObject("PositionedWall");
            GameObject wall = CreateWall(width, height, materialName);

            wall.transform.parent = go.transform;
            wall.transform.position = Vector3.zero;
            wall.transform.Rotate(Vector3.up, -a);
            go.transform.position = start;
            return go;
        }

        public static GameObject CreateWall(WallModel model)
        {
            float width = Vector3.Distance(model.Start, model.End);
            float a = Vector3.Angle(model.Start - model.End, Vector3.right);
            GameObject go = new GameObject("PositionedWall");
            GameObject wall = CreateWall(width, model.Height, model.Material);

            wall.transform.parent = go.transform;
            wall.transform.position = Vector3.zero;
            wall.transform.Rotate(Vector3.up, -a);
            go.transform.position = model.Start;
            go.AddComponent<ModelContainer>().Model = model;
            return go;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">center of room</param>
        /// <param name="size"></param>
        /// <param name="height"></param>
        /// <param name="materialNames">0 floor, 1 ceiling, 2 north (pos z), 3 east (pos x), 4 south (neg z), 5 west (neg x)</param>
        /// <returns></returns>
        public static GameObject CreateCuboidRoom(Vector3 position, float size, float height, string[] materialNames)
        {
            GameObject root = new GameObject("SquareRoom");

            float halfSize = size / 2f;

            // North wall
            GameObject north = CreateWall(size, height, materialNames[2]);
            north.name = "NorthWall";
            north.transform.parent = root.transform;
            north.transform.position = new Vector3(-halfSize, 0, halfSize);
            // East wall
            GameObject east = CreateWall(size, height, materialNames[3]);
            east.name = "EastWall";
            east.transform.parent = root.transform;
            east.transform.position = new Vector3(halfSize, 0, halfSize);
            east.transform.Rotate(Vector3.up, 90);
            // South wall
            GameObject south = CreateWall(size, height, materialNames[4]);
            south.name = "SouthWall";
            south.transform.parent = root.transform;
            south.transform.position = new Vector3(halfSize, 0, -halfSize);
            south.transform.Rotate(Vector3.up, 180);
            // West wall
            GameObject west = CreateWall(size, height, materialNames[5]);
            west.name = "WestWall";
            west.transform.parent = root.transform;
            west.transform.position = new Vector3(-halfSize, 0, -halfSize);
            west.transform.Rotate(Vector3.up, 270);

            // Floor
            GameObject floorAnchor = new GameObject("FloorAnchor");
            floorAnchor.transform.parent = root.transform;

            GameObject floor = CreateWall(size, size, materialNames[0]);
            floor.name = "Floor";
            floor.transform.parent = floorAnchor.transform;
            // North Aligned
            floorAnchor.transform.position = new Vector3(-halfSize, 0, -halfSize);
            floorAnchor.transform.Rotate(Vector3.right, 90);
            // East Aligned
            //floorAnchor.transform.position = new Vector3(-halfSize, 0, halfSize);
            //floorAnchor.transform.Rotate(Vector3f.back,90);

            // Ceiling
            GameObject ceilingAnchor = new GameObject("CeilingAnchor");
            ceilingAnchor.transform.parent = root.transform;

            GameObject ceiling = CreateWall(size, size, materialNames[1]);
            ceiling.name = "Ceiling";
            ceiling.transform.parent = ceilingAnchor.transform;

            root.transform.position = position;
            // North Aligned
            ceilingAnchor.transform.position = new Vector3(halfSize, height, halfSize);
            ceilingAnchor.transform.Rotate(Vector3.right, -90);
            // East Aligned
            //ceilingAnchor.transform.position = new Vector3(halfSize, height, -halfSize);
            //ceilingAnchor.transform.Rotate( Vector3.back, -90);

            return root;
        }

        public static GameObject CreateCuboidRoom(CuboidRoomModel model)
        {
            GameObject root = new GameObject("CuboidRoom");

            float halfSize = model.GetSize() / 2f;

            // North wall
            GameObject north = CreateWall(model.GetSize(), model.Height, model.NorthMaterial);
            north.name = "NorthWall";
            north.transform.parent = root.transform;
            north.transform.position = new Vector3(-halfSize, 0, halfSize);
            // East wall
            GameObject east = CreateWall(model.GetSize(), model.Height, model.EastMaterial);
            east.name = "EastWall";
            east.transform.parent = root.transform;
            east.transform.position = new Vector3(halfSize, 0, halfSize);
            east.transform.Rotate(Vector3.up, 90);
            // South wall
            GameObject south = CreateWall(model.GetSize(), model.Height, model.SouthMaterial);
            south.name = "SouthWall";
            south.transform.parent = root.transform;
            south.transform.position = new Vector3(halfSize, 0, -halfSize);
            south.transform.Rotate(Vector3.up, 180);
            // West wall
            GameObject west = CreateWall(model.GetSize(), model.Height, model.WestMaterial);
            west.name = "WestWall";
            west.transform.parent = root.transform;
            west.transform.position = new Vector3(-halfSize, 0, -halfSize);
            west.transform.Rotate(Vector3.up, 270);

            // Floor
            GameObject floorAnchor = new GameObject("FloorAnchor");
            floorAnchor.transform.parent = root.transform;

            GameObject floor = CreateWall(model.GetSize(), model.GetSize(), model.FloorMaterial);
            floor.name = "Floor";
            floor.transform.parent = floorAnchor.transform;
            // North Aligned
            floorAnchor.transform.position = new Vector3(-halfSize, 0, -halfSize);
            floorAnchor.transform.Rotate(Vector3.right, 90);
            // East Aligned
            //floorAnchor.transform.position = new Vector3(-halfSize, 0, halfSize);
            //floorAnchor.transform.Rotate(Vector3f.back,90);

            // Ceiling
            GameObject ceilingAnchor = new GameObject("CeilingAnchor");
            ceilingAnchor.transform.parent = root.transform;

            GameObject ceiling = CreateWall(model.GetSize(), model.GetSize(), model.CeilingMaterial);
            ceiling.name = "Ceiling";
            ceiling.transform.parent = ceilingAnchor.transform;

            
            // North Aligned
            ceilingAnchor.transform.position = new Vector3(-halfSize, model.Height, halfSize);
            ceilingAnchor.transform.Rotate(Vector3.right, -90);
            // East Aligned
            //ceilingAnchor.transform.position = new Vector3(halfSize, height, -halfSize);
            //ceilingAnchor.transform.Rotate( Vector3.back, -90);

            root.transform.position = model.Position;
            
            root.AddComponent<ModelContainer>().Model = model;
            return root;
        }
        
        /**
         * Creates corridor model
         */
        public static GameObject CreateCorridor(CuboidCorridorModel model)
        {
            GameObject root = new GameObject("CuboidCorridor");

            float halfSize = model.GetSize() / 2f;

            // North wall
            GameObject north = CreateWall(model.GetSize(), model.Height, model.NorthMaterial);
            north.name = "NorthWall";
            north.transform.parent = root.transform;
            north.transform.position = new Vector3(-halfSize, 0, halfSize);
            // South wall
            GameObject south = CreateWall(model.GetSize(), model.Height, model.SouthMaterial);
            south.name = "SouthWall";
            south.transform.parent = root.transform;
            south.transform.position = new Vector3(halfSize, 0, -halfSize);
            south.transform.Rotate(Vector3.up, 180);

            // Floor
            GameObject floorAnchor = new GameObject("FloorAnchor");
            floorAnchor.transform.parent = root.transform;

            GameObject floor = CreateWall(model.GetSize(), model.GetSize(), model.FloorMaterial);
            floor.name = "Floor";
            floor.transform.parent = floorAnchor.transform;
            // North Aligned
            floorAnchor.transform.position = new Vector3(-halfSize, 0, -halfSize);
            floorAnchor.transform.Rotate(Vector3.right, 90);
            // East Aligned
            //floorAnchor.transform.position = new Vector3(-halfSize, 0, halfSize);
            //floorAnchor.transform.Rotate(Vector3f.back,90);

            root.transform.position = model.Position;
            
            root.AddComponent<ModelContainer>().Model = model;
            return root;
        }

        public static GameObject CreatePolygonalRoom(PolygonRoomModel model)
        {
            
            GameObject go=new GameObject("PolyRoom");
            //find vertices of ceiling and floor depends on the structure of the wallcoordinates
            var floorvertices=new System.Collections.Generic.List<Vector3>();
            var ceilingvertices=new System.Collections.Generic.List<Vector3>();
            for (int i = 0; i < model.numberOfWalls; i++) {
                if (floorvertices.Contains(model.walls[i].wallCoordinates[0])) {
                    Debug.Log("Floor has same vertice multiple times");
                }
                else {
                    floorvertices.Add(model.walls[i].wallCoordinates[0]);
                }

                if (ceilingvertices.Contains(model.walls[i].wallCoordinates[2])) {
                    Debug.Log("Ceiling contains same vertice multiple times");
                }
                else {
                    ceilingvertices.Add(model.walls[i].wallCoordinates[2]);
                }
            }
           
            
            // ceiling scaling
            var scaling = model.ceiling_scale; // Hardcode for testing
            
            Vector3 midOfCeiling=new Vector3(0,0,0);
            for (int i = 0; i < ceilingvertices.Count; i++) {
                midOfCeiling += ceilingvertices[i];

            }

            Vector3 scalevector0;
            Vector3 scalevector1;
            Vector3 scalevector2;
            midOfCeiling = midOfCeiling / ceilingvertices.Count;
            //Debug.Log("mid   "+ midOfCeiling);
            for (int i = 0; i < model.walls.Length; i++) {
                scalevector0 = ceilingvertices[i]-midOfCeiling;
                scalevector1 = model.walls[i].wallCoordinates[2]- midOfCeiling;
                scalevector2 = model.walls[i].wallCoordinates[3]- midOfCeiling;
                
                ceilingvertices[i] = midOfCeiling+ (scalevector1 * (float) scaling);
               
                model.walls[i].wallCoordinates[2].x = midOfCeiling.x+ (scalevector1.x* (float) scaling);
                model.walls[i].wallCoordinates[2].z = midOfCeiling.z+ (scalevector1.z* (float) scaling);
                model.walls[i].wallCoordinates[3].x = midOfCeiling.x+ (scalevector2.x* (float) scaling);
                model.walls[i].wallCoordinates[3].z = midOfCeiling.z+ (scalevector2.z* (float) scaling);

            }
            
            // convert list to array, since its needed as parameter
            var ceilingArray = ceilingvertices.ToArray();
            var floorArray=floorvertices.ToArray();
            
            // Ceiling
            GameObject ceilingAnchor = new GameObject("CeilingAnchor");
            ceilingAnchor.transform.parent = root.transform;

            GameObject ceiling = CreateWall(model.GetSize(), model.GetSize(), model.CeilingMaterial);
            ceiling.name = "Ceiling";
            ceiling.transform.parent = ceilingAnchor.transform;
            //ceilingAnchor.transform.position = new Vector3(0, model.height, 0);
            
            // North Aligned
            ceilingAnchor.transform.position = new Vector3(-halfSize, model.Height, halfSize);
            ceilingAnchor.transform.Rotate(Vector3.right, -90);
            // East Aligned
            //ceilingAnchor.transform.position = new Vector3(halfSize, height, -halfSize);
            //ceilingAnchor.transform.Rotate( Vector3.back, -90);

            root.transform.position = model.Position;
            
            root.AddComponent<ModelContainer>().Model = model;
            return root;
            
        }
        
        


        /// <summary>
        /// Creates a wall game object to position later.
        /// The wall is flat and always "upright", e.g. the normal of the mesh is negative z.
        /// Use the resulting gameobject to rotate and re-position the wall.
        /// </summary>
        /// <param name="width">The width of the wall in Unity units</param>
        /// <param name="height">The height of the wall in Unity units</param>
        /// <param name="materialName">The wall's material name. Expects the material file to be at Resources/Materials/materalName. If no present, the word Material will be suffixed.</param>
        /// <returns></returns>
        public static GameObject CreateWall(float width, float height, string materialName = null)
        {

            return CreateWall(width, height, LoadMaterialByName(materialName));
        }

        private static Material LoadMaterialByName(string materialName)
        {
            if (!string.IsNullOrEmpty(materialName))
            {
                if (!materialName.EndsWith("Material"))
                {
                    materialName = materialName + "Material";
                }

                return Resources.Load<Material>("Materials/" + materialName);
            }

            return null;
        }

        public static GameObject CreateWall(float width, float height, Material mat = null)
        {
            GameObject go = new GameObject("Wall");
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
            Mesh mesh = meshFilter.mesh;
            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(0, 0, 0);
            vertices[1] = new Vector3(width, 0, 0);
            vertices[2] = new Vector3(0, height, 0);
            vertices[3] = new Vector3(width, height, 0);

            mesh.vertices = vertices;

            int[] tri = new int[6];

            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;

            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;

            mesh.triangles = tri;

            Vector3[] normals = new Vector3[4];

            normals[0] = -Vector3.forward;
            normals[1] = -Vector3.forward;
            normals[2] = -Vector3.forward;
            normals[3] = -Vector3.forward;

            mesh.normals = normals;

            Vector2[] uv = new Vector2[4];

            float xUnit = 1;
            float yUnit = 1;

            if (width > height)
            {
                xUnit = width / height;
            }
            else
            {
                yUnit = height / width;
            }

            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(xUnit, 0);
            uv[2] = new Vector2(0, yUnit);
            uv[3] = new Vector2(xUnit, yUnit);

            mesh.uv = uv;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            if (mat != null)
            {
                meshRenderer.material.CopyPropertiesFromMaterial(mat);
                //meshRenderer.material.SetTextureScale("_MainTex", new Vector2(1,1));
                meshRenderer.material.name = mat.name;
            }
            else
            {
                meshRenderer.material = new Material(Shader.Find("Standard"));
                meshRenderer.material.color = Color.white;
            }

            // TODO Highly experimental!

            var boxCollider = go.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(width,height,0.0001f);

            return go;
        }


        private static Vector3 CalculateUnit(Vector3 dimensions)
        {
            float m = Math.Max(Math.Max(dimensions.x, dimensions.y), dimensions.z);
            return new Vector3(m/dimensions.x, m/dimensions.y, m/dimensions.z);
        }
        
        private static Vector2 CalculateUnit(float width, float height)
        {
            return CalculateNormalizedToLeastSquareUnit(width, height);
            //return CalculateNormalizedToOneUnit(width, height);
        }

        private static Vector2 CalculateNormalizedToLeastSquareUnit(float width, float height)
        {
            float xUnit = 1,
                yUnit = 1;

            if (width > height)
            {
                xUnit = width / height;
            }
            else
            {
                yUnit = height / width;
            }

            return new Vector2(xUnit, yUnit);
        }

        private static Vector2 CalculateNormalizedToOneUnit(float width, float height)
        {
            return new Vector2(1f / width, 1f / height);
        }

        public static GameObject CreateCuboid(CuboidModel cuboid)
        {
            GameObject cub = CreateCuboid(cuboid.Width, cuboid.Height, cuboid.Depth);
            MeshRenderer meshRenderer = cub.GetComponent<MeshRenderer>();
            if (cuboid.Material != null)
            {
                meshRenderer.material.CopyPropertiesFromMaterial(cuboid.Material);
            }
            else
            {
                meshRenderer.material = new Material(Shader.Find("Standard"));
                meshRenderer.material.name = "Default";
                meshRenderer.material.color = Color.white;
            }
            cub.AddComponent<ModelContainer>().Model = cuboid;
            return cub;
        }

        public static GameObject CreateCuboid(float width, float height, float depth)
        {
            GameObject go = new GameObject("Cuboid");
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
            Mesh mesh = meshFilter.mesh;

            // The naming is always from the front and downwards looking! e.g. From the back, left and right is swapped
            Vector3 frontLeftDown = Vector3.zero;
            Vector3 frontRightDown = new Vector3(width, 0, 0);
            Vector3 frontLeftUp = new Vector3(0, height, 0);
            Vector3 frontRightUp = new Vector3(width, height, 0);

            Vector3 backLeftDown = new Vector3(0, 0, depth);
            Vector3 backRightDown = new Vector3(width, 0, depth);
            Vector3 backLeftUp = new Vector3(0, height, depth);
            Vector3 backRightUp = new Vector3(width, height, depth);

            Vector3[] vertices = new[]
            {
                // Front
                frontLeftDown, frontRightDown, frontLeftUp, frontRightUp,
                // Back
                backLeftDown, backRightDown, backLeftUp, backRightUp,
                // Left
                backLeftDown, frontLeftDown, backLeftUp, frontLeftUp,
                // Right
                frontRightDown, backRightDown, frontRightUp, backRightUp,
                // Up
                frontLeftUp, frontRightUp, backLeftUp, backRightUp,
                // Down
                frontLeftDown, frontRightDown, backLeftDown, backRightDown
            };
            mesh.vertices = vertices;

            int[] triangles = new[]
            {
                // Front
                0, 2, 1, 2, 3, 1,
                // Back
                5, 7, 4, 7, 6, 4,
                // Left
                8, 10, 9, 10, 11, 9,
                // Right
                12, 14, 13, 14, 15, 13,
                // Up
                16, 18, 17, 18, 19, 17,
                // Down
                21, 23, 20, 23, 22, 20
            };
            mesh.triangles = triangles;

            Vector3[] normals = new[]
            {
                // Front
                -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward,
                // Back
                -Vector3.back, -Vector3.back, -Vector3.back, -Vector3.back,
                // Left
                -Vector3.left, -Vector3.left, -Vector3.left, -Vector3.left,
                // Right
                -Vector3.right, -Vector3.right, -Vector3.right, -Vector3.right,
                // Up
                -Vector3.up, -Vector3.up, -Vector3.up, -Vector3.up,
                // Down
                -Vector3.down, -Vector3.down, -Vector3.down, -Vector3.down
            };
            mesh.normals = normals;

            
            /*
             * Unwrapping of mesh for uf like following
             *  U
             * LFRB
             *  D
            */

            var u = Math.Min(Math.Min(width, height), depth);
            var w = width / u;
            var h = height / u;
            var d = depth / u;

            Vector2 uvUnits = new Vector2(u,u);
            var fOff = uvUnits.x * depth;
            var rOff = uvUnits.x * width + fOff;
            var bOff = uvUnits.x * depth + rOff;
            var uOff = uvUnits.y * depth + uvUnits.y * height;
            Vector2[] uv = new[]
            {
                // Front
                new Vector2(fOff, uvUnits.y * depth), new Vector2(fOff + uvUnits.x * width, uvUnits.y * depth),
                new Vector2(fOff, uvUnits.y * depth + uvUnits.y * height),
                new Vector2(fOff + uvUnits.x * width, uvUnits.y * depth + uvUnits.y * height),

                // Back
                new Vector2(bOff, uvUnits.y * depth), new Vector2(bOff + uvUnits.x * width, uvUnits.y * depth),
                new Vector2(bOff, uvUnits.y * depth + uvUnits.y * height),
                new Vector2(bOff + uvUnits.x * width, uvUnits.y * depth + uvUnits.y * height),

                // Left
                new Vector2(0, uvUnits.y * depth), new Vector2(uvUnits.x * depth, uvUnits.y * depth),
                new Vector2(0, uvUnits.y * depth + uvUnits.y * height),
                new Vector2(uvUnits.x * depth, uvUnits.y * depth + uvUnits.y * height),
                // Right
                new Vector2(rOff, uvUnits.y * depth), new Vector2(rOff + uvUnits.x * depth, uvUnits.y * depth),
                new Vector2(rOff, uvUnits.y * depth + uvUnits.y * height),
                new Vector2(rOff + uvUnits.x * depth, uvUnits.y * depth + uvUnits.y * height),
                // Up
                new Vector2(fOff, uOff), new Vector2(fOff + uvUnits.x * width, uOff),
                new Vector2(fOff, uOff + uvUnits.y * depth),
                new Vector2(fOff + uvUnits.x * width, uOff + uvUnits.y * depth),

                // Down
                new Vector2(fOff, 0), new Vector2(fOff + uvUnits.x * width, 0), new Vector2(fOff, uvUnits.y * depth),
                new Vector2(fOff + uvUnits.x * width, uvUnits.y * depth)
            };

            mesh.uv = uv;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            meshRenderer.material = new Material(Shader.Find("Standard"));
            meshRenderer.material.name = "Default";
            meshRenderer.material.color = Color.green;

            return go;
        }

        public static GameObject CreateModel(ComplexCuboidModel model)
        {
            GameObject root = new GameObject("ComplexCuboid");
            for (int i = 0; i < model.Size(); i++)
            {
                Vector3 pos = model.GetPositionAt(i);
                CuboidModel cuboid = model.GetCuboidAt(i);
                GameObject cub = CreateCuboid(cuboid);
                cub.transform.parent = root.transform;
                cub.transform.position = pos;
            }

            root.AddComponent<ModelContainer>().Model = model;
            return root;
        }
    }
}