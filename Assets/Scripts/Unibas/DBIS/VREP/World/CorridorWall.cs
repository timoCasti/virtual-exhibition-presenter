using System.Collections.Generic;
using DefaultNamespace;
using Unibas.DBIS.DynamicModelling.Models;
using Unibas.DBIS.VREP;
using UnityEngine;
using World;

namespace Unibas.DBIS.VREP.World
{
    public class CorridorWall : MonoBehaviour
    {
        /**
         * The walls data
         */
        public DefaultNamespace.VREM.Model.Wall WallData { get; set; }
        
        public WallModel WallModel { get; set; }
        
        public GameObject Anchor{ get; set; }
        
        public WallOrientation GetOrientation()
        {
            return WallData.GetOrientation();
        }
    }
}