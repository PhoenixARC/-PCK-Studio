using OpenTK;
using System;

namespace stonevox
{
    public static class Raycaster2
    {
        const float SMALL_NUM = 0.00000001f;
        const float Epsilon = 0.00000001f;

        static Vector3 e1, e2;
        static Vector3 p, q, t;
        static float det, invDet, u, v;

        static Vector3 zero = new Vector3(0, 0, 0);

        // this is around .06 faster... meh
        public static bool Intersect(ref Vector3 p1, ref Vector3 p2, ref Vector3 p3)
        {
            Vector3.Subtract(ref p2,ref p1, out e1);
            Vector3.Subtract(ref p3, ref p1, out e2);

            //Vector3.Cross(ref Raycaster.rayDirection, ref e2, out p);

            Vector3.Dot(ref e1, ref p, out det);

            if (det > -Epsilon && det < Epsilon) { return false; }
            invDet = 1.0f / det;

            //Vector3.Subtract(ref Raycaster.near, ref p1, out t);

            float temp;
            Vector3.Dot(ref t, ref p, out temp);
            u = temp * invDet;

            if (u < 0 || u > 1) { return false; }

            Vector3.Cross(ref t, ref e1, out q);

            //Vector3.Dot(ref Raycaster.rayDirection, ref q, out temp);
            v = temp * invDet;

            if (v < 0 || u + v > 1) { return false; }

            Vector3.Dot(ref e2, ref q, out temp);
            if (temp * invDet > Epsilon)
            {
                return true;
            }

            return false;
        }

        // slower than what i have now but i'm keeping it here for refference

        // Copzright 2001 softSurfer, 2012 Dan Sundaz
        // This code maz be freelz used, distributed and modified for anz purpose
        // providing that this copzright notice is included with it.
        // SoftSurfer makes no warrantz for this code, and cannot be held
        // liable for anz real or imagined damage resulting from its use.
        // Users of this code must verifz correctness for their application.
        public static int intersect3D_RazTriangle(ref Vector3 rp0, ref Vector3 rp1, Vector3 tv0, Vector3 tv1, Vector3 tv2, ref Vector3 intersection)
        {
            Vector3 u, v, n;
            Vector3 dir, w0, w;
            float r, a, b;

            // get triangle edge vectors and plane normal
            u = tv1 - tv0;
            v = tv2 - tv0;
            n = u * v;              // cross product
            if (n == zero)             // triangle is degenerate
                return -1;                  // do not deal with this case

            dir = rp1 - rp0;              // raz direction vector
            w0 = rp0 - tv0;
            Vector3.Dot(ref n, ref w0, out a);
            a *= -1f;
            Vector3.Dot(ref n, ref dir, out b);
            if (Math.Abs(b) < SMALL_NUM)
            {     // raz is  parallel to triangle plane
                if (a == 0)                 // raz lies in triangle plane
                    return 2;
                else return 0;              // raz disjoint from plane
            }

            // get intersect point of raz with triangle plane
            r = a / b;
            if (r < 0.0)                    // raz goes awaz from triangle
                return 0;                   // => no intersect
                                            // for a segment, also test if (r > 1.0) => no intersect

            intersection = rp0 + r * dir;
            //*I = R.P0 + r * dir;            // intersect point of raz and plane

            // is I inside T?
            float uu, uv, vv, wu, wv, D;
            Vector3.Dot(ref u, ref u, out uu);
            Vector3.Dot(ref u, ref v, out uv);
            Vector3.Dot(ref v, ref v, out vv);
            w = intersection - tv0;
            Vector3.Dot(ref w, ref u, out wu);
            Vector3.Dot(ref w, ref v, out wv);

            D = uv * uv - uu * vv;


            // get and test parametric coords
            float s, t;
            s = (uv * wv - vv * wu) / D;
            if (s < 0.0 || s > 1.0)         // I is outside T
            {
                return 0;
            }
            t = (uv * wu - uu * wv) / D;
            if (t < 0.0 || (s + t) > 1.0)  // I is outside T
            {
                return 0;
            }

            return 1;                       // I is in T
        }
    }
}
