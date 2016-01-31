using UnityEngine;
using System.Collections.Generic;
using MIConvexHull;
using System;
using UnityEngine.UI;

/// <summary>
/// Assists the user in the creation of MinimapZones, and demonstrates how to create them dynamically
/// </summary>
public class MinimapGenerator : MonoBehaviour
{
    #region Public fields
    /// <summary>
    /// Layer number of each layer that shouldn't be included in the Raycast when determining zone bounds
    /// </summary>
    public List<int> ExcludedLayers;

    /// <summary>
    /// Object containing MinimapZone script + any other desired behavior
    /// </summary>
    public GameObject MinimapZonePrefab;

    /// <summary>
    /// GUI status text
    /// </summary>
    public Text StatusText;
    #endregion

    #region Private fields
    /// <summary>
    /// Mask used to mask out undesired layers (such as the Minimap itself)
    /// </summary> 
    private int m_layerMask;

    /// <summary>
    /// Keep track of where the user has clicked to generate a mesh
    /// </summary>
    private List<Vector3> m_currentVertexList;

    /// <summary>
    /// Whether we are currently creating a mesh
    /// </summary>
    private bool m_creatingMesh;

    /// <summary>
    /// True after one or more zones have been created
    /// </summary>
    private bool m_createdZone;
    #endregion

    #region MonoBehaviour
    void Start()
    {
        Debug.Assert(MinimapZonePrefab != null, "MinimapGenerator needs a MinimapZonePrefab assigned in the Inspector");
        Debug.Assert(StatusText != null, "MinimapGenerator needs a StatusText assigned in the Inspector");

        int temp = 0;
        foreach (int layer in ExcludedLayers)
            temp |= 1 << layer;
        m_layerMask = ~temp;

        m_creatingMesh = false;
    }

    void Update()
    {
        // Usage: User presses Esc to begin creating a Mesh, clicks around a bunch of points,
        // each of which is saved into a Mesh, then user presses Esc to finalize the Mesh.
        if (!m_creatingMesh)
        {
            if (m_createdZone)
            {
                StatusText.text = "Your Zone is now visible in the Minimap.\n";
                StatusText.text += "Hover over it to highlight,\n";
                StatusText.text += "or click it to select.\n";
            }
            else
            {
                StatusText.text = "";
            }

            StatusText.text += "Press Esc to begin creating a new Zone";
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Beginning new Zone");
                StatusText.text = "Creating new Zone...\n";
                StatusText.text += "Click on the plane to add a vertex";
                m_currentVertexList = new List<Vector3>();
                m_creatingMesh = true;
                return;
            }

            else
            {
                // If we aren't creating a mesh and we don't press Esc then there's nothing to do here
                return;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Determine where on the plane the user clicked and save the position
            RaycastHit rh;
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out rh, Mathf.Infinity, m_layerMask))
            {
                Debug.Log("Added point #" + (m_currentVertexList.Count + 1) + rh.point + " to mesh");
                m_currentVertexList.Add(rh.point);

                StatusText.text = "Click on the plane to add a vertex\n";
                foreach (Vector3 v in m_currentVertexList)
                {
                    StatusText.text += v.ToString() + "\n";
                }
                StatusText.text += "Press Esc to finalize Zone";
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Finalizing mesh...");
            m_creatingMesh = false;

            // Finalize this mesh
            Mesh m = ComputeMesh();
            if (m == null)
            {
                return;
            }

            GameObject zoneObj = GameObject.Instantiate(MinimapZonePrefab) as GameObject;
            MinimapZone minimapZone = zoneObj.GetComponent<MinimapZone>();
            if (minimapZone == null)
            {
                Debug.LogError("MinimapZonePrefab lacks a MinimapZone Component. Please assign a MinimapZone component to the " + MinimapZonePrefab.name + " prefab.");
                return;
            }

            minimapZone.ZoneMesh = m;

            // Push this GameObject up just a tad
            zoneObj.transform.Translate(0.0f, 0.03f, 0.0f);
        }
    }
    #endregion


    /// <summary>
    /// Use Delauney triangulation to retrieve the triangles for this mesh
    /// </summary>
    /// <returns></returns>
    private Mesh ComputeMesh()
    {
        if (m_currentVertexList.Count < 3)
        {
            Debug.LogWarning("Not creating Zone (need >= 3 vertices)");
            return null;
        }

        Mesh mesh = new Mesh();
        // Drop the y-coordinate in order to triangulate in two dimensions.
        // We don't require the ability to triangulate in 3+ dimensions, although the MIConvexHull lib is capable of doing so.
        Vertex[] vertices = m_currentVertexList.ConvertAll(vert3 => new Vertex(vert3.x, vert3.z)).ToArray();

        // Triangulate the mesh
        var config = new TriangulationComputationConfig
        {
            PointTranslationType = PointTranslationType.TranslateInternal,
            PlaneDistanceTolerance = 0.00001,
            // the translation radius should be lower than PlaneDistanceTolerance / 2
            PointTranslationGenerator = TriangulationComputationConfig.RandomShiftByRadius(0.000001, 0)
        };

        List<Vector3> meshVerts = new List<Vector3>();
        List<int> indices = new List<int>();

        try
        {
            VoronoiMesh<Vertex, Cell, VoronoiEdge<Vertex, Cell>> voronoiMesh;
            voronoiMesh = VoronoiMesh.Create<Vertex, Cell>(vertices, config);

            foreach (var cell in voronoiMesh.Vertices)
            {
                for (int vertNum = 0; vertNum < 3; vertNum++)
                {
                    Vector3 vert = new Vector3((float)cell.Vertices[vertNum].Position[0], 0.0f, (float)cell.Vertices[vertNum].Position[1]);
                    int idx;
                    if (meshVerts.Contains(vert))
                    {
                        idx = meshVerts.IndexOf(vert);
                    }
                    else
                    {
                        meshVerts.Add(vert);
                        idx = meshVerts.Count - 1;
                    }
                    indices.Add(idx);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        mesh.vertices = meshVerts.ToArray();
        mesh.triangles = indices.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        m_createdZone = true;
        return mesh;
    }
}
