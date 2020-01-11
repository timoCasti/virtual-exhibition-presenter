using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using World;

namespace DefaultNamespace.VREM.Model
{
    [Serializable]
    public class Corridor
    {
        public string text;
        public Vector3 size;
        public Vector3 position;
        public Vector3 entrypoint;
        public Wall[] walls;

        public string floor;
        public string ceiling;

        public string ambient;

        public Exhibit[] exhibits; 
        
        [FormerlySerializedAs("connectsRoom")] public Room[] connects;
    
    
        public string GetURLEncodedAudioPath() {
            if (!string.IsNullOrEmpty(ambient))
            {
                return ServerSettings.SERVER_ID+"content/get/"+ ambient.Substring(0).Replace("/", "%2F").Replace(" ", "%20");
            }
            else
            {
                return null;
            }
      
        }
    
        public Wall GetWall(int wallNumber)
        {
            foreach (var wall in walls)
            {
                int wallnb = wall.wallNumber;
                if (wallnb.Equals(wallNumber))
                {
                    return wall;
                }
            }

            return null;
        }

        public Room[] GetConnectsRoom()
        {
            return connects;
        }
        
        /// <summary>
        /// calculate position of corridor depending on the rooms which have to be connected
        /// </summary>
        public void CalculatePosition()
        {
            //connects room is NULL
            Room room0 = connects[0];
            Room room1 = connects[1];

            
            List<DistanceAndCoordinate> distanceList = new List<DistanceAndCoordinate>();
            
            //is this the most efficient way?
            //find neares corners in the rooms to connect
            foreach (Wall wall0 in room0.walls)
            {
                foreach (Vector3 wallCoord0 in wall0.wallCoordinates)
                {
                    if (wallCoord0.y.Equals(0))
                    {
                        foreach (Wall wall1 in room1.walls)
                        {
                            foreach (Vector3 wallCoord1 in wall1.wallCoordinates)
                            {
                                if (wallCoord1.y.Equals(0))
                                {
                                    var dist = Vector3.Distance(wallCoord0, wallCoord1);
                                    distanceList.Add(new DistanceAndCoordinate(wallCoord0, wallCoord1, dist));
                                }
                            }
                        }
                    }
                }
            }

            float lowestDist = distanceList.Min(dist => dist.distance);
            DistanceAndCoordinate lowestDistanceCoord = distanceList.Find(x => x.distance.Equals(lowestDist));
            distanceList.Remove(lowestDistanceCoord);

            //find the second connection with the second lowest distance with two different corners
            bool found = false;
            float secondlowestDist = distanceList.Min(dist => dist.distance);
            DistanceAndCoordinate secondLowestDistanceCoord = distanceList.Find(x => x.distance.Equals(secondlowestDist));
            while (!found)
            {
                secondlowestDist = distanceList.Min(dist => dist.distance);
                secondLowestDistanceCoord = distanceList.Find(x => x.distance.Equals(secondlowestDist));
                distanceList.Remove(secondLowestDistanceCoord);
                
                if (!HaveTheSameCorners(lowestDistanceCoord, secondLowestDistanceCoord))
                {
                    found = true;
                }
            }//end while
            
            //assign new coordinates to the corridor walls
            walls[0].wallCoordinates[0]=lowestDistanceCoord.wallCoord0;
            walls[0].wallCoordinates[1]=lowestDistanceCoord.wallCoord0;
            walls[0].wallCoordinates[1].y = 5;
            walls[0].wallCoordinates[2]=lowestDistanceCoord.wallCoord1;
            walls[0].wallCoordinates[3]=lowestDistanceCoord.wallCoord1;
            walls[0].wallCoordinates[3].y = 5;
            
            walls[1].wallCoordinates[0]=secondLowestDistanceCoord.wallCoord0;
            walls[1].wallCoordinates[1]=secondLowestDistanceCoord.wallCoord0;
            walls[1].wallCoordinates[1].y = 5;
            walls[1].wallCoordinates[2]=secondLowestDistanceCoord.wallCoord1;
            walls[1].wallCoordinates[3]=secondLowestDistanceCoord.wallCoord1;                   
            walls[1].wallCoordinates[3].y = 5;     
                  
            //calculate position and size        
            size = CrossProduct(lowestDistanceCoord.wallCoord0, secondLowestDistanceCoord.wallCoord0);
            position = PositionFromDAC(lowestDistanceCoord,secondLowestDistanceCoord);
            entrypoint = position;
            Console.WriteLine("----Test n Corridor {0}", walls[1].wallCoordinates[0]);
            
            

        }//CalculatePosition

