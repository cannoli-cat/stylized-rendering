using UnityEngine;

namespace CannoliCat.Stylized {
    public static class OklabUtility {
        // must match SZ_LinearToOklab; see StylizedColor.hlsl
        public static Vector3 LinearToOklab(Vector3 linear) {
            var l = Vector3.Dot(linear, new Vector3(0.4122214708f, 0.5363325363f, 0.0514459929f));
            var m = Vector3.Dot(linear, new Vector3(0.2119034982f, 0.6806995451f, 0.1073969566f));
            var s = Vector3.Dot(linear, new Vector3(0.0883024619f, 0.2817188376f, 0.6299787005f));

            var lmsCbrt = new Vector3(Cbrt(l), Cbrt(m), Cbrt(s));

            return new Vector3(Vector3.Dot(lmsCbrt, new Vector3(0.2104542553f, 0.7936177850f, -0.0040720468f)), 
                               Vector3.Dot(lmsCbrt, new Vector3(1.9779984951f, -2.4285922050f, 0.4505937099f)), 
                               Vector3.Dot(lmsCbrt, new Vector3(0.0259040371f, 0.7827717662f, -0.8086757660f)));
        }
        
        public static Vector3 LinearToOklab(Color linear) {
            return LinearToOklab(new Vector3(linear.r, linear.g, linear.b));
        }
        
        private static float Cbrt(float x) => Mathf.Sign(x) * Mathf.Pow(Mathf.Abs(x), 1f / 3f);
    }
}   