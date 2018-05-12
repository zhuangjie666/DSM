using Kingdee.BOS.Core.Bill.PlugIn;
using Kingdee.BOS.Core.Metadata.EntityElement;
using Kingdee.BOS.Orm.DataEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingdee.K3.BEM.SCM.Bussiness.PlugIn.client.bill
{
    [Description("根据投入批号自动产生产出批号")]
    public class LOTProOutNew : AbstractBillPlugIn
    {
        public override void DataChanged(BOS.Core.DynamicForm.PlugIn.Args.DataChangedEventArgs e)
        {
            base.DataChanged(e);
            switch (e.Field.Key)
            {
                case "F_PAEZ_ProOutLot":
                   
                    string proOutLot = Convert.ToString(this.View.Model.GetValue("F_PAEZ_ProOutLot"));
                    if (proOutLot!="")
                    {
                    string Fid = Convert.ToString(this.View.Model.GetValue("FBillNo"));
                    string LOTProOutNew = proOutLot + "_" + Fid;
                    this.View.Model.SetValue("F_PAEZ_LOTProOutNew", LOTProOutNew, e.Row);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
