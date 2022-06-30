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
		return (vFComp * vFComp - vIComp * vIComp) / (2 * aComp);
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
	public static float GetDesiredVelocity(float deltaDComp, float deltaT, float aComp)
	{
		// d = vI * deltaT + 1/2 * a * deltaT ^ 2
		// -1/2 * a * deltaT ^ 2 + d = vI * deltaT
		// vI = (d / deltaT) - (1/2 * a * deltaT)
		return (float)((deltaDComp / deltaT) - ((1.0 / 2.0) * aComp * deltaT));
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
		//Debug.Log($"{vIComp} * {deltaTime} + ({1.0 / 2.0}) * {accelComp} * {deltaTime} ^ 2 = {(1.0 / 2.0) * accelComp * deltaTime * deltaTime}");
		return (float)(vIComp * deltaTime + (1.0 / 2.0) * accelComp * deltaTime * deltaTime);
	}
}
