public static class HelperMethods
{
    public static float GetSignedAngleFromEuler(float eulerAngle)
    {
        return eulerAngle > 180 ? eulerAngle - 360 : eulerAngle;
    }

}