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
            Debug.LogWarning("Cannot place 3d objects yet");
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

        public Vector3 GetPostionForTeleportButtons()
        {
            var roomGo=GameObject.Find(this.RoomData.text);
            Mesh meschOfFloor = null;

            var v = roomGo.GetComponentsInChildren<MeshFilter>();
            for (int i = 0; i < v.Length; i++)
            {
                if (string.Equals(v[i].name, "Floor")) ;
                {
                    meschOfFloor = v[i].mesh;
                }
            }

            
            //var pos = (meschOfFloor.vertices[meschOfFloor.triangles[0]] +meschOfFloor.vertices[meschOfFloor.triangles[1]] + meschOfFloor.vertices[meschOfFloor.triangles[2]])/3f;

            int iteration = 0;
            Vector3 pos = new Vector3();
            var posVar=new Vector3();
            var sizeF = AreaOfTriangle(meschOfFloor.vertices[meschOfFloor.triangles[0]],meschOfFloor.vertices[meschOfFloor.triangles[1]], meschOfFloor.vertices[meschOfFloor.triangles[2]]);
            for (int i = 0; i < meschOfFloor.vertices.Length; i++) {
                posVar = (meschOfFloor.vertices[meschOfFloor.triangles[0]] +meschOfFloor.vertices[meschOfFloor.triangles[1]] + meschOfFloor.vertices[meschOfFloor.triangles[2]])/3f;
                var sizeVar=AreaOfTriangle(meschOfFloor.vertices[meschOfFloor.triangles[i*3]],meschOfFloor.vertices[meschOfFloor.triangles[i*3+1]],meschOfFloor.vertices[meschOfFloor.triangles[i*3+2]]);
                if(sizeVar>sizeF) {
                    sizeF = sizeVar;
                    iteration = i;
                }
            }
            
            pos = (meschOfFloor.vertices[meschOfFloor.triangles[iteration*3]] +meschOfFloor.vertices[meschOfFloor.triangles[iteration*3+1]] + meschOfFloor.vertices[meschOfFloor.triangles[iteration*3+2]])/3f;

            
            
            
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
        

        public void RestoreWallExhibits() {
            Walls.ForEach(w => w.RestoreDisplayals());
        }
    }

    }
