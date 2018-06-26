using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Tobii.Gaming;

public class TrackerEvaluation : MonoBehaviour {

    public GameObject leadingDot;

    private Vector3 startPoint = new Vector3(0.1f, 0.9f, 0);
    private Vector3 endRightPoint = new Vector3(0.9f, 0.9f, 0);
    private Vector3 endLeftPoint = new Vector3(0.1f, 0.1f, 0);
    private Vector3 endPoint = new Vector3(0.9f, 0.1f, 0);

    StreamWriter sw;
    FileInfo fi;

    private String dateTime;
    private Boolean recordFlag = true;

    // gazePlot map
    List<Vector3> gazePoints = new List<Vector3>();
    List<Vector3> movePoints = new List<Vector3>();
    public GameObject plotPrefab;
    public GameObject movePrefab;
    public GameObject canvas;


    // Use this for initialization
    void Start () {
        dateTime = DateTime.Now.ToString("ddhhmmss");
        fi = new FileInfo(Application.dataPath + "/leadingPointData" + dateTime + ".csv");
        sw = fi.AppendText();
        sw.WriteLine("point_x, point_y, eye_x, eye_y");
        sw.Flush();
        sw.Close();

        leadingDot.transform.position = Camera.main.ViewportToScreenPoint(startPoint);

        // First Move
        var moveHash1 = new Hashtable();
        moveHash1.Add("position", Camera.main.ViewportToScreenPoint(endRightPoint));
        moveHash1.Add("time", 8f);
        moveHash1.Add("delay", 1f);
        moveHash1.Add("easetype", "easeInOutQuint");
        iTween.MoveTo(leadingDot, moveHash1);

        // Second Move
        var moveHash2 = new Hashtable();
        moveHash2.Add("position", Camera.main.ViewportToScreenPoint(endLeftPoint));
        moveHash2.Add("time", 10f);
        moveHash2.Add("delay", 9f);
        moveHash2.Add("easetype", "easeInOutQuint");
        iTween.MoveTo(leadingDot, moveHash2);

        // Third Move
        var moveHash3 = new Hashtable();
        moveHash3.Add("position", Camera.main.ViewportToScreenPoint(endPoint));
        moveHash3.Add("time", 8f);
        moveHash3.Add("delay", 19f);
        moveHash3.Add("easetype", "easeInOutQuint");
        moveHash3.Add("oncompletetarget", this.gameObject);
        moveHash3.Add("oncomplete", "visualize");
        iTween.MoveTo(leadingDot, moveHash3);
    }
	
	// Update is called once per frame
	void Update () {
        if (recordFlag)
        {
            GazePoint gazePoint = TobiiAPI.GetGazePoint();
            if (gazePoint.IsValid)
            {
                sw = fi.AppendText();
                sw.WriteLine(leadingDot.transform.position.x + "," + leadingDot.transform.position.y + "," + gazePoint.Screen.x +
                    "," + gazePoint.Screen.y);
                sw.Flush();
                sw.Close();
            }
        }
    }


    private void visualize()
    {
        recordFlag = false;

        print("start reading...");

        // ファイル読み取り
        System.IO.StreamReader tReader = new System.IO.StreamReader(Application.dataPath + "/leadingPointData" + dateTime + ".csv");

        string line;

        bool skip = true;

        while ((line = tReader.ReadLine()) != null)
        {
            if (skip)
            {
                skip = false;
                continue;
            }
            var coord = line.Split(',');
            movePoints.Add(new Vector3(float.Parse(coord[0]), float.Parse(coord[1]), 0f));
            gazePoints.Add(new Vector3(float.Parse(coord[2]), float.Parse(coord[3]), 0f));
        }

        tReader.Close();

        // コルーチンスタート
        print("start plotting...");
        StartCoroutine("plotGaze");
    }

    private IEnumerator plotGaze()
    {
        for (int i = 0; i < gazePoints.Count; i++)
        {
            print(i);
            GameObject plot = Instantiate(plotPrefab) as GameObject;
            plot.transform.SetParent(canvas.transform, true);
            plot.transform.position = gazePoints[i];

            GameObject movePlot = Instantiate(movePrefab) as GameObject;
            movePlot.transform.SetParent(canvas.transform, true);
            movePlot.transform.position = movePoints[i];
            yield return null;
        }
        print("end");
        yield break;
    }
}
