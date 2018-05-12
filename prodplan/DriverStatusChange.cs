using Kingdee.BOS.Core.Bill.PlugIn;
using Kingdee.BOS.Core.Metadata.EntityElement;
using Kingdee.BOS.Orm.DataEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Kingdee.K3.BEM.SCM.Bussiness.PlugIn.client.prodplan
{
    public class DriverStatusChange : AbstractBillPlugIn
    {
        [Description("生产计划单开工须修改设备状态为占用中")]
        public override void DataChanged(BOS.Core.DynamicForm.PlugIn.Args.DataChangedEventArgs e)
        {
            base.DataChanged(e);
            switch (e.Field.Key)
            {
                case "FBillStatus":
                    string proOutLot = Convert.ToString(this.View.Model.GetValue("FBillStatus"));
                    string newProdStatus = "占用";
                    if (proOutLot.Equals("开工"))
                    {
                        this.View.Model.SetValue("F_PAEZ_Equipment", newProdStatus, e.Row);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
