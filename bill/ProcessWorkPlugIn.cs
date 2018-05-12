using Kingdee.BOS.Core.Bill.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingdee.K3.BEM.SCM.Bussiness.PlugIn.client.bill
{
    [Description("根据技术标准进行删选对应的生产标准")]
   public class ProcessWorkPlugIn : AbstractBillPlugIn
    {
        public override void BeforeF7Select(BeforeF7SelectEventArgs e)
        {
            if (e.BaseDataField == null)
            {
                return;
            }
            if (e.FieldKey == "F_PAEZ_Base")
            {
                int TsRow = this.Model.GetEntryCurrentRowIndex("F_PAEZ_TSID"); 
                DynamicObject Paez_Tsid = this.Model.GetValue("F_PAEZ_TSID", TsRow) as DynamicObject;
                if (null != Paez_Tsid)
                {
                    string filter = string.Format(" (f_paez_tano= {0}  )", Paez_Tsid["number"].ToString());
               
                //销售厚度过滤条件加上去
                if (string.IsNullOrEmpty(e.ListFilterParameter.Filter))
                {
                    e.ListFilterParameter.Filter = filter;
                }
                else
                {
                    filter = " And " + filter;
                    e.ListFilterParameter.Filter += filter;
                }
                }
            }

        }
    }
}