use assecont2;

select * from SealCliente
select * from SealDocumento
select * from SealDocumentoDados
select * from SealContaReceber
select * from SealEmailConfig

--	CONFIG EMAIL BEGIN --
insert into SealEmailConfig (cfg_des_host, cfg_int_port, cfg_bit_ssl, cfg_des_from, cfg_des_user, cfg_des_pass, cfg_dt_data, cfg_bit_ativo)
values ('smtplw.com.br', 587, 1, 'notificacao@assecont.com.br', 'assecont', 'BwpGCkuA7951', GETDATE(), 1)
declare @config int = (select IDENT_CURRENT('SealEmailConfig'))
insert into SealEmailConfigSend (sed_des_email, sed_des_nome) values ('henrique@assecont.com.br', 'Henrique Chamizo')
declare @send int = (select IDENT_CURRENT('SealEmailConfigSend'))
insert into SealEmailConfig_CongigSend (sed_int_id, cfg_int_id) values (@send, @config)
-- CONFIG EMAIL END --

-- UPDATE DOCUMENTO ERROR BEGIN -- 
update SealDocumento 
set doc_bit_erro = 1 
where doc_int_id = @id
-- UPDATE DOCUMENTO ERROR END --

-- UPDATE DOCUMENTODADOS APROVA BEGIN -- 
update SealDocumentoDados 
set dcd_int_aprovacao = @aprova
where dcd_int_id = @id
-- UPDATE DOCUMENTODADOS APROVA END -- 

-- INSERT DOCUMENTODADOS BEGIN -- 
insert into SealDocumentoDados (dcd_int_id, dcd_num_privadocontribuinte, dcd_num_privadoncontribuinte, dcd_num_publicocontribuinte, dcd_num_publiconcontribuinte, doc_int_id)
values (@dcd_int_id, @dcd_num_privadocontribuinte, @dcd_num_privadoncontribuinte, @dcd_num_publicocontribuinte, @dcd_num_publiconcontribuinte, @doc_int_id)
-- INSERT DOCUMENTODADOS END --

-- SELECT DOCUMENTODADOS BEGIN -- 
select dcd_int_id, dcd_num_privadocontribuinte, dcd_num_privadoncontribuinte, dcd_num_publicocontribuinte, dcd_num_publiconcontribuinte, doc_int_id 
from SealDocumentoDados
-- SELECT DOCUMENTODADOS END -- 

--INSERT CLIENTES BEGIN
--UPDATE CLIENTES BEGIN
declare @codigo_0 varchar(6) = @cli_des_codigo, 
		@nome varchar(150) = @cli_des_nome, 
		@tipo varchar(50) = @cli_des_tipo, 
		@grupo varchar(50) = @cli_des_grupo 

if exists(select cli_int_id from SealCliente where cli_des_codigo = @codigo_0)
begin 
	update SealCliente 
	set cli_des_nome = @nome, 
		cli_des_tipo = @tipo, 
		cli_des_grupo = @grupo 
	where cli_des_codigo = @codigo_0
end else 
begin
	insert into SealCliente (cli_des_codigo, cli_des_nome, cli_des_tipo, cli_des_grupo) 
	values (@codigo_0, @nome, @tipo, @grupo)
end
--INSERT CLIENTES END
--UPDATE CLIENTES END

--INSERT CONTARECEBER BEGIN
declare @codigo varchar(6) = @cli_des_codigo, 
		@doc int = @doc_int_id, 
		@cnpjorigem varchar(14) = @crb_des_cnpjorigem, 
		@docorigem int = @crb_int_docorigem, 
		@notafiscal int = @crb_int_notafiscal, 
		@prest varchar(10) = @crb_des_prest, 
		@emissao datetime = @crb_dt_emissao, 
		@vencimento datetime = @crb_dt_vencimento, 
		@valor numeric(18, 6) = @crb_num_valor, 
		@desconto numeric(18, 6) = @crb_num_desconto, 
		@juros numeric(18, 6) = @crb_num_juros, 
		@valorrecebido numeric(18, 6) = @crb_num_valorrecebido, 
		@recebimento datetime = @crb_dt_recebimento, 
		@forma varchar(20) = @crb_des_forma, 
		@observacao varchar(250) = @crb_des_observacao 

