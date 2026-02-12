using System;
using System.Collections.Generic;
using FairyGUI;

namespace Engine
{
    public class Group
    {
        public List<Node> member;
        public string groupName;
    }

    public class UIRedsRoot
    {
        public Dictionary<string, Group> reds;
        public bool useLocal;

        public UIRedsRoot()
        {
            reds = new Dictionary<string, Group>();
            useLocal = false;
        }

        public Node AddNode(string gName, string id, GComponent item, Node parent, int number = -1)
        {
            Group group = GetOrAddGroup(gName);
            Node node = GetOrAddNode(group, id);
            InternalSetNode(group, node, item, parent, number);
            return node;
        }

        public void PreAddNode(string gName, string id, string pid, int number, int format = -1)
        {
            Group group = GetOrAddGroup(gName);
            Node parent = FindNode(gName, pid);
            Node node = GetOrAddNode(group, id, format);
            if (parent == null)
            {
                node.parentId = pid;
            }

            InternalSetCleanNode(group, node, parent, number);
        }

        public Node AddNodeWithPID(string gName, string id, GComponent item, string pId, Layout dotLayout, int number = -1)
        {
            Node parent = FindNode(gName, pId);
            Node node = AddNode(gName, id, item, parent, number);
            if (node != null)
            {
                if (parent == null)
                {
                    node.parentId = pId;
                }

                if (dotLayout != null)
                {
                    node.SetDotLayout(dotLayout);
                }
            }

            return node;
        }

        public void SetNodeItem(string gName, string id, GComponent item, Layout dotLayout)
        {
            Node node = FindNode(gName, id);
            if (node != null)
            {
                node.SetItem(item);
                if (dotLayout != null)
                {
                    node.SetDotLayout(dotLayout);
                }
            }
        }

        public void SetNodeNumber(string gName, string id, int number, int format = -1)
        {
            Node node = FindNode(gName, id);
            if (node != null)
            {
                if (format != -1)
                {
                    node.format = format;
                }

                node.SetNumber(number);
            }
        }

        public void ClearAll()
        {
            foreach (var group in reds.Values)
            {
                if (group != null)
                {
                    ClearGroup(group);
                }
            }

            reds.Clear();
        }

        public void ClearGroupByName(string gName)
        {
            if (reds.TryGetValue(gName, out Group group))
            {
                ClearGroup(group);
                reds.Remove(gName);
            }
        }

        public void ClearGroup(Group group)
        {
            foreach (var node in group.member)
            {
                RemoveNode(node, true);
            }

            group.member.Clear();
        }

        public void ClearGroupNumber(string gName)
        {
            if (reds.TryGetValue(gName, out Group group))
            {
                foreach (var node in group.member)
                {
                    node.SetNumber(0, true);
                }
            }
        }

        private void InternalSetNode(Group group, Node node, GComponent item, Node parent, int number = -1)
        {
            node.SetItem(item);
            InternalSetCleanNode(group, node, parent, number == -1 ? GetLocalNum(node) : number);
        }

        private void InternalSetCleanNode(Group group, Node node, Node parent, int number = -1)
        {
            node.SetNumber(number == -1 ? 0 : number);
            node.SetParent(parent);
            foreach (var gNode in group.member)
            {
                if (gNode != node && gNode.parentId == node.id)
                {
                    gNode.SetParent(node);
                }
            }
        }

        private int GetLocalNum(Node node)
        {
            if (!useLocal)
                return 0;

            string data = LocalSave.GetStringWithAccount(node.Key(), "");
            if (!string.IsNullOrEmpty(data))
                return int.Parse(data);
            else
                return 0;
        }

        public void SetLocalNum(Node node, int num)
        {
            if (useLocal)
            {
                LocalSave.SetStringWithAccount(node.Key(), num.ToString());
            }
        }

        private Group GetOrAddGroup(string gName)
        {
            if (!reds.TryGetValue(gName, out Group group))
            {
                group = new Group();
                group.groupName = gName;
                group.member = new List<Node>();
                reds[gName] = group;
            }

            return group;
        }

        private Node GetOrAddNode(Group group, string id, int format = -1)
        {
            Node node = group.member.Find(n => n.id.Equals(id));
            if (node == null)
            {
                node = new Node(group.groupName, id, format);
                group.member.Add(node);
            }

            return node;
        }

        private Node FindNode(string gName, string id)
        {
            if (reds.TryGetValue(gName, out Group group))
            {
                return group.member.Find(n => n.id.Equals(id));
            }

            return null;
        }

        private void RemoveNodeById(string gName, string id, bool silent)
        {
            if (reds.TryGetValue(gName, out Group group))
            {
                Node node = group.member.Find(n => n.id.Equals(id));
                if (node != null)
                {
                    if (!silent)
                        node.Release();
                    node.Dispose();
                    group.member.Remove(node);
                }
            }
        }

        private void RemoveNode(Node node, bool silent)
        {
            RemoveNodeById(node.group, node.id, silent);
        }
    }
}