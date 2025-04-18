using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Cameras
{
    public interface ICamera
    {
        Vector3 GetPosition();
        public Vector3 GetDirection();

        public Vector3 RotateAroundY(Vector3 vector, float angleDegrees);
        Matrix4 Projection { get; }
        Matrix4 View { get; }
    }
}
