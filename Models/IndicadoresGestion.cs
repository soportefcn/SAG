using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class IndicadoresGestion
    {
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        public int Periodo { get; set; }
        public int? CapCobertura { get; set; }
        public decimal? CapIntervenciones { get; set; }
        public int? IngSubvencion { get; set; }
        public int? IngOtros { get; set; }
        public decimal? CosPersonal { get; set; }
        public decimal? CosFuncionamiento { get; set; }
        public decimal? CosTecnico { get; set; }
        public decimal? CosBeneficiario { get; set; }
        public decimal? CosInversion { get; set; }
        public decimal? CosFondo { get; set; }
        public int? FinPrestamos { get; set; }
        public int? FinPrestamosOtros { get; set; }
        public int? FinAportes { get; set; }
        public int? FinAportesExcendentes { get; set; }
        public int? FinAportesTerceros { get; set; }
        public int? FinProveedores { get; set; }
        public int? CalObjSename { get; set; }
        public int? CalRecSename { get; set; }
        public int? CalObsSename { get; set; }
        public int? CalObjCodeni { get; set; }
        public int? CalRecCodeni { get; set; }
        public int? CalObsCodeni { get; set; }
        public int? CalImpuestos { get; set; }
        public int? CalInventario { get; set; }
        public decimal? ProAporte { get; set; }
        public int? ProHoras { get; set; }
        public int? ProSaldo { get; set; }
        public decimal? ProResultado { get; set; }
        public decimal? ProDesviacion { get; set; }
        public decimal? ProDesviacionEgr { get; set; }
        public decimal? ProCostoNino { get; set; }

        public virtual Proyecto Proyecto { get; set; }
    }
}