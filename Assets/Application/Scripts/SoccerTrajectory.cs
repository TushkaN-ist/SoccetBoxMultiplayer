using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SoccerTrajectory : NetworkBehaviour
{
	[SerializeField]
	Rigidbody _rigidbody;

	[SerializeField]
	AnimationCurve _power;

	[SerializeField]
	Vector3 _targetVelocity;

	float _spawnTime = 0;

	[SerializeField]
	float liveTime = 15;

	public NetworkPlayerSoccerField field;

	[Server]
	public void SetTraectory(Vector3 velocity, Vector3 trajectory)
	{
		_rigidbody.velocity = velocity;
		_targetVelocity = trajectory;
	}

	public override void OnStartClient()
	{
		if (!isOwned && !isServer)
			enabled = false;
	}
	public override void OnStartServer()
	{
		enabled = true;
		_spawnTime = Time.time;
		Invoke(nameof(DestroySelf), liveTime);
	}
	private void OnDestroy()
	{
		CancelInvoke();
	}

	void DestroySelf()
	{
		Destroy(gameObject);
	}

	private void FixedUpdate()
	{
		_rigidbody.AddForce(_targetVelocity * _power.Evaluate(Time.time - _spawnTime), ForceMode.Acceleration);
	}
	[Server]
	private void OnTriggerEnter(Collider other)
	{
		if (NetworkPlayerSoccerField.GoalTrigged(other, field))
		{
			Destroy(gameObject);
		}
	}
}
