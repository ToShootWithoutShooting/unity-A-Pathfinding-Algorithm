using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PathFind : MonoBehaviour
{
    [DllImport("PathFindCplusplus")]
    private static extern bool InitCPP(int w, int h, byte[] pathdata, int length);
    [DllImport("PathFindCplusplus")]
    private static extern bool ReleaseCPP();

    [DllImport("PathFindCplusplus")]
    private static extern bool FindCPP(Vector3 Start,Vector3 End, Vector2[] outPath,ref int pathCount);

    private bool isInitCPP = false;
    public Texture2D dataTex;
    byte[] data = null;
    List<Vector2> lstpath = new List<Vector2>();
    Point[,] map;
    int data_width = 0;
    void LoadData()
    {
        data_width = dataTex.width;
        map = new Point[data_width, data_width];
        Color32[] tempdata = dataTex.GetPixels32();
        data = new byte[tempdata.Length];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = tempdata[i].r;

        }
        int k = 0;
        for (int i = 0; i < data_width; i++)
        {
            for (int j = 0; j < data_width; j++)
            {
                map[i, j] = new Point(i, j, !isWalkable(i, j));
      
            }
        }

    }
    public bool isWalkable(Vector2 v)
    {
        int x = (int)v.x;
        int y = (int)v.y;
        return data[y * data_width + x] != 0;
    }
    public bool isWalkable(int x,int y)
    {
        return data[y * data_width + x] != 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadData();
        Point start = map[50, 50];
        //Point end = map[150, 200];
        Point end = map[1000, 1000];
        FindPath(start, end);
        ShowPath(start, end);

    }

    #region A*
    private void ShowPath(Point start, Point end)
    {
        Point temp = end;
        while (true)
        {
            if (temp.Parent == null)
                break;
            lstpath.Add(new Vector2(temp.X,temp.Y));
            temp = temp.Parent;
        }
        
    }
    private void FindPath(Point start, Point end)
    {
        List<Point> openList = new List<Point>();
        List<Point> closeList = new List<Point>();
        openList.Add(start);
        while (openList.Count > 0)
        {
            Point point = FindMinFOfPoint(openList);
            openList.Remove(point);
            closeList.Add(point);
            List<Point> surroundPoints = GetSurroundPoints(point);
            PointsFilter(surroundPoints, closeList);
            foreach (Point surroundPoint in surroundPoints)
            {
                if (openList.IndexOf(surroundPoint) > -1)
                {
                    float nowG = CalcG(surroundPoint, point);
                    if (nowG < surroundPoint.G)
                    {
                        surroundPoint.UpdateParent(point, nowG);
                    }
                }
                else
                {
                    surroundPoint.Parent = point;
                    CalcF(surroundPoint, end);
                    openList.Add(surroundPoint);
                }
            }
            //判断一下
            if (openList.IndexOf(end) > -1)
            {
                break;
            }
        }

    }

    private void PointsFilter(List<Point> src, List<Point> closeList)
    {
        foreach (Point p in closeList)
        {
            if (src.IndexOf(p) > -1)
            {
                src.Remove(p);
            }
        }
    }

    private List<Point> GetSurroundPoints(Point point)
    {
        Point up = null, down = null, left = null, right = null;
        Point lu = null, ru = null, ld = null, rd = null;
        if (point.Y < data_width - 1)
        {
            up = map[point.X, point.Y + 1];
        }
        if (point.Y > 0)
        {
            down = map[point.X, point.Y - 1];
        }
        if (point.X > 0)
        {
            left = map[point.X - 1, point.Y];
        }
        if (point.X < data_width - 1)
        {
            right = map[point.X + 1, point.Y];
        }
        if (up != null && left != null)
        {
            lu = map[point.X - 1, point.Y + 1];
        }
        if (up != null && right != null)
        {
            ru = map[point.X + 1, point.Y + 1];
        }
        if (down != null && left != null)
        {
            ld = map[point.X - 1, point.Y - 1];
        }
        if (down != null && right != null)
        {
            rd = map[point.X + 1, point.Y - 1];
        }
        List<Point> list = new List<Point>();
        if (down != null && down.IsWall == false)
        {
            list.Add(down);
        }
        if (up != null && up.IsWall == false)
        {
            list.Add(up);
        }
        if (left != null && left.IsWall == false)
        {
            list.Add(left);
        }
        if (right != null && right.IsWall == false)
        {
            list.Add(right);
        }
        if (lu != null && lu.IsWall == false && left.IsWall == false && up.IsWall == false)
        {
            list.Add(lu);
        }
        if (ld != null && ld.IsWall == false && left.IsWall == false && down.IsWall == false)
        {
            list.Add(ld);
        }
        if (ru != null && ru.IsWall == false && right.IsWall == false && up.IsWall == false)
        {
            list.Add(ru);
        }
        if (rd != null && rd.IsWall == false && right.IsWall == false && down.IsWall == false)
        {
            list.Add(rd);
        }
        return list;
    }

    private Point FindMinFOfPoint(List<Point> openList)
    {
        float f = float.MaxValue;
        Point temp = null;
        foreach (Point p in openList)
        {
            if (p.F < f)
            {
                temp = p;
                f = p.F;
            }
        }
        return temp;
    }

    private float CalcG(Point now, Point parent)
    {
        return Vector2.Distance(new Vector2(now.X, now.Y), new Vector2(parent.X, parent.Y)) + parent.G;
    }

    private void CalcF(Point now, Point end)
    {
        //F = G + H
        float h = Mathf.Abs(end.X - now.X) + Mathf.Abs(end.Y - now.Y);
        float g = 0;
        if (now.Parent == null)
        {
            g = 0;
        }
        else
        {
            g = Vector2.Distance(new Vector2(now.X, now.Y), new Vector2(now.Parent.X, now.Parent.Y)) + now.Parent.G;
        }
        float f = g + h;
        now.F = f;
        now.G = g;
        now.H = h;
    }

    #endregion

    private void OnDestroy()
    {
        if (isInitCPP)
        {
            ReleaseCPP();
        }
    }
    //测试函数 仅用于演示
    public bool TestFind(Vector2 begin,Vector2 end,List<Vector2> lstPath)
    {
        lstPath.Clear();

        if(!isWalkable(begin))
        {
            return false;
        }
        if (!isWalkable(end))
        {
            return false;
        }
        lstPath.Add(begin);
        lstPath.Add(new Vector2(890,143));
        lstPath.Add(new Vector2(868, 901));
        lstPath.Add(new Vector2(858, 901));
        lstPath.Add(new Vector2(661, 358));
        lstPath.Add(end);

        return true;
    }
    //寻路函数入口 
    public bool Find(Vector2 begin, Vector2 end, List<Vector2> lstPath)
    {
        lstPath.Clear();

        Point Start = map[(int)begin.x, (int)begin.y];
        //Point end = map[150, 200];
        Point End = map[(int)end.x, (int)end.y];
        FindPath(Start, End);
        ShowPath(Start, End);


        return false;
    }
    private void OnGUI()
    {
        DrawDebug();
        DrawButton();
    }

    //显示按钮
    void DrawButton()
    {

        if (GUI.Button(new Rect(0, 0, 100, 100), "Find Example"))
        {
            if (TestFind(new Vector2(50, 50), new Vector2(512, 512), lstpath))
            {
                Debug.Log("find success");
            }
            else
            {
                Debug.Log("find failed");
            }
        }

        if (GUI.Button(new Rect(0, 100, 100, 100), "Find"))
        {
            if (Find(new Vector2(50, 50), new Vector2(322, 536), lstpath))
            {
                Debug.Log("find success");
            }
            else
            {
                Debug.Log("find failed");
            }
        }
        if (GUI.Button(new Rect(0, 200, 100, 100), "FindCPP"))
        {
            if(!isInitCPP)
            {
                InitCPP(data_width, data_width, data, data.Length);
                isInitCPP = true;
            }
            Vector2[] allPath = new Vector2[1024];
            int path_len = allPath.Length;
            if (FindCPP(new Vector2(50, 50), new Vector2(322, 536), allPath, ref path_len))
            {
                lstpath.Clear();
                for (int i = 0; i < path_len; i++)
                {
                    lstpath.Add(allPath[i]);
                }
                Debug.Log("find success");
            }
            else
            {
                lstpath.Clear();
                Debug.Log("find failed");
            }
        }
    }
    //显示地图和路径
    void DrawDebug(float scale = 0.5f)
    {
        GUI.DrawTexture(new Rect(0, 0, dataTex.width*scale, dataTex.height*scale), dataTex);
        for (int i = 1; i < lstpath.Count; i++)
        {
            Vector2 last = lstpath[i - 1];
            Vector2 next = lstpath[i];
            float length = (next - last).magnitude;
            if (length > 4.0f)
            {
                int count = (int)(length*0.25f);
                Vector2 step = (next - last) / count;
                for (int j = 0; j < count; j++)
                {
                    Vector2 current = last + step * j;
                    GUI.DrawTexture(new Rect(current.x* scale, current.y* scale, 1, 1), Texture2D.whiteTexture);
                }
            }
            else
            {
                GUI.DrawTexture(new Rect(next.x* scale, next.y* scale, 1, 1), Texture2D.whiteTexture);
            }
        }
    }


}
