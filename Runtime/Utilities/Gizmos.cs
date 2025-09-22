using NUnit.Framework.Constraints;
using UnityEngine;

namespace Muco
{
    public enum DrawType { Line, Solid };

    public class Gizmos
    {
        public static void DrawCube(Transform trans, Vector3 center, Vector3 size, DrawType drawType)
        {
            DrawCube(GetMatrix(trans), center, size, drawType);
        }

        public static void DrawCube(Matrix4x4 trans, Vector3 center, Vector3 size, DrawType drawType)
        {
            switch (drawType)
            {
                case DrawType.Line:
                    DrawWireCube(trans, center, size);
                    break;
                case DrawType.Solid:
                    Debug.Log("Not implemented");
                    break;
            }
        }

        public static void DrawWireCube(Matrix4x4 trans, Vector3 center, Vector3 size)
        {
            Vector3 v000 = TransformPoint(trans, center + new Vector3(-size.x, -size.y, -size.z) * 0.5f);
            Vector3 v001 = TransformPoint(trans, center + new Vector3(size.x, -size.y, -size.z) * 0.5f);
            Vector3 v010 = TransformPoint(trans, center + new Vector3(-size.x, size.y, -size.z) * 0.5f);
            Vector3 v011 = TransformPoint(trans, center + new Vector3(size.x, size.y, -size.z) * 0.5f);
            Vector3 v100 = TransformPoint(trans, center + new Vector3(-size.x, -size.y, size.z) * 0.5f);
            Vector3 v101 = TransformPoint(trans, center + new Vector3(size.x, -size.y, size.z) * 0.5f);
            Vector3 v110 = TransformPoint(trans, center + new Vector3(-size.x, size.y, size.z) * 0.5f);
            Vector3 v111 = TransformPoint(trans, center + new Vector3(size.x, size.y, size.z) * 0.5f);

            UnityEngine.Gizmos.DrawLine(v000, v001);
            UnityEngine.Gizmos.DrawLine(v001, v011);
            UnityEngine.Gizmos.DrawLine(v011, v010);
            UnityEngine.Gizmos.DrawLine(v010, v000);

            UnityEngine.Gizmos.DrawLine(v100, v101);
            UnityEngine.Gizmos.DrawLine(v101, v111);
            UnityEngine.Gizmos.DrawLine(v111, v110);
            UnityEngine.Gizmos.DrawLine(v110, v100);

            UnityEngine.Gizmos.DrawLine(v000, v100);
            UnityEngine.Gizmos.DrawLine(v001, v101);
            UnityEngine.Gizmos.DrawLine(v010, v110);
            UnityEngine.Gizmos.DrawLine(v011, v111);
        }

        public static void DrawSphere(Transform trans, Vector3 center, float r, DrawType drawType)
        {
            DrawSphere(GetMatrix(trans), center, r, drawType);
        }

        public static void DrawSphere(Matrix4x4 trans, Vector3 center, float r, DrawType drawType)
        {
            switch (drawType)
            {
                case DrawType.Line:
                    DrawLineSphere(trans, center, r);
                    break;
                case DrawType.Solid:
                    DrawSolidSphere(trans, center, r);
                    break;
            }
        }

        public static void DrawSolidSphere(Matrix4x4 trans, Vector3 center, float r)
        {
            //TODO draw properly transformed sphere mesh.
            UnityEngine.Gizmos.DrawSphere(TransformPoint(trans, center), trans.lossyScale.x * r);
        }

        public static Matrix4x4 GetMatrix(Transform trans)
        {
            if (trans == null)
                return Matrix4x4.identity;
            return trans.localToWorldMatrix;
        }

        public static void DrawLineSphere(Matrix4x4 trans, Vector3 center, float r)
        {
            var center_trans = trans * Matrix4x4.Translate(center);
            DrawLineCircle(center_trans, r);
            DrawLineCircle(center_trans * Matrix4x4.Rotate(Quaternion.Euler(new Vector3(90, 0, 0))), r);
            DrawLineCircle(center_trans * Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, 90, 0))), r);
        }

        public static void DrawLineCircle(Matrix4x4 trans, float r)
        {
            var n = 36;
            for (int i = 0; i < n; i++)
            {
                var w0 = (float)i / n;
                var w1 = (float)(i + 1) / n;

                Vector3 local_from = new Vector3(Mathf.Sin(w0 * Mathf.PI * 2), Mathf.Cos(w0 * Mathf.PI * 2), 0) * r;
                Vector3 local_to = new Vector3(Mathf.Sin(w1 * Mathf.PI * 2), Mathf.Cos(w1 * Mathf.PI * 2), 0) * r;

                Vector3 from = TransformPoint(trans, local_from);
                Vector3 to = TransformPoint(trans, local_to);

                UnityEngine.Gizmos.DrawLine(from, to);
            }
        }

        public static Vector3 TransformPoint(Matrix4x4 trans, Vector3 point)
        {
            Vector4 x = new Vector4(point.x, point.y, point.z, 1.0f);
            Vector4 y = trans * x;
            Vector3 z = new Vector3(y.x / y.w, y.y / y.w, y.z / y.w);
            return z;
        }
    }
}
