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

namespace Kingdee.K3.BEM.SCM.Service.Plugin
{
    public class BizQanoService
    {
        private static BizQanoService instance;
        private static object _lock = new object();
        private string tableName1 = "";
        private string tableName2 = "";
        private  BizQanoService()
        {
        }


        /// <summary>
        /// 获取唯一实例
        /// </summary>
        /// <returns></returns>
        public static BizQanoService GetInstance()
        {
            if (instance == null)
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new BizQanoService();
                    }
                }
            }
            return instance;
        }
        public List<Bem_QanoArgs> GetBillMaterialList(Context ctx, string ids, string entryIds, string pformId)
        {
            List<Bem_QanoArgs> list = new List<Bem_QanoArgs>();
            StringBuilder stringBuilder = new StringBuilder();
            getTableName(pformId);
            getSelectSql(stringBuilder, ids, entryIds);
            


            using (IDataReader dataReader = DBUtils.ExecuteReader(ctx, stringBuilder.ToString()))
            {
                while (dataReader.Read())
                {
                    Bem_QanoArgs qanoArgs = new Bem_QanoArgs();
                    qanoArgs.seq = dataReader["seq"].ToString();
                    qanoArgs.entryid = dataReader["entryid"].ToString();
                    qanoArgs.masterId = dataReader["FMATMASTERID"].ToString();
                    qanoArgs.FLotNo = dataReader["FLOT"].ToString();
                    qanoArgs.FBILLNO = dataReader["FBILLNO"].ToString();

                    qanoArgs.qano = dataReader["qano"].ToString();
                    qanoArgs.RealLength = Convert.ToDecimal(dataReader["realLen"]);
                    qanoArgs.RealWidth = Convert.ToDecimal(dataReader["realwidth"]);
                    qanoArgs.RealThinkness = Convert.ToDecimal(dataReader["realthinkness"]);
                    qanoArgs.Length = Convert.ToDecimal(dataReader["len"]);
                    qanoArgs.Width = Convert.ToDecimal(dataReader["width"]);
                    qanoArgs.Thinkness = Convert.ToDecimal(dataReader["thinkness"]);
                    qanoArgs.Specification = dataReader["spec"].ToString();
                    qanoArgs.CContent = dataReader["cContent"].ToString();
                    qanoArgs.CrContent = dataReader["cRcontent"].ToString();
                    qanoArgs.ElementRemark = dataReader["elementRemark"].ToString();
                    qanoArgs.PerRemark = dataReader["preRemark"].ToString();
                    qanoArgs.SurfaceRemark = dataReader["surfaceRemark"].ToString();
                    qanoArgs.OtherRemaek = dataReader["otherRemark"].ToString();                  
                    qanoArgs.surface = dataReader["surface"].ToString();
                    qanoArgs.mQuailty = dataReader["mQuailty"].ToString();
                    qanoArgs.category = dataReader["category"].ToString();
                    qanoArgs.standard = dataReader["standard"].ToString();
                    qanoArgs.pCategory = dataReader["pCategory"].ToString();
                    qanoArgs.weightMethod = dataReader["weightMethod"].ToString();
                    qanoArgs.qualityLevel = dataReader["qulityLevel"].ToString();
                    qanoArgs.edgeState = dataReader["edgeState"].ToString();
                    qanoArgs.origin = dataReader["origin"].ToString();


                    list.Add(qanoArgs);
                }
                dataReader.Close();
            }
            return list;
        
        }
        private void getSelectSql(StringBuilder stringBuilder, string ids, string entryIds )
        {
            string filter1 = "";
            if (!ids.IsEmpty())
            {
                filter1 =string.Format(@" t1.fid in ('{0}')",ids);
            }
            else if (!entryIds.IsEmpty())
            {
                filter1 =string.Format(@" t2.fentryid in ('{0}') ",entryIds);
            }
                
            stringBuilder.AppendFormat(@"SELECT 
                                        t1.fid,t1.FBILLNO,
                                         t2.fentryid  entryid,
                                         t2.fseq  seq,
                                        t2.F_QANo qano,
                                        t2.F_REALLENGTH realLen,
                                        t2.F_REALWIDTH realwidth,
                                        t2.F_REALTHINKNESS realthinkness,
                                        t2.F_SPECIFICATION spec,
                                        t2.F_LENGTH len,
                                        t2.F_WIDTH width,
                                        t2.F_THINKNESS thinkness,
                                        t2.F_CCONTENT cContent,
                                        t2.F_CRCONTENT cRcontent,
                                        t2.F_ELEMENTREMARK elementRemark,
                                        t2.F_PERREMARK preRemark,
                                        t2.F_SURFACEREMARK surfaceRemark,
                                        t2.F_OTHERREMAEK otherRemark,
                                        t2.F_Origin origin,
                                        t4.F_WeightMethod weightMethod,
                                        t4.F_QualityLevel qulityLevel,
                                        t4.F_EdgeState edgeState,
                                        t3.F_BEM_BM surface,
                                        t3.F_BEM_CZ mQuailty,
                                        t3.F_BEM_LB category,
                                        t4.F_Standard standard,
                                        t4.F_PCategory pCategory,
                                        ISNULL(t4.FLOTID,0) AS FLOT, ISNULL(t4.FNUMBER,'') AS FLotNo,
                                        t3.FMATERIALID FMATMASTERID
                                        FROM {0}  t1
                                        inner join  {1} t2 on t1.fid=t2.fid
                                        INNER JOIN T_BD_MATERIAL t3 ON t2.FMATERIALID=t3.FMATERIALID 
                                        LEFT JOIN T_BD_LOTMASTER t4 ON t2.FLOT=t4.FLOTID 
                                        where {2} order by t1.FBILLNO,t2.fseq ", tableName1, tableName2, filter1);
            
        }
        private void getTableName(string pformId)
        {
            switch (pformId)
            {
                case "STK_InStock":  //采购入库
                    tableName1 = "T_STK_InStock";
                    tableName2 = "T_STK_INSTOCKENTRY";
                    break;
                case "STK_MISCELLANEOUS"://其他入库
                    tableName1 = "T_STK_MISCELLANEOUS";
                    tableName2 = "T_STK_MISCELLANEOUSENTRY";
                    break;
                case "SP_InStock"://简单生产入库
                    tableName1 = "T_SP_INSTOCK";
                    tableName2 = "T_SP_INSTOCKENTRY";
                    break;
                case "STK_InitInStock"://期初采购入库
                    tableName1 = "T_STK_INITINSTOCK";
                    tableName2 = "T_STK_INITINSTOCKENTRY";
                    break;
                case "STK_OEMInStock"://受托加工材料入库
                    tableName1 = "T_STK_OEMINSTOCK";
                    tableName2 = "T_STK_OEMINSTOCKENTRY";
                    break;
                case "STK_InvInit":
                    tableName1 = "T_STK_INVINIT";
                    tableName2 = "T_STK_INVINITDETAIL";
                    break;
                default: break;
            }
        }
    }
}
