using System.Collections.Generic;
using Muco;
using UnityEngine;
using UnityEngine.XR;

public class DragWorldJoystick : MonoBehaviour
{
    public DragWorld dragWorld;
    public Transform fwdAxis;
    public Transform rightAxis;
    public VrInputData vrInputData;

    public bool rightZeroOnY = true;
    public bool isFlying = false;

    public float moveSpeed = 3f;
    bool flyingButtonWasPressed;
    bool resetPositionButtonWasPressed;

    private void Update() {
        var devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        var debugString = "";
        foreach (var device in devices) {
            debugString += device.name + ":\n";
            debugString += device.characteristics + "\n";
            debugString += "\n";
        }
        VrDebug.SetValue("Devices", "Devices", debugString);

        vrInputData.LCtrl.TryGetFeatureValue(CommonUsages.triggerButton, out bool resetPositionButtonIsPressedLeft);
        vrInputData.RCtrl.TryGetFeatureValue(CommonUsages.triggerButton, out bool resetPositionButtonIsPressedRight);
        var resetPositionButtonIsPressed = resetPositionButtonIsPressedLeft || resetPositionButtonIsPressedRight;

        var resetPositionButtonDown = resetPositionButtonIsPressed && !resetPositionButtonWasPressed;
        resetPositionButtonWasPressed = resetPositionButtonIsPressed;

        vrInputData.LCtrl.TryGetFeatureValue(CommonUsages.triggerButton, out bool flyingButtonIsPressedLeft);
        vrInputData.RCtrl.TryGetFeatureValue(CommonUsages.triggerButton, out bool flyingButtonIsPressedRight);
        var flyingButtonIsPressed = flyingButtonIsPressedLeft || flyingButtonIsPressedRight;

        var flyingButtonDown = flyingButtonIsPressed && !flyingButtonWasPressed;
        flyingButtonWasPressed = flyingButtonIsPressed;

        if (resetPositionButtonDown)
        {
            dragWorld.ResetPosition();
        }

        if (flyingButtonDown)
        {
            isFlying = !isFlying;
        }

        // var axis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick)
        //          + OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        Vector2 axisLeft;
        vrInputData.LCtrl.TryGetFeatureValue(CommonUsages.primary2DAxis, out axisLeft);
        Vector2 axisRight;
        vrInputData.RCtrl.TryGetFeatureValue(CommonUsages.primary2DAxis, out axisRight);
        var axis = axisLeft + axisRight;

        var right = rightAxis.right * axis.x;
        if (rightZeroOnY)
            right.y = 0;

        var forward = fwdAxis.forward;
        if (!isFlying)
        {
            forward.y = 0f;
        }
        forward = forward.normalized * axis.y;

        var dir = (forward + right);
        var vel = dir * moveSpeed;
        var delta = vel * Time.deltaTime;

        dragWorld.MoveFPS(delta);
    }
}
