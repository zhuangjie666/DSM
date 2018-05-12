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

namespace Kingdee.K3.BEM.SCM.Bussiness.PlugIn.client.bill
{
    [Description("解析批号带出长宽厚等")]
    public class FlotResolution : AbstractBillPlugIn
    {
        public override void DataChanged(BOS.Core.DynamicForm.PlugIn.Args.DataChangedEventArgs e)
        {
            if (this.View.UserParameterKey.Equals("SAL_OUTSTOCK") || this.View.UserParameterKey.Equals("SP_ReturnMtrl") || this.View.UserParameterKey.Equals("SAL_RETURNSTOCK") ||
                this.View.UserParameterKey.Equals("STK_MisDelivery") || this.View.UserParameterKey.Equals("STK_OEMInStockRETURN") || this.View.UserParameterKey.Equals("SAL_SaleOrder"))
            {
                if (e.Field.Key.EqualsIgnoreCase("FLot"))
                {
                    string sFlot = "";
                    string sMaterial="";
                    Object lot = this.View.Model.GetValue(e.Field.Key, e.Row);
                    if (null != lot && null != lot as DynamicObject)
                    {
                        DynamicObject lotObj = lot as DynamicObject;
                        sFlot = lotObj.DynamicObjectType.Properties.Contains("Number") ? lotObj["Number"].ToString() : null;
                    }
                    else
                    {
                        sFlot = Convert.ToString(this.View.Model.GetValue(e.Field.Key, e.Row));
                    }
                    object material = this.View.Model.GetValue("FMaterialID", e.Row);
                    if (null != material && null != material as DynamicObject)
                    {
                        DynamicObject materialObj = material as DynamicObject;
                        sMaterial = materialObj.DynamicObjectType.Properties.Contains("msterID") ? materialObj["msterID"].ToString() : null;
                    }
                    //       queryParam.FilterClauseWihtKey = " FNUMBER = @FNUMBER AND FCREATEORGID = @FCREATEORGID AND FMaterialID=@FMaterialID AND FBIZTYPE = 1"; 
                    //     queryParam.SqlParams.Add(new SqlParam("@FMaterialID", KDDbType.Int64, Convert.ToInt64(mainOrg["msterID"])));

                    DynamicObject lotEntity = null;
                    if (!sFlot.IsNullOrEmptyOrWhiteSpace())
                    {
                        FormMetadata metadata = MetaDataServiceHelper.Load(this.Context, "BD_BatchMainFile") as FormMetadata;
                        List<SqlParam> lstParams = new List<SqlParam>();
                        QueryBuilderParemeter queryParam = new QueryBuilderParemeter();
                        queryParam.BusinessInfo = metadata.BusinessInfo;
                        queryParam.FilterClauseWihtKey = " FNUMBER = @FNUMBER AND FMaterialID = @FMaterialID AND FBIZTYPE = 1";              
                        queryParam.SqlParams.Add(new SqlParam("@FNUMBER", KDDbType.String, sFlot));
                        string mainOrgKey = this.View.BusinessInfo.MainOrgField.Key;
                        DynamicObject mainOrg = this.View.Model.GetValue(mainOrgKey) as DynamicObject;                
                        queryParam.SqlParams.Add(new SqlParam("@FMaterialID", KDDbType.String, sMaterial));
                        DynamicObject[] orgs = BusinessDataServiceHelper.Load(this.Context, metadata.BusinessInfo.GetDynamicObjectType(), queryParam);
                        if (orgs.Count() > 0)
                        {
                            lotEntity = orgs.FirstOrDefault();
                        }
                    }
                    if (null != lotEntity)
                    {

                        //长度
                        this.View.Model.SetValue("F_Length", lotEntity["F_Length"], e.Row);
                        //宽度
                        this.View.Model.SetValue("F_Width", lotEntity["F_Width"], e.Row);
                        //厚度
                        this.View.Model.SetValue("F_Thinkness", lotEntity["F_Thinkness"], e.Row);
                        //实长
                        this.View.Model.SetValue("F_RealLength", lotEntity["F_RealLength"], e.Row);
                        //实宽
                        this.View.Model.SetValue("F_RealWidth", lotEntity["F_RealWidth"], e.Row);
                        //实厚
                        this.View.Model.SetValue("F_RealThinkness", lotEntity["F_RealThinkness"], e.Row);
                        //规格
                        this.View.Model.SetValue("F_Specification", lotEntity["F_Specification"], e.Row);
                        //计重方式
                        this.View.Model.SetValue("F_WeightMethod", lotEntity["F_WeightMethod"], e.Row);
                        //质量等级
                        this.View.Model.SetValue("F_QualityLevel", lotEntity["F_QualityLevel"], e.Row);
                        //边部状况
                        this.View.Model.SetValue("F_EdgeState", lotEntity["F_EdgeState"], e.Row);
                        ////表面
                        //this.View.Model.SetValue("F_Surface", lotEntity["F_Surface"], e.Row);
                        ////材质
                        //this.View.Model.SetValue("F_MQuailty", lotEntity["F_MQuailty"], e.Row);
                        ////类别
                        //this.View.Model.SetValue("F_Category", lotEntity["F_Category"], e.Row);
                        //标准
                        this.View.Model.SetValue("F_Standard", lotEntity["F_Standard"], e.Row);
                        //碳含量
                        this.View.Model.SetValue("F_CContent", lotEntity["F_CContent"], e.Row);
                        //铬含量
                        this.View.Model.SetValue("F_CrContent", lotEntity["F_CrContent"], e.Row);
                        //成份备注
                        this.View.Model.SetValue("F_ElementRemark", lotEntity["F_ElementRemark"], e.Row);
                        //性能备注
                        this.View.Model.SetValue("F_PerRemark", lotEntity["F_PerRemark"], e.Row);
                        //表面备注
                        this.View.Model.SetValue("F_Thinkness", lotEntity["F_Thinkness"], e.Row);
                        //其他备注
                        this.View.Model.SetValue("F_SurfaceRemark", lotEntity["F_SurfaceRemark"], e.Row);
                        //产地
                        this.View.Model.SetValue("F_Origin", lotEntity["F_Origin"], e.Row);
                        //质保书号
                        this.View.Model.SetValue("F_QANo", lotEntity["F_QANo"], e.Row);
                        //成品类别
                        this.View.Model.SetValue("F_PCategory", lotEntity["F_PCategory"], e.Row);

                    }
                }
            }
        }
    }
}
