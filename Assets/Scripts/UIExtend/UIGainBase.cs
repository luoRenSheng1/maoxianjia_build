using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using FairyGUI;

namespace Engine
{
    internal class FragInfo
    {
        public Vector2 pos;
        public UI_FlyIcon frag;

        public FragInfo(Vector2 p, UI_FlyIcon f)
        {
            pos = p;
            frag = f;
        }

        public void SetAlpha(float alpha)
        {
            if (frag != null)
                frag.alpha = alpha;
        }

        public void SetXY(Vector2 xy)
        {
            if (frag != null)
                frag.xy = xy;
        }
    }

    internal enum State
    {
        Init = 0,
        Start = 1,
        End = 2
    }

    public class UIGainBase
    {
        private static float fragTime1 = 0.2f;
        private static float rollNumberTime = 0.8f;
        private static float fragDelay1 = 0.3f;
        private static float fragDelay2 = 0.3f;
        private static int scatterAmount = 100;
        private static float fragAppearTime = 0.4f;

        private int minFrags = 5;
        private int maxFrags = 10;
        private int fragDivider = 10;

        private double currentValue = 0;
        private double startValue = 0;
        private double endValue = 0;
        private double addValue = 1;
        private int fragCounter = 0;
        private string baseIcon = "";

        private Action<GObject> onActionScrollEnd = null;
        private Action<GObject, double> onActionScrolling = null;
        private Action<GObject> onActionScroll = null;

        private List<FragInfo> fragItems = new List<FragInfo>();

        private State state = State.Init;

        public bool autoSizeFrag = true;
        public bool isSmoothNum = false;
        public float fragWidth = 0;
        public float fragHeight = 0;
        public GComponent receiver = null;

        // Constructor
        public UIGainBase()
        {
            OnCtor();
        }

        // Protected method
        protected virtual void OnCtor()
        {
            state = State.Init;
            FadeCancel();
            SetIcon(UIResource.GoldIcon);
        }

        // Private method
        private UI_FlyIcon SpawnFrag(string icon, Vector2 pos)
        {
            UI_FlyIcon frag = MapObjectManager.Instance.Pool.GetObject("ui://Common/FlyIcon") as UI_FlyIcon;
            frag.RemoveFromParent();
            frag.visible = true;
            frag.touchable = false;
            frag.SetPivot(0.5f, 0.5f);
            frag.pivotAsAnchor = true;
            frag.scale = new Vector2(1.5f, 1.5f);
            frag.iconUrl.priority = true;
            frag.iconUrl.url = icon;
            GRoot.inst.AddChild(frag);// FlyRootNumIndex());
            frag.xy = pos;
            return frag;
        }

        private Vector2 ScatterPos()
        {
            float x = (UnityEngine.Random.value - 0.5f) * 2 * scatterAmount;
            float y = (UnityEngine.Random.value - 0.5f) * 2 * scatterAmount;
            return new Vector2(x, y);
        }

        private bool IsAlive()
        {
            return state != State.End;
        }

        private bool IsDead()
        {
            return state == State.End;
        }

        private Tuple<int, float> CalcFragAmount(int total)
        {
            if (total <= minFrags)
            {
                return Tuple.Create(total, 1f);
            }

            int amount = Mathf.CeilToInt((float)total / fragDivider);
            amount = Mathf.Clamp(amount, minFrags, maxFrags);
            float fragValue = (float)total / amount;
            return Tuple.Create(amount, fragValue);
        }

        private float CreateFrags(Vector2 sourcePos)
        {
            var tuple = CalcFragAmount((int)addValue);
            int fragCount = tuple.Item1;
            float fragValue = tuple.Item2;

            fragItems.Clear();
            for (int i = 0; i < fragCount; i++)
            {
                UI_FlyIcon frag = SpawnFrag(baseIcon, sourcePos);
                Vector2 pos = frag.xy + ScatterPos();

                FragInfo fInfo = new FragInfo(pos, frag);
                fragItems.Add(fInfo);
            }

            return fragValue;
        }

        private void MoveToFrags(Vector2 sourcePos, Vector2 targetPos, bool flag)
        {
            float fragValue = CreateFrags(GRoot.inst.GlobalToLocal(sourcePos));
            Vector2 flyPos = GRoot.inst.GlobalToLocal(targetPos);
            foreach (var fInfo in fragItems)
            {
                fragCounter++;
                Action moveCoroutine = null;
                moveCoroutine = () =>
                {
                    GameManager.Instance.StartCoroutine(CoroutineAlpha(fInfo));
                    GameManager.Instance.StartCoroutine(CoroutineXY(flyPos, fragValue, fInfo, flag));
                };
                moveCoroutine();
            }
        }

