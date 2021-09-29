using UnityEngine;
using System.Collections;

public static class MyMath
{

//    public static bool CircleRectangleIntersectTest(Rect r, Circle c)
//    {
//        float cx = Math.abs(c.x - r.x - r.width/2.0f);
//        float xDist = r.halfWidth + c.radius;
//        if (cx > xDist)
//            return false;
//        float cy = Math.abs(c.y - r.y - r.halfHeight);
//        float yDist = r.halfHeight + c.radius;
//        if (cy > yDist)
//            return false;
//        if (cx <= r.halfWidth || cy <= r.halfHeight)
//            return true;
//        float xCornerDist = cx - r.halfWidth;
//        float yCornerDist = cy - r.halfHeight;
//        float xCornerDistSq = xCornerDist * xCornerDist;
//        float yCornerDistSq = yCornerDist * yCornerDist;
//        float maxCornerDistSq = c.radius * c.radius;
//        return xCornerDistSq + yCornerDistSq <= maxCornerDistSq;
//    }

    public static bool CircleRectangleIntersectTest(Rect rect, Vector2 circleCenter, float circleRadius)
    {
        float DeltaX = circleCenter.x - Mathf.Max(rect.x, Mathf.Min(circleCenter.x, rect.x + rect.width));
        float DeltaY = circleCenter.y - Mathf.Max(rect.y, Mathf.Min(circleCenter.y, rect.y + rect.height));
        return (DeltaX * DeltaX + DeltaY * DeltaY) < (circleRadius * circleRadius);
    }

	public static float AngleBtwVectors(Vector3 FirstVector, Vector3 SecondVector)
	{
		float AngleFirstVector = 57.3f * (Mathf.Atan2(FirstVector.y,FirstVector.x));
        Debug.Log(FirstVector.x + "," + FirstVector.z + " : " + AngleFirstVector);
		float AngleSecondVector = 57.3f * (Mathf.Atan2(SecondVector.y,SecondVector.x));
		//if(AngleFirstVector<0.0f)
		//	AngleFirstVector = AngleFirstVector + 360.0f;
		//if(AngleSecondVector<0.0f)
		//	AngleSecondVector = AngleSecondVector + 360.0f;
		float AngleBtw = AngleSecondVector - AngleFirstVector;
		//if(AngleSecondVector<AngleFirstVector)
		//	AngleBtw = AngleBtw + 360.0f;
		return AngleBtw;
	}

	public static float NormalizedAngle(float inputAngle)
	{
		if(inputAngle>180)
			inputAngle = inputAngle - 360;
		else if(inputAngle<=-180)
			inputAngle = inputAngle + 360;
		
		return inputAngle;
	}

    /// <summary>
    /// Ensure that the angle is within -180 to 180 range.
    /// </summary>

    [System.Diagnostics.DebuggerHidden]
    [System.Diagnostics.DebuggerStepThrough]
    static public float WrapAngle (float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }

    /// <summary>
    /// Wrap the index using repeating logic, so that for example +1 past the end means index of '1'.
    /// </summary>

    [System.Diagnostics.DebuggerHidden]
    [System.Diagnostics.DebuggerStepThrough]
    static public int RepeatIndex (int val, int max)
    {
        if (max < 1) return 0;
        while (val < 0) val += max;
        while (val >= max) val -= max;
        return val;
    }

    /// <summary>
    /// This code is not framerate-independent:
    /// 
    /// target.position += velocity;
    /// velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * 9f);
    /// 
    /// But this code is:
    /// 
    /// target.position += NGUIMath.SpringDampen(ref velocity, 9f, Time.deltaTime);
    /// </summary>

    static public Vector3 SpringDampen (ref Vector3 velocity, float strength, float deltaTime)
    {
        if (deltaTime > 1f) deltaTime = 1f;
        float dampeningFactor = 1f - strength * 0.001f;
        int ms = Mathf.RoundToInt(deltaTime * 1000f);
        float totalDampening = Mathf.Pow(dampeningFactor, ms);
        Vector3 vTotal = velocity * ((totalDampening - 1f) / Mathf.Log(dampeningFactor));
        velocity = velocity * totalDampening;
        return vTotal * 0.06f;
    }

    /// <summary>
    /// Same as the Vector3 version, it's a framerate-independent Lerp.
    /// </summary>

