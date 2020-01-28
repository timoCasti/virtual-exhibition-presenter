using System;
using System.Collections.Generic;
using UnityEngine;
using World;

namespace DefaultNamespace.VREM.Model {
  [Serializable]
  public class Wall {
    
    
    public Vector3 color;
    
    public int wallNumber; //name instead of direction?
    
    public Exhibit[] exhibits;

    public string texture; // NONE -> debug: colors

    public Vector3[] wallCoordinates; // Walls are defined through four coordinates
    /*
     *wC[2] *--------* wC[3]
     *      |        |
     *wC[0] *--------* wC[1]
     */
    
    
/*
    public WallOrientation GetOrientation()
    {
      return (WallOrientation) Enum.Parse(typeof(WallOrientation), direction, true);
    }
  */
  }
}