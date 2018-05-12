using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.Metadata.ConvertElement.PlugIn;
using Kingdee.BOS.Core.Metadata.ConvertElement.PlugIn.Args;
using Kingdee.BOS.Core.Metadata.EntityElement;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.ServiceHelper;
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
    [Description("生产根据材质过滤生产合同")]
    public class PRODMaterialQualityForContract : AbstractConvertPlugIn
    {
        List<string> materialList = new List<string>();

        public override void OnParseFilterOptions(ParseFilterOptionsEventArgs e)
        {
            string PRODMaterialQuality = Convert.ToString(e.TargetData["F_PAEZ_Material_Id"]);
            string MaterialQualityFilterProd = "";

            int a = e.SourceBusinessInfo.GetEntryCount();

            Entity FillStatus = e.SourceBusinessInfo.GetEntity("FBillHead");
            DynamicObject FillStatusObj = new DynamicObject(FillStatus.DynamicObjectType);

            string FillStatusSqlFilter = Convert.ToString(FillStatusObj["FBillStatus"]);


            //先查物料表 查出物料ID 然后去合同信息过滤数据
            DynamicObject[] lotMasters = null;
            string filter = string.Format("F_BEM_CZ ='{0}'", PRODMaterialQuality);
            OQLFilter of = OQLFilter.CreateHeadEntityFilter(filter);
            lotMasters = BusinessDataServiceHelper.Load(this.Context, "BD_MATERIAL", null, of);

            if (lotMasters.Count() > 0)
            {
                foreach (DynamicObject objma in lotMasters)
                {
                    materialList.Add(Convert.ToString(objma[0]));
                }
            }




            string sqlSearchCondition = " FMATERIALID in ('{0}')" + string.Join("','", materialList);


            //      MaterialQualityFilterProd = sqlSearchCondition;

            if (String.IsNullOrEmpty(e.FilterOptionsSQL))
            {
                e.FilterOptionsSQL = sqlSearchCondition;
            }



        }


    }
}
