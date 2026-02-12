using System.Collections.Generic;
using UnityEngine;

public class DijkstraHelp
{
    public const int c_path = 1;
    public const int c_no_path = 2000;
    public const int c_max_size = 1000;

    private int m_nLength = 0;
    private int[,] m_arrG = null;

    public void Clear()
    {
        m_nLength = 0;
        m_arrG = null;
    }

    /*
        var help = new DijkstraHelp();
        help.Init(7);
        var start = Random.Range(0, 7);
        var end = Random.Range(0, 7);
        
        LogUtils.LogWarning("Find: " +  start + " " + end);
        
        var lstPath = help.GetShortedPath(start, end);
        
        foreach (var path in lstPath)
        {
            LogUtils.LogWarning(path.ToString());
        }
     */
    public bool Init(int nLength)
    {
        // 位置数量/设定通路, 建议走配置
        if (nLength <= 0)
        {
            return false;
        }
        
        Clear();

        m_nLength = nLength;

        m_arrG = new int[m_nLength, m_nLength];

        for (int i = 0; i < m_nLength; i++)
        {
            for (int j = 0; j < m_nLength; j++)
            {
                m_arrG[i, j] = c_no_path;
            }
        }
        
        // 设定通路
        for (int i = 0; i < m_nLength - 1; i++)
        {
            m_arrG[i, i + 1] = c_path;
            m_arrG[i + 1, i] = c_path;
        }
        
        m_arrG[0, m_nLength - 1] = c_path;
        m_arrG[m_nLength - 1, 0] = c_path;

        return true;
    }

    public void SetPath(int a, int b, int cost)
    {
        m_arrG[a, b] = cost;
    }

    // 从某一源点出发，找到到某一结点的最短路径
    public List<int> GetShortedPath(int start, int end)
    {
        if (start < 0 || start >= m_nLength)
        {
            return null;
        }

        if (end < 0 || end >= m_nLength)
        {
            return null;
        }

        int[] path = new int[m_nLength];

        for (int i = 0; i < m_nLength; i++)
        {
            path[i] = 0;
        }

        bool[] s = new bool[m_nLength]; //表示找到起始结点与当前结点间的最短路径
        int min;  //最小距离临时变量
        int curNode = 0; //临时结点，记录当前正计算结点
        int[] dist = new int[m_nLength];
        int[] prev = new int[m_nLength];

        //初始结点信息
        for (int v = 0; v < m_nLength; v++)
        {
            s[v] = false;
            dist[v] = m_arrG[start, v];
            if (dist[v] > c_max_size)
            {
                prev[v] = 0;
            }
            else
            {
                prev[v] = start;
            }
        }

        path[0] = end;
        dist[start] = 0;
        s[start] = true;

        //主循环
        for (int i = 1; i < m_nLength; i++)
        {
            min = c_max_size;

            for (int w = 0; w < m_nLength; w++)
            {
                if (!s[w] && dist[w] < min)
                {
                    curNode = w;
                    min = dist[w];
                }
            }

            s[curNode] = true;

            for (int j = 0; j < m_nLength; j++)
            {
                if (!s[j] && min + m_arrG[curNode, j] < dist[j])
                {
                    dist[j] = min + m_arrG[curNode, j];
                    prev[j] = curNode;
                }
            }
        }

        //输出路径结点
        int e = end, step = 0;

        while (e != start)
        {
            step++;
            path[step] = prev[e];
            e = prev[e];

            if (step + 1 >= m_nLength)
            {
                break;
            }
        }

        for (int i = step; i > step / 2; i--)
        {
            int temp = path[step - i];
            path[step - i] = path[i];
            path[i] = temp;
        }

        if (dist[end] >= c_no_path)
        {
            return null;
        }

        List<int> lstPath = new List<int>();

        for (int i = 0; i < m_nLength; i++)
        {
            lstPath.Add(path[i]);

            if (path[i] == end)
            {
                break;
            }
        }

        return lstPath;
    }
}
