using UnityEngine;

public class DragWorld : MonoBehaviour {
    static DragWorld _dragWorld;
    public static DragWorld TheDragWorld {
        get {
            if(_dragWorld != null)
                return _dragWorld;
            var dragWorld = FindFirstObjectByType<DragWorld>();
            if (dragWorld == null)
                Debug.Log("Could not find DragWorld");
            _dragWorld = dragWorld;
            return dragWorld;
        }
    }
    public Vector3 vel;
    public bool isSliding;
    public float pitch;
    public float yaw;

    public float maxSpeed = 4;
    public float maxDistFromOrigin = 20;

    public bool enableDrag;

    void Update()
    {
        if (!enableDrag)
            return;

        if (transform.localPosition.magnitude > maxDistFromOrigin)
            isSliding = false;
        
        transform.localPosition += vel * Time.deltaTime;

        if (isSliding)
        {
            var speed = vel.magnitude;

            if (speed > maxSpeed)
                speed = maxSpeed;

            if (speed > 1.0)
            {
                vel *= Mathf.Pow(0.5f, Time.deltaTime / 2.0f);
            }
            else if (speed > 0.1)
            {
                vel *= Mathf.Pow(0.5f, Time.deltaTime / 0.5f);
            }
            else
            {
                isSliding = false;
            }
        }

        if (!isSliding)
            vel = Vector3.zero;
    }

    public void Drag(Vector3 delta)
    {
        if (!enableDrag)
            return;

        vel = delta * (1.0f / Time.deltaTime);
        isSliding = true;
    }

    public void MoveFPS(Vector3 delta)
    {
        if (!enableDrag)
            return;

        vel += transform.rotation * delta * (1.0f / Time.deltaTime);
        isSliding = false;
    }

    public void RotateFps(float deltaPitch, float deltaYaw)
    {
        if (!enableDrag)
            return;

        pitch += deltaPitch;
        yaw += deltaYaw;

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }

    public void ResetPosition()
    {
        if (!enableDrag)
            return;

        transform.position = Vector3.zero;
    }
}
