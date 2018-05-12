using Kingdee.BOS.Core.Bill.PlugIn;
using Kingdee.BOS.Orm.DataEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingdee.K3.BEM.SCM.Bussiness.PlugIn.client.bill
{
     [Description("单据界面处理规格")]
    public class SpecificationEdit : AbstractBillPlugIn
    {
         public override void DataChanged(BOS.Core.DynamicForm.PlugIn.Args.DataChangedEventArgs e)
         {
             base.DataChanged(e);
           
                 switch (e.Field.Key)
                 {
                     case "F_Length":
                     case "F_Width":
                     case "F_Thinkness":
                     case "F_EdgeState":
                         decimal lengthdec = Convert.ToDecimal(this.View.Model.GetValue("F_Length", e.Row));
                         decimal widthdec = Convert.ToDecimal(this.View.Model.GetValue("F_Width", e.Row));
                         decimal thinknessdec = Convert.ToDecimal(this.View.Model.GetValue("F_Thinkness", e.Row));

                         string strLong = lengthdec.ToString("0.00");     //长度
                         string strWidth = widthdec.ToString("0.00");       //宽度
                         string strThick = thinknessdec.ToString("0.00");     //标准厚度
                         var edgeState = this.View.Model.GetValue("F_EdgeState", e.Row);
                         DynamicObject edgeObj = null;
                         string strEdge = "";
                         if (null != edgeState)
                         {
                             edgeObj = edgeState as DynamicObject;
                             strEdge = Convert.ToString(edgeObj["FDataValue"]);
                         }

                         string spec = getSpecifications(strThick, strWidth, strLong, strEdge);

                         this.View.Model.SetValue("F_Specification", spec, e.Row);
                         break;
                     default:
                         break;
                 }
             }
         
         private  string getSpecifications(string strThick, string strWidth, string strLong, string edgeState)
         {
             strThick = strThick.Trim();
             strWidth = strWidth.Trim();
             strLong = strLong.Trim();
             if (strWidth.EndsWith(".00"))
             {
                 strWidth = strWidth.Substring(0, strWidth.Length - 3);
             }
             if (strLong.EndsWith(".00"))
             {
                 strLong = strLong.Substring(0, strLong.Length - 3);
             }
             if (strThick.EndsWith(".00"))
             {
                 strThick = strThick.Substring(0, strThick.Length - 3);
             }

             if (strWidth.EndsWith("0") && strWidth.Contains("."))
             {
                 strWidth = strWidth.Substring(0, strWidth.Length - 1);
             }
             if (strLong.EndsWith("0") && strLong.Contains("."))
             {
                 strLong = strLong.Substring(0, strLong.Length - 1);
             }
             if (strThick.EndsWith("0") && strThick.Contains("."))
             {
                 strThick = strThick.Substring(0, strThick.Length - 1);
             }

             //if ((strLong == "" || strLong == "0") && (strWidth == "" || strWidth == "0"))
             //{
             //    return strThick + "*N*N";
             //}
             //else if (strLong == "" || strLong == "0")
             //{
             //    return strThick + "*" + strWidth + "*C";
             //}
             return strThick + "*" + strWidth + "/" + edgeState + "*" + strLong;


         }

    }
}
