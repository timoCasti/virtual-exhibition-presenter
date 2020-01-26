using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DefaultNamespace.VREM.Model;
using Unibas.DBIS.DynamicModelling;
using Unibas.DBIS.DynamicModelling.Models;
using Unibas.DBIS.VREP.World;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

namespace World
{
    /// <summary>
    /// Static class for object generation.
    /// </summary>
    public static class ObjectFactory
    {
        public static ExhibitionBuildingSettings Settings = ExhibitionBuildingSettings.Instance;

        public static GameObject GetDisplayalPrefab()
        {
            // TODO Use other prefabs
            /*if (Settings.UseStandardDisplayalPrefab)
            {*/
            var prefab = Resources.Load("Prefabs/"+Settings.StandardDisplayalPrefabName, typeof(GameObject)) as GameObject;
            if (prefab == null)
            {
                throw new Exception(string.Format("Could not load '{0}' as Resource", Settings.StandardDisplayalPrefabName));
            }
            
            return prefab;
            //}
        }
        
        
        // this method got simplified since we dont know the size of the room anymore
        public static Vector3 CalculateRoomPosition(DefaultNamespace.VREM.Model.Room room)
        {
            // TODO exhibition-dependet calculation
            float x = room.position.x, y = room.position.y, z = room.position.z;
            var off = Settings.RoomOffset;
            //return new Vector3(x * room.size.x + x * off, y * room.size.y + y * off, z * room.size.z + z * off);
            //return new Vector3(x * room.size.x + x * off, y * room.size.y + y * off, z * room.size.z + z * off);
            return new Vector3(x + x * off, y + y * off, z + z * off);
        }

        public static Vector3 CalculateCorridorPosition(DefaultNamespace.VREM.Model.Corridor corridor)
        {
            float x = corridor.position.x, y = corridor.position.y, z = corridor.position.z;

            return new Vector3()
            {
                x = x * corridor.size.x,
                y = y * corridor.size.y,
                z = z * corridor.size.z
            };
        }

        public static GameObject BuildRoom(DefaultNamespace.VREM.Model.Room roomData)
        {
            Material[] mats = new Material[roomData.walls.Length+2];
            Material[] matsWallonly = new Material[roomData.walls.Length];
            mats[0] = TexturingUtility.LoadMaterialByName(roomData.floor);
            mats[1] = TexturingUtility.LoadMaterialByName(roomData.ceiling);

            for (int i = 0; i < roomData.walls.Length; i++) {
                mats[2 + i] = TexturingUtility.LoadMaterialByName(roomData.walls[i].texture);
                matsWallonly[i] = TexturingUtility.LoadMaterialByName(roomData.walls[i].texture);
            }

            int numberOfWalls = roomData.walls.Length;

            float scaleCeiling = 1f;
            
            PolygonRoomModel poly=new PolygonRoomModel(roomData.position,numberOfWalls,roomData.ceiling_scale,roomData.height,roomData.floor,roomData.ceiling,roomData.walls);
            GameObject roompoly = ModelFactory.CreatePolygonalRoom(poly);
            var exRoom = roompoly.AddComponent<PolygonalExhibitionRoom>();
            
            exRoom.roomModel = poly;
            exRoom.Model = roompoly;
            exRoom.RoomData = roomData;
            

 /*           GameObject teleportArea = new GameObject("TeleportArea");
            var col = teleportArea.AddComponent<BoxCollider>();
            col.size = new Vector3(modelData.GetSize(), 0.01f, modelData.GetSize());
            teleportArea.AddComponent<MeshRenderer>();
            var tpa = teleportArea.AddComponent<TeleportArea>();
            tpa.transform.parent = room.transform;
            tpa.transform.localPosition = new Vector3(0, 0.01f, 0);*/

            GameObject[] anchors = new GameObject[numberOfWalls];

            ExhibitionWall[] exhibitionWalls=new ExhibitionWall[numberOfWalls];
            
            
            for (int i = 0; i < numberOfWalls; i++) {
                anchors[i] = CreateAnchorFreePoly(i, roompoly, poly);
                exhibitionWalls[i] = CreateExhibitionWall(i, roomData, anchors[i]);
            }


            exRoom.Walls = exhibitionWalls.ToList();
            exRoom.Populate();
            
            // calculate Light positions based on trianlges of floor
            var v = roompoly.GetComponentsInChildren<MeshFilter>();
            Mesh fm = null;
            for (int i = 0; i < v.Length; i++) {
                if (string.Equals(v[i].name , "Floor")) {
                    fm = v[i].mesh;
                }

            }
            var tri = fm.triangles;
            var vertices = fm.vertices;

            for (int i = 0; i < tri.Length/3; i++) {
                GameObject lPoly = new GameObject("RoomLight "+i);
                var lipo = lPoly.AddComponent<Light>();
                lipo.type = LightType.Point;
                lipo.range = 8;
                lipo.color = Color.white;
                lipo.intensity = 0.75f;
                lipo.renderMode = LightRenderMode.ForcePixel;
                //calculate middle of triangle
                Vector3 mid= (vertices[tri[i*3]]+vertices[tri[i*3+1]]+vertices[tri[i*3+2]])/3f;
                lipo.transform.parent = roompoly.transform;
                lipo.transform.localPosition = new Vector3(mid.x, 2.5f, mid.z);
                
            }
          
            if (roomData.text != null) {
                roompoly.name = roomData.text;
            }
            else {
                roompoly.name = "Room";
            }

            GameObject teleportAreaPoly = new GameObject("TeleportArea");
            //var colPoly = teleportAreaPoly.AddComponent<BoxCollider>();
            //colPoly.size = new Vector3(10, 0.01f, 10); // needs to be changed
            var colM = teleportAreaPoly.AddComponent<MeshCollider>();
            colM.sharedMesh = fm; //transform.position=fm.
            teleportAreaPoly.AddComponent<MeshRenderer>();
            var tpaPoly = teleportAreaPoly.AddComponent<TeleportArea>();
            tpaPoly.transform.parent = roompoly.transform;
            tpaPoly.transform.localPosition = Vector3.zero;
            
            return roompoly;
        }
        
