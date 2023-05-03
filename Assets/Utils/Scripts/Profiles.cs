using UnityEngine;

public static class Profiles
{
    // MASS PROFILES

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

    // LIGHT PROFILES

    // Compute the brightness of the Sérsic profile
    // x and y coordinates and half Light Radius in arcsec
    // Angle in degree
    public static float BrightnessSersic(float x, float y, float amp, float sersicIndex, float halfLightRadius, float q, float angle, bool log10 = false)
    {
        // From COOLEST :
        // With the major axis of the ellipsoid along the x axis
        // b_n = 1.9992*sersicIndex - 0.3271
        // OR from Wikipedia for sersicIndex > 0.36 : 
        // b_n = 2*sersicIndex - 1/3 + 4/(405*sersicIndex) + 46/(25515*sersicIndex**2) + 131/(1148175*sersicIndex**3) - 2194697/(30690717750*sersicIndex**4) 
        // Brightness = I_eff * exp(-b_n * ((Mathf.sqrt(q * (x^2) + (y^2) / q) / halfLightRadius) ^ (1/sersicIndex) - 1))

        // MAYBE CHANGE THE RESULT IF HALFLIGHTRADIUS = 0
        if (halfLightRadius == 0f) return 0f;

        float angleInRad = (90f + angle) * Mathf.Deg2Rad;
        float rotatedX = x * Mathf.Cos(angleInRad) + y * Mathf.Sin(angleInRad);
        float rotatedY = -x * Mathf.Sin(angleInRad) + y * Mathf.Cos(angleInRad);

        // Compute b_n as COOLEST
        // float bn = 1.9992f*sersicIndex - 0.3271f;

        // Compute b_n as Wikipedia (maybe more precise)
        float sersicIndexTwo = sersicIndex * sersicIndex;

        float bn = 2*sersicIndex - 1f/3f + 4f/(405f*sersicIndex) + 46f/(25515f*sersicIndexTwo) + 
                    131f/(1148175f*sersicIndexTwo*sersicIndex) - 2194697f/(30690717750f*sersicIndexTwo*sersicIndexTwo);
        
        float ratio = Mathf.Pow(Mathf.Sqrt((q * (rotatedX * rotatedX) + (rotatedY * rotatedY) / q)) / halfLightRadius, 1/sersicIndex);
        float result = amp * Mathf.Exp(-bn * (ratio - 1f));

        if (log10) Mathf.Log10(result);

        return result;
    }
}
