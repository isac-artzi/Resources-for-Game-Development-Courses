// LandingValidator.cs — Activity 5.1: Teleport, Properly
// Decides whether a candidate landing point is a good place to put a player: not too steep, and with
// enough clear space around the body so you do not arrive with your face inside a wall.
// TeleportArcPreview asks this component about the point its arc hits and colors the arc accordingly.
// Attached to: Arc Preview (next to TeleportArcPreview) in the starter scene.
// Created by Isac Artzi

using UnityEngine;

public class LandingValidator : MonoBehaviour
{
    [Header("Slope")]
    [Tooltip("Steepest surface (degrees from horizontal) the player may land on. 20° is a gentle ramp; 35° is a scramble.")]
    public float maxSlopeDegrees = 20f;

    [Header("Clearance")]
    [Tooltip("Radius (m) of the imaginary body cylinder that must be free of obstacles at the landing point.")]
    public float bodyRadius = 0.45f;

    [Tooltip("Height (m) above the landing point at which the clearance sphere is centered — roughly chest height.")]
    public float clearanceHeight = 1.0f;

    [Tooltip("Which layers count as obstacles for the clearance test. Default = everything.")]
    public LayerMask obstacleMask = ~0;

    /// <summary>
    /// Angle in degrees between a surface normal and straight up. A flat floor returns 0; a vertical wall 90.
    /// </summary>
    public static float SlopeDegrees(Vector3 surfaceNormal)
    {
        // TODO 1: Implement: Mathf.Acos(Mathf.Clamp(Vector3.Dot(surfaceNormal.normalized, Vector3.up), -1f, 1f)) * Mathf.Rad2Deg
        //         Look at: Vector3.Dot, Mathf.Acos (returns RADIANS), Mathf.Rad2Deg. Clamp first: floating-point noise
        //         can give a dot product of 1.0000001 and Acos of that is NaN.
        //         Check: SlopeDegrees(Vector3.up) == 0; SlopeDegrees(new Vector3(0, 0.866f, 0.5f)) is about 30.
        return 0f;
    }

    /// <summary>
    /// Returns true when the point is a valid landing spot. 'reason' explains a rejection in a few words
    /// (shown on the floating label) and is empty when the spot is fine.
    /// </summary>
    public bool IsValid(Vector3 point, Vector3 surfaceNormal, out string reason)
    {
        reason = "";

        // TODO 2: Slope test. float slope = SlopeDegrees(surfaceNormal); if slope > maxSlopeDegrees,
        //         set reason = "Too steep (" + slope.ToString("F0") + " deg)" and return false.
        //         Check: aim your arc at the stone ramp in the starter scene — the label reports about 30 deg and the arc turns red.

        // TODO 3: Clearance test. Center a sphere at point + Vector3.up * clearanceHeight with radius bodyRadius and ask
        //         Physics.CheckSphere(center, bodyRadius, obstacleMask) whether anything overlaps it. If it does,
        //         set reason = "Too close to a wall" and return false.
        //         Why a sphere at chest height and not at the feet: the feet always touch the floor you are landing on,
        //         which would make every spot "blocked". At 1 m up, only walls and pillars can intersect.
        //         Look at: Physics.CheckSphere, LayerMask.
        //         Check: aim right next to the ruined wall — red; step the aim 0.5 m away from it — green.

        return true; // placeholder: everything is valid until you implement the tests
    }

    void OnDrawGizmosSelected()
    {
        // Shows the clearance sphere at this object's position while selected — handy for tuning bodyRadius.
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position + Vector3.up * clearanceHeight, bodyRadius);
    }
}