        //TODO
        public static GameObject BuildCorridor(DefaultNamespace.VREM.Model.Corridor corridorData)
        {
        
            Material[] mats = new Material[corridorData.walls.Length+2];
            Material[] matsWallonly = new Material[corridorData.walls.Length];
            mats[0] = TexturingUtility.LoadMaterialByName(corridorData.floor);
            mats[1] = TexturingUtility.LoadMaterialByName(corridorData.ceiling);
            
            corridorData.CalculatePosition();
            
            //calculate Position before loop!
            for (int i = 0; i < corridorData.walls.Length; i++) {
                mats[2 + i] = TexturingUtility.LoadMaterialByName(corridorData.walls[i].texture);
                matsWallonly[i] = TexturingUtility.LoadMaterialByName(corridorData.walls[i].texture);
            }

            
            CuboidCorridorModel cuboidCorridorModelData = new CuboidCorridorModel(CalculateCorridorPosition(corridorData),
                corridorData.size.x, corridorData.size.y,
                mats[0], mats[1], mats[2], mats[3], corridorData.walls);
            
            GameObject corridor = ModelFactory.CreateCorridor(cuboidCorridorModelData,corridorData.connects);
            
            var er = corridor.AddComponent<CuboidExhibitionCorridor>();

            er.CorridorModel = cuboidCorridorModelData;

           
            er.Model = corridor;
            er.CorridorData = corridorData;
      
            var na = CreateAnchorCorridors(0, corridor, cuboidCorridorModelData, corridorData.GetWall(0));
            var sa = CreateAnchorCorridors(1, corridor, cuboidCorridorModelData, corridorData.GetWall(1));
            
            var nw = CreateCorridorWall(0, corridorData, na);
            var sw = CreateCorridorWall(1, corridorData, sa);

            er.Walls = new List<CorridorWall>(new []{nw,sw});
            er.Populate();
            
            GameObject light = new GameObject("RoomLight");
            var l = light.AddComponent<Light>();
            l.type = LightType.Point;
            l.range = 8;
            l.color = Color.white;
            l.intensity = 1.5f;
            l.renderMode = LightRenderMode.ForcePixel;
            //l.lightmapBakeType = LightmapBakeType.Mixed; // Build fails with this line uncommented, even though unity automatically upgrades to this one.
            //l.lightmappingMode = LightmappingMode.Mixed; // Build fails with this line uncommented. it is obsolete
            // Results in mode Realtime (in Unity editor inspector)
            l.transform.parent = corridor.transform;
            l.transform.localPosition = new Vector3(0, 2.5f, 0);
            corridor.name = "Corridor";
            //todo lock
            GameObject teleportArea = new GameObject("TeleportArea");
            var col = teleportArea.AddComponent<BoxCollider>();
            col.size = new Vector3(cuboidCorridorModelData.GetSize(), 0.01f, cuboidCorridorModelData.GetSize());
            teleportArea.AddComponent<MeshRenderer>();
            var tpa = teleportArea.AddComponent<TeleportArea>();
            tpa.transform.parent = corridor.transform;
            tpa.transform.localPosition = new Vector3(0, 0.01f, 0);

            return corridor;
        }

