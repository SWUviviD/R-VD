//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Actions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @Actions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Actions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Actions"",
    ""maps"": [
        {
            ""name"": ""PlayerActions"",
            ""id"": ""44b3c9d4-7c0b-4e70-9f0e-cab5a72f974a"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""7ac87012-21f5-417b-bf88-a68c940f9037"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""d3a45b30-2164-43c3-b4da-c66d3fda1aa1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""13938caa-525f-439a-b548-59d1cc67e0fb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Magic"",
                    ""type"": ""Button"",
                    ""id"": ""a1503251-f16a-40a4-806d-7f868cfb2491"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CameraRotation"",
                    ""type"": ""Value"",
                    ""id"": ""26c5ebff-150b-4567-9b5b-444e882cdfb2"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""StarHunt"",
                    ""type"": ""Button"",
                    ""id"": ""38fb29df-4f5b-4849-bb71-1d14f1444d6a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""StarFusion"",
                    ""type"": ""Button"",
                    ""id"": ""e89d7a47-84b3-4522-9665-f59702484f6b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Arrow"",
                    ""id"": ""790ea63d-831b-4b7c-ac03-248bb02791dd"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6303ec6b-8dc2-4dac-96ca-da5fe7962f5e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""764e1965-8b24-4081-990a-30064cc000f1"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""2b6c5e5d-28d2-404d-b06a-9e2739074166"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""68efc805-f3e7-4b5c-90e5-a9b92802ab26"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""d3932525-7bfb-4eb3-b0d1-bd63bbf258bb"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b3ce3295-2611-43f3-8f51-a0879a78757f"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""27cf3cec-349f-4520-ad34-a45cced70e49"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Magic"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""b177f2a7-0580-4852-a30a-8fa5559529a4"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraRotation"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""5824770b-f1bf-4d53-9758-8e4e28a4d484"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""CameraRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""8e004be9-6d6d-4c1e-a690-58e7e48e80c2"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""CameraRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""e6a2c5d8-4916-49de-8bbf-1ee9a9daaaf8"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""StarHunt"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3e8f9082-8cb5-4e3d-b1e5-30f309fde797"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""StarFusion"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UIActions"",
            ""id"": ""c3f5d059-d212-44a6-a652-5b59ca277517"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""13dc2c9a-14e4-4c93-be47-0ec5d6c76edb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ac1dd383-0134-4a41-aecc-8938065094ee"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""PC"",
            ""bindingGroup"": ""PC"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // PlayerActions
        m_PlayerActions = asset.FindActionMap("PlayerActions", throwIfNotFound: true);
        m_PlayerActions_Move = m_PlayerActions.FindAction("Move", throwIfNotFound: true);
        m_PlayerActions_Jump = m_PlayerActions.FindAction("Jump", throwIfNotFound: true);
        m_PlayerActions_Dash = m_PlayerActions.FindAction("Dash", throwIfNotFound: true);
        m_PlayerActions_Magic = m_PlayerActions.FindAction("Magic", throwIfNotFound: true);
        m_PlayerActions_CameraRotation = m_PlayerActions.FindAction("CameraRotation", throwIfNotFound: true);
        m_PlayerActions_StarHunt = m_PlayerActions.FindAction("StarHunt", throwIfNotFound: true);
        m_PlayerActions_StarFusion = m_PlayerActions.FindAction("StarFusion", throwIfNotFound: true);
        // UIActions
        m_UIActions = asset.FindActionMap("UIActions", throwIfNotFound: true);
        m_UIActions_Newaction = m_UIActions.FindAction("New action", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // PlayerActions
    private readonly InputActionMap m_PlayerActions;
    private List<IPlayerActionsActions> m_PlayerActionsActionsCallbackInterfaces = new List<IPlayerActionsActions>();
    private readonly InputAction m_PlayerActions_Move;
    private readonly InputAction m_PlayerActions_Jump;
    private readonly InputAction m_PlayerActions_Dash;
    private readonly InputAction m_PlayerActions_Magic;
    private readonly InputAction m_PlayerActions_CameraRotation;
    private readonly InputAction m_PlayerActions_StarHunt;
    private readonly InputAction m_PlayerActions_StarFusion;
    public struct PlayerActionsActions
    {
        private @Actions m_Wrapper;
        public PlayerActionsActions(@Actions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PlayerActions_Move;
        public InputAction @Jump => m_Wrapper.m_PlayerActions_Jump;
        public InputAction @Dash => m_Wrapper.m_PlayerActions_Dash;
        public InputAction @Magic => m_Wrapper.m_PlayerActions_Magic;
        public InputAction @CameraRotation => m_Wrapper.m_PlayerActions_CameraRotation;
        public InputAction @StarHunt => m_Wrapper.m_PlayerActions_StarHunt;
        public InputAction @StarFusion => m_Wrapper.m_PlayerActions_StarFusion;
        public InputActionMap Get() { return m_Wrapper.m_PlayerActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActionsActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActionsActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Dash.started += instance.OnDash;
            @Dash.performed += instance.OnDash;
            @Dash.canceled += instance.OnDash;
            @Magic.started += instance.OnMagic;
            @Magic.performed += instance.OnMagic;
            @Magic.canceled += instance.OnMagic;
            @CameraRotation.started += instance.OnCameraRotation;
            @CameraRotation.performed += instance.OnCameraRotation;
            @CameraRotation.canceled += instance.OnCameraRotation;
            @StarHunt.started += instance.OnStarHunt;
            @StarHunt.performed += instance.OnStarHunt;
            @StarHunt.canceled += instance.OnStarHunt;
            @StarFusion.started += instance.OnStarFusion;
            @StarFusion.performed += instance.OnStarFusion;
            @StarFusion.canceled += instance.OnStarFusion;
        }

        private void UnregisterCallbacks(IPlayerActionsActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Dash.started -= instance.OnDash;
            @Dash.performed -= instance.OnDash;
            @Dash.canceled -= instance.OnDash;
            @Magic.started -= instance.OnMagic;
            @Magic.performed -= instance.OnMagic;
            @Magic.canceled -= instance.OnMagic;
            @CameraRotation.started -= instance.OnCameraRotation;
            @CameraRotation.performed -= instance.OnCameraRotation;
            @CameraRotation.canceled -= instance.OnCameraRotation;
            @StarHunt.started -= instance.OnStarHunt;
            @StarHunt.performed -= instance.OnStarHunt;
            @StarHunt.canceled -= instance.OnStarHunt;
            @StarFusion.started -= instance.OnStarFusion;
            @StarFusion.performed -= instance.OnStarFusion;
            @StarFusion.canceled -= instance.OnStarFusion;
        }

        public void RemoveCallbacks(IPlayerActionsActions instance)
        {
            if (m_Wrapper.m_PlayerActionsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActionsActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActionsActions @PlayerActions => new PlayerActionsActions(this);

    // UIActions
    private readonly InputActionMap m_UIActions;
    private List<IUIActionsActions> m_UIActionsActionsCallbackInterfaces = new List<IUIActionsActions>();
    private readonly InputAction m_UIActions_Newaction;
    public struct UIActionsActions
    {
        private @Actions m_Wrapper;
        public UIActionsActions(@Actions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_UIActions_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_UIActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActionsActions set) { return set.Get(); }
        public void AddCallbacks(IUIActionsActions instance)
        {
            if (instance == null || m_Wrapper.m_UIActionsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_UIActionsActionsCallbackInterfaces.Add(instance);
            @Newaction.started += instance.OnNewaction;
            @Newaction.performed += instance.OnNewaction;
            @Newaction.canceled += instance.OnNewaction;
        }

        private void UnregisterCallbacks(IUIActionsActions instance)
        {
            @Newaction.started -= instance.OnNewaction;
            @Newaction.performed -= instance.OnNewaction;
            @Newaction.canceled -= instance.OnNewaction;
        }

        public void RemoveCallbacks(IUIActionsActions instance)
        {
            if (m_Wrapper.m_UIActionsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IUIActionsActions instance)
        {
            foreach (var item in m_Wrapper.m_UIActionsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_UIActionsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public UIActionsActions @UIActions => new UIActionsActions(this);
    private int m_PCSchemeIndex = -1;
    public InputControlScheme PCScheme
    {
        get
        {
            if (m_PCSchemeIndex == -1) m_PCSchemeIndex = asset.FindControlSchemeIndex("PC");
            return asset.controlSchemes[m_PCSchemeIndex];
        }
    }
    public interface IPlayerActionsActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnMagic(InputAction.CallbackContext context);
        void OnCameraRotation(InputAction.CallbackContext context);
        void OnStarHunt(InputAction.CallbackContext context);
        void OnStarFusion(InputAction.CallbackContext context);
    }
    public interface IUIActionsActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
}
