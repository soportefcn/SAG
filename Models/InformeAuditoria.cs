using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class InformeAuditoria
    {
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        public int AuditoraID { get; set; }
       // public int LineasAtencionID { get; set; }
        public int TipoAuditoriaID { get; set; }
        public DateTime FechaAuditoria { get; set; }
        public DateTime FechaInforme { get; set; }
        public int MesDesde { get; set; }
        public int PeriodoDesde { get; set; }
        public int MesHasta { get; set; }
        public int PeriodoHasta { get; set; }
        public string MontoAuditado { get; set; }
        public string Documentos { get; set; }

        //Documentacion

        public string Documento_1 { get; set; }
        public string Documento_2 { get; set; }
        public string Documento_3 { get; set; }
        public string Documento_4 { get; set; }
        public string Documento_5 { get; set; }
        public string Documento_6 { get; set; }
        public string Documento_7 { get; set; }
        public string Documento_8 { get; set; }
        public string Documento_9 { get; set; }
        public string Documento_10 { get; set; }
        public string Documento_11 { get; set; }
        public string Documento_12 { get; set; }
        public string Documento_13 { get; set; }
        public string Documento_14 { get; set; }
        public string Documento_15 { get; set; }
        public string Documento_16 { get; set; }
        public string Documento_17 { get; set; }


        public string III_Ing_1 { get; set; }
        public string III_Ing_2 { get; set; }
        public string III_Ing_3 { get; set; }
        public string III_Ing_4 { get; set; }
        public string III_Ing_5 { get; set; }
        public string III_Ing_6 { get; set; }
        public string III_Rei_1 { get; set; }
        public string III_Rei_2 { get; set; }
        public string III_Rei_3 { get; set; }
        public string III_Rei_4 { get; set; }
        public string III_Rei_5 { get; set; }
        public string III_Rei_6 { get; set; }
        public string III_Egr_1 { get; set; }
        public string III_Egr_2 { get; set; }
        public string III_Egr_3 { get; set; }
        public string III_Egr_4 { get; set; }
        public string III_Egr_5 { get; set; }
        public string III_Egr_6 { get; set; }

        // Montos auditados

        public string III_MA_I_1 { get; set; }
        public string III_MA_I_2 { get; set; }
        public string III_MA_I_3 { get; set; }
        public string III_MA_I_4 { get; set; }
        public string III_MA_I_5 { get; set; }
        public string III_MA_I_6 { get; set; }
        public string III_MA_E_1 { get; set; }
        public string III_MA_E_2 { get; set; }
        public string III_MA_E_3 { get; set; }
        public string III_MA_E_4 { get; set; }
        public string III_MA_E_5 { get; set; }
        public string III_MA_E_6 { get; set; }

        // V Seguimiento

        public int? V_NUltimaSename { get; set; }
        public DateTime? V_NUltimaSenameFecha { get; set; }
        public string V_US_ObsSuperadas { get; set; }
        public int? V_US_ObsSuperadasOfSename { get; set; }
        public DateTime? V_US_ObsSuperadasOfSenameFecha { get; set; }
        public string V_US_ObsPendientes { get; set; }
        public int? V_US_ObsPendientesOfSename { get; set; }
        public DateTime? V_US_ObsPendientesOfSenameFecha { get; set; }
        public string V_US_Obs { get; set; }
        public int? V_NUltimaCodeni { get; set; }
        public DateTime? V_NUltimaCodeniFecha { get; set; }
        public string V_UC_ObsSuperadas { get; set; }
        public DateTime? V_UC_ObsSuperadasFecha { get; set; }
        public string V_UC_ObsPendientes { get; set; }
        public DateTime? V_UC_ObsPendientesFecha { get; set; }
        public string V_UC_Obs { get; set; }

        // VIII. Plazos

        public int? VIII_Dias { get; set; }
        public DateTime? VIII_FechaEntrega { get; set; }

        //VII. Conclusiones
        public string VII_A_Cumple { get; set; }
        public string VII_A_Obs { get; set; }
        public string VII_B1_SiNo { get; set; }
        public string VII_B1_Obs { get; set; }
        public string VII_B2_SiNo { get; set; }
        public string VII_B2_Obs { get; set; }
        public string VII_C_Obs { get; set; }

        // VI. Resultados
        public string VI_1_a_Director { get; set; }
        public string VI_1_a_2do { get; set; }
        public string VI_1_a_3er { get; set; }
        public string VI_1_a_Obs { get; set; }
        public string VI_1_b_Emitido { get; set; }
        public string VI_1_b_Archivo { get; set; }
        public string VI_1_b_Firmado { get; set; }
        public string VI_1_b_Correlativo { get; set; }
        public string VI_1_b_Cheques { get; set; }
        public string VI_1_b_Obs { get; set; }
        public string VI_1_c_Cartolas { get; set; }
        public string VI_1_c_EnviaCartolas { get; set; }
        public string VI_1_c_Obs { get; set; }
        public string VI_1_d_Suma { get; set; }
        public string VI_1_d_Archivadas { get; set; }
        public string VI_1_d_Cheques { get; set; }
        public string VI_1_d_Firma { get; set; }
        public string VI_1_d_Obs { get; set; }
        public string VI_2_a_Coincide { get; set; }
        public string VI_2_a_Comp { get; set; }
        public string VI_2_b_Glosa { get; set; }
        public string VI_2_b_Comp { get; set; }
        public string VI_2_c_Firma { get; set; }
        public string VI_2_c_Comp { get; set; }
        public string VI_2_d_Egresos { get; set; }
        public string VI_2_d_Comp { get; set; }
        public string VI_2_e_EgresosNoConforme { get; set; }
        public string VI_2_e_Comp { get; set; }
        public string VI_2_f_SinDoc { get; set; }
        public string VI_2_f_Comp { get; set; }
        public string VI_2_g_Envia { get; set; }
        public string VI_2_g_Comp { get; set; }
        public string VI_2_Obs { get; set; }
        public string VI_3_a_Monto { get; set; }
        public string VI_3_b_Arqueo { get; set; }
        public string VI_3_c_Firma { get; set; }
        public string VI_3_d_Efecto { get; set; }
        public string VI_3_e_Reposicion { get; set; }
        public string VI_3_f_Proyecto { get; set; }
        public string VI_3_g_Reembolsos { get; set; }
        public string VI_3_h_Boletas { get; set; }
        public string VI_3_i_Superiores { get; set; }
        public string VI_3_j_Combustibles { get; set; }
        public string VI_3_k_Planillas { get; set; }
        public string VI_3_l_Actividades { get; set; }
        public string VI_3_m_Movilizacion { get; set; }
        public string VI_3_n_Interurbana { get; set; }
        public string VI_3_nn_Obs { get; set; }
        public string VI_4_a_Programas { get; set; }
        public string VI_4_b_Elaborados { get; set; }
        public string VI_4_c_Firmas { get; set; }
        public string VI_4_d_Obs { get; set; }
        public string VI_5_a_Direcciones { get; set; }
        public string VI_5_b_Firma { get; set; }
        public string VI_5_c_Obs { get; set; }
        public string VI_6_a_Secretaria { get; set; }
        public string VI_6_b_Impuesto { get; set; }
        public string VI_6_c_Declaraciones { get; set; }
        public string VI_6_d_Plazo { get; set; }
        public string VI_6_e_Formulario29 { get; set; }
        public string VI_6_f_Autocuidado { get; set; }
        public string VI_6_g_Contrato { get; set; }
        public string VI_6_h_Inconsistencias { get; set; }
        public string VI_6_y_Rectificadas { get; set; }
        public string VI_6_j_Obs { get; set; }
        public string VI_7_a_Provisiona { get; set; }
        public string VI_7_b_Costos { get; set; }
        public string VI_7_c_Fondos { get; set; }
        public string VI_7_d_Monto { get; set; }
        public string VI_7_e_Superavit { get; set; }
        public string VI_7_f_Obs { get; set; }

        public string VI_B_1_Convenio { get; set; }
        public string VI_B_2_Remuneraciones { get; set; }
        public string VI_B_3_Electronica { get; set; }
        public string VI_B_4_InduccionDirector { get; set; }
        public string VI_B_5_Induccion2do { get; set; }
        public string VI_B_6_InduccionSecretaria { get; set; }
        public string VI_B_7_Sueldo { get; set; }
        public string VI_B_8_Contrato { get; set; }
        public string VI_B_9_Bono { get; set; }
        public string VI_B_10_Descuentos { get; set; }
        public string VI_B_11_Obs { get; set; }

        public string VI_C_1a1_Arriendo { get; set; }
        public string VI_C_1a2_Copia { get; set; }
        public string VI_C_1a3_Recibos { get; set; }
        public string VI_C_1a4_Termino { get; set; }
        public string VI_C_1a5_Nombre { get; set; }
        public string VI_C_1a6_Obs { get; set; }
        public string VI_C_1b1_Copia { get; set; }
        public string VI_C_1b2_Escritura { get; set; }
        public string VI_C_1b3_Devolucion { get; set; }
        public string VI_C_1b4_Registro { get; set; }
        public string VI_C_1b5_Llamadas { get; set; }
        public string VI_C_1b6_Evidencia { get; set; }
        public string VI_C_1b7_Obs { get; set; }
        public string VI_C_1c1_Copia { get; set; }
        public string VI_C_1c2_Escritura { get; set; }
        public string VI_C_1c3_Asignados { get; set; }
        public string VI_C_1c4_Devolucion { get; set; }
        public string VI_C_1c5_Controles { get; set; }
        public string VI_C_1c6_Obs { get; set; }
        public string VI_C_1d1_Copia { get; set; }
        public string VI_C_1d2_Escritura { get; set; }
        public string VI_C_1d3_Equipo { get; set; }
        public string VI_C_1d4_Devolucion { get; set; }
        public string VI_C_1d5_Obs { get; set; }
        public string VI_C_1e1_Copia { get; set; }
        public string VI_C_1e2_Escritura { get; set; }
        public string VI_C_1e3_Equipos { get; set; }
        public string VI_C_1e4_Devolucion { get; set; }
        public string VI_C_1e5_Obs { get; set; }
        public string VI_C_1f1_Copia { get; set; }
        public string VI_C_1f2_Certificados { get; set; }
        public string VI_C_1f3_Bitacoras { get; set; }
        public string VI_C_1f4_Factura { get; set; }
        public string VI_C_1f5_Obs { get; set; }
        public string VI_C_1g1_Copia { get; set; }
        public string VI_C_1g2_Obs { get; set; }

        public string VI_C_2a1_General { get; set; }
        public string VI_C_2a2_Firmado { get; set; }
        public string VI_C_2a3_Copia { get; set; }
        public string VI_C_2a4_Distribucion { get; set; }
        public string VI_C_2a5_Activo { get; set; }
        public string VI_C_2a6_Robos { get; set; }
        public string VI_C_2a7_Informado { get; set; }
        public string VI_C_2b1_Hoja { get; set; }
        public string VI_C_2b2_Copia { get; set; }
        public string VI_C_2b3_Actualizadas { get; set; }
        public string VI_C_2c1_Pendientes { get; set; }
        public string VI_C_2c2_NoIncorporadas { get; set; }
        public string VI_C_2d1_Pendientes { get; set; }
        public string VI_C_2d2_NoIncorporadas { get; set; }
        public string VI_C_2e1_Pendientes { get; set; }
        public string VI_C_2e2_NoIncorporadas { get; set; }
        public string VI_C_2f_Obs { get; set; }
        public string VI_C_3a_Control { get; set; }
        public string VI_C_3b_Existe { get; set; }
        public string VI_C_3c_Director { get; set; }
        public string VI_C_3d_Obs { get; set; }
        public int? VI_C_4a_Cob1 { get; set; }
        public int? VI_C_4a_Cob2 { get; set; }
        public int? VI_C_4a_Cob3 { get; set; }
        public int? VI_C_4a_Cob4 { get; set; }
        public int? VI_C_4a_Cob5 { get; set; }
        public int? VI_C_4a_Cob6 { get; set; }
        public int? VI_C_4b_Int1 { get; set; }
        public int? VI_C_4b_Int2 { get; set; }
        public int? VI_C_4b_Int3 { get; set; }
        public int? VI_C_4b_Int4 { get; set; }
        public int? VI_C_4b_Int5 { get; set; }
        public int? VI_C_4b_Int6 { get; set; }
        public decimal? VI_C_4c_Por1 { get; set; }
        public decimal? VI_C_4c_Por2 { get; set; }
        public decimal? VI_C_4c_Por3 { get; set; }
        public decimal? VI_C_4c_Por4 { get; set; }
        public decimal? VI_C_4c_Por5 { get; set; }
        public decimal? VI_C_4c_Por6 { get; set; }
        public string VI_C_4d_Obs { get; set; }
        public string VI_C_5a_Compras { get; set; }
        public string VI_C_5b_Autorizado { get; set; }
        public string VI_C_5c_Ordenes { get; set; }
        public string VI_C_5d_Actualizados { get; set; }
        public string VI_C_5e_Saldos { get; set; }
        public string VI_C_5g_Obs { get; set; }
        public string V_D_1a { get; set; }
        public string V_D_1a_ { get; set; }
        public string V_D_1b { get; set; }
        public string V_D_1b_ { get; set; }
        public string V_D_1c { get; set; }
        public string V_D_1c_ { get; set; }
        public string V_D_1d { get; set; }
        public string V_D_1d_ { get; set; }
        public string V_D_1e { get; set; }
        public string V_D_1e_ { get; set; }
        public string V_D_1_Obs { get; set; }
        public string V_D_2a { get; set; }
        public string V_D_2a_ { get; set; }
        public string V_D_2b { get; set; }
        public string V_D_2b_ { get; set; }
        public string V_D_2c { get; set; }
        public string V_D_2c_ { get; set; }
        public string V_D_2d { get; set; }
        public string V_D_2d_ { get; set; }
        public string V_D_2e { get; set; }
        public string V_D_2e_ { get; set; }
        public string V_D_2_Obs { get; set; }

        public string VI_C_5_Utiliza { get; set; }

        public virtual Proyecto Proyecto { get; set; }
        public virtual TipoAuditoria TipoAuditoria { get; set; }
        public virtual Persona Auditora { get; set; }
    }
}