using OptimusCustomsWebApp.Interface;
using OptimusCustomsWebApp.Model.Enum;
using System.Collections.Generic;

namespace OptimusCustomsWebApp.Data.Service
{
    public class NavigationQueryService : INavigationQuery
    {
        private Dictionary<TipoPagina, Dictionary<string, string>> Uri = new Dictionary<TipoPagina, Dictionary<string, string>>();

        public Dictionary<string, string> GetQueryString(TipoPagina tp)
        {
            if (tp == TipoPagina.Factura)
            {
                return Uri.GetValueOrDefault(TipoPagina.Factura);
            }
            if (tp == TipoPagina.Operacion)
            {
                return Uri.GetValueOrDefault(TipoPagina.Operacion);
            }
            if (tp == TipoPagina.Usuario)
            {
                return Uri.GetValueOrDefault(TipoPagina.Usuario);
            }
            return null;
        }

        public void SetQueryString(TipoPagina tp, Dictionary<string, string> query)
        {
            if (Uri.ContainsKey(tp))
                Uri.Remove(tp);
            Uri.Add(tp, query);
        }
    }
}
