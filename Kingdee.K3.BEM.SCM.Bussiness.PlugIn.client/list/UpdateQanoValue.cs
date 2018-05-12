using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.ControlModel;
using Kingdee.BOS.Core.Metadata.EntityElement;
using Kingdee.BOS.Core.Metadata.FieldElement;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.ServiceHelper;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.Core.Bill.PlugIn;
using Kingdee.BOS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Kingdee.BOS;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.SqlBuilder;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.List.PlugIn;
using Kingdee.BOS.Core.List.PlugIn.Args;
using Kingdee.BOS.Contracts;
//using Kingdee.BOS.Business.DynamicForm;
using Kingdee.BOS.App.Data;

namespace Kingdee.K3.BEM.SCM.Service.Plugin
{
     [Description("更新质保书")]
    class UpdateQanoValue:AbstractDynamicFormPlugIn
    {
        private string ids = "";  //所选 数据行的IDS
        private string lotIds = "";  //所选 的批号IDS
        private string PformId = "";
        private string tableName = "";
        private string entryIds = "";
        private List<string> updateSqls = new List<string>();
        object value;
        public override void ButtonClick(BOS.Core.DynamicForm.PlugIn.Args.ButtonClickEventArgs e)
        {
            base.ButtonClick(e);
            string PformId = this.View.BillBusinessInfo.GetForm().Id;
            switch (PformId)
            {
                case "STK_InStock":  //采购入库
                    tableName = "T_STK_INSTOCKENTRY";
                    break;
                case "STK_MISCELLANEOUS"://其他入库
                    tableName = "T_STK_MISCELLANEOUSENTRY";
                    break;
                case "SP_InStock"://简单生产入库
                    tableName = "T_SP_INSTOCKENTRY";
                    break;
                case "STK_InitInStock"://期初采购入库
                    tableName = "T_STK_INITINSTOCKENTRY";
                    break;
                case "STK_OEMInStock"://受托加工材料入库
                    tableName = "T_STK_OEMINSTOCKENTRY";
                    break;
                case "STK_InvInit":
                    tableName = "T_STK_INVINITDETAIL";
                    break;
                default: break;
            }

            //string a;
            //if ((a = e.Key.ToUpper()) != null)
            //{
            //    if (!(a == "FBTNCONFIRM"))
            //    {
            //        if (a == "FBTNCANCEL")
            //        {
            //            this.View.Close();
            //        }
            //    }
            //    else
            //    {
                    //value = this.View.Model.GetValue("F_BEM_QANO");
                    //if (value == null)
                    //{
                    //    this.View.ShowMessage("请输入质保书号");
                    //    return;
                    //}
                    if (!lotIds.IsEmpty() && !lotIds.Equals("0"))
                    {
                        string filter = string.Format("FLOTID IN ('{0}')", lotIds);
                        OQLFilter of = OQLFilter.CreateHeadEntityFilter(filter);
                        var lotMasters = BusinessDataServiceHelper.Load(this.Context, "BD_BatchMainFile", null, of);
                        foreach (var lot in lotMasters)
                        {
                            lot["F_QANo"] = value;
                        }
                        ISaveService service = Kingdee.BOS.App.ServiceHelper.GetService<ISaveService>();
                        service.Save(this.Context, lotMasters);
                        updateSqls.Add(string.Format(@"UPDATE  {0}  set F_QANo ='{1}'  WHERE flot  in ('{2}')  ", tableName, value, lotIds));
                    }


                    if (!ids.IsEmpty())
                    {
                        updateSqls.Add(string.Format(@"UPDATE  {0}  set F_QANo ='{1}'  WHERE fid  in ('{2}')  ", tableName, value, ids));
                    }
                    else if (!entryIds.IsEmpty())
                    {
                        updateSqls.Add(string.Format(@"UPDATE  {0}  set F_QANo ='{1}'  WHERE fentryid  in ('{2}')  ", tableName, value, entryIds));
                    }

                    DBUtils.ExecuteBatch(this.Context, updateSqls, updateSqls.Count);

                //}
                //返回值到父窗口
              //  this.View.ReturnToParentWindow("true");
                this.View.Close();
            }
        }
    }

