using BigMap;
using Common;
using CommonEx;
using FairyGUI;
using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using Village;

namespace Engine
{
    public static class UIExtensions
    {
        private static Dictionary<EN_DAMAGE_TYPE, int> _flyHurtDict = new Dictionary<EN_DAMAGE_TYPE, int>();
        private static StringBuilder _stringBuilder = new StringBuilder();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        public static void SetUISafeAreaOffset(this GComponent component)
        {
            float topOffset = 0;
            float bottomOffset = 0;
#if USE_WEIXIN
                if (SDKInterface.Instance.IsMinigamePlatform(MinigamePlatform.ios) 
                    || SDKInterface.Instance.IsMinigamePlatform(MinigamePlatform.android) 
                    || SDKInterface.Instance.IsMinigamePlatform(MinigamePlatform.devtools))
                {
                    var sysInfo = SDKInterfaceWeiXin.Instance.GetSystemInfoSync();
                    bool len = (sysInfo.screenHeight / sysInfo.screenWidth) > 1.8f;
                    var menuRect = SDKInterfaceWeiXin.Instance.GetMenuButtonBoundingClientRect();
                    if (SDKInterface.Instance.IsMinigamePlatform(MinigamePlatform.ios))
                    {
                        topOffset = (int)(menuRect.bottom * (Screen.height / sysInfo.screenHeight)) + (len ? 20 : 15); //优化显示多偏移20个像素
                    }
                    else
                    {
                        topOffset = (int)(menuRect.bottom * sysInfo.pixelRatio) + (len ? 20 : 15); //优化显示多偏移20个像素
                    }
                    var offset = (int)(sysInfo.screenHeight - sysInfo.safeArea.bottom);
                    bottomOffset = Mathf.Clamp(offset, 0, 60) + 3; //优化显示多偏移3个像素
                }
#else
            //topCorrectionOffset ---- 1696, bottomCorrectionOffset ---- 68, Screen.width --- 720, Screen.height --- 1280
            int topCorrectionOffset = (int)Screen.safeArea.yMax;
            int bottomCorrectionOffset = (int)Screen.safeArea.yMin;
            if (topCorrectionOffset != Screen.height)
            {
                topOffset = (int)Math.Abs(topCorrectionOffset - Screen.height * 0.96f);
                //Debug.LogFormat("topOffset：{0}", topOffset);
                topOffset = Mathf.Max(30f, topOffset);
            }

            if (bottomCorrectionOffset != 0)
            {
                bottomOffset = bottomCorrectionOffset * 0.52f;
            }
#endif
            component.x = 0;
            component.y = topOffset;
            component.SetSize(GRoot.inst.width, GRoot.inst.height - topOffset - bottomOffset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctr"></param>
        /// <param name="index"></param>
        public static void SetSelectedIndexEx(this Controller ctr, int index)
        {
            if (null != ctr)
            {
                var selectedIndex = ctr.selectedIndex;
                if (selectedIndex == index)
                {
                    ctr.onChanged.Call();
                }
                else
                {
                    ctr.selectedIndex = index;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctr"></param>
        /// <param name="page"></param>
        public static void SetSelectedPageEx(this Controller ctr, string page)
        {
            if (null != ctr)
            {
                var selectedIndex = ctr.selectedPage;
                if (selectedIndex.Equals(page))
                {
                    ctr.onChanged.Call();
                }
                else
                {
                    ctr.selectedPage = page;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="vo"></param>
        public static void InitUI(this UI_BarHp t, MapObjectAttr vo)
        {
            if (vo == null)
            {
                return;
            }

            t.hpBar.tweenBar.max = vo.HPMax;
            t.hpBar.tweenBar.value = vo.HP;
            t.hpBar.tweenBar.visible = false;

            t.hpBar.tweenBar2.max = vo.HPMax;
            t.hpBar.tweenBar2.value = vo.HP;
            t.hpBar.tweenBar2.visible = false;

            t.hpBar.hpBar.max = vo.HPMax;
            t.hpBar.hpBar.value = vo.HP;
            // t.valueText.SetVar("value", vo.HP.ToString()).SetVar("max", vo.HP.ToString()).FlushVars();
            t.hpBar.HPText?.SetVar("value", StringUtils.FormatCurrency(vo.HP)).FlushVars();
            t.hpBar.ATKText.SetVar("value", StringUtils.FormatCurrency(vo.Atk)).FlushVars();
        }

        public static void UpdateUI(this UI_BarHp t, MapObjectAttr vo, bool isTween)
        {
            if (vo == null)
            {
                return;
            }

            if (isTween)
            {
                t.hpBar.tweenBar.visible = true;
                t.hpBar.tweenBar.max = vo.HPMax;
                t.hpBar.tweenBar?.TweenValue(vo.HP, 0.8f);

                t.hpBar.tweenBar2.visible = true;
                t.hpBar.tweenBar2.max = vo.HPMax;
                t.hpBar.tweenBar2?.TweenValue(vo.HP, 0.8f);
            }
            t.hpBar.hpBar.value = vo.HP;
            t.hpBar.hpBar.max = vo.HPMax;
            t.hpBar.hpBar.EnsureBoundsCorrect();
            // Debug.LogWarningFormat(" UpdateUI max:{0}  value:{1}",vo.HPMax, vo.HP);
            t.hpBar.HPText?.SetVar("value", StringUtils.FormatCurrency(vo.HP)).FlushVars();
            t.hpBar.ATKText.SetVar("value", StringUtils.FormatCurrency(vo.Atk)).FlushVars();
        }

        public static void InitMonsterHP(this UI_BossBlood t, MapObjectAttr vo)
        {
            if (vo == null)
            {
                return;
            }
            
            t.tweenBar.max = vo.HPMax;
            t.tweenBar.value = vo.HP;
            t.tweenBar.visible = false;
            // t.tweenBar.GetChild("bar").asLoader.url = "ui://Common/3";

            t.tweenBar2.max = vo.HPMax;
            t.tweenBar2.value = vo.HP;
            t.tweenBar2.visible = false;

            t.hpBar.max = vo.HPMax;
            t.hpBar.value = vo.HP;
        }

        public static void UpdateMonsterHP(this UI_BossBlood t, MapObjectAttr vo, bool isTween)
        {
            if (vo == null)
            {
                return;
            }

            if (isTween)
            {
                t.tweenBar.visible = true;
                t.tweenBar.max = vo.HPMax;
                t.tweenBar?.TweenValue(vo.HP, 0.8f);

                t.tweenBar2.visible = true;
                t.tweenBar2.max = vo.HPMax;
                t.tweenBar2?.TweenValue(vo.HP, 0.8f);
            }
            t.hpBar.value = vo.HP;
            t.hpBar.max = vo.HPMax;
            t.hpBar.EnsureBoundsCorrect();
        }

        private static Dictionary<int, int> dictDamageFly = new Dictionary<int, int>();

        /// <summary>
        /// 伤害飘字
        /// </summary>
        /// <param name="t"></param>
        /// <param name="obj"></param>
        /// <param name="damage"></param>
        public static void DamageFly(this UI_BarHp t, MapObject obj, UnitDamageVo damage)
        {
            var objRoot = MapObjectManager.Instance.GetMapObjectSceneFlyRootTrans();

            if (objRoot == null)
            {
                return;
            }

            if (damage.damageType == EN_DAMAGE_TYPE.RECOVERY)
            {
                Debug.LogWarning("=qa测试后面删除====id=" + damage.targetID + "======damage=  +" + damage.damage);
            }
            else
            {
                Debug.LogWarning("=qa测试后面删除====id=" + damage.targetID + "======damage=  -" + damage.damage);
            }



            if (damage.damageType == EN_DAMAGE_TYPE.RECOVERY)
            {
                if (_flyHurtDict.ContainsKey(damage.damageType))
                {
                    int count = _flyHurtDict[damage.damageType];
                    if (count >= 5)
                    {
                        return;
                    }
                    else
                    {
                        _flyHurtDict[damage.damageType]++;
                    }
                }
                else
                {
                    _flyHurtDict.Add(damage.damageType, 1);
                }
            }
            else if (damage.damageType == EN_DAMAGE_TYPE.Jouk)  //闪避
            {
                if (_flyHurtDict.ContainsKey(damage.damageType))
                {
                    int count = _flyHurtDict[damage.damageType];
                    if (count >= 5)
                    {
                        return;
                    }
                    else
                    {
                        _flyHurtDict[damage.damageType]++;
                    }
                }
                else
                {
                    _flyHurtDict.Add(damage.damageType, 1);
                }
            }
            else
            {
                if (_flyHurtDict.ContainsKey(damage.damageType))
                {
                    int count = _flyHurtDict[damage.damageType];
                    if (count >= 15)
                    {
                        return;
                    }
                    else
                    {
                        _flyHurtDict[damage.damageType]++;
                    }
                }
                else
                {
                    _flyHurtDict.Add(damage.damageType, 1);
                }
            }

            var damageFly = MapObjectManager.Instance.Pool.GetObject("ui://Common/CustomFly") as UI_CustomFly;
            if (damageFly == null) return;
            damageFly.visible = true;
            damageFly.RemoveFromParent();
            objRoot.AddChild(damageFly);

            Vector2 offset = new Vector2();
            offset.x = 0;
            offset.y = -50;
            var width = 10;
            var pos = obj.GetHeadUIPos();
            if (!dictDamageFly.ContainsKey(damage.targetID))
            {
                dictDamageFly.Add(damage.targetID, 1);
            }

            var index = dictDamageFly[damage.targetID];
            dictDamageFly[damage.targetID] += 1;
            offset.y = offset.y - (index - 1) * width;
            damageFly.CustomFlyByHurt(damage.damageType, damage.damage, pos, offset,
                () =>
                {
                    dictDamageFly[damage.targetID] -= 1;
                    if (_flyHurtDict.ContainsKey(damage.damageType))
                    {
                        _flyHurtDict[damage.damageType]--;
                    }
                });
        }

        public static void CustomFlyByHurt(this UI_CustomFly t, EN_DAMAGE_TYPE enDamageType, double textDes, Vector2 pos, Vector2 offset, Action callBack)
        {
            var tanType = FLYFORNT_TRANTYPE.NORMAL;
            var color = FORNT_COLOR.New_White;
            _stringBuilder.Clear();
            // t.tip.text = "";
            if (textDes != 0 || enDamageType == EN_DAMAGE_TYPE.Jouk)
            {
                if (enDamageType == EN_DAMAGE_TYPE.RECOVERY)
                {
                    color = FORNT_COLOR.GREEN2;
                    _stringBuilder.Append("+");
                    _stringBuilder.Append(StringUtils.FormatCurrency(textDes));
                    // _stringBuilder.Append(textDes);
                }
                else if (enDamageType == EN_DAMAGE_TYPE.Jouk)  //闪避率
                {
                    color = FORNT_COLOR.BLUE1;
                    _stringBuilder.Append(ConfigUtils.GetStringByKey(8044));
                }
                else
                {
                    color = FORNT_COLOR.New_White;
                    _stringBuilder.Append("-");
                    _stringBuilder.Append(StringUtils.FormatCurrency(textDes));
                    // _stringBuilder.Append(textDes);
                }
            }
            else
            {
                _stringBuilder.Clear();
            }

            if (enDamageType == EN_DAMAGE_TYPE.NORMAL)
            {
                tanType = FLYFORNT_TRANTYPE.NORMAL;
            }
            else if (enDamageType == EN_DAMAGE_TYPE.RECOVERY)  //--回血
            {
                tanType = FLYFORNT_TRANTYPE.NORMAL;
                color = FORNT_COLOR.GREEN2;
            }
            else if (enDamageType == EN_DAMAGE_TYPE.COMBO)  // 连击
            {
                tanType = FLYFORNT_TRANTYPE.NORMAL;
                color = FORNT_COLOR.ORANGE;
            }
            else if (enDamageType == EN_DAMAGE_TYPE.COUNTER)  // 反击
            {
                tanType = FLYFORNT_TRANTYPE.NORMAL;
                color = FORNT_COLOR.YELLOW1;
            }
            else if (enDamageType == EN_DAMAGE_TYPE.DODGE)   // 闪避
            {
                tanType = FLYFORNT_TRANTYPE.NORMAL;
                _stringBuilder.Clear();
            }
            else if (enDamageType == EN_DAMAGE_TYPE.STRIKE)  // 暴击
            {
                tanType = FLYFORNT_TRANTYPE.BAOJI;
                color = FORNT_COLOR.New_Red;
                //this.tip.text = UIConfig:GetStringByKey(StringDefine.CRITICAL_STRING)
            }
            else if (enDamageType == EN_DAMAGE_TYPE.Jouk)  // 闪避率
            {
                tanType = FLYFORNT_TRANTYPE.NORMAL;
                color = FORNT_COLOR.BLUE1;
            }
            else if (enDamageType == EN_DAMAGE_TYPE.Parry)  // 格挡
            {
                tanType = FLYFORNT_TRANTYPE.NORMAL;
                color = FORNT_COLOR.GRAY;
            }
            else if (enDamageType == EN_DAMAGE_TYPE.Bounce)  // 反弹
            {
                tanType = FLYFORNT_TRANTYPE.NORMAL;
                color = FORNT_COLOR.YELLOW;
            }
            else if (enDamageType == EN_DAMAGE_TYPE.LifeShield)  //护盾值
            {
                tanType = FLYFORNT_TRANTYPE.NORMAL;
                color = FORNT_COLOR.BLUE1;
            }

            var flyCom = t.GetTransition(tanType);  //根据 动效名称 获取动效对象
            t.textNumber.text = "";
            t.tip.text = "";
            if (!string.IsNullOrEmpty(_stringBuilder.ToString()))
            {
                if (enDamageType == EN_DAMAGE_TYPE.Jouk)
                {
                    t.tip.text = string.Format("[color={0}]{1}[/color]", color, _stringBuilder);
                    t.tip.SetXY(t.textNumber.x, t.textNumber.y);
                }
                else
                    t.textNumber.text = string.Format("[color={0}]{1}[/color]", color, _stringBuilder);
            }

            t.SetXY(pos.x + offset.x, pos.y + offset.y);
            t.SetScale(1.2f, 1.2f);
            flyCom.Play(() =>
            {
                callBack?.Invoke();
                t.visible = false;
                MapObjectManager.Instance.Pool.ReturnObject(t);
            });
        }

        private static Dictionary<int, int> dictDamageFlyAttack = new Dictionary<int, int>();

        public static void DamageFlyAttack(this UI_BarHp t, MapObject obj, EN_DAMAGE_TYPE enDamageType)
        {
            if (enDamageType != EN_DAMAGE_TYPE.COUNTER && enDamageType != EN_DAMAGE_TYPE.COMBO)
            {
                return;
            }

            var objRoot = MapObjectManager.Instance.GetMapObjectSceneFlyRootTrans();

            if (objRoot == null)
            {
                return;
            }

            var damageFly = MapObjectManager.Instance.Pool.GetObject("ui://Common/CustomFlyAttack") as UI_CustomFlyAttack;
            damageFly.visible = true;
            objRoot.AddChild(damageFly);

            Vector2 offset = new Vector2();
            offset.x = 0;
            offset.y = -50;
            var width = 10;
            var pos = obj.GetHeadUIPos();
            if (!dictDamageFlyAttack.ContainsKey(obj.id))
            {
                dictDamageFlyAttack.Add(obj.id, 1);
            }
            var index = dictDamageFlyAttack[obj.id];
            dictDamageFlyAttack[obj.id] += 1;
            offset.y = offset.y - (index - 1) * width;
            var flyID = obj.id;
            damageFly.CustomFlyByHurt(enDamageType, pos, offset,
                () =>
                {
                    if (dictDamageFlyAttack.ContainsKey(flyID))
                        dictDamageFlyAttack[flyID] -= 1;
                });
        }

        public static void CustomFlyByHurt(this UI_CustomFlyAttack t, EN_DAMAGE_TYPE enDamageType, Vector2 pos, Vector2 offset, Action callBack)
        {
            if (enDamageType == EN_DAMAGE_TYPE.COUNTER)
            {
                t.ctrlType.selectedIndex = 0;
            }
            else if (enDamageType == EN_DAMAGE_TYPE.COMBO)
            {
                t.ctrlType.selectedIndex = 1;
            }
            var flyCom = t.GetTransition(FLYFORNT_TRANTYPE.NORMAL);
            t.SetXY(pos.x + offset.x, pos.y + offset.y);
            t.SetScale(1.2f, 1.2f);
            flyCom.Play(() =>
            {
                callBack?.Invoke();
                t.visible = false;
                MapObjectManager.Instance.Pool.ReturnObject(t);
            });
        }

        /// <summary>
        /// UI_HuntingShopItem的数据设置
        /// </summary>
        /// <param name="item">组件</param>
        /// <param name="itemId">道具id</param>
        /// <param name="itemNum">道具数量</param>
        /// <param name="priceType">货币类型</param>
        /// <param name="priceNum">货币数量</param>
        public static void SetHuntingShopItem(UI_HuntingShopItem item, int itemId, int itemNum, int priceType, int priceNum)
        {
            if (item == null) { return; }

            var itemData = ConfigUtils.GetConfigItemTypeUnitById(itemId);
            //商品的名字
            item.name.text = ConfigUtils.GetTextById(itemData.Name);
            if (priceNum > 0)
            {
                //商品的价格
                item.num.text = priceNum.ToString();
            }
            else
            {
                item.num.text = "";
            }
            if (priceType > 0)
            {
                //货币类型
                item.icon = UIResource.GetItemUrl(priceType.ToString());//货币图标
            }
            else
            {
                item.icon = "";
            }

            var itemCom = item.itemCom as UI_ItemCom;
            //商品的ICON
            itemCom.icon = UIResource.GetItemUrl(itemData.Icon);
            //商品的数量
            if (itemNum > 0)
            {
                itemCom.txtLv.text = itemNum.ToString();
            }
            else
            {
                itemCom.txtLv.text = "";
            }
            //品质
            itemCom.ctrlQuality.selectedIndex = itemData.Quality - 1;
        }

        /// <summary>
        /// 开始传送
        /// </summary>
        public static void PlayChuanSongBegin(GGraph hero, SkeletonAnimation sa, Action cb, Vector2 skewing, bool isPlayEntrance = true)
        {
            hero.sortingOrder = 2;

            //身上上升特效
            var eff1 = UI_ChuanSong1.CreateInstance();
            eff1.xy = hero.xy;

            eff1.x += eff1.width * skewing.x;
            eff1.y += eff1.height * skewing.y;

            eff1.sortingOrder = 3;
            hero.parent.AddChild(eff1);

            //身体渐变消失
            GameManager.Instance.TimerManager.SetTimer(0.3f, () =>
            {
                GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.TransferSE);
                
                GTween.To(1f, 0f, 0.3f)
                .OnUpdate((GTweener tweener) =>
                {
                    sa.skeleton.SetColor(new Color(1, 1, 1, (float)tweener.value.d));
                    //Debug.Log(hero.alpha);
                })
                .OnComplete(() =>
                {
                    eff1.Dispose();
                    if (cb != null)
                    { cb.Invoke(); }
                    if (isPlayEntrance)
                    {
                        UIExtensions.PlayChuanSongEnd(hero, sa, skewing);
                    }
                });
            });
        }
        /// <summary>
        /// 结束传送
        /// </summary>
        public static void PlayChuanSongEnd(GGraph hero, SkeletonAnimation sa, Vector2 skewing)
        {
            if(hero == null || sa == null) { return; }

            sa.skeleton.SetColor(new Color(1, 1, 1, 0));
            //hero.parent.SetChildIndex(hero, hero.parent.numChildren);
            hero.sortingOrder = 2;
            var eff1 = UI_ChuanSong2.CreateInstance();
            eff1.xy = hero.xy;
            eff1.x += eff1.width * skewing.x;
            eff1.y += eff1.height * skewing.y;
            eff1.sortingOrder = 3;
            hero.parent.AddChild(eff1);
            UIExtensions.PlayHeroState(sa, HeroState.luodi);
            GameManager.Instance.TimerManager.SetTimer(0.1f, () =>
            {
                sa.skeleton.SetColor(new Color(1, 1, 1, 1));
                GameManager.Instance.TimerManager.SetTimer(0.3f, () =>
                {
                    UIExtensions.PlayHeroState(sa, HeroState.idle);
                });
                //出现盖在上面的降下特效
                //GTween.To(0f, 1f, 0.1f)
                //.OnUpdate((GTweener tweener) =>
                //{
                //    //sa.skeleton.SetColor(new Color(1, 1, 1, (float)tweener.value.d));
                //    //hero.alpha = (float)tweener.value.d;
                //    //Debug.Log(hero.alpha);
                //})
                //.OnComplete(() =>
                //{
                //});
            });
            //延迟播放下面的圈圈
            //GameManager.Instance.TimerManager.SetTimer(0.05f, () =>
            //{
                var eff2 = UI_ChuanSong3.CreateInstance();
                eff2.xy = hero.xy;
                eff2.x += eff2.width * skewing.x;
                eff2.y += eff2.height * (skewing.y - 0.1f);
                eff2.sortingOrder = 1;
                hero.parent.AddChildAt(eff2, hero.parent.GetChildIndex(hero) - 1);

                GameManager.Instance.TimerManager.SetTimer(1.0f, () =>
                {
                    eff1.Dispose();
                    eff2.Dispose();
                });
            //});
        }
        
        /// <summary>
        /// 播放英雄的状态
        /// </summary>
        /// <param name="HeroState">动画名枚举</param>
        /// <param name="isLoop">是否循环</param>
        public static void PlayHeroState(SkeletonAnimation heroSpine, HeroState stateName, bool isLoop = true)
        {
            string name = stateName.ToString();
            if (heroSpine != null && heroSpine.state.Data.SkeletonData.FindAnimation(name) != null && !heroSpine.AnimationName.Contains(name))
            {
                heroSpine.state?.SetAnimation(0, name, isLoop);
            }
        }
    }
}