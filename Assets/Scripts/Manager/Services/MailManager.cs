using System;
using System.Collections.Generic;
using EngineBase;
using msg;

namespace Engine
{
    public class MailAttachInfo
    {
        /// <summary>
        /// 附件类型ID---目前就只有一种itemID
        /// </summary>
        public int AttachType;
        /// <summary>
        /// 目前只有 item ID对应的数量
        /// </summary>
        public int AttachNum;
    }

    public class TitleInfo
    {
        public int id;
    }
    
    public class MailInfo
    {
        public ulong MailId;
        public ulong SenderGuid;
        public string SenderName;
        public int MailType;
        public int Status;
        public string Title;
        public string Content;
        public List<MailAttachInfo> MailAttachInfos = new List<MailAttachInfo>();
        public ulong EndTime;
        public ulong SendTime;
        public List<string> TitleParams = new List<string>();
        public List<string> ContentParams = new List<string>();

        public void Read(MailDetail mailDetail)
        {
            this.MailId = mailDetail.Guid;
            this.SenderGuid = mailDetail.SenderGuid;
            this.SenderName = mailDetail.SenderName;
            this.MailType = mailDetail.Type;
            this.Status = mailDetail.Status;
            this.Title =  mailDetail.Title;
            this.Content = mailDetail.Content;

            foreach (var item in mailDetail.AttachsList)
            {
                MailAttachInfo mailAttachInfo = new MailAttachInfo();
                mailAttachInfo.AttachType = (int) item.AttachType;
                mailAttachInfo.AttachNum = (int) item.AttachNum;
                this.MailAttachInfos.Add(mailAttachInfo);
            }

            this.SendTime = (ulong)mailDetail.SendTime;
            this.EndTime = mailDetail.ExpireCd + ServerTimeManager.Instance.CurServerTime;

            foreach (var item in mailDetail.TitleParamsList)
            {
                TitleParams.Add(item);
            }
            
            foreach (var item in mailDetail.ContentParamsList)
            {
                ContentParams.Add(item);
            }
        }
    }
    
    public class MailManager : Singleton<MailManager>
    {
        private List<MailInfo> _mailInfos = new List<MailInfo>();

        public void UpdateMailInfo(MailInfo mailInfo)
        {
            bool isHasMail = false;
            for (int i = 0; i < _mailInfos.Count; i++)
            {
                if (_mailInfos[i].MailId == mailInfo.MailId)
                {
                    _mailInfos[i] = mailInfo;
                    isHasMail = true;
                    break;
                }
            }
            if(!isHasMail)
                _mailInfos.Add(mailInfo);
        }

        public MailInfo GetMailInfo(ulong mailId)
        {
            foreach (var item in _mailInfos)
            {
                if (item.MailId == mailId)
                    return item;
            }

            return null;
        }
        
        public void DelMailInfo(ulong mailId)
        {
            for (int i = _mailInfos.Count -1; i >=0; i--)
            {
                if (_mailInfos[i].MailId == mailId)
                    _mailInfos.RemoveAt(i);
            }
        }

        public List<MailInfo> GetMailList()
        {
            _mailInfos.Sort((a, b) =>
            {
                int result = 0;
                int sortA = 0;
                int sortB = 0;
                
                bool isRead = (a.Status & (int)eMailStatus.eMailStatus_Read) > 0;
                bool hasAttach = (a.Status & (int)eMailStatus.eMailStatus_Attach) > 0;
                if (!isRead)
                    sortA = 1;
                else if (hasAttach && a.MailAttachInfos.Count > 0)
                {
                    sortA = 3;
                }
                else
                {
                    if (a.MailAttachInfos.Count == 0)
                        sortA = 3;
                    else
                        sortA = 2;
                }
                
                bool isReadB = (b.Status & (int)eMailStatus.eMailStatus_Read) > 0;
                bool hasAttachB = (b.Status & (int)eMailStatus.eMailStatus_Attach) > 0;
                if (!isReadB)
                    sortB = 1;
                else if (hasAttachB && b.MailAttachInfos.Count > 0)
                {
                    sortB = 3;
                }
                else
                {
                    if (b.MailAttachInfos.Count == 0)
                        sortB = 3;
                    else
                        sortB = 2;
                }
                
                result = sortA > sortB ? 1 : (sortA == sortB ? 0 : -1);
                
                if (result == 0)
                {
                    result = a.MailId > b.MailId ? -1 : 1;
                }

                return result;
            });
            return _mailInfos;
        }

        public void SendMailListCS()
        {
            var builder = MailList_CS.CreateBuilder();
            builder.MailType = 0;
            builder.Count = 0;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_MailList_CS, builder.Build());
        }

        public bool HasNotReadMail()
        {
            RedPointInfo redPointInfo = ReddotSysManager.Instance.GetRedPointByType(eRedPointType.eRedPointType_Mail);

            foreach (var item in _mailInfos)
            {
                double statusRead = Math.Pow(2, (double) eMailStatus.eMailStatus_Read);
                bool isRead = (item.Status & (int)statusRead) > 0;
                if (!isRead)
                {
                    return true;
                }
            }

            return  (redPointInfo!=null && !redPointInfo.IsRead);
        }
    }
}