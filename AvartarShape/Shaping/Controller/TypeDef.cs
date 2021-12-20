namespace ShapingController
{
    public class Vector3d
    {
        public float x, y, z;

        public Vector3d()
        {
            x = 0.0f;
            y = 0.0f;
            z = 0.0f;
        }

        public Vector3d(float valuex, float valuey, float valuez)
        {
            x = valuex;
            y = valuey;
            z = valuez;
        }

        public static Vector3d operator +(Vector3d lhs, Vector3d rhs)
        {
            Vector3d ret = new Vector3d();
            ret.x = lhs.x + rhs.x;
            ret.y = lhs.y + rhs.y;
            ret.z = lhs.z + rhs.z;

            return ret;
        }
    }
}
