using System;
using System.Collections.Generic;
using DefaultNamespace;
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

        public static Vector3 CalculateRoomPosition(DefaultNamespace.VREM.Model.Room room)
        {
            // TODO exhibition-dependet calculation
            float x = room.position.x, y = room.position.y, z = room.position.z;
            var off = Settings.RoomOffset;
            return new Vector3(x * room.size.x + x * off, y * room.size.y + y * off, z * room.size.z + z * off);
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
            Material[] mats =
            {
                TexturingUtility.LoadMaterialByName(roomData.floor),
                TexturingUtility.LoadMaterialByName(roomData.ceiling),
                GetMaterialForWallOrientation(WallOrientation.NORTH, roomData),
                GetMaterialForWallOrientation(WallOrientation.EAST, roomData),
                GetMaterialForWallOrientation(WallOrientation.SOUTH, roomData),
                GetMaterialForWallOrientation(WallOrientation.WEST, roomData)
            };
            CuboidRoomModel modelData = new CuboidRoomModel(CalculateRoomPosition(roomData), roomData.size.x, roomData.size.y,
                mats[0], mats[1], mats[2], mats[3], mats[4], mats[5]);
            GameObject room = ModelFactory.CreateCuboidRoom(modelData);
            var er = room.AddComponent<CuboidExhibitionRoom>();
            er.RoomModel = modelData;
            er.Model = room;
            er.RoomData = roomData;
            var na = CreateAnchor(WallOrientation.NORTH, room, modelData);
            var ea = CreateAnchor(WallOrientation.EAST, room, modelData);
            var sa = CreateAnchor(WallOrientation.SOUTH, room, modelData);
            var wa = CreateAnchor(WallOrientation.WEST, room, modelData);

            var nw = CreateExhibitionWall(WallOrientation.NORTH, roomData, na);
            var ew = CreateExhibitionWall(WallOrientation.EAST, roomData, ea);
            var sw = CreateExhibitionWall(WallOrientation.SOUTH, roomData, sa);
            var ww = CreateExhibitionWall(WallOrientation.WEST, roomData, wa);
            
            er.Walls = new List<ExhibitionWall>(new []{nw,ew,sw,ww});
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
            l.transform.parent = room.transform;
            l.transform.localPosition = new Vector3(0, 2.5f, 0);
            room.name = "Room";

            GameObject teleportArea = new GameObject("TeleportArea");
            var col = teleportArea.AddComponent<BoxCollider>();
            col.size = new Vector3(modelData.Size, 0.01f, modelData.Size);
            teleportArea.AddComponent<MeshRenderer>();
            var tpa = teleportArea.AddComponent<TeleportArea>();
            tpa.transform.parent = room.transform;
            tpa.transform.localPosition = new Vector3(0, 0.01f, 0);

            return room;
        }
        
        //TODO
        public static GameObject BuildCorridor(DefaultNamespace.VREM.Model.Corridor corridorData)
        {
            Material[] mats =
            {
                TexturingUtility.LoadMaterialByName(corridorData.floor),
                TexturingUtility.LoadMaterialByName(corridorData.ceiling),
                GetMaterialForWallOrientation(WallOrientation.NORTH, corridorData),
                GetMaterialForWallOrientation(WallOrientation.SOUTH, corridorData)
            };
            
            CuboidCorridorModel modelData = new CuboidCorridorModel(CalculateCorridorPosition(corridorData), corridorData.size.x, corridorData.size.y,
                mats[0], mats[1], mats[2], mats[3]);
            GameObject corridor = ModelFactory.CreateCorridor(modelData);
            var er = corridor.AddComponent<CuboidExhibitionCorridor>();
            er.CorridorModel = modelData;
            er.Model = corridor;
            er.CorridorData = corridorData;
            var na = CreateAnchor(WallOrientation.NORTH, corridor, modelData);
            var sa = CreateAnchor(WallOrientation.SOUTH, corridor, modelData);

            var nw = CreateCorridorWall(WallOrientation.NORTH, corridorData, na);
            var sw = CreateCorridorWall(WallOrientation.SOUTH, corridorData, sa);

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

            GameObject teleportArea = new GameObject("TeleportArea");
            var col = teleportArea.AddComponent<BoxCollider>();
            col.size = new Vector3(modelData.Size, 0.01f, modelData.Size);
            teleportArea.AddComponent<MeshRenderer>();
            var tpa = teleportArea.AddComponent<TeleportArea>();
            tpa.transform.parent = corridor.transform;
            tpa.transform.localPosition = new Vector3(0, 0.01f, 0);

            return corridor;
        }

        private static ExhibitionWall CreateExhibitionWall(WallOrientation orientation, DefaultNamespace.VREM.Model.Room room, GameObject anchor)
        {
            var wall = anchor.AddComponent<ExhibitionWall>();
            wall.Anchor = anchor;
            wall.WallModel = null;
            wall.WallData = room.GetWall(orientation);
            return wall;
        }
        
        //TODO
        private static CorridorWall CreateCorridorWall(WallOrientation orientation,
            DefaultNamespace.VREM.Model.Corridor corridor, GameObject anchor)
        {
            var wall = anchor.AddComponent<CorridorWall>();
            wall.Anchor = anchor;
            wall.WallModel = null;
            wall.WallData = corridor.GetWall(orientation);
            return wall;
        }

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
        
        private static Material GetMaterialForWallOrientation(WallOrientation orientation,
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
        }
        
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
            var sizeHalf = model.Size / 2f;
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
            var sizeHalf = model.Size / 2f;
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