        //check if two distance and corner pairs have the same corners
        //returns true if one corner has the same coordinates.
        private bool HaveTheSameCorners(DistanceAndCoordinate dc0, DistanceAndCoordinate dc1)
        {
            return dc0.wallCoord0.Equals(dc1.wallCoord0) ||
                   dc0.wallCoord0.Equals(dc1.wallCoord1) ||
                    dc0.wallCoord1.Equals(dc1.wallCoord0) ||
                    dc0.wallCoord1.Equals(dc1.wallCoord1);
        }
        /**
         * calculate size and position of corridor based on the rooms to connect
         */
        /*
        public void CalculateSizeAndPosition()
        {
            
            Vector3 corridorPosition = new Vector3(0, 0, 0);
            Vector3 corridorSize = new Vector3(0, 0, 0);
            
            Room[] rooms=this.GetConnectsRoom();
            if (rooms.Length != 2)
            {    
                //todo error
                Debug.LogError("Couldn't calculate corridor size, not 2 rooms to connect");
                return;
            }
            
            //change room so that thirst room has lower x position
            if (rooms[0].position.x > rooms[1].position.x)
            {
                var temp = rooms[0];
                rooms[0] = rooms[1];
                rooms[1] = temp;
            }
            
            Vector3 pos0 = rooms[0].position;
            Vector3 pos1 = rooms[1].position;
            Vector3 size0 = rooms[0].size;
            Vector3 size1 = rooms[1].size;

            //room0----room1
            if (rooms[0].position.z+rooms[0].size.z > rooms[1].position.z)
            {
                corridorPosition.x = rooms[1].position.x;
                corridorPosition.y = rooms[1].position.y;
                corridorPosition.z = rooms[0].size.z;
                corridorSize.x=corridorPosition.x+2;
                corridorSize.y=5;
                corridorSize.z=rooms[1].size.z;
            }
            //room1----room0
            else if (rooms[1].position.z+rooms[1].size.z > rooms[0].position.z)
            {
                corridorPosition.x = rooms[1].position.x;
                corridorPosition.y = rooms[1].position.y;
                corridorPosition.z = rooms[1].size.z;
                corridorSize.x=corridorPosition.x+2;
                corridorSize.y=5;
                corridorSize.z=rooms[0].size.z;
            }
            /*            room1
                         *   \
                         *   \
                         * room0   *
            else
            {
                corridorPosition.x = rooms[0].size.x;
                corridorPosition.y = rooms[1].position.y;
                corridorPosition.z = rooms[1].size.z;
                corridorSize.x=rooms[1].position.x;
                corridorSize.y=5;
                corridorSize.z=corridorPosition.z+2;
            }
            
            this.position=corridorPosition;
            this.size=corridorSize;
        }*/

        private Vector3 PositionFromDAC(DistanceAndCoordinate d1, DistanceAndCoordinate d2)
        {
            Vector3 v1 = d1.wallCoord0;
            Vector3 v2 = d1.wallCoord1;
            Vector3 v3 = d2.wallCoord0;
            Vector3 v4 = d2.wallCoord1;
            
            float x = (v1.x + v2.x + v3.x + v4.x) / 4;
            float y = (v1.y + v2.y + v3.y + v4.y) / 4;
            float z = (v1.z + v2.z + v3.z + v4.z) / 4;
            return new Vector3(x,y,z);
        }
        private Vector3 CrossProduct(Vector3 v1, Vector3 v2)
        {
            
            /*
            float x = v1.y * v2.z - v2.y * v1.z;
            float y = (v1.x * v2.z - v2.x * v1.z) * -1;
            float z = v1.x * v2.y - v2.x * v1.y;
            */
            float x = (v1.x + v2.x) / 2;
            float y = (v1.y + v2.y ) / 2;
            float z = (v1.z + v2.z ) / 2;
            
            return new Vector3(x, y, z);
        }

    }//end class corridor

    //Local class to combine coordinates and distance 
    public class DistanceAndCoordinate
    {
        public Vector3 wallCoord0;
        public Vector3 wallCoord1;
        public float distance;

        public DistanceAndCoordinate(Vector3 wallcoord0, Vector3 wallcoord1, float dist)
        {
            wallCoord0 = wallcoord0;
            wallCoord1 = wallcoord1;
            distance = dist;
        }
    }


}