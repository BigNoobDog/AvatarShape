
namespace ShapingController
{
    public class Vector3d: System.ICloneable
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

        public object Clone()
        {
            Vector3d newV = new Vector3d();
            newV.x = this.x;
            newV.y = this.y;
            newV.z = this.z;
            return (object)newV;
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

    public class Vector2d : System.ICloneable
    {
        public float x, y;

        public Vector2d()
        {
            x = 0.0f;
            y = 0.0f;
        }

        public Vector2d(float valuex, float valuey)
        {
            x = valuex;
            y = valuey;
        }

        public object Clone()
        {
            Vector3d newV = new Vector3d();
            newV.x = this.x;
            newV.y = this.y;
            return (object)newV;
        }

        public static Vector2d operator +(Vector2d lhs, Vector2d rhs)
        {
            Vector2d ret = new Vector2d();
            ret.x = lhs.x + rhs.x;
            ret.y = lhs.y + rhs.y;
            
            return ret;
        }
    }
}
