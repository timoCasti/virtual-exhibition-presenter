using System.Collections.Generic;
using UnityEngine;

namespace Unibas.DBIS.DynamicModelling.Models
{
    [System.Serializable]
    public class PolygonRoomModel : IModel
    {
        public Vector3 Position;
        
        //private List<DefaultNamespace.VREM.Model.Wall> _walls;

        public int numberOfWalls;
        
        public float size;
        public float height;

        public Material[] materials;
        //private List<WallModel> _walls;
        public Material FloorMaterial;
        public Material CeilingMaterial;

        
        public PolygonRoomModel(Vector3 position, int NumberOfWalls, float Size, float Height)
        {
            Position = position;
            numberOfWalls = NumberOfWalls;
            size = Size;
            height = Height;
           // FloorMaterial = floorMaterial;
           // CeilingMaterial = ceilingMaterial;
        }
        
        public PolygonRoomModel(Vector3 position, int NumberOfWalls, float Size, float Height, Material floor, Material ceiling, Material[] wallmats)
        {
            Position = position;
            numberOfWalls = NumberOfWalls;
            size = Size;
            height = Height;
            FloorMaterial = floor;
            CeilingMaterial = ceiling;
            materials = wallmats;
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