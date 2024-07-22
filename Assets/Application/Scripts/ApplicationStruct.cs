using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace ApplicationStruct {

	[System.Serializable]
	public class EventFloat : UnityEvent<float>{}
	[System.Serializable]
	public class EventInt : UnityEvent<int>{ }
	[System.Serializable]
	public class EventUInt : UnityEvent<uint> { }
	[System.Serializable]
	public class EventLong : UnityEvent<long>{ }
	[System.Serializable]
	public class EventULong : UnityEvent<ulong> { }
	[System.Serializable]
	public class EventString : UnityEvent<string>{}
	[System.Serializable]
	public class EventColor : UnityEvent<Color>{}
	[System.Serializable]
	public class EventBool : UnityEvent<bool>{}
	[System.Serializable]
	public class EventOrientation : UnityEvent<DeviceOrientation> { }
	[System.Serializable]
	public class EventVector2 : UnityEvent<Vector2> { }
	[System.Serializable]
	public class EventVector3 : UnityEvent<Vector3> { }
	[System.Serializable]
	public class EventVector4 : UnityEvent<Vector4> { }
	[System.Serializable]
	public class EventRay : UnityEvent<Ray> { }
}

static class AppExtensions{
	public static int getCountsOfDigits(this long number) {
		return(number == 0) ? 1 : (int) Mathf.Ceil(Mathf.Log10(Mathf.Abs(number) + 0.5f));
	}

	public static Vector3 GetPointByDirectionTime(this Vector3 startPos, Vector3 velocity, float time)
	{
		return GetPointByDirectionTime(startPos, velocity, Physics.gravity, time);
	}
	public static Vector3 GetPointByDirectionTime(this Vector3 startPos, Vector3 velocity, Vector3 gravity, float time)
	{
		return startPos + velocity * time + gravity * time * time / 2f;
	}
	public static void GetPrediction(this Vector3 startPos, Vector3 forceDirection,float dt,float drag,float mass,ref Vector3[] array)
	{
		float timestep = dt;
		float stepDrag = 1f - drag * timestep;
		Vector3 velocity = forceDirection * timestep;
		Vector3 gravity = Physics.gravity * timestep * timestep;
		Vector3 position = startPos;


		for(int i=0;i < array.Length;i++)
		{
			array[i] = position;

			velocity += gravity;
			velocity *= stepDrag;

			position += velocity;
		}
	}
	public static T GetRandomInst<T>(this IEnumerable<T> randomArray,Vector3 pos, Quaternion qRot) where T: UnityEngine.Object
	{
		T rand = randomArray.GetRandom();
		rand = MonoBehaviour.Instantiate(rand, pos, qRot);
		return rand;
	}
	public static T GetRandom<T>(this IEnumerable<T> randomArray)
	{
		return randomArray.ElementAt(UnityEngine.Random.Range(0, randomArray.Count()));
	}
	public static void Destroy(this Object gObject)
	{
		MonoBehaviour.Destroy(gObject);
	}
	public static void Destroy(this Object gObject,float time)
	{
		MonoBehaviour.Destroy(gObject, time);
	}
	public static T Clone<T>(this T item,Transform parent=null) where T : UnityEngine.Object
	{
		if (parent)
			return MonoBehaviour.Instantiate(item, parent, false);
		else
			return MonoBehaviour.Instantiate(item);
	}
}
