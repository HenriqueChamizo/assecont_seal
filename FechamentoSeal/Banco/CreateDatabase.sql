use assecont2;

create table SealDocumento(
doc_int_id int identity, 
doc_des_descricao varchar(255) not null, 
doc_des_nameclientes varchar(255) null, 
doc_file_clientes varbinary(max) null,
doc_des_namecontasreceber varchar(255) null,
doc_file_contasreceber varbinary(max) null,
doc_dt_data datetime not null, 
doc_bit_erro bit not null,
constraint Pk_SealDocumento primary key (doc_int_id)
);

create table SealDocumentoDados(
dcd_int_id int identity, 
dcd_num_privadocontribuinte numeric(18, 6) not null, 
dcd_num_privadoncontribuinte numeric(18, 6) not null, 
dcd_num_publicocontribuinte numeric(18, 6) not null, 
dcd_num_publiconcontribuinte numeric(18, 6) not null, 
dcd_int_aprovacao int not null,
doc_int_id int,
constraint Pk_SealDocumentoDados primary key (dcd_int_id), 
constraint Fk_SealDocumentoDados_SealDocumento foreign key (doc_int_id) references SealDocumento (doc_int_id)
);

alter table SealDocumentoDados
add constraint DF_SealDocumentoDados_PrivadoContribuinte default (0) for [dcd_num_privadocontribuinte]
alter table SealDocumentoDados
add constraint DF_SealDocumentoDados_PrivadoNContribuinte default (0) for [dcd_num_privadoncontribuinte]
alter table SealDocumentoDados
add constraint DF_SealDocumentoDados_PublicoContribuinte default (0) for [dcd_num_publicocontribuinte]
alter table SealDocumentoDados
add constraint DF_SealDocumentoDados_PublicoNContribuinte default (0) for [dcd_num_publiconcontribuinte]
alter table SealDocumentoDados
add constraint DF_SealDocumentoDados_Aprovacao default (0) for [dcd_int_aprovacao]

alter table SealDocumento
add constraint DF_SealDocumento_Descricao default ('Referente a ' + CONVERT(varchar(255), CONVERT(date, GETDATE()), 101)) for [doc_des_descricao]
alter table SealDocumento
add constraint DF_SealDocumento_Erro default (0) for [doc_bit_erro]

create table SealCliente(
cli_int_id int identity,
cli_des_codigo varchar(6) not null, 
cli_des_nome varchar(150) null, 
cli_des_tipo varchar(50) not null, 
cli_des_grupo varchar(50) not null, 
cli_des_cnpj varchar(14) null, 
cli_bit_atual bit not null, 
constraint Pk_SealCliente primary key (cli_int_id)
);

create table SealNotaFiscalServicoEmpresa(
nse_int_id int identity, 
nse_des_cnpj varchar(14) null, 
nse_des_razaosocial varchar(200) null, 
nse_des_rua varchar(200) null, 
nse_des_numero varchar(10) null, 
nse_des_complemento varchar(100) null,
nse_des_bairro varchar(100) null,
nse_des_cidade varchar(100) null, 
nse_des_estado varchar(2) null, 
nse_des_cep varchar(8) null, 
nse_des_telefone varchar(11) null, 
nse_des_email varchar(200) null, 
constraint Pk_NotaFiscalServicoTomador primary key (nse_int_id)
);

