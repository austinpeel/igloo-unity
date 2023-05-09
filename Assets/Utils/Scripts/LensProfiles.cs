using UnityEngine;

public static class LensProfiles
{
    // Compute the convergence Kappa of the SIE profile
    // x and y coordinates and Einstein Radius in arcsec
    // Angle in degree
    public static float KappaSIE(float x, float y, float einsteinRadius, float q, float angle)
    {
        // From COOLEST :
        // With the major axis of the ellipsoid along the x axis
        // Kappa = einsteinRadius / (2 * Mathf.sqrt(q * (x^2) + (y^2) / q))
        float angleInRad = (90f + angle) * Mathf.Deg2Rad;

        float rotatedX = x * Mathf.Cos(angleInRad) + y * Mathf.Sin(angleInRad);
        float rotatedY = -x * Mathf.Sin(angleInRad) + y * Mathf.Cos(angleInRad);

        if (x == 0 && y == 0) return float.MaxValue;

        return einsteinRadius / (2f * Mathf.Sqrt(q * (rotatedX*rotatedX) + (rotatedY*rotatedY) / q));
    }

    // Compute the convergence Kappa of the SIS profile
    // x and y coordinates and Einstein Radius in arcsec
    public static float KappaSIS(float x, float y, float einsteinRadius)
    {
        // From COOLEST :
        // Kappa = einsteinRadius / (2 * Mathf.sqrt((x^2) + (y^2)))
        return einsteinRadius / (2f * Mathf.Sqrt((x*x) + (y*y)));
    }
}
