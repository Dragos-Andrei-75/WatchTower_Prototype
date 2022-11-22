// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/_InputSystem/InputMenu.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMenu : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMenu()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMenu"",
    ""maps"": [
        {
            ""name"": ""MenuPause"",
            ""id"": ""3320de38-4278-4739-9b9e-fdc918eef897"",
            ""actions"": [
                {
                    ""name"": ""PauseResume"",
                    ""type"": ""Button"",
                    ""id"": ""ed2469d3-c1a0-440a-ad9a-ebc60d2b2d45"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0fbd11d9-69e9-438c-9bdb-962cb4fe6e29"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PauseResume"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // MenuPause
        m_MenuPause = asset.FindActionMap("MenuPause", throwIfNotFound: true);
        m_MenuPause_PauseResume = m_MenuPause.FindAction("PauseResume", throwIfNotFound: true);
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

    // MenuPause
    private readonly InputActionMap m_MenuPause;
    private IMenuPauseActions m_MenuPauseActionsCallbackInterface;
    private readonly InputAction m_MenuPause_PauseResume;
    public struct MenuPauseActions
    {
        private @InputMenu m_Wrapper;
        public MenuPauseActions(@InputMenu wrapper) { m_Wrapper = wrapper; }
        public InputAction @PauseResume => m_Wrapper.m_MenuPause_PauseResume;
        public InputActionMap Get() { return m_Wrapper.m_MenuPause; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuPauseActions set) { return set.Get(); }
        public void SetCallbacks(IMenuPauseActions instance)
        {
            if (m_Wrapper.m_MenuPauseActionsCallbackInterface != null)
            {
                @PauseResume.started -= m_Wrapper.m_MenuPauseActionsCallbackInterface.OnPauseResume;
                @PauseResume.performed -= m_Wrapper.m_MenuPauseActionsCallbackInterface.OnPauseResume;
                @PauseResume.canceled -= m_Wrapper.m_MenuPauseActionsCallbackInterface.OnPauseResume;
            }
            m_Wrapper.m_MenuPauseActionsCallbackInterface = instance;
            if (instance != null)
            {
                @PauseResume.started += instance.OnPauseResume;
                @PauseResume.performed += instance.OnPauseResume;
                @PauseResume.canceled += instance.OnPauseResume;
            }
        }
    }
    public MenuPauseActions @MenuPause => new MenuPauseActions(this);
    public interface IMenuPauseActions
    {
        void OnPauseResume(InputAction.CallbackContext context);
    }
}
