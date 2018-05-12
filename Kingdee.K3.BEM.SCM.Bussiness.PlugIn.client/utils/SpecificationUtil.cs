using Kingdee.BOS;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.ServiceHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingdee.K3.BEM.SCM.Bussiness.PlugIn.client.utils
{
    public static class SpecificationUtil
    {
        public static string getSpecifications(string strThick, string strWidth, string strLong,string edgeState) 
        {
            strThick = strThick.Trim();
            strWidth = strWidth.Trim();
            strLong = strLong.Trim();
            if (strWidth.EndsWith(".0"))
            {
                strWidth = strWidth.Substring(0, strWidth.Length - 2);
            }
            if (strLong.EndsWith(".0"))
            {
                strLong = strLong.Substring(0, strLong.Length - 2);
            }

            //if ((strLong == "" || strLong == "0") && (strWidth == "" || strWidth == "0"))
            //{
            //    return strThick + "*N*N";
            //}
            //else if (strLong == "" || strLong == "0")
            //{
            //    return strThick + "*" + strWidth + "*C";
            //}
            return strThick + "*" + strWidth + "/" + edgeState + "*" + strLong;

        
        }
    }
}
