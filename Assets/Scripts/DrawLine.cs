using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{

    [SerializeField]
    protected LineRenderer m_LineRenderer;
    [SerializeField]
    protected Camera m_Camera;

    private bool isWriting = false;
    private bool writeFlag = true;
    private int pointCount = 0;

    private List<List<Vector3>> areaList = new List<List<Vector3>>();
    private int areaCounter = 0;


    public virtual LineRenderer lineRenderer
    {
        get
        {
            return m_LineRenderer;
        }
    }

    public virtual new Camera camera
    {
        get
        {
            return m_Camera;
        }
    }

    protected virtual void Awake()
    {
        if (m_LineRenderer == null)
        {
            Debug.LogWarning("DrawLine: Line Renderer not assigned, Adding and Using default Line Renderer.");
            CreateDefaultLineRenderer();
        }
        if (m_Camera == null)
        {
            Debug.LogWarning("DrawLine: Camera not assigned, Using Main Camera or Creating Camera if main not exists.");
            CreateDefaultCamera();
        }
    }

    protected virtual void Update()
    {
        if (writeFlag)
        {
            WriteLine();
        }

    }

    protected virtual void WriteLine()
    {
        if (!isWriting)
        {
            areaList.Add(new List<Vector3>());
            isWriting = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isWriting = false;
            Reset();
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = m_Camera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = m_LineRenderer.transform.position.z;

            if (pointCount > 50)
            {
                if (Vector3.Distance(mousePosition, areaList[areaCounter][0]) < 0.1)
                {
                    writeFlag = false;
                    areaCounter++;
                    return;
                }
            }

            if (!areaList[areaCounter].Contains(mousePosition))
            {
                areaList[areaCounter].Add(mousePosition);
                m_LineRenderer.positionCount = areaList[areaCounter].Count;
                m_LineRenderer.SetPosition(m_LineRenderer.positionCount - 1, mousePosition);
                if (pointCount <= 50)
                {
                    pointCount++;
                }
            }
        }
    }

    protected virtual void Reset()
    {
        if (m_LineRenderer != null)
        {
            m_LineRenderer.positionCount = 0;
        }

        if (areaList[areaCounter] != null)
        {
            areaList[areaCounter].Clear();
        }
    }

    protected virtual void CreateDefaultLineRenderer()
    {
        m_LineRenderer = gameObject.AddComponent<LineRenderer>();
        m_LineRenderer.positionCount = 0;
        m_LineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        m_LineRenderer.startColor = Color.red;
        m_LineRenderer.endColor = Color.white;
        m_LineRenderer.startWidth = 0.3f;
        m_LineRenderer.endWidth = 0.3f;
        m_LineRenderer.useWorldSpace = true;
    }

    protected virtual void CreateDefaultCamera()
    {
        m_Camera = Camera.main;
        if (m_Camera == null)
        {
            m_Camera = gameObject.AddComponent<Camera>();
        }
        m_Camera.orthographic = true;
    }

}