using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class MinimapCameraEffects : MonoBehaviour
{
    #region Public Editor fields
    public Material MinimapRenderMat;
    #endregion

    #region Private fields
    /// <summary>
    /// The object pointed to by the user in the Minimap
    /// </summary>
    private GameObject m_pointedObject;

    #endregion

    #region MonoBehaviour
    void Start()
    {
        Debug.Assert(MinimapRenderMat != null, "MinimapCameraEffects requires a MinimapRenderMat assigned in the Inspector");
    }
    #endregion
}