create table SealNotaFiscalServico(
nfs_int_id int identity,
nfs_int_numero int not null, 
nfs_des_codverificacao varchar(20) not null, 
nfs_dt_emissao datetime not null, 
nfs_int_naturezaop int not null, 
nfs_int_regimetributacao int not null, 
nfs_int_optantesimples int not null, 
nfs_int_icentivcult int not null, 
nfs_int_competencia int not null, 
nfs_int_nfsesubst int not null, 
nfs_des_outrasinformacoes varchar(max) not null,
nfs_int_listaservico int not null, 
nfs_int_cnae int not null, 
nfs_int_tribmunici int not null, 
nfs_des_dicriminacao varchar(max) not null, 
nfs_int_municiprestserv int not null, 
nfs_num_vlrservico numeric(18,6) not null, 
nfs_num_vlrdeducoes numeric(18,6) not null, 
nfs_num_vlrpis numeric(18,6) not null, 
nfs_num_vlrcofins numeric(18,6) not null, 
nfs_num_vlrinss numeric(18,6) not null, 
nfs_num_vlrir numeric(18,6) not null, 
nfs_num_vlrcsll numeric(18,6) not null, 
nfs_num_issretido numeric(18,6) not null, 
nfs_num_outrasretencoes numeric(18,6) not null, 
nfs_num_basecalculo numeric(18,6) not null, 
nfs_num_aliquota numeric(18,6) not null, 
nfs_num_vlrliquidonfe numeric(18,6) not null, 
nfs_num_vlrissretido numeric(18,6) not null,  
nfs_num_vlriss numeric(18,6) not null,  
nfs_num_desccondicionado numeric(18,6) not null,  
nfs_num_descincondicionado numeric(18,6) not null, 
nfs_int_municipio int not null, 
nfs_des_ufmunicipio varchar(2) not null,
nse_int_prestador int null,
nse_int_tomador int null, 
constraint Pk_NotaFiscalServico primary key (nfs_int_id), 
constraint Fk_NotaFiscalServico_Empresa_Prestador foreign key (nse_int_prestador) references SealNotaFiscalServicoEmpresa (nse_int_id), 
constraint Fk_NotaFiscalServico_Empresa_Tomador foreign key (nse_int_tomador) references SealNotaFiscalServicoEmpresa (nse_int_id)
);

create table SealNotaFiscalDanfeEmpresa(
nde_int_id int identity, 
nde_des_cnpj varchar(14) not null, 
nde_des_nome varchar(255) not null, 
nde_des_fant varchar(255) null, 
nde_des_lgr varchar(255) null,
nde_des_nro varchar(255) null, 
nde_des_cpl varchar(255) null, 
nde_des_bairro varchar(100) null, 
nde_int_cMun int null, 
nde_des_mun varchar(100) null, 
nde_des_uf varchar(2) null, 
nde_des_cep varchar(8) null, 
nde_int_cPais int null, 
nde_des_pais varchar(100) null,
nde_des_fone varchar(10) null, 
nde_int_ie int null, 
nde_int_crt int null,
nde_int_indIEDest int null,
nde_des_email varchar(255) null,
constraint Pk_SealNotaFiscalDanfeEmpresa primary key (nde_int_id)
);

create table SealNotaFiscalDanfe(
nfd_int_id int identity, 
nfd_int_nUF int null, 
nfd_int_cNF int null, 
nfd_des_natPo varchar(255) null, 
nfd_int_indOag int null, 
nfd_int_mod int null, 
nfd_int_serie int null, 
nfd_int_nNf int null, 
nfd_dt_dhEmi datetime null, 
nfd_int_tpNf int null, 
nfd_int_idDest int null, 
nfd_int_cMunicFG int null, 
nfd_int_tpImp int null,
nfd_int_tpEmis int null, 
nfd_int_cDV int null, 
nfd_int_tpAmb int null, 
nfd_int_finNfe int null, 
nfd_int_indFinal int null, 
nfd_int_indPres int null, 
nfd_int_procEmi int null, 
nfd_des_verProc varchar(255) null, 
nfd_des_chave varchar(100) null,
nde_int_emit int null, 
nde_int_dest int null, 
nfd_des_xml varchar(max) null, 
constraint Pk_SealNotaFiscalDanfe primary key (nfd_int_id), 
constraint Fk_SealNotaFiscalDanfe_SealNotaFiscalDanfeEmpresa_Emit foreign key (nde_int_emit) references SealNotaFiscalDanfeEmpresa (nde_int_id), 
constraint Fk_SealNotaFiscalDanfe_SealNotaFiscalDanfeEmpresa_Dest foreign key (nde_int_dest) references SealNotaFiscalDanfeEmpresa (nde_int_id)
);

