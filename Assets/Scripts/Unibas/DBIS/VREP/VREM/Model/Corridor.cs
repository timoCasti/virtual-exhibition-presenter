using System;
using System.Collections.Generic;
using UnityEngine;
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
        
        public Room[] connectsRoom;
    
    
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
    
        public Wall GetWall(WallOrientation orientation)
        {
            foreach (var wall in walls)
            {
                WallOrientation wor = (WallOrientation) Enum.Parse(typeof(WallOrientation), wall.direction, true);
                if (wor.Equals(orientation))
                {
                    return wall;
                }
            }

            return null;
        }

        public Room[] GetConnectsRoom()
        {
            return connectsRoom;
        }
        
        /**
         * calculate size and position of corridor based on the rooms to connect
         */
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
                         * room0   */ 
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
        }

    }
}