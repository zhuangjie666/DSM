using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.Metadata.ConvertElement.PlugIn;
using Kingdee.BOS.Core.Metadata.ConvertElement.PlugIn.Args;
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
    [Description("大分条-根据材质过滤生产选料")]
    public class DFTMaterialQualityForSourceMaterial : AbstractConvertPlugIn
    {

        List<string> materialList = new List<string>();

        public override void OnParseFilterOptions(ParseFilterOptionsEventArgs e)
        {
            string MaterialQualityDTF = Convert.ToString(e.TargetData["F_PAEZ_Material_Id"]);
            string MaterialQualityFilterDTF = "";

            //先查物料表 查出物料ID 然后去即时库存表查
            DynamicObject[] lotMasters = null;       
            string filter = string.Format("F_BEM_CZ ='{0}'", MaterialQualityDTF);
            OQLFilter of = OQLFilter.CreateHeadEntityFilter(filter);
            lotMasters = BusinessDataServiceHelper.Load(this.Context, "BD_MATERIAL", null, of);

            if (lotMasters.Count() > 0)
            {
                foreach (DynamicObject objma in lotMasters)
                {
                    materialList.Add(Convert.ToString(objma[0]));
                }
            }

            MaterialQualityFilterDTF = String.Format(" FMATERIALID in ('{0}')", string.Join("','", materialList));

            if (String.IsNullOrEmpty(e.FilterOptionsSQL))
            {
                e.FilterOptionsSQL = MaterialQualityFilterDTF;
            }
        }
    }
}
