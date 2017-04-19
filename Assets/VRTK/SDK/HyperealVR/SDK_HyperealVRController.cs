// HyperealVR Controller|SDK_HyperealVR|004
namespace VRTK
{
#if VRTK_DEFINE_SDK_HYPEREALVR
    using UnityEngine;
    using System.Collections.Generic;
    using Valve.VR;
    using Hypereal;
#endif

    /// <summary>
    /// The HyperealVR Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    [SDK_Description(typeof(SDK_HyperealVRSystem))]
    public class SDK_HyperealVRController
#if VRTK_DEFINE_SDK_HYPEREALVR
        : SDK_BaseController
#else
        : SDK_FallbackController
#endif
    {
#if VRTK_DEFINE_SDK_HYPEREALVR
        private VRTK_TrackedController cachedLeftController;
        private VRTK_TrackedController cachedRightController;
        private VRTK_TrackedController cachedLeftTrackedObject;
        private VRTK_TrackedController cachedRightTrackedObject;

        private bool[] previousHairTriggerState = new bool[2];
        private bool[] currentHairTriggerState = new bool[2];

        private bool[] previousHairGripState = new bool[2];
        private bool[] currentHairGripState = new bool[2];
        //private Dictionary<GameObject, HyperealVR_TrackedObject> cachedTrackedObjectsByGameObject = new Dictionary<GameObject, HyperealVR_TrackedObject>();
        //private Dictionary<uint, HyperealVR_TrackedObject> cachedTrackedObjectsByIndex = new Dictionary<uint, HyperealVR_TrackedObject>();
        

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="index">The index of the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(uint index, Dictionary<string, object> options)
        {
        }

        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="index">The index of the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public override void ProcessFixedUpdate(uint index, Dictionary<string, object> options)
        {
        }

        /// <summary>
        /// The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.
        /// </summary>
        /// <param name="hand">The controller hand to check for</param>
        /// <returns>A path to the resource that contains the collider GameObject.</returns>
        public override string GetControllerDefaultColliderPath(ControllerHand hand)
        {
            return "ControllerColliders/Fallback";
        }

        /// <summary>
        /// The GetControllerElementPath returns the path to the game object that the given controller element for the given hand resides in.
        /// </summary>
        /// <param name="element">The controller element to look up.</param>
        /// <param name="hand">The controller hand to look up.</param>
        /// <param name="fullPath">Whether to get the initial path or the full path to the element.</param>
        /// <returns>A string containing the path to the game object that the controller element resides in.</returns>
        public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)
        {
            return null;
        }

        /// <summary>
        /// The GetControllerIndex method returns the index of the given controller.
        /// </summary>
        /// <param name="controller">The GameObject containing the controller.</param>
        /// <returns>The index of the given controller.</returns>
        public override uint GetControllerIndex(GameObject controller)
        {
            var trackedObject = GetTrackedObject(controller);
            return (trackedObject ? (uint)trackedObject.index : uint.MaxValue);
        }

