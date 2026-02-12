using FairyGUI;
using System;
using System.Collections.Generic;

namespace Engine
{
    public static class RED_FORMAT
    {
        public const int None = 0;
        public const int Normal = 1;
        public const int WarningMark = 2;
        public const int Numbers = 3;
    }

    public class Red
    {
        public GComponent dot;
        public int count;
        public GComponent item;
    }

    public class Layout
    {
        public RelationType relType;
        public float offsetX;
        public float offsetY;
    }

    public class Node
    {
        private static Dictionary<object, Red> RedDots = new Dictionary<object, Red>();

        public string group;
        public string id;
        public List<Node> children;
        public Node parent;
        public GComponent item;
        public string parentId;
        public Red rInfo;
        public int format = -1;

        public int number = -1;
        public string localKey;
        public RelationType dotRelType;
        public float dotOffsetX;
        public float dotOffsetY;

        public Node(string group, string id, int format = -1)
        {
            this.group = group;
            this.id = id;
            this.children = new List<Node>();
            this.format = format != -1 ? format : RED_FORMAT.Normal;
            DotDefaultLayout();
        }

        public bool IsRoot()
        {
            return this.parent == null;
        }

        public string Key()
        {
            if (this.localKey == null)
            {
                this.localKey = $"{this.group}:{id}";
            }

            return this.localKey;
        }

        public void SetItem(GComponent item)
        {
            if (this.item != null)
            {
                if (this.item.Equals(item))
                    return;

                DestroyRed(this.item);
            }

            this.item = item;

            if (this.number != -1)
            {
                SetNumber(this.number);
            }
        }

        public void SetParent(Node parent)
        {
            if (this.parent == parent)
            {
                return;
            }

            if (this.parent != null)
            {
                this.parent.RemoveChild(this);
            }

            this.parent = parent;
            if (parent != null)
            {
                parent.AddChild(this);
                this.parentId = parent.id;
            }
            else
            {
                this.parentId = null;
            }
        }

        public void AddChild(Node child)
        {
            this.children.Add(child);
            CountChildren();
        }

        public void RemoveChild(Node child)
        {
            this.children.Remove(child);
            CountChildren();
        }

        public void CountChildren()
        {
            int count = 0;
            foreach (Node node in this.children)
            {
                count += node.number;
            }

            SetNumber(count);
        }

        public void SetNumber(int number, bool silent = false)
        {
            number = Math.Max(0, number);
            if (number > 0)
            {
                CreateRedIfNot();
            }

            if (this.number != number)
            {
                this.number = number;
                UIRedsManager.Instance.SetLocalNum(this, number);

                if (!silent && this.parent != null)
                {
                    this.parent.CountChildren();
                }
            }

            Red red = FindRed();
            if (red != null)
            {
                FormatNumber(red.dot, number);
            }
        }

        public void FormatNumber(GComponent dot, int number)
        {
            // implementation for FormatNumber method
            if (dot.isDisposed)
                return;

            var t = dot.GetTransition("t0");
            var status = dot.GetController("status");
            var txt_num = dot.GetChild("txt_num");

            switch (this.format)
            {
                case RED_FORMAT.Numbers:
                {
                    if (number > 1)
                    {
                        status.selectedIndex = RED_FORMAT.Numbers;
                        txt_num.text = number > 99 ? "99+" : number.ToString();
                    }
                    else
                    {
                        status.selectedIndex = number == 1 ? RED_FORMAT.WarningMark : RED_FORMAT.None;
                    }
                }
                    break;
                case RED_FORMAT.WarningMark:
                    status.selectedIndex = number > 0 ? RED_FORMAT.WarningMark : RED_FORMAT.None;
                    break;
                default:
                    status.selectedIndex = number > 0 ? RED_FORMAT.Normal : RED_FORMAT.None;
                    break;
            }

            if (status.selectedIndex == RED_FORMAT.Normal || status.selectedIndex == RED_FORMAT.None)
            {
                t.Stop();
            }
            else
            {
                t.Play(-1, 0, null);
            }
        }

        public void Evaluate()
        {
            if (this.item == null)
            {
                Release();
                Dispose();
            }
        }

        public Red AddRedInfo(GComponent item)
        {
            Red red;
            if (!RedDots.TryGetValue(item, out red))
            {
                red = new Red();
                red.dot = null;
                red.count = 0;
                red.item = item;
                RedDots[item] = red;
            }

            if (red.dot == null)
            {
                red.dot = (GComponent)UIPackage.CreateObject("Common", "ComRedPoint");
                item.AddChild(red.dot);
                PlaceDot(item, red.dot);
            }

            red.count++;
            return red;
        }

        public void DotDefaultLayout()
        {
            this.dotRelType = RelationType.Right_Right;
            this.dotOffsetX = 0;
            this.dotOffsetY = 0;
        }

        public void SetDotLayout(Layout layout)
        {
            this.dotRelType = layout.relType;
            this.dotOffsetX = layout.offsetX;
            this.dotOffsetY = layout.offsetY;
            if (this.rInfo != null)
            {
                PlaceDot(this.item, this.rInfo.dot);
            }
        }

        public void CreateRedIfNot()
        {
            if (this.rInfo == null && this.item != null)
            {
                if (this.item.isDisposed)
                {
                    LogUtils.LogWarning("attending to red for disposed item" + this.item);
                    return;
                }

                this.rInfo = AddRedInfo(this.item);
            }
        }

        public Red FindRed()
        {
            if (this.item != null)
            {
                Red red;
                if (RedDots.TryGetValue(this.item, out red))
                {
                    return red;
                }
            }

            return null;
        }

        public void DestroyRed(GComponent item)
        {
            Red rInfo;
            if (RedDots.TryGetValue(item, out rInfo) && this.rInfo == rInfo)
            {
                rInfo.count--;
                if (rInfo.count <= 0)
                {
                    item.RemoveChild(rInfo.dot);
                    rInfo.dot.Dispose();
                    rInfo.dot = null;
                }

                this.rInfo = null;
            }
        }

        public void PlaceDot(GObject parent, GObject dot)
        {
            dot.AddRelation(parent, this.dotRelType);
            if (this.dotRelType == RelationType.Right_Right)
            {
                dot.SetXY(parent.width + this.dotOffsetX, this.dotOffsetY);
            }
            else if (this.dotRelType == RelationType.Left_Left)
            {
                dot.SetXY(parent.width - this.dotOffsetX, this.dotOffsetY);
            }
        }

        public void Release()
        {
            List<Node> children = new List<Node>(this.children);
            foreach (Node child in children)
            {
                child.SetParent(this.parent);
            }

            SetParent(null);
        }

        public void Dispose()
        {
            SetItem(null);
            this.parent = null;
            this.parentId = null;
            this.number = 0;
            this.group = null;
            this.id = null;
            this.children = null;
        }
    }
}