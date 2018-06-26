using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectAreas : MonoBehaviour {

    private List<LineRenderer> lines = new List<LineRenderer>();
    public Camera camera;

    private bool isWriting = false;
    private int areaCounter = 0;
    private List<List<Vector3>> areaList = new List<List<Vector3>>();
    private int pointCount = 0;

    private bool enableDrawing = true;

    public GameObject LinePrefab;

    private bool isJudgeMode = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!isJudgeMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                enableDrawing = true;
            }

            if (enableDrawing)
            {
                WriteLine();
            }
        }
        else
        {
            //print("area:" + areaList[0][0] + "point:" + camera.ScreenToWorldPoint(Input.mousePosition));
            //isInArea(0, camera.ScreenToWorldPoint(Input.mousePosition));
            print(isInArea(0, camera.ScreenToWorldPoint(Input.mousePosition)));
        }

	}

    private void WriteLine()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isWriting = false;
            ClearLine(areaCounter);
        }

        if (Input.GetMouseButton(0))
        {
            if (!isWriting)
            {
                pointCount = 0;
                CreateLineRenderer();
                areaList.Add(new List<Vector3>());
                isWriting = true;
            }
            Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = lines[areaCounter].transform.position.z;
            
            if (pointCount > 50 && Vector3.Distance(mousePosition, areaList[areaCounter][0]) < 0.1)
            {
                lines[areaCounter].endColor = Color.red;
                lines[areaCounter].startColor = Color.red;
                areaCounter++;
                enableDrawing = false;
                isWriting = false;
                return;
            }

            if (!areaList[areaCounter].Contains(mousePosition))
            {
                if (pointCount <= 50)
                {
                    pointCount++;
                }
                areaList[areaCounter].Add(mousePosition);
                lines[areaCounter].positionCount = areaList[areaCounter].Count;
                lines[areaCounter].SetPosition(lines[areaCounter].positionCount - 1, mousePosition);

            }
        }
    }

    private void ClearLine(int areaCounter)
    {
        if (lines[areaCounter] != null)
        {
            lines[areaCounter].positionCount = 0;
        }

        if (areaList[areaCounter] != null)
        {
            areaList[areaCounter].Clear();
        }
    }

    private void CreateLineRenderer()
    {
        print(areaCounter);
        GameObject obj = Instantiate(LinePrefab);
        lines.Add(obj.AddComponent<LineRenderer>());
        lines[areaCounter].positionCount = 0;
        lines[areaCounter].material = new Material(Shader.Find("Particles/Additive"));
        lines[areaCounter].startColor = Color.white;
        lines[areaCounter].endColor = Color.white;
        lines[areaCounter].startWidth = 0.2f;
        lines[areaCounter].endWidth = 0.2f;
        lines[areaCounter].useWorldSpace = true;
    }

    private float GetAngle(Vector2 from, Vector2 to)
    {
        float angle = Vector2.Angle(from, to);
        angle *= Mathf.Sign(V2NmlzCross((from), (to)));

        return angle;
    }

    private float V2NmlzCross(Vector2 v1, Vector2 v2)
    {
        return v1.normalized.x * v2.normalized.y - v1.normalized.y * v2.normalized.x;
    }

    private bool isInArea(int areaIndex, Vector2 point)
    {
        float angle = 0;
        for (int i = 0; i < areaList[areaIndex].Count - 1; i++)
        {
            Vector2 from = new Vector2(areaList[areaIndex][i].x, areaList[areaIndex][i].y);
            Vector2 to = new Vector2(areaList[areaIndex][i + 1].x, areaList[areaIndex][i + 1].y);
            angle += GetAngle(from - point, to - point);
        }

        return Math.Abs(angle) > 340;
    }

    public void ChangeMode()
    {
        isJudgeMode = true;
    }
}
