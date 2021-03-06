using System;
using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.VREM.Model;
using Unibas.DBIS.VREP.World;
using UnityEngine;
using World;

namespace Unibas.DBIS.VREP.Core
{
    public class ExhibitionManager
    {
        private Exhibition _exhibition;

        public ExhibitionManager(Exhibition exhibition)
        {
            this._exhibition = exhibition;
        }

        // is now made for polygonal rooms only
        
        //private List<CuboidExhibitionRoom> _rooms = new List<CuboidExhibitionRoom>();
        private List<CuboidExhibitionCorridor> _corridors = new List<CuboidExhibitionCorridor>();

        private List<PolygonalExhibitionRoom> _rooms = new List<PolygonalExhibitionRoom>();

        
        /*public CuboidExhibitionRoom GetRoomByIndex(int index)
        {
            return _rooms[index];
        }
        */
        public PolygonalExhibitionRoom GetRoomByIndex(int index)
        {
            return _rooms[index];
        }

        public CuboidExhibitionCorridor GetCorridorByIndex(int index)
        {
            return _corridors[index];
        }

        public void RestoreExhibits() {
            _rooms.ForEach(r => r.RestoreWallExhibits());
        }

        private int GetNextRoomIndex(int pos)
        {
            return (pos + 1) % _exhibition.rooms.Length;
        }

        private int GetPreviousRoomIndex(int pos)
        {
            return (pos - 1 + _exhibition.rooms.Length) % _exhibition.rooms.Length;
        }

        private int GetRoomIndex(DefaultNamespace.VREM.Model.Room room)
        {
            for (int i = 0; i < _exhibition.rooms.Length; i++)
            {
                if (room.Equals(_exhibition.rooms[i]))
                {
                    return i;
                }
            }

            return -1;
        }
        
        private int GetNextCorridorIndex(int pos)
        {
            return (pos + 1) % _exhibition.corridors.Length;
        }

        private int GetPreviousCorridorIndex(int pos)
        {
            return (pos - 1 + _exhibition.corridors.Length) % _exhibition.corridors.Length;
        }

