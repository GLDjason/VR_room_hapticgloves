using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

[RequireComponent(typeof(XRGrabInteractable))]
public class StickHaptics : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float maxAmplitude = 1f;
    [SerializeField] private float duration = 0.08f;

    private XRGrabInteractable grab;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
    }

    public void Hit(float strength)
    {
        if (!grab.isSelected) return;

        // De hand/interactor die de stick nu vasthoudt
        if (grab.firstInteractorSelecting is Component interactor)
        {
            var player = interactor.GetComponentInParent<HapticImpulsePlayer>();
            if (player != null)
            {
                player.SendHapticImpulse(Mathf.Clamp01(strength) * maxAmplitude, duration);
            }
        }
    }
}