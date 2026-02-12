using System.Collections.Generic;
using EngineBase;

namespace Engine
{

    public class ClassicAttr
    {
        public int AttrId;
        public double AttrVal;
    }
    
    public class ClassicInfo
    {
        public int classicId;//Classics.xlsx表格中的 典籍id
        public int level;//典籍id对应的等级
        public List<ClassicAttr> classicAttrs = new List<ClassicAttr>();//当前加的属性总和  attr+attr2
        public List<ClassicAttr> attr = new List<ClassicAttr>();//attr字段所加属性s
        public List<ClassicAttr> attr2 = new List<ClassicAttr>();//attr2字段所加属性
        public int nextLevel;//下一等级  为0表示没有下一等级
        public long nextLevelCostGold;//升到下一等级所需金币   为0表示没有下一等级
        public List<ClassicAttr> nextAttrs = new List<ClassicAttr>();//下一等级 attr字段所加属性   为空表示没有下一等级
        public List<ClassicAttr> nextAttrs2 = new List<ClassicAttr>();//下一等级 attr2字段所加属性  为空表示没有下一等级
        public List<ClassicAttr> maxAttrs = new List<ClassicAttr>();//满级额外加成
    }

    public class GrimoireManager : TSingleton<GrimoireManager>
    {
        private Dictionary<int,ClassicInfo>  _grimoireDict = new Dictionary<int,ClassicInfo>();//int为classicId
        
        public bool firstSortClassic = true;
        public bool isChangeUI = false;//是否切换界面

        public void UpdateGrimoireDict(int classicid, ClassicInfo info)
        {
            _grimoireDict[classicid] = info;// 存在则更新数据，不在则添加数据
        }

        public Dictionary<int,ClassicInfo> GetGrimoireDict()
        {
            return _grimoireDict;
        }
        
    }
}