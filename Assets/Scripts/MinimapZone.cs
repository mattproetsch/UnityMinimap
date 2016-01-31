using UnityEngine;

/// <summary>
/// The MinimapZone is an area that can be hovered over and clicked in the Minimap.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MinimapZone : MonoBehaviour
{
    #region Public fields / props
    /// <summary>
    /// The Color to highlight this mesh when the user initiates an interaction (by e.g. mousing over it in the minimap)
    /// </summary>
    public Color HighlightColor;

    /// <summary>
    /// The Color to use when the user clicks on this Zone
    /// </summary>
    public Color ClickColor;

    /// <summary>
    /// The Mesh bounding the current MinimapZone
    /// </summary>
    public Mesh ZoneMesh
    {
        get
        {
            return m_zoneMesh;
        }
        set
        {
            m_zoneMesh = value;
            // Update the MeshFilter and MeshColliders with the new shared mesh
            m_meshFilter.mesh = m_zoneMesh;
            m_meshCollider.sharedMesh = m_zoneMesh;
        }
    }
    #endregion

    #region Private fields
    private Mesh m_zoneMesh;
    private MeshFilter m_meshFilter;
    private MeshRenderer m_meshRenderer;
    private MeshCollider m_meshCollider;

    private Color m_defaultColor;
    private bool m_clicked;
    #endregion

    #region MonoBehaviour
    void Start()
    {
        m_meshFilter = GetComponent<MeshFilter>();
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_meshCollider = GetComponent<MeshCollider>();

        m_defaultColor = m_meshRenderer.material.GetColor("_Color");

        ClickColor = new Color(Random.Range(0.0f, 0.5f), Random.Range(0.25f, 0.75f), Random.Range(0.0f, 0.5f));
        HighlightColor = new Color(Random.Range(0.25f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(0.35f, 1.0f));
    }

    // If we generate this programmatically, we should still execute the Start() function
    void Awake()
    {
        Start();
    }

    void OnRenderObject()
    {
        if (!m_clicked)
        {
            // Reset the material's color
            m_meshRenderer.material.SetColor("_Color", m_defaultColor);
        }
    }
    #endregion

    #region Public functions
    public void MinimapZoneMousedOver()
    {
        // Do something to identify this zone in the Minimap's view
        if (!m_clicked)
        {
            m_meshRenderer.material.SetColor("_Color", HighlightColor);
        }
    }

    public void MinimapZoneClicked()
    {
        // Take some action: Recenter the camera, zoom to a location, etc.
        if (!m_clicked)
        {
            m_clicked = true;
            Camera.main.transform.position = MeshCentroid() + Vector3.up * 5.0f;
            Camera.main.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            m_meshRenderer.material.SetColor("_Color", ClickColor);
        }
        else
        {
            m_clicked = false;
        }
    }

    private Vector3 MeshCentroid()
    {
        Vector3 centroid = Vector3.zero;
        foreach (Vector3 v in m_meshFilter.mesh.vertices)
        {
            centroid += v;
        }
        return centroid / m_meshFilter.mesh.vertexCount;
    }
    #endregion

}
