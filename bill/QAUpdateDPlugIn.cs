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
using Kingdee.BOS.App.Data;
using Kingdee.K3.BEM.SCM.Service.Plugin;


namespace Kingdee.K3.BEM.SCM.Bussiness.PlugIn.client.bill
{
    [Description("按钮更新质保书号")]
    public class QAUpdateDPlugIn : AbstractDynamicFormPlugIn
    {
        private string ids = "";  //所选 数据行的IDS
        private string lotIds = "";  //所选 的批号IDS
        private string PformId = "";
        private string tableName = "";
        private string entryIds ="";

        private List<string> updateSqls = new List<string>();

        object value;
     //   DynamicObject[] lotMasters;
    //    List<long> lotEntryID = new List<long>();
        public override void OnInitialize(InitializeEventArgs e)
        {
       
            ids = this.GetCustomParameterStringValue(e.Paramter.GetCustomParameter("ids"));
            lotIds = this.GetCustomParameterStringValue(e.Paramter.GetCustomParameter("lotIds"));          
            entryIds = this.GetCustomParameterStringValue(e.Paramter.GetCustomParameter("entryIds"));
            PformId = this.GetCustomParameterStringValue(e.Paramter.GetCustomParameter("PformId"));
            
            //if (!entryIds.IsNullOrEmptyOrWhiteSpace())
            //{

            //    string sql = string.Format("SELECT FLOTID FROM {0} WHERE FLOTID IN (SELECT FLOT FROM T_STK_INSTOCKENTRY WHERE FENTRYID IN('{1}'))", "T_BD_LOTMASTER", string.Join(",", entryIds));
            //    using (IDataReader reader = DBUtils.ExecuteReader(this.Context, sql))
            //    {
            //        while (reader.Read())
            //        {
            //            lotEntryID.Add(reader.GetInt32(0));
            //        }
            //    }

            //    string filter = string.Format("FLOTID IN ({0})", string.Join(",", lotEntryID));
            //    OQLFilter of = OQLFilter.CreateHeadEntityFilter(filter);
            //    lotMasters = BusinessDataServiceHelper.Load(this.Context, "BD_BatchMainFile", null, of);

            //}
            //foreach (DynamicObject item in lotMasters)
            //{
            //    int i = 1;
            //    if (null != item)
            //    {
            //        this.View.Model.SetValue("F_RealLength", item["F_RealLength"],i);
            //        i = i + 1;
            //    }


            //}

            base.OnInitialize(e);
           
            
        }
        private string GetCustomParameterStringValue(object obj)
        {
            if (obj != null)
            {
                return (string)obj;
            }
            return "";
        }