    static public Vector2 SpringDampen (ref Vector2 velocity, float strength, float deltaTime)
    {
        if (deltaTime > 1f) deltaTime = 1f;
        float dampeningFactor = 1f - strength * 0.001f;
        int ms = Mathf.RoundToInt(deltaTime * 1000f);
        float totalDampening = Mathf.Pow(dampeningFactor, ms);
        Vector2 vTotal = velocity * ((totalDampening - 1f) / Mathf.Log(dampeningFactor));
        velocity = velocity * totalDampening;
        return vTotal * 0.06f;
    }

    /// <summary>
    /// Calculate how much to interpolate by.
    /// </summary>

    static public float SpringLerp (float strength, float deltaTime)
    {
        if (deltaTime > 1f) deltaTime = 1f;
        int ms = Mathf.RoundToInt(deltaTime * 1000f);
        deltaTime = 0.001f * strength;
        float cumulative = 0f;
        for (int i = 0; i < ms; ++i) cumulative = Mathf.Lerp(cumulative, 1f, deltaTime);
        return cumulative;
    }

    /// <summary>
    /// Mathf.Lerp(from, to, Time.deltaTime * strength) is not framerate-independent. This function is.
    /// </summary>

    static public float SpringLerp (float from, float to, float strength, float deltaTime)
    {
        if (deltaTime > 1f) deltaTime = 1f;
        int ms = Mathf.RoundToInt(deltaTime * 1000f);
        deltaTime = 0.001f * strength;
        for (int i = 0; i < ms; ++i) from = Mathf.Lerp(from, to, deltaTime);
        return from;
    }

    /// <summary>
    /// Vector2.Lerp(from, to, Time.deltaTime * strength) is not framerate-independent. This function is.
    /// </summary>

    static public Vector2 SpringLerp (Vector2 from, Vector2 to, float strength, float deltaTime)
    {
        return Vector2.Lerp(from, to, SpringLerp(strength, deltaTime));
    }

    /// <summary>
    /// Vector3.Lerp(from, to, Time.deltaTime * strength) is not framerate-independent. This function is.
    /// </summary>

    static public Vector3 SpringLerp (Vector3 from, Vector3 to, float strength, float deltaTime)
    {
        return Vector3.Lerp(from, to, SpringLerp(strength, deltaTime));
    }

    /// <summary>
    /// Quaternion.Slerp(from, to, Time.deltaTime * strength) is not framerate-independent. This function is.
    /// </summary>

    static public Quaternion SpringLerp (Quaternion from, Quaternion to, float strength, float deltaTime)
    {
        return Quaternion.Slerp(from, to, SpringLerp(strength, deltaTime));
    }

    /// <summary>
    /// Since there is no Mathf.RotateTowards...
    /// </summary>

    static public float RotateTowards (float from, float to, float maxAngle)
    {
        float diff = WrapAngle(to - from);
        if (Mathf.Abs(diff) > maxAngle) diff = maxAngle * Mathf.Sign(diff);
        return from + diff;
    }

    /// <summary>
    /// Determine the distance from the specified point to the line segment.
    /// </summary>

    static float DistancePointToLineSegment (Vector2 point, Vector2 a, Vector2 b)
    {
        float l2 = (b - a).sqrMagnitude;
        if (l2 == 0f) return (point - a).magnitude;
        float t = Vector2.Dot(point - a, b - a) / l2;
        if (t < 0f) return (point - a).magnitude;
        else if (t > 1f) return (point - b).magnitude;
        Vector2 projection = a + t * (b - a);
        return (point - projection).magnitude;
    }

    /// <summary>
    /// Determine the distance from the mouse position to the screen space rectangle specified by the 4 points.
    /// </summary>

