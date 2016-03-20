using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VoxelToy
{
    /// <summary>
    /// A free-moving camera.
    /// </summary>
    class FreeCamera:Camera
    {
        /// <summary>
        /// Horizontal rotation.
        /// </summary>
        float hAngle = 0.0f;

        /// <summary>
        /// Vertical rotation.
        /// </summary>
        float vAngle = 0.0f;

        /// <summary>
        /// Matrix to rotate all movement by before applying.
        /// </summary>
        Matrix rotationMatrix;

        /// <summary>
        /// The maximum vertical rotation from horizontal.
        /// Slightly less than pi/2 to avoid being perpendicular to the camera's "up" vector
        /// during rendering.
        /// </summary>
        private static double maxVAngle = Math.PI * 0.4999;
        private static Vector3 forwardVector = new Vector3(0, 0, 1);
        
        public FreeCamera(Vector3 position)
            : base(position, new Vector3(position.X, position.Y, position.Z + 1))
        {
            RecalculateRotationMatrix();
        }

        /// <summary>
        /// Moves the camera right along its local X axis.
        /// Use a negative amount to move the camera left.
        /// </summary>
        /// <param name="amount">The distance to move the camera.</param>
        public void TranslateX(float amount)
        {
            position += Vector3.Transform(amount * Vector3.Left, rotationMatrix);
            MoveTarget();
        }

        /// <summary>
        /// Moves the camera up along its local Y axis.
        /// Use a negative amount to move the camera down.
        /// </summary>
        /// <param name="amount">The distance to move the camera.</param>
        public void TranslateY(float amount)
        {
            position += Vector3.Transform(amount * Vector3.Up, rotationMatrix);
            MoveTarget();
        }

        /// <summary>
        /// Moves the camera forwards along its local Z axis.
        /// Use a negative amount to move the camera backwards.
        /// </summary>
        /// <param name="amount">The distance to move the camera.</param>
        public void TranslateZ(float amount)
        {
            // MonoGame's Vector3.Backward vector is actually forwards.
            position += Vector3.Transform(amount * Vector3.Backward, rotationMatrix);
            MoveTarget();
        }

        /// <summary>
        /// Rotates the camera around its local Y axis.
        /// </summary>
        /// <param name="amount"></param>
        public void RotateY(float amount)
        {
            hAngle += amount;
            hAngle %= MathHelper.TwoPi; // Wrap
            RecalculateRotationMatrix();
            MoveTarget();
        }

        /// <summary>
        /// Rotates the camera around its local X axis.
        /// </summary>
        /// <param name="amount"></param>
        public void RotateX(float amount)
        {
            vAngle += amount;

            // Clamp within range.
            if (vAngle < -maxVAngle)
            {
                vAngle = (float)-maxVAngle;
            }
            else if (vAngle > maxVAngle)
            {
                vAngle = (float)maxVAngle;
            }

            RecalculateRotationMatrix();
            MoveTarget();
        }

        private void RecalculateRotationMatrix()
        {
            rotationMatrix = Matrix.CreateRotationX(vAngle) * Matrix.CreateRotationY(hAngle);
        }

        private void MoveTarget()
        {
            target = position + Vector3.Transform(forwardVector, rotationMatrix);
        }
    }
}
