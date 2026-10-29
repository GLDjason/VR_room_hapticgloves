using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Interactables
{
    /// <summary>
    /// XRGrabInteractable met een extra "Activate Hold" event dat elke frame (of per interval)
    /// afgaat zolang de Activate-input (trigger) ingedrukt blijft.
    /// </summary>
    [AddComponentMenu("XR/XR Grab Interactable (Activate Hold)")]
    public class XRGrabInteractableHold : XRGrabInteractable
    {
        [Header("Activate Hold")]
        [SerializeField, Tooltip("Tijd in seconden tussen twee hold-events. 0 = elke frame.")]
        float m_HoldInterval = 0f;

        [SerializeField, Tooltip("Wordt herhaald aangeroepen zolang de trigger ingedrukt blijft.")]
        UnityEvent m_ActivateHold = new UnityEvent();

        /// <summary>Herhaald event zolang de activate-input vastgehouden wordt.</summary>
        public UnityEvent activateHold => m_ActivateHold;

        /// <summary>Tijd tussen twee hold-events (0 = elke frame).</summary>
        public float holdInterval
        {
            get => m_HoldInterval;
            set => m_HoldInterval = Mathf.Max(0f, value);
        }

        /// <summary>True zolang de trigger ingedrukt is.</summary>
        public bool isActivateHeld { get; private set; }

        /// <summary>De interactor die momenteel activeert (null als er niet geactiveerd wordt).</summary>
        public IXRActivateInteractor activatingInteractor { get; private set; }

        float m_HoldTimer;

        /// <inheritdoc />
        protected override void OnActivated(ActivateEventArgs args)
        {
            base.OnActivated(args);

            // args niet bewaren: XRI hergebruikt dit object (pooling). Alleen de interactor opslaan.
            activatingInteractor = args.interactorObject;
            isActivateHeld = true;
            m_HoldTimer = 0f;
        }

        /// <inheritdoc />
        protected override void OnDeactivated(DeactivateEventArgs args)
        {
            base.OnDeactivated(args);
            StopHold();
        }

        /// <inheritdoc />
        protected override void OnSelectExiting(SelectExitEventArgs args)
        {
            base.OnSelectExiting(args);

            // Als je het object loslaat terwijl de trigger nog ingedrukt is, stoppen we de hold.
            if (!isSelected)
                StopHold();
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            StopHold();
            base.OnDisable();
        }

        /// <inheritdoc />
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            // Dynamic fase = één keer per frame in Update.
            if (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic || !isActivateHeld)
                return;

            if (m_HoldInterval <= 0f)
            {
                m_ActivateHold.Invoke();
                return;
            }

            m_HoldTimer += Time.deltaTime;
            while (m_HoldTimer >= m_HoldInterval)
            {
                m_HoldTimer -= m_HoldInterval;
                m_ActivateHold.Invoke();
            }
        }

        void StopHold()
        {
            isActivateHeld = false;
            activatingInteractor = null;
            m_HoldTimer = 0f;
        }
    }
}