create table SealNotaFiscalDanfeItem(
ndi_int_id int identity, 
ndi_int_num int, 
ndi_des_cProd varchar(100) not null, 
ndi_des_prod varchar(100) not null, 
ndi_int_ncm int null, 
ndi_int_cfop int null, 
ndi_des_uCom varchar(2) null, 
ndi_num_qCom numeric(14, 4) null, 
ndi_num_vUnCom numeric(22, 10) null,
ndi_num_vProd numeric(16, 2) null, 
ndi_num_vFrete numeric(16, 2) null, 
ndi_des_uTrib varchar(2) null, 
ndi_num_qTrib numeric(14, 4) null, 
ndi_num_vUnTrib numeric(22, 10) null, 
ndi_int_indTot int null, 
ndi_num_vTotTrib numeric(16, 2) null, 
ndi_int_icmsOrig int null, 
ndi_des_icmsCst varchar(2) null, 
ndi_int_icmsModBc int null, 
ndi_num_icmsVBC numeric(16, 2) null, 
ndi_num_pICMS numeric(16, 2) null, 
ndi_num_vICMS numeric(16, 2) null,
ndi_int_ipiCEnq int null, 
ndi_des_ipiCst varchar(2) null, 
ndi_des_pisCst varchar(2) null, 
ndi_num_pisVBC numeric(16, 2) null, 
ndi_num_pPIS numeric(16, 2) null, 
ndi_num_vPIS numeric(16, 2) null, 
ndi_des_cofinsCst varchar(2), 
ndi_num_cofinsVBC numeric(16, 2) null, 
ndi_num_pCofins numeric(16, 2) null, 
ndi_num_vCofins numeric(16, 2) null, 
nfd_int_id int not null,
constraint Pk_SealNotaFiscalDanfeItem primary key (ndi_int_id),
constraint Fk_SealNotaFiscalDanfeItem_SealNotaFiscalDanfe foreign key (nfd_int_id) references SealNotaFiscalDanfe (nfd_int_id)
);

--alter table SealNotaFiscalDanfeItem 
--add ndi_num_vFrete numeric(16, 2) null 

create table SealNotaFiscalRecebida(
nfr_int_id int identity, 
nfd_int_id int, 
nfs_int_id int, 
constraint Pk_SealNotaFiscalRecebida primary key (nfr_int_id), 
constraint Fk_SealNotaFiscalRecebida_SealNotaFiscalDanfe foreign key (nfd_int_id) references SealNotaFiscalDanfe (nfd_int_id), 
constraint Fk_SealNotaFiscalRecebida_SealNotaFiscalServico foreign key (nfs_int_id) references SealNotaFiscalServico (nfs_int_id)
);

create table SealContaReceber(
crb_int_id int identity, 
crb_des_codigo varchar(6) not null, 
crb_des_cnpjorigem varchar(14) not null, 
crb_int_docorigem int not null, 
crb_int_notafiscal int not null, 
crb_des_prest varchar(10) null, 
crb_dt_emissao datetime not null, 
crb_dt_vencimento datetime not null,
crb_num_valor numeric(18, 6) not null, 
crb_num_desconto numeric(18, 6) not null, 
crb_num_juros numeric(18, 6) not null, 
crb_num_valorrecebido numeric(18, 6) not null, 
crb_dt_recebimento datetime not null, 
crb_des_forma varchar(20) null, 
crb_des_observacao varchar(250) not null, 
cli_int_id int null, 
doc_int_id int null,
nfr_int_id int null,
constraint Pk_SealContaReceber primary key (crb_int_id), 
constraint Fk_SealContaReceber_SealCliente foreign key (cli_int_id) references SealCliente (cli_int_id), 
constraint Fk_SealContaReceber_SealDocumento foreign key (doc_int_id) references SealDocumento (doc_int_id), 
constraint Fk_SealContaReceber_SealNotaFiscalRecebida foreign key (nfr_int_id) references SealNotaFiscalRecebida (nfr_int_id)
);

create table SealEmailConfig(
cfg_int_id int identity, 
cfg_des_host varchar(255) not null, 
cfg_int_port int not null, 
cfg_bit_ssl bit not null, 
cfg_des_from varchar(255) not null, 
cfg_des_user varchar(255) null, 
cfg_des_pass varchar(255) null, 
cfg_dt_data datetime not null,
cfg_bit_ativo bit null,
constraint Pk_SealEmailConfig primary key (cfg_int_id)
);

create table SealEmailConfigSend(
sed_int_id int identity,
sed_des_email varchar(255) NOT NULL,
sed_des_nome varchar(255) NULL,
constraint Pk_SealEmailConfigSend primary key (sed_int_id),
constraint UN_SealEmailConfigSend_Email unique (sed_des_email)
);

create table SealEmailConfig_CongigSend(
ecs_int_id int identity,
sed_int_id int NULL,
cfg_int_id int NULL,
constraint Fk_SealEmailConfig_CongigSend_SealEmailConfig foreign key (cfg_int_id) references SealEmailConfig (cfg_int_id),
constraint Fk_SealEmailConfig_CongigSend_SealEmailConfigSend foreign key (sed_int_id) references SealEmailConfigSend (sed_int_id)
);

