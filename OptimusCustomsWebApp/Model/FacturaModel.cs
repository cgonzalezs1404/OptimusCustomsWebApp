﻿using System;

namespace OptimusCustomsWebApp.Model
{
    public class FacturaModel
    {
        public int IdOperacion { get; set; }
        public int IdFactura { get; set; }
        public int IdTipoFactura { get; set; }
        public string TipoFactura { get; set; }
        public int IdEstadoFactura { get; set; }
        public string EstadoFactura { get; set; }
        public string RazonSocial { get; set; }
        public DateTime FechaEmision { get; set; }
        public string RFC { get; set; }
        public string Serie { get; set; }
        public string Folio { get; set; }
        public double Total { get; set; }
        public string Descripcion { get; set; }
        public byte[] FilePdf { get; set; }
        public byte[] FileXml { get; set; }
        public bool EsAprobado { get; set; }
        public bool EsPagada { get; set; }
        public string Comentarios { get; set; }
    }
}
