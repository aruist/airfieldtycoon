using UnityEngine;
using System.Collections;

public class SmoothScroller : MonoBehaviour {
	private enum MovementTypes {
		Elastic,
		Clamped
	}
	public Transform trToScroll;
	public Camera targetCamera;
	public bool isActive;
	[SerializeField]
	private MovementTypes m_movementType;
	public bool horizontal;
	public bool vertical;
	public Vector3 minPos = new Vector3(-2.5f, -6.5f, 0f);
	public Vector3 maxPos = new Vector3(2.5f, 7.3f, 0f);
	[SerializeField]
	private float m_DecelerationRate = 0.135f;
	public bool useCamZoom;
	public Camera zoomCamera;
	public float zoomMin = 2.0f;
	public float zoomMax = 10f;
	public float zoomSpeed = 1.5f;

	private bool m_Zooming = false;

	// Touch controls:
	private Touch m_touch0;
	private Touch m_touch1;
	public float camMinSize = 2.0f;
	public float camMaxSize = 10.0f;
	private Vector2 prevMousePos;
	private Vector3 m_CurrentPointerPos;
	private Vector3 m_ContentStartPosition = Vector3.zero;
	private bool m_Dragging = true;
	public Vector2 m_Velocity = Vector2.zero;
	private Vector3 m_PrevPosition = Vector3.zero;

	// Update is called once per frame
	void Update () {
		if (!isActive)
			return;

		if (useCamZoom) {
			CheckZoom();
		}

		if (!m_Zooming) {
			CheckScrolling();
		}
	
	}

	void LateUpdate() {
		float deltaTime = Time.unscaledDeltaTime;
		if (!m_Dragging && m_Velocity != Vector2.zero) {
			Vector3 position = trToScroll.position;
			
			for (int axis = 0; axis < 2; axis++) {
				m_Velocity[axis] *= Mathf.Pow(m_DecelerationRate, deltaTime);
				if (Mathf.Abs(m_Velocity[axis]) < 0.51f)
					m_Velocity[axis] = 0f;
				position[axis] += m_Velocity[axis] * deltaTime;
			}
			
			if (m_Velocity != Vector2.zero) {
				Vector3 newPosition = position;
				if (horizontal)
					newPosition.x = Mathf.Clamp(position.x, minPos.x, maxPos.x );
				else 
					newPosition.x = trToScroll.position.x;
				if (vertical)
					newPosition.y = Mathf.Clamp(position.y, minPos.y, maxPos.y );
				else 
					newPosition.y = trToScroll.position.y;
				if (newPosition.x != position.x)
					m_Velocity.x = 0f;
				if (newPosition.y != position.y)
					m_Velocity.y = 0f;
				trToScroll.position = newPosition;
			}
		}
		
		if (m_Dragging) {
			Vector3 newVelocity = (trToScroll.position - m_PrevPosition) / deltaTime;
			m_Velocity = Vector3.Lerp(m_Velocity, newVelocity, deltaTime * 10);
		}
		
		if (trToScroll.position != m_PrevPosition) {
			m_PrevPosition = trToScroll.position;
		}
	}

	private void InputBeingDrag(Vector3 pos) {
		//Debug.Log("InputBeingDrag " + pos.ToString());
		m_ContentStartPosition = pos;
		m_Dragging = true;
		m_Velocity = Vector2.zero;
	}
	
	private void InputEndDrag(Vector3 pos) {
		//Debug.Log("InputEndDrag " + pos.ToString());
		m_Dragging = false;
	}
	
	private void InputDrag(Vector3 pos) {
		Vector3 pointerDelta = pos - trToScroll.position;
		Vector3 newPosition = m_ContentStartPosition - pointerDelta;
		if (horizontal)
			newPosition.x = Mathf.Clamp(newPosition.x, minPos.x, maxPos.x );
		else
			newPosition.x = trToScroll.position.x;

		if (vertical)
			newPosition.y = Mathf.Clamp(newPosition.y, minPos.y, maxPos.y );
		else
			newPosition.y = trToScroll.position.y;
		trToScroll.position = newPosition;
	}


	private void CheckZoom() {
		// Zoom:
#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(1)) {
			prevMousePos = Input.mousePosition;
		}
		else if (Input.GetMouseButton(1)) {
			Vector2 touch0PrevPos = Input.mousePosition;
			float mag = (touch0PrevPos.y - prevMousePos.y);//.magnitude;
			float size = zoomCamera.orthographicSize + mag * zoomSpeed * Time.deltaTime;
			zoomCamera.orthographicSize = Mathf.Clamp(size, camMinSize, camMaxSize);
			
			prevMousePos = Input.mousePosition;
		}
#endif
		
		if (m_Zooming && Input.touchCount < 2) {
			if (Input.touchCount == 0)
				m_Zooming = false;
		}
		if (Input.touchCount == 2) {
			m_Zooming = true;
			m_touch0 = Input.GetTouch(0);
			m_touch1 = Input.GetTouch(1);
			Vector2 touch0PrevPos = m_touch0.position - m_touch0.deltaPosition;
			Vector2 touch1PrevPos = m_touch1.position - m_touch1.deltaPosition;
			float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
			float touchdeltaMag = (m_touch0.position - m_touch1.position).magnitude;
			float deltaMagnutudeDiff = prevTouchDeltaMag - touchdeltaMag;
			float size = zoomCamera.orthographicSize + deltaMagnutudeDiff * zoomSpeed * Time.deltaTime;
			zoomCamera.orthographicSize = Mathf.Clamp(size, camMinSize, camMaxSize);
		}
	}

	public void CheckScrolling() {
		bool handled = false;
		if (Input.touchCount == 1) {
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began) {
				handled = true;
				InputBeingDrag(targetCamera.ScreenToWorldPoint(touch.position));
			}
			else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
				handled = true;
				InputEndDrag(targetCamera.ScreenToWorldPoint(touch.position));
			}
			else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
				handled = true;
				InputDrag(targetCamera.ScreenToWorldPoint(touch.position));
			}
		}
		if (!handled) {
			if (Input.GetMouseButtonDown(0)) {
				InputBeingDrag(targetCamera.ScreenToWorldPoint(Input.mousePosition));
			}
			else if (Input.GetMouseButtonUp(0)) {
				InputEndDrag(targetCamera.ScreenToWorldPoint(Input.mousePosition));
			}
			else if (Input.GetMouseButton(0)) {
				InputDrag(targetCamera.ScreenToWorldPoint(Input.mousePosition));
			}
		}
	}
}
