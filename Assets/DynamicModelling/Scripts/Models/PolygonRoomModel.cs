using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unibas.DBIS.DynamicModelling.Models
{
    [System.Serializable]
    public class PolygonRoomModel : IModel
    {
        public Vector3 Position;

        public double ceiling_scale;
        
        //private List<DefaultNamespace.VREM.Model.Wall> _walls;

        public DefaultNamespace.VREM.Model.Wall[] walls;
        
        public int numberOfWalls;
        
        //public float size;
        public float height;

        //public Material[] materials;
        //private List<WallModel> _walls;
        public string FloorMaterial;
        public string CeilingMaterial;

        
        public PolygonRoomModel(Vector3 position, int NumberOfWalls, float Height)
        {
            Position = position;
            numberOfWalls = NumberOfWalls;
           // size = Size;
            height = Height;
           // FloorMaterial = floorMaterial;
           // CeilingMaterial = ceilingMaterial;
        }
        
        public PolygonRoomModel(Vector3 position, int NumberOfWalls, float scale, float Height, string floor, string ceiling, DefaultNamespace.VREM.Model.Wall[] walls)
        {
            Position = position;
            numberOfWalls = NumberOfWalls;
           // size = Size;
            ceiling_scale = scale;
            height = Height;
            FloorMaterial = floor;
            CeilingMaterial = ceiling;
            this.walls = walls;
        }
        


/*
        public DefaultNamespace.VREM.Model.Wall[] GetWalls()
        {
            return _walls.ToArray();
        }

        public DefaultNamespace.VREM.Model.Wall GetWallAt(int index)
        {
            return _walls[index];
        }

        public void Add(DefaultNamespace.VREM.Model.Wall model)
        {
            _walls.Add(model);
        }
        */
    }
}