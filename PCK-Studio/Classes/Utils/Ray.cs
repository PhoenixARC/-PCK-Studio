using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using PckStudio.Rendering;

namespace PckStudio.Classes.Utils
{
    public class Ray
    {
        public Vector3 CurrentRay { get; set; } = new Vector3();

        public Matrix4 ViewMatrix { get; set; }
        public Matrix4 ProjectionMatrix { get; set; }

        public Vector3 CamPos { get; set; }

        public Size Size { get; set; }

        public Point Pos { get; set; }

        public struct RayResult
        {
            public float Distance { get; set; }

            public enum eAxis
            {
                X,
                Y,
                Z
            }

            public eAxis Axis { get; set; }

            public RayResult(float distance, eAxis axis)
            {
                Distance = distance;
                Axis =  axis;
            }
        }

        public Ray(ref Matrix4 viewMatrix, ref Matrix4 projectionMatrix, Size size, Vector3 cameraPosition)
        {
            ProjectionMatrix = projectionMatrix;
            ViewMatrix = viewMatrix;
            Size = size;
            CamPos = cameraPosition;
        }

        private void Update(int X, int Y)
        {
            CurrentRay = CalculateRay(X, Y);
        }

        private Vector3 CalculateRay(int x, int y)
        {
            var normalizedCoords = GetNormalisedDeviceCoordinates(x, y);
            var clipCoords = new Vector4(normalizedCoords.X, normalizedCoords.Y, -1.0f, 1.0f);
            var eyeCoords = ToEyeCoords(clipCoords);
            var worldRay = ToWorldCoords(eyeCoords);
            return worldRay;
        }

        private Vector4 ToEyeCoords(Vector4 clipCoords)
        {
            var invertedProjection = Matrix4.Invert(ProjectionMatrix);
            var eyeCoords = Vector4.Transform(clipCoords, invertedProjection);
            return new Vector4(eyeCoords.X, eyeCoords.Y, -1.0f, 0f);
        }

        private Vector3 ToWorldCoords(Vector4 eyeCoords)
        {
            var invertedView = Matrix4.Invert(ViewMatrix);
            var rayWorld = Vector4.Transform(eyeCoords, invertedView);
            var ray = new Vector3(rayWorld);
            ray.Normalize();
            return ray;
        }

        private Vector2 GetNormalisedDeviceCoordinates(int x, int y)
        {
            return new Vector2()
            {
                X = 2.0f * x / Size.Width - 1.0f,
                Y = -(2.0f * y / Size.Height - 1.0f)
            };
        }

        private Vector3 GetPointOnRay(Vector3 ray, float distance)
        {
            var start = new Vector3(CamPos);
            var scaledRay = ray * distance;
            return start + scaledRay;
        }
    }
}
