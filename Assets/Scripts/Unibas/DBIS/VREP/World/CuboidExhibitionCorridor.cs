using System.Collections.Generic;
using DefaultNamespace;
using Unibas.DBIS.DynamicModelling.Models;
using UnityEngine;
using World;

namespace Unibas.DBIS.VREP.World
{
    [RequireComponent(typeof(AudioLoader))]
    public class CuboidExhibitionCorridor : MonoBehaviour
    {
        /**
         * The corridors's 3d appearance as a model.
         */
        [SerializeField] 
        public CuboidCorridorModel CorridorModel { get; set; }

        /**
         * The model of the corridor which defines its appearance, its walls and everything.
         * This shall directly be passed from the backend server.
         */
        [SerializeField] 
        public DefaultNamespace.VREM.Model.Corridor CorridorData { get; set; }

        /// <summary>
        /// The actual model game object.
        /// </summary>
        public GameObject Model { get; set; }

        /// <summary>
        /// A list of CorridorWalls, which form the walls of this room.
        /// </summary>
        public List<CorridorWall> Walls { get; set; }

        /// <summary>
        /// The private audio loader component reference.
        /// </summary>
        private AudioLoader _audioLoader;

        private void Start()
        {
            _audioLoader = GetComponent<AudioLoader>();
        }

        /// <summary>
        /// Populates this room.
        /// In other words: The walls will load their exhibits and this room will place its exhibits in its space.
        /// </summary>
        public void Populate()
        {
            Debug.Log("Populate Wall in Corridor");
            PopulateCorridor();
            PopulateWalls();
        }

        /// <summary>
        /// Handler for leaving this room.
        /// Shall be called whenever the player leaves this room
        /// </summary>
        public void OnCorridorLeave()
        {
            _audioLoader.Stop();
        }

        /// <summary>
        /// Handler for entering this room
        /// Shall be called whenever the player enters this room
        /// </summary>
        public void OnCorridorEnter()
        {
            _audioLoader.Play();
        }

        /// <summary>
        /// Places the exhibits in this room's space.
        /// Currently not implemented.
        /// </summary>
        public void PopulateCorridor()
        {
            
            Debug.LogWarning("Cannot place 3d objects yet");
            /*
             * GameObject parent = new GameObject("Model Anchor");
		     * GameObject model = new GameObject("Model");
		     * model.transform.SetParent(parent.transform);
		     * parent.transform.position = pos;
		     * ObjLoader objLoader = model.AddComponent<ObjLoader>();
		     * model.transform.Rotate(-90,0,0);
		     * model.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		     * objLoader.Load(url);
             */
        }

        /// <summary>
        /// Induces the walls to place their exhibits.
        /// </summary>
        public void PopulateWalls()
        {
            //Walls.ForEach(ew => ew.AttachExhibits());
        }
        /*
        /// <summary>
        /// Returns the wall for the orientation.
        /// </summary>
        /// <param name="orientation">The orientation for which the wall is requested</param>
        /// <returns>The CorridorWall component for the specified orientation</returns>
        public CorridorWall GetWallForOrientation(WallOrientation orientation)
        {
            return Walls.Find(wall => wall.GetOrientation() == orientation);
        }*/

        /// <summary>
        /// Loads the ambient audio of this room.
        /// As soon as the audio is loaded, it will be played.
        /// </summary>
        public void LoadAmbientAudio()
        {
            if (!string.IsNullOrEmpty(CorridorData.GetURLEncodedAudioPath()))
            {
                Debug.Log("add audio to room");

                if (_audioLoader == null)
                {
                    _audioLoader = gameObject.AddComponent<AudioLoader>();
                }

                _audioLoader.ReloadAudio(CorridorData.GetURLEncodedAudioPath());
            }
        }

        public Vector3 GetEntryPoint()
        {
            return transform.position + CorridorData.entrypoint;
        }

        /*
        public void RestoreWallExhibits() {
            Walls.ForEach(w => w.RestoreDisplayals());
        }
        */
    }
}