        private int GetCorridorIndex(DefaultNamespace.VREM.Model.Corridor corridor)
        {
            for (int i = 0; i < _exhibition.corridors.Length; i++)
            {
                if (corridor.Equals(_exhibition.corridors[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        private DefaultNamespace.VREM.Model.Room GetNext(DefaultNamespace.VREM.Model.Room room)
        {
            var pos = GetRoomIndex(room);
            if (pos == -1)
            {
                // TODO This should not happen
                return null;
            }

            return _exhibition.rooms[GetNextRoomIndex(pos)];
        }

        private DefaultNamespace.VREM.Model.Room GetPrevious(DefaultNamespace.VREM.Model.Room room)
        {
            var pos = GetRoomIndex(room);
            if (pos == -1)
            {
                // TODO This should not happen
                return null;
            }

            return _exhibition.rooms[GetPreviousRoomIndex(pos)];
        }
        
        private DefaultNamespace.VREM.Model.Corridor GetNext(DefaultNamespace.VREM.Model.Corridor corridor)
        {
            var pos = GetCorridorIndex(corridor);
            if (pos == -1)
            {
                // TODO This should not happen
                return null;
            }

            return _exhibition.corridors[GetNextCorridorIndex(pos)];
        }

        private DefaultNamespace.VREM.Model.Corridor GetPrevious(DefaultNamespace.VREM.Model.Corridor corridor)
        {
            var pos = GetCorridorIndex(corridor);
            if (pos == -1)
            {
                // TODO This should not happen
                return null;
            }

            return _exhibition.corridors[GetPreviousCorridorIndex(pos)];
        }

        public void GenerateExhibition()
        {
            Debug.Log("ExhibitionManager GenerateExhibition");
            foreach (Corridor c in _exhibition.corridors)
            {
  //              Debug.Log(c);
//                Debug.Log(c.connects);//NULL
                //This is ok
//                Debug.Log(c.size);
//                Debug.Log(c.position);
//                Debug.Log(c.entrypoint);
            }
            //Debug.Log(_exhibition.corridors.);//NULL
            //This is ok
            
            
            foreach (var room in _exhibition.rooms)
            {
                var roomGameObject = ObjectFactory.BuildRoom(room);
                
                //var exhibitionRoom = roomGameObject.GetComponent<CuboidExhibitionRoom>();
                var exhibitionRoom = roomGameObject.GetComponent<PolygonalExhibitionRoom>();

                _rooms.Add(exhibitionRoom);

                /*
                if (VREPController.Instance.Settings.CeilingLogoEnabled)
                {
                    GameObject pref = Resources.Load<GameObject>("Objects/unibas");
                    var logo = GameObject.Instantiate(pref);
                    logo.name = "UnibasLogo";
                    logo.transform.SetParent(exhibitionRoom.transform, false);
                    //logo.transform.localPosition = new Vector3(-1.493f, room.size.y-.01f, -0.642f); // manually found values
                    logo.transform.localPosition =
                        new Vector3(-1.493f, room.size.y - .01f, 3.35f); // manually found values
                    logo.transform.localRotation = Quaternion.Euler(new Vector3(90, 180));
                    logo.transform.localScale = Vector3.one * 10000;
                }
                */
            }
            
            foreach (var corridor in _exhibition.corridors)
            {
                //corridor.CalculatePosition();
  //              Debug.Log("Generate Corridor corridor-----------------------");
//                Debug.Log(corridor.connects);//NULL
                
                var corridorGameObject = ObjectFactory.BuildCorridor(corridor);
                
                var exhibitionCorridor = corridorGameObject.GetComponent<CuboidExhibitionCorridor>();
                _corridors.Add(exhibitionCorridor);
                
                
                /*
                if (VREPController.Instance.Settings.CeilingLogoEnabled)
                {
                    GameObject pref = Resources.Load<GameObject>("Objects/unibas");
                    var logo = GameObject.Instantiate(pref);
                    logo.name = "UnibasLogo";
                    logo.transform.SetParent(exhibitionCorridor.transform, false);
                    //logo.transform.localPosition = new Vector3(-1.493f, room.size.y-.01f, -0.642f); // manually found values
                    logo.transform.localPosition =
                        new Vector3(-1.493f, corridor.size.y - .01f, 3.35f); // manually found values
                    logo.transform.localRotation = Quaternion.Euler(new Vector3(90, 180));
                    logo.transform.localScale = Vector3.one * 10000;
                }*/
            }

            // For teleporting, each room needs to be created.
            foreach (var room in _rooms)
            {
                CreateAndAttachTeleporters(room);
            }
        }


      
        
        private void CreateAndAttachTeleporters(CuboidExhibitionRoom room)
        {
            var index = GetRoomIndex(room.RoomData);
            var next = _rooms[GetNextRoomIndex(index)];
            var prev = _rooms[GetPreviousRoomIndex(index)];

            var nd = next.GetEntryPoint();
            var pd = prev.GetEntryPoint();

            var backPos = new Vector3(-.25f, 0, .2f);
            var nextPos = new Vector3(.25f, 0, .2f);

            // TODO Configurable TPBtnModel
            var model = new SteamVRTeleportButton.TeleportButtonModel(0.1f, .02f, 1f,
                TexturingUtility.LoadMaterialByName("none"),
                TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic"));

            if (_exhibition.rooms.Length > 1) {
                // back teleporter
                var backTpBtn = SteamVRTeleportButton.Create(room.gameObject, backPos, pd, model
                    ,
                    Resources.Load<Sprite>("Sprites/UI/chevron-left"));

                backTpBtn.OnTeleportStart = room.OnRoomLeave;
                backTpBtn.OnTeleportEnd = prev.OnRoomEnter;

                // back teleporter
                var nextTpBtn = SteamVRTeleportButton.Create(room.gameObject, nextPos, nd,
                    model,
                    Resources.Load<Sprite>("Sprites/UI/chevron-right"));

                nextTpBtn.OnTeleportStart = room.OnRoomLeave;
                nextTpBtn.OnTeleportEnd = next.OnRoomEnter;
            }
        }

        // same method for polys
        private void CreateAndAttachTeleporters(PolygonalExhibitionRoom room)
            {
         
                //Debug.Log("pos of go " +what.transform.position);
                
                var index = GetRoomIndex(room.RoomData);
                var next = _rooms[GetNextRoomIndex(index)];
                var prev = _rooms[GetPreviousRoomIndex(index)];
                var thisRoom = _rooms[index];
                //var backPos = next.GetPostionForTeleportButtons();
                //var nextPos = prev.GetPostionForTeleportButtons();
               // Debug.Log(pd);
               // Debug.Log(nd);

               
                var posmid = thisRoom.GetPositionForTeleportButtons();
                var backPos = new Vector3(posmid.x-.25f, 0, posmid.z-.2f);
                var nextPos = new Vector3(posmid.x+.25f, 0, posmid.z+.2f);
                var nd = next.GetEntryPoint();
                var pd = prev.GetEntryPoint();
//                Debug.Log("next  "+ nd + "  prev  "+pd);
                //var backPos = new Vector3(-.25f, 0, .2f);
                //var nextPos = new Vector3(.25f, 0, .2f);

                // TODO Configurable TPBtnModel
                var model = new SteamVRTeleportButton.TeleportButtonModel(0.1f, .02f, 1f,
                    TexturingUtility.LoadMaterialByName("none"),
                    TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic"));

                if (_exhibition.rooms.Length > 1)
                {
                    // back teleporter
                    var backTpBtn = SteamVRTeleportButton.Create(room.gameObject, backPos, pd, model
                        ,
                        Resources.Load<Sprite>("Sprites/UI/chevron-left"));

                    backTpBtn.OnTeleportStart = room.OnRoomLeave;
                    backTpBtn.OnTeleportEnd = prev.OnRoomEnter;

                    // back teleporter
                    var nextTpBtn = SteamVRTeleportButton.Create(room.gameObject, nextPos, nd,
                        model,
                        Resources.Load<Sprite>("Sprites/UI/chevron-right"));

                    nextTpBtn.OnTeleportStart = room.OnRoomLeave;
                    nextTpBtn.OnTeleportEnd = next.OnRoomEnter;
                }


            if (VREPController.Instance.Settings.StartInLobby)
            {
                var lobbyTpBtn = SteamVRTeleportButton.Create(room.gameObject, new Vector3(0, 0, .2f),
                    VREPController.Instance.LobbySpawn,
                    model,
                    "Lobby");
                lobbyTpBtn.OnTeleportStart = room.OnRoomLeave;
            }
        }
        
        /*
        private void CreateAndAttachTeleporters(CuboidCorridor corridor)
        {
            var index = GetCorridorIndex(corridor.CorridorData);
            var next = _corridors[GetNextCorridorIndex(index)];
            var prev = _corridors[GetPreviousCorridorIndex(index)];

            var nd = next.GetEntryPoint();
            var pd = prev.GetEntryPoint();

            var backPos = new Vector3(-.25f, 0, .2f);
            var nextPos = new Vector3(.25f, 0, .2f);

            // TODO Configurable TPBtnModel
            var model = new SteamVRTeleportButton.TeleportButtonModel(0.1f, .02f, 1f,
                TexturingUtility.LoadMaterialByName("none"),
                TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic"));

            if (_exhibition.corridors.Length > 1)
            {
                // back teleporter
                var backTpBtn = SteamVRTeleportButton.Create(corridor.gameObject, backPos, pd, model
                    ,
                    Resources.Load<Sprite>("Sprites/UI/chevron-left"));

                backTpBtn.OnTeleportStart = corridor.OnCorridorLeave;
                backTpBtn.OnTeleportEnd = prev.OnCorridorEnter;

                // back teleporter
                var nextTpBtn = SteamVRTeleportButton.Create(corridor.gameObject, nextPos, nd,
                    model,
                    Resources.Load<Sprite>("Sprites/UI/chevron-right"));

                nextTpBtn.OnTeleportStart = corridor.OnCorridorLeave;
                nextTpBtn.OnTeleportEnd = next.OnCorridorEnter;
            }


            if (VREPController.Instance.Settings.StartInLobby)
            {
                var lobbyTpBtn = SteamVRTeleportButton.Create(corridor.gameObject, new Vector3(0, 0, .2f),
                    VREPController.Instance.LobbySpawn,
                    model,
                    "Lobby");
                lobbyTpBtn.OnTeleportStart = corridor.OnCorridorLeave;
            }
        }
        */
    }
}