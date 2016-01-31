using UnityEngine;
using System.Collections;

/// <summary>
/// Usage:
/// Assign as child of a Camera to use the Minimap.
/// Set up a secondary Camera and point it at whatever part of the scene should be drawn to the Minimap.
/// You can play around with the Minimap render camera, culling out parts of the scene you don't need 
/// and using a custom skybox or solid color as a backdrop.
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class Minimap : MonoBehaviour
{
    #region Public Editor fields
    public Camera MinimapCamera;
    public RenderTexture MinimapRenderTexture;
    #endregion

    #region Private fields
    private Camera m_parentCam;
    #endregion

    #region MonoBehaviour
    void Start()
    {
        // Establish needed references when game loads
        Debug.Assert(MinimapCamera != null, "Minimap requires a Camera assigned in the Inspector");
        Debug.Assert(MinimapRenderTexture != null, "Minimap requires a RenderTexture assigned in the Inspector");
        m_parentCam = transform.parent.GetComponent<Camera>();
        Debug.Assert(m_parentCam != null, "Minimap requires a parent Camera");

        // Set the RenderTexture as the target texture of the MeshRenderer
        Material m = GetComponent<MeshRenderer>().material;
        m.SetTexture("_MainTex", MinimapRenderTexture);

        // Make sure that the minimap camera renders to our target texture
        MinimapCamera.GetComponent<Camera>().targetTexture = MinimapRenderTexture;
    }

    void OnMouseOver()
    {
        // Map the user's mouse coordinates into normalized ([0..1], with (0, 0) at bottom-left) local space for the Minimap.
        // We will use coordinates as viewport coords for the MinimapCamera to determine if there are any intersections.

        Vector2 mousePosition = Input.mousePosition;
        RaycastHit rh;
        Ray r = m_parentCam.ScreenPointToRay(mousePosition);
        if (!Physics.Raycast(r, out rh))
        {
            // We shouldn't get an OnMouseOver message if there is not a collision
            Debug.LogError("Inconsistent mouse coordinates: " + mousePosition.ToString());
        }

        // Map the collision point into the minimap's local space
        Vector3 collisionPointLocalSpace = transform.InverseTransformPoint(rh.point);
        // Transform the collision point into normalized (0..1) space with (0,0) at bottom-left corner
        // This is also called Viewport space
        collisionPointLocalSpace.x = collisionPointLocalSpace.x + 0.5f;  // Map to [0..1]
        collisionPointLocalSpace.y = collisionPointLocalSpace.y + 0.5f;  // Map to [0..1]

        // Now raycast into the minimap camera using this point
        r = MinimapCamera.ViewportPointToRay(collisionPointLocalSpace);
        if (Physics.Raycast(r, out rh))
        {
            MinimapZone mz = rh.collider.GetComponent<MinimapZone>();

            if (mz != null)
            {
                mz.MinimapZoneMousedOver();

                if (Input.GetMouseButtonDown(0))
                {
                    mz.MinimapZoneClicked();
                }
            }
        }
    }
    #endregion
}
