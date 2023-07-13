using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace AI_assignment
{
    internal static class UtilitiesClass
    {
        public static Vector2 YFlip(this Point pPoint, float pScreenHeight)
        {
            return new Vector2(pPoint.X, pScreenHeight * (1f - pPoint.Y / pScreenHeight));
        }

        public static Vector2 YFlip(this Vector2 pVector, float pScreenHeight)
        {
            pVector.Y = pScreenHeight * (1f - (pVector.Y / pScreenHeight));
            return pVector;
        }

        public static Vector2 CentreTexture(this Vector2 pCurrentPosition, int pTextureWidth, int pTextureHeight)
        {
            return new Vector2(pCurrentPosition.X - pTextureWidth * 0.5f, pCurrentPosition.Y - pTextureHeight * 0.5f);
        }

        public static bool IsInsideCircle(Vector2 pPoint, Vector2 pCircle, float pRadius)
        {
            return Vector2.DistanceSquared(pPoint, pCircle) < pRadius * pRadius;
        }

        public static Vector2 Clamp(this Vector2 pVector, float pMaxLength)
        {
            float length = pVector.Length();

            if(length > pMaxLength)
            {
                float scaleFactor = pMaxLength / length;
                pVector.X *= scaleFactor;
                pVector.Y *= scaleFactor;
            }
            return pVector;
        }
    }
}