        /// <summary>
        /// The GetControllerByIndex method returns the GameObject of a controller with a specific index.
        /// </summary>
        /// <param name="index">The index of the controller to find.</param>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject of the controller</returns>
        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            SetTrackedControllerCaches();
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftController != null && cachedLeftController.index == index)
                {
                    return (actual ? sdkManager.actualLeftController : sdkManager.scriptAliasLeftController);
                }

                if (cachedRightController != null && cachedRightController.index == index)
                {
                    return (actual ? sdkManager.actualRightController : sdkManager.scriptAliasRightController);
                }
            }
            return null;
        }

        /// <summary>
        /// The GetControllerOrigin method returns the origin of the given controller.
        /// </summary>
        /// <param name="controller">The controller to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the controller.</returns>
        public override Transform GetControllerOrigin(GameObject controller)
        {
            return VRTK_SDK_Bridge.GetPlayArea();
        }

        /// <summary>
        /// The GenerateControllerPointerOrigin method can create a custom pointer origin Transform to represent the pointer position and forward.
        /// </summary>
        /// <param name="parent">The GameObject that the origin will become parent of. If it is a controller then it will also be used to determine the hand if required.</param>
        /// <returns>A generated Transform that contains the custom pointer origin.</returns>
        public override Transform GenerateControllerPointerOrigin(GameObject parent)
        {
            return null;
        }

        /// <summary>
        /// The GetControllerLeftHand method returns the GameObject containing the representation of the left hand controller.
        /// </summary>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject containing the left hand controller.</returns>
        public override GameObject GetControllerLeftHand(bool actual = false)
        {
            var strPrefab = "Prefabs/HyFeelLeft";
            GameObject temp = Resources.Load<GameObject>(strPrefab);
            return temp;
        }

        /// <summary>
        /// The GetControllerRightHand method returns the GameObject containing the representation of the right hand controller.
        /// </summary>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject containing the right hand controller.</returns>
        public override GameObject GetControllerRightHand(bool actual = false)
        {
            var strPrefab = "Prefabs/HyFeelLeft";
            GameObject temp = Resources.Load<GameObject>(strPrefab);
            return temp;
        }

        /// <summary>
        /// The IsControllerLeftHand/1 method is used to check if the given controller is the the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given controller is the left hand controller.</returns>
        public override bool IsControllerLeftHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsLeftHand(controller);
        }

        /// <summary>
        /// The IsControllerRightHand/1 method is used to check if the given controller is the the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given controller is the right hand controller.</returns>
        public override bool IsControllerRightHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsRightHand(controller);
        }

        /// <summary>
        /// The IsControllerLeftHand/2 method is used to check if the given controller is the the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the left hand controller.</returns>
        public override bool IsControllerLeftHand(GameObject controller, bool actual)
        {
            return CheckControllerLeftHand(controller, actual);
        }

        /// <summary>
        /// The IsControllerRightHand/2 method is used to check if the given controller is the the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the right hand controller.</returns>
        public override bool IsControllerRightHand(GameObject controller, bool actual)
        {
            return CheckControllerRightHand(controller, actual);
        }

        /// <summary>
        /// The GetControllerModel method returns the model alias for the given GameObject.
        /// </summary>
        /// <param name="controller">The GameObject to get the model alias for.</param>
        /// <returns>The GameObject that has the model alias within it.</returns>
        public override GameObject GetControllerModel(GameObject controller)
        {
            return GetControllerModelFromController(controller);
        }

        /// <summary>
        /// The GetControllerModel method returns the model alias for the given controller hand.
        /// </summary>
        /// <param name="hand">The hand enum of which controller model to retrieve.</param>
        /// <returns>The GameObject that has the model alias within it.</returns>
        public override GameObject GetControllerModel(ControllerHand hand)
        {
            var model = GetSDKManagerControllerModelForHand(hand);
            if (!model)
            {
                GameObject controller = null;
                switch (hand)
                {
                    case ControllerHand.Left:
                        controller = GetControllerLeftHand(true);
                        break;
                    case ControllerHand.Right:
                        controller = GetControllerRightHand(true);
                        break;
                }

                if (controller != null)
                {
                    model = controller.transform.Find("Model").gameObject;
                }
            }
            return model;
        }

        /// <summary>
        /// The GetControllerRenderModel method gets the game object that contains the given controller's render model.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>A GameObject containing the object that has a render model for the controller.</returns>
        public override GameObject GetControllerRenderModel(GameObject controller)
        {
            //TODO: NOT IMPLEMENTED
            return null;
        }

        /// <summary>
        /// The SetControllerRenderModelWheel method sets the state of the scroll wheel on the controller render model.
        /// </summary>
        /// <param name="renderModel">The GameObject containing the controller render model.</param>
        /// <param name="state">If true and the render model has a scroll wheen then it will be displayed, if false then the scroll wheel will be hidden.</param>
        public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
            
        }

        /// <summary>
        /// The HapticPulseOnIndex method is used to initiate a simple haptic pulse on the tracked object of the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to initiate the haptic pulse on.</param>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        public override void HapticPulseOnIndex(uint index, float strength = 0.5f)
        {
            if (index < uint.MaxValue)
            {
                var controller = GetControllerByIndex(index);

                if (IsControllerLeftHand(controller))
                {
                    //hapticsProceduralClipLeft.Reset();
                    //hapticsProceduralClipLeft.WriteSample((byte)(strength * byte.MaxValue));
                    //OVRHaptics.LeftChannel.Preempt(hapticsProceduralClipLeft);
                }
                else if (IsControllerRightHand(controller))
                {
                    //hapticsProceduralClipRight.Reset();
                    //hapticsProceduralClipRight.WriteSample((byte)(strength * byte.MaxValue));
                    //OVRHaptics.RightChannel.Preempt(hapticsProceduralClipRight);
                }
            }
        }

        /// <summary>
        /// The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.
        /// </summary>
        /// <returns>An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.</returns>
        public override SDK_ControllerHapticModifiers GetHapticModifiers()
        {
            return new SDK_ControllerHapticModifiers();
        }

        /// <summary>
        /// The GetVelocityOnIndex method is used to determine the current velocity of the tracked object on the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public override Vector3 GetVelocityOnIndex(uint index)
        {
            SetTrackedControllerCaches();
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftController != null && cachedLeftController.index == index)
                {
                    HyTrackingState controllerState = HyperealVR.Instance.GetTrackingState(HyDevice.Device_Controller0);
                    return controllerState.velocity;
                }

                if (cachedRightController != null && cachedRightController.index == index)
                {
                    HyTrackingState controllerState = HyperealVR.Instance.GetTrackingState(HyDevice.Device_Controller1);
                    return controllerState.velocity;
                }
            }
            return Vector3.zero;
        }

        /// <summary>
        /// The GetAngularVelocityOnIndex method is used to determine the current angular velocity of the tracked object on the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public override Vector3 GetAngularVelocityOnIndex(uint index)
        {
            SetTrackedControllerCaches();
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftController != null && cachedLeftController.index == index)
                {
                    HyTrackingState controllerState = HyperealVR.Instance.GetTrackingState(HyDevice.Device_Controller0);
                    return controllerState.angularVelocity;
                }

                if (cachedRightController != null && cachedRightController.index == index)
                {
                    HyTrackingState controllerState = HyperealVR.Instance.GetTrackingState(HyDevice.Device_Controller1);
                    return controllerState.angularVelocity;
                }
            }
            return Vector3.zero;
        }

        /// <summary>
        /// The GetTouchpadAxisOnIndex method is used to get the current touch position on the controller touchpad.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current x,y position of where the touchpad is being touched.</returns>
        public override Vector2 GetTouchpadAxisOnIndex(uint index)
        {
            return HyInputManager.Instance.GetInputDevice(HyDevice.Device_Controller0).GetTouchpadAxis();
        }

        /// <summary>
        /// The GetTriggerAxisOnIndex method is used to get the current trigger position on the controller.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current position of the trigger.</returns>
        public override Vector2 GetTriggerAxisOnIndex(uint index)
        {
            var triggerAxis =  HyInputManager.Instance.GetInputDevice(HyDevice.Device_Controller0).GetTriggerAxis(HyInputKey.IndexTrigger);
            return new Vector2(triggerAxis, 0f);
        }

        /// <summary>
        /// The GetGripAxisOnIndex method is used to get the current grip position on the controller.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current position of the grip.</returns>
        public override Vector2 GetGripAxisOnIndex(uint index)
        {
            return Vector2.zero;
        }

        /// <summary>
        /// The GetTriggerHairlineDeltaOnIndex method is used to get the difference between the current trigger press and the previous frame trigger press.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>The delta between the trigger presses.</returns>
        public override float GetTriggerHairlineDeltaOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return 0f;
            }
            return 0.1f;
        }

        /// <summary>
        /// The GetGripHairlineDeltaOnIndex method is used to get the difference between the current grip press and the previous frame grip press.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>The delta between the grip presses.</returns>
        public override float GetGripHairlineDeltaOnIndex(uint index)
        {
            return 0f;
        }

        /// <summary>
        /// The IsTriggerPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsTriggerPressedOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.Press, HyInputKey.IndexTrigger);//HyperealVR_Controller.ButtonMask.Trigger);
        }

        /// <summary>
        /// The IsTriggerPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsTriggerPressedDownOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.PressDown, HyInputKey.IndexTrigger);
        }

        /// <summary>
        /// The IsTriggerPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTriggerPressedUpOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.PressUp, HyInputKey.IndexTrigger);
        }

        /// <summary>
        /// The IsTriggerTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsTriggerTouchedOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.Touch, HyInputKey.IndexTrigger);
        }

        /// <summary>
        /// The IsTriggerTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsTriggerTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.TouchDown, HyInputKey.IndexTrigger);
        }

        /// <summary>
        /// The IsTriggerTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTriggerTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.TouchUp, HyInputKey.IndexTrigger);
        }

        /// <summary>
        /// The IsHairTriggerDownOnIndex method is used to determine if the controller button has passed it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has passed it's press threshold.</returns>
        public override bool IsHairTriggerDownOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }
            return (currentHairTriggerState[index] && !previousHairTriggerState[index]);
        }

        /// <summary>
        /// The IsHairTriggerUpOnIndex method is used to determine if the controller button has been released from it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released from it's press threshold.</returns>
        public override bool IsHairTriggerUpOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }
            return (!currentHairTriggerState[index] && previousHairTriggerState[index]);
        }

        /// <summary>
        /// The IsGripPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsGripPressedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsGripPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsGripPressedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsGripPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsGripPressedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsGripTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsGripTouchedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsGripTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsGripTouchedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsGripTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsGripTouchedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsHairGripDownOnIndex method is used to determine if the controller button has passed it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has passed it's press threshold.</returns>
        public override bool IsHairGripDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsHairGripUpOnIndex method is used to determine if the controller button has been released from it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released from it's press threshold.</returns>
        public override bool IsHairGripUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTouchpadPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsTouchpadPressedOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.Press, HyInputKey.Touchpad);
        }

        /// <summary>
        /// The IsTouchpadPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsTouchpadPressedDownOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.PressDown, HyInputKey.Touchpad);
        }

        /// <summary>
        /// The IsTouchpadPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTouchpadPressedUpOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.PressUp, HyInputKey.Touchpad);
        }

        /// <summary>
        /// The IsTouchpadTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsTouchpadTouchedOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.Touch, HyInputKey.Touchpad);
        }

        /// <summary>
        /// The IsTouchpadTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsTouchpadTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.TouchDown, HyInputKey.Touchpad);
        }

        /// <summary>
        /// The IsTouchpadTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTouchpadTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.TouchUp, HyInputKey.Touchpad);
        }

        /// <summary>
        /// The IsButtonOnePressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsButtonOnePressedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonOnePressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsButtonOnePressedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonOnePressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsButtonOnePressedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonOneTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsButtonOneTouchedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonOneTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsButtonOneTouchedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonOneTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsButtonOneTouchedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonTwoPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsButtonTwoPressedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonTwoPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsButtonTwoPressedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonTwoPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsButtonTwoPressedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonTwoTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsButtonTwoTouchedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonTwoTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsButtonTwoTouchedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonTwoTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsButtonTwoTouchedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsStartMenuPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsStartMenuPressedOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.Press, HyInputKey.Menu);
        }

        /// <summary>
        /// The IsStartMenuPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsStartMenuPressedDownOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.PressDown, HyInputKey.Menu);
        }

        /// <summary>
        /// The IsStartMenuPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsStartMenuPressedUpOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.PressUp, HyInputKey.Menu);
        }

        /// <summary>
        /// The IsStartMenuTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsStartMenuTouchedOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.Touch, HyInputKey.Menu);
        }

        /// <summary>
        /// The IsStartMenuTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsStartMenuTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.TouchDown, HyInputKey.Menu);
        }

        /// <summary>
        /// The IsStartMenuTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsStartMenuTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed((HyDevice)index, ButtonPressTypes.TouchUp, HyInputKey.Menu);
        }

        private void OnTrackedDeviceRoleChanged<T>(T ignoredArgument)
        {
            SetTrackedControllerCaches(true);
        }

        private void SetTrackedControllerCaches(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                cachedLeftTrackedObject = null;
                cachedRightTrackedObject = null;
            }

            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftTrackedObject == null && sdkManager.actualLeftController)
                {
                    cachedLeftController = sdkManager.actualLeftController.GetComponent<VRTK_TrackedController>();
                    cachedLeftController.index = 0;
                }
                if (cachedRightTrackedObject == null && sdkManager.actualRightController)
                {
                    cachedLeftController = sdkManager.actualLeftController.GetComponent<VRTK_TrackedController>();
                    cachedLeftController.index = 1;
                }
            }
        }

        private VRTK_TrackedController GetTrackedObject(GameObject controller)
        {
            SetTrackedControllerCaches();
            VRTK_TrackedController trackedObject = null;

            if (IsControllerLeftHand(controller))
            {
                trackedObject = cachedLeftController;
            }
            else if (IsControllerRightHand(controller))
            {
                trackedObject = cachedRightController;
            }
            return trackedObject;
        }

        private bool IsButtonPressed(HyDevice index, ButtonPressTypes type, HyInputKey button)
        {
            //todo use index to input type
            var device = HyInputManager.Instance.GetInputDevice((HyDevice)index);

            switch (type)
            {
                case ButtonPressTypes.Press:
                    return device.GetPress((HyInputKey)button);
                case ButtonPressTypes.PressDown:
                    return device.GetPressDown((HyInputKey)button);
                case ButtonPressTypes.PressUp:
                    return device.GetPressUp((HyInputKey)button);
                case ButtonPressTypes.Touch:
                    return device.GetTouch((HyInputKey)button);
                case ButtonPressTypes.TouchDown:
                    return device.GetTouchDown((HyInputKey)button);
                case ButtonPressTypes.TouchUp:
                    return device.GetTouchUp((HyInputKey)button);
            }

            return false;
        }

        private string GetControllerGripPath(ControllerHand hand, string suffix, ControllerHand forceHand)
        {
            switch (VRTK_DeviceFinder.GetHeadsetType(true))
            {
                case VRTK_DeviceFinder.Headsets.Vive:
                    return (forceHand == ControllerHand.Left ? "lgrip" : "rgrip") + suffix;
                case VRTK_DeviceFinder.Headsets.OculusRift:
                    return "grip" + suffix;
            }
            return null;
        }

        private string GetControllerTouchpadPath(ControllerHand hand, string suffix)
        {
            switch (VRTK_DeviceFinder.GetHeadsetType(true))
            {
                case VRTK_DeviceFinder.Headsets.Vive:
                    return "trackpad" + suffix;
                case VRTK_DeviceFinder.Headsets.OculusRift:
                    return "thumbstick" + suffix;
            }
            return null;
        }

        private string GetControllerButtonOnePath(ControllerHand hand, string suffix)
        {
            switch (VRTK_DeviceFinder.GetHeadsetType(true))
            {
                case VRTK_DeviceFinder.Headsets.Vive:
                    return null;
                case VRTK_DeviceFinder.Headsets.OculusRift:
                    return (hand == ControllerHand.Left ? "x_button" : "a_button") + suffix;
            }
            return null;
        }

        private string GetControllerButtonTwoPath(ControllerHand hand, string suffix)
        {
            switch (VRTK_DeviceFinder.GetHeadsetType(true))
            {
                case VRTK_DeviceFinder.Headsets.Vive:
                    return "button" + suffix;
                case VRTK_DeviceFinder.Headsets.OculusRift:
                    return (hand == ControllerHand.Left ? "y_button" : "b_button") + suffix;
            }
            return null;
        }

        private string GetControllerSystemMenuPath(ControllerHand hand, string suffix)
        {
            switch (VRTK_DeviceFinder.GetHeadsetType(true))
            {
                case VRTK_DeviceFinder.Headsets.Vive:
                    return "sys_button" + suffix;
                case VRTK_DeviceFinder.Headsets.OculusRift:
                    return (hand == ControllerHand.Left ? "enter_button" : "home_button") + suffix;
            }
            return null;
        }

        private string GetControllerStartMenuPath(ControllerHand hand, string suffix)
        {
            switch (VRTK_DeviceFinder.GetHeadsetType(true))
            {
                case VRTK_DeviceFinder.Headsets.Vive:
                    return null;
                case VRTK_DeviceFinder.Headsets.OculusRift:
                    return (hand == ControllerHand.Left ? "enter_button" : "home_button") + suffix;
            }
            return null;
        }
#endif
    }
}