--	CONFIG EMAIL BEGIN --
insert into SealEmailConfig (cfg_des_host, cfg_int_port, cfg_bit_ssl, cfg_des_from, cfg_des_user, cfg_des_pass, cfg_dt_data, cfg_bit_ativo)
values ('smtplw.com.br', 587, 1, 'notificacao@assecont.com.br', 'assecont', 'BwpGCkuA7951', GETDATE(), 1)
declare @config int = (select IDENT_CURRENT('SealEmailConfig'))
insert into SealEmailConfigSend (sed_des_email, sed_des_nome) values ('henrique@assecont.com.br', 'Henrique Chamizo')
declare @send int = (select IDENT_CURRENT('SealEmailConfigSend'))
insert into SealEmailConfig_CongigSend (sed_int_id, cfg_int_id) values (@send, @config)
-- CONFIG EMAIL END --

--  CREATE INSERTS CLIENTS BEGIN --
insert into SealCliente (cli_des_codigo, cli_des_nome, cli_des_tipo, cli_des_grupo, cli_des_cnpj)
values ('C00365', 'GERIS ENGENHARIA E SERVICOS LTDA', 'Cliente', 'CONTRIBUINTE', '03062917000109')
,('C00648', 'FUNDACAO UNIVERSIDADE FED.  MATO GROSSO', 'Cliente', 'Ñ CONTRIB - Lei 9718', '33004540000100')
,('C05533', 'SERVICO SOCIAL DA INDUSTRIA SESI', 'Cliente', 'NÃO CONTRIBUINTE', '03775655000120')
,('C05536', 'DEPARTAMENTO DE AGUA E ESGOTOS DE SAO CAETANO DO SUL', 'Cliente', 'Ñ CONTRIB - Lei 9718', '59330936000123')
,('C05537', 'GOVERNO DO PARANA - CASA MILITAR', 'Cliente', 'Ñ CONTRIB - Lei 9718', '14788457000117')
,('C05541', 'CONSELHO ADMINISTRATIVO DE DEFESA ECONOMICA-CADE', 'Cliente', 'Ñ CONTRIB - Lei 9718', '00418993000116')
,('C05591', 'SUBSECRETARIA DE ASSUNTOS ADMINISTRATIVOS DO MINISTERIO DO DESENVOLVIMENTO SOCIAL E AGRARIO', 'Cliente', 'Ñ CONTRIB - Lei 9718', '05756246000101')
,('C05593', 'TVSBT CANAL 5 DE PORTO ALEGRE S/A', 'Cliente', 'CONTRIBUINTE', '54313556000248')
,('C05597', 'PROCURADORIA GERAL DE JUSTICA DO ESTADO DE MATO GROSSO', 'Cliente', 'Ñ CONTRIB - Lei 9718', '14921092000157')
,('C05600', 'CENTRO DE AVALIACAO DO EXERCITO', 'Cliente', 'Ñ CONTRIB - Lei 9718', '09687482000174')
,('C05610', 'CONDOMINIO DO EDIFICIO GENERAL ALENCASTRO', 'Cliente', 'NÃO CONTRIBUINTE', '03391595000141')
,('C05649', 'IMAGEM E PROJECAO COMERCIO LTDA - EPP', 'Cliente', 'CONTRIBUINTE', '07135006000115')

,('C05312', 'FD DO BRASIL SOLUCOES DE PAGAMENTO LTDA', 'Cliente', 'NÃO CONTRIBUINTE', '04962772000165')
,('C05540', 'GO2NEXT CYNET TELEINFORMATICA LTDA - EPP', 'Cliente', 'CONTRIBUINTE', '07474492000104')
,('C05581', 'EMS S/A', 'Cliente', 'CONTRIBUINTE', '57507378000365')
,('C05542', 'BRASIL TELECOM COMUNICACAO MULTIMIDIA LTDA.', 'Cliente', 'CONTRIBUINTE', '02041460001670')
,('C05611', 'WARBURG PINCUS DO BRASIL LTDA.', 'Cliente', 'NÃO CONTRIBUINTE', '11263972000195')
,('C01502', 'RUNGE SERVICOS DE CONSULTORIA DO BRASIL', 'Cliente', 'NÃO CONTRIBUINTE', '07776080000110')
,('C05641', 'REVIVER ADMINISTRACAO PRISIONAL PRIVADA LTDA', 'Cliente', 'NÃO CONTRIBUINTE', '05146393000402')
,('C05607', 'SHURE LATIN AMERICA SERVICOS DE ANALISE DE MERCADO, CONSULTORIA E PROMOCAO DE VENDAS LTDA.', 'Cliente', 'NÃO CONTRIBUINTE', '24071132000172')
,('C05486', 'SAMSUNG ELETRONICA DA AMAZONIA LTDA', 'Cliente', 'CONTRIBUINTE', '00280273000722')
--  CREATE INSERTS CLIENTS END --

