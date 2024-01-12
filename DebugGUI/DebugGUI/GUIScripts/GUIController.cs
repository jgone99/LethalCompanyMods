using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ModGUI.GUI
{
    internal class GUIController : MonoBehaviour
    {
        public ModGUI guiScript { get; set; }
        PlayerControllerB player;

        void Awake()
        {
            player = gameObject.GetComponent<PlayerControllerB>();
        }

        void Update()
        {
            if (player.isHostPlayerObject)
            {
                if (guiScript != null)
                {
                    if (Keyboard.current.f7Key.wasPressedThisFrame)
                    {
                        guiScript.enabled = !guiScript.enabled;
                        if (guiScript.enabled)
                        {
                            player.disableLookInput = true;
                            Cursor.visible = true;
                            Cursor.lockState = CursorLockMode.Confined;
                        }
                        else
                        {
                            player.disableLookInput = false;
                            Cursor.visible = false;
                            Cursor.lockState = CursorLockMode.Locked;
                        }

                    }
                }
            }
        }
    }
}
