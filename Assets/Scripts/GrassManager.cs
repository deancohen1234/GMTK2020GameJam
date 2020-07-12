using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

public class GrassManager : MonoBehaviour
{
    [Header("Robot Properties")]
    public Robot m_Robot;
    public float m_EnergyPerGrass = 0.5f;
    public float m_CutRadius = 10.0f;

    [Header("Scene References")]
    public Mesh m_GrassMesh;
    public Material m_GrassMaterial;
    public GameObject m_PlaneObject;

    [Header("Grass Properties")]
    public Texture2D m_SplatMap;
    public int m_GrassDensityWidth = 30;
    public int m_GrassDensityHeight = 30;

    private Grass[] m_AllGrassArray;

    private MaterialPropertyBlock m_PropertyBlock;

    const int BATCH_MAX = 1023;
    const int BATCH_MAX_FLOAT = 1023;
    // Start is called before the first frame update
    void Start()
    {
        MarchPlane();

        m_PropertyBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        FillCutGrass();

        int batches = Mathf.CeilToInt((float)m_AllGrassArray.Length / BATCH_MAX_FLOAT);

        for (int i = 0; i < batches; ++i)
        {
            int batchCount = Mathf.Min(BATCH_MAX, m_AllGrassArray.Length - (BATCH_MAX * i));
            int start = Mathf.Max(0, i * BATCH_MAX);


            float[] batchedArray = GetBatchedArray(start, batchCount);
            Matrix4x4[] batchedMatrices = GetBatchedMatrices(start, batchCount);

            //m_PropertyBlock.SetVectorArray("_Color", m_ColorsArray);
            m_PropertyBlock.SetFloatArray("_HideGrass", batchedArray);

            Graphics.DrawMeshInstanced(m_GrassMesh, 0, m_GrassMaterial, batchedMatrices, batchCount, m_PropertyBlock);
        }
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
                    m_Robot.AddEnergy(m_EnergyPerGrass);
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

                Matrix4x4 matrix = Matrix4x4.identity;
                matrix.SetTRS(position, Quaternion.identity, Vector3.one);

                grass.m_Transform = matrix;

                int grassIndex = (i * m_GrassDensityHeight) + j;

                m_AllGrassArray[grassIndex] = grass;
            }
        }
    }

    //return true if we place a grass boi (black(0))
    //float for the shader
    private float ReadSplatMap(Vector3 point, Vector3 worldCenter, Vector3 worldSize) 
    {
        Vector2Int coords = Map3DCoordToTextureCoord(point, worldCenter.x - (worldSize.x * 0.5f), worldCenter.x + (worldSize.x * 0.5f), worldCenter.z - (worldSize.z * 0.5f), worldCenter.z + (worldSize.z * 0.5f));

        Color color = m_SplatMap.GetPixel(coords.x, coords.y);

        if (color.r <= 0.1f)
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
        float localXPos = point.x / (worldTextureWidthMax - worldTextureWidthMin); //percentage
        float localZPos = point.z / (worldTextureHeightMax - worldTextureHeightMin);

        localXPos = Mathf.Clamp01(localXPos + 0.5f); //convert to 0-1 scale
        localZPos = Mathf.Clamp01(localZPos + 0.5f); //convert to 0-1 scale

        int xPixel = Mathf.FloorToInt(m_SplatMap.width * localXPos);
        int yPixel = Mathf.FloorToInt(m_SplatMap.height * localZPos);

        Vector2Int pixelCoord = new Vector2Int(xPixel, yPixel);

        return pixelCoord;
    }

    public int GetCutGrass()
    {
        int cutGrass = 0;
        for (int i = 0; i < m_AllGrassArray.Length; i++) 
        {
            if (m_AllGrassArray[i].m_IsCut == -1) 
            {
                cutGrass++;
            }
        }

        return cutGrass;
    }

    public int GetTotalGrass() 
    {
        return m_AllGrassArray.Length;
    }
}

public struct Grass 
{
    public Matrix4x4 m_Transform;
    public float m_IsCut;
    public Vector4 m_Color;
    public Vector3 m_Position;
}
