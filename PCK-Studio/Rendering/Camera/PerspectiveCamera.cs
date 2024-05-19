/* Copyright (c) 2024-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
// movement code taken from: 
// https://github.com/TheCherno/Hazel/blob/master/Hazel/src/Hazel/Renderer/EditorCamera.cpp
using System;
using System.Drawing;
using OpenTK;

namespace PckStudio.Rendering.Camera
{
    internal class PerspectiveCamera : Camera
    {
        public float NearClip
        {
            get => _nearClip;
            set
            {
                _nearClip = value;
                UpdateProjection();
            }
        }

        public float FarClip
        {
            get => _farClip;
            set
            {
                _farClip = value;
                UpdateProjection();
            }
        }

        public float Distance
        {
            get => _spherical.Radius;
            set
            {
                _spherical.Radius = Math.Max(value, 2f);
                UpdateViewMatrix();
            }
        }

        public float RotationSpeed { get; set; }  = 5f;

        public Size ViewportSize
        {
            get => _viewportSize;
            set
            {
                _viewportSize = value;
                UpdateProjection();
            }
        }

        public Vector3 WorldPosition => _position;

        public Vector3 FocalPoint
        {
            get => _focalPoint;
            set
            {
                _focalPoint = value;
                UpdateViewMatrix();
            }
        }

        public float Pitch
        {
            get => _spherical.Theta;
            set
            {
                _spherical.Theta = MathHelper.Clamp(value, -90f, 90f);
                UpdateViewMatrix();
            }
        }

        public float Yaw
        {
            get => _spherical.Phi;
            set
            {
                _spherical.Phi = value % 360f;
                UpdateViewMatrix();
            }
        }

        public Vector3 Orientation => -Vector3.UnitZ;

        public Vector3 Up => Vector3.UnitY;
        
        public float MinimumFov { get; } = 30f;
        public float MaximumFov { get; } = 180f;

        public float Fov
        {
            get => _fov;
            set
            {
                _fov = MathHelper.Clamp(value, MinimumFov, MaximumFov);
                UpdateProjection();
            }
        }

        public PerspectiveCamera(float fov, Vector3 target)
        {
            _fov = fov;
            _focalPoint = target;
            _nearClip = 1f;
            _farClip = 1000f;
            UpdateProjection();
        }

        private Matrix4 viewMatrix;
        private float _fov;

        private float _nearClip;
        private float _farClip;
        private Spherical _spherical;
        private Size _viewportSize;
        private Vector3 _position;
        private Vector3 _focalPoint;

        public override Matrix4 GetViewProjection()
        {
            return viewMatrix * projectionMatrix;
        }

        public Matrix4 GetProjection()
        {
            return projectionMatrix;
        }

        private void UpdateViewMatrix()
        {
            Matrix4 rotation = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Yaw)) * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Pitch));

            viewMatrix = Matrix4.CreateTranslation(FocalPoint) * rotation * Matrix4.CreateTranslation(0, 0, -Distance);
            // Position in Right-handed coordinates
            _position = viewMatrix.Inverted().ExtractTranslation();
        }

        private void UpdateProjection()
        {
            float aspect = (float)ViewportSize.Width / (float)ViewportSize.Height;
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)MathHelper.DegreesToRadians(Fov), aspect, NearClip, FarClip);
        }

        private Vector2 GetPanSpeed()
        {
            float x = Math.Min(ViewportSize.Width / 100.0f, 1.4f); // max = 2.4f
            float xFactor = 0.0366f * (x * x) - 0.1778f * x + 0.3021f;

            float y = Math.Min(ViewportSize.Height / 100.0f, 1.4f); // max = 2.4f
            float yFactor = 0.0366f * (y * y) - 0.1778f * y + 0.3021f;

            return new Vector2(xFactor, yFactor);
        }

        public void Pan(Vector2 delta)
        {
            Pan(delta.X, delta.Y);
        }

        public void Pan(float deltaX, float deltaY)
        {
            Vector2 panSpeed = GetPanSpeed();

            // Taken from: blockbench
            // https://github.com/JannisX11/blockbench/blob/a56fe01a517ace8d013f67bbd3d02442c44d3141/js/preview/OrbitControls.js#L271-L322
            Vector3 left = viewMatrix.Column0.Xyz * -Distance;
            Vector3 up = viewMatrix.Column1.Xyz * Distance;
            _focalPoint -= left * deltaX * panSpeed.X;
            _focalPoint -= up * deltaY * panSpeed.Y;
            UpdateViewMatrix();
        }

        public void Rotate(Vector2 delta) 
        {
            Rotate(delta.X, delta.Y);
        }

        public void Rotate(float deltaX,float deltaY)
        {
            Yaw += deltaX * RotationSpeed;
            Pitch += deltaY * RotationSpeed;
        }

        public override string ToString()
        {
            return $"FOV: {Fov}\nPosition: {WorldPosition}\nRotation: {_spherical}";
        }
    }
}
