using Kingdee.BOS.Core.List.PlugIn;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.Metadata.ConvertElement.PlugIn;
using Kingdee.BOS.Core.Metadata.EntityElement;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.ServiceHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingdee.K3.BEM.SCM.Bussiness.PlugIn.client.pordm
{
    [Description("生产任务单下推工序计划单带入合同信息，投入料，工序计划等信息")]
    public class Prod_downPull : AbstractConvertPlugIn
    {
        //获取源单数据(选单)
        DynamicObjectCollection sourceDataCollection = null;
        Entity FZLSubEntity = null;
        Dictionary<int, DynamicObject> IdDynamicMap = new Dictionary<int, DynamicObject>();
        
        public override void OnGetDrawSourceData(BOS.Core.Metadata.ConvertElement.PlugIn.Args.GetDrawSourceDataEventArgs e)
        {
            base.OnGetDrawSourceData(e);
            sourceDataCollection = e.SourceData;
        }

        //获取源单数据(下推)
        public override void OnGetSourceData(BOS.Core.Metadata.ConvertElement.PlugIn.Args.GetSourceDataEventArgs e)
        {
            base.OnGetSourceData(e);
            sourceDataCollection = e.SourceData;
        }
        public override void AfterConvert(Kingdee.BOS.Core.Metadata.ConvertElement.PlugIn.Args.AfterConvertEventArgs e)
        {
            base.AfterConvert(e);
            //获取头数据
            var headEntity = e.Result.FindByEntityKey("FBillHead");
            //获取元数据 用于获取源单需要转化的动态集合
            object[] pkids = sourceDataCollection.Select(p => p["FID"]).Distinct().ToArray();
            FormMetadata metadata = MetaDataServiceHelper.Load(this.Context, "PAEZ_ICMO") as FormMetadata;
            DynamicObject[] objs = BusinessDataServiceHelper.Load(this.Context, pkids, metadata.BusinessInfo.GetDynamicObjectType());
            
            
            Entity FEntityWorkSeq = e.TargetBusinessInfo.GetEntity("FEntity");
            Entity FentityICMO = e.TargetBusinessInfo.GetEntity("FEntityICMOIN");
            DynamicObjectCollection sourceProdPlanCollection = null;
            DynamicObjectCollection sourceICMOCollection = null;
            
            foreach(var extendDataEntity in headEntity)
            {
                DynamicObjectCollection objectProdPlanCollect = extendDataEntity["FEntity"] as DynamicObjectCollection;
                DynamicObjectCollection objectMetailICOMCollection = extendDataEntity["PAEZ_SHWorkMaterialIN"] as DynamicObjectCollection;
               
                for (int i = objectProdPlanCollect.Count - 1; i >= 0; i--)
                {
                    objectProdPlanCollect.RemoveAt(i);               
                }
       
                DynamicObjectCollection objectICMO = null;
                foreach (var sourceItem in objs)
                {
                    sourceProdPlanCollection = sourceItem["PAEZ_ICMO_Entry_ProProcedure"] as DynamicObjectCollection;
                    sourceICMOCollection = sourceItem["PAEZ_ICMO_Entry_ICItem"] as DynamicObjectCollection;
        
                }
        
                foreach (var sourceItem in sourceProdPlanCollection)
                {
                    DynamicObject objectProdPlan = new DynamicObject(FEntityWorkSeq.DynamicObjectType);
                    objectProdPlan["seq"]=sourceItem["seq"];
                    objectProdPlan["F_PAEZ_ProduceID_Id"] = sourceItem["F_PAEZ_ProduceID_Id"];
                    objectProdPlan["F_PAEZ_ProduceID"] = sourceItem["F_PAEZ_ProduceID"];
                    objectProdPlan["F_PAEZ_Times"] = sourceItem["F_PAEZ_Times"];
                    objectProdPlan["F_PAEZ_Equipment_Id"] = sourceItem["F_PAEZ_MachineID_Id"];
                    objectProdPlan["F_PAEZ_Equipment"] = sourceItem["F_PAEZ_MachineID"];
                    objectProdPlan["F_PAEZ_Rolling"] = sourceItem["F_PAEZ_Rolling"];
                    objectProdPlan["F_PAEZ_Notes"] = sourceItem["F_PAEZ_Notes"];
                    objectProdPlanCollect.Add(objectProdPlan);             
                }

                foreach (var sourceItem in sourceICMOCollection)
                {
                    DynamicObject sourceICMO = new DynamicObject(FentityICMO.DynamicObjectType);
                    sourceICMO["Seq"] = sourceItem["seq"];
                    sourceICMO["F_PAEZ_Material"] = sourceItem["F_PAEZ_ICItemID"];                      //原料代码
                    sourceICMO["F_PAEZ_Material_Id"] = sourceItem["F_PAEZ_ICItemID_Id"];                //ID
            //        sourceICMO["F_PAEZ_BaseProperty"] = sourceItem["F_PAEZ_ICItemName"];                //原料名称
             //       sourceICMO["F_PAEZ_BaseProperty2_Id"] = sourceItem["F_PAEZ_ICItemMaterial"];           //材质
                    sourceICMO["F_PAEZ_IcItemLevel"] = sourceItem["F_PAEZ_IcItemLevel"];                //等级
                    sourceICMO["F_PAEZ_Model"] = sourceItem["F_PAEZ_ICItemModel"];                            //规格
                    sourceICMO["F_PAEZ_LotMa"] = sourceItem["F_PAEZ_Lot"];                            //批号
                    sourceICMO["F_PAEZ_ReelItemIN"] = sourceItem["F_PAEZ_ICItemReelNO"];                  //卷号
                    sourceICMO["F_PAEZ_MaWeight"] = sourceItem["F_PAEZ_ICItemWeight"];                      //重量
                    sourceICMO["F_PAEZ_UnitIDMAWeight"] = sourceItem["F_PAEZ_ICItemWeightUNITID"];          //重量单位
                    sourceICMO["F_PAEZ_QtyMa"] = sourceItem["F_PAEZ_IcItemQty"];                            //数量
                    sourceICMO["F_PAEZ_UnitID1"] = sourceItem["F_PAEZ_ICItemQtyUnitID"];                        //数量单位
                    sourceICMO["F_PAEZ_MaWidth"] = sourceItem["F_PAEZ_ICItemWidth"];                        //宽度
                    sourceICMO["F_PAEZ_MaThickness"] = sourceItem["F_PAEZ_ICItemThickness"];                //厚度
                    sourceICMO["F_PAEZ_MaLength"] = sourceItem["F_PAEZ_ICItemLength"];                      //长度
                    sourceICMO["F_PAEZ_MaWidthRE"] = sourceItem["F_PAEZ_ICItemRefWidth"];                    //参宽
                    sourceICMO["F_PAEZ_MaThicknessRE"] = sourceItem["F_PAEZ_ICItemRefThickness"];            //參厚
                    sourceICMO["F_PAEZ_CCONTENTSItemIN"] = sourceItem["F_PAEZ_ICItemCContents"];        //碳含量
                    sourceICMO["F_PAEZ_CRCONTENTSItemIN"] = sourceItem["F_PAEZ_ICItemCRContents"];      //铬含量
                    sourceICMO["F_PAEZ_PRODUCTEDGEItemIN"] = sourceItem["F_PAEZ_ICItemEdge"];    //边部状况
                 //   sourceICMO["F_PAEZ_StockLot"] = sourceItem["F_PAEZ_LotStock"];                      //库存批次

                    objectMetailICOMCollection.Add(sourceICMO);


                }

                
            
            }
        }
     }  
}