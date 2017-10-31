using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class NfeDAO : Connection
    {
        public NfeDAO() 
            : base()
        {
            conn = new SqlConnection(connectionstring);
        }

        public bool GetDanfes(ref List<Assecontweb.Extend.Nfe.Danfe.NotaFiscal> danfes, ref string erro)
        {
            string query = "";
            bool retorno = false;
            erro = "OK";
            try
            {
                conn.Open();
                query = @"select a.nfd_int_id,a.nfd_int_nUF,a.nfd_int_cNF,a.nfd_des_natPo,a.nfd_int_indOag,a.nfd_int_mod,a.nfd_int_serie,a.nfd_int_nNf,a.nfd_dt_dhEmi,a.nfd_int_tpNf, 
                            a.nfd_int_idDest,a.nfd_int_cMunicFG,a.nfd_int_tpImp,a.nfd_int_tpEmis,a.nfd_int_cDV,a.nfd_int_tpAmb,a.nfd_int_finNfe,a.nfd_int_indFinal,a.nfd_int_indPres, 
                            a.nfd_int_procEmi,a.nfd_des_verProc,a.nfd_des_xml, SUM(d.ndi_num_vProd) VProds, SUM(d.ndi_num_qTrib) qTrib
                            from SealNotaFiscalDanfe a 
                            inner join SealNotaFiscalDanfeEmpresa b on a.nde_int_emit = b.nde_int_id 
                            inner join SealNotaFiscalDanfeEmpresa c on a.nde_int_dest = c.nde_int_id
                            inner join SealNotaFiscalDanfeItem d on a.nfd_int_id = d.nfd_int_id 
                            group by a.nfd_int_id,a.nfd_int_nUF,a.nfd_int_cNF,a.nfd_des_natPo,a.nfd_int_indOag,a.nfd_int_mod,a.nfd_int_serie,a.nfd_int_nNf,a.nfd_dt_dhEmi,a.nfd_int_tpNf, 
                            a.nfd_int_idDest,a.nfd_int_cMunicFG,a.nfd_int_tpImp,a.nfd_int_tpEmis,a.nfd_int_cDV,a.nfd_int_tpAmb,a.nfd_int_finNfe,a.nfd_int_indFinal,a.nfd_int_indPres, 
                            a.nfd_int_procEmi,a.nfd_des_verProc,a.nfd_des_xml
                            order by a.nfd_dt_dhEmi desc";

                cmd = new SqlCommand(query, conn);

                DbDataReader dr = cmd.ExecuteReader();

                Assecontweb.Extend.Nfe.Danfe.NotaFiscal danfe;
                while (dr.Read())
                {
                    danfe = new Assecontweb.Extend.Nfe.Danfe.NotaFiscal();
                    danfe.Ide = new Assecontweb.Extend.Nfe.Danfe.IdeModel();
                    danfe.Ide.cUF = Convert.ToString(dr["nfd_int_nUF"]);
                    danfe.Ide.cNF = Convert.ToInt32(dr["nfd_int_cNF"]);
                    danfe.Ide.natOp = Convert.ToString(dr["nfd_des_natPo"]);
                    danfe.Ide.indPag = Convert.ToInt32(dr["nfd_int_indOag"]);
                    danfe.Ide.mod = Convert.ToInt32(dr["nfd_int_mod"]);
                    danfe.Ide.serie = Convert.ToInt32(dr["nfd_int_serie"]);
                    danfe.Ide.nNf = Convert.ToInt32(dr["nfd_int_nNf"]);
                    danfe.Ide.dhEmi = Convert.ToDateTime(dr["nfd_dt_dhEmi"]).ToString("dd/MM/yyyy HH:mm");
                    danfe.Ide.tpNF = Convert.ToInt32(dr["nfd_int_tpNf"]);
                    danfe.Ide.idDest = Convert.ToInt32(dr["nfd_int_idDest"]);
                    danfe.Ide.cMunFG = Convert.ToInt32(dr["nfd_int_cMunicFG"]);
                    danfe.Ide.tpImp = Convert.ToInt32(dr["nfd_int_tpImp"]);
                    danfe.Ide.tpEmis = Convert.ToInt32(dr["nfd_int_tpEmis"]);
                    danfe.Ide.cDV = Convert.ToInt32(dr["nfd_int_cDV"]);
                    danfe.Ide.tpAmb = Convert.ToInt32(dr["nfd_int_tpAmb"]);
                    danfe.Ide.finNFe = Convert.ToInt32(dr["nfd_int_finNfe"]);
                    danfe.Ide.indFinal = Convert.ToInt32(dr["nfd_int_indFinal"]);
                    danfe.Ide.indPres = Convert.ToInt32(dr["nfd_int_indPres"]);
                    danfe.Ide.procEmi = Convert.ToInt32(dr["nfd_int_procEmi"]);
                    danfe.Ide.veProc = Convert.ToString(dr["nfd_des_verProc"]);
                    danfe.xmlConteudo = Convert.ToString(dr["nfd_des_xml"]);

                    danfe.Total = new Assecontweb.Extend.Nfe.Danfe.ICMSTotModel();
                    danfe.Total.vProd = (Convert.ToDouble(dr["VProds"]));
                    danfe.Total.vTotTrib = (Convert.ToDouble(dr["qTrib"]));

                    danfes.Add(danfe);
                }
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

        public bool SetDanfes(List<Assecontweb.Extend.Nfe.Danfe.NotaFiscal> danfes, out string erro)
        {
            string query = "";
            bool retorno = false;
            int i;
            erro = "OK";
            try
            {
                conn.Open();
                foreach (Assecontweb.Extend.Nfe.Danfe.NotaFiscal danfe in danfes)
                {
                    #region Emitente
                    query = @"declare @nde_int_emit int
                              
                              insert into SealNotaFiscalDanfeEmpresa
                              (nde_des_cnpj,nde_des_nome,nde_des_fant,nde_des_lgr,nde_des_nro,nde_des_cpl,nde_des_bairro,nde_int_cMun,nde_des_mun,nde_des_uf,nde_des_cep,nde_int_cPais,
                              nde_des_pais,nde_des_fone,nde_int_ie,nde_int_crt,nde_des_email)
                              values (@nde_des_cnpj_emit,@nde_des_nome_emit,@nde_des_fant_emit,@nde_des_lgr_emit,@nde_des_nro_emit,@nde_des_cpl_emit,@nde_des_bairro_emit,@nde_int_cMun_emit,
                              @nde_des_mun_emit,@nde_des_uf_emit,@nde_des_cep_emit,@nde_int_cPais_emit,@nde_des_pais_emit,@nde_des_fone_emit,@nde_int_ie_emit,@nde_int_crt_emit,@nde_des_email_emit)
                              set @nde_int_emit = (select IDENT_CURRENT('SealNotaFiscalDanfeEmpresa'))
                              select @nde_int_emit";

                    if (string.IsNullOrEmpty(danfe.Emit.enderEmit.xCpl))
                        query = query.Replace("@nde_des_cpl_emit,", "").Replace("nde_des_cpl,", "");
                    if (string.IsNullOrEmpty(danfe.Emit.IE))
                        query = query.Replace("@nde_int_ie_emit,", "").Replace("nde_int_ie,", "");
                    if (string.IsNullOrEmpty(danfe.Emit.enderEmit.Email))
                        query = query.Replace(",@nde_des_email_emit", "").Replace(",nde_des_email", "");

                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nde_des_cnpj_emit", danfe.Emit.CNPJ);
                    cmd.Parameters.AddWithValue("@nde_des_nome_emit", danfe.Emit.xNome);
                    cmd.Parameters.AddWithValue("@nde_des_fant_emit", danfe.Emit.xFant);
                    cmd.Parameters.AddWithValue("@nde_des_lgr_emit", danfe.Emit.enderEmit.xLgr);
                    cmd.Parameters.AddWithValue("@nde_des_nro_emit", danfe.Emit.enderEmit.nro);
                    if (!string.IsNullOrEmpty(danfe.Emit.enderEmit.xCpl))
                        cmd.Parameters.AddWithValue("@nde_des_cpl_emit", danfe.Emit.enderEmit.xCpl);
                    cmd.Parameters.AddWithValue("@nde_des_bairro_emit", danfe.Emit.enderEmit.xBairro);
                    cmd.Parameters.AddWithValue("@nde_int_cMun_emit", danfe.Emit.enderEmit.cMun);
                    cmd.Parameters.AddWithValue("@nde_des_mun_emit", danfe.Emit.enderEmit.xMun);
                    cmd.Parameters.AddWithValue("@nde_des_uf_emit", danfe.Emit.enderEmit.UF);
                    cmd.Parameters.AddWithValue("@nde_des_cep_emit", danfe.Emit.enderEmit.CEP);
                    cmd.Parameters.AddWithValue("@nde_int_cPais_emit", danfe.Emit.enderEmit.cPais);
                    cmd.Parameters.AddWithValue("@nde_des_pais_emit", danfe.Emit.enderEmit.xPais);
                    cmd.Parameters.AddWithValue("@nde_des_fone_emit", danfe.Emit.enderEmit.Fone);
                    if (!string.IsNullOrEmpty(danfe.Emit.IE))
                        cmd.Parameters.AddWithValue("@nde_int_ie_emit", danfe.Emit.IE);
                    cmd.Parameters.AddWithValue("@nde_int_crt_emit", danfe.Emit.CRT);
                    if (!string.IsNullOrEmpty(danfe.Emit.enderEmit.Email))
                        cmd.Parameters.AddWithValue("@nde_des_email_emit", danfe.Emit.enderEmit.Email);

                    int nde_int_emit = Convert.ToInt32(cmd.ExecuteScalar());
                    #endregion
                    #region Destinatario
                    query = @"declare @nde_int_dest int
                              
                              insert into SealNotaFiscalDanfeEmpresa
                              (nde_des_cnpj,nde_des_nome,nde_des_fant,nde_des_lgr,nde_des_nro,nde_des_cpl,nde_des_bairro,nde_int_cMun,nde_des_mun,nde_des_uf,nde_des_cep,nde_int_cPais,nde_des_pais,nde_des_fone,nde_int_ie,nde_int_indIEDest,nde_des_email)
                              values (@nde_des_cnpj_dest,@nde_des_nome_dest,@nde_des_fant_dest,@nde_des_lgr_dest,@nde_des_nro_dest,@nde_des_cpl_dest,@nde_des_bairro_dest,@nde_int_cMun_dest,@nde_des_mun_dest,@nde_des_uf_dest,@nde_des_cep_dest,@nde_int_cPais_dest,@nde_des_pais_dest,@nde_des_fone_dest,@nde_int_ie_dest,@nde_int_indIEDest_dest,@nde_des_email_dest)
                              set @nde_int_dest = (select IDENT_CURRENT('SealNotaFiscalDanfeEmpresa')) 
                              select @nde_int_dest";

                    if (string.IsNullOrEmpty(danfe.Dest.xFant))
                        query = query.Replace("@nde_des_fant_dest,", "").Replace("nde_des_fant,", "");
                    if (string.IsNullOrEmpty(danfe.Dest.enderDest.xLgr))
                        query = query.Replace("@nde_des_lgr_dest,", "").Replace("nde_des_lgr,", "");
                    if (string.IsNullOrEmpty(danfe.Dest.enderDest.nro))
                        query = query.Replace("@nde_des_nro_dest,", "").Replace("nde_des_nro,", "");
                    if (string.IsNullOrEmpty(danfe.Dest.enderDest.xBairro))
                        query = query.Replace("@nde_des_bairro_dest,", "").Replace("nde_des_bairro,", "");
                    if (string.IsNullOrEmpty(danfe.Dest.enderDest.cMun))
                        query = query.Replace("@nde_int_cMun_dest,", "").Replace("nde_des_cMun,", "");
                    if (string.IsNullOrEmpty(danfe.Dest.enderDest.xMun))
                        query = query.Replace("@nde_des_mun_dest,", "").Replace("nde_des_mun,", "");
                    if (string.IsNullOrEmpty(danfe.Dest.enderDest.UF))
                        query = query.Replace("@nde_des_uf_dest,", "").Replace("nde_des_uf,", "");
                    if (string.IsNullOrEmpty(danfe.Dest.enderDest.CEP))
                        query = query.Replace("@nde_des_cep_dest,", "").Replace("nde_des_cep,", "");
                    if (string.IsNullOrEmpty(danfe.Dest.enderDest.cPais.ToString()) || danfe.Dest.enderDest.cPais == 0)
                        query = query.Replace("@nde_int_cPais_dest,", "").Replace("nde_des_cPais,", "");
                    if (string.IsNullOrEmpty(danfe.Dest.enderDest.xPais))
                        query = query.Replace("@nde_des_pais_dest,", "").Replace("nde_des_pais,", "");
                    if (string.IsNullOrEmpty(danfe.Dest.enderDest.Fone))
                        query = query.Replace("@nde_des_fone_dest,", "").Replace("nde_des_fone,", "");
                    if (string.IsNullOrEmpty(danfe.Dest.enderDest.xCpl))
                        query = query.Replace("@nde_des_cpl_dest,", "").Replace("nde_des_cpl,", "");
                    if (string.IsNullOrEmpty(danfe.Dest.IE))
                        query = query.Replace("@nde_int_ie_dest,", "").Replace("nde_int_ie,", "");
                    if (string.IsNullOrEmpty(danfe.Dest.indIEDest.ToString()) || danfe.Dest.indIEDest == 0)
                        query = query.Replace("@nde_int_indIEDest_dest,", "").Replace("nde_int_indIEDest,", "");
                    if (string.IsNullOrEmpty(danfe.Dest.enderDest.Email))
                        query = query.Replace(",@nde_des_email_dest", "").Replace(",nde_des_email", "");

                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nde_des_cnpj_dest", danfe.Dest.CNPJ);
                    cmd.Parameters.AddWithValue("@nde_des_nome_dest", danfe.Dest.xNome);
                    if (!string.IsNullOrEmpty(danfe.Dest.xFant))
                        cmd.Parameters.AddWithValue("@nde_des_fant_dest", danfe.Dest.xFant);
                    if (!string.IsNullOrEmpty(danfe.Dest.enderDest.xLgr))
                        cmd.Parameters.AddWithValue("@nde_des_lgr_dest", danfe.Dest.enderDest.xLgr);
                    if (!string.IsNullOrEmpty(danfe.Dest.enderDest.nro))
                        cmd.Parameters.AddWithValue("@nde_des_nro_dest", danfe.Dest.enderDest.nro);
                    if (!string.IsNullOrEmpty(danfe.Dest.enderDest.xCpl))
                        cmd.Parameters.AddWithValue("@nde_des_cpl_dest", danfe.Dest.enderDest.xCpl);
                    if (!string.IsNullOrEmpty(danfe.Dest.enderDest.xBairro))
                        cmd.Parameters.AddWithValue("@nde_des_bairro_dest", danfe.Dest.enderDest.xBairro);
                    if (!string.IsNullOrEmpty(danfe.Dest.enderDest.cMun))
                        cmd.Parameters.AddWithValue("@nde_int_cMun_dest", danfe.Dest.enderDest.cMun);
                    if (!string.IsNullOrEmpty(danfe.Dest.enderDest.xMun))
                        cmd.Parameters.AddWithValue("@nde_des_mun_dest", danfe.Dest.enderDest.xMun);
                    if (!string.IsNullOrEmpty(danfe.Dest.enderDest.UF))
                        cmd.Parameters.AddWithValue("@nde_des_uf_dest", danfe.Dest.enderDest.UF);
                    if (!string.IsNullOrEmpty(danfe.Dest.enderDest.CEP))
                        cmd.Parameters.AddWithValue("@nde_des_cep_dest", danfe.Dest.enderDest.CEP);
                    if (!string.IsNullOrEmpty(danfe.Dest.enderDest.cPais.ToString()) && danfe.Dest.enderDest.cPais != 0)
                        cmd.Parameters.AddWithValue("@nde_int_cPais_dest", danfe.Dest.enderDest.cPais);
                    if (!string.IsNullOrEmpty(danfe.Dest.enderDest.xPais))
                        cmd.Parameters.AddWithValue("@nde_des_pais_dest", danfe.Dest.enderDest.xPais);
                    if (!string.IsNullOrEmpty(danfe.Dest.enderDest.Fone))
                        cmd.Parameters.AddWithValue("@nde_des_fone_dest", danfe.Dest.enderDest.Fone);
                    if (!string.IsNullOrEmpty(danfe.Dest.IE))
                        cmd.Parameters.AddWithValue("@nde_des_fone_dest", danfe.Dest.enderDest.Fone);
                    if (!string.IsNullOrEmpty(danfe.Dest.IE))
                        cmd.Parameters.AddWithValue("@nde_int_ie_dest", danfe.Dest.IE);
                    if (!string.IsNullOrEmpty(danfe.Dest.indIEDest.ToString()) || danfe.Dest.indIEDest == 0)
                        cmd.Parameters.AddWithValue("@nde_int_indIEDest_dest", danfe.Dest.indIEDest);
                    if (!string.IsNullOrEmpty(danfe.Dest.enderDest.Email))
                        cmd.Parameters.AddWithValue("@nde_des_email_dest", danfe.Dest.enderDest.Email);

                    int nde_int_dest = Convert.ToInt32(cmd.ExecuteScalar());
                    #endregion
                    #region Nota Fiscal
                    query = @"declare @nfd_int_nUF int              = @nUF,
		                                @nfd_int_cNF int              = @cNF,
		                                @nfd_des_natPo varchar(255)   = @natPo,
		                                @nfd_int_indOag int           = @indOag,
		                                @nfd_int_mod int              = @mod,
		                                @nfd_int_serie int            = @serie,
		                                @nfd_int_nNf int              = @nNf,
		                                @nfd_dt_dhEmi datetime        = @dhEmi,
		                                @nfd_int_tpNf int             = @tpNf,
		                                @nfd_int_idDest int           = @idDest,
		                                @nfd_int_cMunicFG int         = @cMunicFG,
		                                @nfd_int_tpImp int            = @tpImp,
		                                @nfd_int_tpEmis int           = @tpEmis,
		                                @nfd_int_cDV int              = @cDV,
		                                @nfd_int_tpAmb int            = @tpAmb,
		                                @nfd_int_finNfe int           = @finNfe,
		                                @nfd_int_indFinal int         = @indFinal,
		                                @nfd_int_indPres int          = @indPres,
		                                @nfd_int_procEmi int          = @procEmi,
		                                @nfd_des_verProc varchar(255) = @verProc, 
		                                @nfd_int_id int 

                                --if (not exists(select nfd_int_id from SealNotaFiscalDanfe where nfd_int_nNf = @nfd_int_nNf))
                                --begin
	                                insert into SealNotaFiscalDanfe
	                                (nfd_int_nUF,nfd_int_cNF,nfd_des_natPo,nfd_int_indOag,nfd_int_mod,nfd_int_serie,nfd_int_nNf,nfd_dt_dhEmi,nfd_int_tpNf,
	                                nfd_int_idDest,nfd_int_cMunicFG,nfd_int_tpImp,nfd_int_tpEmis,nfd_int_cDV,nfd_int_tpAmb,nfd_int_finNfe,nfd_int_indFinal,nfd_int_indPres,nfd_int_procEmi,
	                                nfd_des_verProc,nde_int_emit,nde_int_dest, nfd_des_xml)
	                                values (@nfd_int_nUF,@nfd_int_cNF,@nfd_des_natPo,@nfd_int_indOag,@nfd_int_mod,@nfd_int_serie,@nfd_int_nNf,@nfd_dt_dhEmi,@nfd_int_tpNf,@nfd_int_idDest,
	                                @nfd_int_cMunicFG,@nfd_int_tpImp,@nfd_int_tpEmis,@nfd_int_cDV,@nfd_int_tpAmb,@nfd_int_finNfe,@nfd_int_indFinal,@nfd_int_indPres,@nfd_int_procEmi,
	                                @nfd_des_verProc,@nde_int_emit,@nde_int_dest,@nfd_des_xml)
	                                set @nfd_int_id = (select IDENT_CURRENT('SealNotaFiscalDanfe')) 
	                                select @nfd_int_id
                                --end else begin
	                                --set @nfd_int_id = (select nfd_int_id from SealNotaFiscalDanfe where nfd_int_nNf = @nfd_int_nNf)
	                                --update SealNotaFiscalDanfe
	                                --set nfd_int_nUF = @nfd_int_nUF,
		                                --nfd_int_cNF = @nfd_int_cNF,
		                                --nfd_des_natPo = @nfd_des_natPo,
		                                --nfd_int_indOag = @nfd_int_indOag,
		                                ----nfd_int_mod = @nfd_int_mod,
		                                --nfd_int_serie = @nfd_int_serie,
		                                ----nfd_int_nNf = @nfd_int_nNf,
		                                --nfd_dt_dhEmi = @nfd_dt_dhEmi,
		                                --nfd_int_tpNf = @nfd_int_tpNf,
		                                --nfd_int_idDest = @nfd_int_idDest,
		                                ----nfd_int_cMunicFG = @nfd_int_cMunicFG,
		                                --nfd_int_tpImp = @nfd_int_tpImp,
		                                --nfd_int_tpEmis = @nfd_int_tpEmis,
		                                --nfd_int_cDV = @nfd_int_cDV,
		                                --nfd_int_tpAmb = @nfd_int_tpAmb,
		                                --nfd_int_finNfe = @nfd_int_finNfe,
		                                --nfd_int_indFinal = @nfd_int_indFinal,
		                                --nfd_int_indPres = @nfd_int_indPres,
		                                --nfd_int_procEmi = @nfd_int_procEmi,
		                                --nfd_des_verProc = @nfd_des_verProc,
		                                --nde_int_emit = @nde_int_emit,
		                                --nde_int_dest = @nde_int_dest, 
		                                --nfd_des_xml = @nfd_des_xml 
	                                --where nfd_int_id = @nfd_int_id
	                                --select '0-' + CONVERT(varchar(100), @nfd_int_id)
                                --end";

                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nUF", danfe.Ide.cUF);
                    cmd.Parameters.AddWithValue("@cNF", danfe.Ide.cNF);
                    cmd.Parameters.AddWithValue("@natPo", danfe.Ide.natOp);
                    cmd.Parameters.AddWithValue("@indOag", danfe.Ide.indPag);
                    cmd.Parameters.AddWithValue("@mod", danfe.Ide.mod);
                    cmd.Parameters.AddWithValue("@serie", danfe.Ide.serie);
                    cmd.Parameters.AddWithValue("@nNf", danfe.Ide.nNf);
                    cmd.Parameters.AddWithValue("@dhEmi", danfe.Ide.dhEmi);
                    cmd.Parameters.AddWithValue("@tpNf", danfe.Ide.tpNF);
                    cmd.Parameters.AddWithValue("@idDest", danfe.Ide.idDest);
                    cmd.Parameters.AddWithValue("@cMunicFG", danfe.Ide.cMunFG);
                    cmd.Parameters.AddWithValue("@tpImp", danfe.Ide.tpImp);
                    cmd.Parameters.AddWithValue("@tpEmis", danfe.Ide.tpEmis);
                    cmd.Parameters.AddWithValue("@cDV", danfe.Ide.cDV);
                    cmd.Parameters.AddWithValue("@tpAmb", danfe.Ide.tpAmb);
                    cmd.Parameters.AddWithValue("@finNfe", danfe.Ide.finNFe);
                    cmd.Parameters.AddWithValue("@indFinal", danfe.Ide.indFinal);
                    cmd.Parameters.AddWithValue("@indPres", danfe.Ide.indPres);
                    cmd.Parameters.AddWithValue("@procEmi", danfe.Ide.procEmi);
                    cmd.Parameters.AddWithValue("@verProc", danfe.Ide.veProc);
                    cmd.Parameters.AddWithValue("@nde_int_emit", nde_int_emit);
                    cmd.Parameters.AddWithValue("@nde_int_dest", nde_int_dest);
                    cmd.Parameters.AddWithValue("@nfd_des_xml", danfe.xmlConteudo);

                    //string strRetorno = Convert.ToString(cmd.ExecuteScalar());
                    //string[] split = strRetorno.Split(new string[] { "-" }, StringSplitOptions.None);
                    int nfd_int_id = Convert.ToInt32(cmd.ExecuteScalar());
                    #endregion
                    #region Item de NotaFiscal
                    int count = 1;
                    foreach (Assecontweb.Extend.Nfe.Danfe.detModel item in danfe.Itens)
                    {
                        try
                        {
                            query = @"declare @ndi_int_num int                  = @num,
		                                  @ndi_des_cProd varchar(100)       = @cProd,
		                                  @ndi_des_prod varchar(100)        = @prod,
		                                  @ndi_int_ncm int                  = @ncm,
		                                  @ndi_int_cfop int                 = @cfop,
		                                  @ndi_des_uCom varchar(2)          = @uCom,
		                                  @ndi_num_qCom numeric(14,4)       = @qCom,
		                                  @ndi_num_vUnCom numeric(22,10)    = @nUnCom,
		                                  @ndi_num_vProd numeric(16,2)      = @vProd,
		                                  @ndi_des_uTrib varchar(2)         = @uTrib,
		                                  @ndi_num_qTrib numeric(14,4)      = @qTrib,
		                                  @ndi_num_vUnTrib numeric(22,10)   = @vUnTrib,
		                                  @ndi_int_indTot int               = @indTot 
                                  
                                  insert into SealNotaFiscalDanfeItem
                                  ( ndi_int_num, ndi_des_cProd, ndi_des_prod, ndi_int_ncm, ndi_int_cfop, ndi_des_uCom, ndi_num_qCom, ndi_num_vUnCom, ndi_num_vProd, ndi_des_uTrib, ndi_num_qTrib,
                                   ndi_num_vUnTrib, ndi_int_indTot, nfd_int_id)
                                  values
                                  (@ndi_int_num,@ndi_des_cProd,@ndi_des_prod,@ndi_int_ncm,@ndi_int_cfop,@ndi_des_uCom,@ndi_num_qCom,@ndi_num_vUnCom,@ndi_num_vProd,@ndi_des_uTrib,@ndi_num_qTrib,
                                  @ndi_num_vUnTrib,@ndi_int_indTot,@nfd_int_id)";

                            cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@num", count);
                            cmd.Parameters.AddWithValue("@cProd", item.prod.cProd);
                            cmd.Parameters.AddWithValue("@prod", item.prod.xProd);
                            cmd.Parameters.AddWithValue("@ncm", Convert.ToInt32(item.prod.NCM));
                            cmd.Parameters.AddWithValue("@cfop", Convert.ToInt32(item.prod.CFOP));
                            cmd.Parameters.AddWithValue("@uCom", item.prod.uCom);
                            string qCom = item.prod.qCom.ToString();
                            cmd.Parameters.AddWithValue("@qCom", (float)(Convert.ToDouble(item.prod.qCom)));
                            string vUnCom = item.prod.vUnCom.ToString();
                            cmd.Parameters.AddWithValue("@nUnCom", (float)(Convert.ToDouble(item.prod.vUnCom)));
                            string vProd = item.prod.vProd.ToString();
                            cmd.Parameters.AddWithValue("@vProd", (float)(Convert.ToDouble(item.prod.vProd)));
                            cmd.Parameters.AddWithValue("@uTrib", item.prod.uTrib);
                            string qTrib = item.prod.qTrib.ToString();
                            cmd.Parameters.AddWithValue("@qTrib", (float)(Convert.ToDouble(item.prod.qTrib)));
                            string vUnTrib = item.prod.vUnTrib.ToString();
                            cmd.Parameters.AddWithValue("@vUnTrib", (float)(Convert.ToDouble(item.prod.vUnTrib)));
                            cmd.Parameters.AddWithValue("@indTot", item.prod.indTot);
                            cmd.Parameters.AddWithValue("@nfd_int_id", nfd_int_id);

                            i = cmd.ExecuteNonQuery();
                            retorno = (i > 0);
                            if (!retorno)
                                break;

                            count++;
                        }
                        catch(Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                    #endregion
                }
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
    }
}
