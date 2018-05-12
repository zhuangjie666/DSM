using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Orm.DataEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingdee.K3.BEM.SCM.Service.Plugin.prodServer
{
    [Description("生产任务单审核时更新生产合同单据状态为已关闭")]

    public class updateContractStatusForPROD : AbstractOperationServicePlugIn
    {
        List<string> ContractIDList = new List<string>();
        private List<long> lotIds = new List<long>();
        public override void OnPreparePropertys(BOS.Core.DynamicForm.PlugIn.Args.PreparePropertysEventArgs e)
        {
            e.FieldKeys.Add("FFormId");
            e.FieldKeys.Add("FId");
            e.FieldKeys.Add("FFormId");
            base.OnPreparePropertys(e);
        }
        public override void EndOperationTransaction(BOS.Core.DynamicForm.PlugIn.Args.EndOperationTransactionArgs e)
        {
            foreach (DynamicObject item in e.DataEntitys)
            {
                string formId = "";

                if (item != null)
                {
              //      formId = Convert.ToString(item["PAEZ_ICMO_Entry_Sales"]);
                    DynamicObjectCollection detailsForContract = null;
                    detailsForContract = item["PAEZ_ICMO_Entry_Sales"] as DynamicObjectCollection;
                    //if (item["PAEZ_ICMO_Entry_Sales"])
                    foreach (DynamicObject ContractObj in detailsForContract)
                    {
                       
                        ContractIDList.Add(Convert.ToString(ContractObj["F_PAEZ_ICContractNo"]));
                    }

                    //if (formId == "PAEZ_ICContract")
                    //{
                    //    ContractIDList.Add(Convert.ToString(item["FID"])); 
                        string SQLfilterSelect = string.Format(@"select FID from F_PAEZ_ICContract 
                                        WHERE F_PAEZ_ICContractNo  IN ({0})", string.Join(",", ContractIDList));

                        using (IDataReader rd = DBUtils.ExecuteReader(this.Context, SQLfilterSelect))
                        {     
                             while (rd.Read())
                             {
                                 lotIds.Add(rd.GetInt32(0));

                             }
                         }
                         string SQLfilter = string.Format(@"UPDATE F_PAEZ_ICContract SET FBILLSTATUS = 5
                                                   WHERE FID IN ({0})", string.Join(",", lotIds));

                         DBUtils.Execute(this.Context, SQLfilter);
                        
                    }
                }


                //DynamicObjectCollection detailsForContract = null;
                //detailsForContract = item["PAEZ_ICMO"] as DynamicObjectCollection;





              }

            


                    
        }



    }
