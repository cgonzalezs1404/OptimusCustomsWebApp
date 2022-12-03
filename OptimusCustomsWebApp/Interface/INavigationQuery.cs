using OptimusCustomsWebApp.Model.Enum;
using System.Collections.Generic;

namespace OptimusCustomsWebApp.Interface
{
    public interface INavigationQuery
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetQueryString(TipoPagina tp);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tp"></param>
        /// <param name="queryString"></param>
        void SetQueryString(TipoPagina tp, Dictionary<string, string> query);
    }
}