        IEnumerator UpdateReceiverReacts(float num)
        {
            float rollTimer = 0;
            OnScrollStartImp();
            float deltaValue =Mathf.Ceil(num / rollNumberTime);
            while (rollTimer < rollNumberTime)
            {
                if (IsDead())
                    yield break;

                float deltaTime = Time.deltaTime;
                currentValue = (double) Math.Min(endValue, currentValue + Math.Max(1,deltaValue * deltaTime));
                double value = isSmoothNum ? currentValue : (double)currentValue;
                OnScrollingImp(value);
                rollTimer += deltaTime;
                yield return null;
            }

            fragCounter--;
            if (fragCounter <= 0)
                OnScrollEndImp();
        }

        private void OnScrollStartImp()
        {
            onActionScroll?.Invoke(receiver);
        }

        private void OnScrollingImp(double value)
        {
            onActionScrolling?.Invoke(receiver, Math.Ceiling(value));
        }

        private void OnScrollEndImp()
        {
            onActionScrollEnd?.Invoke(receiver);
            Clear(true);
        }

        // Optional methods
        private void Clear(bool fade)
        {
            state = State.End;
            currentValue = 0;
            startValue = 0;
            endValue = 0;
            fragCounter = 0;

            if (fragItems.Count > 0)
            {
                foreach (var item in fragItems)
                {
                    if (null != item && null != item.frag && !item.frag.isDisposed)
                    {
                        item.frag.visible = false;
                        MapObjectManager.Instance.Pool.ReturnObject(item.frag);
                    }
                }

                fragItems.Clear();
            }

            if (fade)
            {
                FadeCancel();
            }
        }

        private Coroutine receiverCoroutine = null;

        private void FadeCancel()
        {
            if (null != receiverCoroutine)
            {
                GameManager.Instance.StopCoroutine(receiverCoroutine);
                receiverCoroutine = null;
            }
        }

        private IEnumerator CoroutineAlpha(FragInfo fInfo)
        {
            fInfo.SetAlpha(0);
            float alphaAmount = 0;
            float speed = 1 / fragAppearTime;
            while (alphaAmount < 1)
            {
                if (IsDead())
                    yield break;

                fInfo.frag.alpha = alphaAmount;
                alphaAmount = Mathf.Min(1, alphaAmount + Time.deltaTime * speed);
                yield return null;
            }

            fInfo.SetAlpha(1);
            yield return null;
        }

        private IEnumerator CoroutineXY(Vector2 flyPos, float fragValue, FragInfo fInfo, bool flag)
        {
            float timer = 0;
            Vector2 start = fInfo.frag.xy;

            if (flag)
            {
                yield return new WaitForSeconds(UnityEngine.Random.value * fragDelay1);
                
                while (timer < fragTime1)
                {
                    if (IsDead())
                        yield break;
                
                    fInfo.SetXY(Vector2.Lerp(start, fInfo.pos, timer / fragTime1));
                    timer += Time.deltaTime;
                    yield return null;
                }
            }

            timer = 0;
            start = fInfo.frag.xy;
            yield return new WaitForSeconds(fragDelay2);
            float flySpeed = 0;
            Vector2 pos = start;
            while (pos != flyPos)
            {
                if (IsDead())
                    yield break;

                float deltaTime = Time.deltaTime;
                flySpeed += timer * 140;
                pos = Vector2.MoveTowards(pos, flyPos, flySpeed * deltaTime);
                timer += deltaTime;
                fInfo.frag.xy = pos;
                yield return null;
            }

            fInfo.SetAlpha(0);

            GameManager.Instance.StartCoroutine(UpdateReceiverReacts(fragValue));
        }

        protected virtual void OnScroll(Action<GObject> callback)
        {
            onActionScroll = callback;
        }

        protected virtual void OnScrolling(Action<GObject, double> callback)
        {
            onActionScrolling = callback;
        }

        protected virtual void OnScrollEnd(Action<GObject> callback)
        {
            onActionScrollEnd = callback;
        }

        protected virtual void SetReceiver(GComponent value)
        {
            receiver = value;
        }

        protected virtual void SetIcon(string value)
        {
            baseIcon = value;
        }

        protected virtual void SetValue(double startVl, double endVl)
        {
            currentValue = startVl;
            startValue = startVl;
            endValue = endVl;
            if (endValue > startValue)
            {
                addValue = endValue - startValue;
            }
        }

        protected virtual void Start(Vector2 sourcePos, Vector2 targetPos, bool flag)
        {
            state = State.Start;
            MoveToFrags(sourcePos, targetPos, flag);
        }
    }
}