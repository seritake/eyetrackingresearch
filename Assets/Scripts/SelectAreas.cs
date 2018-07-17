using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Tobii.Gaming;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SelectAreas : MonoBehaviour
{
    private string path = "C:/Users/MEIP-users/project/heatmap.js/プロジェクト演習/coordinates.csv";

    private List<LineRenderer> lines = new List<LineRenderer>();
    public Camera camera;

    private bool isWriting = false;
    private int areaCounter = 0;
    private List<List<Vector3>> areaList = new List<List<Vector3>>();
    private int pointCount = 0;

    private bool enableDrawing = true;

    public GameObject LinePrefab;

    private bool isJudgeMode = false;

    public Image image;
    public Image anotherImage;
    public GameObject canvas;

    public InputField input;

    public Boolean recordFlag = false;
    StreamWriter sw;
    FileInfo fi;

    // Use this for initialization
    void Start () {
        //Display.displays[0].Activate(1024, 768, 70);
        //Display.displays[1].Activate(1280, 1024, 70);
        image.GetComponent<Image>().sprite = GetPhotos.getSprite();
        anotherImage.gameObject.SetActive(false);
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
            print(isInArea(0, camera.ScreenToWorldPoint(Input.mousePosition)));
        }

        if (recordFlag)
        {
            GazePoint gazePoint = TobiiAPI.GetGazePoint();
            if (gazePoint.IsValid)
            {
                sw = fi.AppendText();
                int xt = (int)(gazePoint.Screen.x - 200) * 954 / 1525;
                int yt = (int)(1070 - gazePoint.Screen.y) * 954 / 1070;
                String xs = xt.ToString();
                String ys = yt.ToString();
                sw.WriteLine(xs + "," + ys + "," + "0.3");
                sw.Flush();
                sw.Close();
            }
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
        lines[areaCounter].startWidth = 0.05f;
        lines[areaCounter].endWidth = 0.05f;
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

    public void StartMeasurement()
    {
        float waittime;
        try
        {
            waittime = float.Parse(input.text);
        }catch
        {
            return;
        }
        Invoke("StopMeasurement", waittime);
        anotherImage.gameObject.SetActive(true);
        anotherImage.GetComponent<Image>().sprite = GetPhotos.getSprite();

        fi = new FileInfo(path);

        if (File.Exists(path))
        {
            // 削除
            fi.Delete();
        }

        sw = fi.AppendText();
        sw.WriteLine("unity/unity_Data/Resources/photos/" +  GetPhotos.getName() + ".jpg");
        sw.Flush();
        sw.Close();
        recordFlag = true;
    }

    private void StopMeasurement()
    {
        anotherImage.gameObject.SetActive(false);
        recordFlag = false;
    }

    public void backToPhoto()
    {
        SceneManager.LoadScene("DisplayPhoto");
    }
}
