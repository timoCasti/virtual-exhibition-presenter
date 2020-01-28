using System;
using System.Collections.Generic;
using System.Linq;
using Unibas.DBIS.DynamicModelling.Models;
using UnityEngine;
using Object = UnityEngine.Object;

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
        public static GameObject CreateFreeform(Vector3 a, Vector3 b, Vector3 c, Vector3 d,
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

            if (material != null) {
                meshRenderer.material.CopyPropertiesFromMaterial(material);
                //meshRenderer.material.SetTextureScale("_MainTex", new Vector2(1,1));
                meshRenderer.material.name = material.name + "(Instance)";
            }
            else {
                meshRenderer.material = new Material(Shader.Find("Standard"));
                meshRenderer.material.color = Color.white;
            }


            return go;
        }

        /*
         * Creates a wall based on 4 coordinates (Vector3)
         * para:  Vector3 [] coordinates // the coordinates of the walls vertices ( "4"!!! )
         *        Material material // material of the wall can be null
         * returns the Wall as GameObject
         */
        public static GameObject CreateFreeWall(Vector3[] coordinates, Material material = null,Boolean ceilingOrFloor= false)
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

            var uv2 = new Vector2[coordinates.Length];
            //math for uv coordinates
            Vector3[] copy = new Vector3[4];
            coordinates.CopyTo(copy, 0);

            copy[1] = copy[1] - copy[0];
            copy[2] = copy[2] - copy[0];
            copy[3] = copy[3] - copy[0];
            copy[0] = new Vector3(0, 0, 0);
            mesh.RecalculateNormals();
            Vector3 normal = Vector3.Cross(copy[1] - copy[0], copy[2] - copy[0]).normalized;
            Vector3 u = new Vector3();
            u = Vector3.ProjectOnPlane(Vector3.up, normal);
            if (ceilingOrFloor) {
                u = Vector3.ProjectOnPlane(Vector3.back, normal);
            }
            Vector3 v = Vector3.Cross(u, normal).normalized;
            uv2[0] = new Vector2(Vector3.Dot(copy[0], v), Vector3.Dot(copy[0], u));
            uv2[1] = new Vector2(Vector3.Dot(copy[1], v), Vector3.Dot(copy[1], u));
            uv2[2] = new Vector2(Vector3.Dot(copy[2], v), Vector3.Dot(copy[2], u));
            uv2[3] = new Vector2(Vector3.Dot(copy[3], v), Vector3.Dot(copy[3], u));
            mesh.uv = uv2;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            if (material != null) {
                meshRenderer.material.CopyPropertiesFromMaterial(material);
                meshRenderer.material.SetTextureScale("_MainTex", new Vector2(0.5f, 0.5f));
                meshRenderer.material.name = material.name + "(Instance)";
            }
            else {
                meshRenderer.material = new Material(Shader.Find("Standard"));
                meshRenderer.material.color = Color.white;
            }

            //add collider for teleportation limits
            var collider = go.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;

            return go;
        }

        /*
         * Creates the "walls" for ceiling or floor out of various amounts of vertices
         *
         * gets all vertices of floor or ceiling as Vector3[]
         * A Material
         * String "Floor" or "Ceiling" necessary to face in correct direction
         */
        public static GameObject CreatePolygonalMeshes(Vector3[] vertices, Material material, String FloororCeiling,
            bool swap)
        {
            GameObject go = new GameObject("PolygonalWall");
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
            Mesh mesh = meshFilter.mesh;

            mesh.vertices = vertices;

            // transform Vector3 to Vector2 with x and z only, since height not needed
            var shorty = 1000f;
            var j = 0;
            Vector2[] vector2s = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++) {
                vector2s[i].x = vertices[i].x;
                vector2s[i].y = vertices[i].z;
                if (i != vertices.Length - 1) {
                    j = i + 1;
                }
                else {
                    j = 0;
                }

                var calc = Vector2.Distance(vector2s[i], vector2s[j]);
                if (calc < shorty) {
                    shorty = calc;
                }
            }

            // Method which calculates polygons triangluars
            Triangulator tr = new Triangulator(vector2s);
            int[] indices = tr.Triangulate();


            // Corrects the "Direction of Ceiling and Floor
            if (string.Equals(FloororCeiling, "Ceiling") && !swap) {
                System.Array.Reverse(indices);
            }
            else if (swap) {
                System.Array.Reverse(indices);
            }

            mesh.triangles = indices;
            mesh.uv = vector2s;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();


            if (material != null) {
                meshRenderer.material.CopyPropertiesFromMaterial(material);
                meshRenderer.material.SetTextureScale("_MainTex", new Vector2(0.5f, 0.5f));
                meshRenderer.material.name = material.name + "(Instance)";
            }
            else {
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

            float halfSize = model.Size.x / 2f;

            // North wall
            GameObject north = CreateWall(model.Size, model.Height, model.NorthMaterial);
            north.name = "NorthWall";
            north.transform.parent = root.transform;
            north.transform.position = new Vector3(-halfSize, 0, halfSize);
            // East wall
            GameObject east = CreateWall(model.Size, model.Height, model.EastMaterial);
            east.name = "EastWall";
            east.transform.parent = root.transform;
            east.transform.position = new Vector3(halfSize, 0, halfSize);
            east.transform.Rotate(Vector3.up, 90);
            // South wall
            GameObject south = CreateWall(model.Size, model.Height, model.SouthMaterial);
            south.name = "SouthWall";
            south.transform.parent = root.transform;
            south.transform.position = new Vector3(halfSize, 0, -halfSize);
            south.transform.Rotate(Vector3.up, 180);
            // West wall
            GameObject west = CreateWall(model.Size, model.Height, model.WestMaterial);
            west.name = "WestWall";
            west.transform.parent = root.transform;
            west.transform.position = new Vector3(-halfSize, 0, -halfSize);
            west.transform.Rotate(Vector3.up, 270);

            // Floor
            GameObject floorAnchor = new GameObject("FloorAnchor");
            floorAnchor.transform.parent = root.transform;

            GameObject floor = CreateWall(model.Size, model.Size, model.FloorMaterial);
            floor.name = "Floor";
            floor.transform.parent = floorAnchor.transform;
            // North Aligned
            floorAnchor.transform.position = new Vector3(-halfSize, 0, -halfSize);
            floorAnchor.transform.Rotate(Vector3.right, 90);
            // Ceiling
            GameObject ceilingAnchor = new GameObject("CeilingAnchor");
            ceilingAnchor.transform.parent = root.transform;
            GameObject ceiling = CreateWall(model.Size, model.Size, model.CeilingMaterial);
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

        public static IEnumerable<WaitForSeconds> WaitForAnchor(float s)
        {
            yield return new WaitForSeconds(s);
        }
        /**
         * Creates corridor model
         */
        public static GameObject CreateCorridor(CuboidCorridorModel model, DefaultNamespace.VREM.Model.Room[] connects)
        {
            GameObject root = new GameObject("Corridor ");

            Vector3[] wallCoordinates1;
            Vector3[] wallCoordinates2;
            Vector3[] floorCoordinates;
            Vector3[] ceilingCoordinates;
            int[] wallsToDestroy;
            
            //calculate which walls get removed and the coordinates of the corridor
            (wallCoordinates1, wallCoordinates2, floorCoordinates, ceilingCoordinates, wallsToDestroy) = CalculateCorridorCoordinates2(connects[0],connects[1]);
         
            
            //Destroy the walls 
            var wall1Des = "Wall" + wallsToDestroy[0];
            var wall2Des = "Wall" + wallsToDestroy[1];
            var anchorDes0 = "Anchor " + wallsToDestroy[0];
            var anchorDes1 = "Anchor " + wallsToDestroy[1];
            GameObject gogo = GameObject.Find(connects[0].text);
            GameObject.Destroy(gogo.transform.Find(wall1Des).gameObject);
            GameObject gogoA = GameObject.Find(connects[0].text);
            GameObject.Destroy(gogoA.transform.Find(anchorDes0).gameObject);
            GameObject gogo2 = GameObject.Find(connects[1].text);
            GameObject gogo2A = GameObject.Find(connects[1].text);
            GameObject.Destroy(gogo2.transform.Find(wall2Des).gameObject);
            GameObject.Destroy(gogo2A.transform.Find(anchorDes1).gameObject);

            GameObject north = CreateFreeWall(wallCoordinates1, model.NorthMaterial);
            north.name = "CorridorWall0";
            north.transform.parent = root.transform;
            GameObject south = CreateFreeWall(wallCoordinates2, model.SouthMaterial);
            south.name = "CorridorWall1";
            south.transform.parent = root.transform;

            GameObject floorAnchor = new GameObject("FloorAnchor");
            floorAnchor.transform.parent = root.transform;
            //GameObject floor = CreateFreeWall(floorCoordinates, model.FloorMaterial,true);
            GameObject floor = CreatePolygonalMeshes(floorCoordinates, model.FloorMaterial, "bla", false);
            floor.name = "Floor";
            floor.transform.parent = floorAnchor.transform;

            GameObject ceilingAnchor = new GameObject("CeilingAnchor");
            ceilingAnchor.transform.parent = root.transform;
            GameObject ceiling = CreateFreeWall(ceilingCoordinates, model.CeilingMaterial,true);
            ceiling.name = "Ceiling";
            ceiling.transform.parent = ceilingAnchor.transform;

            root.AddComponent<ModelContainer>().Model = model;
            return root;
        }

        private static GameObject CreateWall(Vector2 modelSize, float modelHeight, Material modelFloorMaterial)
        {
            throw new NotImplementedException();
        }

        private static GameObject CreateWall(Vector2 modelSize, Vector2 height, Material modelFloorMaterial)
        {
            throw new NotImplementedException();
        }

        /*
         * Method to create regular polygonaial rooms not used anymore.
         */
        public static GameObject CreateRegularPolygonalRoom(PolygonRoomModel model)
        {
            GameObject root = new GameObject("PolygonalRoom");

            // math: from size to radius
            //float rad = (float) (model.size / (2 * (Math.Sin((Math.PI / model.numberOfWalls)))));

            //var walls = model.GetWalls();
            List<GameObject> goWall = new List<GameObject>();
            for (int i = 0; i < model.numberOfWalls; i++) {
                String wallName = "Wall" + i;
            }

            for (int i = 0; i < model.numberOfWalls; i++) {
                goWall[i].name = i.ToString();
                goWall[i].transform.parent = root.transform;
                goWall[i].transform.Rotate(Vector3.up,
                    (((1f / model.numberOfWalls) * 360f * i) + ((1f / model.numberOfWalls) * 360f) / 2f));
            }

            // Ceiling
            GameObject ceilingAnchor = new GameObject("CeilingAnchor");
            ceilingAnchor.transform.parent = root.transform;
            ceilingAnchor.transform.position = new Vector3(0, model.height, 0);
            ceilingAnchor.transform.Rotate(Vector3.right, -90);

            // No idea why ceiling has to be rotated, only needs to be rotated if numberofwalls is odd
            if (model.numberOfWalls % 2 == 1) {
                ceilingAnchor.transform.Rotate(Vector3.back, ((1f / model.numberOfWalls) * 360f) / 2f);
            }

            // Floor
            GameObject floorAnchor = new GameObject("FloorAnchor");
            floorAnchor.transform.parent = root.transform;
            //
            //Not sure if 0 0 0 is right!!!!!! done with -halfsize for x and z in cuboid
            // pretty sure its wrong since wall coordinates are still all positive since not my logic
            //
            floorAnchor.transform.position = new Vector3(0, 0, 0);
            floorAnchor.transform.Rotate(Vector3.right, 90);
            root.transform.position = model.Position;

            root.AddComponent<ModelContainer>().Model = model;
            return root;
        }

        public static GameObject CreatePolygonalRoom(PolygonRoomModel model)
        {
            GameObject go = new GameObject("PolyRoom");
            //find vertices of ceiling and floor depends on the structure of the wallcoordinates
            var floorvertices = new System.Collections.Generic.List<Vector3>();
            var ceilingvertices = new System.Collections.Generic.List<Vector3>();
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
            var scaling = model.ceiling_scale;
            Vector3 midOfCeiling = new Vector3(0, 0, 0);
            for (int i = 0; i < ceilingvertices.Count; i++) {
                midOfCeiling += ceilingvertices[i];
            }

            Vector3 scalevector0;
            Vector3 scalevector1;
            Vector3 scalevector2;
            midOfCeiling = midOfCeiling / ceilingvertices.Count;
            //Debug.Log("mid   "+ midOfCeiling);
            for (int i = 0; i < model.walls.Length; i++) {
                scalevector0 = ceilingvertices[i] - midOfCeiling;
                scalevector1 = model.walls[i].wallCoordinates[2] - midOfCeiling;
                scalevector2 = model.walls[i].wallCoordinates[3] - midOfCeiling;

                ceilingvertices[i] = midOfCeiling + (scalevector1 * (float) scaling);

                model.walls[i].wallCoordinates[2].x = midOfCeiling.x + (scalevector1.x * (float) scaling);
                model.walls[i].wallCoordinates[2].z = midOfCeiling.z + (scalevector1.z * (float) scaling);
                model.walls[i].wallCoordinates[3].x = midOfCeiling.x + (scalevector2.x * (float) scaling);
                model.walls[i].wallCoordinates[3].z = midOfCeiling.z + (scalevector2.z * (float) scaling);
            }
            // convert list to array, since its needed as parameter
            var ceilingArray = ceilingvertices.ToArray();
            var floorArray = floorvertices.ToArray();


            // Floor
            GameObject floorAnchorTest = new GameObject("FloorAnchor");
            floorAnchorTest.transform.parent = go.transform;
            GameObject floortest =
                CreatePolygonalMeshes(floorArray, LoadMaterialByName(model.FloorMaterial), "Floor", false);
            floortest.name = "Floor";
            floortest.transform.parent = floorAnchorTest.transform;


            /*
             * checks if walls face right direction which is a matter of which direction the room was drawn
             * if room gets drawn counter clockwise walls would be visible outside the room
             * therefore it gets checked here and swapped if necessary
             * info: this approach is rather complicated and only practical correct.
             * idea: check if a point very close to a wall is "inside" the floor or not
             */
            Mesh m = floortest.GetComponentInChildren<MeshFilter>().mesh;
            Vector3 point = model.walls[0].wallCoordinates[0];
            Vector3 point2 = model.walls[0].wallCoordinates[1];
            Vector3 mid = (point + point2) / 2f;
            GameObject testWall = CreateFreeWall(model.walls[0].wallCoordinates, null);
            Vector3[] normals = testWall.GetComponentInChildren<MeshFilter>().mesh.normals;
            Object.Destroy(testWall);
            Vector3 avg = (normals[0] + normals[1]) / 2f;
            Vector3 inOut = mid + (avg * 0.1f);
            //Debug.Log("Point to test "+inOut);
            int[] tri = m.triangles;
            var vert = m.vertices;
            bool b = false;
            for (int i = 0; i < (tri.Length / 3); i++) {
                if (point_inside_trigon(inOut, vert[tri[i * 3]], vert[tri[i * 3 + 1]], vert[tri[i * 3 + 2]])) {
                    b = true;
                }
            }

            var turneCeilingFloor = "ceiling";
            /*
             * Swaps the coordinates of the walls
             * wallcoordinate[0] => wallcordinate[1] and wallcoordinate[2]->[3] and vice versa
             */
            if (b == false) {
                turneCeilingFloor = "floor";
                Debug.Log("Swapped Direction of Wall");
                for (int i = 0; i < model.walls.Length; i++) {
                    Vector3 swap = model.walls[i].wallCoordinates[0];
                    Vector3 swap2 = model.walls[i].wallCoordinates[2];
                    model.walls[i].wallCoordinates[0] = model.walls[i].wallCoordinates[1];
                    model.walls[i].wallCoordinates[2] = model.walls[i].wallCoordinates[3];
                    model.walls[i].wallCoordinates[1] = swap;
                    model.walls[i].wallCoordinates[3] = swap2;
                }
            }

            // Ceiling
            GameObject ceilingAnchor = new GameObject("CeilingAnchor");
            ceilingAnchor.transform.parent = go.transform;
            GameObject ceiling =
                CreatePolygonalMeshes(ceilingArray, LoadMaterialByName(model.CeilingMaterial), "Ceiling", b);
            ceiling.name = "Ceiling";
            ceiling.transform.parent = ceilingAnchor.transform;


            List<GameObject> goWall = new List<GameObject>();
            for (int i = 0; i < model.numberOfWalls; i++) {
                String wallName = "Wall" + i;
                goWall.Add(CreateFreeWall(model.walls[i].wallCoordinates, LoadMaterialByName(model.walls[i].texture)));
                goWall[i].name = wallName;
                goWall[i].transform.parent = go.transform;
            }

            go.transform.position = model.Position;
            go.AddComponent<ModelContainer>().Model = model;

            return go;
        }

        public static bool point_inside_trigon(Vector3 s, Vector3 a, Vector3 b, Vector3 c)
        {
            var as_x = s.x - a.x;
            var as_y = s.z - a.z;

            bool s_ab = (b.x - a.x) * as_y - (b.z - a.z) * as_x > 0;

            if ((c.x - a.x) * as_y - (c.z - a.z) * as_x > 0 == s_ab) return false;

            if ((c.x - b.x) * (s.z - b.z) - (c.z - b.z) * (s.x - b.x) > 0 != s_ab) return false;

            return true;
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
            if (!string.IsNullOrEmpty(materialName)) {
                if (!materialName.EndsWith("Material")) {
                    materialName = materialName + "Material";
                }

                return Resources.Load<Material>("Materials/" + materialName);
            }

            return null;
        }

        public static GameObject CreatePolygonalWall(int corners, float sizeRadius, Material mat)
        {
            GameObject go = new GameObject("PolygonalWall");
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
            Mesh mesh = meshFilter.mesh;

            // Math for polygon
            Vector3[] vertices = new Vector3[corners];
            for (int i = 1; i <= corners; i++) {
                vertices[i - 1] = new Vector3((float) (sizeRadius * Math.Sin((2 * Math.PI / corners) * i)),
                    (float) (sizeRadius * Math.Cos((2 * Math.PI / corners) * i)), 0);
            }

            mesh.vertices = vertices;

            // There are n-2 Trinagles in a polygon, every Trinagle defined by 3 vertices
            int numberOfTrinagles = corners - 2;
            int numberOfTrinaglePoints = (corners - 2) * 3;
            int[] tri = new int[numberOfTrinaglePoints];

            // trinangles
            for (int i = 0; i < numberOfTrinagles; i++) {
                int mult = i * 3;
                tri[mult] = 0;
                tri[mult + 1] = i + 1;
                tri[mult + 2] = i + 2;
            }

            mesh.triangles = tri;

            // normals
            Vector3[] normals = new Vector3[corners];
            for (int i = 0; i < corners; i++) {
                normals[i] = -Vector3.forward;
            }

            mesh.normals = normals;

            // texture coordinates = uv
            Vector2[] uv = new Vector2[corners];
            for (int i = 0; i < corners; i++) {
                uv[i] = new Vector2(vertices[i].x, vertices[i].y);
            }

            mesh.uv = uv;


            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            if (mat != null) {
                meshRenderer.material.CopyPropertiesFromMaterial(mat);
                //meshRenderer.material.SetTextureScale("_MainTex", new Vector2(1,1));
                meshRenderer.material.name = mat.name;
            }
            else {
                meshRenderer.material = new Material(Shader.Find("Standard"));
                meshRenderer.material.color = Color.white;
            }

            return go;
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

            if (width > height) {
                xUnit = width / height;
            }
            else {
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

            if (mat != null) {
                meshRenderer.material.CopyPropertiesFromMaterial(mat);
                //meshRenderer.material.SetTextureScale("_MainTex", new Vector2(1,1));
                meshRenderer.material.name = mat.name;
            }
            else {
                meshRenderer.material = new Material(Shader.Find("Standard"));
                meshRenderer.material.color = Color.white;
            }

            // TODO Highly experimental!

            //var boxCollider = go.AddComponent<BoxCollider>();
            //boxCollider.size = new Vector3(width,height,0.0001f);

            return go;
        }


        private static Vector3 CalculateUnit(Vector3 dimensions)
        {
            float m = Math.Max(Math.Max(dimensions.x, dimensions.y), dimensions.z);
            return new Vector3(m / dimensions.x, m / dimensions.y, m / dimensions.z);
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

            if (width > height) {
                xUnit = width / height;
            }
            else {
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
            if (cuboid.Material != null) {
                meshRenderer.material.CopyPropertiesFromMaterial(cuboid.Material);
            }
            else {
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

            Vector2 uvUnits = new Vector2(u, u);
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
            for (int i = 0; i < model.Size(); i++) {
                Vector3 pos = model.GetPositionAt(i);
                CuboidModel cuboid = model.GetCuboidAt(i);
                GameObject cub = CreateCuboid(cuboid);
                cub.transform.parent = root.transform;
                cub.transform.position = pos;
            }

            root.AddComponent<ModelContainer>().Model = model;
            return root;
        }
        
    /// <summary>
        /// calculate the coordinates of a corridor walls, floor and ceiling, depending on the two rooms to connect
        /// input two rooms
        /// output Vector3[] size 4 (needed for CreateFreeWall() )
        /// 
        /// 
        public static (Vector3[],Vector3[],Vector3[],Vector3[]) CalculateCorridorCoordinates(DefaultNamespace.VREM.Model.Room room0, DefaultNamespace.VREM.Model.Room room1)
        {
            List<DistanceAndCoordinate> distanceList = new List<DistanceAndCoordinate>();
            
            //is this the most efficient way?
            //find nearest corners in the rooms to connect
            foreach (DefaultNamespace.VREM.Model.Wall wall0 in room0.walls)
            {
                foreach (Vector3 wallCoord0 in wall0.wallCoordinates)
                {
                    if (wallCoord0.y.Equals(0))
                    {
                        foreach (DefaultNamespace.VREM.Model.Wall wall1 in room1.walls)
                        {
                            foreach (Vector3 wallCoord1 in wall1.wallCoordinates)
                            {
                                if (wallCoord1.y.Equals(0))
                                {
                                    var dist = Vector3.Distance(wallCoord0, wallCoord1);
                                    distanceList.Add(new DistanceAndCoordinate(wallCoord0, wallCoord1, dist));
                                }
                            }
                        }
                    }
                }
            }

            float lowestDist = distanceList.Min(dist => dist.distance);
            DistanceAndCoordinate lowestDistanceCoord = distanceList.Find(x => x.distance.Equals(lowestDist));
            distanceList.Remove(lowestDistanceCoord);

            //find the second connection with the second lowest distance with two different corners
            bool found = false;
            float secondlowestDist = distanceList.Min(dist => dist.distance);
            DistanceAndCoordinate secondLowestDistanceCoord = distanceList.Find(x => x.distance.Equals(secondlowestDist));
            while (!found)
            {
                secondlowestDist = distanceList.Min(dist => dist.distance);
                secondLowestDistanceCoord = distanceList.Find(x => x.distance.Equals(secondlowestDist));
                distanceList.Remove(secondLowestDistanceCoord);
                
                if (!HaveTheSameCorners(lowestDistanceCoord, secondLowestDistanceCoord))
                {
                    found = true;
                }
            }//end while
            Vector3[] wall_1= new Vector3[4];
            Vector3[] wall2= new Vector3[4];
            Vector3[] floor= new Vector3[4];
            Vector3[] ceiling_0= new Vector3[4];
            
            wall_1[0]=lowestDistanceCoord.wallCoord0;
            wall_1[1]=lowestDistanceCoord.wallCoord0;
            wall_1[1].y = 5;
            wall_1[2]=lowestDistanceCoord.wallCoord1;
            wall_1[3]=lowestDistanceCoord.wallCoord1;
            wall_1[3].y = 5;
            
            wall2[0]=secondLowestDistanceCoord.wallCoord0;
            wall2[1]=secondLowestDistanceCoord.wallCoord0;
            wall2[1].y = 5;
            wall2[2]=secondLowestDistanceCoord.wallCoord1;
            wall2[3]=secondLowestDistanceCoord.wallCoord1;                   
            wall2[3].y = 5;

            ceiling_0[0] = wall_1[0];
            ceiling_0[0] = wall_1[2];
            ceiling_0[0] = wall2[0];
            ceiling_0[0] = wall2[2];
            
            floor[0] = wall_1[1];
            floor[0] = wall_1[3];
            floor[0] = wall2[1];
            floor[0] = wall2[3];



            return (wall_1, wall2, ceiling_0, floor);
        }
    
    //check if two distance and corner pairs have the same corners
    //returns true if one corner has the same coordinates.
    private static bool HaveTheSameCorners(DistanceAndCoordinate dc0, DistanceAndCoordinate dc1)
    {
        return dc0.wallCoord0.Equals(dc1.wallCoord0) ||
               dc0.wallCoord0.Equals(dc1.wallCoord1) ||
               dc0.wallCoord1.Equals(dc1.wallCoord0) ||
               dc0.wallCoord1.Equals(dc1.wallCoord1);
    }
    
    public static (Vector3[],Vector3[],Vector3[],Vector3[],int[]) CalculateCorridorCoordinates2(DefaultNamespace.VREM.Model.Room room0, DefaultNamespace.VREM.Model.Room room1)
    {
        // ceiling scaling room0
        
        
        var ceilingvertices = new System.Collections.Generic.List<Vector3>();
        for (int i = 0; i < room0.walls.Length; i++) {
            if (ceilingvertices.Contains(room0.walls[i].wallCoordinates[2])) {
            }
            else {
                ceilingvertices.Add(room0.walls[i].wallCoordinates[2]);
            }
        }
        var ceilingArray = ceilingvertices.ToArray();
        // ceiling for direction
        GameObject floorAnchorTest = new GameObject("FloorAnchor");
        //floorAnchorTest.transform.parent = go.transform;
        GameObject floortest =
            CreatePolygonalMeshes(ceilingArray, LoadMaterialByName(room0.ceiling), "Floor", false);
        floortest.name = "ceilingTestingCorridor";
        floortest.transform.parent = floorAnchorTest.transform;
        
        Mesh m = floortest.GetComponentInChildren<MeshFilter>().mesh;
        Vector3 point = room0.walls[0].wallCoordinates[0];
        Vector3 point2 = room0.walls[0].wallCoordinates[1];
        Vector3 mid = (point + point2) / 2f;
        GameObject testWall = CreateFreeWall(room0.walls[0].wallCoordinates, null);
        Vector3[] normals = testWall.GetComponentInChildren<MeshFilter>().mesh.normals;
        Object.Destroy(testWall);
        Object.Destroy(floortest);
        Object.Destroy(floorAnchorTest);
        Vector3 avg = (normals[0] + normals[1]) / 2f;
        Vector3 inOut = mid + (avg * 0.1f);
        //Debug.Log("Point to test "+inOut);
        int[] tri = m.triangles;
        var vert = m.vertices;
        bool b = false;
        for (int i = 0; i < (tri.Length / 3); i++) {
            if (point_inside_trigon(inOut, vert[tri[i * 3]], vert[tri[i * 3 + 1]], vert[tri[i * 3 + 2]])) {
                b = true;
            }
        }
        if (b == true) {
            //turneCeilingFloor = "floor";
            Debug.Log("Swapped Direction of Wall in Corridor");
            for (int i = 0; i < room0.walls.Length; i++) {
                Vector3 swap = room0.walls[i].wallCoordinates[0];
                Vector3 swap2 = room0.walls[i].wallCoordinates[2];
                room0.walls[i].wallCoordinates[0] = room0.walls[i].wallCoordinates[1];
                room0.walls[i].wallCoordinates[2] = room0.walls[i].wallCoordinates[3];
                room0.walls[i].wallCoordinates[1] = swap;
                room0.walls[i].wallCoordinates[3] = swap2;
            }
        }
        
        
        
        
        //scaling room0
        var scaling = room0.ceiling_scale;
        Vector3 midOfCeiling = new Vector3(0, 0, 0);
        for (int i = 0; i < room0.walls.Length; i++) {
            midOfCeiling += ceilingvertices[i];
        }
        

        Vector3 scalevector0;
        Vector3 scalevector1;
        Vector3 scalevector2;
        midOfCeiling = midOfCeiling / ceilingvertices.Count;
        for (int i = 0; i < room0.walls.Length; i++) {
            scalevector0 = ceilingvertices[i] - midOfCeiling;
            scalevector1 = room0.walls[i].wallCoordinates[2] - midOfCeiling;
            scalevector2 = room0.walls[i].wallCoordinates[3] - midOfCeiling;

            ceilingvertices[i] = midOfCeiling + (scalevector1 * (float) scaling);

            room0.walls[i].wallCoordinates[2].x = midOfCeiling.x + (scalevector1.x * (float) scaling);
            room0.walls[i].wallCoordinates[2].z = midOfCeiling.z + (scalevector1.z * (float) scaling);
            room0.walls[i].wallCoordinates[3].x = midOfCeiling.x + (scalevector2.x * (float) scaling);
            room0.walls[i].wallCoordinates[3].z = midOfCeiling.z + (scalevector2.z * (float) scaling);
        }

        // ceiling scaling room1
        var ceilingvertices2 = new System.Collections.Generic.List<Vector3>();
        for (int i = 0; i < room1.walls.Length; i++) {
            if (ceilingvertices2.Contains(room1.walls[i].wallCoordinates[2])) {
            }
            else {
                ceilingvertices2.Add(room1.walls[i].wallCoordinates[2]);
            }
        }
         var ceilingArray2 = ceilingvertices.ToArray();
        // ceiling for direction
        GameObject floorAnchorTest2 = new GameObject("FloorAnchor");
        //floorAnchorTest.transform.parent = go.transform;
        GameObject floortest2 =
            CreatePolygonalMeshes(ceilingArray2, LoadMaterialByName(room1.ceiling), "Floor", false);
        floortest2.name = "ceilingTestingCorridor";
        floortest2.transform.parent = floorAnchorTest.transform;
        
        Mesh m2 = floortest.GetComponentInChildren<MeshFilter>().mesh;
        Vector3 point12 = room1.walls[0].wallCoordinates[0];
        Vector3 point22 = room1.walls[0].wallCoordinates[1];
        Vector3 mid2 = (point12 + point22) / 2f;
        GameObject testWall2 = CreateFreeWall(room1.walls[0].wallCoordinates, null);
        Vector3[] normals2 = testWall.GetComponentInChildren<MeshFilter>().mesh.normals;
        Object.Destroy(testWall2);
        Object.Destroy(floortest2);
        Object.Destroy(floorAnchorTest2);
        Vector3 avg2 = (normals[0] + normals[1]) / 2f;
        Vector3 inOut2 = mid2 + (avg2 * 0.1f);
        //Debug.Log("Point to test "+inOut);
        int[] tri2 = m2.triangles;
        var vert2 = m2.vertices;
        bool b2 = false;
        for (int i = 0; i < (tri2.Length / 3); i++) {
            if (point_inside_trigon(inOut2, vert2[tri2[i * 3]], vert2[tri2[i * 3 + 1]], vert2[tri2[i * 3 + 2]])) {
                b2 = true;
            }
        }
        if (b2 == true) {
            //turneCeilingFloor = "floor";
            Debug.Log("Swapped Direction of Wall in Corridor Room2");
            for (int i = 0; i < room1.walls.Length; i++) {
                Vector3 swap = room1.walls[i].wallCoordinates[0];
                Vector3 swap2 = room1.walls[i].wallCoordinates[2];
                room1.walls[i].wallCoordinates[0] = room1.walls[i].wallCoordinates[1];
                room1.walls[i].wallCoordinates[2] = room1.walls[i].wallCoordinates[3];
                room1.walls[i].wallCoordinates[1] = swap;
                room1.walls[i].wallCoordinates[3] = swap2;
            }
        }
        
        
        
        
        var scaling2 = room1.ceiling_scale;
        Vector3 midOfCeiling2 = new Vector3(0, 0, 0);
        for (int i = 0; i < room1.walls.Length; i++) {
            midOfCeiling2 += ceilingvertices2[i];
        }

        Vector3 scalevector02;
        Vector3 scalevector12;
        Vector3 scalevector22;
        midOfCeiling2 = midOfCeiling2 / ceilingvertices2.Count;
        
        for (int i = 0; i < room1.walls.Length; i++) {
            scalevector02 = ceilingvertices2[i] - midOfCeiling2;
            scalevector12 = room1.walls[i].wallCoordinates[2] - midOfCeiling2;
            scalevector22 = room1.walls[i].wallCoordinates[3] - midOfCeiling2;

            ceilingvertices2[i] = midOfCeiling2 + (scalevector12 * (float) scaling2);

            room1.walls[i].wallCoordinates[2].x = midOfCeiling2.x + (scalevector12.x * (float) scaling2);
            room1.walls[i].wallCoordinates[2].z = midOfCeiling2.z + (scalevector12.z * (float) scaling2);
            room1.walls[i].wallCoordinates[3].x = midOfCeiling2.x + (scalevector22.x * (float) scaling2);
            room1.walls[i].wallCoordinates[3].z = midOfCeiling2.z + (scalevector22.z * (float) scaling2);
        }
        

        DefaultNamespace.VREM.Model.Wall wall0;
        DefaultNamespace.VREM.Model.Wall wall1;
        
        int room0wall = 0;
        int room1wall = 0;
        Vector3 wall1cor0;
        Vector3 wall2cor0;
        var dist1 = Vector3.Distance(room0.walls[0].wallCoordinates[0]+room0.position, room1.walls[0].wallCoordinates[0]+room1.position);;
        
        // find the 2 closest points of 2 rooms save its wall;
        for (int i = 0; i < room0.walls.Length; i++) {
            for (int j = 0; j < room1.walls.Length; j++) {

                var a = Vector3.Distance(room0.walls[i].wallCoordinates[0]+room0.position, room1.walls[j].wallCoordinates[0]+room1.position);
                if (a< dist1) {
                    dist1 = a;
                    room0wall = i;
                    room1wall = j;
                }
                
            }
            
        }
       // wall1cor0=room0.walls[i].wallCoordinates
        
        
    // find the second closest point pair (1 of 2 neigbor points)
        var wall0before = room0wall-1;
        if (room0wall == 0) {
            wall0before = room0.walls.Length - 1;
        }

        var wall1before = room1wall-1;
        if (room1wall == 0) {
            wall1before = room1.walls.Length - 1;
        }
        /*
         *  Find the best combination, choose 4 of 6 points
         *     A            A*            A        C*
         *        \       /
         *  R0      B    B*    R1 or        B    B*
         *       /         \
         *     C            C*            C        A*
         */
        var Point_A = room0.walls[wall0before].wallCoordinates[0]+ room0.position;
        var Point_B = room0.walls[room0wall].wallCoordinates[0]+ room0.position;
        var Point_C = room0.walls[room0wall].wallCoordinates[1]+ room0.position;
        var Point_Astar = room1.walls[wall1before].wallCoordinates[0]+ room1.position;
        var Point_Bstar = room1.walls[room1wall].wallCoordinates[0]+ room1.position;
        var Point_Cstar = room1.walls[room1wall].wallCoordinates[1]+ room1.position;
        var Point_A_ceiling = room0.walls[wall0before].wallCoordinates[2]+ room0.position;
        var Point_B_ceiling = room0.walls[room0wall].wallCoordinates[2]+ room0.position;
        var Point_C_ceiling = room0.walls[room0wall].wallCoordinates[3]+ room0.position;
        var Point_Astar_ceiling = room1.walls[wall1before].wallCoordinates[2]+ room1.position;
        var Point_Bstar_ceiling = room1.walls[room1wall].wallCoordinates[2]+ room1.position;
        var Point_Cstar_ceiling = room1.walls[room1wall].wallCoordinates[3]+ room1.position;

        var AABB = Vector3.Distance(Point_A, Point_Astar) + Vector3.Distance(Point_B, Point_Bstar);
        var ABBC = Vector3.Distance(Point_A, Point_Bstar) + Vector3.Distance(Point_B, Point_Cstar);
        var BBCC = Vector3.Distance(Point_B, Point_Bstar) + Vector3.Distance(Point_C, Point_Cstar);
        var BACB = Vector3.Distance(Point_B, Point_Astar) + Vector3.Distance(Point_C, Point_Bstar);

        var ACBB = Vector3.Distance(Point_A, Point_Cstar) + Vector3.Distance(Point_B, Point_Bstar);
        var CABB = Vector3.Distance(Point_C, Point_Astar) + Vector3.Distance(Point_B, Point_Bstar);
        var ABBA =Vector3.Distance(Point_A, Point_Bstar) + Vector3.Distance(Point_B, Point_Astar);
        var BCCB =Vector3.Distance(Point_B, Point_Cstar) + Vector3.Distance(Point_C, Point_Bstar);            


        
        Debug.Log(AABB);
        Debug.Log(ABBC);
        Debug.Log(BBCC);
        Debug.Log(BACB);
        Debug.Log(ACBB);
        Debug.Log(CABB);
        Debug.Log(ABBA);
        Debug.Log(BCCB);
        
        
        if (AABB<=ABBC&&AABB<=BBCC&&AABB<=BACB&&AABB<=ACBB&&AABB<=CABB&&AABB<=ABBA&&AABB<=BCCB) 
        {
            Debug.Log("AABB");
            Vector3[] wall_1= new Vector3[]{Point_A,Point_Astar,Point_A_ceiling,Point_Astar_ceiling};
            Vector3[] wall2 = new Vector3[] {Point_Bstar,Point_B,Point_Bstar_ceiling,Point_B_ceiling};
            Vector3[] floor = new Vector3[] {Point_A, Point_B, Point_Astar, Point_Bstar};
            Vector3[] ceiling_0 = new Vector3[]
                {Point_B_ceiling,Point_A_ceiling,Point_Bstar_ceiling,  Point_Astar_ceiling};
            int[] destroy = {wall0before, wall1before};
            return (wall_1, wall2, floor, ceiling_0,destroy);


        }else if (ABBC<=AABB&&ABBC<=BBCC&&ABBC<=BACB&&AABB<=ACBB&&AABB<=CABB&&AABB<=ABBA&&AABB<=BCCB) {
            Debug.Log("ABBC");
            Vector3[] wall_1= new Vector3[]{Point_A,Point_Bstar,Point_A_ceiling,Point_Bstar_ceiling};
            Vector3[] wall2 = new Vector3[] {Point_Cstar,Point_B,Point_Cstar_ceiling,Point_B_ceiling};
            Vector3[] floor = new Vector3[] {Point_A, Point_B, Point_Bstar, Point_Cstar};
            Vector3[] ceiling_0 = new Vector3[]
                {Point_B_ceiling,Point_A_ceiling, Point_Cstar_ceiling, Point_Bstar_ceiling};
            int[] destroy = {wall0before, room1wall};
            return (wall_1, wall2, floor, ceiling_0,destroy);
        }
        else if (BBCC<=AABB&&BBCC<=ABBC&&BBCC<=BACB&&BBCC<=ACBB&&BBCC<=CABB&&BBCC<=ABBA&&BBCC<=BCCB) {
           
            Debug.Log("BBCC" );
            
            Vector3[] wall_1= new Vector3[]{Point_B,Point_Bstar,Point_B_ceiling,Point_Bstar_ceiling};
            Vector3[] wall2 = new Vector3[] {Point_Cstar,Point_C,Point_Cstar_ceiling,Point_C_ceiling};
            Vector3[] floor = new Vector3[] {Point_C,Point_B,Point_Bstar,Point_Cstar};
            Vector3[] ceiling_0 = new Vector3[]
                {Point_C_ceiling,Point_B_ceiling,Point_Cstar_ceiling,Point_Bstar_ceiling};
            int[] destroy = {room0wall, room1wall};
            return (wall_1, wall2, floor, ceiling_0,destroy);
        }
        else if (BACB<=AABB&&BACB<=ABBC&&BACB<=BBCC&&BACB<=ACBB&&BACB<=CABB&&BACB<=ABBA&&BACB<=BCCB) {
            
            Debug.Log("BACB");
            Vector3[] wall_1= new Vector3[]{Point_B,Point_Astar,Point_B_ceiling,Point_Astar_ceiling};
            Vector3[] wall2 = new Vector3[] {Point_Bstar,Point_C,Point_Bstar_ceiling,Point_C_ceiling};
            Vector3[] floor = new Vector3[] {Point_B, Point_C, Point_Astar, Point_Bstar};
            Vector3[] ceiling_0 = new Vector3[]
                {Point_C_ceiling,Point_B_ceiling, Point_Bstar_ceiling, Point_Astar_ceiling };
            int[] destroy = {room0wall, wall1before};
            return (wall_1, wall2, floor, ceiling_0,destroy);
        }
        else if (ACBB<=AABB&&ACBB<=ABBC&&ACBB<=BACB&&ACBB<=BBCC&&ACBB<=CABB&&ACBB<=ABBA&&ACBB<=BCCB) {
            Debug.Log("ACBB");
            
            Vector3[] wall_1= new Vector3[]{Point_A,Point_Cstar,Point_A_ceiling,Point_Cstar_ceiling};
            Vector3[] wall2 = new Vector3[] {Point_Bstar,Point_B,Point_Bstar_ceiling,Point_B_ceiling};
            Vector3[] floor = new Vector3[] {Point_A, Point_B,Point_Bstar, Point_Cstar};
            Vector3[] ceiling_0 = new Vector3[]
                {Point_B_ceiling,Point_A_ceiling,Point_Bstar_ceiling,  Point_Cstar_ceiling};
            int[] destroy = {wall0before, room1wall};
            return (wall_1, wall2, floor, ceiling_0,destroy);
            
            
        }
        else if (CABB <= AABB && CABB <= ABBC && CABB <= BACB && CABB <= BBCC && CABB <= ACBB && CABB <= ABBA &&
                 CABB <= BCCB) {
            Debug.Log("CABB ");
            
            Vector3[] wall_1= new Vector3[]{Point_Astar,Point_C,Point_Astar_ceiling,Point_C_ceiling};
            Vector3[] wall2 = new Vector3[] {Point_B,Point_Bstar,Point_B_ceiling,Point_Bstar_ceiling};
            Vector3[] floor = new Vector3[] {Point_C,Point_B,Point_Bstar, Point_Astar};
            Vector3[] ceiling_0 = new Vector3[]
                {Point_C_ceiling,Point_B_ceiling,  Point_Astar_ceiling,Point_Bstar_ceiling};
            int[] destroy = {room0wall, wall1before};
            return (wall_1, wall2, floor, ceiling_0,destroy);
        }
        else if (ABBA < AABB && ABBA < ABBC && ABBA < BACB && ABBA < BBCC && ABBA < ACBB && ABBA < CABB &&
                 ABBA < BCCB) {
            Debug.Log("ABBA");
            
            Vector3[] wall_1= new Vector3[]{Point_A,Point_Bstar,Point_A_ceiling,Point_Bstar_ceiling};
            Vector3[] wall2 = new Vector3[] {Point_Astar,Point_B,Point_Astar_ceiling,Point_B_ceiling};
            Vector3[] floor = new Vector3[] {Point_B,Point_A, Point_Bstar, Point_Astar};
            Vector3[] ceiling_0 = new Vector3[]
                {Point_B_ceiling,Point_A_ceiling,Point_Astar_ceiling,Point_Bstar_ceiling};
            int[] destroy = {wall0before, wall1before};
            return (wall_1, wall2, floor, ceiling_0,destroy);
        }
        else {
            //Debug.Log("BCCB");
            if (BCCB < AABB && BCCB < ABBC && BCCB < BACB && BCCB < BBCC && BCCB < ACBB && BCCB < CABB &&
                BCCB < ABBA) {
                Debug.Log("BCCB");
            }
            Vector3[] wall_1= new Vector3[]{Point_B,Point_Cstar,Point_B_ceiling,Point_Cstar_ceiling};
            Vector3[] wall2 = new Vector3[] {Point_Bstar,Point_C,Point_Bstar_ceiling,Point_C_ceiling};
            Vector3[] floor = new Vector3[] {Point_B,Point_C,Point_Cstar, Point_Bstar};
            
            Vector3[] ceiling_0 = new Vector3[]
                {Point_C_ceiling,Point_B_ceiling,Point_Bstar_ceiling,Point_Cstar_ceiling};
            
            
            int[] destroy = {room0wall, room1wall};
            return (wall_1, wall2, floor, ceiling_0,destroy);

        
            
        }

        
        
        

        var beforeafter = Vector3.Distance(room0.walls[wall0before].wallCoordinates[0] + room0.position,
            room1.walls[room1wall].wallCoordinates[0] + room1.position);
        var afterbefore=Vector3.Distance(room0.walls[room0wall].wallCoordinates[3] + room0.position,
            room1.walls[wall1before].wallCoordinates[3] + room1.position);
        
        var afterafter=Vector3.Distance(room0.walls[wall0before].wallCoordinates[0] + room0.position,
            room1.walls[wall1before].wallCoordinates[0] + room1.position);
        
        var beforebefore=Vector3.Distance(room0.walls[room0wall].wallCoordinates[0] + room0.position,
            room1.walls[room1wall].wallCoordinates[0] + room1.position);
        
        
        
// das stimmt nid was wenn .....es git nid nur die 2 möglichkeite
        if (beforeafter < afterbefore) {
            wall0 = room0.walls[wall0before];
            wall1 = room1.walls[room1wall];
        }
        else {
            wall0 = room0.walls[room0wall];
            wall1 = room1.walls[wall1before];
        }

        if (Vector3.Distance(wall0.wallCoordinates[0]+ room0.position, wall1.wallCoordinates[0]+ room1.position)+
            Vector3.Distance(wall0.wallCoordinates[1]+ room0.position, wall1.wallCoordinates[1]+ room1.position)<Vector3.Distance(wall0.wallCoordinates[0]+ room0.position, wall1.wallCoordinates[1]+ room1.position)+Vector3.Distance(wall0.wallCoordinates[1]+ room0.position, wall1.wallCoordinates[0]+ room1.position)) {
            Debug.Log(" here we need a swap");
            
            
            Vector3[] wall_1= new Vector3[]{wall0.wallCoordinates[0]+room0.position,wall1.wallCoordinates[0]+room1.position,wall0.wallCoordinates[2]+room0.position,wall1.wallCoordinates[2]+room1.position};
            Vector3[] wall2 = new Vector3[]
                {wall0.wallCoordinates[1]+room0.position, wall1.wallCoordinates[1]+room1.position, wall0.wallCoordinates[3]+room0.position, wall1.wallCoordinates[3]+room1.position};
            Vector3[] floor= new Vector3[]
                {wall0.wallCoordinates[1]+ room0.position, wall0.wallCoordinates[0]+ room0.position, wall1.wallCoordinates[0]+room1.position, wall1.wallCoordinates[1]+room1.position};
            Vector3[] ceiling_0=new Vector3[]
                {wall0.wallCoordinates[3]+room0.position, wall0.wallCoordinates[2]+room0.position, wall1.wallCoordinates[2]+room1.position, wall1.wallCoordinates[3]+room1.position};

            
           // return (wall_1, wall2, floor,ceiling_0);
            
        }
        else {





            Vector3[] wall_1 = new Vector3[]
            {
                wall0.wallCoordinates[0] + room0.position, wall1.wallCoordinates[1] + room1.position,
                wall0.wallCoordinates[2] + room0.position, wall1.wallCoordinates[3] + room1.position
            };
            Vector3[] wall2 = new Vector3[]
            {
                wall0.wallCoordinates[1] + room0.position, wall1.wallCoordinates[0] + room1.position,
                wall0.wallCoordinates[3] + room0.position, wall1.wallCoordinates[2] + room1.position
            };
            Vector3[] floor = new Vector3[]
            {
                wall0.wallCoordinates[1]+ room0.position, wall0.wallCoordinates[0]+ room0.position, wall1.wallCoordinates[1] + room1.position,
                wall1.wallCoordinates[0] + room1.position
            };
            Vector3[] ceiling_0 = new Vector3[]
            {
                wall0.wallCoordinates[3] + room0.position, wall0.wallCoordinates[2] + room0.position,
                wall1.wallCoordinates[3] + room1.position, wall1.wallCoordinates[2] + room1.position
            };

            //  System.Array.Reverse(ceiling_0);
            // System.Array.Reverse(wall_1);
            //System.Array.Reverse(floor);
        //    return (wall_1, wall2, floor, ceiling_0);
        }
    }
    
    
    //Local class to combine coordinates and distance 
    internal class DistanceAndCoordinate
    {
        public Vector3 wallCoord0;
        public Vector3 wallCoord1;
        public float distance;

        public DistanceAndCoordinate(Vector3 wallcoord0, Vector3 wallcoord1, float dist)
        {
            wallCoord0 = wallcoord0;
            wallCoord1 = wallcoord1;
            distance = dist;
        }
    }
        
    
    
    
    
    
    
    
    }

    /*
     * calculates triangles of polygons
     * http://wiki.unity3d.com/index.php/Triangulator
     */
    public class Triangulator
    {
        private List<Vector2> m_points = new List<Vector2>();

        public Triangulator(Vector2[] points)
        {
            m_points = new List<Vector2>(points);
        }

        public int[] Triangulate()
        {
            List<int> indices = new List<int>();

            int n = m_points.Count;
            if (n < 3)
                return indices.ToArray();

            int[] V = new int[n];
            if (Area() > 0) {
                for (int v = 0; v < n; v++)
                    V[v] = v;
            }
            else {
                for (int v = 0; v < n; v++)
                    V[v] = (n - 1) - v;
            }

            int nv = n;
            int count = 2 * nv;
            for (int v = nv - 1; nv > 2;) {
                if ((count--) <= 0)
                    return indices.ToArray();

                int u = v;
                if (nv <= u)
                    u = 0;
                v = u + 1;
                if (nv <= v)
                    v = 0;
                int w = v + 1;
                if (nv <= w)
                    w = 0;

                if (Snip(u, v, w, nv, V)) {
                    int a, b, c, s, t;
                    a = V[u];
                    b = V[v];
                    c = V[w];
                    indices.Add(a);
                    indices.Add(b);
                    indices.Add(c);
                    for (s = v, t = v + 1; t < nv; s++, t++)
                        V[s] = V[t];
                    nv--;
                    count = 2 * nv;
                }
            }

            indices.Reverse();
            return indices.ToArray();
        }

        private float Area()
        {
            int n = m_points.Count;
            float A = 0.0f;
            for (int p = n - 1, q = 0; q < n; p = q++) {
                Vector2 pval = m_points[p];
                Vector2 qval = m_points[q];
                A += pval.x * qval.y - qval.x * pval.y;
            }

            return (A * 0.5f);
        }

        private bool Snip(int u, int v, int w, int n, int[] V)
        {
            int p;
            Vector2 A = m_points[V[u]];
            Vector2 B = m_points[V[v]];
            Vector2 C = m_points[V[w]];
            if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
                return false;
            for (p = 0; p < n; p++) {
                if ((p == u) || (p == v) || (p == w))
                    continue;
                Vector2 P = m_points[V[p]];
                if (InsideTriangle(A, B, C, P))
                    return false;
            }

            return true;
        }

        private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
        {
            float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
            float cCROSSap, bCROSScp, aCROSSbp;

            ax = C.x - B.x;
            ay = C.y - B.y;
            bx = A.x - C.x;
            by = A.y - C.y;
            cx = B.x - A.x;
            cy = B.y - A.y;
            apx = P.x - A.x;
            apy = P.y - A.y;
            bpx = P.x - B.x;
            bpy = P.y - B.y;
            cpx = P.x - C.x;
            cpy = P.y - C.y;

            aCROSSbp = ax * bpy - ay * bpx;
            cCROSSap = cx * apy - cy * apx;
            bCROSScp = bx * cpy - by * cpx;

            return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
        }
    }
}