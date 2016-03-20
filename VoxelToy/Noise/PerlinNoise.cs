using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VoxelToy.Noise
{
    class PerlinNoise
    {
        /// <summary>
        /// A random array of the numbers 0-256. Used for hashing input co-ordinates.
        /// The array is duplicated to avoid a buffer overflow in noise computation functions.
        /// </summary>
        private int[] permutation;

        public PerlinNoise(int seed)
        {
            Random rand = new Random(seed);

            // Initialise the permutation array.
            int[] p = new int[256]; // Temporary array of the numbers 0 to 256
            for (int i = 0; i < 256; ++i)
            {
                p[i] = i;
            }
            p = p.OrderBy(x => rand.Next()).ToArray(); // Randomise p[]

            permutation = new int[512];
            for (int i = 0; i < 256; ++i)
            {
                permutation[i] = p[i]; // Copy p[] into permutation[] twice
                permutation[i + 256] = p[i];
            }
        }

        /// <summary>
        /// Generates a Perlin noise value for the 2D co-ordinate (x, y).
        /// NOTE: This function returns 0 for all integer co-ordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="frequency">The frequency of the noise function. A lower value will result in smoother noise.</param>
        /// <returns>A pseudo-random number between 0.0 and 1.0.</returns>
        public double Generate2D(double x, double y, double frequency = 1.0)
        {
            // Each axis is split into 256, forming 256^3 "unit squares".
            // Each point on the grid is given a random "gradient" vector, which are
            // interpolated based on the position of the given point within a square.
            // More: https://flafla2.github.io/2014/08/09/perlinnoise.html

            x = x * frequency;
            y = y * frequency;

            // Calculate which square the given co-ordinates lie in.
            int squareX = (int)x & 255;  // These will repeat every 256 units, since we only take
            int squareY = (int)y & 255;  // into account the 8 least significant bits.

            // Now calculate the local co-ordinates of the point within that square.
            double localX = x - (int)x;
            double localY = y - (int)y;

            // Work out which gradient vectors to use for the 4 corners of this square.
            // First, hash the corner co-ordinates of this square using the pre-calculated random
            // permutation array. This hashing method is defined as part of the Perlin noise
            // algorithm.
            // Variables are named gradientHashXY.
            // An A indicates the squareX or squareY value (lesser-axis-value side of the unit
            // square), B indicates those values plus 1 (greater-axis-value side of the unit square).
            int gradientHashAA = permutation[permutation[squareX] + squareY],
                gradientHashAB = permutation[permutation[squareX] + squareY + 1],
                gradientHashBA = permutation[permutation[squareX + 1] + squareY],
                gradientHashBB = permutation[permutation[squareX + 1] + squareY + 1];

            // Get eased co-ordinates to use with linear interpolation.
            double easedX = Fade(localX);
            double easedY = Fade(localY);

            // Use bilinear interpolation to find the value at the eased co-ordinates from the dot
            // products found at the corners of the square.
            double x1, x2;
            // First, interpolate along the X axis to find the interpolated values at the two edges.
            x1 = Lerp(GradientDot2D(gradientHashAA, localX, localY), GradientDot2D(gradientHashBA, localX - 1, localY), easedX);
            x2 = Lerp(GradientDot2D(gradientHashAB, localX, localY - 1), GradientDot2D(gradientHashBB, localX - 1, localY - 1), easedX);
            // Then interpolate these values along the Y axis to find the final result.
            // The range of this value is -1.0 to 1.0, so also bind to the range 0.0 to 1.0 before
            // returning.
            return (Lerp(x1, x2, easedY) + 1.0) / 2.0;
        }

        /// <summary>
        /// Generates a Perlin noise value for the 3D co-ordinate (x, y, z).
        /// NOTE: This function returns 0 for all integer co-ordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="frequency">The frequency of the noise function. A lower value will result in smoother noise.</param>
        /// <returns>A pseudo-random number between 0.0 and 1.0.</returns>
        public double Generate3D(double x, double y, double z, double frequency = 1.0){
            // Each axis is split into 256, forming 256^3 "unit cubes".
            // Each vertex of the cube grid is given a random "gradient" vector, which are
            // interpolated based on the position of the given point within a cube.
            // More: https://flafla2.github.io/2014/08/09/perlinnoise.html

            x = x * frequency;
            y = y * frequency;
            z = z * frequency;

            // Calculate which cube the given co-ordinates lie in.
            int cubeX = (int)x & 255;  // These will repeat every 256 units, since we only take
            int cubeY = (int)y & 255;  // into account the 8 least significant bits.
            int cubeZ = (int)z & 255;

            // Now calculate the local co-ordinates of the point within that cube.
            double localX = x - (int)x;
            double localY = y - (int)y;
            double localZ = z - (int)z;

            // Work out which gradient vectors to use for the 8 corners of this cube.
            // First, hash the corner co-ordinates of this cube using the pre-calculated random
            // permutation array. This hashing method is defined as part of the Perlin noise
            // algorithm.
            // Variables are named gradientHashXYZ.
            // An A indicates the cubeX, cubeY or cubeZ value (lesser-axis-value side of the unit
            // cube), B indicates those values plus 1 (greater-axis-value side of the unit cube).
            int gradientHashAAA = permutation[permutation[permutation[cubeX] + cubeY] + cubeZ],
                gradientHashABA = permutation[permutation[permutation[cubeX] + cubeY + 1] + cubeZ],
                gradientHashAAB = permutation[permutation[permutation[cubeX] + cubeY] + cubeZ + 1],
                gradientHashABB = permutation[permutation[permutation[cubeX] + cubeY + 1] + cubeZ + 1],
                gradientHashBAA = permutation[permutation[permutation[cubeX + 1] + cubeY] + cubeZ],
                gradientHashBBA = permutation[permutation[permutation[cubeX + 1] + cubeY + 1] + cubeZ],
                gradientHashBAB = permutation[permutation[permutation[cubeX + 1] + cubeY] + cubeZ + 1],
                gradientHashBBB = permutation[permutation[permutation[cubeX + 1] + cubeY + 1] + cubeZ + 1];

            // Get eased co-ordinates to use with linear interpolation.
            double easedX = Fade(localX);
            double easedY = Fade(localY);
            double easedZ = Fade(localZ);

            // Use trilinear interpolation to find the value at the eased co-ordinates from the dot
            // products found at the corners of the cube.
            double x1, x2, x3, x4, y1, y2;
            // First, interpolate along the X axis to find the interpolated values at the four corners of the YZ plane.
            x1 = Lerp(GradientDot3D(gradientHashAAA, localX, localY, localZ), GradientDot3D(gradientHashBAA, localX - 1, localY, localZ), easedX);
            x2 = Lerp(GradientDot3D(gradientHashABA, localX, localY - 1, localZ), GradientDot3D(gradientHashBBA, localX - 1, localY - 1, localZ), easedX);
            x3 = Lerp(GradientDot3D(gradientHashAAB, localX, localY, localZ - 1), GradientDot3D(gradientHashBAB, localX - 1, localY, localZ - 1), easedX);
            x4 = Lerp(GradientDot3D(gradientHashABB, localX, localY - 1, localZ - 1), GradientDot3D(gradientHashBBB, localX - 1, localY - 1, localZ - 1), easedX);
            // Now interpolate these values along the Y axis to find the two values at the vertical edges of the new plane.
            y1 = Lerp(x1, x2, easedY);
            y2 = Lerp(x3, x4, easedY);
            // Finally, interpolate these values along the Z axis to find the final result.
            // The range of this value is -1.0 to 1.0, so also bind to the range 0.0 to 1.0 before
            // returning.
            return (Lerp(y1, y2, easedZ) + 1.0) / 2.0;
        }

        /// <summary>
        /// Generates more natural-looking noise by taking a weighted average of several noise
        /// values generated with different frequencies.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="frequency">The frequency of the noise function. A lower value will result
        /// in smoother noise.</param>
        /// <param name="octaves">The number of values to average together. A larger number will
        /// produce less continuous noise.</param>
        /// <param name="persistence">How much each successive noise value contributes to the
        /// result, usually more than 0.0 and less than 1.0.</param>
        /// <returns>A psudo-random noise value in the range 0.0 to 1.0.</returns>
        public double OctaveGenerate2D(double x, double y, double frequency, int octaves, double persistence = 0.5)
        {
            double total = 0; // The running total of noise values generated.
            double max = 0;   // The maximum noise total that could have been generated.
            double amplitude = 1; // The maximum value for each noise value.
            for (int i = 0; i < octaves; ++i)
            {
                // Generate a noise value, multiply by the amplitude and add to the total.
                total += amplitude * Generate2D(x, y, frequency);

                // Update the maximum value that could have been generated.
                max += amplitude;

                // Reduce the amplitude and double the frequency for the next value.
                amplitude *= persistence;
                frequency *= 2;
            }

            // Finally, normalise to the range 0.0 to 1.0 by dividing by the maximum.
            return total / max;
        }

        /// <summary>
        /// Eases local co-ordinates towards integer values, smoothing the end result.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private double Fade(double num)
        {
            // The easing function defined by Ken Perlin: 6t^5 - 15t^4 + 10t^3
            return num * num * num * (num * (6 * num - 15) + 10);
        }

        /// <summary>
        /// Linearly interpolates between two values based on a given amount.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private double Lerp(double value1, double value2, double amount)
        {
            // Safe method, guarantees that floating-point errors will not affect return value for
            // amount = 0 or 1.
            return (1 - amount) * value1 + amount * value2;
        }

        /// <summary>
        /// Returns the dot product between the hashed gradient vector and a 2D local location vector.
        /// </summary>
        /// <param name="gradientHash"></param>
        /// <param name="localX"></param>
        /// <param name="localY"></param>
        /// <returns></returns>
        private static double GradientDot2D(int gradientHash, double localX, double localY)
        {
            // Gradient vectors could be completely random, but are restricted to only the vectors
            // that describe the direction from the centre of a square to its edges. For reasons why,
            // see Ken Perlin's paper "Improving Noise" at http://mrl.nyu.edu/~perlin/paper445.pdf

            // This restriction also makes calculating the dot products easy, since the only values
            // the gradient vector components can have are -1, 0 and 1.


            // Select a random gradient vector using the 2 least significant bits of the hash.
            switch (gradientHash & 0x3)
            {
                case 0x0: return localY;  // Gradient ( 0,  1)
                case 0x1: return -localY; // Gradient ( 0, -1)
                case 0x2: return -localX;  // Gradient (-1,  0)
                case 0x3: return localX; // Gradient ( 1,  0)

                default: return 0;
            }
        }

        /// <summary>
        /// Returns the dot product between the hashed gradient vector and a 3D local location vector.
        /// </summary>
        /// <param name="gradientHash"></param>
        /// <param name="localX"></param>
        /// <param name="localY"></param>
        /// <param name="localZ"></param>
        /// <returns></returns>
        private static double GradientDot3D(int gradientHash, double localX, double localY, double localZ)
        {
            // Gradient vectors could be completely random, but are restricted to only the vectors
            // that describe the direction from the centre of a cube to its edges. For reasons why,
            // see Ken Perlin's paper "Improving Noise" at http://mrl.nyu.edu/~perlin/paper445.pdf

            // This restriction also makes calculating the dot products easy, since the only values
            // the gradient vector components can have are -1, 0 and 1.


            // Select a random gradient vector using the 4 least significant bits of the hash.
            switch (gradientHash & 0xf)
            {
                case 0x0: return localX + localY;  // Gradient ( 1,  1,  0)
                case 0x1: return -localX + localY; // Gradient (-1,  1,  0)
                case 0x2: return localX - localY;  // Gradient ( 1, -1,  0)
                case 0x3: return -localX - localY; // Gradient (-1, -1,  0)
                case 0x4: return localX + localZ;  // Gradient ( 1,  0,  1)
                case 0x5: return -localX + localZ; // Gradient (-1,  0,  1)
                case 0x6: return localX - localZ;  // Gradient ( 1,  0, -1)
                case 0x7: return -localX - localZ; // Gradient (-1,  0, -1)
                case 0x8: return localY + localZ;  // Gradient ( 0,  1,  1)
                case 0x9: return -localY + localZ; // Gradient ( 0, -1,  1)
                case 0xA: return localY - localZ;  // Gradient ( 0,  1, -1)
                case 0xB: return -localY - localZ; // Gradient ( 0, -1, -1)

                // The following 4 are repeated to avoid additional unnecessary computations.
                // For information on why this can be done without negatively affecting the
                // outcome, see http://mrl.nyu.edu/~perlin/paper445.pdf
                case 0xC: return localX + localY;  // Gradient ( 1,  1,  0)
                case 0xD: return -localY + localZ; // Gradient ( 0, -1,  1)
                case 0xE: return -localX + localY; // Gradient (-1,  1,  0)
                case 0xf: return -localY - localZ; // Gradient ( 0, -1, -1)

                default: return 0;
            }
        }
    }
}
