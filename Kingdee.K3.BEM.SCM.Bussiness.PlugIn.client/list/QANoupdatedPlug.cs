using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.ControlModel;
using Kingdee.BOS.Core.Metadata.EntityElement;
using Kingdee.BOS.Core.Metadata.FieldElement;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.ServiceHelper;
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
using Kingdee.BOS.Core.List;

namespace Kingdee.K3.BEM.SCM.Bussiness.PlugIn.client.list
{
    [Description("批量更新质保书号")]
    public class QANoupdatedPlug : AbstractListPlugIn
    {
        //点击时 
        //1. 判断是那些form需要被更新
        //2. 是否选中所需要更新的行
        //3. 弹出窗口，同时把需要更新的key送到新的form
        //4. 得到输入的质保书号，通过key去更新数据库，同时更新批号主档
        //5. 点击确定后，返回主form，并且刷新form 
        List<String> sqlList = new List<String>();
        private string tableName = "";


        public override void BarItemClick(BOS.Core.DynamicForm.PlugIn.Args.BarItemClickEventArgs e)
        {
            base.BarItemClick(e);

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




            if (e.BarItemKey.Equals("tbQANo"))
            {

                List<string> ids = new List<string>(); //所选择行的数据ID
                List<string> entryIds = new List<string>(); //所选择行的数据ID
                List<int> lotIds = new List<int>();
                string sql = "";


                ListSelectedRowCollection selectedRowsInfo = this.ListView.SelectedRowsInfo;
                if (selectedRowsInfo.Count == 0)
                {
                    this.ListView.ShowMessage("请选择至少一条数据！");
                    e.Cancel = true;
                }
                else
                {

                    foreach (var one in selectedRowsInfo)
                    {
                        if (null == one.EntryEntityKey)
                        {
                            ids.Add(one.PrimaryKeyValue);
                        }
                        else
                        {
                            entryIds.Add(one.EntryPrimaryKeyValue);

                        }

                    }
                    if (entryIds.Count != 0)
                    {
                        sql = string.Format("SELECT FLOT FROM {0} WHERE FLOT<>0 AND  FENTRYID IN  ({1})", tableName, string.Join(",", entryIds));
                    }
                    else if (ids.Count != 0)
                    {
                        sql = string.Format("SELECT FLOT FROM {0} WHERE  FLOT<>0 AND  fid IN  ({1})", tableName, string.Join(",", ids));
                    }

                    using (IDataReader reader = DBUtils.ExecuteReader(this.Context, sql))
                    {
                        while (reader.Read())
                        {
                            int s = reader.GetInt32(0);
                            lotIds.Add(s);
                        }
                    }

                    ShowBemForm(string.Join("','", ids), string.Join("','", entryIds), string.Join("','", lotIds), PformId);

                }

            }
        }  
        private void ShowBemForm(string ids, string entryIds, string lotIds, string PformId)
        {
            DynamicFormShowParameter dynamicFormShowParameter = new DynamicFormShowParameter();
            dynamicFormShowParameter.MultiSelect = false;
            dynamicFormShowParameter.ParentPageId = this.View.PageId;
            dynamicFormShowParameter.FormId = "BEM_QANO";
            
             List<string> lotEntity = new List<string>();
           
            
             
            if (!string.IsNullOrWhiteSpace(lotIds.Trim()))
            {
                dynamicFormShowParameter.CustomParams.Add("lotIds", lotIds);
                dynamicFormShowParameter.CustomParams.Add("ids", ids);
                dynamicFormShowParameter.CustomParams.Add("pformid", PformId);
                dynamicFormShowParameter.CustomParams.Add("entryIds", entryIds);
                
                
           
            }

            this.View.ShowForm(dynamicFormShowParameter, delegate(FormResult result)
           {
               if (result.ReturnData != null)
               {
                   this.View.Refresh();
                  
               }
           });
        }
    }

}

            


 