        private static ExhibitionWall CreateExhibitionWall(int orientation, DefaultNamespace.VREM.Model.Room room, GameObject anchor)
        {
            var wall = anchor.AddComponent<ExhibitionWall>();
            wall.Anchor = anchor;
            wall.WallModel = null;
            wall.WallData = room.walls[orientation];
            return wall;
        }
        
        //TODO
        private static CorridorWall CreateCorridorWall(int wallnumber,
            DefaultNamespace.VREM.Model.Corridor corridor, GameObject anchor)
        {
            var wall = anchor.AddComponent<CorridorWall>();
            wall.Anchor = anchor;
            wall.WallModel = null;
            wall.WallData = corridor.GetWall(wallnumber);
            return wall;
        }

        //not used for polygon rooms
        /*
        private static Material GetMaterialForWallOrientation(WallOrientation orientation,
            DefaultNamespace.VREM.Model.Room roomData)
        {
            foreach (DefaultNamespace.VREM.Model.Wall wallData in roomData.walls)
            {
                WallOrientation wor = (WallOrientation) Enum.Parse(typeof(WallOrientation), wallData.direction, true);
                if (wor.Equals(orientation))
                {
                    Debug.Log("Material "+wallData.texture+" for room " + roomData.position);
                    return TexturingUtility.LoadMaterialByName(wallData.texture, true);
                }
            }

            throw new ArgumentException("Couldn't find material for orientation " + orientation + " in room at " +
                                        roomData.position);
        }
        */
       /* private static Material GetMaterialForWallOrientation(WallOrientation orientation,
            DefaultNamespace.VREM.Model.Corridor corridorData)
        {
            foreach (DefaultNamespace.VREM.Model.Wall wallData in corridorData.walls)
            {
                WallOrientation wor = (WallOrientation) Enum.Parse(typeof(WallOrientation), wallData.direction, true);
                if (wor.Equals(orientation))
                {
                    Debug.Log("Material "+wallData.texture+" for corridor " + corridorData.position);
                    return TexturingUtility.LoadMaterialByName(wallData.texture, true);
                }
            }

            throw new ArgumentException("Couldn't find material for orientation " + orientation + " in corridor at " +
                                        corridorData.position);
        }*/
        
        /// <summary>
        /// Anchor for room
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="room"></param>
        /// <param name="model"></param>
        /// <returns>GameObject Anchor</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static GameObject CreateAnchor(WallOrientation orientation, GameObject room, CuboidRoomModel model)
        {
            GameObject anchor = new GameObject(orientation + "Anchor");
            anchor.transform.parent = room.transform;
            Vector3 pos = Vector3.zero;
            var a = 0f;
            var sizeHalf = model.GetSize() / 2f;
            switch (orientation)
            {
                case WallOrientation.NORTH:
                    pos = new Vector3(-sizeHalf, 0, sizeHalf);
                    a = 0;
                    break;
                case WallOrientation.EAST:
                    pos = new Vector3(sizeHalf, 0, sizeHalf);
                    a = 90;
                    break;
                case WallOrientation.SOUTH:
                    pos = new Vector3(sizeHalf, 0, -sizeHalf);
                    a = 180;
                    break;
                case WallOrientation.WEST:
                    pos = new Vector3(-sizeHalf, 0, -sizeHalf);
                    a = 270;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("orientation", orientation, null);
            }
            anchor.transform.Rotate(Vector3.up, a);
            anchor.transform.localPosition = pos;
            return anchor;
        }
        
