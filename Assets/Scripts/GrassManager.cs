using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public class GrassManager : MonoBehaviour
{
    public Robot m_Robot;
    public float m_CutRadius = 10.0f;

    public Mesh m_GrassMesh;
    public Material m_GrassMaterial;
    public Transform m_GrassPlane;

    public Vector2 m_StageSize;
    public int m_GrassDensity = 900;
    public int m_GrassDensityWidth = 30;
    public int m_GrassDensityHeight = 30;

    public Texture2D m_SplatMap;
    public GameObject m_PlaneObject;

    private Grass[] m_AllGrassArray;
    private Matrix4x4[] m_MatrixArray;
    private float[] m_CutGrassArray;
    private Vector4[] m_ColorsArray;
    private Vector3[] m_Positions;

    private MaterialPropertyBlock m_PropertyBlock;

    const int BATCH_MAX = 1023;
    const int BATCH_MAX_FLOAT = 1023;
    // Start is called before the first frame update
    void Start()
    {
        MarchPlane();

        m_PropertyBlock = new MaterialPropertyBlock();
    }

    void InitGrass() 
    {
        //Debug.Log("Mesh Data: " + m_GrassMesh.vertices.Length + "\n" + m_GrassMesh.triangles.Length + "Normals" + m_GrassMesh.normals.Length);

        m_Positions = new Vector3[m_GrassDensity];

        for (int i = 0; i < m_GrassDensity; i++)
        {
            Vector3 randomPos = transform.position + new Vector3(Random.Range(-m_StageSize.x, m_StageSize.x), 0, Random.Range(-m_StageSize.y, m_StageSize.y));
            m_Positions[i] = randomPos;
        }

        // Put positions in Matrix4x4 Array:
        m_MatrixArray = new Matrix4x4[m_Positions.Length];
        m_CutGrassArray = new float[m_Positions.Length];
        m_ColorsArray = new Vector4[m_Positions.Length];

        for (int i = 0; i < m_Positions.Length; i++)
        {
            m_MatrixArray[i].SetTRS(m_Positions[i], Quaternion.identity, Vector3.one);

            float distance = Vector3.Distance(m_Positions[i], Vector3.zero);
            if (distance <= 20.0f)
            {
                m_CutGrassArray[i] = -1f;
                m_ColorsArray[i] = Color.red;
            }
            else
            {
                m_CutGrassArray[i] = 1f;
                m_ColorsArray[i] = Color.blue;
            }

            //m_ColorsArray[i] = Color.HSVToRGB(Random.value, 1, .9f);

        }

    }

    // Update is called once per frame
    void Update()
    {
        FillCutGrass();

        int batches = Mathf.CeilToInt((float)m_AllGrassArray.Length / BATCH_MAX_FLOAT);

        for (int i = 0; i < batches; ++i)
        {
            int batchCount = Mathf.Min(BATCH_MAX, m_AllGrassArray.Length - (BATCH_MAX * i));
            int start = Mathf.Max(0, (i - 1) * BATCH_MAX);


            float[] batchedArray = GetBatchedArray(start, batchCount);
            Matrix4x4[] batchedMatrices = GetBatchedMatrices(start, batchCount);

            //m_PropertyBlock.SetVectorArray("_Color", m_ColorsArray);
            m_PropertyBlock.SetFloatArray("_HideGrass", batchedArray);

            Graphics.DrawMeshInstanced(m_GrassMesh, 0, m_GrassMaterial, batchedMatrices, batchCount, m_PropertyBlock);

        }

        //Graphics.DrawMeshInstanced(m_GrassMesh, 0, m_GrassMaterial, m_MatrixArray, m_MatrixArray.Length);
    }

    private Matrix4x4[] GetBatchedMatrices(int offset, int batchCount)
    {
        Matrix4x4[] batchedMatrices = new Matrix4x4[batchCount];

        for (int i = 0; i < batchCount; ++i)
        {
            batchedMatrices[i] = m_AllGrassArray[i + offset].m_Transform;
        }

        return batchedMatrices;
    }

    private float[] GetBatchedArray(int offset, int batchCount)
    {
        float[] batchedArray = new float[batchCount];

        for (int i = 0; i < batchCount; ++i)
        {
            batchedArray[i] = m_AllGrassArray[i + offset].m_IsCut;
        }

        return batchedArray;
    }

    private void FillCutGrass() 
    {
        for (int i = 0; i < m_AllGrassArray.Length; i++) 
        {
            //don't bother check distance if grass is cut
            if (m_AllGrassArray[i].m_IsCut != -1)
            {
                float distance = Vector3.Distance(m_Robot.transform.position, m_AllGrassArray[i].m_Position);
                if (distance <= 10.0f)
                {
                    m_AllGrassArray[i].m_IsCut = -1;
                }
            }
        }
    }

    //march across plane and fill it with points
    private void MarchPlane() 
    {
        MeshRenderer meshRenderer = m_PlaneObject.GetComponent<MeshRenderer>();
        Vector3 center = meshRenderer.bounds.center;
        Vector3 size = meshRenderer.bounds.size;

        float xBeginning = center.x - (size.x * 0.5f);
        float zBeginning = center.z - (size.z * 0.5f);

        float xStep = size.x / m_GrassDensityWidth;
        float zStep = size.z / m_GrassDensityHeight;

        m_AllGrassArray = new Grass[m_GrassDensityWidth * m_GrassDensityHeight];

        for (int i = 0; i < m_GrassDensityWidth; i++) 
        {
            for (int j = 0; j < m_GrassDensityHeight; j++) 
            {
                float xPos = xBeginning + (xStep * i);
                float zPos = zBeginning + (zStep * j);

                Vector3 position = new Vector3(xPos, m_PlaneObject.transform.position.y, zPos);
                float isCut = ReadSplatMap(position, center, size);

                Grass grass;
                grass.m_Position = position;
                grass.m_IsCut = isCut;
                grass.m_Color = Color.green;
                grass.m_Transform = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);

                int grassIndex = (i * m_GrassDensityWidth) + j;
                m_AllGrassArray[grassIndex] = grass;
            }
        }
    }

    //return true if we place a grass boi (white(1))
    //float for the shader
    private float ReadSplatMap(Vector3 point, Vector3 worldCenter, Vector3 worldSize) 
    {
        Vector2Int coords = Map3DCoordToTextureCoord(point, worldCenter.x - (worldSize.x * 0.5f), worldCenter.x + (worldSize.x * 0.5f), worldCenter.z - (worldSize.z * 0.5f), worldCenter.z + (worldSize.z * 0.5f));

        Color color = m_SplatMap.GetPixel(coords.x, coords.y);

        if (color.r >= 0.9f)
        {
            return 1;
        }
        else 
        {
            return -1;
        }
    }

    private Vector2Int Map3DCoordToTextureCoord(Vector3 point, float worldTextureWidthMin, float worldTextureWidthMax, float worldTextureHeightMin, float worldTextureHeightMax)
    {
        float localXPos = point.x / (worldTextureWidthMax - worldTextureWidthMin);
        float localZPos = point.z / (worldTextureHeightMax - worldTextureHeightMin);

        Debug.Log("Local: " + worldTextureHeightMax);

        localXPos = Mathf.Clamp01((localXPos + 1) / 2.0f); //convert to 0-1 scale 
        localZPos = Mathf.Clamp01((localZPos + 1) / 2.0f); //convert to 0-1 scale 

        int xPixel = Mathf.FloorToInt(m_SplatMap.width * localXPos);
        int yPixel = Mathf.FloorToInt(m_SplatMap.height * localZPos);

        Vector2Int pixelCoord = new Vector2Int(xPixel, yPixel);

        return pixelCoord;
    }
}

public struct Grass 
{
    public Matrix4x4 m_Transform;
    public float m_IsCut;
    public Vector4 m_Color;
    public Vector3 m_Position;
}
