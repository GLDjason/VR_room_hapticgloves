using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DrumPad : MonoBehaviour
{
    [SerializeField] private string stickTag = "DrumStick";
    [SerializeField] public float minVelocity = 0.3f;   // te zachte aanrakingen negeren
    [SerializeField] public float maxVelocity = 3f;     // snelheid waarbij het volume maximaal is
    [SerializeField] public float cooldown = 0.05f;     // voorkomt dubbele triggers

    private AudioSource audioSource;
    private CymbalWobble wobble;
    private float lastHitTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        wobble = GetComponent<CymbalWobble>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag(stickTag)) return;
        if (Time.time - lastHitTime < cooldown) return;

        float speed = collision.relativeVelocity.magnitude;
        if (speed < minVelocity) return;

        float volume = Mathf.InverseLerp(minVelocity, maxVelocity, speed);
        volume = Mathf.Clamp(volume, 0.1f, 1f);

        audioSource.PlayOneShot(audioSource.clip, volume);
        if (wobble != null) wobble.Hit(volume, collision.GetContact(0).point);
        var haptics = collision.collider.GetComponentInParent<StickHaptics>();
        if (haptics != null) haptics.Hit(volume);
        lastHitTime = Time.time;

        // Optioneel: haptische feedback (zie hieronder)
    }
}