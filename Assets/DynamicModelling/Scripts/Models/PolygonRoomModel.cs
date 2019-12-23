using System.Collections.Generic;
using UnityEngine;

namespace Unibas.DBIS.DynamicModelling.Models
{
    [System.Serializable]
    public class PolygonRoomModel : IModel
    {
        public Vector3 Position;

        public double ceiling_scale;
        
        public PolygonRoomModel(Vector3 position, List<WallModel> walls, Material floorMaterial, Material ceilingMaterial)
        {
            Position = position;
            _walls = walls;
            FloorMaterial = floorMaterial;
            CeilingMaterial = ceilingMaterial;
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
        

        public WallModel[] GetWalls()
        {
            return _walls.ToArray();
        }

        public WallModel GetWallAt(int index)
        {
            return _walls[index];
        }

        public void Add(WallModel model)
        {
            _walls.Add(model);
        }
        
    }
}