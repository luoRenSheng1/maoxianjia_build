using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    public class ClassicsLevelUp_SCRecv : IReceiver
    {
        public ClassicsLevelUp_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClassicsLevelUp_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                ClassicInfo info = new ClassicInfo();
                info.classicId = msg.Classic.ClassicId;
                info.level = msg.Classic.Level;
                
                //当前加的属性总和
                List<ClassicAttr> allAttrList = new List<ClassicAttr>(); 
                foreach (var allAttrItem in msg.Classic.AllAttrsList)
                {
                    ClassicAttr attr = new ClassicAttr();
                    attr.AttrId = (int)allAttrItem.AttrId;
                    attr.AttrVal = allAttrItem.AttrValue;
                    allAttrList.Add(attr);
                }
                info.classicAttrs = allAttrList;
                
                //当前属性1
                List<ClassicAttr> attrList = new List<ClassicAttr>(); 
                foreach (var allAttrItem in msg.Classic.AttrList)
                {
                    ClassicAttr attr = new ClassicAttr();
                    attr.AttrId = (int)allAttrItem.AttrId;
                    attr.AttrVal = allAttrItem.AttrValue;
                    attrList.Add(attr);
                }
                info.attr = attrList;
                
                //当前属性2
                List<ClassicAttr> attr2List = new List<ClassicAttr>();
                foreach (var allAttrItem in msg.Classic.Attr2List)
                {
                    ClassicAttr attr = new ClassicAttr();
                    attr.AttrId = (int)allAttrItem.AttrId;
                    attr.AttrVal = allAttrItem.AttrValue;
                    attr2List.Add(attr);
                }
                info.attr2 = attr2List;
                
                info.nextLevel = msg.Classic.NextLevel;
                info.nextLevelCostGold = msg.Classic.NextCostGold;
                
                //下一等级属性1
                List<ClassicAttr> nextAttrList = new List<ClassicAttr>();
                foreach (var attrItem in msg.Classic.NextAttrList)
                {
                    ClassicAttr attr = new ClassicAttr();
                    attr.AttrId = (int)attrItem.AttrId;
                    attr.AttrVal = attrItem.AttrValue;
                    nextAttrList.Add(attr);
                }
                info.nextAttrs = nextAttrList;
                
                //下一等级属性2
                List<ClassicAttr> nextAttrList2 = new List<ClassicAttr>();
                foreach (var attrItem2 in msg.Classic.NextAttr2List)
                {
                    ClassicAttr attr = new ClassicAttr();
                    attr.AttrId = (int)attrItem2.AttrId;
                    attr.AttrVal = attrItem2.AttrValue;
                    nextAttrList2.Add(attr);
                }
                info.nextAttrs2 = nextAttrList2;
                
                //满级属性
                List<ClassicAttr> maxAttrList = new List<ClassicAttr>();
                foreach (var maxAttrItem in msg.Classic.TopExtraAttrList)
                {
                    ClassicAttr attr = new ClassicAttr();
                    attr.AttrId = (int)maxAttrItem.AttrId;
                    attr.AttrVal = maxAttrItem.AttrValue;
                    maxAttrList.Add(attr);
                }
                info.maxAttrs = maxAttrList;
                
                // GrimoireManager.Instance.UpdateGrimoireDict(msg.Classic.ClassicId,msg.Classic.Level);
                GrimoireManager.Instance.UpdateGrimoireDict(info.classicId,info);
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                
                //本次的真实升级次数
                int levelups = msg.FinalLevelups;
                Debug.LogWarningFormat("服务器反馈秘典升级次数：levelups={0}", levelups);
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CLASSIC_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(10039);
            }
        }

        public bool Read(BaseStructRecv  mRecv)
        {
            msg = ClassicsLevelUp_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}