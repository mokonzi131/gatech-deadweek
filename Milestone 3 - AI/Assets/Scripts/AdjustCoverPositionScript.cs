using UnityEngine;
using System.Collections;

public class AdjustCoverPositionScript : MonoBehaviour {

	Transform _transform;
	Transform _player;
	Transform _cover;

	public float dist_offset = 1.5f;
	public float y_offset = -1.0f;
	public void AdjustCoverPosition()
	{
		_transform = GetComponent<Transform>();
		_cover = _transform.Find ("Cover");
		if (_cover == null)
			Debug.LogError("No cover component attached to this tree!");
		
		_player = GameObject.Find("Player").GetComponent<Transform>();
		
		Vector3 direction = (_transform.position - _player.position);
		direction.y = 0;
		direction.Normalize ();


		_cover.position = _transform.position + dist_offset * direction + new Vector3(0, y_offset, 0);
		
		_cover.LookAt (_player.position);

	}

}
