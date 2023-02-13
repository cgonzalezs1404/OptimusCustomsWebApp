using OptimusCustomsWebApp.Interface;

namespace OptimusCustomsWebApp.Data.Service
{
    public class ServiceContainer : IServiceContainer
    {
        private CatalogoService catalogoService;
        private FacturaService facturaService;
        private NavigationQueryService queryService;
        private OperacionService operacionService;
        private UsuarioService usuarioService;

        public ServiceContainer(
            CatalogoService catalogoService, 
            FacturaService facturaService, 
            NavigationQueryService queryService, 
            UsuarioService usuarioService, 
            OperacionService operacionService)
        {
            this.catalogoService = catalogoService;
            this.facturaService = facturaService;
            this.queryService = queryService;
            this.usuarioService = usuarioService;
            this.operacionService = operacionService;
        }

        public CatalogoService CatalogoService
        {
            get { return catalogoService; }
        }

        public FacturaService FacturaService
        {
            get { return facturaService; }
        }

        public NavigationQueryService QueryService
        {
            get { return queryService; }
        }

        public OperacionService OperacionService
        {
            get { return operacionService; }
        }

        public UsuarioService UsuarioService
        {
            get { return usuarioService; }
        }

        public bool IsBusy { get; set; }
    }
}
