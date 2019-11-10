using UnityEngine;

namespace Unibas.DBIS.DynamicModelling.Models
{
    [System.Serializable]
    public class CuboidCorridorModel : IModel
    {
        public Vector3 Position;
        public float Size;
        public float Height;

        public Material FloorMaterial;
        public Material CeilingMaterial;
        public Material NorthMaterial;
        public Material SouthMaterial;

        public CuboidCorridorModel(Vector3 position, float size, float height)
        {
            Position = position;
            Size = size;
            Height = height;
        }

        public CuboidCorridorModel(Vector3 position, float size, float height, Material floorMaterial = null, 
            Material ceilingMaterial = null, Material northMaterial = null, Material southMaterial = null)
        {
            Position = position;
            Size = size;
            Height = height;
            FloorMaterial = floorMaterial;
            CeilingMaterial = ceilingMaterial;
            NorthMaterial = northMaterial;
            SouthMaterial = southMaterial;
        }
    }
    
}