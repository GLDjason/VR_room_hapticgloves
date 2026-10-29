using UnityEngine;

public class CymbalWobble : MonoBehaviour
{
    [SerializeField] private Transform visual;           // het mesh-child
    [SerializeField] public float maxAngle = 8f;        // maximale uitslag in graden
    [SerializeField] public float kick = 120f;          // hoe hard een slag het bekken aanstoot
    [SerializeField] public float stiffness = 400f;     // hoger = sneller trillen
    [SerializeField] public float damping = 4f;         // hoger = sneller uitgetrild

    private Quaternion startRotation;
    private Vector3 axis = Vector3.right;
    private float angle;          // huidige uitslag (graden)
    private float angularVelocity;

    private void Awake()
    {
        if (visual == null) visual = transform;
        startRotation = visual.localRotation;
    }

    public void Hit(float strength, Vector3 worldHitPoint)
    {
        // Kantel om de as die loodrecht staat op de richting van het middelpunt naar de slagplek
        Vector3 local = transform.InverseTransformPoint(worldHitPoint);
        Vector3 dir = new Vector3(local.x, 0f, local.z);
        if (dir.sqrMagnitude > 0.0001f)
            axis = Vector3.Cross(Vector3.up, dir.normalized);

        angularVelocity += strength * kick;
    }

    private void Update()
    {
        if (Mathf.Abs(angle) < 0.01f && Mathf.Abs(angularVelocity) < 0.01f) return;

        float dt = Time.deltaTime;
        float acceleration = -stiffness * angle - damping * angularVelocity;
        angularVelocity += acceleration * dt;
        angle += angularVelocity * dt;
        angle = Mathf.Clamp(angle, -maxAngle, maxAngle);

        visual.localRotation = startRotation * Quaternion.AngleAxis(angle, axis);
    }
}