
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.ServiceHelper;
//using Kingdee.BOS.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingdee.BOS;
using Kingdee.BOS.Contracts;
using Kingdee.BOS.Orm.Metadata.DataEntity;
using Kingdee.BOS.Core.SqlBuilder;
namespace Kingdee.K3.BEM.SCM.Service.Plugin
{
     [Description("入库单更新批号主档")]
    
    public class BatchMainFileFlotSplitPulgin : AbstractOperationServicePlugIn
    {
        private List<long> lotIds = new List<long>();
         public override void OnPreparePropertys(BOS.Core.DynamicForm.PlugIn.Args.PreparePropertysEventArgs e)
         {
             e.FieldKeys.Add("FLot");
             e.FieldKeys.Add("F_RealLength");
             e.FieldKeys.Add("F_RealWidth");
             e.FieldKeys.Add("F_RealThinkness");
             e.FieldKeys.Add("F_Length");
             e.FieldKeys.Add("F_Width");
             e.FieldKeys.Add("F_Thinkness");
             e.FieldKeys.Add("F_QualityLevel");
             e.FieldKeys.Add("F_WeightMethod");
             e.FieldKeys.Add("F_EdgeState");
             e.FieldKeys.Add("F_Surface");
             e.FieldKeys.Add("F_MQuailty");
             e.FieldKeys.Add("F_Category");
             e.FieldKeys.Add("F_Standard");
             e.FieldKeys.Add("F_CContent");
             e.FieldKeys.Add("F_CrContent");
             e.FieldKeys.Add("F_ElementRemark");
             e.FieldKeys.Add("F_PerRemark");
             e.FieldKeys.Add("F_SurfaceRemark");
             e.FieldKeys.Add("F_OtherRemaek");   
             e.FieldKeys.Add("F_Origin");
             e.FieldKeys.Add("F_QANo");
             e.FieldKeys.Add("F_PCategory");
             e.FieldKeys.Add("F_YJNUMBER");
             e.FieldKeys.Add("FFormId");

             base.OnPreparePropertys(e);
         }
         public override void EndOperationTransaction(BOS.Core.DynamicForm.PlugIn.Args.EndOperationTransactionArgs e)
         {
             lotIds.Clear();
             List<SqlObject> sqlList = new List<SqlObject>();
            
             foreach (var item in e.DataEntitys)
             {
                 string formId = "";
                 if (item.DynamicObjectType.Name.Equals("InvInit"))
                 {
                     formId = "STK_InvInit";
                 }
                 else
                 {
                     formId = item["FFormId"].ToString();
                 }
                
                 DynamicObjectCollection details = null;
                 switch (formId)
                 {
                     case "STK_MISCELLANEOUS":
                        details  = item["STK_MISCELLANEOUSENTRY"] as DynamicObjectCollection;
                         break;
                     case "STK_InStock":
                         details = item["InStockEntry"] as DynamicObjectCollection;
                         break;
                     case "SP_InStock":
                         details = item["Entity"] as DynamicObjectCollection;
                         break;
                     case "STK_OEMInStock":
                         details = item["OEMInStockEntry"] as DynamicObjectCollection;
                         break;
                     case "STK_InitInStock":
                         details = item["InitInStockEntry"] as DynamicObjectCollection;
                         break;
                     case "STK_InvInit":
                         details = item["InvInitDetail"] as DynamicObjectCollection;
                         break;

                     default: break;
                 }
                 if (details == null)
                 {
                     return;
                 }
                 foreach (var entryItem in details)
                 {
                     Object lotObj = entryItem["LOT"];
                     if (null != lotObj && Convert.ToInt16((lotObj as DynamicObject)["BizType"]) == 1)
                     {
                         sqlList.Add(this.getSqlParams(formId, entryItem, item));
                     }
                 }
             }
             if (sqlList.Count > 0 && lotIds.Count > 0)
             {
                 DBUtils.ExecuteBatch(this.Context, sqlList);
                 string filter = string.Format("FLOTID IN ({0})", string.Join(",", lotIds));
                 OQLFilter of = OQLFilter.CreateHeadEntityFilter(filter);
                 DynamicObject[] lotMasters = BusinessDataServiceHelper.Load(this.Context, "BD_BatchMainFile", null, of);
                 ISaveService service = Kingdee.BOS.App.ServiceHelper.GetService<ISaveService>();
                 service.Save(this.Context, lotMasters);
             }
         }
         private SqlObject getSqlParams(string formId, DynamicObject entryItem, DynamicObject item)
         {
             List<SqlParam> sqlParams = new List<SqlParam>();
             List<string> updateFields = new List<string>();
              List<string> edgeList = new List<string>();
             //其他入库单组织标识带F，其他单不带
            long orgID = 0;
            if ("STK_MISCELLANEOUS".Equals(formId))
             {
                 orgID = Convert.ToInt64(item["FStockOrgId_Id"]); //其他入库单的库存组织给到仓库创建组织   更新时，其他入库单的库存字段=仓库的创建组织
             }
             else
             {
                 orgID = Convert.ToInt64(item["StockOrgId_Id"]);
             }
     
             long materialID = 0;
             //物料ID
             if (entryItem.DynamicObjectType.Properties.Contains("FMaterialID"))
             {
                 materialID = Convert.ToInt64(entryItem["FMaterialID"]);
             }
            else if (entryItem.DynamicObjectType.Properties.Contains("MaterialId_Id"))
             {
                 materialID = Convert.ToInt64(entryItem["MaterialId_Id"]);
             }
             else
             {
                 materialID = Convert.ToInt64(entryItem["MaterialID_Id"]);
             }

             //长度
             if (entryItem.DynamicObjectType.Properties.Contains("F_Length") && null != entryItem["F_Length"])
             {
                 updateFields.Add("F_Length = @F_Length");
                 sqlParams.Add(new SqlParam("@F_Length", KDDbType.Decimal, Convert.ToDecimal(entryItem["F_Length"])));
             }
             //宽度
             if (entryItem.DynamicObjectType.Properties.Contains("F_Width") && null != entryItem["F_Width"])
             {
                 updateFields.Add("F_Width = @F_Width");
                 sqlParams.Add(new SqlParam("@F_Width", KDDbType.Decimal, Convert.ToDecimal(entryItem["F_Width"])));
             }
             //厚度
             if (entryItem.DynamicObjectType.Properties.Contains("F_Thinkness") && null != entryItem["F_Thinkness"])
             {
                 updateFields.Add("F_Thinkness = @F_Thinkness");
                 sqlParams.Add(new SqlParam("@F_Thinkness", KDDbType.Decimal, Convert.ToDecimal(entryItem["F_Thinkness"])));
             }

             //实际长度
             if (entryItem.DynamicObjectType.Properties.Contains("F_RealLength") && null != entryItem["F_RealLength"])
             {
                 updateFields.Add("F_RealLength = @F_RealLength");
                 sqlParams.Add(new SqlParam("@F_RealLength", KDDbType.Decimal, Convert.ToDecimal(entryItem["F_RealLength"])));
             }
             //实际宽度
             if (entryItem.DynamicObjectType.Properties.Contains("F_RealWidth") && null != entryItem["F_RealWidth"])
             {
                 updateFields.Add("F_RealWidth = @F_RealWidth");
                 sqlParams.Add(new SqlParam("@F_RealWidth", KDDbType.Decimal, Convert.ToDecimal(entryItem["F_RealWidth"])));
             }
             //实际厚度
             if (entryItem.DynamicObjectType.Properties.Contains("F_RealThinkness") && null != entryItem["F_RealThinkness"])
             {
                 updateFields.Add("F_RealThinkness = @F_RealThinkness");
                 sqlParams.Add(new SqlParam("@F_RealThinkness", KDDbType.Decimal, Convert.ToDecimal(entryItem["F_RealThinkness"])));
             }
             //计重方式
             if (entryItem.DynamicObjectType.Properties.Contains("F_WeightMethod_Id") && null != entryItem["F_WeightMethod"]
                 && !string.IsNullOrWhiteSpace(entryItem["F_WeightMethod"].ToString()))
             {
                 updateFields.Add("F_WeightMethod = @F_WeightMethod");
                 sqlParams.Add(new SqlParam("@F_WeightMethod", KDDbType.AnsiString, Convert.ToString(entryItem["F_WeightMethod_Id"])));
             }
             //质量等级
             if (entryItem.DynamicObjectType.Properties.Contains("F_QualityLevel_Id") && null != entryItem["F_QualityLevel"]
                 && !string.IsNullOrWhiteSpace(entryItem["F_QualityLevel"].ToString()))
             {
                 updateFields.Add("F_QualityLevel = @F_QualityLevel");
                 sqlParams.Add(new SqlParam("@F_QualityLevel", KDDbType.AnsiString, Convert.ToString(entryItem["F_QualityLevel_Id"])));
             }
             //边部状况
             if (entryItem.DynamicObjectType.Properties.Contains("F_EdgeState_Id") && null != entryItem["F_EdgeState"]
                  && !string.IsNullOrWhiteSpace(entryItem["F_EdgeState"].ToString()))
             {
                 updateFields.Add("F_EdgeState = @F_EdgeState");
                 sqlParams.Add(new SqlParam("@F_EdgeState", KDDbType.AnsiString, Convert.ToString(entryItem["F_EdgeState_Id"])));
             }
             //表面
             if (entryItem.DynamicObjectType.Properties.Contains("F_Surface_Id") && null != entryItem["F_Surface"])
             {
                 updateFields.Add("F_Surface = @F_Surface");
                 sqlParams.Add(new SqlParam("@F_Surface", KDDbType.AnsiString, Convert.ToString(entryItem["F_Surface_Id"])));
             }
             //材质
             if (entryItem.DynamicObjectType.Properties.Contains("F_MQuailty_Id") && null != entryItem["F_MQuailty"])
             {
                 updateFields.Add("F_MQuailty = @F_MQuailty");
                 sqlParams.Add(new SqlParam("@F_MQuailty", KDDbType.AnsiString, Convert.ToString(entryItem["F_MQuailty_Id"])));
             }
             //类别
             if (entryItem.DynamicObjectType.Properties.Contains("F_Category_Id") && null != entryItem["F_Category"])
             {
                 updateFields.Add("F_Category = @F_Category");
                 sqlParams.Add(new SqlParam("@F_Category", KDDbType.AnsiString, Convert.ToString(entryItem["F_Category_Id"])));
             }
             
             //标准   
             if (entryItem.DynamicObjectType.Properties.Contains("F_Standard_Id") && null != entryItem["F_Standard"])
             {
                 updateFields.Add("F_Standard = @F_Standard");
                 sqlParams.Add(new SqlParam("@F_Standard", KDDbType.AnsiString, Convert.ToString(entryItem["F_Standard_Id"])));
             }
             //碳含量
             if (entryItem.DynamicObjectType.Properties.Contains("F_CContent") && null != entryItem["F_CContent"])
             {
                 updateFields.Add("F_CContent = @F_CContent");
                 sqlParams.Add(new SqlParam("@F_CContent", KDDbType.AnsiString, Convert.ToString(entryItem["F_CContent"])));
             }
             //铬含量
             if (entryItem.DynamicObjectType.Properties.Contains("F_CrContent") && null != entryItem["F_CrContent"])
             {
                 updateFields.Add("F_CrContent = @F_CrContent");
                 sqlParams.Add(new SqlParam("@F_CrContent", KDDbType.AnsiString, Convert.ToString(entryItem["F_CrContent"])));
             }
             //成份备注
             if (entryItem.DynamicObjectType.Properties.Contains("F_ElementRemark") && null != entryItem["F_ElementRemark"])
             {
                 updateFields.Add("F_ElementRemark = @F_ElementRemark");
                 sqlParams.Add(new SqlParam("@F_ElementRemark", KDDbType.AnsiString, Convert.ToString(entryItem["F_ElementRemark"])));
             }
             //源卷号
             if (entryItem.DynamicObjectType.Properties.Contains("F_YJNUMBER") && null != entryItem["F_YJNUMBER"])
             {
                 updateFields.Add("F_YJNUMBER = @F_YJNUMBER");
                 sqlParams.Add(new SqlParam("@F_YJNUMBER", KDDbType.AnsiString, Convert.ToString(entryItem["F_YJNUMBER"])));
             }
             //性能备注
             if (entryItem.DynamicObjectType.Properties.Contains("F_PerRemark") && null != entryItem["F_PerRemark"])
             {
                 updateFields.Add("F_PerRemark = @F_PerRemark");
                 sqlParams.Add(new SqlParam("@F_PerRemark", KDDbType.AnsiString, Convert.ToString(entryItem["F_PerRemark"])));
             }
             //表面备注
             if (entryItem.DynamicObjectType.Properties.Contains("F_SurfaceRemark") && null != entryItem["F_SurfaceRemark"])
             {
                 updateFields.Add("F_SurfaceRemark = @F_SurfaceRemark");
                 sqlParams.Add(new SqlParam("@F_SurfaceRemark", KDDbType.AnsiString, Convert.ToString(entryItem["F_SurfaceRemark"])));
             }
             //其他备注
             if (entryItem.DynamicObjectType.Properties.Contains("F_OtherRemaek") && null != entryItem["F_OtherRemaek"])
             {
                 updateFields.Add("F_OtherRemaek = @F_OtherRemaek");
                 sqlParams.Add(new SqlParam("@F_OtherRemaek", KDDbType.AnsiString, Convert.ToString(entryItem["F_OtherRemaek"])));
             }
             //产地
             if (entryItem.DynamicObjectType.Properties.Contains("F_Origin_Id") && null != entryItem["F_Origin"])
             {
                 updateFields.Add("F_Origin = @F_Origin");
                 sqlParams.Add(new SqlParam("@F_Origin", KDDbType.AnsiString, Convert.ToString(entryItem["F_Origin_Id"])));
             }
             //质保书
             if (entryItem.DynamicObjectType.Properties.Contains("F_QANo") && null != entryItem["F_QANo"]
                 && !string.IsNullOrWhiteSpace(entryItem["F_QANo"].ToString()))
             {
                 updateFields.Add("F_QANo = @F_QANo");
                 sqlParams.Add(new SqlParam("@F_QANo", KDDbType.AnsiString, Convert.ToString(entryItem["F_QANo"])));
             }
             //成品类别
             if (entryItem.DynamicObjectType.Properties.Contains("F_PCategory_Id") && null != entryItem["F_PCategory"])    
             {
                 updateFields.Add("F_PCategory = @F_PCategory");
                 sqlParams.Add(new SqlParam("@F_PCategory", KDDbType.AnsiString, Convert.ToString(entryItem["F_PCategory_Id"])));
             }

             //规格
             //string F_Thinkness = Convert.ToString(entryItem["F_Thinkness"]);
             //string F_Width = Convert.ToString(entryItem["F_Width"]);
             //string F_EdgeState_Id = Convert.ToString(entryItem["F_EdgeState_Id"]);
             //string F_Length = Convert.ToString(entryItem["F_Length"]);
             //string F_Specification = "";

             //if (!(F_Thinkness.Equals("")) && !(F_Width.Equals("")) && !(F_EdgeState_Id.Equals("")) && !(F_Length.Equals("")))
             //{
             //    DynamicObject F_SpecificationNo = null;
             //    FormMetadata metadata = MetaDataServiceHelper.Load(this.Context, "BOS_ASSISTANTDATA_DETAIL") as FormMetadata;
             //    List<SqlParam> lstParams = new List<SqlParam>();
             //    QueryBuilderParemeter queryParam = new QueryBuilderParemeter();
             //    queryParam.BusinessInfo = metadata.BusinessInfo;
             //    queryParam.FilterClauseWihtKey = " FENTRYID = @FENTRYID ";
             //    queryParam.SqlParams.Add(new SqlParam("@FENTRYID", KDDbType.String, F_EdgeState_Id));
             //    F_SpecificationNo = BusinessDataServiceHelper.Load(this.Context, metadata.BusinessInfo.GetDynamicObjectType(), queryParam).FirstOrDefault();
             //    string dataValue = "";
             //    if (!F_SpecificationNo["DATAVALUE"].Equals(""))
             //    {
             //        dataValue = Convert.ToString(F_SpecificationNo["DATAVALUE"]);
             //    }
             //    F_Specification = F_Thinkness + "*" + F_Width + "/" + Convert.ToString(dataValue) + "*" + F_Length;
             if (entryItem.DynamicObjectType.Properties.Contains("F_Specification") && null != entryItem["F_Specification"])
             {
                 updateFields.Add("F_Specification = @F_Specification");
                 sqlParams.Add(new SqlParam("@F_Specification", KDDbType.AnsiString, Convert.ToString(entryItem["F_Specification"])));
             }

            

             sqlParams.Add(new SqlParam("@lotNo", KDDbType.AnsiString, Convert.ToString(entryItem["LOT_Text"])));

             string sql = string.Format(@"UPDATE T_BD_LOTMASTER SET {0}
                                        WHERE FNUMBER = @lotNo AND FCREATEORGID = {1} AND FMATERIALID IN(SELECT FMASTERID FROM T_BD_MATERIAL WHERE FMATERIALID = {2}) AND FBIZTYPE = '1'"
                                            , string.Join(",", updateFields),orgID, materialID);

             string querySql = string.Format(@"SELECT FLOTID FROM T_BD_LOTMASTER 
                                        WHERE FNUMBER = '{0}' AND FCREATEORGID = {1} AND FMATERIALID IN(SELECT FMASTERID FROM T_BD_MATERIAL WHERE FMATERIALID = {2}) AND FBIZTYPE = '1'"
                                            , Convert.ToString(entryItem["LOT_Text"]), orgID, materialID);
             //string querySql = string.Format(@"SELECT FLOTID FROM T_BD_LOTMASTER WHERE FCREATEORGID = {0} AND FMATERIALID IN(SELECT FMASTERID FROM T_BD_MATERIAL WHERE FMATERIALID = {1}) AND FBIZTYPE = '1'"
             //                              ,  orgID, materialID);
             using (IDataReader rd = DBUtils.ExecuteReader(this.Context, querySql))
             {
                 
                     while (rd.Read())
                     {

                         lotIds.Add(rd.GetInt32(0));


                     }
                 }
             
             return new SqlObject(sql,sqlParams);
      
         }
    }
 }
