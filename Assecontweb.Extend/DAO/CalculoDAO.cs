using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class CalculoDAO : Connection
    {
        public CalculoDAO() 
            : base()
        {
            conn = new SqlConnection(connectionstring);
        }
        
        public bool SetCalculoByPeriodo(DateTime inicio, DateTime fim, out string erro)
        {
            string query = "";
            bool retorno = false;
            erro = "OK";
            try
            {
                conn.Open();
                #region Super Mega Hiper Query Zika
                query = @"declare @inicio datetime = @dt_inicio, 
		                            @fim datetime = @dt_fim, 
		                            @cal_id int,
		                            @exists int = 0, --(0 - Não Existe, 1 - Existe, 2 - bloqueado)
		                            @detail int, 
		                            @e_detail int = 0, 
		                            @consolidado int, 
		                            @e_consol int = 0, 
		                            @retencao int, 
		                            @e_retenc int = 0 
                            set @cal_id = isnull((select cal_int_id from SealCalculo where cal_int_mes = MONTH(@inicio) and cal_int_ano = YEAR(@inicio)), 0)
                            set @detail = isnull((select cal_int_detail from SealCalculo where cal_int_id = @cal_id), 0)
                            set @consolidado = isnull((select cal_int_consolidado from SealCalculo where cal_int_id = @cal_id), 0)
                            set @retencao = isnull((select cal_int_retencao from SealCalculo where cal_int_id = @cal_id), 0)

                            if @cal_id != 0 
                            begin 
	                            if (select cal_bit_finalizado from SealCalculo where cal_int_id = @cal_id) = 0
	                            begin
		                            set @exists = 1
	                            end else 
	                            begin 
		                            set @exists = 2
	                            end 
                            end else begin 
	                            set @exists = 0
                            end

                            if @exists = 1
                            begin
	                            if (select cal_int_detail from SealCalculo where cal_int_id = @cal_id) is not null
	                            begin
		                            set @e_detail = 1
	                            end
	                            if (select cal_int_consolidado from SealCalculo where cal_int_id = @cal_id) is not null
	                            begin
		                            set @e_consol = 1
	                            end
	                            if (select cal_int_retencao from SealCalculo where cal_int_id = @cal_id) is not null
	                            begin
		                            set @e_retenc = 1
	                            end
                            end

                            if (@exists != 2)
                            begin
	                            declare @MsDanfePri numeric(18, 6), @SpDanfePri numeric(18, 6), @MsDanfePub numeric(18, 6), @SpDanfePub numeric(18, 6), 
			                            @MsDanfeRecPri numeric(18, 6), @SpDanfeRecPri numeric(18, 6), @MsDanfeRecPub numeric(18, 6), @SpDanfeRecPub numeric(18, 6),
			                            @MsDanfeNRecPri numeric(18, 6), @SpDanfeNRecPri numeric(18, 6), @MsDanfeNRecPub numeric(18, 6), @SpDanfeNRecPub numeric(18, 6), 
			                            @MsDanfeContPri numeric(18, 6), @MsDanfeContPub numeric(18, 6), @SpDanfeContPri numeric(18, 6), @SpDanfeContPub numeric(18, 6),
			                            @MsDanfeNContPri numeric(18, 6), @MsDanfeNContPub numeric(18, 6), @SpDanfeNContPri numeric(18, 6), @SpDanfeNContPub numeric(18, 6), 
			
			                            @MsServPri numeric(18, 6), @MsServPub numeric(18, 6), @SpServPri numeric(18, 6),  @SpServPub numeric(18, 6),
			                            @MsServRecPri numeric(18, 6), @MsServRecPub numeric(18, 6), @SpServRecPri numeric(18, 6), @SpServRecPub numeric(18, 6), 
			                            @MsServNRecPri numeric(18, 6),@MsServNRecPub numeric(18, 6), @SpServNRecPri numeric(18, 6), @SpServNRecPub numeric(18, 6),
			                            @MsServCumPri numeric(18, 6), @MsServCumPub numeric(18, 6), @SpServCumPri numeric(18, 6), @SpServCumPub numeric(18, 6), 
			                            @MsServNCumPri numeric(18, 6), @MsServNCumPub numeric(18, 6), @SpServNCumPri numeric(18, 6), @SpServNCumPub numeric(18, 6)
	 
	                            select @MsDanfePri = sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE' or f.cli_des_grupo = 'NÃO CONTRIBUINTE')) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 			
			                            @SpDanfePri = sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and  a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE' or f.cli_des_grupo = 'NÃO CONTRIBUINTE')) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 

			                            @MsDanfePub = sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIB - Lei 9718' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718')) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 
			                            @SPDanfePub = sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIB - Lei 9718' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718'))then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 
			                            --Recebidas
			                            @MsDanfeRecPri = sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE' or f.cli_des_grupo = 'NÃO CONTRIBUINTE')) and (e.nfr_int_id is not null) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 
			                            @SpDanfeRecPri = sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE' or f.cli_des_grupo = 'NÃO CONTRIBUINTE')) and (e.nfr_int_id is not null) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 
			                            @MsDanfeRecPub = sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIB - Lei 9718' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718')) and (e.nfr_int_id is not null) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 
			                            @SpDanfeRecPub = sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIB - Lei 9718' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718')) and (e.nfr_int_id is not null) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 

			                            --Não Recebidas
			                            @MsDanfeNRecPri = sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE' or f.cli_des_grupo = 'NÃO CONTRIBUINTE')) and (e.nfr_int_id is null) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 
			                            @SpDanfeNRecPri = sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE' or f.cli_des_grupo = 'NÃO CONTRIBUINTE')) and (e.nfr_int_id is null) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 
			                            @MsDanfeNRecPub = sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIB - Lei 9718' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718')) and (e.nfr_int_id is null) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 
			                            @SpDanfeNRecPub = sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIB - Lei 9718' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718')) and (e.nfr_int_id is null) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 

			                            --Contribuinte
			                            @MsDanfeContPri = sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE')) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 
			                            @MsDanfeContPub = sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIB - Lei 9718')) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 
			                            @SpDanfeContPri = sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE')) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 
			                            @SpDanfeContPub = sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIB - Lei 9718')) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 

			                            --Não Contribuinte
			                            @MsDanfeNContPri = sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'NÃO CONTRIBUINTE')) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 
			                            @MsDanfeNContPub = sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718')) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 
			                            @SpDanfeNContPri = sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'NÃO CONTRIBUINTE')) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 
			                            @SpDanfeNContPub = sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
							                            (f.cli_int_id is not null and (f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718')) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end)
	                            from SealNotaFiscalDanfeItem a
	                            inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
	                            left join SealNotaFiscalDanfeEmpresa c on b.nde_int_emit = c.nde_int_id
	                            left join SealNotaFiscalDanfeEmpresa d on b.nde_int_dest = d.nde_int_id
	                            outer apply(
		                            select top 1 nfr_int_id, nfd_int_id, nfs_int_id from SealNotaFiscalRecebida 
		                            where nfd_int_id = b.nfd_int_id
	                            ) e
	                            left join SealCliente f on d.nde_des_cnpj = f.cli_des_cnpj

	                            select	@MsServPri = SUM(case when (b.nse_des_estado = 'MS' and b.nse_int_id is not null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
							                            (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) then a.nfs_num_vlrservico else 0 end), 
			                            @MsServPub = SUM(case when (b.nse_des_estado = 'MS' and b.nse_int_id is not null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
							                            (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) then a.nfs_num_vlrservico else 0 end), 
			                            @SpServPri = SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
							                            (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) then a.nfs_num_vlrservico else 0 end), 
			                            @SpServPub = SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
							                            (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) then a.nfs_num_vlrservico else 0 end), 

			                            --Recebidos
			                            @MsServRecPri = SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
							                            (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) and (d.nfr_int_id is not null) then a.nfs_num_vlrservico else 0 end), 
			                            @MsServRecPub = SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
							                            (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and (d.nfr_int_id is not null) then a.nfs_num_vlrservico else 0 end), 
			                            @SpServRecPri = SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
							                            (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) and (d.nfr_int_id is not null) then a.nfs_num_vlrservico else 0 end), 
			                            @SpServRecPub = SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
							                            (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and (d.nfr_int_id is not null) then a.nfs_num_vlrservico else 0 end), 

			                            --Não Recebidos
			                            @MsServNRecPri = SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
							                            (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) and (d.nfr_int_id is null) then a.nfs_num_vlrservico else 0 end), 
			                            @MsServNRecPub = SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
							                            (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and (d.nfr_int_id is null) then a.nfs_num_vlrservico else 0 end), 
			                            @SpServNRecPri = SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
							                            (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) and (d.nfr_int_id is null) then a.nfs_num_vlrservico else 0 end), 
			                            @SpServNRecPub = SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
							                            (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and (d.nfr_int_id is null) then a.nfs_num_vlrservico else 0 end), 

			                            --Cumulativos
			                            @MsServCumPri = SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and (e.cli_int_id is not null and 
							                            (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) and (a.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.nfs_num_vlrservico else 0 end), 
			                            @MsServCumPub = SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and (e.cli_int_id is not null and 
							                            (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and (a.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.nfs_num_vlrservico else 0 end), 
			                            @SpServCumPri = SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and (e.cli_int_id is not null and 
							                            (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) and (a.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.nfs_num_vlrservico else 0 end), 
			                            @SpServCumPub = SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and (e.cli_int_id is not null and 
							                            (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and (a.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918))  then a.nfs_num_vlrservico else 0 end), 

			                            --Não Cumulativos
			                            @MsServNCumPri = SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and (e.cli_int_id is not null and 
							                            (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) and (a.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918))then a.nfs_num_vlrservico else 0 end), 
			                            @MsServNCumPub = SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and (e.cli_int_id is not null and 
							                            (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and (a.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918))then a.nfs_num_vlrservico else 0 end), 
			                            @SpServNCumPri = SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and (e.cli_int_id is not null and 
							                            (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) and (a.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918))then a.nfs_num_vlrservico else 0 end), 
			                            @SpServNCumPub = SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and (e.cli_int_id is not null and 
							                            (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and (a.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.nfs_num_vlrservico else 0 end)
		                            from SealNotaFiscalServico a
		                            left join SealNotaFiscalServicoEmpresa b on a.nse_int_prestador = b.nse_int_id 
		                            left join SealNotaFiscalServicoEmpresa c on a.nse_int_tomador = c.nse_int_id 
		                            outer apply(
		                            select top 1 aa.nfr_int_id, aa.nfd_int_id, aa.nfs_int_id, ab.crb_dt_recebimento  
		                            from SealNotaFiscalRecebida aa 
		                            inner join SealContaReceber ab on aa.nfr_int_id = ab.nfr_int_id
		                            where aa.nfs_int_id = a.nfs_int_id
		                            ) d
		                            left join SealCliente e on c.nse_des_cnpj = e.cli_des_cnpj

	                            if (@e_detail = 0)
	                            begin
		                            insert into SealCalculoDetail (cald_int_mes, cald_int_ano, 
		                                MsDanfePri,		SpDanfePri,		MsDanfePub,		SpDanfePub, 
			                            MsDanfeRecPri,	SpDanfeRecPri,	MsDanfeRecPub,	SpDanfeRecPub,
			                            MsDanfeNRecPri,	SpDanfeNRecPri, MsDanfeNRecPub, SpDanfeNRecPub, 
			                            MsDanfeContPri,	MsDanfeContPub, SpDanfeContPri, SpDanfeContPub,
			                            MsDanfeNContPri,MsDanfeNContPub,SpDanfeNContPri,SpDanfeNContPub, 
			
			                            MsServPri,		MsServPub,		SpServPri,		SpServPub,
			                            MsServRecPri,	MsServRecPub,	SpServRecPri,	SpServRecPub, 
			                            MsServNRecPri,	MsServNRecPub,	SpServNRecPri,	SpServNRecPub,
			                            MsServCumPri,	MsServCumPub,	SpServCumPri,	SpServCumPub, 
			                            MsServNCumPri,	MsServNCumPub,	SpServNCumPri,	SpServNCumPub) 
	                            values (DATEPART(MONTH, @inicio), DATEPART(YEAR, @inicio), 
			                            @MsDanfePri,		@SpDanfePri,		@MsDanfePub,		@SpDanfePub, 
			                            @MsDanfeRecPri,		@SpDanfeRecPri,		@MsDanfeRecPub,		@SpDanfeRecPub,
			                            @MsDanfeNRecPri,	@SpDanfeNRecPri,	@MsDanfeNRecPub,	@SpDanfeNRecPub, 
			                            @MsDanfeContPri,	@MsDanfeContPub,	@SpDanfeContPri,	@SpDanfeContPub,
			                            @MsDanfeNContPri,	@MsDanfeNContPub,	@SpDanfeNContPri,	@SpDanfeNContPub, 
			
			                            @MsServPri,			@MsServPub,		@SpServPri,		@SpServPub,
			                            @MsServRecPri,		@MsServRecPub,	@SpServRecPri,	@SpServRecPub, 
			                            @MsServNRecPri,		@MsServNRecPub, @SpServNRecPri, @SpServNRecPub,
			                            @MsServCumPri,		@MsServCumPub,	@SpServCumPri,	@SpServCumPub, 
			                            @MsServNCumPri,		@MsServNCumPub, @SpServNCumPri, @SpServNCumPub);
		                            set @detail = (select IDENT_CURRENT('SealCalculoDetail'))
	                            end else begin
		                            update SealCalculoDetail 
		                            set cald_int_mes = DATEPART(MONTH, @inicio), cald_int_ano = DATEPART(YEAR, @inicio), 
		                                MsDanfePri = @MsDanfePri, SpDanfePri = @SpDanfePri, MsDanfePub = @MsDanfePub, SpDanfePub = @SpDanfePub, 
			                            MsDanfeRecPri = @MsDanfeRecPri,	SpDanfeRecPri = @SpDanfeRecPri,	MsDanfeRecPub = @MsDanfeRecPub, SpDanfeRecPub = @SpDanfeRecPub,
			                            MsDanfeNRecPri = @MsDanfeNRecPri, SpDanfeNRecPri = @SpDanfeNRecPri, MsDanfeNRecPub = @MsDanfeNRecPub, SpDanfeNRecPub = @SpDanfeNRecPub, 
			                            MsDanfeContPri = @MsDanfeContPri, MsDanfeContPub = @MsDanfeContPub, SpDanfeContPri = @SpDanfeContPri, SpDanfeContPub = @SpDanfeContPub,
			                            MsDanfeNContPri = @MsDanfeNContPri, MsDanfeNContPub = @MsDanfeNContPub,SpDanfeNContPri = @SpDanfeNContPri,SpDanfeNContPub = @SpDanfeNContPub, 
			
			                            MsServPri = @MsServPri,		MsServPub = @MsServPub,		SpServPri = @SpServPri,		SpServPub = @SpServPub,
			                            MsServRecPri = @MsServRecPri,	MsServRecPub = @MsServRecPub,	SpServRecPri = @SpServRecPri,	SpServRecPub = @SpServRecPub, 
			                            MsServNRecPri = @MsServNRecPri,	MsServNRecPub = @MsServNRecPub,	SpServNRecPri = @SpServNRecPri,	SpServNRecPub = @SpServNRecPub, 
			                            MsServCumPri = @MsServCumPri,	MsServCumPub = @MsServCumPub,	SpServCumPri = @SpServCumPri,	SpServCumPub = @SpServCumPub, 
			                            MsServNCumPri = @MsServNCumPri,	MsServNCumPub = @MsServNCumPub,	SpServNCumPri = @SpServNCumPri,	SpServNCumPub = @SpServNCumPub
		                            where cald_int_id = @detail
	                            end
                            end

                            if (@exists != 2)
                            begin
	                            declare @SpDanfeNorm numeric(18, 6), @MsDanfeNorm numeric(18, 6), @SpDanfeExc numeric(18, 6), @MsDanfeExc numeric(18, 6), @SpDanfeAdc numeric(18, 6), 
			                            @MsDanfeAdc numeric(18, 6), @SpServNCumNorm numeric(18, 6), @MsServNCumNorm numeric(18, 6), @SpServCumNorm numeric(18, 6), @MsServCumNorm numeric(18, 6), 
			                            @SpServNCumExc numeric(18, 6), @MsServNCumExc numeric(18, 6), @SpServCumExc numeric(18, 6), @MsServCumExc numeric(18, 6), @SpServNCumAdc numeric(18, 6), 
			                            @MsServNCumAdc numeric(18, 6), @SpServCumAdc numeric(18, 6), @MsServCumAdc numeric(18, 6) 
	                            select  --Danfes Não Cumulativo
			                            --(+) Normais
			                            @SpDanfeNorm = sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) 
							                            and (e.cli_int_id is not null) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 
			                            @MsDanfeNorm = sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) 
							                            and (e.cli_int_id is not null) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 
			                            --(-) Exclusão
			                            @SpDanfeExc = sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and (e.cli_int_id is not null and 
							                            (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and (f.nfd_int_id is null or f.crb_dt_recebimento > @fim) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end), 
			                            @MsDanfeExc = sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and (e.cli_int_id is not null and 
							                            (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and (f.nfd_int_id is null or f.crb_dt_recebimento > @fim) then a.ndi_num_vProd + isnull(a.ndi_num_vFrete, 0) else 0 end)  
	                            from SealNotaFiscalDanfeItem a
	                            inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
	                            left join SealNotaFiscalDanfeEmpresa c on b.nde_int_emit = c.nde_int_id
	                            left join SealNotaFiscalDanfeEmpresa d on b.nde_int_dest = d.nde_int_id
	                            left join SealCliente e on d.nde_des_cnpj = e.cli_des_cnpj
	                            outer apply(
		                            select aa.nfd_int_id, ab.crb_dt_recebimento 
		                            from SealNotaFiscalRecebida aa 
		                            inner join SealContaReceber ab on aa.nfr_int_id = ab.nfr_int_id 
		                            where b.nfd_int_id = aa.nfd_int_id and (ab.crb_dt_recebimento > @fim)
		                            group by aa.nfd_int_id, ab.crb_dt_recebimento 
	                            ) f 

	                            select  --Danfes Não Cumulativo
			                            --(+) Adição Receita Diferida
			                            @SpDanfeAdc = sum(case when e.nde_des_uf = 'SP' then a.crb_num_valorrecebido else 0 end), 
			                            @MsDanfeAdc = sum(case when e.nde_des_uf = 'MS' then a.crb_num_valorrecebido else 0 end) 
	                            from SealContaReceber a 
	                            left join SealNotaFiscalRecebida b on a.nfr_int_id = b.nfr_int_id 
	                            left join SealNotaFiscalDanfe c on b.nfd_int_id = c.nfd_int_id
	                            left join SealNotaFiscalDanfeEmpresa e on c.nde_int_emit = e.nde_int_id
	                            left join SealNotaFiscalDanfeEmpresa f on c.nde_int_dest = f.nde_int_id  
	                            left join SealCliente g on f.nde_des_cnpj = g.cli_des_cnpj
	                            where (a.crb_dt_recebimento between @inicio and @fim) and 
	                            (g.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))
	
	                            select  --Serviços Não Cumulativo
			                            --(+) Normais
			                            @SpServNCumNorm = SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
							                            (e.cli_int_id is not null) and (a.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.nfs_num_vlrservico else 0 end), 
			                            @MsServNCumNorm = SUM(case when (b.nse_des_estado = 'MS' and b.nse_int_id is not null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and (e.cli_int_id is not null) and 
							                            (a.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.nfs_num_vlrservico else 0 end), 
			                            --Serviços Cumulativo
			                            --(+) Normais
			                            @SpServCumNorm = SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
							                            (e.cli_int_id is not null) and (a.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.nfs_num_vlrservico else 0 end), 
			                            @MsServCumNorm = SUM(case when (b.nse_des_estado = 'MS' and b.nse_int_id is not null) and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
							                            (e.cli_int_id is not null) and (a.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.nfs_num_vlrservico else 0 end)
	                            from SealNotaFiscalServico a
	                            left join SealNotaFiscalServicoEmpresa b on a.nse_int_prestador = b.nse_int_id 
	                            left join SealNotaFiscalServicoEmpresa c on a.nse_int_tomador = c.nse_int_id 
	                            left join SealCliente e on c.nse_des_cnpj = e.cli_des_cnpj
	                            outer apply(
		                            select aa.nfs_int_id, ab.crb_dt_recebimento 
		                            from SealNotaFiscalRecebida aa 
		                            inner join SealContaReceber ab on aa.nfr_int_id = ab.nfr_int_id 
		                            inner join SealCliente ac on ab.crb_des_cnpjorigem = ac.cli_des_cnpj 
		                            where a.nfs_int_id = aa.nfs_int_id and (ab.crb_dt_recebimento > @fim) and 
				                            (ab.crb_dt_emissao >= @inicio and ab.crb_dt_emissao <= @fim) and 
				                            (ac.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))
		                            group by aa.nfs_int_id, ab.crb_dt_recebimento 
	                            ) f 

	                            select  --Serviços Não Cumulativo
			                            --(-) Exclusao Receitas Diferidas
			                            @SpServNCumExc = SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and (a.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918))
						                            then (case when f.nfs_int_id is null or a.nfs_num_vlrservico = isnull(f.crb_num_valorrecebido, 0) then a.nfs_num_vlrservico 
						                            when a.nfs_num_vlrservico > isnull(f.crb_num_valorrecebido, 0) then a.nfs_num_vlrservico - isnull(f.crb_num_valorrecebido, 0) else 0 end) else 0 end), 
			                            @MsServNCumExc = SUM(case when a.nfs_des_ufmunicipio = 'MS' and (a.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918))
						                            then (case when f.nfs_int_id is null or a.nfs_num_vlrservico = isnull(f.crb_num_valorrecebido, 0) then a.nfs_num_vlrservico 
						                            when a.nfs_num_vlrservico > isnull(f.crb_num_valorrecebido, 0) then a.nfs_num_vlrservico - isnull(f.crb_num_valorrecebido, 0) else 0 end) else 0 end), 
			                            --Serviços Cumulativo
			                            --(-) Exclusao Receitas Diferidas
			                            @SpServCumExc = SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and (a.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918))
						                            then (case when f.nfs_int_id is null or a.nfs_num_vlrservico = isnull(f.crb_num_valorrecebido, 0) then a.nfs_num_vlrservico 
						                            when a.nfs_num_vlrservico > isnull(f.crb_num_valorrecebido, 0) then a.nfs_num_vlrservico - isnull(f.crb_num_valorrecebido, 0) else 0 end) else 0 end), 
			                            @MsServCumExc = SUM(case when (a.nfs_des_ufmunicipio = 'MS') and (a.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918))
						                            then (case when f.nfs_int_id is null or a.nfs_num_vlrservico = isnull(f.crb_num_valorrecebido, 0) then a.nfs_num_vlrservico 
						                            when a.nfs_num_vlrservico > isnull(f.crb_num_valorrecebido, 0) then a.nfs_num_vlrservico - isnull(f.crb_num_valorrecebido, 0) else 0 end) else 0 end) 
	                            from SealNotaFiscalServico a
	                            left join SealNotaFiscalServicoEmpresa b on a.nse_int_prestador = b.nse_int_id 
	                            left join SealNotaFiscalServicoEmpresa c on a.nse_int_tomador = c.nse_int_id 
	                            left join SealCliente e on c.nse_des_cnpj = e.cli_des_cnpj
	                            outer apply(
		                            select aa.nfs_int_id, ab.crb_dt_recebimento, ab.crb_num_valor, sum(ab.crb_num_valorrecebido) as crb_num_valorrecebido
		                            from SealNotaFiscalRecebida aa 
		                            inner join SealContaReceber ab on aa.nfr_int_id = ab.nfr_int_id 
		                            inner join SealCliente ac on ab.crb_des_cnpjorigem = ac.cli_des_cnpj 
		                            where aa.nfs_int_id = a.nfs_int_id and (ab.crb_dt_recebimento > @fim) --and 
				                            --(ab.crb_dt_emissao >= @inicio and ab.crb_dt_emissao <= @fim) and 
				                            --(ac.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))
		                            group by aa.nfs_int_id, ab.crb_dt_recebimento, ab.crb_num_valor 
	                            ) f 
	                            where (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
			                            (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) --and 
			                            --(f.nfs_int_id is null or f.crb_dt_recebimento > @fim) or 
			                            --((a.nfs_num_vlrservico > f.crb_num_valorrecebido and 
			                            --(f.crb_dt_recebimento >= @inicio and f.crb_dt_recebimento <= @fim))) 

	                            select  --Serviços Não Cumulativo
			                            --(+) Adição Receita Diferida
			                            @SpServNCumAdc = SUM(case when (c.nse_int_prestador is null) and (c.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.crb_num_valorrecebido else 0 end), 
			                            @MsServNCumAdc = SUM(case when e.nse_des_estado = 'MS' and (c.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.crb_num_valorrecebido else 0 end), 
			                            --(+) Adição Receita Diferida
			                            @SpServCumAdc = SUM(case when (c.nse_int_prestador is null) and (c.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.crb_num_valorrecebido else 0 end), 
			                            @MsServCumAdc = SUM(case when e.nse_des_estado = 'MS' and (c.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.crb_num_valorrecebido else 0 end)
	                            from SealContaReceber a 
	                            left join SealNotaFiscalRecebida b on a.nfr_int_id = b.nfr_int_id 
	                            left join SealNotaFiscalServico c on b.nfs_int_id = c.nfs_int_id
	                            left join SealNotaFiscalServicoEmpresa e on c.nse_int_prestador = e.nse_int_id
	                            left join SealCliente g on a.crb_des_cnpjorigem = g.cli_des_cnpj
	                            where (a.crb_dt_recebimento between @inicio and @fim) and 
			                            (g.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))
	
	                            if (@e_consol = 0)
	                            begin
		                            insert into SealCalculoConsolidado (calc_int_mes, calc_int_ano, 
			                            SpDanfeNor, MsDanfeNor, SpDanfeExc, MsDanfeExc, SpDanfeAdc, MsDanfeAdc, 
			                            SpServNCumNor, MsServNCumNor, SpServNCumExc, MsServNCumExc, SpServNCumAdc, MsServNCumAdc, 
			                            SpServCumNor, MsServCumNor, SpServCumExc, MsServCumExc, SpServCumAdc, MsServCumAdc)
			                            values (DATEPART(MONTH, @inicio), DATEPART(YEAR, @inicio), 
			                            @SpDanfeNorm,@MsDanfeNorm,@SpDanfeExc,@MsDanfeExc, @SpDanfeAdc, @MsDanfeAdc, 
			                            @SpServNCumNorm,@MsServNCumNorm,@SpServNCumExc,@MsServNCumExc, @SpServNCumAdc, @MsServNCumAdc, 
			                            @SpServCumNorm,@MsServCumNorm,@SpServCumExc,@MsServCumExc, @SpServCumAdc, @MsServCumAdc) 
		                            set @consolidado = (select IDENT_CURRENT('SealCalculoConsolidado'))
	                            end else begin
		                            update SealCalculoConsolidado 
		                            set calc_int_mes = DATEPART(MONTH, @inicio), calc_int_ano = DATEPART(YEAR, @inicio), 
			                            SpDanfeNor = @SpDanfeNorm, MsDanfeNor = @MsDanfeNorm, SpDanfeExc = @SpDanfeExc, MsDanfeExc = @MsDanfeExc, SpDanfeAdc = @SpDanfeAdc, MsDanfeAdc = @MsDanfeAdc, 
			                            SpServNCumNor = @SpServNCumNorm, MsServNCumNor = @MsServNCumNorm, SpServNCumExc = @SpServNCumExc, MsServNCumExc = @MsServNCumExc, SpServNCumAdc = @SpServNCumAdc, MsServNCumAdc = @MsServNCumAdc, 
			                            SpServCumNor = @SpServCumNorm, MsServCumNor = @MsServCumNorm, SpServCumExc = @SpServCumExc, MsServCumExc = @MsServCumExc, SpServCumAdc = @SpServCumAdc, MsServCumAdc = @MsServCumAdc
		                            where calc_int_id = @consolidado
	                            end
                            end

                            if (@exists != 2)
                            begin
	                            declare @NCumPisPri numeric(18, 6), @NCumPisPub numeric(18, 6), @NCumCsslPri numeric(18, 6), @NCumCsslPub numeric(18, 6), @NCumCofiPri numeric(18, 6), @NCumCofiPub numeric(18, 6),
			                            @NCumIrPri numeric(18, 6), @NCumIrPub numeric(18, 6), @NCumInssPri numeric(18, 6), @NCumInssPub numeric(18, 6), @NCumIssPri numeric(18, 6), @NCumIssPub numeric(18, 6),
			                            @CumPisPri numeric(18, 6), @CumPisPub numeric(18, 6), @CumCsslPri numeric(18, 6), @CumCsslPub numeric(18, 6), @CumCofiPri numeric(18, 6), @CumCofiPub numeric(18, 6),
			                            @CumIrPri numeric(18, 6), @CumIrPub numeric(18, 6), @CumInssPri numeric(18, 6), @CumInssPub numeric(18, 6), @CumIssPri numeric(18, 6), @CumIssPub numeric(18, 6)

	                            select  --Não Cumulativos
			                            --PIS
			                            @NCumPisPub = sum(case when a.crb_des_observacao = 'PIS crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
							                            (d.nfd_int_id is not null or e.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.crb_num_valorrecebido else 0 end), 
			                            @NCumPisPri = sum(case when a.crb_des_observacao = 'PIS crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') and 
							                            (d.nfd_int_id is not null or e.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.crb_num_valorrecebido else 0 end), 
			                            --CSSL
			                            @NCumCsslPub = sum(case when a.crb_des_observacao = 'CSSL crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
							                            (d.nfd_int_id is not null or e.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.crb_num_valorrecebido else 0 end), 
			                            @NCumCsslPri = sum(case when a.crb_des_observacao = 'CSSL crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') and 
							                            (d.nfd_int_id is not null or e.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.crb_num_valorrecebido else 0 end),
			                            --COFINS
			                            @NCumCofiPub = sum(case when a.crb_des_observacao = 'COFINS crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
							                            (d.nfd_int_id is not null or e.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.crb_num_valorrecebido else 0 end), 
			                            @NCumCofiPri = sum(case when a.crb_des_observacao = 'COFINS crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') and 
							                            (d.nfd_int_id is not null or e.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.crb_num_valorrecebido else 0 end),
			                            --IR
			                            @NCumIrPub = sum(case when a.crb_des_observacao = 'IR crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
							                            (d.nfd_int_id is not null or e.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.crb_num_valorrecebido else 0 end), 
			                            @NCumIrPri = sum(case when a.crb_des_observacao = 'IR crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') and 
							                            (d.nfd_int_id is not null or e.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.crb_num_valorrecebido else 0 end),
			                            --INSS
			                            @NCumInssPub = sum(case when a.crb_des_observacao = 'INSS crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
							                            (d.nfd_int_id is not null or e.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.crb_num_valorrecebido else 0 end), 
			                            @NCumInssPri = sum(case when a.crb_des_observacao = 'INSS crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') and 
							                            (d.nfd_int_id is not null or e.nfs_int_listaservico not in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918)) then a.crb_num_valorrecebido else 0 end),
			                            --ISS
			                            @NCumIssPub = sum(case when a.crb_des_observacao = 'Iss retido Serv. Prestados' and b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') then a.crb_num_valorrecebido else 0 end), 
			                            @NCumIssPri = sum(case when a.crb_des_observacao = 'Iss retido Serv. Prestados' and b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') then a.crb_num_valorrecebido else 0 end), 
			                            --Cumulativos
			                            --PIS
			                            @CumPisPub = sum(case when a.crb_des_observacao = 'PIS crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
							                            e.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918) then a.crb_num_valorrecebido else 0 end), 
			                            @CumPisPri = sum(case when a.crb_des_observacao = 'PIS crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') and 
							                            e.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918) then a.crb_num_valorrecebido else 0 end), 
			                            --CSSL
			                            @CumCsslPub = sum(case when a.crb_des_observacao = 'CSSL crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
							                            e.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918) then a.crb_num_valorrecebido else 0 end), 
			                            @CumCsslPri = sum(case when a.crb_des_observacao = 'CSSL crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') and 
							                            e.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918) then a.crb_num_valorrecebido else 0 end),
			                            --COFINS
			                            @CumCofiPub = sum(case when a.crb_des_observacao = 'COFINS crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
							                            e.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918) then a.crb_num_valorrecebido else 0 end), 
			                            @CumCofiPri = sum(case when a.crb_des_observacao = 'COFINS crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') and 
							                            e.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918) then a.crb_num_valorrecebido else 0 end),
			                            --IR
			                            @CumIrPub = sum(case when a.crb_des_observacao = 'IR crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
							                            e.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918) then a.crb_num_valorrecebido else 0 end), 
			                            @CumIrPri = sum(case when a.crb_des_observacao = 'IR crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') and 
							                            e.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918) then a.crb_num_valorrecebido else 0 end),
			                            --INSS
			                            @CumInssPub = sum(case when a.crb_des_observacao = 'INSS crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
							                            e.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918) then a.crb_num_valorrecebido else 0 end), 
			                            @CumInssPri = sum(case when a.crb_des_observacao = 'INSS crédito sobre impostos retidos' and b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') and 
							                            e.nfs_int_listaservico in (702, 1015, 1023, 105, 2798, 2836, 107, 2917, 2918) then a.crb_num_valorrecebido else 0 end),
			                            --ISS
			                            @CumIssPub = sum(case when a.crb_des_observacao = 'Iss retido Serv. Prestados' and b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') then a.crb_num_valorrecebido else 0 end), 
			                            @CumIssPri = sum(case when a.crb_des_observacao = 'Iss retido Serv. Prestados' and b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') then a.crb_num_valorrecebido else 0 end)
	                            from SealContaReceber a 
	                            inner join SealCliente b on a.crb_des_cnpjorigem = b.cli_des_cnpj
	                            left join SealNotaFiscalRecebida c on a.nfr_int_id = c.nfr_int_id 
	                            left join SealNotaFiscalDanfe d on c.nfd_int_id = d.nfd_int_id
	                            left join SealNotaFiscalServico e on c.nfs_int_id = e.nfs_int_id 
	                            where a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim 
	                            if (@e_retenc = 0)
	                            begin
		                            insert into SealCalculoRetencao (calr_int_mes, calr_int_ano,
		                            NCumPisPri, NCumPisPub, NCumCsslPri, NCumCsslPub, NCumCofiPri, NCumCofiPub,
		                            NCumIrPri, NCumIrPub, NCumInssPri, NCumInssPub, NCumIssPri, NCumIssPub,
		                            CumPisPri, CumPisPub, CumCsslPri, CumCsslPub, CumCofiPri, CumCofiPub,
		                            CumIrPri, CumIrPub, CumInssPri, CumInssPub, CumIssPri, CumIssPub ) 
		                            values (DATEPART(MONTH, @inicio), DATEPART(YEAR, @inicio), 
		                            @NCumPisPri, @NCumPisPub, @NCumCsslPri, @NCumCsslPub, @NCumCofiPri, @NCumCofiPub,
		                            @NCumIrPri, @NCumIrPub, @NCumInssPri, @NCumInssPub, @NCumIssPri, @NCumIssPub,
		                            @CumPisPri, @CumPisPub, @CumCsslPri, @CumCsslPub, @CumCofiPri, @CumCofiPub,
		                            @CumIrPri, @CumIrPub, @CumInssPri, @CumInssPub, @CumIssPri, @CumIssPub)
		                            set @retencao = (select IDENT_CURRENT('SealCalculoRetencao'))
	                            end else begin 
		                            update SealCalculoRetencao 
		                            set calr_int_mes = DATEPART(MONTH, @inicio), calr_int_ano = DATEPART(YEAR, @inicio),
			                            NCumPisPri = @NCumPisPri, NCumPisPub = @NCumPisPub, NCumCsslPri = @NCumCsslPri, NCumCsslPub = @NCumCsslPub, NCumCofiPri = @NCumCofiPri, NCumCofiPub = @NCumCofiPub,
			                            NCumIrPri = @NCumIrPri, NCumIrPub = @NCumIrPub, NCumInssPri = @NCumInssPri, NCumInssPub = @NCumInssPub, NCumIssPri = @NCumIssPri, NCumIssPub = @NCumIssPub,
			                            CumPisPri = @CumPisPri, CumPisPub = @CumPisPub, CumCsslPri = @CumCsslPri, CumCsslPub = @CumCsslPub, CumCofiPri = @CumCofiPri, CumCofiPub = @CumCofiPub,
			                            CumIrPri = @CumIrPri, CumIrPub = @CumIrPub, CumInssPri = @CumInssPri, CumInssPub = @CumInssPub, CumIssPri = @CumIssPri, CumIssPub = @CumIssPub 
		                            where calr_int_id = @retencao
	                            end
                            end

                            if (@exists = 0)
                            begin
	                            insert into SealCalculo (cal_int_mes, cal_int_ano, cal_bit_finalizado, cal_int_detail, cal_int_consolidado, cal_int_retencao)
	                            values (DATEPART(MONTH, @inicio), DATEPART(YEAR, @inicio), 0, @detail, @consolidado, @retencao)
                            end else begin
	                            if (@exists = 1) 
	                            begin
		                            update SealCalculo set cal_int_detail = @detail where cal_int_id = @cal_id
		                            update SealCalculo set cal_int_consolidado = @consolidado where cal_int_id = @cal_id
		                            update SealCalculo set cal_int_retencao = @retencao where cal_int_id = @cal_id
	                            end
                            end";
                #endregion
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dt_inicio", inicio);
                cmd.Parameters.AddWithValue("@dt_fim", fim);

                cmd.ExecuteNonQuery();
                retorno = true;
            }
            catch (Exception ex)
            {
                retorno = false;
                erro = "ERRO: " + ex.Message;
            }
            finally
            {
                //dr.Close();
                conn.Close();
            }
            return retorno;
        }

        public bool SetCloseCalculoByPeriodo(DateTime inicio, DateTime fim, out string erro)
        {
            string query = "";
            bool retorno = false;
            erro = "OK";
            try
            {
                conn.Open();
                #region Super Mega Hiper Query Zika
                query = @"declare @inicio datetime, @mes int, @ano int
                            set @inicio = @dt_inicio
                            set @mes = MONTH(@inicio)
                            set @ano = YEAR(@inicio)

                            update SealCalculo 
                            set cal_bit_finalizado = 1 
                            where cal_int_mes = @mes and cal_int_ano = @ano";
                #endregion
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dt_inicio", inicio);

                cmd.ExecuteNonQuery();
                retorno = true;
            }
            catch (Exception ex)
            {
                retorno = false;
                erro = "ERRO: " + ex.Message;
            }
            finally
            {
                //dr.Close();
                conn.Close();
            }
            return retorno;
        }

        public bool GetCalculoDetail(ref Detail detail, DateTime inicio, DateTime fim, out string erro)
        {
            string query = "";
            bool retorno = false;
            erro = "OK";
            SetCalculoByPeriodo(inicio, fim, out erro);
            DbDataReader dr;
            if (detail == null)
                detail = new Detail();
            try
            {
                conn.Open();
                #region Query
                query = @"declare @inicio datetime, @mes int, @ano int
                            set @inicio = @dt_inicio
                            set @mes = MONTH(@inicio)
                            set @ano = YEAR(@inicio)

                            select a.cal_bit_finalizado,b.cald_int_id,b.cald_int_mes,b.cald_int_ano
                            ,b.SpDanfePri,b.SpDanfePub,b.SpDanfeRecPri,b.SpDanfeRecPub,b.SpDanfeNRecPri,b.SpDanfeNRecPub
                            ,b.SpDanfeContPri,b.SpDanfeContPub,b.SpDanfeNContPri,b.SpDanfeNContPub,b.SpServPri,b.SpServPub
                            ,b.SpServRecPri,b.SpServRecPub,b.SpServNRecPri,b.SpServNRecPub,b.SpServCumPri,b.SpServCumPub
                            ,b.SpServNCumPri,b.SpServNCumPub,b.MsDanfePri,b.MsDanfePub,b.MsDanfeRecPri,b.MsDanfeRecPub
                            ,b.MsDanfeNRecPri,b.MsDanfeNRecPub,b.MsDanfeContPri,b.MsDanfeContPub,b.MsDanfeNContPri,b.MsDanfeNContPub
                            ,b.MsServPri,b.MsServPub,b.MsServRecPri,b.MsServRecPub,b.MsServNRecPri,b.MsServNRecPub
                            ,b.MsServCumPri,b.MsServCumPub,b.MsServNCumPri,b.MsServNCumPub
                            from SealCalculo a
                            inner join SealCalculoDetail b on a.cal_int_detail = b.cald_int_id 
                            where a.cal_int_mes = @mes and a.cal_int_ano = @ano";
                #endregion
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dt_inicio", inicio);

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    detail.finalizado = Convert.ToBoolean(dr["cal_bit_finalizado"] is DBNull ? "false" : dr["cal_bit_finalizado"]);
                    #region SP
                    #region Danfes
                    detail.DanfeFaturada.SPPrivado = Convert.ToDouble(dr["SpDanfePri"]);
                    detail.DanfeFaturada.SPLei = Convert.ToDouble(dr["SpDanfePub"]);
                    detail.DanfeFaturada.SPFatu = detail.DanfeFaturada.SPPrivado + detail.DanfeFaturada.SPLei;
                    detail.DanfeFaturada.Recebidas.SPPrivado = Convert.ToDouble(dr["SpDanfeRecPri"]);
                    detail.DanfeFaturada.Recebidas.SPLei = Convert.ToDouble(dr["SpDanfeRecPub"]);
                    detail.DanfeFaturada.Recebidas.SPFatu = detail.DanfeFaturada.Recebidas.SPPrivado + detail.DanfeFaturada.Recebidas.SPLei;
                    detail.DanfeFaturada.NRecebidas.SPPrivado = Convert.ToDouble(dr["SpDanfeNRecPri"]);
                    detail.DanfeFaturada.NRecebidas.SPLei = Convert.ToDouble(dr["SpDanfeNRecPub"]);
                    detail.DanfeFaturada.NRecebidas.SPFatu = detail.DanfeFaturada.NRecebidas.SPPrivado + detail.DanfeFaturada.NRecebidas.SPLei;
                    detail.DanfeFaturada.Contribuinte.SPPrivado = Convert.ToDouble(dr["SpDanfeContPri"]);
                    detail.DanfeFaturada.Contribuinte.SPLei = Convert.ToDouble(dr["SpDanfeContPub"]);
                    detail.DanfeFaturada.Contribuinte.SPFatu = detail.DanfeFaturada.Contribuinte.SPPrivado + detail.DanfeFaturada.Contribuinte.SPLei;
                    detail.DanfeFaturada.NContribuinte.SPPrivado = Convert.ToDouble(dr["SpDanfeNContPri"]);
                    detail.DanfeFaturada.NContribuinte.SPLei = Convert.ToDouble(dr["SpDanfeNContPub"]);
                    detail.DanfeFaturada.NContribuinte.SPFatu = detail.DanfeFaturada.NContribuinte.SPPrivado + detail.DanfeFaturada.NContribuinte.SPLei;
                    #endregion
                    #region Servicos
                    detail.ServicoFaturado.SPPrivado = Convert.ToDouble(dr["SpServPri"]);
                    detail.ServicoFaturado.SPLei = Convert.ToDouble(dr["SpServPub"]);
                    detail.ServicoFaturado.SPFatu = detail.ServicoFaturado.SPPrivado + detail.ServicoFaturado.SPLei;
                    detail.ServicoFaturado.Recebidas.SPPrivado = Convert.ToDouble(dr["SpServRecPri"]);
                    detail.ServicoFaturado.Recebidas.SPLei = Convert.ToDouble(dr["SpServRecPub"]);
                    detail.ServicoFaturado.Recebidas.SPFatu = detail.ServicoFaturado.Recebidas.SPPrivado + detail.ServicoFaturado.Recebidas.SPLei;
                    detail.ServicoFaturado.NRecebidas.SPPrivado = Convert.ToDouble(dr["SpServNRecPri"]);
                    detail.ServicoFaturado.NRecebidas.SPLei = Convert.ToDouble(dr["SpServNRecPub"]);
                    detail.ServicoFaturado.NRecebidas.SPFatu = detail.ServicoFaturado.NRecebidas.SPPrivado + detail.ServicoFaturado.NRecebidas.SPLei;
                    detail.ServicoFaturado.Cumulativo.SPPrivado = Convert.ToDouble(dr["SpServCumPri"]);
                    detail.ServicoFaturado.Cumulativo.SPLei = Convert.ToDouble(dr["SpServCumPub"]);
                    detail.ServicoFaturado.Cumulativo.SPFatu = detail.ServicoFaturado.Cumulativo.SPPrivado + detail.ServicoFaturado.Cumulativo.SPLei;
                    detail.ServicoFaturado.NCumulativo.SPPrivado = Convert.ToDouble(dr["SpServNCumPri"]);
                    detail.ServicoFaturado.NCumulativo.SPLei = Convert.ToDouble(dr["SpServNCumPub"]);
                    detail.ServicoFaturado.NCumulativo.SPFatu = detail.ServicoFaturado.NCumulativo.SPPrivado + detail.ServicoFaturado.NCumulativo.SPLei;
                    #endregion
                    #endregion
                    #region MS
                    #region Danfes
                    detail.DanfeFaturada.MSPrivado = Convert.ToDouble(dr["MsDanfePri"]);
                    detail.DanfeFaturada.MSLei = Convert.ToDouble(dr["MsDanfePub"]);
                    detail.DanfeFaturada.MSFatu = detail.DanfeFaturada.MSPrivado + detail.DanfeFaturada.MSLei;
                    detail.DanfeFaturada.Recebidas.MSPrivado = Convert.ToDouble(dr["MsDanfeRecPri"]);
                    detail.DanfeFaturada.Recebidas.MSLei = Convert.ToDouble(dr["MsDanfeRecPub"]);
                    detail.DanfeFaturada.Recebidas.MSFatu = detail.DanfeFaturada.Recebidas.MSPrivado + detail.DanfeFaturada.Recebidas.MSLei;
                    detail.DanfeFaturada.NRecebidas.MSPrivado = Convert.ToDouble(dr["MsDanfeNRecPri"]);
                    detail.DanfeFaturada.NRecebidas.MSLei = Convert.ToDouble(dr["MsDanfeNRecPub"]);
                    detail.DanfeFaturada.NRecebidas.MSFatu = detail.DanfeFaturada.NRecebidas.MSPrivado + detail.DanfeFaturada.NRecebidas.MSLei;
                    detail.DanfeFaturada.Contribuinte.MSPrivado = Convert.ToDouble(dr["MsDanfeContPri"]);
                    detail.DanfeFaturada.Contribuinte.MSLei = Convert.ToDouble(dr["MsDanfeContPub"]);
                    detail.DanfeFaturada.Contribuinte.MSFatu = detail.DanfeFaturada.Contribuinte.MSPrivado + detail.DanfeFaturada.Contribuinte.MSLei;
                    detail.DanfeFaturada.NContribuinte.MSPrivado = Convert.ToDouble(dr["MsDanfeNContPri"]);
                    detail.DanfeFaturada.NContribuinte.MSLei = Convert.ToDouble(dr["MsDanfeNContPub"]);
                    detail.DanfeFaturada.NContribuinte.MSFatu = detail.DanfeFaturada.NContribuinte.MSPrivado + detail.DanfeFaturada.NContribuinte.MSLei;
                    #endregion
                    #region Servicos
                    detail.ServicoFaturado.MSPrivado = Convert.ToDouble(dr["MsServPri"]);
                    detail.ServicoFaturado.MSLei = Convert.ToDouble(dr["MsServPub"]);
                    detail.ServicoFaturado.MSFatu = detail.ServicoFaturado.MSPrivado + detail.ServicoFaturado.MSLei;
                    detail.ServicoFaturado.Recebidas.MSPrivado = Convert.ToDouble(dr["MsServRecPri"]);
                    detail.ServicoFaturado.Recebidas.MSLei = Convert.ToDouble(dr["MsServRecPub"]);
                    detail.ServicoFaturado.Recebidas.MSFatu = detail.ServicoFaturado.Recebidas.MSPrivado + detail.ServicoFaturado.Recebidas.MSLei;
                    detail.ServicoFaturado.NRecebidas.MSPrivado = Convert.ToDouble(dr["MsServNRecPri"]);
                    detail.ServicoFaturado.NRecebidas.MSLei = Convert.ToDouble(dr["MsServNRecPub"]);
                    detail.ServicoFaturado.NRecebidas.MSFatu = detail.ServicoFaturado.NRecebidas.MSPrivado + detail.ServicoFaturado.NRecebidas.MSLei;
                    detail.ServicoFaturado.Cumulativo.MSPrivado = Convert.ToDouble(dr["MsServCumPri"]);
                    detail.ServicoFaturado.Cumulativo.MSLei = Convert.ToDouble(dr["MsServCumPub"]);
                    detail.ServicoFaturado.Cumulativo.MSFatu = detail.ServicoFaturado.Cumulativo.MSPrivado + detail.ServicoFaturado.Cumulativo.MSLei;
                    detail.ServicoFaturado.NCumulativo.MSPrivado = Convert.ToDouble(dr["MsServNCumPri"]);
                    detail.ServicoFaturado.NCumulativo.MSLei = Convert.ToDouble(dr["MsServNCumPub"]);
                    detail.ServicoFaturado.NCumulativo.MSFatu = detail.ServicoFaturado.NCumulativo.MSPrivado + detail.ServicoFaturado.NCumulativo.MSLei;
                    #endregion
                    #endregion
                }
                dr.Close();
                retorno = true;
            }
            catch (Exception ex)
            {
                retorno = false;
                erro = "ERRO: " + ex.Message;
            }
            finally
            {
                conn.Close();
            }
            return retorno;
        }

        public bool GetCalculoConsolidado(ref Consolidado consolidado, DateTime inicio, DateTime fim, out string erro)
        {
            string query = "";
            bool retorno = false;
            erro = "OK";
            SetCalculoByPeriodo(inicio, fim, out erro);
            DbDataReader dr;
            if (consolidado == null)
                consolidado = new Consolidado();
            try
            {
                conn.Open();
                #region Query Diferimentos
                query = @"declare @inicio datetime, @mes int, @ano int
                            set @inicio = @dt_inicio
                            set @mes = MONTH(@inicio)
                            set @ano = YEAR(@inicio)

                            select a.cal_bit_finalizado,b.calc_int_id,b.calc_int_mes,b.calc_int_ano
                            ,b.SpDanfeNor,b.MsDanfeNor,b.SpDanfeExc,b.MsDanfeExc,b.SpDanfeAdc,b.MsDanfeAdc
                            ,b.SpServNCumNor,b.MsServNCumNor,b.SpServNCumExc,b.MsServNCumExc,b.SpServNCumAdc,b.MsServNCumAdc
                            ,b.SpServCumNor,b.MsServCumNor,b.SpServCumExc,b.MsServCumExc,b.SpServCumAdc,b.MsServCumAdc
                            from SealCalculo a
                            inner join SealCalculoConsolidado b on a.cal_int_consolidado = b.calc_int_id 
                            where a.cal_int_mes = @mes and a.cal_int_ano = @ano";
                #endregion
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dt_inicio", inicio);

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    consolidado.finalizado = Convert.ToBoolean(dr["cal_bit_finalizado"] is DBNull ? "false" : dr["cal_bit_finalizado"]);
                    #region Danfes
                    consolidado.DanfeConsolidado.Normais.SP = Convert.ToDouble(dr["SpDanfeNor"] is DBNull ? "0" : dr["SpDanfeNor"]);
                    consolidado.DanfeConsolidado.Normais.MS = Convert.ToDouble(dr["MsDanfeNor"] is DBNull ? "0" : dr["MsDanfeNor"]);
                    consolidado.DanfeConsolidado.Normais.Tot = consolidado.DanfeConsolidado.Normais.SP + consolidado.DanfeConsolidado.Normais.MS;
                    consolidado.DanfeConsolidado.Exclusao.SP = Convert.ToDouble(dr["SpDanfeExc"] is DBNull ? "0" : dr["SpDanfeExc"]);
                    consolidado.DanfeConsolidado.Exclusao.MS = Convert.ToDouble(dr["MsDanfeExc"] is DBNull ? "0" : dr["MsDanfeExc"]);
                    consolidado.DanfeConsolidado.Exclusao.Tot = consolidado.DanfeConsolidado.Exclusao.SP + consolidado.DanfeConsolidado.Exclusao.MS;
                    consolidado.DanfeConsolidado.Adicao.SP = Convert.ToDouble(dr["SpDanfeAdc"] is DBNull ? "0" : dr["SpDanfeAdc"]);
                    consolidado.DanfeConsolidado.Adicao.MS = Convert.ToDouble(dr["MsDanfeAdc"] is DBNull ? "0" : dr["MsDanfeAdc"]);
                    consolidado.DanfeConsolidado.Adicao.Tot = consolidado.DanfeConsolidado.Adicao.SP + consolidado.DanfeConsolidado.Adicao.MS;
                    #endregion
                    #region Servicos Não Cumulativos
                    consolidado.ServicoConsuNCum.Normais.SP = Convert.ToDouble(dr["SpServNCumNor"] is DBNull ? "0" : dr["SpServNCumNor"]);
                    consolidado.ServicoConsuNCum.Normais.MS = Convert.ToDouble(dr["MsServNCumNor"] is DBNull ? "0" : dr["MsServNCumNor"]);
                    consolidado.ServicoConsuNCum.Normais.Tot = consolidado.ServicoConsuNCum.Normais.SP + consolidado.ServicoConsuNCum.Normais.MS;
                    consolidado.ServicoConsuNCum.Exclusao.SP = Convert.ToDouble(dr["SpServNCumExc"] is DBNull ? "0" : dr["SpServNCumExc"]);
                    consolidado.ServicoConsuNCum.Exclusao.MS = Convert.ToDouble(dr["MsServNCumExc"] is DBNull ? "0" : dr["MsServNCumExc"]);
                    consolidado.ServicoConsuNCum.Exclusao.Tot = consolidado.ServicoConsuNCum.Exclusao.SP + consolidado.ServicoConsuNCum.Exclusao.MS;
                    consolidado.ServicoConsuNCum.Adicao.SP = Convert.ToDouble(dr["SpServNCumAdc"] is DBNull ? "0" : dr["SpServNCumAdc"]);
                    consolidado.ServicoConsuNCum.Adicao.MS = Convert.ToDouble(dr["MsServNCumAdc"] is DBNull ? "0" : dr["MsServNCumAdc"]);
                    consolidado.ServicoConsuNCum.Adicao.Tot = consolidado.ServicoConsuNCum.Adicao.SP + consolidado.ServicoConsuNCum.Adicao.MS;
                    #endregion
                    #region Servicos Cumulativos
                    consolidado.ServicoConsuCum.Normais.SP = Convert.ToDouble(dr["SpServCumNor"] is DBNull ? "0" : dr["SpServCumNor"]);
                    consolidado.ServicoConsuCum.Normais.MS = Convert.ToDouble(dr["MsServCumNor"] is DBNull ? "0" : dr["MsServCumNor"]);
                    consolidado.ServicoConsuCum.Normais.Tot = consolidado.ServicoConsuCum.Normais.SP + consolidado.ServicoConsuCum.Normais.MS;
                    consolidado.ServicoConsuCum.Exclusao.SP = Convert.ToDouble(dr["SpServCumExc"] is DBNull ? "0" : dr["SpServCumExc"]);
                    consolidado.ServicoConsuCum.Exclusao.MS = Convert.ToDouble(dr["MsServCumExc"] is DBNull ? "0" : dr["MsServCumExc"]);
                    consolidado.ServicoConsuCum.Exclusao.Tot = consolidado.ServicoConsuCum.Exclusao.SP + consolidado.ServicoConsuCum.Exclusao.MS;
                    consolidado.ServicoConsuCum.Adicao.SP = Convert.ToDouble(dr["SpServCumAdc"] is DBNull ? "0" : dr["SpServCumAdc"]);
                    consolidado.ServicoConsuCum.Adicao.MS = Convert.ToDouble(dr["MsServCumAdc"] is DBNull ? "0" : dr["MsServCumAdc"]);
                    consolidado.ServicoConsuCum.Adicao.Tot = consolidado.ServicoConsuCum.Adicao.SP + consolidado.ServicoConsuCum.Adicao.MS;
                    #endregion
                }
                dr.Close();

                #region Query Diferimentos
                query = @"declare @inicio datetime, @mes int, @ano int
                            set @inicio = @dt_inicio
                            set @mes = MONTH(@inicio)
                            set @ano = YEAR(@inicio)

                            SELECT calr_int_id,calr_int_mes,calr_int_ano
                            ,NCumPisPri,NCumPisPub,NCumCsslPri,NCumCsslPub,NCumCofiPri,NCumCofiPub
                            ,NCumIrPri,NCumIrPub,NCumInssPri,NCumInssPub,NCumIssPri,NCumIssPub
                            ,CumPisPri,CumPisPub,CumCsslPri,CumCsslPub,CumCofiPri,CumCofiPub
                            ,CumIrPri,CumIrPub,CumInssPri,CumInssPub,CumIssPri,CumIssPub
                            FROM SealCalculoRetencao
                            where calr_int_mes = @mes and calr_int_ano = @ano";
                #endregion
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dt_inicio", inicio);

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    #region Retenções Não Cumulativas
                    consolidado.RetencaoConsuNCum.PisPrivado = Convert.ToDouble(dr["NCumPisPri"] is DBNull ? "0" : dr["NCumPisPri"]);
                    consolidado.RetencaoConsuNCum.PisPublico = Convert.ToDouble(dr["NCumPisPub"] is DBNull ? "0" : dr["NCumPisPub"]);
                    consolidado.RetencaoConsuNCum.CsslPrivado = Convert.ToDouble(dr["NCumCsslPri"] is DBNull ? "0" : dr["NCumCsslPri"]);
                    consolidado.RetencaoConsuNCum.CsslPublico = Convert.ToDouble(dr["NCumCsslPub"] is DBNull ? "0" : dr["NCumCsslPub"]);
                    consolidado.RetencaoConsuNCum.CofinsPrivado = Convert.ToDouble(dr["NCumCofiPri"] is DBNull ? "0" : dr["NCumCofiPri"]);
                    consolidado.RetencaoConsuNCum.CofinsPublico = Convert.ToDouble(dr["NCumCofiPub"] is DBNull ? "0" : dr["NCumCofiPub"]);
                    consolidado.RetencaoConsuNCum.IrPrivado = Convert.ToDouble(dr["NCumIrPri"] is DBNull ? "0" : dr["NCumIrPri"]);
                    consolidado.RetencaoConsuNCum.IrPublico = Convert.ToDouble(dr["NCumIrPub"] is DBNull ? "0" : dr["NCumIrPub"]);
                    consolidado.RetencaoConsuNCum.InssPrivado = Convert.ToDouble(dr["NCumInssPri"] is DBNull ? "0" : dr["NCumInssPri"]);
                    consolidado.RetencaoConsuNCum.InssPublico = Convert.ToDouble(dr["NCumInssPub"] is DBNull ? "0" : dr["NCumInssPub"]);
                    consolidado.RetencaoConsuNCum.IssPrivado = Convert.ToDouble(dr["NCumIssPri"] is DBNull ? "0" : dr["NCumIssPri"]);
                    consolidado.RetencaoConsuNCum.IssPublico = Convert.ToDouble(dr["NCumIssPub"] is DBNull ? "0" : dr["NCumIssPub"]);
                    #endregion
                    #region Retenções Cumulativas
                    consolidado.RetencaoConsuCum.PisPrivado = Convert.ToDouble(dr["CumPisPri"] is DBNull ? "0" : dr["CumPisPri"]);
                    consolidado.RetencaoConsuCum.PisPublico = Convert.ToDouble(dr["CumPisPub"] is DBNull ? "0" : dr["CumPisPub"]);
                    consolidado.RetencaoConsuCum.CsslPrivado = Convert.ToDouble(dr["CumCsslPri"] is DBNull ? "0" : dr["CumCsslPri"]);
                    consolidado.RetencaoConsuCum.CsslPublico = Convert.ToDouble(dr["CumCsslPub"] is DBNull ? "0" : dr["CumCsslPub"]);
                    consolidado.RetencaoConsuCum.CofinsPrivado = Convert.ToDouble(dr["CumCofiPri"] is DBNull ? "0" : dr["CumCofiPri"]);
                    consolidado.RetencaoConsuCum.CofinsPublico = Convert.ToDouble(dr["CumCofiPub"] is DBNull ? "0" : dr["CumCofiPub"]);
                    consolidado.RetencaoConsuCum.IrPrivado = Convert.ToDouble(dr["CumIrPri"] is DBNull ? "0" : dr["CumIrPri"]);
                    consolidado.RetencaoConsuCum.IrPublico = Convert.ToDouble(dr["CumIrPub"] is DBNull ? "0" : dr["CumIrPub"]);
                    consolidado.RetencaoConsuCum.InssPrivado = Convert.ToDouble(dr["CumInssPri"] is DBNull ? "0" : dr["CumInssPri"]);
                    consolidado.RetencaoConsuCum.InssPublico = Convert.ToDouble(dr["CumInssPub"] is DBNull ? "0" : dr["CumInssPub"]);
                    consolidado.RetencaoConsuCum.IssPrivado = Convert.ToDouble(dr["CumIssPri"] is DBNull ? "0" : dr["CumIssPri"]);
                    consolidado.RetencaoConsuCum.IssPublico = Convert.ToDouble(dr["CumIssPub"] is DBNull ? "0" : dr["CumIssPub"]);
                    #endregion
                }
                dr.Close();
                retorno = true;
            }
            catch (Exception ex)
            {
                retorno = false;
                erro = "ERRO: " + ex.Message;
            }
            finally
            {
                conn.Close();
            }
            return retorno;
        }
    }
}