insert into SealContaReceber (crb_des_codigo, crb_des_cnpjorigem, crb_int_docorigem, crb_int_notafiscal, crb_des_prest, crb_dt_emissao, 
								crb_dt_vencimento, crb_num_valor, crb_num_desconto, crb_num_juros, crb_num_valorrecebido, crb_dt_recebimento, 
								crb_des_forma, crb_des_observacao, cli_int_id, doc_int_id, nfr_int_id) values 
(@codigo, @cnpjorigem, @docorigem, @notafiscal, @prest, @emissao, @vencimento, @valor, @desconto, @juros, @valorrecebido, @recebimento, @forma, @observacao, 
 (case 
	when not exists(select cli_int_id from SealCliente where cli_des_codigo = @codigo) 
	then null 
	else (select cli_int_id from SealCliente where cli_des_codigo = @codigo) 
 end), 
 (case 
	when not exists(select doc_int_id from SealDocumento where doc_int_id = @doc) 
	then null 
	else @doc 
 end), 
 (case 
	when not exists(select a.nfr_int_id 
					from SealNotaFiscalRecebida a 
					left join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id 
					left join SealNotaFiscalServico c on a.nfs_int_id = c.nfs_int_id 
					where b.nfd_int_nNf = @notafiscal or c.nfs_int_numero = @notafiscal) 
	then null 
	else (select a.nfr_int_id 
		  from SealNotaFiscalRecebida a 
		  left join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id 
		  left join SealNotaFiscalServico c on a.nfs_int_id = c.nfs_int_id 
		  where b.nfd_int_nNf = @notafiscal or c.nfs_int_numero = @notafiscal) 
 end)
)
--INSERT CONTARECEBER END

--INSERT NOTAFISCALDANFEEMPRESA EMIT BEGIN
declare @nde_int_emit int,
		@nde_des_cnpj_emit varchar(14) = @emit_cnpj,
		@nde_des_nome_emit varchar(255) = @emit_nome, 
		@nde_des_fant_emit varchar(255) = @emit_fant, 
		@nde_des_lgr_emit varchar(255) = @emit_lgr, 
		@nde_des_nro_emit varchar(20) = @emit_nro,
		@nde_des_cpl_emit varchar(255) = @emit_cpl,
		@nde_des_bairro_emit varchar(100) = @emit_bairro,
		@nde_int_cMun_emit int = @emit_cMun,
		@nde_des_mun_emit varchar(100) = @emit_mun,
		@nde_des_uf_emit varchar(2) = @emit_uf,
		@nde_des_cep_emit varchar(8) = @emit_cep,
		@nde_int_cPais_emit int = @emit_cPais,
		@nde_des_pais_emit varchar(100) = @emit_pais,
		@nde_des_fone_emit varchar(10) = @emit_fone,
		@nde_int_ie_emit int = @emit_ie,
		@nde_int_crt_emit int = @emit_crt,
		@nde_des_email_emit varchar(255) = @emit_email

insert into SealNotaFiscalDanfeEmpresa
(nde_des_cnpj,nde_des_nome,nde_des_fant,nde_des_lgr,nde_des_nro,nde_des_cpl,nde_des_bairro,nde_int_cMun,nde_des_mun,nde_des_uf,nde_des_cep,nde_int_cPais,
nde_des_pais,nde_des_fone,nde_int_ie,nde_int_crt,nde_int_indIEDest,nde_des_email)
values (@nde_des_cnpj_emit,@nde_des_nome_emit,@nde_des_fant_emit,@nde_des_lgr_emit,@nde_des_nro_emit,@nde_des_cpl_emit,@nde_des_bairro_emit,@nde_int_cMun_emit,
@nde_des_mun_emit,@nde_des_uf_emit,@nde_des_cep_emit,@nde_int_cPais_emit,@nde_des_pais_emit,@nde_des_fone_emit,@nde_int_ie_emit,@nde_int_crt_emit,
@nde_des_email_emit)
set @nde_int_emit = (select SCOPE_IDENTITY('SealNotaFiscalDanfeEmpresa'))
select @nde_int_emit
--INSERT NOTAFISCALDANFEEMPRESA EMIT END

