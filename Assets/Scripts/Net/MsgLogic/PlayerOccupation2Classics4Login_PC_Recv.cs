using System.Collections.Generic;
using EngineBase;
using msg;

namespace Engine
{
    public class PlayerOccupation2Classics4Login_PC_Recv : IReceiver
    {
        public PlayerOccupation2Classics4Login_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerOccupation2Classics4Login_PC;
        }

        public void Process()
        {
            foreach (var item in msg.ClassicsList)
            {
                ClassicInfo info = new ClassicInfo();
                info.classicId = item.ClassicId;
                info.level = item.Level;

                //当前加的属性总和
                List<ClassicAttr> allAttrList = new List<ClassicAttr>(); 
                foreach (var allAttrItem in item.AllAttrsList)
                {
                    ClassicAttr attr = new ClassicAttr();
                    attr.AttrId = (int)allAttrItem.AttrId;
                    attr.AttrVal = allAttrItem.AttrValue;
                    allAttrList.Add(attr);
                }
                info.classicAttrs = allAttrList;
                
                //当前属性1
                List<ClassicAttr> attrList = new List<ClassicAttr>(); 
                foreach (var allAttrItem in item.AttrList)
                {
                    ClassicAttr attr = new ClassicAttr();
                    attr.AttrId = (int)allAttrItem.AttrId;
                    attr.AttrVal = allAttrItem.AttrValue;
                    attrList.Add(attr);
                }
                info.attr = attrList;
                
                //当前属性2
                List<ClassicAttr> attr2List = new List<ClassicAttr>(); 
                foreach (var allAttrItem in item.Attr2List)
                {
                    ClassicAttr attr = new ClassicAttr();
                    attr.AttrId = (int)allAttrItem.AttrId;
                    attr.AttrVal = allAttrItem.AttrValue;
                    attr2List.Add(attr);
                }
                info.attr2 = attr2List;

                info.nextLevel = item.NextLevel;
                info.nextLevelCostGold = item.NextCostGold;
                
                //下一等级属性1
                List<ClassicAttr> nextLvAttrList = new List<ClassicAttr>();
                foreach (var attrItem in item.NextAttrList)
                {
                    ClassicAttr attr = new ClassicAttr();
                    attr.AttrId = (int)attrItem.AttrId;
                    attr.AttrVal = attrItem.AttrValue;
                    nextLvAttrList.Add(attr);
                }
                info.nextAttrs = nextLvAttrList;
                
                //下一等级属性2
                List<ClassicAttr> nextLvAttrList2 = new List<ClassicAttr>();
                foreach (var attrItem2 in item.NextAttr2List)
                {
                    ClassicAttr attr = new ClassicAttr();
                    attr.AttrId = (int)attrItem2.AttrId;
                    attr.AttrVal = attrItem2.AttrValue;
                    nextLvAttrList2.Add(attr);
                }
                info.nextAttrs2 = nextLvAttrList2;
                
                //满级属性
                List<ClassicAttr> maxAttrList = new List<ClassicAttr>();
                foreach (var maxAttrItem in item.TopExtraAttrList)
                {
                    ClassicAttr attr = new ClassicAttr();
                    attr.AttrId = (int)maxAttrItem.AttrId;
                    attr.AttrVal = maxAttrItem.AttrValue;
                    maxAttrList.Add(attr);
                }
                info.maxAttrs = maxAttrList;
                
                // GrimoireManager.Instance.UpdateGrimoireDict(item.ClassicId,item.Level);
                GrimoireManager.Instance.UpdateGrimoireDict(info.classicId, info);
            }
            
            if(msg.SplitInfo.IsEnd)
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CLASSIC_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerOccupation2Classics4Login_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}