        /// <summary>
        /// Anchor for corridor
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="corridor"></param>
        /// <param name="model"></param>
        /// <returns>GameObject Anchor</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static GameObject CreateAnchor(WallOrientation orientation, GameObject corridor, CuboidCorridorModel model)
        {
            GameObject anchor = new GameObject(orientation + "Anchor");
            anchor.transform.parent = corridor.transform;
            Vector3 pos = Vector3.zero;
            
            var a = 0f;
            var sizeHalf = model.GetSize() / 2f;
            switch (orientation)
            {
                case WallOrientation.NORTH:
                    pos = new Vector3(-sizeHalf, 0, sizeHalf);
                    a = 0;
                    break;
                case WallOrientation.SOUTH:
                    pos = new Vector3(sizeHalf, 0, -sizeHalf);
                    a = 180;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("orientation", orientation, null);
            }
            anchor.transform.Rotate(Vector3.up, a);
            anchor.transform.localPosition = pos;
            return anchor;
        }
        
        //creates anchor for corridors
        private static GameObject CreateAnchorCorridors(/*WallOrientation orientation,*/
            int Wallnumber, GameObject corridor, CuboidCorridorModel model, DefaultNamespace.VREM.Model.Wall wall)
        {
            //todo walorientation 0 1
//            Debug.Log("ObjectFactory CreateAnchorCorridors");
            GameObject anchor = new GameObject(Wallnumber + "Anchor");
            anchor.transform.parent = corridor.transform;
            //Vector3 pos = Vector3.zero;
            //Vector3 pos = corridor;
            //Vector3 pos; // = wall.wallCoordinates[0];
            //Vector3 pos = model.walls[Wallnumber].wallCoordinates[0];
            Vector3 pos = model.walls[Wallnumber].wallCoordinates[0];
            var a = 0f;
            var sizeHalf = model.GetSize() / 2f;
            /*
            switch (orientation)
            {
                case WallOrientation.NORTH:
                    pos = new Vector3(-sizeHalf, 0, sizeHalf);
                    a = 0;
                    break;
                case WallOrientation.SOUTH:
                    pos = new Vector3(sizeHalf, 0, -sizeHalf);
                    a = 180;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("orientation", orientation, null);
            }*/
            string wallname = "CorridorWall" + Wallnumber;
            GameObject gogo = GameObject.Find(wallname);
            Vector3 nor = gogo.GetComponentInChildren<MeshFilter>().mesh.normals[0];
            Vector3 vec = Quaternion.FromToRotation(Vector3.back, nor).eulerAngles;
            anchor.transform.Rotate(Vector3.up,vec.y);
            //anchor.transform.localPosition = pos;
            anchor.transform.position = gogo.GetComponentInChildren<MeshFilter>().mesh.vertices[0];
//            Debug.Log(anchor.ToString());
            return anchor;
            
            Debug.Log(a.ToString());
            anchor.transform.Rotate(Vector3.up, a);
            anchor.transform.localPosition = pos;
            return anchor;
        }
        
        //creates anchors for regular polgonal rooms
        private static GameObject CreateAnchorPoly(int Wallnumber, GameObject room, PolygonRoomModel model)
        {
            GameObject anchor = new GameObject(Wallnumber + " Anchor ");
            anchor.transform.parent = room.transform;
            Vector3 pos = Vector3.zero;
            
            //float rad = (float) (model.size / (2 * (Math.Sin((Math.PI / model.numberOfWalls)))));
            
            var a = 0f;
            //var sizeHalf = model.size / 2f;
            
            //pos=new Vector3((float) (rad * Math.Sin((2 * Math.PI / model.numberOfWalls) * Wallnumber)),0,(float) (rad * Math.Cos((2 * Math.PI / model.numberOfWalls) * Wallnumber)));
            
            //a = (((model.numberOfWalls - 2) * 180 / model.numberOfWalls)*Wallnumber)+(((model.numberOfWalls - 2) * 180 / model.numberOfWalls)/2);
            a = (((1f / model.numberOfWalls) * 360f * Wallnumber) + ((1f / model.numberOfWalls) * 360f) / 2f);
            
            anchor.transform.Rotate(Vector3.up, a);
            anchor.transform.localPosition = pos;
            return anchor;
        }
        