    static public float DistanceToRectangle (Vector2[] screenPoints, Vector2 mousePos)
    {
        bool oddNodes = false;
        int j = 4;

        for (int i = 0; i < 5; i++)
        {
            Vector3 v0 = screenPoints[MyMath.RepeatIndex(i, 4)];
            Vector3 v1 = screenPoints[MyMath.RepeatIndex(j, 4)];

            if ((v0.y > mousePos.y) != (v1.y > mousePos.y))
            {
                if (mousePos.x < (v1.x - v0.x) * (mousePos.y - v0.y) / (v1.y - v0.y) + v0.x)
                {
                    oddNodes = !oddNodes;
                }
            }
            j = i;
        }

        if (!oddNodes)
        {
            float dist, closestDist = -1f;

            for (int i = 0; i < 4; i++)
            {
                Vector3 v0 = screenPoints[i];
                Vector3 v1 = screenPoints[MyMath.RepeatIndex(i + 1, 4)];

                dist = DistancePointToLineSegment(mousePos, v0, v1);

                if (dist < closestDist || closestDist < 0f) closestDist = dist;
            }
            return closestDist;
        }
        else return 0f;
    }

    /// <summary>
    /// Determine the distance from the mouse position to the world rectangle specified by the 4 points.
    /// </summary>

    static public float DistanceToRectangle (Vector3[] worldPoints, Vector2 mousePos, Camera cam)
    {
        Vector2[] screenPoints = new Vector2[4];
        for (int i = 0; i < 4; ++i)
            screenPoints[i] = cam.WorldToScreenPoint(worldPoints[i]);
        return DistanceToRectangle(screenPoints, mousePos);
    }

    /// <summary>
    /// Adjust the specified value by DPI: height * 96 / DPI.
    /// This will result in in a smaller value returned for higher pixel density devices.
    /// </summary>

    static public int AdjustByDPI (float height)
    {
        float dpi = Screen.dpi;

        RuntimePlatform platform = Application.platform;

        if (dpi == 0f)
        {
            dpi = (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer) ? 160f : 96f;
            #if UNITY_BLACKBERRY
            if (platform == RuntimePlatform.BB10Player) dpi = 160f;
            #elif UNITY_WP8 || UNITY_WP_8_1
            if (platform == RuntimePlatform.WP8Player) dpi = 160f;
            #endif
        }

        int h = Mathf.RoundToInt(height * (96f / dpi));
        if ((h & 1) == 1) ++h;
        return h;
    }

    /// <summary>
    /// Convert the specified world point from one camera's world space to another, then make it relative to the specified transform.
    /// You should use this function if you want to position a widget using some 3D point in space.
    /// Pass your main camera for the "worldCam", and your UI camera for "uiCam", then the widget's transform for "relativeTo".
    /// You can then assign the widget's localPosition to the returned value.
    /// </summary>

    static public Vector3 WorldToLocalPoint (Vector3 worldPos, Camera worldCam, Camera uiCam, Transform relativeTo)
    {
        worldPos = worldCam.WorldToViewportPoint(worldPos);
        worldPos = uiCam.ViewportToWorldPoint(worldPos);
        if (relativeTo == null) return worldPos;
        relativeTo = relativeTo.parent;
        if (relativeTo == null) return worldPos;
        return relativeTo.InverseTransformPoint(worldPos);
    }

    /// <summary>
    /// Helper function used to make the vector use integer numbers.
    /// </summary>

    static public Vector3 Round (Vector3 v)
    {
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        v.z = Mathf.Round(v.z);
        return v;
    }

    static public float EaseOutElastic(float t)
    {
        t = Mathf.Clamp01(t);
        t = (t / 1.036f) + 0.036f;
        if (t == 0.0f || t==1.0f)
            return t;
        return 1.0f + Mathf.Sin(10.0f * (t - 1.8f)) * .04f * (t-1.0f) / t;

        //t = (t / 1.03f) + 0.03f;
        //if (t == 0.0f || t == 1.0f)
        //    return t;
        //return 1.0f + Mathf.Sin(25.0f * (t - 1.0f)) * .04f * (t - 1.0f) / t;
    }

    static public float EaseInQuad(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t;
    }

    static public float EaseInOutQuad(float t)
    {
        t = Mathf.Clamp01(t);
        float sqt = t * t;
        return sqt / (2.0f * (sqt - t) + 1.0f);
    }