--INSERT NOTAFISCALDANFEEMPRESA DEST BEGIN
declare @nde_int_dest int, 
		@nde_des_cnpj_dest varchar(14) = @dest_cnpj,
		@nde_des_nome_dest varchar(255) = @dest_nome, 
		@nde_des_fant_dest varchar(255) = @dest_fant, 
		@nde_des_lgr_dest varchar(255) = @dest_lgr,
		@nde_des_nro_dest varchar(20) = @dest_nro,
		@nde_des_cpl_dest varchar(255) = @dest_cpl,
		@nde_des_bairro_dest varchar(100) = @dest_bairro,
		@nde_int_cMun_dest int = @dest_cMun,
		@nde_des_mun_dest varchar(100) = @dest_mun,
		@nde_des_uf_dest varchar(2) = @dest_uf,
		@nde_des_cep_dest varchar(8) = @dest_cep,
		@nde_int_cPais_dest int = @dest_cPais,
		@nde_des_pais_dest varchar(100) = @dest_pais,
		@nde_des_fone_dest varchar(10) = @dest_fone,
		@nde_int_ie_dest int = @dest_ie,
		@nde_int_indIEDest_dest int = @dest_indIEDest,
		@nde_des_email_dest varchar(255) = @dest_email

insert into SealNotaFiscalDanfeEmpresa
(nde_des_cnpj,nde_des_nome,nde_des_fant,nde_des_lgr,nde_des_nro,nde_des_cpl,nde_des_bairro,nde_int_cMun,nde_des_mun,nde_des_uf,nde_des_cep,nde_int_cPais,
nde_des_pais,nde_des_fone,nde_int_ie,nde_int_crt,nde_int_indIEDest,nde_des_email)
values (@nde_des_cnpj_dest,@nde_des_nome_dest,@nde_des_fant_dest,@nde_des_lgr_dest,@nde_des_nro_dest,@nde_des_cpl_dest,@nde_des_bairro_dest,@nde_int_cMun_dest,
@nde_des_mun_dest,@nde_des_uf_dest,@nde_des_cep_dest,@nde_int_cPais_dest,@nde_des_pais_dest,@nde_des_fone_dest,@nde_int_ie_dest,@nde_int_indIEDest_dest,
@nde_des_email_dest)
set @nde_int_dest = (select SCOPE_IDENTITY('SealNotaFiscalDanfeEmpresa'))
select @nde_int_dest
--INSERT NOTAFISCALDANFEEMPRESA DEST END

--INSERT NOTAFISCALDANFE BEGIN
declare @nfd_int_nUF int = @nUF,
		@nfd_int_cNF int = @cNF,
		@nfd_des_natPo varchar(255) = @natPo,
		@nfd_int_indOag int = @indOag,
		@nfd_int_mod int = @mod,
		@nfd_int_serie int = @serie,
		@nfd_int_nNf int = @nNf,
		@nfd_dt_dhEmi datetime = @dhEmi,
		@nfd_int_tpNf int = @tpNf,
		@nfd_int_idDest int = @idDest,
		@nfd_int_cMunicFG int = @cMunicFG,
		@nfd_int_tpImp int = @tpImp,
		@nfd_int_tpEmis int = @tpEmis,
		@nfd_int_cDV int = @cDV,
		@nfd_int_tpAmb int = @tpAmb,
		@nfd_int_finNfe int = @finNfe,
		@nfd_int_indFinal int = @indFinal,
		@nfd_int_indPres int = @indPres,
		@nfd_int_procEmi int = @procEmi,
		@nfd_des_verProc varchar(255) = @verProc, 
		@nfd_int_id int 

insert into SealNotaFiscalDanfe
(nfd_int_nUF,nfd_int_cNF,nfd_des_natPo,nfd_int_indOag,nfd_int_mod,nfd_int_serie,nfd_int_nNf,nfd_dt_dhEmi,nfd_int_tpNf,
nfd_int_idDest,nfd_int_cMunicFG,nfd_int_tpImp,nfd_int_tpEmis,nfd_int_cDV,nfd_int_tpAmb,nfd_int_finNfe,nfd_int_indFinal,nfd_int_indPres,nfd_int_procEmi,
nfd_des_verProc,nde_int_emit,nde_int_dest)
values (@nfd_int_nUF,@nfd_int_cNF,@nfd_des_natPo,@nfd_int_indOag,@nfd_int_mod,@nfd_int_serie,@nfd_int_nNf,@nfd_dt_dhEmi,@nfd_int_tpNf,@nfd_int_idDest,
@nfd_int_cMunicFG,@nfd_int_tpImp,@nfd_int_tpEmis,@nfd_int_cDV,@nfd_int_tpAmb,@nfd_int_finNfe,@nfd_int_indFinal,@nfd_int_indPres,@nfd_int_procEmi,
@nfd_des_verProc,@nde_int_emit,@nde_int_dest)
set @nfd_int_id = (select SCOPE_IDENTITY('SealNotaFiscalDanfe'))
--INSERT NOTAFISCALDANFE END

