using Kingdee.BOS.Core.Metadata.ConvertElement.PlugIn;
using Kingdee.BOS.Core.Metadata.ConvertElement.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingdee.K3.BEM.SCM.Bussiness.PlugIn.client.pordm
{
    [Description("生产根据材质过滤生产合同")]
    public class DTFMaterialQualityForContract : AbstractConvertPlugIn
    {
        
        public override void OnParseFilterOptions(ParseFilterOptionsEventArgs e)
        {
            string MaterialQualityProd = Convert.ToString(e.TargetData["F_PAEZ_Material"]);
            string MaterialQualityFilterProd = "";

            

            MaterialQualityFilterProd = String.Format(" F_PAEZ_PRODUCTMATERIAL = {0} ", MaterialQualityProd);

            if (String.IsNullOrEmpty(e.FilterOptionsSQL))
            {
                e.FilterOptionsSQL = MaterialQualityFilterProd;
            }



        }
    }
}