create table SealDetalheValorDanfe(
dvd_int_id int identity,
dvd_num_spprivado numeric(18, 6) not null,
dvd_num_splei numeric(18, 6) not null, 
dvd_num_sp numeric(18, 6) not null, 
dvd_num_msprivado numeric(18, 6) not null, 
dvd_num_mslei numeric(18, 6) not null, 
dvd_num_ms numeric(18, 6) not null, 
dvd_int_rec int null, 
dvd_int_nrec int null, 
dvd_int_contrib int null, 
dvd_int_ncontrib int null, 
constraint PK_SealDetalheValorDanfe primary key (dvd_int_id), 
constraint FK_SealDetalheValorDanfe_Recebida foreign key (dvd_int_rec) references SealDetalheValorDanfe (dvd_int_id), 
constraint FK_SealDetalheValorDanfe_NRecebida foreign key (dvd_int_nrec) references SealDetalheValorDanfe (dvd_int_id), 
constraint FK_SealDetalheValorDanfe_Contribuinte foreign key (dvd_int_contrib) references SealDetalheValorDanfe (dvd_int_id), 
constraint FK_SealDetalheValorDanfe_NContribuinte foreign key (dvd_int_ncontrib) references SealDetalheValorDanfe (dvd_int_id)
);

create table SealDetalheValorServico(
dvs_int_id int identity,
dvs_num_spprivado numeric(18, 6) not null,
dvs_num_splei numeric(18, 6) not null, 
dvs_num_sp numeric(18, 6) not null, 
dvs_num_msprivado numeric(18, 6) not null, 
dvs_num_mslei numeric(18, 6) not null, 
dvs_num_ms numeric(18, 6) not null, 
dvs_int_rec int null, 
dvs_int_nrec int null, 
dvs_int_cum int null, 
dvs_int_ncum int null, 
constraint PK_SealDetalheValorServico primary key (dvs_int_id), 
constraint FK_SealDetalheValorServico_Recebida foreign key (dvs_int_rec) references SealDetalheValorServico (dvs_int_id), 
constraint FK_SealDetalheValorServico_NRecebida foreign key (dvs_int_nrec) references SealDetalheValorServico (dvs_int_id), 
constraint FK_SealDetalheValorServico_Cumulativo foreign key (dvs_int_cum) references SealDetalheValorServico (dvs_int_id), 
constraint FK_SealDetalheValorServico_NCumulativo foreign key (dvs_int_ncum) references SealDetalheValorServico (dvs_int_id)
);

create table SealPeriodoDetalhe(
pdt_int_id int identity, 
dvd_int_id int null, 
dvs_int_id int null, 
constraint PK_SealPeriodoDetalhe primary key (pdt_int_id), 
constraint FK_SealPeriodoDetalhe_SealDetalheValorDanfe foreign key (dvd_int_id) references SealDetalheValorDanfe (dvd_int_id), 
constraint FK_SealPeriodoDetalhe_SealDetalheValorServico foreign key (dvs_int_id) references SealDetalheValorServico (dvs_int_id)
);

create table SealPeriodo(
prd_int_id int identity, 
prd_int_mes int not null, 
prd_int_ano int not null, 
pdt_int_id int null, 
constraint PK_SealPeriodo primary key (prd_int_id), 
constraint FK_SealPeriodo_SealPeriodoDetalhe foreign key (pdt_int_id) references SealPeriodoDetalhe (pdt_int_id)
);

