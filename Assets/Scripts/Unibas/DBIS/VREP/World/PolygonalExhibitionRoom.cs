using System;
using System.Collections.Generic;
using DefaultNamespace;
using Unibas.DBIS.DynamicModelling.Models;
using UnityEngine;
using World;

namespace Unibas.DBIS.VREP.World
{
    public class PolygonalExhibitionRoom:MonoBehaviour
    {

        public PolygonRoomModel roomModel { get; set; }

        
        public DefaultNamespace.VREM.Model.Room RoomData { get; set; }

        public GameObject Model { get; set; }
        
        public List<ExhibitionWall> Walls { get; set; }
 private AudioLoader _audioLoader;

        private void Start()
        {
            _audioLoader = GetComponent<AudioLoader>();
        }

        /// <summary>
        /// Populates this room.
        /// In other words: The walls will load their exhibits and this room will place its exhibits in its space.
        /// </summary>
        public void Populate()
        {
            PopulateRoom();
            PopulateWalls();
        }

        /// <summary>
        /// Handler for leaving this room.
        /// Shall be called whenever the player leaves this room
        /// </summary>
        public void OnRoomLeave()
        {
//            _audioLoader.Stop();
        }

        /// <summary>
        /// Handler for entering this room
        /// Shall be called whenever the player enters this room
        /// </summary>
        public void OnRoomEnter()
        {
  //          _audioLoader.Play();
        }

        /// <summary>
        /// Places the exhibits in this room's space.
        /// Currently not implemented.
        /// </summary>
        public void PopulateRoom()
        {
//            Debug.LogWarning("Cannot place 3d objects yet");
            /*
             * GameObject parent = new GameObject("Model Anchor");
		     * GameObject model = new GameObject("Model");
		     * model.transform.SetParent(parent.transform);
		     * parent.transform.position = pos;
		     * ObjLoader objLoader = model.AddComponent<ObjLoader>();
		     * model.transform.Rotate(-90,0,0);
		     * model.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		     * objLoader.Load(url);
             */
        }

        /// <summary>
        /// Induces the walls to place their exhibits.
        /// </summary>
        public void PopulateWalls()
        {
            Walls.ForEach(ew => ew.AttachExhibits());
        }

        /// <summary>
        /// Returns the wall for the orientation.
        /// </summary>
        /// <param name="orientation">The orientation for which the wall is requested</param>
        /// <returns>The ExhibitionWall component for the specified orientation</returns>
        /// 
        /*
        public ExhibitionWall GetWallForOrientation(WallOrientation orientation)
        {
            return Walls.Find(wall => wall.GetOrientation() == orientation);
        }
*/
        /// <summary>
        /// Loads the ambient audio of this room.
        /// As soon as the audio is loaded, it will be played.
        /// </summary>
        public void LoadAmbientAudio()
        {
            if (!string.IsNullOrEmpty(RoomData.GetURLEncodedAudioPath()))
            {
                Debug.Log("add audio to room");


                if (_audioLoader == null)
                {
                    _audioLoader = gameObject.AddComponent<AudioLoader>();
                }

                _audioLoader.ReloadAudio(RoomData.GetURLEncodedAudioPath());
            }
        }

        public Vector3 GetEntryPoint()
        {
            return transform.position + RoomData.entrypoint;
        }

        public Vector3 GetPositionForTeleportButtons()
        {
            
            List<Vector3> floorvertices=new System.Collections.Generic.List<Vector3>();
            for (int i = 0; i < roomModel.numberOfWalls; i++) {
                if (floorvertices.Contains(roomModel.walls[i].wallCoordinates[0])) {
                    //Debug.Log("Floor has same vertice multiple times");
                }
                else {
                    floorvertices.Add(roomModel.walls[i].wallCoordinates[0]);
                }
            }
            // convert list to array, since its needed as parameter
            var vertices=floorvertices.ToArray();
            
            
            Vector2[] vector2s=new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++) {
                vector2s[i].x = vertices[i].x;
                vector2s[i].y = vertices[i].z;
            }
            // Method which calculates polygons triangluars
            Triangulator tr = new Triangulator(vector2s);
            int[] triangles = tr.Triangulate();

            /*
            if (string.Equals(FloororCeiling, "Floor")) {
                System.Array.Reverse(indices);
            }
            */
            // Create the Vector3 vertices  ??
            Vector3[] verticesOfpoly = new Vector3[vector2s.Length];
            for (int i=0; i<vertices.Length; i++) {
                vertices[i] = new Vector3(vector2s[i].x, vector2s[i].y, 0);
            }
            

            
            //var pos = (meschOfFloor.vertices[meschOfFloor.triangles[0]] +meschOfFloor.vertices[meschOfFloor.triangles[1]] + meschOfFloor.vertices[meschOfFloor.triangles[2]])/3f;

            int iteration = 0;
            Vector3 pos = new Vector3();
            var posVar=new Vector3();
            var sizeF = AreaOfTriangle(vertices[triangles[0]],vertices[triangles[1]], vertices[triangles[2]]);
           
            for (int i = 0; i < triangles.Length/3f; i++) {
                //posVar = (vertices[triangles[0]] +vertices[triangles[1]] + vertices[triangles[2]])/3f;
                var sizeVar=AreaOfTriangle(vertices[triangles[i*3]],vertices[triangles[i*3+1]],vertices[triangles[i*3+2]]);
                if(sizeVar>sizeF) {
                    sizeF = sizeVar;
                    iteration = i;
                }
            }
            
            pos = (vertices[triangles[iteration*3]] +vertices[triangles[iteration*3+1]] + vertices[triangles[iteration*3+2]])/3f;

            
            
            
            return pos;
        }
        
        // method to calculate surface of triangles
        public double AreaOfTriangle(Vector3 pt1, Vector3 pt2, Vector3 pt3)
        {
            //d = (Math.Pow(x2 - x1)2 + (y2 - y1)2 + (z2 - z1)2)1/2
            double a = Math.Pow(Math.Pow((pt2.x - pt1.x), 2) + Math.Pow((pt2.y - pt1.y), 2) + Math.Pow((pt2.z - pt1.z), 2), (1 / 2f));      //pt1.DistanceTo(pt2);
            double b = Math.Pow(Math.Pow((pt2.x - pt3.x), 2) + Math.Pow((pt2.y - pt3.y), 2) + Math.Pow((pt2.z - pt3.z), 2), (1 / 2f));  
            //pt2.DistanceTo(pt3);
            double c =Math.Pow(Math.Pow((pt3.x - pt1.x), 2) + Math.Pow((pt3.y - pt1.y), 2) + Math.Pow((pt3.z - pt1.z), 2), (1 / 2f));  
            //pt3.DistanceTo(pt1);
            double s = (a + b + c) / 2;
            return Math.Sqrt(s * (s-a) * (s-b) * (s-c));
        }
        public class Triangulator
{
    private List<Vector2> m_points = new List<Vector2>();
 
    public Triangulator (Vector2[] points) {
        m_points = new List<Vector2>(points);
    }
 
    public int[] Triangulate() {
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
        for (int v = nv - 1; nv > 2; ) {
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
 
    private float Area () {
        int n = m_points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++) {
            Vector2 pval = m_points[p];
            Vector2 qval = m_points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }
 
    private bool Snip (int u, int v, int w, int n, int[] V) {
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
 
    private bool InsideTriangle (Vector2 A, Vector2 B, Vector2 C, Vector2 P) {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;
 
        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;
 
        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;
 
        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}
        

        public void RestoreWallExhibits() {
            Walls.ForEach(w => w.RestoreDisplayals());
        }
    }

    }