        private static GameObject CreateAnchorFreePoly(int Wallnumber, GameObject room, PolygonRoomModel model)
        {
            GameObject anchor = new GameObject("Anchor " + Wallnumber);
            anchor.transform.parent = room.transform;
            Vector3 pos = Vector3.zero;
            
            
            var a = 0f;

            pos = model.walls[Wallnumber].wallCoordinates[0];

            int wall2 = (Wallnumber + 1);
            if (wall2 == model.walls.Length)
            {
                wall2 = 0;
            }
            Vector3 v = new Vector3(model.walls[Wallnumber].wallCoordinates[0].x-model.walls[Wallnumber].wallCoordinates[1].x,0,model.walls[Wallnumber].wallCoordinates[0].y-model.walls[Wallnumber].wallCoordinates[1].y);
            Vector3 v2 = new Vector3(model.walls[wall2].wallCoordinates[0].x - model.walls[wall2].wallCoordinates[1].x,0,model.walls[wall2].wallCoordinates[0].y-model.walls[wall2].wallCoordinates[1].y);


            float angleTry = Vector3.Angle(v, v2);
            a = Vector3.Angle(model.walls[Wallnumber].wallCoordinates[0] - model.walls[Wallnumber].wallCoordinates[1], Vector3.right);
            
            // get the normal of the Mesh of the wall to adjust the angle of the anchor (Mesh gets created agein since its hard to receive the right one)
            string wallname = "Wall" + Wallnumber;
            

            room.GetComponentsInChildren<MeshFilter>();
            var coordinates=model.walls[Wallnumber].wallCoordinates;
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

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            var pos2 = meshFilter.transform.position;
            
            //nor old
            //Vector3 nor = gogo.GetComponentInChildren<MeshFilter>().mesh.normals[0];
            Vector3 nor = mesh.normals[0];
            GameObject.DestroyImmediate(go);
            //var ro = gogo.GetComponentInChildren<MeshFilter>().transform.rotation;
            Vector3 vec1 = Quaternion.FromToRotation(Vector3.back, nor).eulerAngles;
            var vec2 = Quaternion.FromToRotation(Vector3.back, nor);
            anchor.transform.Rotate(Vector3.up,vec1.y);
            anchor.transform.Rotate(Vector3.right,vec1.x);
            //anchor.transform.Rotate(Vector3.back,vec1.z);
            
//            Debug.Log("postion2  "+ pos2);
  //          Debug.Log("postion  "+ pos);
            anchor.transform.localPosition = pos;

            return anchor;
        }
        
        private static ComplexCuboidModel GenerateButtonModel(float size, float border, float height)
        {
            ComplexCuboidModel model = new ComplexCuboidModel();
            // TODO Add material somehow
            model.Add(Vector3.zero, new CuboidModel(size, size, height));
            model.Add(new Vector3(border, border, -height), new CuboidModel(size - 2 * border, size - 2 * border, height));
            return model;
        }

        public static GameObject CreateTeleportButtonModel(Vector3 position, Vector3 destination, float size, float border)
        {
            var modelData = GenerateButtonModel(size, border, border / 2f);
            GameObject buttonObj = ModelFactory.CreateModel(modelData);
            PlayerTeleporter tpBtn = buttonObj.AddComponent<PlayerTeleporter>();
            tpBtn.Destination = destination;
            BoxCollider col = buttonObj.AddComponent<BoxCollider>();
            col.size = new Vector3(size,size,border*2);
            col.center = new Vector3(size/2f, size/2f, -border);
            buttonObj.AddComponent<Button>();
            var hand = new CustomEvents.UnityEventHand();
            hand.AddListener(h =>
            {
                tpBtn.TeleportPlayer();
            });
            buttonObj.AddComponent<UIElement>().onHandClick = hand;
            buttonObj.transform.position = position;
            buttonObj.name = "TeleportButton (Instance)";
            return buttonObj;
        }
        
        
        
        /// <summary>
        /// TODO Fix rotations!
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Vector3 CalculateRotation(WallOrientation orientation) {
            switch (orientation) {
                case WallOrientation.NORTH:
                    return new Vector3(90, 180, 0);
                case WallOrientation.EAST:
                    return new Vector3(90, 0, 0);
                case WallOrientation.SOUTH:
                    return new Vector3(90, -180, 0);
                case WallOrientation.WEST:
                    return new Vector3(90, 90, 0);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Vector3 CalculateRotation(string orientation)
        {
            return CalculateRotation((WallOrientation) Enum.Parse(typeof(WallOrientation), orientation, true));
        }
    }
}