create table SealCalculoDetail(
cald_int_id int identity,
cald_int_mes int not null, 
cald_int_ano int not null,
SpDanfePri numeric(18, 6),
SpDanfePub numeric(18, 6),
SpDanfeRecPri numeric(18, 6),
SpDanfeRecPub numeric(18, 6),
SpDanfeNRecPri numeric(18, 6),
SpDanfeNRecPub numeric(18, 6),
SpDanfeContPri numeric(18, 6),
SpDanfeContPub numeric(18, 6),
SpDanfeNContPri numeric(18, 6),
SpDanfeNContPub numeric(18, 6),
SpServPri numeric(18, 6),
SpServPub numeric(18, 6),
SpServRecPri numeric(18, 6),
SpServRecPub numeric(18, 6),
SpServNRecPri numeric(18, 6),
SpServNRecPub numeric(18, 6),
SpServCumPri numeric(18, 6),
SpServCumPub numeric(18, 6),
SpServNCumPri numeric(18, 6),
SpServNCumPub numeric(18, 6),
MsDanfePri numeric(18, 6),
MsDanfePub numeric(18, 6),
MsDanfeRecPri numeric(18, 6),
MsDanfeRecPub numeric(18, 6),
MsDanfeNRecPri numeric(18, 6),
MsDanfeNRecPub numeric(18, 6),
MsDanfeContPri numeric(18, 6),
MsDanfeContPub numeric(18, 6),
MsDanfeNContPri numeric(18, 6),
MsDanfeNContPub numeric(18, 6),
MsServPri numeric(18, 6),
MsServPub numeric(18, 6),
MsServRecPri numeric(18, 6),
MsServRecPub numeric(18, 6),
MsServNRecPri numeric(18, 6),
MsServNRecPub numeric(18, 6),
MsServCumPri numeric(18, 6),
MsServCumPub numeric(18, 6),
MsServNCumPri numeric(18, 6),
MsServNCumPub numeric(18, 6), 
constraint PK_SealCalculoDetail primary key (cald_int_id)
);

create table SealCalculoConsolidado(
calc_int_id int identity,
calc_int_mes int not null, 
calc_int_ano int not null,
SpDanfeNor numeric(18, 6),
MsDanfeNor numeric(18, 6),
SpDanfeExc numeric(18, 6),
MsDanfeExc numeric(18, 6),
SpDanfeAdc numeric(18, 6),
MsDanfeAdc numeric(18, 6),
SpServNCumNor numeric(18, 6),
MsServNCumNor numeric(18, 6),
SpServNCumExc numeric(18, 6),
MsServNCumExc numeric(18, 6),
SpServNCumAdc numeric(18, 6),
MsServNCumAdc numeric(18, 6),
SpServCumNor numeric(18, 6),
MsServCumNor numeric(18, 6),
SpServCumExc numeric(18, 6),
MsServCumExc numeric(18, 6),
SpServCumAdc numeric(18, 6),
MsServCumAdc numeric(18, 6),
constraint PK_SealCalculoConsolidado primary key (calc_int_id)
);

create table SealCalculoRetencao(
calr_int_id int identity,
calr_int_mes int not null, 
calr_int_ano int not null,
NCumPisPri numeric(18, 6),
NCumPisPub numeric(18, 6),
NCumCsslPri numeric(18, 6),
NCumCsslPub numeric(18, 6),
NCumCofiPri numeric(18, 6),
NCumCofiPub numeric(18, 6),
NCumIrPri numeric(18, 6),
NCumIrPub numeric(18, 6),
NCumInssPri numeric(18, 6),
NCumInssPub numeric(18, 6),
NCumIssPri numeric(18, 6),
NCumIssPub numeric(18, 6),
CumPisPri numeric(18, 6),
CumPisPub numeric(18, 6),
CumCsslPri numeric(18, 6),
CumCsslPub numeric(18, 6),
CumCofiPri numeric(18, 6),
CumCofiPub numeric(18, 6),
CumIrPri numeric(18, 6),
CumIrPub numeric(18, 6),
CumInssPri numeric(18, 6),
CumInssPub numeric(18, 6),
CumIssPri numeric(18, 6),
CumIssPub numeric(18, 6),
constraint PK_SealCalculoRetencao primary key (calr_int_id)
);

create table SealCalculo(
cal_int_id int identity, 
cal_int_mes int not null,
cal_int_ano int not null, 
cal_bit_finalizado bit, 
cal_int_detail int, 
cal_int_consolidado int, 
cal_int_retencao int, 
constraint PK_SealCalculo primary key (cal_int_id), 
constraint FK_SealCalculo_Detail foreign key (cal_int_detail) references SealCalculoDetail (cald_int_id),
constraint FK_SealCalculo_Consolidado foreign key (cal_int_consolidado) references SealCalculoConsolidado (calc_int_id),
constraint FK_SealCalculo_Retencao foreign key (cal_int_retencao) references SealCalculoRetencao (calr_int_id),
);