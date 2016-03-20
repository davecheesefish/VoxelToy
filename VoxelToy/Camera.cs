using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VoxelToy
{
    /// <summary>
    /// A camera used as a viewpoint for rendering 3D scenes.
    /// </summary>
    class Camera
    {
        /// <summary>
        /// The LookAt matrix for the camera, calculated from the position and target.
        /// </summary>
        public Matrix LookAtMatrix
        {
            get { return Matrix.CreateLookAt(Position, Target, Vector3.Up); }
        }

        /// <summary>
        /// Position of the camera in 3D space.
        /// </summary>
        public Vector3 Position { get { return position; } }
        protected Vector3 position;

        /// <summary>
        /// The position in the 3D space that the camera is looking at.
        /// </summary>
        public Vector3 Target { get { return target; } }
        protected Vector3 target;

        public Camera(Vector3 position, Vector3 target)
        {
            this.position = position;
            this.target = target;
        }
    }
}
