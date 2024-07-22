using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoccerMovement : MonoBehaviour
{
	[SerializeField]
	Camera _camera;

	[SerializeField]
	Collider _collider;

	[SerializeField]
	ApplicationStruct.EventBool OnActive;

	[SerializeField]
	float _sentesivity = 1f, _friction = 2f, _frictionShoot = 2f, _angle = 1f, _powerShoot = 150, _speedMovement = 2;
	[SerializeField]
	AnimationCurve _curveShoot;

	float _currentTimeShoot;

	Vector2 _move, _movePosition;

	[SerializeField]
	LineRenderer _lineRenderer;

	[SerializeField]
	Rigidbody _body;

	[SerializeField]
	Transform _launchPoint;

	public System.Action<Vector4> OnLaunch;

	public Transform LaunchPoint => _launchPoint;

	private void OnEnable()
	{
		OnActive.Invoke(true);

		Cursor.lockState = CursorLockMode.Locked;
	}
	private void OnDisable()
	{
		OnActive.Invoke(false);
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			_currentTimeShoot = Time.time;
			_lineRenderer.enabled = true;
		}

		if (Input.GetMouseButtonUp(0))
		{
			float power = _curveShoot.Evaluate(Time.time - _currentTimeShoot);
			Vector3 trajectory = (transform.right * _move.x + transform.up * _move.y) * _angle * _powerShoot * power;

			OnLaunch?.Invoke(new Vector4(trajectory.x, trajectory.y, trajectory.z, _powerShoot * power));

			_move = _movePosition;
			_lineRenderer.enabled = false;
		}

		_move -= _move * Time.deltaTime * _friction;
		_move += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Time.deltaTime * _sentesivity / Screen.width;

		if (!Input.GetMouseButton(0))
		{
			_movePosition = _move;
		}
		else
		{
			_movePosition -= _movePosition * Time.deltaTime * _frictionShoot;

			float power = _curveShoot.Evaluate(Time.time - _currentTimeShoot);
			Vector3 trajectory = (transform.right * _move.x + transform.up * _move.y) * _angle * _powerShoot;

			float p = _curveShoot.Evaluate(Time.time - _currentTimeShoot);
			Vector3[] points = new Vector3[10];
			for (int i = 0; i < points.Length; i++)
			{
				points[i] = LaunchPoint.position.GetPointByDirectionTime(transform.forward * _powerShoot * power, Physics.gravity + trajectory * power, i / (float)points.Length);
			}
			_lineRenderer.SetPositions(points);
		}
		_collider.enabled = true;
		Vector3 newPostion = _collider.ClosestPoint(transform.position + _collider.transform.rotation * _movePosition);
		_collider.enabled = false;
		transform.position = newPostion;
		float controlMove = Input.GetAxis("Horizontal");
		if (Mathf.Abs(controlMove) > float.Epsilon)
		{
			_body.AddForce(_collider.transform.right * controlMove * _speedMovement, ForceMode.VelocityChange);
		}
		transform.rotation = Quaternion.LookRotation(_collider.transform.forward + (_collider.transform.up * _movePosition.y + _collider.transform.right * _movePosition.x) * _angle, _collider.transform.up);
		
	}

}
