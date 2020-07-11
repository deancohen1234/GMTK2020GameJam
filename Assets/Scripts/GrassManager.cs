using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        m_PropertyBlock = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        FillCutGrass();

        int batches = Mathf.CeilToInt((float)m_GrassDensity / BATCH_MAX_FLOAT);

        for (int i = 0; i < batches; ++i)
        {
            int batchCount = Mathf.Min(BATCH_MAX, m_GrassDensity - (BATCH_MAX * i));
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
            batchedMatrices[i] = m_MatrixArray[i + offset];
        }

        return batchedMatrices;
    }

    private float[] GetBatchedArray(int offset, int batchCount)
    {
        float[] batchedArray = new float[batchCount];

        for (int i = 0; i < batchCount; ++i)
        {
            batchedArray[i] = m_CutGrassArray[i + offset];
        }

        return batchedArray;
    }

    private void FillCutGrass() 
    {
        for (int i = 0; i < m_CutGrassArray.Length; i++) 
        {
            float distance = Vector3.Distance(m_Robot.transform.position, m_Positions[i]);
            if (distance <= 10.0f) 
            {
                m_CutGrassArray[i] = -1;
            }
            
        }
    }
}
