using EPDM.Interop.epdm;
using System;

namespace BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Core
{
    public class BlueByteTaskInstance : IEdmTaskInstance
    {
        #region Public Properties

        public long ID
        { get { return this.ID; } }

        public string InstanceGUID
        { get { return Guid.NewGuid().ToString(); } }

        public string TaskGUID
        { get { return this.TaskGUID; } }

        public string TaskName
        { get { return this.TaskName; } }

        #endregion

        #region Public Methods

        public EdmTaskStatus GetStatus()
        {
            return this.GetStatus();
        }

        public object GetValEx(string bsValName)
        {
            return this.GetValEx(bsValName);
        }

        public object GetVar(object oVarIDorName)
        {
            return this.GetVar(oVarIDorName);
        }

        public void SetProgressPos(int lPos, string bsDocStr)
        {
            this.SetProgressPos(lPos, bsDocStr);
        }

        public void SetProgressRange(int lMax, int lPos, string bsDocStr)
        {
            this.SetProgressRange(lMax, lPos, bsDocStr);
        }

        public void SetStatus(EdmTaskStatus eStatus, int lHRESULT = default(int), string bsCustomMsg = default(string), object oNotificationAttachments = default(object), string bsExtraNotificationMsg = default(string))
        {
            this.SetStatus(eStatus, lHRESULT, bsCustomMsg, oNotificationAttachments, bsExtraNotificationMsg);
        }

        public void SetValEx(string bsValName, object oValue)
        {
            this.SetValEx(bsValName, oValue);
        }

        public void SetVar(object oVarIDorName, object oValue)
        {
            this.SetVar(oVarIDorName, oValue);
        }

        #endregion
    }
}