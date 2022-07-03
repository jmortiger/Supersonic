using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyPhysicsHelper
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="vIComp">Initial Velocity Component</param>
	/// <param name="aComp">Acceleration Component</param>
	/// <param name="vFComp">Final Velocity Component; default = 0</param>
	/// <returns>The component change in location.</returns>
	/// <remarks>Uses formula <c>vFinal ^ 2 = vInitial ^ 2 + 2 * a * deltaY</c></remarks>
	public static float GetDisplacementComponent(float vIComp, float aComp, float vFComp = 0)
	{
		// vFinal ^ 2 = vInitial ^ 2 + 2 * a * deltaY
		// vFinal ^ 2 - vInitial ^ 2 = 2 * a * deltaY
		// (vFinal ^ 2 - vInitial ^ 2) / (2 * a) = deltaY
		// deltaY = (vFinal ^ 2 - vInitial ^ 2) / (2 * a)
		return (vFComp * vFComp - vIComp * vIComp) / (2f * aComp);
	}

	/// <summary>
	/// Get time to travel given distance at given average speed.
	/// </summary>
	/// <param name="deltaXComp">Displacement (component)</param>
	/// <param name="vAvgComp">Average Velocity (component)</param>
	/// <returns>Delta Time to travel distance <paramref name="deltaXComp"/>.</returns>
	public static float GetTravelTime(float deltaXComp, float vAvgComp)
	{
		return deltaXComp / vAvgComp;
	}

	/// <summary>
	/// Get velocity to travel given distance in given time with given acceleration.
	/// </summary>
	/// <param name="deltaDComp">The component distance to travel.</param>
	/// <param name="deltaT">The travel time.</param>
	/// <param name="aComp">The component acceleration.</param>
	/// <returns>The component velocity.</returns>
	public static float GetDesiredVelocityFromDistance(float deltaDComp, float deltaT, float aComp)
	{
		// d = vI * deltaT + 1/2 * a * deltaT ^ 2
		// -1/2 * a * deltaT ^ 2 + d = vI * deltaT
		// vI = (d / deltaT) - (1/2 * a * deltaT)
		return (float)((deltaDComp / deltaT) - ((1.0 / 2.0) * aComp * deltaT));
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="accelComp"></param>
	/// <param name="deltaT"></param>
	/// <param name="vFComp"></param>
	/// <returns></returns>
	public static float GetDesiredVelocity(float accelComp, float deltaT, float vFComp = 0f)
	{
		// vF = vI + at
		// vI = vF - at
		return vFComp - accelComp * deltaT;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="vIComp"></param>
	/// <param name="deltaTime"></param>
	/// <param name="accelComp"></param>
	/// <returns></returns>
	public static float GetDistanceTravelled(float vIComp, float deltaTime, float accelComp)
	{
		// d = vI * deltaT + 1/2 * a * deltaT ^ 2
		return (float)(vIComp * deltaTime + (1.0 / 2.0) * accelComp * deltaTime * deltaTime);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="vIComp"></param>
	/// <param name="accelComp"></param>
	/// <param name="deltaDComp"></param>
	/// <returns></returns>
	public static float GetVelocityFinalComponent(float vIComp, float accelComp, float deltaDComp)
	{
		// vF^2 = vI^2 + 2ad
		return Mathf.Sqrt(vIComp * vIComp + 2 * accelComp * deltaDComp);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="vIComp"></param>
	/// <param name="accelComp"></param>
	/// <param name="deltaT"></param>
	/// <returns></returns>
	public static float GetVelocityFinalComponentByTime(float vIComp, float accelComp, float deltaT)
	{
		// vF = vI + at
		return vIComp + accelComp * deltaT;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="accelComp"></param>
	/// <param name="deltaDComp"></param>
	/// <param name="vIComp"></param>
	/// <returns></returns>
	public static float GetInitialVelocityComponentFromDistance(float accelComp, float deltaDComp, float vFComp = 0f)
	{
		// vF^2 = vI^2 + 2ad
		// vI^2 = vF^2 - 2ad
		return vFComp * vFComp - 2 * accelComp * deltaDComp;
	}

	public static float GetInitialVelocityTrial(float deltaDComp, float deltaT, float vFComp = 0f)
	{
		return ((2 * deltaDComp) / deltaT) + vFComp;
	}
}