--INSERT NOTAFISCALDANFEITEM BEGIN
declare @ndi_int_num int = @num,
		@ndi_des_cProd varchar(100) = @cProd,
		@ndi_des_prod varchar(100) = @prod,
		@ndi_int_ncm int = @ncm,
		@ndi_int_cfop int = @cfop,
		@ndi_des_uCom varchar(2) = @uCom,
		@ndi_num_qCom numeric(14,4) = @qCom,
		@ndi_num_vUnCom numeric(22,10) = @nUnCom,
		@ndi_num_vProd numeric(16,2) = @vProd,
		@ndi_des_uTrib varchar(2) = @uTrib,
		@ndi_num_qTrib numeric(14,4) = @qTrib,
		@ndi_num_vUnTrib numeric(22,10) = @vUnTrib,
		@ndi_int_indTot int = @indTot,
		@ndi_num_vTotTrib numeric(16,2) = @vTotTrib,
		@ndi_int_icmsOrig int = @imcsOrig,
		@ndi_des_icmsCst varchar(2) = @icmsCst,
		@ndi_int_icmsModBc int = @icmsModBc,
		@ndi_num_icmsVBC numeric(16,2) = @icmsVBC,
		@ndi_num_pICMS numeric(16,2) = @pICMS,
		@ndi_num_vICMS numeric(16,2) = @cICMS,
		@ndi_int_ipiCEnq int = @ipiCEnq,
		@ndi_des_ipiCst varchar(2) = @ipiCst,
		@ndi_des_pisCst varchar(2) = @pisCst,
		@ndi_num_pisVBC numeric(16,2) = @pisVBC,
		@ndi_num_pPIS numeric(16,2) = @pPIS,
		@ndi_num_vPIS numeric(16,2) = @vPIS,
		@ndi_des_cofinsCst varchar(2) = @cofinsCst,
		@ndi_num_cofinsVBC numeric(16,2) = @confinsVBC,
		@ndi_num_pCofins numeric(16,2) = @pCofins,
		@ndi_num_vCofins numeric(16,2) = @vCofins

insert into SealNotaFiscalDanfeItem
(ndi_int_num,ndi_des_cProd,ndi_des_prod,ndi_int_ncm,ndi_int_cfop,ndi_des_uCom,ndi_num_qCom,ndi_num_vUnCom,ndi_num_vProd,ndi_des_uTrib,ndi_num_qTrib,ndi_num_vUnTrib
,ndi_int_indTot,ndi_num_vTotTrib,ndi_int_icmsOrig,ndi_des_icmsCst,ndi_int_icmsModBc,ndi_num_icmsVBC,ndi_num_pICMS,ndi_num_vICMS,ndi_int_ipiCEnq,ndi_des_ipiCst
,ndi_des_pisCst,ndi_num_pisVBC,ndi_num_pPIS,ndi_num_vPIS,ndi_des_cofinsCst,ndi_num_cofinsVBC,ndi_num_pCofins,ndi_num_vCofins,nfd_int_id)
values
(@ndi_int_num,@ndi_des_cProd,@ndi_des_prod,@ndi_int_ncm,@ndi_int_cfop,@ndi_des_uCom,@ndi_num_qCom,@ndi_num_vUnCom,@ndi_num_vProd,@ndi_des_uTrib,@ndi_num_qTrib,
@ndi_num_vUnTrib,@ndi_int_indTot,@ndi_num_vTotTrib,@ndi_int_icmsOrig,@ndi_des_icmsCst,@ndi_int_icmsModBc,@ndi_num_icmsVBC,@ndi_num_pICMS,@ndi_num_vICMS,
@ndi_int_ipiCEnq,@ndi_des_ipiCst,@ndi_des_pisCst,@ndi_num_pisVBC,@ndi_num_pPIS,@ndi_num_vPIS,@ndi_des_cofinsCst,@ndi_num_cofinsVBC,@ndi_num_pCofins,
@ndi_num_vCofins,@nfd_int_id)
--INSERT NOTAFISCALDANFEITEM END