    static public float EaseInOutSine(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t * (3.0f - 2.0f * t);
    }

    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Mathf.Clamp01(Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB));
    }

    public static float InverseLerp(Vector2 a, Vector2 b, Vector2 value)
    {
        Vector2 AB = b - a;
        Vector2 AV = value - a;
        return Mathf.Clamp01(Vector2.Dot(AV, AB) / Vector2.Dot(AB, AB));
    }









    //-----------------------------------------------------------------------------
    // Name: CylTest_CapsFirst
    // Orig: Greg James - gjames@NVIDIA.com
    // Lisc: Free code - no warranty & no money back.  Use it all you want
    // Desc: 
    //    This function tests if the 3D point 'testpt' lies within an arbitrarily
    // oriented cylinder.  The cylinder is defined by an axis from 'pt1' to 'pt2',
    // the axis having a length squared of 'lengthsq' (pre-compute for each cylinder
    // to avoid repeated work!), and radius squared of 'radius_sq'.
    //    The function tests against the end caps first, which is cheap -> only 
    // a single dot product to test against the parallel cylinder caps.  If the
    // point is within these, more work is done to find the distance of the point
    // from the cylinder axis.
    //    Fancy Math (TM) makes the whole test possible with only two dot-products
    // a subtract, and two multiplies.  For clarity, the 2nd mult is kept as a
    // divide.  It might be faster to change this to a mult by also passing in
    // 1/lengthsq and using that instead.
    //    Elminiate the first 3 subtracts by specifying the cylinder as a base
    // point on one end cap and a vector to the other end cap (pass in {dx,dy,dz}
    // instead of 'pt2' ).
    //
    //    The dot product is constant along a plane perpendicular to a vector.
    //    The magnitude of the cross product divided by one vector length is
    // constant along a cylinder surface defined by the other vector as axis.
    //
    // Return:  -1.0 if point is outside the cylinder
    // Return:  distance squared from cylinder axis if point is inside.
    //
    //-----------------------------------------------------------------------------

    public static float IsInsideCylinder (Vector3 pt1, Vector3 pt2, float lengthsq, float radius_sq, Vector3 testpt )
	{
		float dx, dy, dz;	// vector d  from line segment point 1 to point 2
		float pdx, pdy, pdz;	// vector pd from point 1 to test point
		float dot, dsq;
		
		dx = pt2.x - pt1.x;	// translate so pt1 is origin.  Make vector from
		dy = pt2.y - pt1.y;     // pt1 to pt2.  Need for this is easily eliminated
		dz = pt2.z - pt1.z;
		
		pdx = testpt.x - pt1.x;		// vector from pt1 to test point.
		pdy = testpt.y - pt1.y;
		pdz = testpt.z - pt1.z;
		
		// Dot the d and pd vectors to see if point lies behind the 
		// cylinder cap at pt1.x, pt1.y, pt1.z
		
		dot = pdx * dx + pdy * dy + pdz * dz;
		
		// If dot is less than zero the point is behind the pt1 cap.
		// If greater than the cylinder axis line segment length squared
		// then the point is outside the other end cap at pt2.
		
		if( dot < 0.0f || dot > lengthsq )
		{
			return( -1.0f );
		}
		else 
		{
			// Point lies within the parallel caps, so find
			// distance squared from point to line, using the fact that sin^2 + cos^2 = 1
			// the dot = cos() * |d||pd|, and cross*cross = sin^2 * |d|^2 * |pd|^2
			// Carefull: '*' means mult for scalars and dotproduct for vectors
			// In short, where dist is pt distance to cyl axis: 
			// dist = sin( pd to d ) * |pd|
			// distsq = dsq = (1 - cos^2( pd to d)) * |pd|^2
			// dsq = ( 1 - (pd * d)^2 / (|pd|^2 * |d|^2) ) * |pd|^2
			// dsq = pd * pd - dot * dot / lengthsq
			//  where lengthsq is d*d or |d|^2 that is passed into this function 
			
			// distance squared to the cylinder axis:
			
			dsq = (pdx*pdx + pdy*pdy + pdz*pdz) - dot*dot/lengthsq;
			
			if( dsq > radius_sq )
			{
				return( -1.0f );
			}
			else
			{
				return( dsq );		// return distance squared to axis
			}
		}
	}

    private static float SignedTriangleArea(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    public static bool PointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        float d1, d2, d3;
        bool has_neg, has_pos;

        d1 = SignedTriangleArea(pt, v1, v2);
        d2 = SignedTriangleArea(pt, v2, v3);
        d3 = SignedTriangleArea(pt, v3, v1);

        has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(has_neg && has_pos);
    }
}