        public override void AfterCreateModelData(EventArgs e)
        {
            List<Bem_QanoArgs> resultList = null;
            BizQanoService service = BizQanoService.GetInstance();
            int seq = 0;//父单据体行

            Entity entity = this.View.BusinessInfo.GetEntity("FEntity");
            DynamicObjectCollection objs = this.View.Model.GetEntityDataObject(entity);

            //this.Model.ClearNoDataRow();
            objs.Clear();
            resultList=service.GetBillMaterialList(this.Context, ids, entryIds, PformId);
           
            if (resultList.Count>0)
            {
                foreach (Bem_QanoArgs one in resultList) 
                {
                    
                   
                    DynamicObject obj = new DynamicObject(entity.DynamicObjectType);
                    objs.Add(obj);
                    //this.Model.CreateNewEntryRow(entity,seq);

                    //obj["F_QANo"] = one.qano;
                    //obj["F_RealLength"] = one.RealLength;
                    //obj["F_RealWidth"] = one.RealWidth;
                    //obj["F_RealThinkness"] = one.RealThinkness;
                    //obj["F_Length"] = one.Length;
                    //obj["F_Width"] = one.Width;
                    //obj["F_Thinkness"] = one.Thinkness;
                    //obj["F_Specification"] = one.Specification;
                    //obj["F_CContent"] = one.CContent;
                    //obj["F_CrContent"] = one.CrContent;
                    //obj["F_ElementRemark"] = one.ElementRemark;
                    //obj["F_PerRemark"] = one.PerRemark;
                    //obj["F_SurfaceRemark"] = one.SurfaceRemark;
                    //obj["F_OtherRemaek"] = one.OtherRemaek;
                    //obj["F_WeightMethod_Id"] = one.weightMethod;
                    //obj["F_QualityLevel_Id"] = one.qualityLevel;
                    //obj["F_EdgeState_Id"] = one.edgeState;
                    ////obj["F_Surface"] = one.surface;
                    ////obj["F_MQuailty_Id"] = one.mQuailty;
                    ////obj["F_Category_Id"] = one.category;
                    //obj["F_Standard_Id"] = one.standard;
                    //obj["F_Origin_Id"] = one.origin;
                    //obj["F_PCategory_Id"] = one.pCategory;
                    //obj["FMATERIALID_Id"] = one.masterId;
                    //obj["F_BEM_billseq"] = one.seq;
                    //obj["FLOT_Id"] = one.FLotNo;
                    //obj["F_BEM_billentryid"] = one.entryid;
                    //obj["F_BEM_billno"] = one.FBILLNO;
                    this.View.Model.SetValue("F_QANo", one.qano, seq);
                    this.View.Model.SetValue("F_RealLength", one.RealLength, seq);
                    this.View.Model.SetValue("F_RealWidth", one.RealWidth, seq);
                    this.View.Model.SetValue("F_RealThinkness", one.RealThinkness, seq);
                    this.View.Model.SetValue("F_Length", one.Length, seq);
                    this.View.Model.SetValue("F_Width", one.Width, seq);
                    this.View.Model.SetValue("F_Thinkness", one.Thinkness, seq);
                    this.View.Model.SetValue("F_Specification", one.Specification, seq);
                    this.View.Model.SetValue("F_CContent", one.CContent, seq);
                    this.View.Model.SetValue("F_CrContent", one.CrContent, seq);
                    this.View.Model.SetValue("F_ElementRemark", one.ElementRemark, seq);
                    this.View.Model.SetValue("F_PerRemark", one.PerRemark, seq);
                    this.View.Model.SetValue("F_SurfaceRemark", one.SurfaceRemark, seq);
                    this.View.Model.SetValue("F_OtherRemaek", one.OtherRemaek, seq);
                    this.View.Model.SetValue("F_WeightMethod", one.weightMethod, seq);
                    this.View.Model.SetValue("F_QualityLevel", one.qualityLevel, seq);
                    this.View.Model.SetValue("F_EdgeState", one.edgeState, seq);
                    this.View.Model.SetValue("F_Surface", one.surface, seq);
                    this.View.Model.SetValue("F_MQuailty", one.mQuailty, seq);
                    this.View.Model.SetValue("F_Category", one.category, seq);
                    this.View.Model.SetValue("F_Standard", one.standard, seq);
                    this.View.Model.SetValue("F_Origin", one.origin, seq);
                    this.View.Model.SetValue("F_PCategory", one.pCategory, seq);

                    this.View.Model.SetValue("FMATERIALID", one.masterId, seq);
                    this.View.Model.SetValue("F_BEM_billseq", one.seq, seq);
                    this.View.Model.SetValue("FLOT", one.FLotNo, seq);
                    this.View.Model.SetValue("F_BEM_billentryid", one.entryid, seq);
                    this.View.Model.SetValue("F_BEM_billno", one.FBILLNO, seq);

                    this.View.Model.SetEntryCurrentRowIndex("FEntity", seq);
                    obj["Seq"] = ++seq;
                    
                    
                }
                
                this.View.UpdateView("FEntity");
            }
        }


        public override void AfterBarItemClick(AfterBarItemClickEventArgs e)
        {
            if (e.BarItemKey.Equals("tbsave"))
            {
                long entryid = 0;
                
                getTableName();
                Entity entity = this.View.BusinessInfo.GetEntity("FEntity");
                DynamicObjectCollection objs = this.View.Model.GetEntityDataObject(entity);
                DynamicObject[] lotMasters=null;
                if (!lotIds.IsEmpty() && !lotIds.Equals("0"))
                {
                    string filter = string.Format("FLOTID IN ('{0}')", lotIds);
                    OQLFilter of = OQLFilter.CreateHeadEntityFilter(filter);
                    lotMasters = BusinessDataServiceHelper.Load(this.Context, "BD_BatchMainFile", null, of);
                }
                foreach (DynamicObject obj in objs)
                {
                    value = obj["F_QANo"];
                    entryid = Convert.ToInt64( obj["F_BEM_billentryid"]);
                    if (entryid==0)
                    {
                        continue;
                    }
                    foreach (DynamicObject lotobj in lotMasters)
                    {
                        if (lotobj["id"].Equals(obj["FLOT_Id"]))
                        {
                            lotobj["F_QANo"] = obj["F_QANo"];
                        }
                        
                    }
                    
                    updateSqls.Add(string.Format(@"UPDATE  {0}  set F_QANo ='{1}'  WHERE fentryid  = {2}  ", tableName, value, entryid));

                }
                
                ISaveService service = Kingdee.BOS.App.ServiceHelper.GetService<ISaveService>();
                service.Save(this.Context, lotMasters);
                DBUtils.ExecuteBatch(this.Context, updateSqls, updateSqls.Count);
                //返回值到父窗口
                this.View.ReturnToParentWindow("true");
                this.View.Close();
            }
            base.AfterBarItemClick(e);
        }
        private void getTableName() {
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
        }
    }
	}

