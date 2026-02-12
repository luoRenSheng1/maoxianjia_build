using System.Collections.Generic;
using UnityEngine;
using Engine;

namespace Engine
{
    public class MapBezierFxObject : MapFxObject
    {
        private const int MAX_POINT_COUNT = 100;    // 曲线点的个数
        private const int MIN_POINT_COUNT = 10;     // 曲线点的个数
        private const float LIMIT_POINT_COUNT_TIME = 1 / 60.0f;
        private const float LIMIT_FOLLOW_TIME = 0.9f;

        protected string bone;
        protected string targetBone;

        /// <summary>
        /// 贝塞尔曲线
        /// </summary>
        private List<Vector3> lstWayPoint = new List<Vector3>();
        private List<Vector3> lstLinePoint = new List<Vector3>();
        private List<Vector3> lstPath = null;
        private float fPointUpdateTime = 0.01f;     // 曲线点刷新频率
        private Vector3 startPos;
        private Vector3 targetPos;
        private MapObject actorOwner;
        private MapObject actorTarget;
        private bool bFollowTarget = false;         // 最后时刻是否跟踪目标
        private float fPointCurTime = 0.0f;         // 计时
        private int lineItem = 1;                   // 目标索引
        private int lineItemTotal = MAX_POINT_COUNT;
        private float fCurTime = 0.0f;
        private float fTotalTime = 0.0f;
        private float fFollowLimitTime = 0.0f;

        public void InitBezier(Config.ConfigSkillEffectUnit cfg, Vector3 pos, Quaternion rot, MapObject owner,
            MapObject target, Vector3 tarPos)
        {
            base.InitByConfig(cfg, pos, rot, owner);

            this.bone = cfg.Bone;
            this.targetBone = cfg.TargetBone;
            this.actorOwner = owner;
            this.actorTarget = target;
            this.startPos = this.actorOwner == null ? position : this.actorOwner.DummyPos(bone);
            this.targetPos = this.actorTarget != null ? this.actorTarget.DummyPos(targetBone) : tarPos;
            this.lstPath = ConfigUtils.GetVector3ByConfigString(cfg.TypeData);
            if (this.lstPath != null && this.lstPath.Count > 0)
            {
                Vector3 v = this.lstPath[0];
                this.bFollowTarget = !Mathf.Approximately(v.x, 0);
                this.lstPath.RemoveAt(0);
            }
            else
            {
                this.bFollowTarget = false;
            }

            this.SetPosition(this.startPos, true);
            float timeTotal = Utils.DistanceIgnoreY(this.targetPos, this.startPos) / (cfg.Speed * ConstDefine.CONFIG_PLACE);
            this.StartEffect(timeTotal);
        }

        public void StartEffect(float timeTotal)
        {
            Vector3 delta = this.targetPos - this.startPos;

            this.lineItemTotal = (int)Mathf.Clamp(timeTotal / LIMIT_POINT_COUNT_TIME, MIN_POINT_COUNT, MAX_POINT_COUNT);
            this.fPointUpdateTime = timeTotal / this.lineItemTotal;

            this.fPointCurTime = 0.0f;
            this.lineItem = 1;
            this.fCurTime = 0.0f;
            this.fTotalTime = timeTotal;
            this.fFollowLimitTime = this.fTotalTime * LIMIT_FOLLOW_TIME;

            Vector3 v = (this.targetPos - this.startPos).normalized;
            Quaternion q = Quaternion.FromToRotation(Vector3.forward, v);

            // 计算贝塞尔曲线的点
            lstWayPoint.Clear();
            lstWayPoint.Add(this.startPos);
            if (lstPath != null && lstPath.Count > 0)
            {
                for (int i = 0; i < lstPath.Count / 2; i++)
                {
                    lstWayPoint.Add(this.startPos + q * lstPath[i] * delta.magnitude);
                }
            }
            lstWayPoint.Add(this.targetPos);

            /*        foreach (var item in lstWayPoint)
                    {
                        LogUtils.LogError("Point " + (item - this.startPos).x + " " + (item - this.startPos).y + " " + (item - this.startPos).z);
                    }*/

            lstLinePoint.Clear();
            for (int i = 0; i < this.lineItemTotal; i++)
            {
                var point = BezierPoint(i / (float)this.lineItemTotal, lstWayPoint);
                lstLinePoint.Add(point);
            }

            SetRotation(Quaternion.LookRotation(delta), true);

            this.SetTick(true);
        }

        public override void Tick(float deltaSeconds)
        {
            base.Tick(deltaSeconds);

            if (lineItem >= lstLinePoint.Count || lstLinePoint.Count <= 0 || lineItem < 1 || this.fCurTime > this.fTotalTime)
            {
                EndPlay();
                return;
            }

            this.fPointCurTime += deltaSeconds;
            this.fCurTime += deltaSeconds;

            if (this.bFollowTarget && this.actorTarget != null && this.actorTarget.Active && this.fCurTime >= this.fFollowLimitTime)
            {
                // 最后时刻，直接跟踪，不走路径
                Vector3 posTarget = this.actorTarget.DummyPos(targetBone);

                if (!posTarget.Equals(this.position))
                {
                    float fPrecent = Mathf.Clamp01(deltaSeconds / (this.fTotalTime - this.fCurTime - deltaSeconds));

                    Vector3 pos = Vector3.Lerp(this.position, posTarget, fPrecent);
                    Quaternion rot = Quaternion.LookRotation(posTarget - this.position);

                    this.SetTransform(pos, rot, true);
                }
            }
            else
            {
                if (this.fPointCurTime > this.fPointUpdateTime)
                {
                    this.fPointCurTime = this.fPointCurTime - this.fPointUpdateTime;

                    Vector3 pos1 = (lineItem - 1) < lstLinePoint.Count ? lstLinePoint[lineItem - 1] : Vector3.zero;
                    Vector3 pos2 = lineItem < lstLinePoint.Count ? lstLinePoint[lineItem] : Vector3.zero;
                    Vector3 pos = Vector3.Lerp(pos1, pos2, 1f);

                    Vector3 posTarget = this.actorTarget == null ? this.targetPos : this.actorTarget.DummyPos(targetBone);
                    Quaternion rot = Quaternion.LookRotation(posTarget - this.position);

                    SetTransform(pos, rot, true);

                    lineItem++;
                    if (lineItem >= lstLinePoint.Count || lstLinePoint.Count <= 0 || lineItem < 1)
                    {
                        EndPlay();
                        return;
                    }
                }
            }
        }

        // n阶曲线，递归实现
        private Vector3 BezierPoint(float t, List<Vector3> lstPoint)
        {
            if (lstPoint.Count < 2)
            {
                return lstPoint[0];
            }

            List<Vector3> lstNewPoint = new List<Vector3>();

            for (int i = 0; i < lstPoint.Count - 1; i++)
            {
                //Debug.DrawLine(p[i], p[i + 1], Color.yellow);
                Vector3 p0p1 = (1 - t) * lstPoint[i] + t * lstPoint[i + 1];
                lstNewPoint.Add(p0p1);
            }

            return BezierPoint(t, lstNewPoint);
        }
    }
}