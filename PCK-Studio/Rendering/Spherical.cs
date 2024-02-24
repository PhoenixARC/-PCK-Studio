using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace PckStudio.Rendering
{
    internal struct Spherical
    {
        private Vector3 vector;

        public Spherical()
        {
            vector = new Vector3();
        }

        /// <summary>
        /// Radial distance
        /// </summary>
        public float Radius
        {
            get => vector.X;
            set => vector.X = value;
        }

        /// <summary>
        /// Polar angle
        /// </summary>
        public float Theta
        {
            get => vector.Y;
            set => vector.Y = value;
        }

        /// <summary>
        /// Azimuthal angle
        /// </summary>
        public float Phi
        {
            get => vector.Z;
            set => vector.Z = value;
        }

        public Vector3 GetPosition()
        {
            float sineAzimuthal   = (float)Math.Sin(MathHelper.DegreesToRadians(Phi));
            float cosineAzimuthal = (float)Math.Cos(MathHelper.DegreesToRadians(Phi));
            float sinePolar       = (float)Math.Sin(MathHelper.DegreesToRadians(Theta));
            float cosinePolar     = (float)Math.Cos(MathHelper.DegreesToRadians(Theta));

            Vector3 direction = new Vector3();
            direction.X = cosinePolar * sineAzimuthal;
            direction.Y = sineAzimuthal;
            direction.Z = sinePolar * cosineAzimuthal;
            return direction * Radius;
        }

        public override string ToString()
        {
            return $"Radius: {Radius}; Theta: {Theta}; Phi: {Phi}; Position: {GetPosition()}";
        }
    }
}
