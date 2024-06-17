using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class AnimatedHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchAnimatitonAction;
    public InputActionProperty gripAnimatitonAction;
    public Animator handAnimator;

    // Update is called once per frame
    void Update()
    {
        float triggerValue = pinchAnimatitonAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);

        float gripValue = gripAnimatitonAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);
    }
}
