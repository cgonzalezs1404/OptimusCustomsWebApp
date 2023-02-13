using OptimusCustomsWebApp.Data.Service;

namespace OptimusCustomsWebApp.Interface
{
    public interface IServiceContainer
    {
        CatalogoService CatalogoService { get; }
        FacturaService FacturaService { get; }
        NavigationQueryService QueryService { get; }
        OperacionService OperacionService { get; }
        UsuarioService UsuarioService { get; }
    }
}
