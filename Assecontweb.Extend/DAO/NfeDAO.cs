using System;
using System.Collections.Generic;
using System.Data;
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

        public bool SetDanfes(List<Assecontweb.Extend.Nfe.Danfe.NotaFiscal> danfes, List<DataSet> events, out string erro)
        {
            string query = "";
            bool retorno = false;
            int i;
            erro = "OK";
            try
            {
                try
                {
                    #region Events
                    i = 0;
                    foreach (DataSet ds in events)
                    {
                        DataTable dt = ds.Tables["infEvento"];
                        if (dt != null)
                        {
                            DataColumn chNFe = dt.Columns["chNFe"];
                            DataColumn tpEvento = dt.Columns["tpEvento"];
                            //110111
                            if (tpEvento != null)
                            {
                                DataTable dcdt1 = tpEvento.Table;
                                if (dcdt1 != null)
                                {
                                    DataRow dr1 = dcdt1.Rows[0];
                                    if (dr1 != null)
                                    {
                                        object[] itens1 = dr1.ItemArray;
                                        if (itens1 != null && itens1.Count() > 3)
                                        {
                                            if (itens1[5].ToString().Trim() == "110111")
                                            {
                                                if (chNFe != null)
                                                {
                                                    DataTable dcdt2 = chNFe.Table;
                                                    if (dcdt2 != null)
                                                    {
                                                        DataRow dr2 = dcdt2.Rows[0];
                                                        if (dr2 != null)
                                                        {
                                                            object[] itens2 = dr2.ItemArray;
                                                            if (itens2 != null && itens2.Count() > 3)
                                                            {
                                                                Assecontweb.Extend.Nfe.Danfe.NotaFiscal nt = danfes.Find(f => f.chave == itens2[3].ToString());
                                                                if (nt != null)
                                                                    danfes.Remove(nt);
                                                                i++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                conn.Open();
                foreach (Assecontweb.Extend.Nfe.Danfe.NotaFiscal danfe in danfes)
                {
                    int nde_int_emit = 0;
                    #region Emitente
                    try
                    {
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

                        nde_int_emit = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    #endregion
                    #region Destinatario
                    int nde_int_dest = 0;
                    try
                    {
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
                        if (!string.IsNullOrEmpty(danfe.Dest.CNPJ))
                            cmd.Parameters.AddWithValue("@nde_des_cnpj_dest", danfe.Dest.CNPJ);
                        else
                            cmd.Parameters.AddWithValue("@nde_des_cnpj_dest", "00000000000000");
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
                            cmd.Parameters.AddWithValue("@nde_int_ie_dest", danfe.Dest.IE);
                        if (!string.IsNullOrEmpty(danfe.Dest.indIEDest.ToString()) || danfe.Dest.indIEDest == 0)
                            cmd.Parameters.AddWithValue("@nde_int_indIEDest_dest", danfe.Dest.indIEDest);
                        if (!string.IsNullOrEmpty(danfe.Dest.enderDest.Email))
                            cmd.Parameters.AddWithValue("@nde_des_email_dest", danfe.Dest.enderDest.Email);

                        nde_int_dest = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    #endregion
                    #region Nota Fiscal
                    int nfd_int_id = 0;
                    try
                    {
                        query = @"declare @nfd_int_nUF int              = @nUF,
		                                @nfd_int_cNF int                = @cNF,
		                                @nfd_des_natPo varchar(255)     = @natPo,
		                                @nfd_int_indOag int             = @indOag,
		                                @nfd_int_mod int                = @mod,
		                                @nfd_int_serie int              = @serie,
		                                @nfd_int_nNf int                = @nNf,
		                                @nfd_dt_dhEmi datetime          = @dhEmi,
		                                @nfd_int_tpNf int               = @tpNf,
		                                @nfd_int_idDest int             = @idDest,
		                                @nfd_int_cMunicFG int           = @cMunicFG,
		                                @nfd_int_tpImp int              = @tpImp,
		                                @nfd_int_tpEmis int             = @tpEmis,
		                                @nfd_int_cDV int                = @cDV,
		                                @nfd_int_tpAmb int              = @tpAmb,
		                                @nfd_int_finNfe int             = @finNfe,
		                                @nfd_int_indFinal int           = @indFinal,
		                                @nfd_int_indPres int            = @indPres,
		                                @nfd_int_procEmi int            = @procEmi,
		                                @nfd_des_verProc varchar(255)   = @verProc, 
                                        @nfd_des_chave varchar(100)     = @chave, 
		                                @nfd_int_id int 

                                if not exists(select * from SealNotaFiscalDanfe where nfd_int_nNf = @nfd_int_nNf)
                                begin
	                                insert into SealNotaFiscalDanfe
	                                (nfd_int_nUF,nfd_int_cNF,nfd_des_natPo,nfd_int_indOag,nfd_int_mod,nfd_int_serie,nfd_int_nNf,nfd_dt_dhEmi,nfd_int_tpNf,
	                                nfd_int_idDest,nfd_int_cMunicFG,nfd_int_tpImp,nfd_int_tpEmis,nfd_int_cDV,nfd_int_tpAmb,nfd_int_finNfe,nfd_int_indFinal,nfd_int_indPres,nfd_int_procEmi,
	                                nfd_des_verProc,nde_int_emit,nde_int_dest, nfd_des_xml, nfd_des_chave)
	                                values (@nfd_int_nUF,@nfd_int_cNF,@nfd_des_natPo,@nfd_int_indOag,@nfd_int_mod,@nfd_int_serie,@nfd_int_nNf,@nfd_dt_dhEmi,@nfd_int_tpNf,@nfd_int_idDest,
	                                @nfd_int_cMunicFG,@nfd_int_tpImp,@nfd_int_tpEmis,@nfd_int_cDV,@nfd_int_tpAmb,@nfd_int_finNfe,@nfd_int_indFinal,@nfd_int_indPres,@nfd_int_procEmi,
	                                @nfd_des_verProc,@nde_int_emit,@nde_int_dest,@nfd_des_xml,@nfd_des_chave)
	                                set @nfd_int_id = (select IDENT_CURRENT('SealNotaFiscalDanfe')) 
	                                select @nfd_int_id
                                end else begin
	                                delete from SealNotaFiscalDanfeEmpresa where nde_int_id = @nde_int_emit or nde_int_id = @nde_int_dest
	                                select -1
                                end";

                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@nUF", danfe.Ide.cUF);
                        cmd.Parameters.AddWithValue("@cNF", danfe.Ide.cNF);
                        cmd.Parameters.AddWithValue("@natPo", danfe.Ide.natOp);
                        cmd.Parameters.AddWithValue("@indOag", danfe.Ide.indPag);
                        cmd.Parameters.AddWithValue("@mod", danfe.Ide.mod);
                        cmd.Parameters.AddWithValue("@serie", danfe.Ide.serie);
                        cmd.Parameters.AddWithValue("@nNf", danfe.Ide.nNf);
                        cmd.Parameters.AddWithValue("@dhEmi", Convert.ToDateTime(danfe.Ide.dhEmi));
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
                        cmd.Parameters.AddWithValue("@chave", danfe.chave);

                        //string strRetorno = Convert.ToString(cmd.ExecuteScalar());
                        //string[] split = strRetorno.Split(new string[] { "-" }, StringSplitOptions.None);
                        nfd_int_id = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    #endregion
                    #region Item de NotaFiscal
                    if (nfd_int_id != -1)
                    {
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
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                    }
                    #endregion
                }

                #region Events
                i = 0;
                foreach (DataSet ds in events)
                {
                    Assecontweb.Extend.Nfe.Danfe.NotaFiscal nt = null;
                    DataTable dt = ds.Tables["infEvento"];
                    if (dt != null)
                    {
                        DataColumn chNFe = dt.Columns["chNFe"];
                        DataColumn tpEvento = dt.Columns["tpEvento"];
                        //110111
                        if (tpEvento != null)
                        {
                            DataTable dcdt1 = tpEvento.Table;
                            if (dcdt1 != null)
                            {
                                DataRow dr1 = dcdt1.Rows[0];
                                if (dr1 != null)
                                {
                                    object[] itens1 = dr1.ItemArray;
                                    if (itens1 != null && itens1.Count() > 3)
                                    {
                                        if (itens1[5].ToString().Trim() == "110111")
                                        {
                                            if (chNFe != null)
                                            {
                                                DataTable dcdt2 = chNFe.Table;
                                                if (dcdt2 != null)
                                                {
                                                    DataRow dr2 = dcdt2.Rows[0];
                                                    if (dr2 != null)
                                                    {
                                                        object[] itens2 = dr2.ItemArray;
                                                        if (itens2 != null && itens2.Count() > 3)
                                                        {
                                                            nt = danfes.Find(f => f.chave == itens2[3].ToString());
                                                            if (nt != null)
                                                            {
                                                                try
                                                                {
                                                                    query = @"declare @chave int = @nfd_int_chave, 
		                                                                                @emit int, 
		                                                                                @dest int, 
		                                                                                @nota int

                                                                                if exists(select * from SealNotaFiscalDanfe where nfd_int_chave = @chave)
                                                                                begin 
	                                                                                set @nota = (select nfd_int_id from SealNotaFiscalDanfe where nfd_int_chave = @chave)
	                                                                                delete from SealNotaFiscalDanfeItem where nfd_int_id = @nota
	                                                                                delete from SealNotaFiscalDanfe where nfd_int_id = @nota

	                                                                                set @emit = (select nfd_int_emit from SealNotaFiscalDanfe where nfd_int_id = @nota)
	                                                                                delete from SealNotaFiscalDanfeEmpresa where nde_int_id = @emit

	                                                                                set @dest = (select nfd_int_dest from SealNotaFiscalDanfe where nfd_int_id = @nota)
	                                                                                delete from SealNotaFiscalDanfeEmpresa where nde_int_id = @dest
                                                                                end ";

                                                                    cmd = new SqlCommand(query, conn);
                                                                    cmd.Parameters.AddWithValue("@chave", Convert.ToInt32(nt.chave));
                                                                    cmd.ExecuteNonQuery();
                                                                }
                                                                catch (Exception ex)
                                                                {

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
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

        public bool GetDanfesNaoProcessadas(ref List<Assecontweb.Extend.Nfe.Danfe.NotaFiscal> danfes, out string erro)
        {
            string query = "";
            bool retorno = false;
            erro = "OK";
            try
            {
                conn.Open();

                query = @"select a.nfd_int_id,a.nfd_int_nNf,a.nfd_dt_dhEmi,
	                               (case when LEN(nde_des_cnpj) < 14 then nde_des_cnpj else null end) as CPF, 
	                               (case when LEN(nde_des_cnpj) = 14 then nde_des_cnpj else null end) as CNPJ
                            from SealNotaFiscalDanfe a  
                            left join SealNotaFiscalRecebida b on a.nfd_int_id = b.nfd_int_id 
                            inner join SealNotaFiscalDanfeItem c on a.nfd_int_id = c.nfd_int_id  
                            inner join SealNotaFiscalDanfeEmpresa d on a.nde_int_dest = d.nde_int_id 
                            where b.nfr_int_id is null";

                cmd = new SqlCommand(query, conn);

                DbDataReader dr = cmd.ExecuteReader();
                
                Assecontweb.Extend.Nfe.Danfe.NotaFiscal danfe;
                while (dr.Read())
                {
                    danfe = new Assecontweb.Extend.Nfe.Danfe.NotaFiscal();
                    danfe.Ide = new Assecontweb.Extend.Nfe.Danfe.IdeModel();
                    danfe.nfd_int_id = Convert.ToInt32(dr["nfd_int_id"]);
                    danfe.Ide.nNf = Convert.ToInt32(dr["nfd_int_nNf"]);
                    danfe.Ide.dhEmi = Convert.ToDateTime(dr["nfd_dt_dhEmi"]).ToString("dd/MM/yyyy HH:mm");

                    //danfe.nde_int_dest = Convert.ToInt32(dr["nde_int_dest"]);
                    //danfe.Total = new Assecontweb.Extend.Nfe.Danfe.ICMSTotModel();
                    //danfe.Total.vNF = Convert.ToDouble(dr["ndi_num_vProd"]);

                    danfe.Dest = new Assecontweb.Extend.Nfe.Danfe.DestModel();
                    danfe.Dest.CNPJ = dr["CNPJ"] is DBNull ? null : Convert.ToString(dr["CNPJ"]);
                    danfe.Dest.CPF = dr["CPF"] is DBNull ? null : Convert.ToString(dr["CPF"]);

                    danfes.Add(danfe);
                }
                
                //foreach (Assecontweb.Extend.Nfe.Danfe.NotaFiscal d in danfes)
                //{
                //    dr.Close();
                //    query = @"select (case when LEN(nde_des_cnpj) < 14 then nde_des_cnpj else null end) as CPF, 
	               //                     (case when LEN(nde_des_cnpj) = 14 then nde_des_cnpj else null end) as CNPJ 
                //                from SealNotaFiscalDanfeEmpresa where nde_int_id = @nde_int_id";

                //    cmd = new SqlCommand(query, conn);
                //    cmd.Parameters.AddWithValue("@nde_int_id", d.nde_int_dest);

                //    dr = cmd.ExecuteReader();

                //    Assecontweb.Extend.Nfe.Danfe.DestModel dest;
                //    while (dr.Read())
                //    {
                //        dest = new Assecontweb.Extend.Nfe.Danfe.DestModel();
                //        dest.CNPJ = dr["CNPJ"] is DBNull ? null : Convert.ToString(dr["CNPJ"]);
                //        dest.CPF = dr["CPF"] is DBNull ? null : Convert.ToString(dr["CPF"]);

                //        d.Dest = dest;
                //    }
                //}
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

        public bool GetServicosNaoProcessados(ref List<Assecontweb.Extend.Nfe.Servico.NotaFiscal> servicos, out string erro)
        {
            string query = "";
            bool retorno = false;
            erro = "OK";
            try
            {
                conn.Open();

                query = @"select a.nfs_int_id,a.nfs_int_numero,a.nfs_des_codverificacao,a.nfs_dt_emissao,a.nfs_int_naturezaop,a.nfs_int_regimetributacao,a.nfs_int_optantesimples,a.nfs_int_icentivcult
                                ,a.nfs_int_competencia,a.nfs_int_nfsesubst,a.nfs_des_outrasinformacoes,a.nfs_int_listaservico,a.nfs_int_cnae,a.nfs_int_tribmunici,a.nfs_des_dicriminacao,a.nfs_int_municiprestserv
                                ,a.nfs_num_vlrservico,a.nfs_num_vlrdeducoes,a.nfs_num_vlrpis,a.nfs_num_vlrcofins,a.nfs_num_vlrinss,a.nfs_num_vlrir,a.nfs_num_vlrcsll,a.nfs_num_issretido,a.nfs_num_outrasretencoes
                                ,a.nfs_num_basecalculo,a.nfs_num_aliquota,a.nfs_num_vlrliquidonfe,a.nfs_num_vlrissretido,a.nfs_num_vlriss,a.nfs_num_desccondicionado,a.nfs_num_descincondicionado
                                ,a.nfs_int_municipio,a.nfs_des_ufmunicipio,a.nse_int_prestador,
                                (case when LEN(nse_des_cnpj) < 14 then nse_des_cnpj else null end) as CPF, 
                                (case when LEN(nse_des_cnpj) = 14 then nse_des_cnpj else null end) as CNPJ 
                                from SealNotaFiscalServico a
                                left join SealNotaFiscalRecebida b on a.nfs_int_id = b.nfs_int_id 
                                inner join SealNotaFiscalServicoEmpresa c on a.nse_int_tomador = c.nse_int_id 
                                where b.nfr_int_id is null";

                cmd = new SqlCommand(query, conn);

                DbDataReader dr = cmd.ExecuteReader();

                Assecontweb.Extend.Nfe.Servico.NotaFiscal servico;
                while (dr.Read())
                {
                    servico = new Assecontweb.Extend.Nfe.Servico.NotaFiscal();
                    servico.nfd_int_id = Convert.ToInt32(dr["nfs_int_id"]);
                    servico.Numero = Convert.ToInt32(dr["nfs_int_numero"]);
                    servico.CodigoVerificacao = Convert.ToString(dr["nfs_des_codverificacao"]);
                    servico.DataEmissao = Convert.ToDateTime(dr["nfs_dt_emissao"]).ToString();
                    servico.NaturezaOperacao = Convert.ToInt32(dr["nfs_int_naturezaop"]);
                    servico.RegimeEspecialTributacao = Convert.ToInt32(dr["nfs_int_regimetributacao"]);
                    servico.OptanteSimplesNacional = Convert.ToInt32(dr["nfs_int_optantesimples"]);
                    servico.IncentivadorCultural = Convert.ToInt32(dr["nfs_int_icentivcult"]);
                    servico.Competencia = Convert.ToString(dr["nfs_int_competencia"]);
                    servico.NfseSubstituida = Convert.ToInt32(dr["nfs_int_nfsesubst"]);
                    servico.OutrasInformacoes = Convert.ToString(dr["nfs_des_outrasinformacoes"]);
                    servico.Servico = new Assecontweb.Extend.Nfe.Servico.ServicoModel();
                    servico.Servico.ItemListaServico = Convert.ToInt32(dr["nfs_int_listaservico"]);
                    servico.Servico.CodigoCnae = Convert.ToString(dr["nfs_int_cnae"]);
                    servico.Servico.CodigoTributacaoMunicipio = Convert.ToInt32(dr["nfs_int_tribmunici"]);
                    servico.Servico.Discriminacao = Convert.ToString(dr["nfs_des_dicriminacao"]);
                    servico.Servico.MunicipioPrestacaoServico = Convert.ToInt32(dr["nfs_int_municiprestserv"]);
                    servico.Servico.Valores = new Assecontweb.Extend.Nfe.Servico.ValoresModel();
                    servico.Servico.Valores.ValorServicos = Convert.ToDouble(dr["nfs_num_vlrservico"]);
                    servico.Servico.Valores.ValorDeducoes = Convert.ToDouble(dr["nfs_num_vlrdeducoes"]);
                    servico.Servico.Valores.ValorCofins = Convert.ToDouble(dr["nfs_num_vlrcofins"]);
                    servico.Servico.Valores.ValorInss = Convert.ToDouble(dr["nfs_num_vlrinss"]);
                    servico.Servico.Valores.ValorIr = Convert.ToDouble(dr["nfs_num_vlrir"]);
                    servico.Servico.Valores.ValorCsll = Convert.ToDouble(dr["nfs_num_vlrcsll"]);
                    servico.Servico.Valores.IssRetido = Convert.ToDouble(dr["nfs_num_issretido"]);
                    servico.Servico.Valores.OutrasRetencoes = Convert.ToDouble(dr["nfs_num_outrasretencoes"]);
                    servico.Servico.Valores.BaseCalculo = Convert.ToDouble(dr["nfs_num_basecalculo"]);
                    servico.Servico.Valores.Aliquota = Convert.ToDouble(dr["nfs_num_aliquota"]);
                    servico.Servico.Valores.ValorLiquidoNfse = Convert.ToDouble(dr["nfs_num_vlrliquidonfe"]);
                    servico.Servico.Valores.ValorIssRetido = Convert.ToDouble(dr["nfs_num_vlrissretido"]);
                    servico.Servico.Valores.ValorIss = Convert.ToDouble(dr["nfs_num_vlriss"]);
                    servico.Servico.Valores.DescontoCondicionado = Convert.ToDouble(dr["nfs_num_desccondicionado"]);
                    servico.Servico.Valores.DescontoIncondicionado = Convert.ToDouble(dr["nfs_num_descincondicionado"]);
                    servico.OrgaoGerador = new Assecontweb.Extend.Nfe.Servico.OrgaoGeradorModel();
                    servico.OrgaoGerador.CodigoMunicipio = Convert.ToString(dr["nfs_int_municipio"]);
                    servico.OrgaoGerador.Uf = Convert.ToString(dr["nfs_des_ufmunicipio"]);
                    //servico.nse_int_tomador = Convert.ToInt32(dr["nse_int_tomador"]);

                    servico.TomadorServico = new Assecontweb.Extend.Nfe.Servico.TomadorServicoModel();
                    servico.TomadorServico.IdentificadorTomador = new Assecontweb.Extend.Nfe.Servico.IdentificacaoModel();
                    servico.TomadorServico.IdentificadorTomador.CpfCnpj = new Assecontweb.Extend.Nfe.Servico.CpfCnpjModel();
                    servico.TomadorServico.IdentificadorTomador.CpfCnpj.Cnpj = dr["CNPJ"] is DBNull ? Convert.ToString(dr["CPF"]) : Convert.ToString(dr["CNPJ"]);

                    servicos.Add(servico);
                }

                //foreach (Assecontweb.Extend.Nfe.Servico.NotaFiscal s in servicos)
                //{
                //    dr.Close();
                //    query = @"select (case when LEN(nse_des_cnpj) < 14 then nse_des_cnpj else null end) as CPF, 
	               //                    (case when LEN(nse_des_cnpj) = 14 then nse_des_cnpj else null end) as CNPJ 
                //                from SealNotaFiscalServicoEmpresa where nse_int_id = @nse_int_id";

                //    cmd = new SqlCommand(query, conn);
                //    cmd.Parameters.AddWithValue("@nse_int_id", s.nse_int_tomador);

                //    dr = cmd.ExecuteReader();

                //    Assecontweb.Extend.Nfe.Servico.TomadorServicoModel tomador;
                //    while (dr.Read())
                //    {
                //        tomador = new Assecontweb.Extend.Nfe.Servico.TomadorServicoModel();
                //        tomador.IdentificadorTomador = new Assecontweb.Extend.Nfe.Servico.IdentificacaoModel();
                //        tomador.IdentificadorTomador.CpfCnpj = new Assecontweb.Extend.Nfe.Servico.CpfCnpjModel();
                //        tomador.IdentificadorTomador.CpfCnpj.Cnpj = dr["CNPJ"] is DBNull ? Convert.ToString(dr["CPF"]) : Convert.ToString(dr["CNPJ"]);

                //        s.TomadorServico = tomador;
                //    }
                //}
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

        public bool SetServicos(List<Assecontweb.Extend.Nfe.Servico.NotaFiscal> servicos, out string erro)
        {
            string query = "";
            bool retorno = false;
            int i;
            erro = "OK";
            try
            {
                List<Assecontweb.Extend.Nfe.Servico.NotaFiscal> servicosc = new List<Assecontweb.Extend.Nfe.Servico.NotaFiscal>();
                conn.Open();
                foreach (Assecontweb.Extend.Nfe.Servico.NotaFiscal servico in servicos)
                {
                    if (servico.Cancelada != "C")
                    {
                        #region Não Cancelada
                        int nse_int_tomador = 0;
                        #region Tomador
                        if (servico.TomadorServico != null)
                        {
                            try
                            {
                                query = @"declare @nse_int_id int
                              
                                    insert into SealNotaFiscalServicoEmpresa (nse_des_cnpj,nse_des_razaosocial,nse_des_rua,nse_des_numero,nse_des_bairro,nse_des_cidade
                                    ,nse_des_estado,nse_des_cep,nse_des_telefone,nse_des_email)
                                    values (@nse_des_cnpj,@nse_des_razaosocial,@nse_des_rua,@nse_des_numero,@nse_des_bairro,@nse_des_cidade,
                                    @nse_des_estado,@nse_des_cep,@nse_des_telefone,@nse_des_email)

                                    set @nse_int_id = (select IDENT_CURRENT('SealNotaFiscalServicoEmpresa'))
                                    select @nse_int_id";

                                cmd = new SqlCommand(query, conn);
                                cmd.Parameters.AddWithValue("@nse_des_cnpj", servico.TomadorServico.IdentificadorTomador.CpfCnpj.Cnpj.Trim());
                                cmd.Parameters.AddWithValue("@nse_des_razaosocial", servico.TomadorServico.RazaoSocial.Trim());
                                cmd.Parameters.AddWithValue("@nse_des_rua", servico.TomadorServico.Endereco.Endereco.Trim());
                                cmd.Parameters.AddWithValue("@nse_des_numero", servico.TomadorServico.Endereco.Numero.Trim());
                                cmd.Parameters.AddWithValue("@nse_des_bairro", servico.TomadorServico.Endereco.Bairro.Trim());
                                cmd.Parameters.AddWithValue("@nse_des_cidade", servico.TomadorServico.Endereco.Cidade.Trim());
                                cmd.Parameters.AddWithValue("@nse_des_estado", servico.TomadorServico.Endereco.Estado.Trim());
                                cmd.Parameters.AddWithValue("@nse_des_cep", servico.TomadorServico.Endereco.Cep.Trim());
                                cmd.Parameters.AddWithValue("@nse_des_telefone", servico.TomadorServico.Contato.Telefone.Trim());
                                cmd.Parameters.AddWithValue("@nse_des_email", servico.TomadorServico.Contato.Email.Trim());

                                nse_int_tomador = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                        #endregion
                        int nse_int_prestador = 0;
                        #region Prestador
                        if (servico.PrestadorServico != null &&
                            servico.PrestadorServico.IdentificacaoPrestador != null &&
                            servico.PrestadorServico.Endereco != null &&
                            servico.PrestadorServico.Contato != null)
                        {
                            try
                            {
                                query = @"declare @nse_int_id int
                              
                                    insert into SealNotaFiscalServicoEmpresa (nse_des_cnpj,nse_des_razaosocial,nse_des_rua,nse_des_numero,nse_des_bairro,nse_des_cidade
                                    ,nse_des_estado,nse_des_cep,nse_des_telefone,nse_des_email)
                                    values (@nse_des_cnpj,@nse_des_razaosocial,@nse_des_rua,@nse_des_numero,@nse_des_bairro,@nse_des_cidade,
                                    @nse_des_estado,@nse_des_cep,@nse_des_telefone,@nse_des_email)

                                    set @nse_int_id = (select IDENT_CURRENT('SealNotaFiscalServicoEmpresa'))
                                    select @nse_int_id";

                                cmd = new SqlCommand(query, conn);
                                cmd.Parameters.AddWithValue("@nse_des_cnpj", servico.PrestadorServico.IdentificacaoPrestador.CpfCnpj.Cnpj);
                                cmd.Parameters.AddWithValue("@nse_des_razaosocial", servico.PrestadorServico.RazaoSocial);
                                cmd.Parameters.AddWithValue("@nse_des_rua", servico.PrestadorServico.Endereco.Endereco);
                                cmd.Parameters.AddWithValue("@nse_des_numero", servico.PrestadorServico.Endereco.Numero);
                                cmd.Parameters.AddWithValue("@nse_des_bairro", servico.PrestadorServico.Endereco.Bairro);
                                cmd.Parameters.AddWithValue("@nse_des_cidade", servico.PrestadorServico.Endereco.Cidade);
                                cmd.Parameters.AddWithValue("@nse_des_estado", servico.PrestadorServico.Endereco.Estado);
                                cmd.Parameters.AddWithValue("@nse_des_cep", servico.PrestadorServico.Endereco.Cep);
                                cmd.Parameters.AddWithValue("@nse_des_telefone", servico.PrestadorServico.Contato.Telefone);
                                cmd.Parameters.AddWithValue("@nse_des_email", servico.PrestadorServico.Contato.Email);

                                nse_int_prestador = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                        #endregion
                        #region Nota Fiscal
                        int nfs_int_id = 0;
                        query = @"insert into SealNotaFiscalServico (nfs_int_numero,nfs_des_codverificacao,nfs_dt_emissao,nfs_int_naturezaop,nfs_int_regimetributacao,nfs_int_optantesimples,
                                nfs_int_icentivcult,nfs_int_competencia,nfs_int_nfsesubst,nfs_des_outrasinformacoes,nfs_int_listaservico,nfs_int_cnae,nfs_int_tribmunici,nfs_des_dicriminacao,
                                nfs_int_municiprestserv,nfs_num_vlrservico,nfs_num_vlrdeducoes,nfs_num_vlrpis,nfs_num_vlrcofins,nfs_num_vlrinss,nfs_num_vlrir,nfs_num_vlrcsll,
                                nfs_num_issretido,nfs_num_outrasretencoes,nfs_num_basecalculo,nfs_num_aliquota,nfs_num_vlrliquidonfe,nfs_num_vlrissretido,nfs_num_vlriss,
                                nfs_num_desccondicionado,nfs_num_descincondicionado,nfs_int_municipio,nfs_des_ufmunicipio,nse_int_prestador,nse_int_tomador)
                                values(@nfs_int_numero,@nfs_des_codverificacao,@nfs_dt_emissao,@nfs_int_naturezaop,@nfs_int_regimetributacao,@nfs_int_optantesimples,@nfs_int_icentivcult
                                ,@nfs_int_competencia,@nfs_int_nfsesubst,@nfs_des_outrasinformacoes,@nfs_int_listaservico,@nfs_int_cnae,@nfs_int_tribmunici,@nfs_des_dicriminacao
                                ,@nfs_int_municiprestserv,@nfs_num_vlrservico,@nfs_num_vlrdeducoes,@nfs_num_vlrpis,@nfs_num_vlrcofins,@nfs_num_vlrinss,@nfs_num_vlrir,@nfs_num_vlrcsll
                                ,@nfs_num_issretido,@nfs_num_outrasretencoes,@nfs_num_basecalculo,@nfs_num_aliquota,@nfs_num_vlrliquidonfe,@nfs_num_vlrissretido,@nfs_num_vlriss,@nfs_num_desccondicionado
                                ,@nfs_num_descincondicionado,@nfs_int_municipio,@nfs_des_ufmunicipio,@nse_int_prestador,@nse_int_tomador)
                                declare @nfs_int_id int = (select IDENT_CURRENT('SealNotaFiscalServico'))
                                select @nfs_int_id";

                        if (nse_int_prestador == 0)
                            query = query.Replace(",nse_int_prestador", "").Replace(",@nse_int_prestador", "");

                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@nfs_int_numero", servico.Numero);
                        cmd.Parameters.AddWithValue("@nfs_des_codverificacao", servico.CodigoVerificacao);
                        //2017 04 26 17 10 15
                        DateTime DataEmissao;
                        string data;
                        if (servico.DataEmissao.Contains("T"))
                        {
                            data = servico.DataEmissao.Replace(":", "").Replace("T", "").Replace("-", "");
                            string ano = data.Substring(0, 4);
                            string mes = data.Substring(4, 2);
                            string dia = data.Substring(6, 2);
                            string hr = data.Substring(8, 2);
                            string min = data.Substring(10, 2);
                            string seg = data.Substring(12, 2);
                            data = dia + "/" + mes + "/" + ano + " " + hr + ":" + min + ":" + seg;
                        }
                        else
                        {
                            string ano = servico.DataEmissao.Substring(0, 4);
                            string mes = servico.DataEmissao.Substring(4, 2);
                            string dia = servico.DataEmissao.Substring(6, 2);
                            string hr = servico.DataEmissao.Substring(8, 2);
                            string min = servico.DataEmissao.Substring(10, 2);
                            string seg = servico.DataEmissao.Substring(12, 2);
                            data = dia + "/" + mes + "/" + ano + " " + hr + ":" + min + ":" + seg;
                        }
                        DataEmissao = Convert.ToDateTime(data);
                        cmd.Parameters.AddWithValue("@nfs_dt_emissao", DataEmissao);
                        cmd.Parameters.AddWithValue("@nfs_int_naturezaop", servico.NaturezaOperacao);
                        cmd.Parameters.AddWithValue("@nfs_int_regimetributacao", servico.RegimeEspecialTributacao);
                        cmd.Parameters.AddWithValue("@nfs_int_optantesimples", servico.OptanteSimplesNacional);
                        cmd.Parameters.AddWithValue("@nfs_int_icentivcult", servico.IncentivadorCultural);
                        cmd.Parameters.AddWithValue("@nfs_int_competencia", servico.Competencia);
                        cmd.Parameters.AddWithValue("@nfs_int_nfsesubst", servico.NfseSubstituida);
                        cmd.Parameters.AddWithValue("@nfs_des_outrasinformacoes", servico.OutrasInformacoes);
                        cmd.Parameters.AddWithValue("@nfs_int_listaservico", servico.Servico.ItemListaServico);
                        cmd.Parameters.AddWithValue("@nfs_int_cnae", servico.Servico.CodigoCnae);
                        cmd.Parameters.AddWithValue("@nfs_int_tribmunici", servico.Servico.CodigoTributacaoMunicipio);
                        cmd.Parameters.AddWithValue("@nfs_des_dicriminacao", servico.Servico.Discriminacao);
                        cmd.Parameters.AddWithValue("@nfs_int_municiprestserv", servico.Servico.MunicipioPrestacaoServico);
                        cmd.Parameters.AddWithValue("@nfs_num_vlrservico", servico.Servico.Valores.ValorServicos);
                        cmd.Parameters.AddWithValue("@nfs_num_vlrdeducoes", servico.Servico.Valores.ValorDeducoes);
                        cmd.Parameters.AddWithValue("@nfs_num_vlrpis", servico.Servico.Valores.ValorPis);
                        cmd.Parameters.AddWithValue("@nfs_num_vlrcofins", servico.Servico.Valores.ValorCofins);
                        cmd.Parameters.AddWithValue("@nfs_num_vlrinss", servico.Servico.Valores.ValorInss);
                        cmd.Parameters.AddWithValue("@nfs_num_vlrir", servico.Servico.Valores.ValorIr);
                        cmd.Parameters.AddWithValue("@nfs_num_vlrcsll", servico.Servico.Valores.ValorCsll);
                        cmd.Parameters.AddWithValue("@nfs_num_issretido", servico.Servico.Valores.IssRetido);
                        cmd.Parameters.AddWithValue("@nfs_num_outrasretencoes", servico.Servico.Valores.OutrasRetencoes);
                        cmd.Parameters.AddWithValue("@nfs_num_basecalculo", servico.Servico.Valores.BaseCalculo);
                        cmd.Parameters.AddWithValue("@nfs_num_aliquota", servico.Servico.Valores.Aliquota);
                        cmd.Parameters.AddWithValue("@nfs_num_vlrliquidonfe", servico.Servico.Valores.ValorLiquidoNfse);
                        cmd.Parameters.AddWithValue("@nfs_num_vlrissretido", servico.Servico.Valores.ValorIssRetido);
                        cmd.Parameters.AddWithValue("@nfs_num_vlriss", servico.Servico.Valores.ValorIss);
                        cmd.Parameters.AddWithValue("@nfs_num_desccondicionado", servico.Servico.Valores.DescontoCondicionado);
                        cmd.Parameters.AddWithValue("@nfs_num_descincondicionado", servico.Servico.Valores.DescontoCondicionado);
                        cmd.Parameters.AddWithValue("@nfs_int_municipio", servico.OrgaoGerador.CodigoMunicipio);
                        cmd.Parameters.AddWithValue("@nfs_des_ufmunicipio", servico.OrgaoGerador.Uf);
                        if (nse_int_prestador != 0)
                            cmd.Parameters.AddWithValue("@nse_int_prestador", nse_int_prestador);
                        cmd.Parameters.AddWithValue("@nse_int_tomador", nse_int_tomador);

                        //string strRetorno = Convert.ToString(cmd.ExecuteScalar());
                        //string[] split = strRetorno.Split(new string[] { "-" }, StringSplitOptions.None);
                        nfs_int_id = Convert.ToInt32(cmd.ExecuteScalar());
                        retorno = (nfs_int_id != 0);
                        #endregion
                        #endregion
                    }
                    else
                        servicosc.Add(servico);
                }

                foreach (Assecontweb.Extend.Nfe.Servico.NotaFiscal servico in servicosc)
                {
                    if (servico.Cancelada == "C")
                    {
                        #region Cancelada
                        query = @"delete from SealNotaFiscalServico where nfs_int_numero = @nfs_int_numero";

                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@nfs_int_numero", servico.Numero);

                        i = Convert.ToInt32(cmd.ExecuteNonQuery());
                        #endregion
                    }
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

        public bool GetFechamentoDetail(DateTime inicio, DateTime fim, ref Detail detail, out string erro)
        {
            string query = "";
            bool retorno = true;
            erro = "OK";
            try
            {
                conn.Open();
                #region Query
                query = @"declare @inicio datetime = @dt_inicio, 
		                          @fim datetime = @dt_fim
                            select --MSMSMSMSMSMSMSMS
	                                --Danfes Faturadas
	                                sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE' or f.cli_des_grupo = 'NÃO CONTRIBUINTE'))
				                            then a.ndi_num_vProd else 0 end) as MSDanfePrivado, 
	                                sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIB - Lei 9718' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718'))
				                            then a.ndi_num_vProd else 0 end) as MSDanfeLei, 
	                                sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) 
						                            and (f.cli_int_id is not null)
				                            then a.ndi_num_vProd else 0 end) as MSDanfeFatu, 
	                                --Recebidas
	                                sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE' or f.cli_des_grupo = 'NÃO CONTRIBUINTE')) and
					                                (e.nfr_int_id is not null)
				                            then a.ndi_num_vProd else 0 end) as MSDanfePrivadoRec, 
	                                sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIB - Lei 9718' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718')) and
					                                (e.nfr_int_id is not null)
				                            then a.ndi_num_vProd else 0 end) as MSDanfeLeiRec, 
	                                sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null) and (e.nfr_int_id is not null)
				                            then a.ndi_num_vProd else 0 end) as MSDanfeFatuRec, 
	                                --Não Recebidas
	                                sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE' or f.cli_des_grupo = 'NÃO CONTRIBUINTE')) and 
					                                (e.nfr_int_id is null)
				                            then a.ndi_num_vProd else 0 end) as MSDanfePrivadoNRec, 
	                                sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIB - Lei 9718' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718')) and 
					                                (e.nfr_int_id is null)
				                            then a.ndi_num_vProd else 0 end) as MSDanfeLeiNRec, 
	                                sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null) and (e.nfr_int_id is null)
				                            then a.ndi_num_vProd else 0 end) as MSDanfeFatuNRec, 
	                                --Contribuinte
	                                sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE')) 
				                            then a.ndi_num_vProd else 0 end) as MSDanfePrivadoContri, 
	                                sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIB - Lei 9718')) 
				                            then a.ndi_num_vProd else 0 end) as MSDanfeLeiContri, 
	                                sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE' or f.cli_des_grupo = 'CONTRIB - Lei 9718')) 
				                            then a.ndi_num_vProd else 0 end) as MSDanfeFatuContri, 
	                                --Não Contribuinte
	                                sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'NÃO CONTRIBUINTE')) 
				                            then a.ndi_num_vProd else 0 end) as MSDanfePrivadoNContri, 
	                                sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718')) 
				                            then a.ndi_num_vProd else 0 end) as MSDanfeLeiNContri, 
	                                sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'NÃO CONTRIBUINTE' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718')) 
				                            then a.ndi_num_vProd else 0 end) as MSDanfeFatuNContri, 
	                                --SPSPSPSPSPSPSPSP
	                                --Danfes Faturadas
	                                sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE' or f.cli_des_grupo = 'NÃO CONTRIBUINTE'))
				                            then a.ndi_num_vProd else 0 end) as  SPDanfePrivado, 
	                                sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIB - Lei 9718' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718'))
				                            then a.ndi_num_vProd else 0 end) as  SPDanfeLei, 
	                                sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) 
					                                and (f.cli_int_id is not null)  
				                            then a.ndi_num_vProd else 0 end) as  SPDanfeFatu, 
	                                --Recebidas
	                                sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE' or f.cli_des_grupo = 'NÃO CONTRIBUINTE')) and 
					                                (e.nfr_int_id is not null)
				                            then a.ndi_num_vProd else 0 end) as  SPDanfePrivadoRec, 
	                                sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIB - Lei 9718' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718')) and
					                                (e.nfr_int_id is not null)
				                            then a.ndi_num_vProd else 0 end) as  SPDanfeLeiRec, 
	                                sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) 
					                                and (f.cli_int_id is not null) and (e.nfr_int_id is not null)
				                            then a.ndi_num_vProd else 0 end) as  SPDanfeFatuRec, 
	                                --Não Recebidas
	                                sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE' or f.cli_des_grupo = 'NÃO CONTRIBUINTE')) and 
					                                (e.nfr_int_id is null) 
				                            then a.ndi_num_vProd else 0 end) as  SPDanfePrivadoNRec, 
	                                sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIB - Lei 9718' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718')) and
					                                (e.nfr_int_id is null)
				                            then a.ndi_num_vProd else 0 end) as  SPDanfeLeiNRec, 
	                                sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null) and (e.nfr_int_id is null)
				                            then a.ndi_num_vProd else 0 end) as  SPDanfeFatuNRec, 
	                                --Contribuinte
	                                sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE'))
				                            then a.ndi_num_vProd else 0 end) as  SPDanfePrivadoContri, 
	                                sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIB - Lei 9718')) 
				                            then a.ndi_num_vProd else 0 end) as  SPDanfeLeiContri, 
	                                sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE' or f.cli_des_grupo = 'CONTRIB - Lei 9718')) 
				                            then a.ndi_num_vProd else 0 end) as  SPDanfeFatuContri, 
	                                --Não Contribuinte
	                                sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'NÃO CONTRIBUINTE')) 
				                            then a.ndi_num_vProd else 0 end) as  SPDanfePrivadoNContri, 
	                                sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718')) 
				                            then a.ndi_num_vProd else 0 end) as  SPDanfeLeiNContri, 
	                                sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (f.cli_int_id is not null and (f.cli_des_grupo = 'NÃO CONTRIBUINTE' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718')) 
				                            then a.ndi_num_vProd else 0 end) as  SPDanfeFatuNContri
                            from SealNotaFiscalDanfeItem a
                            inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
                            left join SealNotaFiscalDanfeEmpresa c on b.nde_int_emit = c.nde_int_id
                            left join SealNotaFiscalDanfeEmpresa d on b.nde_int_dest = d.nde_int_id
                            outer apply(
	                            select top 1 nfr_int_id, nfd_int_id, nfs_int_id from SealNotaFiscalRecebida 
	                            where nfd_int_id = b.nfd_int_id
                            ) e
                            left join SealCliente f on d.nde_des_cnpj = f.cli_des_cnpj
		                        ";
                #endregion
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dt_inicio", inicio);
                cmd.Parameters.AddWithValue("@dt_fim", fim);

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    #region Getting Values 
                    //MSMSMSMSMSMSMS
                    detail.DanfeFaturada.MSPrivado = Convert.ToDouble(dr["MSDanfePrivado"] is DBNull ? 0 : dr["MSDanfePrivado"]);
                    detail.DanfeFaturada.MSLei = Convert.ToDouble(dr["MSDanfeLei"] is DBNull ? 0 : dr["MSDanfeLei"]);
                    detail.DanfeFaturada.MSFatu = Convert.ToDouble(dr["MSDanfeFatu"] is DBNull ? 0 : dr["MSDanfeFatu"]);

                    detail.DanfeFaturada.Recebidas.MSPrivado = Convert.ToDouble(dr["MSDanfePrivadoRec"] is DBNull ? 0 : dr["MSDanfePrivadoRec"]);
                    detail.DanfeFaturada.Recebidas.MSLei = Convert.ToDouble(dr["MSDanfeLeiRec"] is DBNull ? 0 : dr["MSDanfeLeiRec"]);
                    detail.DanfeFaturada.Recebidas.MSFatu = Convert.ToDouble(dr["MSDanfeFatuRec"] is DBNull ? 0 : dr["MSDanfeFatuRec"]);

                    detail.DanfeFaturada.NRecebidas.MSPrivado = Convert.ToDouble(dr["MSDanfePrivadoNRec"] is DBNull ? 0 : dr["MSDanfePrivadoNRec"]);
                    detail.DanfeFaturada.NRecebidas.MSLei = Convert.ToDouble(dr["MSDanfeLeiNRec"] is DBNull ? 0 : dr["MSDanfeLeiNRec"]);
                    detail.DanfeFaturada.NRecebidas.MSFatu = Convert.ToDouble(dr["MSDanfeFatuNRec"] is DBNull ? 0 : dr["MSDanfeFatuNRec"]);

                    detail.DanfeFaturada.Contribuinte.MSPrivado = Convert.ToDouble(dr["MSDanfePrivadoContri"] is DBNull ? 0 : dr["MSDanfePrivadoContri"]);
                    detail.DanfeFaturada.Contribuinte.MSLei = Convert.ToDouble(dr["MSDanfeLeiContri"] is DBNull ? 0 : dr["MSDanfeLeiContri"]);
                    detail.DanfeFaturada.Contribuinte.MSFatu = Convert.ToDouble(dr["MSDanfeFatuContri"] is DBNull ? 0 : dr["MSDanfeFatuContri"]);

                    detail.DanfeFaturada.NContribuinte.MSPrivado = Convert.ToDouble(dr["MSDanfePrivadoNContri"] is DBNull ? 0 : dr["MSDanfePrivadoNContri"]);
                    detail.DanfeFaturada.NContribuinte.MSLei = Convert.ToDouble(dr["MSDanfeLeiNContri"] is DBNull ? 0 : dr["MSDanfeLeiNContri"]);
                    detail.DanfeFaturada.NContribuinte.MSFatu = Convert.ToDouble(dr["MSDanfeFatuNContri"] is DBNull ? 0 : dr["MSDanfeFatuNContri"]);

                    //SPSPSPSPSPSPSP
                    detail.DanfeFaturada.SPPrivado = Convert.ToDouble(dr["SPDanfePrivado"] is DBNull ? 0 : dr["SPDanfePrivado"]);
                    detail.DanfeFaturada.SPLei = Convert.ToDouble(dr["SPDanfeLei"] is DBNull ? 0 : dr["SPDanfeLei"]);
                    detail.DanfeFaturada.SPFatu = Convert.ToDouble(dr["SPDanfeFatu"] is DBNull ? 0 : dr["SPDanfeFatu"]);

                    detail.DanfeFaturada.Recebidas.SPPrivado = Convert.ToDouble(dr["SPDanfePrivadoRec"] is DBNull ? 0 : dr["SPDanfePrivadoRec"]);
                    detail.DanfeFaturada.Recebidas.SPLei = Convert.ToDouble(dr["SPDanfeLeiRec"] is DBNull ? 0 : dr["SPDanfeLeiRec"]);
                    detail.DanfeFaturada.Recebidas.SPFatu = Convert.ToDouble(dr["SPDanfeFatuRec"] is DBNull ? 0 : dr["SPDanfeFatuRec"]);

                    detail.DanfeFaturada.NRecebidas.SPPrivado = Convert.ToDouble(dr["SPDanfePrivadoNRec"] is DBNull ? 0 : dr["SPDanfePrivadoNRec"]);
                    detail.DanfeFaturada.NRecebidas.SPLei = Convert.ToDouble(dr["SPDanfeLeiNRec"] is DBNull ? 0 : dr["SPDanfeLeiNRec"]);
                    detail.DanfeFaturada.NRecebidas.SPFatu = Convert.ToDouble(dr["SPDanfeFatuNRec"] is DBNull ? 0 : dr["SPDanfeFatuNRec"]);

                    detail.DanfeFaturada.Contribuinte.SPPrivado = Convert.ToDouble(dr["SPDanfePrivadoContri"] is DBNull ? 0 : dr["SPDanfePrivadoContri"]);
                    detail.DanfeFaturada.Contribuinte.SPLei = Convert.ToDouble(dr["SPDanfeLeiContri"] is DBNull ? 0 : dr["SPDanfeLeiContri"]);
                    detail.DanfeFaturada.Contribuinte.SPFatu = Convert.ToDouble(dr["SPDanfeFatuContri"] is DBNull ? 0 : dr["SPDanfeFatuContri"]);

                    detail.DanfeFaturada.NContribuinte.SPPrivado = Convert.ToDouble(dr["SPDanfePrivadoNContri"] is DBNull ? 0 : dr["SPDanfePrivadoNContri"]);
                    detail.DanfeFaturada.NContribuinte.SPLei = Convert.ToDouble(dr["SPDanfeLeiNContri"] is DBNull ? 0 : dr["SPDanfeLeiNContri"]);
                    detail.DanfeFaturada.NContribuinte.SPFatu = Convert.ToDouble(dr["SPDanfeFatuNContri"] is DBNull ? 0 : dr["SPDanfeFatuNContri"]);
                    #endregion
                }

                dr.Close();
                #region Query
                query = @"declare @inicio datetime = @dt_inicio, 
		                          @fim datetime = @dt_fim
                            select --MSMSMSMSMSMSMSMS
	                                --Servicos Faturados
	                                SUM(case when (b.nse_des_estado = 'MS' and b.nse_int_id is not null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE')))
			                                then a.nfs_num_vlrservico else 0 end) as MSServicoPrivado, 
	                                SUM(case when (b.nse_des_estado = 'MS' and b.nse_int_id is not null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) 
			                                then a.nfs_num_vlrservico else 0 end) as MSServicoLei, 
	                                SUM(case when (b.nse_des_estado = 'MS' and b.nse_int_id is not null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null) 
			                                then a.nfs_num_vlrservico else 0 end) as MSServicoFatu, 
	                                --Recebidos
	                                SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) and 
					                                (d.nfr_int_id is not null)
			                                then a.nfs_num_vlrservico else 0 end) as MSServicoPrivadoRec, 
	                                SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and 
					                                (d.nfr_int_id is not null)
			                                then a.nfs_num_vlrservico else 0 end) as MSServicoLeiRec, 
	                                SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (d.nfr_int_id is not null) and (e.cli_int_id is not null) 
			                                then a.nfs_num_vlrservico else 0 end) as MSServicoFatuRec, 
	                                --Não Recebidos
	                                SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) and 
					                                (d.nfr_int_id is null)
			                                then a.nfs_num_vlrservico else 0 end) as MSServicoPrivadoNRec, 
	                                SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and 
					                                (d.nfr_int_id is null)
			                                then a.nfs_num_vlrservico else 0 end) as MSServicoLeiNRec, 
	                                SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (d.nfr_int_id is null) and (e.cli_int_id is not null) 
			                                then a.nfs_num_vlrservico else 0 end) as MSServicoFatuNRec, 
	                                --Cumulativos
	                                SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) and 
					                                (a.nfs_int_listaservico in (702, 103, 105, 107, 104, 106))  
			                                then a.nfs_num_vlrservico else 0 end) as MSServicoPrivadoCum, 
	                                SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and 
					                                (a.nfs_int_listaservico in (702, 103, 105, 107, 104, 106)) 
			                                then a.nfs_num_vlrservico else 0 end) as MSServicoLeiCum, 
	                                SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (a.nfs_int_listaservico in (702, 103, 105, 107, 104, 106)) and 
					                                (e.cli_int_id is not null) 
			                                then a.nfs_num_vlrservico else 0 end) as MSServicoFatuCum, 
	                                --Não Cumulativos
	                                SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) and 
					                                (a.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703))
			                                then a.nfs_num_vlrservico else 0 end) as MSServicoPrivadoNCum, 
	                                SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and 
					                                (a.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703))
			                                then a.nfs_num_vlrservico else 0 end) as MSServicoLeiNCum, 
	                                SUM(case when (a.nfs_des_ufmunicipio = 'MS' and b.nse_int_id is not null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (a.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703)) and 
					                                (e.cli_int_id is not null) 
			                                then a.nfs_num_vlrservico else 0 end) as MSServicoFatuNCum, 
	                                --SPSPSPSPSPSPSPSP
	                                --Servicos Faturados
	                                SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE')))  
			                                then a.nfs_num_vlrservico else 0 end) as SPServicoPrivado, 
	                                SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718')))  
			                                then a.nfs_num_vlrservico else 0 end) as SPServicoLei, 
	                                SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null) 
			                                then a.nfs_num_vlrservico else 0 end) as SPServicoFatu, 
	                                --Recebidos
	                                SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) and 
					                                (d.nfr_int_id is not null)
			                                then a.nfs_num_vlrservico else 0 end) as SPServicoPrivadoRec, 
	                                SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and 
					                                (d.nfr_int_id is not null)
			                                then a.nfs_num_vlrservico else 0 end) as SPServicoLeiRec, 
	                                SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (d.nfr_int_id is not null) and (e.cli_int_id is not null) 
			                                then a.nfs_num_vlrservico else 0 end) as SPServicoFatuRec, 
	                                --Não Recebidos
	                                SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) and 
					                                (d.nfr_int_id is null)
			                                then a.nfs_num_vlrservico else 0 end) as SPServicoPrivadoNRec, 
	                                SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and 
					                                (d.nfr_int_id is null)
			                                then a.nfs_num_vlrservico else 0 end) as SPServicoLeiNRec, 
	                                SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (d.nfr_int_id is null) and (e.cli_int_id is not null) 
			                                then a.nfs_num_vlrservico else 0 end) as SPServicoFatuNRec, 
	                                --Cumulativos
	                                SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) and 
					                                (a.nfs_int_listaservico in (1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879))  
			                                then a.nfs_num_vlrservico else 0 end) as SPServicoPrivadoCum, 
	                                SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and 
					                                (a.nfs_int_listaservico in (1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879))  
			                                then a.nfs_num_vlrservico else 0 end) as SPServicoLeiCum, 
	                                SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (a.nfs_int_listaservico in (1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879)) AND 
					                                (e.cli_int_id is not null) 
			                                then a.nfs_num_vlrservico else 0 end) as SPServicoFatuCum, 
	                                --Não Cumulativos
	                                SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE'))) and 
					                                (a.nfs_int_listaservico in (01880, 02119, 02135, 02186, 02194, 03093, 03115, 03450, 03468, 05991, 
													                            06009, 07285, 07315, 07323, 07331, 07366, 07390, 07412, 07439, 07447, 
													                            07455, 07471, 07498, 02496))
			                                then a.nfs_num_vlrservico else 0 end) as SPServicoPrivadoNCum, 
	                                SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and 
					                                (a.nfs_int_listaservico in (01880, 02119, 02135, 02186, 02194, 03093, 03115, 03450, 03468, 05991, 
													                            06009, 07285, 07315, 07323, 07331, 07366, 07390, 07412, 07439, 07447, 
													                            07455, 07471, 07498, 02496))
			                                then a.nfs_num_vlrservico else 0 end) as SPServicoLeiNCum, 
	                                SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
					                                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                                (a.nfs_int_listaservico in (01880, 02119, 02135, 02186, 02194, 03093, 03115, 03450, 03468, 05991, 
													                            06009, 07285, 07315, 07323, 07331, 07366, 07390, 07412, 07439, 07447, 
													                            07455, 07471, 07498, 02496)) and 
					                                (e.cli_int_id is not null) 
			                                then a.nfs_num_vlrservico else 0 end) as SPServicoFatuNCum
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
		                        ";
                #endregion
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dt_inicio", inicio);
                cmd.Parameters.AddWithValue("@dt_fim", fim);

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    #region Getting Values 
                    //MSMSMSMSMSMSMS
                    detail.ServicoFaturado.MSPrivado = Convert.ToDouble(dr["MSServicoPrivado"] is DBNull ? 0 : dr["MSServicoPrivado"]);
                    detail.ServicoFaturado.MSLei = Convert.ToDouble(dr["MSServicoLei"] is DBNull ? 0 : dr["MSServicoLei"]);
                    detail.ServicoFaturado.MSFatu = Convert.ToDouble(dr["MSServicoFatu"] is DBNull ? 0 : dr["MSServicoFatu"]);

                    detail.ServicoFaturado.Recebidas.MSPrivado = Convert.ToDouble(dr["MSServicoPrivadoRec"] is DBNull ? 0 : dr["MSServicoPrivadoRec"]);
                    detail.ServicoFaturado.Recebidas.MSLei = Convert.ToDouble(dr["MSServicoLeiRec"] is DBNull ? 0 : dr["MSServicoLeiRec"]);
                    detail.ServicoFaturado.Recebidas.MSFatu = Convert.ToDouble(dr["MSServicoFatuRec"] is DBNull ? 0 : dr["MSServicoFatuRec"]);

                    detail.ServicoFaturado.NRecebidas.MSPrivado = Convert.ToDouble(dr["MSServicoPrivadoNRec"] is DBNull ? 0 : dr["MSServicoPrivadoNRec"]);
                    detail.ServicoFaturado.NRecebidas.MSLei = Convert.ToDouble(dr["MSServicoLeiNRec"] is DBNull ? 0 : dr["MSServicoLeiNRec"]);
                    detail.ServicoFaturado.NRecebidas.MSFatu = Convert.ToDouble(dr["MSServicoFatuNRec"] is DBNull ? 0 : dr["MSServicoFatuNRec"]);

                    detail.ServicoFaturado.Cumulativo.MSPrivado = Convert.ToDouble(dr["MSServicoPrivadoCum"] is DBNull ? 0 : dr["MSServicoPrivadoCum"]);
                    detail.ServicoFaturado.Cumulativo.MSLei = Convert.ToDouble(dr["MSServicoLeiCum"] is DBNull ? 0 : dr["MSServicoLeiCum"]);
                    detail.ServicoFaturado.Cumulativo.MSFatu = Convert.ToDouble(dr["MSServicoFatuCum"] is DBNull ? 0 : dr["MSServicoFatuCum"]);

                    detail.ServicoFaturado.NCumulativo.MSPrivado = Convert.ToDouble(dr["MSServicoPrivadoNCum"] is DBNull ? 0 : dr["MSServicoPrivadoNCum"]);
                    detail.ServicoFaturado.NCumulativo.MSLei = Convert.ToDouble(dr["MSServicoLeiNCum"] is DBNull ? 0 : dr["MSServicoLeiNCum"]);
                    detail.ServicoFaturado.NCumulativo.MSFatu = Convert.ToDouble(dr["MSServicoFatuNCum"] is DBNull ? 0 : dr["MSServicoFatuNCum"]);

                    //SPSPSPSPSPSPSP
                    detail.ServicoFaturado.SPPrivado = Convert.ToDouble(dr["SPServicoPrivado"] is DBNull ? 0 : dr["SPServicoPrivado"]);
                    detail.ServicoFaturado.SPLei = Convert.ToDouble(dr["SPServicoLei"] is DBNull ? 0 : dr["SPServicoLei"]);
                    detail.ServicoFaturado.SPFatu = Convert.ToDouble(dr["SPServicoFatu"] is DBNull ? 0 : dr["SPServicoFatu"]);

                    detail.ServicoFaturado.Recebidas.SPPrivado = Convert.ToDouble(dr["SPServicoPrivadoRec"] is DBNull ? 0 : dr["SPServicoPrivadoRec"]);
                    detail.ServicoFaturado.Recebidas.SPLei = Convert.ToDouble(dr["SPServicoLeiRec"] is DBNull ? 0 : dr["SPServicoLeiRec"]);
                    detail.ServicoFaturado.Recebidas.SPFatu = Convert.ToDouble(dr["SPServicoFatuRec"] is DBNull ? 0 : dr["SPServicoFatuRec"]);

                    detail.ServicoFaturado.NRecebidas.SPPrivado = Convert.ToDouble(dr["SPServicoPrivadoNRec"] is DBNull ? 0 : dr["SPServicoPrivadoNRec"]);
                    detail.ServicoFaturado.NRecebidas.SPLei = Convert.ToDouble(dr["SPServicoLeiNRec"] is DBNull ? 0 : dr["SPServicoLeiNRec"]);
                    detail.ServicoFaturado.NRecebidas.SPFatu = Convert.ToDouble(dr["SPServicoFatuNRec"] is DBNull ? 0 : dr["SPServicoFatuNRec"]);

                    detail.ServicoFaturado.Cumulativo.SPPrivado = Convert.ToDouble(dr["SPServicoPrivadoCum"] is DBNull ? 0 : dr["SPServicoPrivadoCum"]);
                    detail.ServicoFaturado.Cumulativo.SPLei = Convert.ToDouble(dr["SPServicoLeiCum"] is DBNull ? 0 : dr["SPServicoLeiCum"]);
                    detail.ServicoFaturado.Cumulativo.SPFatu = Convert.ToDouble(dr["SPServicoFatuCum"] is DBNull ? 0 : dr["SPServicoFatuCum"]);

                    detail.ServicoFaturado.NCumulativo.SPPrivado = Convert.ToDouble(dr["SPServicoPrivadoNCum"] is DBNull ? 0 : dr["SPServicoPrivadoNCum"]);
                    detail.ServicoFaturado.NCumulativo.SPLei = Convert.ToDouble(dr["SPServicoLeiNCum"] is DBNull ? 0 : dr["SPServicoLeiNCum"]);
                    detail.ServicoFaturado.NCumulativo.SPFatu = Convert.ToDouble(dr["SPServicoFatuNCum"] is DBNull ? 0 : dr["SPServicoFatuNCum"]);
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
                dr.Close();
                conn.Close();
            }
            return retorno;
        }

        public bool GetFechamentoConsolidado(DateTime inicio, DateTime fim, ref Consolidado conso, out string erro)
        {
            string query = "";
            bool retorno = true;
            erro = "OK";
            try
            {
                #region Danfes Não Cumulativas
                conn.Open();
                #region Query Normais E Exclusão
                query = @"declare @inicio datetime = @dt_inicio, 
		                          @fim datetime = @dt_fim
                            select  --Danfes Não Cumulativo
		                            --(+) Normais
	                                sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) 
					                                and (e.cli_int_id is not null)  
				                            then a.ndi_num_vProd else 0 end) as  DanfeSPNormais, 
	                                sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) 
					                                and (e.cli_int_id is not null)
				                            then a.ndi_num_vProd else 0 end) as DanfeMSNormais, 
	                                sum(case when (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) 
					                                and (e.cli_int_id is not null)
				                            then a.ndi_num_vProd else 0 end) as DanfeTotNormais, 
		                            --(-) Exclusão
	                                sum(case when c.nde_des_uf = 'SP' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and 
					                                (f.nfd_int_id is null or f.crb_dt_recebimento > @fim)
				                            then a.ndi_num_vProd else 0 end) as DanfeSPExclusao, 
	                                sum(case when c.nde_des_uf = 'MS' and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and 
					                                (f.nfd_int_id is null or f.crb_dt_recebimento > @fim)
				                            then a.ndi_num_vProd else 0 end) as DanfeMSExclusao, 
	                                sum(case when (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
					                                a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
					                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and 
					                                (f.nfd_int_id is null or f.crb_dt_recebimento > @fim)
				                            then a.ndi_num_vProd else 0 end) as DanfeTotExclusao 
                            from SealNotaFiscalDanfeItem a
                            inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
                            left join SealNotaFiscalDanfeEmpresa c on b.nde_int_emit = c.nde_int_id
                            left join SealNotaFiscalDanfeEmpresa d on b.nde_int_dest = d.nde_int_id
                            left join SealCliente e on d.nde_des_cnpj = e.cli_des_cnpj
                            outer apply(
	                            select aa.nfd_int_id, ab.crb_dt_recebimento 
	                            from SealNotaFiscalRecebida aa 
	                            inner join SealContaReceber ab on aa.nfr_int_id = ab.nfr_int_id 
	                            where b.nfd_int_id = aa.nfd_int_id and (ab.crb_dt_recebimento < @fim)
	                            group by aa.nfd_int_id, ab.crb_dt_recebimento 
                            ) f 
		                        ";
                #endregion
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dt_inicio", inicio);
                cmd.Parameters.AddWithValue("@dt_fim", fim);

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    #region Getting Values 
                    //Danfes Não Cumulativos
                    conso.DanfeConsolidado.Normais.SP = Convert.ToDouble(dr["DanfeSPNormais"] is DBNull ? 0 : dr["DanfeSPNormais"]);
                    conso.DanfeConsolidado.Normais.MS = Convert.ToDouble(dr["DanfeMSNormais"] is DBNull ? 0 : dr["DanfeMSNormais"]);
                    conso.DanfeConsolidado.Normais.Tot = Convert.ToDouble(dr["DanfeTotNormais"] is DBNull ? 0 : dr["DanfeTotNormais"]);

                    conso.DanfeConsolidado.Exclusao.SP = Convert.ToDouble(dr["DanfeSPExclusao"] is DBNull ? 0 : dr["DanfeSPExclusao"]);
                    conso.DanfeConsolidado.Exclusao.MS = Convert.ToDouble(dr["DanfeMSExclusao"] is DBNull ? 0 : dr["DanfeMSExclusao"]);
                    conso.DanfeConsolidado.Exclusao.Tot = Convert.ToDouble(dr["DanfeTotExclusao"] is DBNull ? 0 : dr["DanfeTotExclusao"]);
                    #endregion
                }

                dr.Close();
                #region Query Adição
                query = @"declare @inicio datetime = @dt_inicio, 
		                          @fim datetime = @dt_fim
                            select  --Danfes Não Cumulativo
	                                --(+) Adição Receita Diferida
	                                sum(case when e.nde_des_uf = 'SP' 
				                            then a.crb_num_valorrecebido else 0 end) as DanfeSPAdicao, 
	                                sum(case when LTRIM(RTRIM(e.nde_des_uf)) = 'MS' 
				                            then a.crb_num_valorrecebido else 0 end) as DanfeMSAdicao, 
	                                sum(a.crb_num_valorrecebido) as DanfeTotAdicao
                            from SealContaReceber a 
                            left join SealNotaFiscalRecebida b on a.nfr_int_id = b.nfr_int_id 
                            left join SealNotaFiscalDanfe c on b.nfd_int_id = c.nfd_int_id
                            left join SealNotaFiscalDanfeEmpresa e on c.nde_int_emit = e.nde_int_id
                            left join SealNotaFiscalDanfeEmpresa f on c.nde_int_dest = f.nde_int_id
                            left join SealCliente g on f.nde_des_cnpj = g.cli_des_cnpj
                            where (a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim) and 
                            (g.cli_int_id is not null and (g.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718')))
		                        ";
                #endregion
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dt_inicio", inicio);
                cmd.Parameters.AddWithValue("@dt_fim", fim);

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    #region Getting Values 
                    //Danfes Não Cumulativos
                    conso.DanfeConsolidado.Adicao.SP = Convert.ToDouble(dr["DanfeSPAdicao"] is DBNull ? 0 : dr["DanfeSPAdicao"]);
                    conso.DanfeConsolidado.Adicao.MS = Convert.ToDouble(dr["DanfeMSAdicao"] is DBNull ? 0 : dr["DanfeMSAdicao"]);
                    conso.DanfeConsolidado.Adicao.Tot = Convert.ToDouble(dr["DanfeTotAdicao"] is DBNull ? 0 : dr["DanfeTotAdicao"]);
                    #endregion
                }

                dr.Close();
                #endregion

                #region Serviços 
                #region Query Normais 
                #region Comments
                //query = @"declare @inicio datetime = @dt_inicio, 
                //            @fim datetime = @dt_fim
                //            select  --Serviços Não Cumulativo
                //              --(+) Normais
                //                 SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
                //                     (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
                //                     (e.cli_int_id is not null) and 
                //                     (a.nfs_int_listaservico in (01880, 02119, 02135, 02186, 02194, 03093, 
                //                     03115, 03450, 03468, 05991, 06009, 07285, 07315, 07323, 07331, 07366, 
                //                     07390, 07412, 07439, 07447, 07455, 07471, 07498, 02496))
                //                   then a.nfs_num_vlrservico else 0 end) as NCumSPNormais, 
                //                 SUM(case when (b.nse_des_estado = 'MS' and b.nse_int_id is not null) and 
                //                     (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
                //                     (e.cli_int_id is not null) and 
                //                     (a.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703))
                //                   then a.nfs_num_vlrservico else 0 end) as NCumMSNormais, 
                //                 SUM(case when (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
                //                     (e.cli_int_id is not null) and 
                //                     (a.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703, 
                //                     01880, 02119, 02135, 02186, 02194, 03093, 03115, 03450, 03468, 05991, 
                //                     06009, 07285, 07315, 07323, 07331, 07366, 07390, 07412, 07439, 07447, 
                //                     07455, 07471, 07498, 02496))
                //                   then a.nfs_num_vlrservico else 0 end) as NCumTotNormais, 
                //                 --Serviços Cumulativo
                //              --(+) Normais
                //                 SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
                //                     (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
                //                     (e.cli_int_id is not null) and 
                //                     (a.nfs_int_listaservico in (1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879))
                //                   then a.nfs_num_vlrservico else 0 end) as CumSPNormais, 
                //                 SUM(case when (b.nse_des_estado = 'MS' and b.nse_int_id is not null) and 
                //                     (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
                //                     (e.cli_int_id is not null) and 
                //                     (a.nfs_int_listaservico in (702, 103, 105, 107, 104, 106))
                //                   then a.nfs_num_vlrservico else 0 end) as CumMSNormais, 
                //                 SUM(case when (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
                //                     (e.cli_int_id is not null) and 
                //                     (a.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1015, 
                //                     1023, 2682, 2798, 2917, 2918, 2690, 2879))
                //                   then a.nfs_num_vlrservico else 0 end) as CumTotNormais
                //            from SealNotaFiscalServico a
                //            left join SealNotaFiscalServicoEmpresa b on a.nse_int_prestador = b.nse_int_id 
                //            left join SealNotaFiscalServicoEmpresa c on a.nse_int_tomador = c.nse_int_id 
                //            left join SealCliente e on c.nse_des_cnpj = e.cli_des_cnpj
                //            outer apply(
                //             select aa.nfs_int_id, ab.crb_dt_recebimento 
                //             from SealNotaFiscalRecebida aa 
                //             inner join SealContaReceber ab on aa.nfr_int_id = ab.nfr_int_id 
                //             inner join SealCliente ac on ab.crb_des_cnpjorigem = ac.cli_des_cnpj 
                //             where a.nfs_int_id = aa.nfs_int_id and (ab.crb_dt_recebimento > @fim) and 
                //                  (ab.crb_dt_emissao >= @inicio and ab.crb_dt_emissao <= @fim) and 
                //                  (ac.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))
                //             group by aa.nfs_int_id, ab.crb_dt_recebimento 
                //            ) f 
                //          ";
                #endregion
                query = @"declare @inicio datetime = @dt_inicio, 
		                          @fim datetime = @dt_fim
                            select  --Serviços Não Cumulativo
		                        --(+) Normais
	                            SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
					                            (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                            (e.cli_int_id is not null) and 
					                            --(a.nfs_int_listaservico in (01880, 02119, 02135, 02186, 02194, 03093, 03115, 03450, 03468, 05991, 06009, 07285, 07315, 07323, 07331, 07366, 07390, 07412, 07439, 07447, 07455, 07471, 07498, 02496))
					                            (a.nfs_int_listaservico not in (702, 105, 107))
			                            then a.nfs_num_vlrservico else 0 end) as NCumSPNormais, 
	                            SUM(case when (b.nse_des_estado = 'MS' and b.nse_int_id is not null) and 
					                            (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                            (e.cli_int_id is not null) and 
					                            --(a.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703))
					                            (a.nfs_int_listaservico not in (702, 105, 107))
			                            then a.nfs_num_vlrservico else 0 end) as NCumMSNormais, 
	                            SUM(case when (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                            (e.cli_int_id is not null) and 
					                            --(a.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703, 01880, 02119, 02135, 02186, 02194, 03093, 03115, 03450, 03468, 05991, 06009, 07285, 07315, 07323, 07331, 07366, 07390, 07412, 07439, 07447, 07455, 07471, 07498, 02496))
					                            (a.nfs_int_listaservico not in (702, 105, 107))
			                            then a.nfs_num_vlrservico else 0 end) as NCumTotNormais, 
	                            --Serviços Cumulativo
		                        --(+) Normais
	                            SUM(case when ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
					                            (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                            (e.cli_int_id is not null) and 
					                            --(a.nfs_int_listaservico in (1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879))
					                            (a.nfs_int_listaservico in (702, 105, 107))
			                            then a.nfs_num_vlrservico else 0 end) as CumSPNormais, 
	                            SUM(case when (b.nse_des_estado = 'MS' and b.nse_int_id is not null) and 
					                            (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                            (e.cli_int_id is not null) and 
					                            --(a.nfs_int_listaservico in (702, 103, 105, 107, 104, 106))
					                            (a.nfs_int_listaservico in (702, 105, 107))
			                            then a.nfs_num_vlrservico else 0 end) as CumMSNormais, 
	                            SUM(case when (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
					                            (e.cli_int_id is not null) and 
					                            --(a.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879))
					                            (a.nfs_int_listaservico in (702, 105, 107))
			                            then a.nfs_num_vlrservico else 0 end) as CumTotNormais
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
		                        ";
                #endregion
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dt_inicio", inicio);
                cmd.Parameters.AddWithValue("@dt_fim", fim);

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    #region Getting Values 
                    //Servicos Não Cumulativos
                    conso.ServicoConsuNCum.Normais.SP = Convert.ToDouble(dr["NCumSPNormais"] is DBNull ? 0 : dr["NCumSPNormais"]);
                    conso.ServicoConsuNCum.Normais.MS = Convert.ToDouble(dr["NCumMSNormais"] is DBNull ? 0 : dr["NCumMSNormais"]);
                    conso.ServicoConsuNCum.Normais.Tot = Convert.ToDouble(dr["NCumTotNormais"] is DBNull ? 0 : dr["NCumTotNormais"]);

                    //Servicos Cumulativos
                    conso.ServicoConsuCum.Normais.SP = Convert.ToDouble(dr["CumSPNormais"] is DBNull ? 0 : dr["CumSPNormais"]);
                    conso.ServicoConsuCum.Normais.MS = Convert.ToDouble(dr["CumMSNormais"] is DBNull ? 0 : dr["CumMSNormais"]);
                    conso.ServicoConsuCum.Normais.Tot = Convert.ToDouble(dr["CumTotNormais"] is DBNull ? 0 : dr["CumTotNormais"]);
                    #endregion
                }

                dr.Close();
                #region Query Exclusão
                query = @"declare @inicio datetime = @dt_inicio, 
		                          @fim datetime = @dt_fim
                            select  --Serviços Não Cumulativo
		                            --(-) Exclusao Receitas Diferidas
	                                SUM(case when (a.nfs_int_listaservico in (1880, 2119, 2135, 2186, 2194, 3093, 3115, 3450, 3468, 5991, 6009, 7285, 7315, 7323, 
						                            7331, 7366, 7390, 7412, 7439, 7447, 7455, 7471, 7498, 2496))
			                                    then (case when f.nfs_int_id is null or a.nfs_num_vlrservico = isnull(f.crb_num_valorrecebido, 0)
							                               then a.nfs_num_vlrservico 
							                               when a.nfs_num_vlrservico > isnull(f.crb_num_valorrecebido, 0)
							                               then a.nfs_num_vlrservico - isnull(f.crb_num_valorrecebido, 0)
							                               else 0 end) 
				                                else 0 end) as NCumSPExclusao, 
	                                SUM(case when (a.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703))
			                                    then (case when f.nfs_int_id is null or a.nfs_num_vlrservico = isnull(f.crb_num_valorrecebido, 0)
							                               then a.nfs_num_vlrservico 
							                               when a.nfs_num_vlrservico > isnull(f.crb_num_valorrecebido, 0)
							                               then a.nfs_num_vlrservico - isnull(f.crb_num_valorrecebido, 0)
							                               else 0 end) 
				                                else 0 end) as NCumMSExclusao, 
	                                SUM(case when (a.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703, 
					                                1880, 2119, 2135, 2186, 2194, 3093, 3115, 3450, 3468, 5991, 6009, 7285, 7315, 7323, 
						                            7331, 7366, 7390, 7412, 7439, 7447, 7455, 7471, 7498, 2496))
			                                    then (case when f.nfs_int_id is null or a.nfs_num_vlrservico = isnull(f.crb_num_valorrecebido, 0)
							                               then a.nfs_num_vlrservico 
							                               when a.nfs_num_vlrservico > isnull(f.crb_num_valorrecebido, 0)
							                               then a.nfs_num_vlrservico - isnull(f.crb_num_valorrecebido, 0)
							                               else 0 end) 
				                                else 0 end) as NCumTotExclusao, 
		                            --Serviços Cumulativo
	                                --(-) Exclusao Receitas Diferidas
	                                SUM(case when (a.nfs_int_listaservico in (1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879))
			                                    then (case when f.nfs_int_id is null or a.nfs_num_vlrservico = isnull(f.crb_num_valorrecebido, 0)
							                               then a.nfs_num_vlrservico 
							                               when a.nfs_num_vlrservico > isnull(f.crb_num_valorrecebido, 0)
							                               then a.nfs_num_vlrservico - isnull(f.crb_num_valorrecebido, 0)
							                               else 0 end) 
				                                else 0 end) as CumSPExclusao, 
	                                SUM(case when (a.nfs_int_listaservico in (702, 103, 105, 107, 104, 106))
			                                    then (case when f.nfs_int_id is null or a.nfs_num_vlrservico = isnull(f.crb_num_valorrecebido, 0)
							                               then a.nfs_num_vlrservico 
							                               when a.nfs_num_vlrservico > isnull(f.crb_num_valorrecebido, 0)
							                               then a.nfs_num_vlrservico - isnull(f.crb_num_valorrecebido, 0)
							                               else 0 end) 
				                                else 0 end) as CumMSExclusao, 
	                                SUM(case when (a.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879))
			                                    then (case when f.nfs_int_id is null or a.nfs_num_vlrservico = isnull(f.crb_num_valorrecebido, 0)
							                               then a.nfs_num_vlrservico 
							                               when a.nfs_num_vlrservico > isnull(f.crb_num_valorrecebido, 0)
							                               then a.nfs_num_vlrservico - isnull(f.crb_num_valorrecebido, 0)
							                               else 0 end) 
				                                else 0 end) as CumTotExclusao
                            from SealNotaFiscalServico a
                            left join SealNotaFiscalServicoEmpresa b on a.nse_int_prestador = b.nse_int_id 
                            left join SealNotaFiscalServicoEmpresa c on a.nse_int_tomador = c.nse_int_id 
                            left join SealCliente e on c.nse_des_cnpj = e.cli_des_cnpj
                            outer apply(
	                            select aa.nfs_int_id, ab.crb_dt_recebimento, ab.crb_num_valor, sum(ab.crb_num_valorrecebido) as crb_num_valorrecebido
	                            from SealNotaFiscalRecebida aa 
	                            inner join SealContaReceber ab on aa.nfr_int_id = ab.nfr_int_id 
	                            inner join SealCliente ac on ab.crb_des_cnpjorigem = ac.cli_des_cnpj 
	                            where aa.nfs_int_id = a.nfs_int_id --and --(ab.crb_dt_recebimento > @fim) and 
		                                --(ab.crb_dt_emissao >= @inicio and ab.crb_dt_emissao <= @fim) and 
		                                --(ac.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))
	                            group by aa.nfs_int_id, ab.crb_dt_recebimento, ab.crb_num_valor 
                            ) f 
                            where (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
	                                (e.cli_int_id is not null and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and 
	                                (f.nfs_int_id is null or f.crb_dt_recebimento > @fim or 
	                                (a.nfs_num_vlrservico > f.crb_num_valorrecebido and 
	                                (f.crb_dt_recebimento >= @inicio and f.crb_dt_recebimento <= @fim))) 
		                        ";
                #endregion
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dt_inicio", inicio);
                cmd.Parameters.AddWithValue("@dt_fim", fim);

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    #region Getting Values 
                    //Servicos Não Cumulativos
                    conso.ServicoConsuNCum.Exclusao.SP = Convert.ToDouble(dr["NCumSPExclusao"] is DBNull ? 0 : dr["NCumSPExclusao"]);
                    conso.ServicoConsuNCum.Exclusao.MS = Convert.ToDouble(dr["NCumMSExclusao"] is DBNull ? 0 : dr["NCumMSExclusao"]);
                    conso.ServicoConsuNCum.Exclusao.Tot = Convert.ToDouble(dr["NCumTotExclusao"] is DBNull ? 0 : dr["NCumTotExclusao"]);

                    //Servicos Cumulativos
                    conso.ServicoConsuCum.Exclusao.SP = Convert.ToDouble(dr["CumSPExclusao"] is DBNull ? 0 : dr["CumSPExclusao"]);
                    conso.ServicoConsuCum.Exclusao.MS = Convert.ToDouble(dr["CumMSExclusao"] is DBNull ? 0 : dr["CumMSExclusao"]);
                    conso.ServicoConsuCum.Exclusao.Tot = Convert.ToDouble(dr["CumTotExclusao"] is DBNull ? 0 : dr["CumTotExclusao"]);
                    #endregion
                }

                dr.Close();
                #region Query Adição
                query = @"declare @inicio datetime = @dt_inicio, 
		                          @fim datetime = @dt_fim
                            select  --Serviços Não Cumulativo
		                            --(+) Adição Receita Diferida
	                                SUM(case when ((c.nfs_des_ufmunicipio = 'SP' and e.nse_int_id is null) or c.nse_int_prestador is null) and 
					                                (c.nfs_int_listaservico in (01880, 02119, 02135, 02186, 02194, 03093, 
					                                03115, 03450, 03468, 05991, 06009, 07285, 07315, 07323, 07331, 07366, 
					                                07390, 07412, 07439, 07447, 07455, 07471, 07498, 02496))
			                                then a.crb_num_valorrecebido else 0 end) as NCumSPAdicao, 
	                                SUM(case when (e.nse_des_estado = 'MS' and e.nse_int_id is not null) and 
					                                (c.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703))
			                                then a.crb_num_valorrecebido else 0 end) as NCumMSAdicao, 
	                                SUM(case when (c.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703, 
					                                01880, 02119, 02135, 02186, 02194, 03093, 03115, 03450, 03468, 05991, 
					                                06009, 07285, 07315, 07323, 07331, 07366, 07390, 07412, 07439, 07447, 
					                                07455, 07471, 07498, 02496))
			                                then a.crb_num_valorrecebido else 0 end) as NCumTotAdicao, 
	                                --(+) Adição Receita Diferida
	                                SUM(case when ((c.nfs_des_ufmunicipio = 'SP' and e.nse_int_id is null) or c.nse_int_prestador is null) and 
					                                (c.nfs_int_listaservico in (1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879))
			                                then a.crb_num_valorrecebido else 0 end) as CumSPAdicao, 
	                                SUM(case when (e.nse_des_estado = 'MS' and e.nse_int_id is not null) and 
					                                (c.nfs_int_listaservico in (702, 103, 105, 107, 104, 106))
			                                then a.crb_num_valorrecebido else 0 end) as CumMSAdicao, 
	                                SUM(case when (c.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1015, 
					                                1023, 2682, 2798, 2917, 2918, 2690, 2879))
			                                then a.crb_num_valorrecebido else 0 end) as CumTotAdicao 
                            from SealContaReceber a 
                            left join SealNotaFiscalRecebida b on a.nfr_int_id = b.nfr_int_id 
                            left join SealNotaFiscalServico c on b.nfs_int_id = c.nfs_int_id
                            left join SealNotaFiscalServicoEmpresa e on c.nse_int_prestador = e.nse_int_id
                            left join SealNotaFiscalServicoEmpresa f on c.nse_int_tomador = f.nse_int_id
                            left join SealCliente g on f.nse_des_cnpj = g.cli_des_cnpj
                            where (a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim) and 
                            (g.cli_int_id is not null and (g.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718')))
		                        ";
                #endregion
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dt_inicio", inicio);
                cmd.Parameters.AddWithValue("@dt_fim", fim);

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    #region Getting Values 
                    //Servicos Não Cumulativos
                    conso.ServicoConsuNCum.Adicao.SP = Convert.ToDouble(dr["NCumSPAdicao"] is DBNull ? 0 : dr["NCumSPAdicao"]);
                    conso.ServicoConsuNCum.Adicao.MS = Convert.ToDouble(dr["NCumMSAdicao"] is DBNull ? 0 : dr["NCumMSAdicao"]);
                    conso.ServicoConsuNCum.Adicao.Tot = Convert.ToDouble(dr["NCumTotAdicao"] is DBNull ? 0 : dr["NCumTotAdicao"]);

                    //Servicos Cumulativos
                    conso.ServicoConsuCum.Adicao.SP = Convert.ToDouble(dr["CumSPAdicao"] is DBNull ? 0 : dr["CumSPAdicao"]);
                    conso.ServicoConsuCum.Adicao.MS = Convert.ToDouble(dr["CumMSAdicao"] is DBNull ? 0 : dr["CumMSAdicao"]);
                    conso.ServicoConsuCum.Adicao.Tot = Convert.ToDouble(dr["CumTotAdicao"] is DBNull ? 0 : dr["CumTotAdicao"]);
                    #endregion
                }

                dr.Close();
                #endregion

                #region Retenções
                #region Query
                query = @"declare @inicio datetime = @dt_inicio, 
		                          @fim datetime = @dt_fim 
                            select sum(case when a.crb_des_observacao = 'COFINS crédito sobre impostos retidos' and 
			                        b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
			                        (d.nfd_int_id is not null or 
			                        (e.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703, 
					                            01880, 02119, 02135, 02186, 02194, 03093, 03115, 03450, 03468, 05991, 
					                            06009, 07285, 07315, 07323, 07331, 07366, 07390, 07412, 07439, 07447, 
					                            07455, 07471, 07498, 02496)))
		                           then a.crb_num_valorrecebido else 0 end) as CofinsPublicoNCum, 
	                           sum(case when a.crb_des_observacao = 'COFINS crédito sobre impostos retidos' and 
			                        b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') and 
			                        (d.nfd_int_id is not null or 
			                        (e.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703, 
					                            01880, 02119, 02135, 02186, 02194, 03093, 03115, 03450, 03468, 05991, 
					                            06009, 07285, 07315, 07323, 07331, 07366, 07390, 07412, 07439, 07447, 
					                            07455, 07471, 07498, 02496)))
		                           then a.crb_num_valorrecebido else 0 end) as CofinsPrivadoNCum,
	                           sum(case when a.crb_des_observacao = 'PIS crédito sobre impostos retidos' and 
			                        b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
			                        (d.nfd_int_id is not null or 
			                        (e.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703, 
					                            01880, 02119, 02135, 02186, 02194, 03093, 03115, 03450, 03468, 05991, 
					                            06009, 07285, 07315, 07323, 07331, 07366, 07390, 07412, 07439, 07447, 
					                            07455, 07471, 07498, 02496)))
		                           then a.crb_num_valorrecebido else 0 end) as PisPublicoNCum, 
	                           sum(case when a.crb_des_observacao = 'PIS crédito sobre impostos retidos' and 
			                        b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') and 
			                        (d.nfd_int_id is not null or 
			                        (e.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703, 
					                            01880, 02119, 02135, 02186, 02194, 03093, 03115, 03450, 03468, 05991, 
					                            06009, 07285, 07315, 07323, 07331, 07366, 07390, 07412, 07439, 07447, 
					                            07455, 07471, 07498, 02496)))
		                           then a.crb_num_valorrecebido else 0 end) as PisPrivadoNCum, 
	                           sum(case when a.crb_des_observacao = 'IR crédito sobre impostos retidos' and 
			                        b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
			                        (d.nfd_int_id is not null or 
			                        (e.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703, 
					                            01880, 02119, 02135, 02186, 02194, 03093, 03115, 03450, 03468, 05991, 
					                            06009, 07285, 07315, 07323, 07331, 07366, 07390, 07412, 07439, 07447, 
					                            07455, 07471, 07498, 02496)))
		                           then a.crb_num_valorrecebido else 0 end) as IrPublicoNCum, 
	                           sum(case when a.crb_des_observacao = 'IR crédito sobre impostos retidos' and 
			                        b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') and 
			                        (d.nfd_int_id is not null or 
			                        (e.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703, 
					                            01880, 02119, 02135, 02186, 02194, 03093, 03115, 03450, 03468, 05991, 
					                            06009, 07285, 07315, 07323, 07331, 07366, 07390, 07412, 07439, 07447, 
					                            07455, 07471, 07498, 02496)))
		                           then a.crb_num_valorrecebido else 0 end) as IrPrivadoNCum,

	                           sum(case when a.crb_des_observacao = 'COFINS crédito sobre impostos retidos' and 
			                        b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
			                        (e.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1015, 
					                            1023, 2682, 2798, 2917, 2918, 2690, 2879))
		                           then a.crb_num_valorrecebido else 0 end) as CofinsPublicoCum, 
	                           sum(case when a.crb_des_observacao = 'COFINS crédito sobre impostos retidos' and 
			                        b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') and 
			                        (e.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1015, 
					                            1023, 2682, 2798, 2917, 2918, 2690, 2879))
		                           then a.crb_num_valorrecebido else 0 end) as CofinsPrivadoCum,
	                           sum(case when a.crb_des_observacao = 'PIS crédito sobre impostos retidos' and 
			                        b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
			                        (e.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1015, 
					                            1023, 2682, 2798, 2917, 2918, 2690, 2879))
		                           then a.crb_num_valorrecebido else 0 end) as PisPublicoCum, 
	                           sum(case when a.crb_des_observacao = 'PIS crédito sobre impostos retidos' and 
			                        b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') and 
			                        (e.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1015, 
					                            1023, 2682, 2798, 2917, 2918, 2690, 2879))
		                           then a.crb_num_valorrecebido else 0 end) as PisPrivadoCum, 
	   
	                           sum(case when a.crb_des_observacao = 'IR crédito sobre impostos retidos' and 
			                        b.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
			                        (e.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1015, 
					                            1023, 2682, 2798, 2917, 2918, 2690, 2879))
		                           then a.crb_num_valorrecebido else 0 end) as IrPublicoCum, 
	                           sum(case when a.crb_des_observacao = 'IR crédito sobre impostos retidos' and 
			                        b.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') and 
			                        (e.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1015, 
					                            1023, 2682, 2798, 2917, 2918, 2690, 2879))
		                           then a.crb_num_valorrecebido else 0 end) as IrPrivadoCum 
                        from SealContaReceber a 
                        inner join SealCliente b on a.crb_des_cnpjorigem = b.cli_des_cnpj
                        inner join SealNotaFiscalRecebida c on a.nfr_int_id = c.nfr_int_id 
                        left join SealNotaFiscalDanfe d on c.nfd_int_id = d.nfd_int_id
                        left join SealNotaFiscalServico e on c.nfs_int_id = e.nfs_int_id 
                        where a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim 
		                        ";
                #endregion
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dt_inicio", inicio);
                cmd.Parameters.AddWithValue("@dt_fim", fim);

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    #region Getting Values 
                    //Retenções Não Cumulativos
                    conso.RetencaoConsuNCum.CofinsPrivado = Convert.ToDouble(dr["CofinsPrivadoNCum"] is DBNull ? 0 : dr["CofinsPrivadoNCum"]);
                    conso.RetencaoConsuNCum.CofinsPublico = Convert.ToDouble(dr["CofinsPublicoNCum"] is DBNull ? 0 : dr["CofinsPublicoNCum"]);
                    conso.RetencaoConsuNCum.PisPrivado = Convert.ToDouble(dr["PisPrivadoNCum"] is DBNull ? 0 : dr["PisPrivadoNCum"]);
                    conso.RetencaoConsuNCum.PisPublico = Convert.ToDouble(dr["PisPublicoNCum"] is DBNull ? 0 : dr["PisPublicoNCum"]);
                    conso.RetencaoConsuNCum.IrPrivado = Convert.ToDouble(dr["IrPrivadoNCum"] is DBNull ? 0 : dr["IrPrivadoNCum"]);
                    conso.RetencaoConsuNCum.IrPublico = Convert.ToDouble(dr["IrPublicoNCum"] is DBNull ? 0 : dr["IrPublicoNCum"]);

                    //Retenções Cumulativos
                    conso.RetencaoConsuCum.CofinsPrivado = Convert.ToDouble(dr["CofinsPrivadoCum"] is DBNull ? 0 : dr["CofinsPrivadoCum"]);
                    conso.RetencaoConsuCum.CofinsPublico = Convert.ToDouble(dr["CofinsPublicoCum"] is DBNull ? 0 : dr["CofinsPublicoCum"]);
                    conso.RetencaoConsuCum.PisPrivado = Convert.ToDouble(dr["PisPrivadoCum"] is DBNull ? 0 : dr["PisPrivadoCum"]);
                    conso.RetencaoConsuCum.PisPublico = Convert.ToDouble(dr["PisPublicoCum"] is DBNull ? 0 : dr["PisPublicoCum"]);
                    conso.RetencaoConsuCum.IrPrivado = Convert.ToDouble(dr["IrPrivadoCum"] is DBNull ? 0 : dr["IrPrivadoCum"]);
                    conso.RetencaoConsuCum.IrPublico = Convert.ToDouble(dr["IrPublicoCum"] is DBNull ? 0 : dr["IrPublicoCum"]);
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                retorno = false;
                erro = "ERRO: " + ex.Message;
            }
            finally
            {
                dr.Close();
                conn.Close();
            }
            return retorno;
        }

        public bool DropServicosSP(int[] servicos, out string erro)
        {
            string query = "";
            bool retorno = false;
            erro = "OK";
            try
            {
                conn.Open();
                foreach (int servico in servicos)
                {
                    query = @"delete from SealNotaFiscalRecebida where nfs_int_id = (select a.nfs_int_id from SealNotaFiscalRecebida a
                                                                                     inner join SealNotaFiscalServico b on a.nfs_int_id = b.nfs_int_id 
                                                                                     where nfs_int_numero = @nfs_int_numero and (nfs_des_ufmunicipio <> 'MS' and nfs_int_municipio = 0))
                              delete from SealNotaFiscalServico where nfs_int_numero = @nfs_int_numero and (nfs_des_ufmunicipio <> 'MS' and nfs_int_municipio = 0)
                              delete from SealNotaFiscalServicoEmpresa 
                              where nse_int_id = (select nse_int_prestador from SealNotaFiscalServico where nfs_int_numero = @nfs_int_numero and (nfs_des_ufmunicipio <> 'MS' and nfs_int_municipio = 0)) or  
                                    nse_int_id = (select nse_int_tomador from SealNotaFiscalServico where nfs_int_numero = @nfs_int_numero and (nfs_des_ufmunicipio <> 'MS' and nfs_int_municipio = 0))";

                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nfs_int_numero", servico);

                    int qtd = Convert.ToInt32(cmd.ExecuteNonQuery());
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

        public bool DropDanfesSP(int[] servicos, out string erro)
        {
            string query = "";
            bool retorno = false;
            erro = "OK";
            try
            {
                conn.Open();
                foreach (int servico in servicos)
                {
                    query = @"delete from SealNotaFiscalRecebida where nfd_int_id = (select a.nfd_int_id from SealNotaFiscalRecebida a
                                                                                        inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id 
                                                                                        where b.nfd_int_nNf = @nfd_int_nNf and b.nfd_int_nUF = 35)
                                delete from SealNotaFiscalDanfeItem where nfd_int_id = (select a.nfd_int_id from SealNotaFiscalRecebida a
                                                                                        inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id 
                                                                                        where b.nfd_int_nNf = @nfd_int_nNf and b.nfd_int_nUF = 35)
                                delete from SealNotaFiscalDanfe where nfd_int_nNf = @nfd_int_nNf and nfd_int_nUF = 35
                                delete from SealNotaFiscalDanfeEmpresa 
                                where nde_int_id = (select nde_int_emit from SealNotaFiscalDanfe where nfd_int_nNf = @nfd_int_nNf and nfd_int_nUF = 35) or 
                                    nde_int_id = (select nde_int_dest from SealNotaFiscalDanfe where nfd_int_nNf = @nfd_int_nNf and nfd_int_nUF = 35)";

                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nfs_int_numero", servico);

                    int qtd = Convert.ToInt32(cmd.ExecuteNonQuery());
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

        public bool DropServicosMS(int[] servicos, out string erro)
        {
            string query = "";
            bool retorno = false;
            erro = "OK";
            try
            {
                conn.Open();
                foreach (int servico in servicos)
                {
                    query = @"delete from SealNotaFiscalRecebida where nfs_int_id = (select a.nfs_int_id from SealNotaFiscalRecebida a
                                                                                     inner join SealNotaFiscalServico b on a.nfs_int_id = b.nfs_int_id 
                                                                                     where nfs_int_numero = @nfs_int_numero and (nfs_des_ufmunicipio <> 'MS' and nfs_int_municipio = 0))
                              delete from SealNotaFiscalServico where nfs_int_numero = @nfs_int_numero and (nfs_des_ufmunicipio = 'MS' and nfs_int_municipio <> 0)
                              delete from SealNotaFiscalServicoEmpresa 
                              where nse_int_id = (select nse_int_prestador from SealNotaFiscalServico where nfs_int_numero = @nfs_int_numero and (nfs_des_ufmunicipio = 'MS' and nfs_int_municipio <> 0)) or 
                                    nse_int_id = (select nse_int_tomador from SealNotaFiscalServico where nfs_int_numero = @nfs_int_numero and (nfs_des_ufmunicipio = 'MS' and nfs_int_municipio <> 0))";

                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nfs_int_numero", servico);

                    int qtd = Convert.ToInt32(cmd.ExecuteNonQuery());
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

        public bool DropDanfesMS(int[] servicos, out string erro)
        {
            string query = "";
            bool retorno = false;
            erro = "OK";
            try
            {
                conn.Open();
                foreach (int servico in servicos)
                {
                    query = @"delete from SealNotaFiscalRecebida where nfd_int_id = (select a.nfd_int_id from SealNotaFiscalRecebida a
                                                                                        inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id 
                                                                                        where b.nfd_int_nNf = @nfd_int_nNf and b.nfd_int_nUF = 50)
                                delete from SealNotaFiscalDanfeItem where nfd_int_id = (select a.nfd_int_id from SealNotaFiscalRecebida a
                                                                                        inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id 
                                                                                        where b.nfd_int_nNf = @nfd_int_nNf and b.nfd_int_nUF = 50)
                                delete from SealNotaFiscalDanfe where nfd_int_nNf = @nfd_int_nNf and nfd_int_nUF = 50
                                delete from SealNotaFiscalDanfeEmpresa 
                                where nde_int_id = (select nde_int_emit from SealNotaFiscalDanfe where nfd_int_nNf = @nfd_int_nNf and nfd_int_nUF = 50) or 
                                    nde_int_id = (select nde_int_dest from SealNotaFiscalDanfe where nfd_int_nNf = @nfd_int_nNf and nfd_int_nUF = 50)";

                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nfs_int_numero", servico);

                    int qtd = Convert.ToInt32(cmd.ExecuteNonQuery());
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

        public DataTable GetDetailDanfes(DateTime inicio, DateTime fim, string uf, int lei, int rec, int ctb, out string erro)
        {
            string query = "";
            DataTable retorno = new DataTable();
            erro = "OK";
            try
            {
                conn.Open();
                query = @"declare   @p_lei int = @lei, -- 0 = false, 1 = true, 2 = null
		                            @p_rec int = @rec, -- 0 = false, 1 = true, 2 = null
		                            @p_ctb int = @ctb  -- 0 = false, 1 = true, 2 = null
                            select b.nfd_int_nNf as Nota, 
	                                case when e.nfr_int_id is null then 'Não' else 'Sim' end as Recebida, 
	                                CONVERT(DATE, b.nfd_dt_dhEmi) as Emissão, 
	                                a.ndi_int_cfop as CFOP, 
	                                case when f.cli_des_grupo = 'CONTRIBUINTE' or f.cli_des_grupo = 'NÃO CONTRIBUINTE' then 'Privado' else 'Publico' end as Destinatario, 
	                                REPLACE(CONVERT(varchar(100), SUM(a.ndi_num_vProd)), '.', ',') as Valor 
                            from SealNotaFiscalDanfeItem a
                            inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
                            left join SealNotaFiscalDanfeEmpresa c on b.nde_int_emit = c.nde_int_id
                            left join SealNotaFiscalDanfeEmpresa d on b.nde_int_dest = d.nde_int_id
                            outer apply(
	                            select top 1 nfr_int_id, nfd_int_id, nfs_int_id from SealNotaFiscalRecebida 
	                            where nfd_int_id = b.nfd_int_id
                            ) e
                            left join SealCliente f on d.nde_des_cnpj = f.cli_des_cnpj
                            where c.nde_des_uf = @uf and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
	                            a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403) and 
	                            ((@p_lei = 0 and 
		                            ((@p_ctb = 0 and (f.cli_int_id is not null and f.cli_des_grupo = 'NÃO CONTRIBUINTE')) or 
		                                (@p_ctb = 1 and (f.cli_int_id is not null and f.cli_des_grupo = 'CONTRIBUINTE')) or 
		                                (@p_ctb = 2 and (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE' or f.cli_des_grupo = 'NÃO CONTRIBUINTE'))))) or 
	                                (@p_lei = 1 and 
		                            ((@p_ctb = 0 and (f.cli_int_id is not null and f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718')) or 
		                                (@p_ctb = 1 and (f.cli_int_id is not null and f.cli_des_grupo = 'CONTRIB - Lei 9718')) or 
		                                (@p_ctb = 2 and (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIB - Lei 9718' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718'))))) or
	                                (@p_lei = 2 and 
		                            ((@p_ctb = 0 and (f.cli_int_id is not null and (f.cli_des_grupo = 'NÃO CONTRIBUINTE' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718'))) or 
		                                (@p_ctb = 1 and (f.cli_int_id is not null and (f.cli_des_grupo = 'CONTRIBUINTE' or f.cli_des_grupo = 'CONTRIB - Lei 9718'))) or 
		                                (@p_ctb = 2 and (f.cli_int_id is not null))))) and 
	                            ((@p_rec = 0 and (e.nfr_int_id is null)) or 
	                                (@p_rec = 1 and (e.nfr_int_id is not null)) or 
	                                (@p_rec = 2))
                            group by b.nfd_int_nNf, e.nfr_int_id, b.nfd_dt_dhEmi, a.ndi_int_cfop, f.cli_des_grupo ";

                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@inicio", inicio);
                cmd.Parameters.AddWithValue("@fim", fim);
                cmd.Parameters.AddWithValue("@uf", uf);
                cmd.Parameters.AddWithValue("@lei", lei);
                cmd.Parameters.AddWithValue("@rec", rec);
                cmd.Parameters.AddWithValue("@ctb", ctb);

                DbDataReader dr = cmd.ExecuteReader();
                retorno.Load(dr);
            }
            catch (Exception ex)
            {
                erro = "ERRO: " + ex.Message;
            }
            finally
            {
                //dr.Close();
                conn.Close();
            }
            return retorno;
        }

        public DataTable GetDetailServicos(DateTime inicio, DateTime fim, int uf, int cum, int lei, int rec, out string erro)
        {
            string query = "";
            DataTable retorno = new DataTable();
            erro = "OK";
            try
            {
                conn.Open();
                query = @"declare   @p_uf  int = @uf,  -- 0 = MS   , 1 = SP  , 2 = null 
		                            @p_cum int = @cum, -- 0 = false, 1 = true, 2 = null
		                            @p_lei int = @lei, -- 0 = false, 1 = true, 2 = null
		                            @p_rec int = @rec  -- 0 = false, 1 = true, 2 = null

                            select a.nfs_int_numero as Nota, 
	                               case when d.nfs_int_id is null then 'Não' else 'Sim' end as Recebida, 
	                               CONVERT(DATE, a.nfs_dt_emissao) as Emissão, 
	                               case when e.cli_des_grupo = 'CONTRIBUINTE' or e.cli_des_grupo = 'NÃO CONTRIBUINTE' then 'Privado' else 'Publico' end as Destinatario, 
	                               REPLACE(CONVERT(varchar(100), SUM(a.nfs_num_vlrservico)), '.', ',') as Valor 
                            from SealNotaFiscalServico a
                            left join SealNotaFiscalServicoEmpresa b on a.nse_int_prestador = b.nse_int_id 
                            left join SealNotaFiscalServicoEmpresa c on a.nse_int_tomador = c.nse_int_id 
                            outer apply(
	                            select aa.nfs_int_id, ab.crb_dt_recebimento, 
		                               SUM(ab.crb_num_valorrecebido) as crb_num_valorrecebido
	                            from SealNotaFiscalRecebida aa 
	                            inner join SealContaReceber ab on aa.nfr_int_id = ab.nfr_int_id
	                            where aa.nfs_int_id = a.nfs_int_id 
	                            group by aa.nfs_int_id, ab.crb_dt_recebimento
                            ) d
                            left join SealCliente e on c.nse_des_cnpj = e.cli_des_cnpj 
                            where (e.cli_int_id is not null) and 
                            ((@p_uf = 0 and 
	                            (b.nse_des_estado = 'MS' and b.nse_int_id is not null) and 
	                            ((@p_cum = 0 and (a.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703))) or 
	                             (@p_cum = 1 and (a.nfs_int_listaservico in (702, 103, 105, 107, 104, 106))) or 
	                             (@p_cum = 2 and (a.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703))))) or   
                            (@p_uf = 1 and 
	                            ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
	                            ((@p_cum = 0 and (a.nfs_int_listaservico in (01880, 02119, 02135, 02186, 02194, 03093, 03115, 03450, 03468, 05991, 06009, 07285, 07315, 07323, 07331, 07366, 
												                            07390, 07412, 07439, 07447, 07455, 07471, 07498, 02496))) or 
	                             (@p_cum = 1 and (a.nfs_int_listaservico in (1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879))) or 
	                             (@p_cum = 2 and (a.nfs_int_listaservico in (1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879, 01880, 02119, 02135, 02186, 02194, 03093, 03115, 03450, 03468, 
												                            05991, 06009, 07285, 07315, 07323, 07331, 07366, 07390, 07412, 07439, 07447, 07455, 07471, 07498, 02496))))) or  
                            (@p_uf = 2 and 
	                            ((@p_cum = 0 and (a.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703, 01880, 02119, 02135, 02186, 02194, 03093, 03115, 03450, 03468, 05991, 
					                            06009, 07285, 07315, 07323, 07331, 07366, 07390, 07412, 07439, 07447, 07455, 07471, 07498, 02496))) or 
	                             (@p_cum = 1 and (a.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879))) or 
	                             (@p_cum = 2 and (a.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703, 01880, 02119, 02135, 02186, 02194, 03093, 03115, 03450, 03468, 05991, 
					                            06009, 07285, 07315, 07323, 07331, 07366, 07390, 07412, 07439, 07447, 07455, 07471, 07498, 02496, 702, 103, 105, 107, 104, 106, 1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879)))))) and 
                            (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
                            ((@p_lei = 0 and (e.cli_int_id is not null and (e.cli_des_grupo = 'CONTRIBUINTE' or e.cli_des_grupo = 'NÃO CONTRIBUINTE'))) or 
                             (@p_lei = 1 and (e.cli_int_id is not null and (e.cli_des_grupo = 'CONTRIB - Lei 9718' or e.cli_des_grupo = 'Ñ CONTRIB - Lei 9718'))) or
                             (@p_lei = 2 and (e.cli_int_id is not null))) and 
                            ((@p_rec = 0 and (d.nfs_int_id is null)) or 
                             (@p_rec = 1 and (d.nfs_int_id is not null)) or 
                             (@p_rec = 2))
                            group by a.nfs_int_numero, d.nfs_int_id, a.nfs_dt_emissao, d.crb_dt_recebimento, e.cli_des_grupo ";

                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@inicio", inicio);
                cmd.Parameters.AddWithValue("@fim", fim);
                cmd.Parameters.AddWithValue("@uf", uf);
                cmd.Parameters.AddWithValue("@cum", cum);
                cmd.Parameters.AddWithValue("@lei", lei);
                cmd.Parameters.AddWithValue("@rec", rec);

                DbDataReader dr = cmd.ExecuteReader();
                retorno.Load(dr);
            }
            catch (Exception ex)
            {
                erro = "ERRO: " + ex.Message;
            }
            finally
            {
                //dr.Close();
                conn.Close();
            }
            return retorno;
        }

        public DataTable GetConsolidadoDanfes(DateTime inicio, DateTime fim, int uf, int exc, out string erro)
        {
            string query = "";
            DataTable retorno = new DataTable();
            erro = "OK";
            try
            {
                conn.Open();
                if (exc != 2)
                {
                    query = @"declare   @p_uf  int = @uf , -- 0 = MS   , 1 = SP  , 2 = null
		                            @p_exc int = @exc  -- 0 = false, 1 = true, 2 = null

                              select b.nfd_int_nNf as Nota, 
	                                    case when f.nfd_int_id is null then 'Não' else 'Sim' end as Recebida, 
	                                    CONVERT(DATE, b.nfd_dt_dhEmi) as Emissão, 
	                                    a.ndi_int_cfop as CFOP, 
	                                    case when e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') then 'Privado' else 'Publico' end as Destinatario, 
	                                    'R$ ' + REPLACE(CONVERT(varchar(100), CONVERT(MONEY, SUM(a.ndi_num_vProd))), '.', ',') as Valor 
                                from SealNotaFiscalDanfeItem a
                                inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
                                left join SealNotaFiscalDanfeEmpresa c on b.nde_int_emit = c.nde_int_id
                                left join SealNotaFiscalDanfeEmpresa d on b.nde_int_dest = d.nde_int_id
                                left join SealCliente e on d.nde_des_cnpj = e.cli_des_cnpj
                                outer apply(
	                                select aa.nfd_int_id, ab.crb_dt_recebimento 
	                                from SealNotaFiscalRecebida aa 
	                                inner join SealContaReceber ab on aa.nfr_int_id = ab.nfr_int_id 
	                                where b.nfd_int_id = aa.nfd_int_id and (ab.crb_dt_recebimento < @fim)
	                                group by aa.nfd_int_id, ab.crb_dt_recebimento 
                                ) f 
                                where (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
	                                  (a.ndi_int_cfop in (6102, 5102, 6108, 6119, 5119, 6120, 5120, 6922, 5922, 5405, 6405, 6403)) and 
	                                  ((@p_uf = 0 and (c.nde_des_uf = 'MS')) or 
	                                   (@p_uf = 1 and (c.nde_des_uf = 'SP')) or 
	                                   (@p_uf = 2)) and 
	                                  ((@p_exc = 0 and (e.cli_int_id is not null)) or 
	                                   (@p_exc = 1 and (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
					                                    (f.nfd_int_id is null or f.crb_dt_recebimento > @fim))) or
	                                   (@p_exc = 2 and (e.cli_int_id is null)))
                                group by b.nfd_int_nNf, f.nfd_int_id, b.nfd_dt_dhEmi, a.ndi_int_cfop, e.cli_des_grupo  ";
                }
                else
                {
                    query = @"declare   @p_uf  int = @uf  -- 0 = MS   , 1 = SP  , 2 = null

                            select c.nfd_int_nNf as Nota, 
	                                case when b.nfr_int_id is null then 'Não' else 'Sim' end as Recebida, 
	                                CONVERT(DATE, c.nfd_dt_dhEmi) as Emissão, 
		                            e.nde_des_uf as UF, 
	                                case when g.cli_des_grupo = 'CONTRIBUINTE' or g.cli_des_grupo = 'NÃO CONTRIBUINTE' then 'Privado' else 'Publico' end as Destinatario, 
	                                'R$ ' + REPLACE(CONVERT(varchar(100), CONVERT(MONEY, SUM(a.crb_num_valorrecebido))), '.', ',') as Valor 
                            from SealContaReceber a 
                            left join SealNotaFiscalRecebida b on a.nfr_int_id = b.nfr_int_id 
                            left join SealNotaFiscalDanfe c on b.nfd_int_id = c.nfd_int_id
                            left join SealNotaFiscalDanfeEmpresa e on c.nde_int_emit = e.nde_int_id
                            left join SealNotaFiscalDanfeEmpresa f on c.nde_int_dest = f.nde_int_id
                            left join SealCliente g on f.nde_des_cnpj = g.cli_des_cnpj
                            where (a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim) and 
	                                (g.cli_int_id is not null and (g.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and 
	                                ((@p_uf = 0 and (e.nde_des_uf = 'MS')) or 
	                                (@p_uf = 1 and (e.nde_des_uf = 'SP')) or 
	                                (@p_uf = 2))
                            group by c.nfd_int_nNf, b.nfr_int_id, c.nfd_dt_dhEmi, e.nde_des_uf, g.cli_des_grupo  ";
                }

                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@inicio", inicio);
                cmd.Parameters.AddWithValue("@fim", fim);
                cmd.Parameters.AddWithValue("@uf", uf);
                cmd.Parameters.AddWithValue("@exc", exc);

                DbDataReader dr = cmd.ExecuteReader();
                retorno.Load(dr);
            }
            catch (Exception ex)
            {
                erro = "ERRO: " + ex.Message;
            }
            finally
            {
                //dr.Close();
                conn.Close();
            }
            return retorno;
        }

        public DataTable GetConsolidadoServicos(DateTime inicio, DateTime fim, int uf, int cum, int exc, out string erro)
        {
            string query = "";
            DataTable retorno = new DataTable();
            erro = "OK";
            try
            {
                conn.Open();
                if (exc != 2)
                {
                    query = @"declare   @p_uf  int = @uf,   -- 0 = MS   , 1 = SP  , 2 = null
		                                @p_cum int = @cum,  -- 0 = false, 1 = true, 2 = null
		                                @p_exc int = @exc   -- 0 = false, 1 = true, 2 = null

                            select  a.nfs_int_numero as Nota, 
	                                case when f.nfs_int_id is null then 'Não' else 'Sim' end as Recebida, 
	                                CONVERT(DATE, a.nfs_dt_emissao) as Data, 
	                                case when e.cli_des_grupo in ('CONTRIBUINTE', 'NÃO CONTRIBUINTE') then 'Privado' else 'Publico' end as Destinatario, 
	                                'R$ ' + REPLACE(CONVERT(varchar(100), CONVERT(MONEY, SUM(a.nfs_num_vlrservico))), '.', ',') as 'Valor Serviço', 
	                                'R$ ' + REPLACE(CONVERT(varchar(100), CONVERT(MONEY, SUM(isnull((case when f.crb_dt_recebimento >= @inicio and f.crb_dt_recebimento <= @fim 
																		                                  then f.crb_num_valorrecebido else 0 end), 0)))), '.', ',') as 'Valor Recebido', 
	                                'R$ ' + REPLACE(CONVERT(varchar(100), CONVERT(MONEY, SUM(a.nfs_num_vlrservico - isnull((case when f.crb_dt_recebimento >= @inicio and f.crb_dt_recebimento <= @fim 
																		                                  then f.crb_num_valorrecebido else 0 end), 0)))), '.', ',') as 'Valor Excluido' 
                                from SealNotaFiscalServico a
                                left join SealNotaFiscalServicoEmpresa b on a.nse_int_prestador = b.nse_int_id 
                                left join SealNotaFiscalServicoEmpresa c on a.nse_int_tomador = c.nse_int_id 
                                left join SealCliente e on c.nse_des_cnpj = e.cli_des_cnpj
                                outer apply(
	                                select aa.nfs_int_id, ab.crb_dt_recebimento, ab.crb_num_valor, sum(ab.crb_num_valorrecebido) as crb_num_valorrecebido
	                                from SealNotaFiscalRecebida aa 
	                                inner join SealContaReceber ab on aa.nfr_int_id = ab.nfr_int_id 
	                                inner join SealCliente ac on ab.crb_des_cnpjorigem = ac.cli_des_cnpj 
	                                where aa.nfs_int_id = a.nfs_int_id --and --(ab.crb_dt_recebimento > @fim) and 
		                                    --(ab.crb_dt_emissao >= @inicio and ab.crb_dt_emissao <= @fim) and 
		                                    --(ac.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))
	                                group by aa.nfs_int_id, ab.crb_dt_recebimento, ab.crb_num_valor 
                                ) f 	     
                                where (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and 
	                                  ((@p_exc = 0 and (e.cli_int_id is not null)) or 
	                                   (@p_exc = 1 and (e.cli_int_id is not null and 
						                                (e.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718') and 
						                                (f.nfs_int_id is null or f.crb_dt_recebimento > @fim or 
						                                 (a.nfs_num_vlrservico > f.crb_num_valorrecebido and 
						                                  (f.crb_dt_recebimento >= @inicio and f.crb_dt_recebimento <= @fim)))))) or 
	                                   (@p_exc = 2 and (e.cli_int_id is null))) and 
	                                  ((@p_uf = 0 and (b.nse_des_estado = 'MS' and b.nse_int_id is not null) and 
		                                ((@p_cum = 0 and (a.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703))) or 
	                                     (@p_cum = 1 and (a.nfs_int_listaservico in (702, 103, 105, 107, 104, 106))) or 
	                                     (@p_cum = 2 and (a.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703))))) or 
	                                   (@p_uf = 1 and ((a.nfs_des_ufmunicipio = 'SP' and b.nse_int_id is null) or a.nse_int_prestador is null) and 
		                                ((@p_cum = 0 and (a.nfs_int_listaservico in (1880, 2119, 2135, 2186, 2194, 3093, 3115, 3450, 3468, 5991, 6009, 7285, 
														                                7315, 7323, 7331, 7366, 7390, 7412, 7439, 7447, 7455, 7471, 7498, 2496))) or 
		                                 (@p_cum = 1 and (a.nfs_int_listaservico in (1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879))) or 
		                                 (@p_cum = 2 and (a.nfs_int_listaservico in (1880, 2119, 2135, 2186, 2194, 3093, 3115, 3450, 3468, 5991, 6009, 7285, 
														                                7315, 7323, 7331, 7366, 7390, 7412, 7439, 7447, 7455, 7471, 7498, 2496, 
														                                1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879))))) or 
	                                   (@p_uf = 2 and 
		                                ((@p_cum = 0 and (a.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703 , 1880, 2119, 2135, 2186, 
														                                2194, 3093, 3115, 3450, 3468, 5991, 6009, 7285, 7315, 7323, 7331, 7366, 7390, 
														                                7412, 7439, 7447, 7455, 7471, 7498, 2496))) or 
		                                 (@p_cum = 1 and (a.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879))) or 
		                                 (@p_cum = 2 and (a.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 
														                                703 , 1880, 2119, 2135, 2186, 2194, 3093, 3115, 3450, 3468, 5991, 6009, 7285, 
														                                7315, 7323, 7331, 7366, 7390, 7412, 7439, 7447, 7455, 7471, 7498, 2496, 1015, 
														                                1023, 2682, 2798, 2917, 2918, 2690, 2879))))))
                                group by a.nfs_int_numero, f.nfs_int_id, a.nfs_dt_emissao, e.cli_des_grupo ";
                }
                else
                {
                    query = @"declare   @p_uf  int = @uf,   -- 0 = MS   , 1 = SP  , 2 = null
		                                @p_cum int = @cum,  -- 0 = false, 1 = true, 2 = null
		                                @p_exc int = @exc   -- 0 = false, 1 = true, 2 = null

                            select  c.nfs_int_numero as Nota, 
	                                CONVERT(DATE, c.nfs_dt_emissao) as Emissão, 
	                                CONVERT(DATE, a.crb_dt_recebimento) as Recebimento, 
		                            CASE WHEN LTRIM(RTRIM(e.nse_des_estado)) = '' OR e.nse_des_estado IS NULL THEN 'SP' ELSE e.nse_des_estado END as UF, 
	                                case when g.cli_des_grupo = 'CONTRIBUINTE' or g.cli_des_grupo = 'NÃO CONTRIBUINTE' then 'Privado' else 'Publico' end as Destinatario, 
	                                'R$ ' + REPLACE(CONVERT(varchar(100), CONVERT(MONEY, SUM(c.nfs_num_vlrservico))), '.', ',') as 'Valor Serviço', 
	                                'R$ ' + REPLACE(CONVERT(varchar(100), CONVERT(MONEY, SUM(isnull(a.crb_num_valorrecebido, 0)))), '.', ',') as 'Valor Recebido', 
	                                'R$ ' + REPLACE(CONVERT(varchar(100), CONVERT(MONEY, SUM(c.nfs_num_vlrservico - isnull(a.crb_num_valorrecebido, 0)))), '.', ',') as 'Valor Excluido' 
                            from SealContaReceber a 
                            left join SealNotaFiscalRecebida b on a.nfr_int_id = b.nfr_int_id 
                            left join SealNotaFiscalServico c on b.nfs_int_id = c.nfs_int_id
                            left join SealNotaFiscalServicoEmpresa e on c.nse_int_prestador = e.nse_int_id
                            left join SealNotaFiscalServicoEmpresa f on c.nse_int_tomador = f.nse_int_id
                            left join SealCliente g on f.nse_des_cnpj = g.cli_des_cnpj
                            where (a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim) and 
	                                (g.cli_int_id is not null and (g.cli_des_grupo in ('CONTRIB - Lei 9718', 'Ñ CONTRIB - Lei 9718'))) and 
	                                ((@p_uf = 0 and (e.nse_des_estado = 'MS' and e.nse_int_id is not null) and 
			                            ((@p_cum = 0 and (c.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703))) or 
			                                (@p_cum = 1 and (c.nfs_int_listaservico in (702, 103, 105, 107, 104, 106))) or 
			                                (@p_cum = 2 and (c.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703))))) or 
	                                (@p_uf = 1 and ((c.nfs_des_ufmunicipio = 'SP' and e.nse_int_id is null) or c.nse_int_prestador is null) and 
			                            ((@p_cum = 0 and (c.nfs_int_listaservico in (1880, 2119, 2135, 2186, 2194, 3093, 3115, 3450, 3468, 5991, 6009, 7285, 
														                                7315, 7323, 7331, 7366, 7390, 7412, 7439, 7447, 7455, 7471, 7498, 2496))) or 
			                                (@p_cum = 1 and (c.nfs_int_listaservico in (1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879))) or 
			                                (@p_cum = 2 and (c.nfs_int_listaservico in (1880, 2119, 2135, 2186, 2194, 3093, 3115, 3450, 3468, 5991, 6009, 7285, 
														                                7315, 7323, 7331, 7366, 7390, 7412, 7439, 7447, 7455, 7471, 7498, 2496, 
														                                1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879))))) or 
	                                (@p_uf = 2 and 
		                                ((@p_cum = 0 and (c.nfs_int_listaservico in (1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 703 , 1880, 2119, 2135, 2186, 
														                                2194, 3093, 3115, 3450, 3468, 5991, 6009, 7285, 7315, 7323, 7331, 7366, 7390, 
														                                7412, 7439, 7447, 7455, 7471, 7498, 2496))) or 
		                                    (@p_cum = 1 and (c.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1015, 1023, 2682, 2798, 2917, 2918, 2690, 2879))) or 
		                                    (@p_cum = 2 and (c.nfs_int_listaservico in (702, 103, 105, 107, 104, 106, 1402, 2801, 3201, 1701, 1009, 1406, 1401, 1706, 
														                                703 , 1880, 2119, 2135, 2186, 2194, 3093, 3115, 3450, 3468, 5991, 6009, 7285, 
														                                7315, 7323, 7331, 7366, 7390, 7412, 7439, 7447, 7455, 7471, 7498, 2496, 1015, 
														                                1023, 2682, 2798, 2917, 2918, 2690, 2879))))))
                            group by c.nfs_int_numero, c.nfs_dt_emissao, a.crb_dt_recebimento, e.nse_des_estado, g.cli_des_grupo";
                }

                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@inicio", inicio);
                cmd.Parameters.AddWithValue("@fim", fim);
                cmd.Parameters.AddWithValue("@uf", uf);
                cmd.Parameters.AddWithValue("@cum", cum);
                cmd.Parameters.AddWithValue("@exc", exc);

                DbDataReader dr = cmd.ExecuteReader();
                retorno.Load(dr);
            }
            catch (Exception ex)
            {
                erro = "ERRO: " + ex.Message;
            }
            finally
            {
                //dr.Close();
                conn.Close();
            }
            return retorno;
        }


        #region Comments
        //public DataTable GetNRecebidasDanfes(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfd_int_nNf as 'Número', a.nfd_des_natPo as 'Natureza de Operação', 
        //                     a.nfd_dt_dhEmi as 'Data de Emissão', a.nde_des_uf as UF, sum(a.ndi_num_vProd) as Valor from (
        //                    select b.nfd_int_nNf, b.nfd_des_natPo, b.nfd_dt_dhEmi, f.nde_des_uf, a.ndi_num_vProd, a.ndi_int_num 
        //                    from SealNotaFiscalDanfeItem a
        //                    inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
        //                    left join SealNotaFiscalRecebida c on b.nfd_int_id = c.nfd_int_id
        //                    left join SealContaReceber d on c.nfr_int_id = d.nfr_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa f on b.nde_int_emit = f.nde_int_id
        //                    where c.nfr_int_id is null and (f.nde_des_uf = @uf or (@uf = '' and f.nde_des_uf <> @uf)) and 
        //                    (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and
        //                    --(d.crb_dt_recebimento >= @inicio and d.crb_dt_recebimento <= @fim) and 
        //                    (a.ndi_int_cfop = 6102 or a.ndi_int_cfop = 5102 or a.ndi_int_cfop = 6108 or 
        //                    a.ndi_int_cfop = 6119 or a.ndi_int_cfop = 5119 or a.ndi_int_cfop = 6120 or 
        //                    a.ndi_int_cfop = 5120 or a.ndi_int_cfop = 6922 or a.ndi_int_cfop = 5922 or 
        //                    a.ndi_int_cfop = 5405 or a.ndi_int_cfop = 6405 or a.ndi_int_cfop = 6403)
        //                    group by b.nfd_int_nNf, b.nfd_des_natPo, b.nfd_dt_dhEmi, f.nde_des_uf, a.ndi_num_vProd, a.ndi_int_num 
        //                ) a
        //                group by a.nfd_int_nNf, a.nfd_des_natPo, a.nfd_dt_dhEmi, a.nde_des_uf
        //                order by a.nfd_int_nNf desc";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetTotalDanfes(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfd_int_nNf as 'Número', a.nfd_des_natPo as 'Natureza de Operação', 
        //                        a.nfd_dt_dhEmi as 'Data de Emissão', a.nde_des_uf as UF, sum(a.ndi_num_vProd) as Valor from (
        //                        select b.nfd_int_nNf, b.nfd_des_natPo, b.nfd_dt_dhEmi, f.nde_des_uf, a.ndi_num_vProd, a.ndi_int_num 
        //                        from SealNotaFiscalDanfeItem a
        //                        inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
        //                        left join SealNotaFiscalRecebida c on b.nfd_int_id = c.nfd_int_id
        //                        left join SealContaReceber d on c.nfr_int_id = d.nfr_int_id
        //                        inner join SealNotaFiscalDanfeEmpresa f on b.nde_int_emit = f.nde_int_id
        //                        where (f.nde_des_uf = @uf or (@uf = '' and f.nde_des_uf <> @uf)) and 
        //                        (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and
        //                        --(d.crb_dt_recebimento >= @inicio and d.crb_dt_recebimento <= @fim) and 
        //                        (a.ndi_int_cfop = 6102 or a.ndi_int_cfop = 5102 or a.ndi_int_cfop = 6108 or 
        //                        a.ndi_int_cfop = 6119 or a.ndi_int_cfop = 5119 or a.ndi_int_cfop = 6120 or 
        //                        a.ndi_int_cfop = 5120 or a.ndi_int_cfop = 6922 or a.ndi_int_cfop = 5922 or 
        //                        a.ndi_int_cfop = 5405 or a.ndi_int_cfop = 6405 or a.ndi_int_cfop = 6403)
        //                        group by b.nfd_int_nNf, b.nfd_des_natPo, b.nfd_dt_dhEmi, f.nde_des_uf, a.ndi_num_vProd, a.ndi_int_num 
        //                    ) a
        //                    group by a.nfd_int_nNf, a.nfd_des_natPo, a.nfd_dt_dhEmi, a.nde_des_uf
        //                    order by a.nfd_int_nNf desc";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetDanfesPrivadoContrib(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfd_int_nNf as 'Número', a.nfd_des_natPo as 'Natureza de Operação', 
        //                     a.nfd_dt_dhEmi as 'Data de Emissão', a.nde_des_uf as UF, sum(a.ndi_num_vProd) as Valor from (
        //                    select b.nfd_int_nNf, b.nfd_des_natPo, b.nfd_dt_dhEmi, c.nde_des_uf, a.ndi_num_vProd as ndi_num_vProd
        //                    from SealNotaFiscalDanfeItem a
        //                    inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa c on b.nde_int_emit = c.nde_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa d on b.nde_int_dest = d.nde_int_id
        //                    inner join SealCliente e on d.nde_des_cnpj  = e.cli_des_cnpj
        //                    where e.cli_des_grupo = 'CONTRIBUINTE' and c.nde_des_uf = @uf and 
        //                    (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
        //                    (a.ndi_int_cfop = 6102 or a.ndi_int_cfop = 5102 or a.ndi_int_cfop = 6108 or 
        //                    a.ndi_int_cfop = 6119 or a.ndi_int_cfop = 5119 or a.ndi_int_cfop = 6120 or 
        //                    a.ndi_int_cfop = 5120 or a.ndi_int_cfop = 6922 or a.ndi_int_cfop = 5922 or 
        //                    a.ndi_int_cfop = 5405 or a.ndi_int_cfop = 6405 or a.ndi_int_cfop = 6403)
        //                    ) a
        //                    group by a.nfd_int_nNf, a.nfd_des_natPo, a.nfd_dt_dhEmi, a.nde_des_uf
        //                    order by a.nfd_int_nNf desc";
        //        #region old
        //        query = @"select a.nfd_int_nNf as 'Número', a.nfd_des_natPo as 'Natureza de Operação', 
        //                     a.nfd_dt_dhEmi as 'Data de Emissão', a.nde_des_uf as UF, sum(a.ndi_num_vProd) as Valor from (
        //                    select b.nfd_int_nNf, b.nfd_des_natPo, b.nfd_dt_dhEmi, f.nde_des_uf, a.ndi_num_vProd as ndi_num_vProd
        //                    from SealNotaFiscalDanfeItem a
        //                    inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
        //                    inner join SealNotaFiscalRecebida c on b.nfd_int_id = c.nfd_int_id
        //                    inner join SealContaReceber d on c.nfr_int_id = d.nfr_int_id
        //                    inner join SealCliente e on d.crb_des_codigo = e.cli_des_codigo
        //                    inner join SealNotaFiscalDanfeEmpresa f on b.nde_int_emit = f.nde_int_id
        //                    where e.cli_des_grupo = 'CONTRIBUINTE' and f.nde_des_uf = @uf and 
        //                    (d.crb_dt_recebimento >= @inicio and d.crb_dt_recebimento <= @fim) and 
        //                    (a.ndi_int_cfop = 6102 or a.ndi_int_cfop = 5102 or a.ndi_int_cfop = 6108 or 
        //                    a.ndi_int_cfop = 6119 or a.ndi_int_cfop = 5119 or a.ndi_int_cfop = 6120 or 
        //                    a.ndi_int_cfop = 5120 or a.ndi_int_cfop = 6922 or a.ndi_int_cfop = 5922 or 
        //                    a.ndi_int_cfop = 5405 or a.ndi_int_cfop = 6405 or a.ndi_int_cfop = 6403)
        //                    ) a
        //                    group by a.nfd_int_nNf, a.nfd_des_natPo, a.nfd_dt_dhEmi, a.nde_des_uf
        //                    order by a.nfd_int_nNf desc";
        //        #endregion

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetDanfesPrivadoNContrib(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfd_int_nNf as 'Número', a.nfd_des_natPo as 'Natureza de Operação', 
        //                     a.nfd_dt_dhEmi as 'Data de Emissão', a.nde_des_uf as UF, sum(a.ndi_num_vProd) as Valor from (
        //                    select b.nfd_int_nNf, b.nfd_des_natPo, b.nfd_dt_dhEmi, c.nde_des_uf, a.ndi_num_vProd as ndi_num_vProd 
        //                    from SealNotaFiscalDanfeItem a
        //                    inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa c on b.nde_int_emit = c.nde_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa d on b.nde_int_dest = d.nde_int_id
        //                    inner join SealCliente e on d.nde_des_cnpj  = e.cli_des_cnpj
        //                    where e.cli_des_grupo = 'NÃO CONTRIBUINTE' and c.nde_des_uf = @uf and 
        //                    (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
        //                    (a.ndi_int_cfop = 6102 or a.ndi_int_cfop = 5102 or a.ndi_int_cfop = 6108 or 
        //                    a.ndi_int_cfop = 6119 or a.ndi_int_cfop = 5119 or a.ndi_int_cfop = 6120 or 
        //                    a.ndi_int_cfop = 5120 or a.ndi_int_cfop = 6922 or a.ndi_int_cfop = 5922 or 
        //                    a.ndi_int_cfop = 5405 or a.ndi_int_cfop = 6405 or a.ndi_int_cfop = 6403)
        //                    ) a
        //                    group by a.nfd_int_nNf, a.nfd_des_natPo, a.nfd_dt_dhEmi, a.nde_des_uf
        //                    order by a.nfd_int_nNf desc";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetDanfesPrivado(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfd_int_nNf as 'Número', a.nfd_des_natPo as 'Natureza de Operação', 
        //                     a.nfd_dt_dhEmi as 'Data de Emissão', a.nde_des_uf as UF, sum(a.ndi_num_vProd) as Valor from (
        //                    select b.nfd_int_nNf, b.nfd_des_natPo, b.nfd_dt_dhEmi, c.nde_des_uf, a.ndi_num_vProd as ndi_num_vProd 
        //                    from SealNotaFiscalDanfeItem a
        //                    inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa c on b.nde_int_emit = c.nde_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa d on b.nde_int_dest = d.nde_int_id
        //                    inner join SealCliente e on d.nde_des_cnpj  = e.cli_des_cnpj
        //                    where (e.cli_des_grupo = 'CONTRIBUINTE' or e.cli_des_grupo = 'NÃO CONTRIBUINTE') and c.nde_des_uf = @uf and 
        //                    (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
        //                    (a.ndi_int_cfop = 6102 or a.ndi_int_cfop = 5102 or a.ndi_int_cfop = 6108 or 
        //                    a.ndi_int_cfop = 6119 or a.ndi_int_cfop = 5119 or a.ndi_int_cfop = 6120 or 
        //                    a.ndi_int_cfop = 5120 or a.ndi_int_cfop = 6922 or a.ndi_int_cfop = 5922 or 
        //                    a.ndi_int_cfop = 5405 or a.ndi_int_cfop = 6405 or a.ndi_int_cfop = 6403)
        //                    ) a
        //                    group by a.nfd_int_nNf, a.nfd_des_natPo, a.nfd_dt_dhEmi, a.nde_des_uf
        //                    order by a.nfd_int_nNf desc";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetDanfesPublicoContrib(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfd_int_nNf as 'Número', a.nfd_des_natPo as 'Natureza de Operação', 
        //                     a.nfd_dt_dhEmi as 'Data de Emissão', a.nde_des_uf as UF, sum(a.ndi_num_vProd) as Valor from (
        //                    select b.nfd_int_nNf, b.nfd_des_natPo, b.nfd_dt_dhEmi, c.nde_des_uf, a.ndi_num_vProd as ndi_num_vProd 
        //                    from SealNotaFiscalDanfeItem a
        //                    inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa c on b.nde_int_emit = c.nde_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa d on b.nde_int_dest = d.nde_int_id
        //                    inner join SealCliente e on d.nde_des_cnpj  = e.cli_des_cnpj
        //                    where e.cli_des_grupo = 'CONTRIB - Lei 9718' and c.nde_des_uf = @uf and 
        //                    (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
        //                    (a.ndi_int_cfop = 6102 or a.ndi_int_cfop = 5102 or a.ndi_int_cfop = 6108 or 
        //                    a.ndi_int_cfop = 6119 or a.ndi_int_cfop = 5119 or a.ndi_int_cfop = 6120 or 
        //                    a.ndi_int_cfop = 5120 or a.ndi_int_cfop = 6922 or a.ndi_int_cfop = 5922 or 
        //                    a.ndi_int_cfop = 5405 or a.ndi_int_cfop = 6405 or a.ndi_int_cfop = 6403)
        //                    ) a
        //                    group by a.nfd_int_nNf, a.nfd_des_natPo, a.nfd_dt_dhEmi, a.nde_des_uf
        //                    order by a.nfd_int_nNf desc
        //                    ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetDanfesPublicoNContrib(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfd_int_nNf as 'Número', a.nfd_des_natPo as 'Natureza de Operação', 
        //                     a.nfd_dt_dhEmi as 'Data de Emissão', a.nde_des_uf as UF, sum(a.ndi_num_vProd) as Valor from (
        //                    select b.nfd_int_nNf, b.nfd_des_natPo, b.nfd_dt_dhEmi, c.nde_des_uf, a.ndi_num_vProd as ndi_num_vProd 
        //                    from SealNotaFiscalDanfeItem a
        //                    inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa c on b.nde_int_emit = c.nde_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa d on b.nde_int_dest = d.nde_int_id
        //                    inner join SealCliente e on d.nde_des_cnpj  = e.cli_des_cnpj
        //                    where e.cli_des_grupo = 'Ñ CONTRIB - Lei 9718' and c.nde_des_uf = @uf and 
        //                    (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
        //                    (a.ndi_int_cfop = 6102 or a.ndi_int_cfop = 5102 or a.ndi_int_cfop = 6108 or 
        //                    a.ndi_int_cfop = 6119 or a.ndi_int_cfop = 5119 or a.ndi_int_cfop = 6120 or 
        //                    a.ndi_int_cfop = 5120 or a.ndi_int_cfop = 6922 or a.ndi_int_cfop = 5922 or 
        //                    a.ndi_int_cfop = 5405 or a.ndi_int_cfop = 6405 or a.ndi_int_cfop = 6403)
        //                    ) a
        //                    group by a.nfd_int_nNf, a.nfd_des_natPo, a.nfd_dt_dhEmi, a.nde_des_uf
        //                    order by a.nfd_int_nNf desc";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetDanfesPublico(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfd_int_nNf as 'Número', a.nfd_des_natPo as 'Natureza de Operação', 
        //                     a.nfd_dt_dhEmi as 'Data de Emissão', a.nde_des_uf as UF, sum(a.ndi_num_vProd) as Valor from (
        //                    select b.nfd_int_nNf, b.nfd_des_natPo, b.nfd_dt_dhEmi, c.nde_des_uf, a.ndi_num_vProd as ndi_num_vProd 
        //                    from SealNotaFiscalDanfeItem a
        //                    inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa c on b.nde_int_emit = c.nde_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa d on b.nde_int_dest = d.nde_int_id
        //                    inner join SealCliente e on d.nde_des_cnpj  = e.cli_des_cnpj
        //                    where (e.cli_des_grupo = 'CONTRIB - Lei 9718' or e.cli_des_grupo = 'Ñ CONTRIB - Lei 9718') and c.nde_des_uf = @uf and 
        //                    (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim) and 
        //                    (a.ndi_int_cfop = 6102 or a.ndi_int_cfop = 5102 or a.ndi_int_cfop = 6108 or 
        //                    a.ndi_int_cfop = 6119 or a.ndi_int_cfop = 5119 or a.ndi_int_cfop = 6120 or 
        //                    a.ndi_int_cfop = 5120 or a.ndi_int_cfop = 6922 or a.ndi_int_cfop = 5922 or 
        //                    a.ndi_int_cfop = 5405 or a.ndi_int_cfop = 6405 or a.ndi_int_cfop = 6403)
        //                    ) a
        //                    group by a.nfd_int_nNf, a.nfd_des_natPo, a.nfd_dt_dhEmi, a.nde_des_uf
        //                    order by a.nfd_int_nNf desc
        //                    ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetDanfesPublicoExcluido(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfd_int_nNf as 'Número', a.nfd_des_natPo as 'Natureza de Operação', 
        //                     a.nfd_dt_dhEmi as 'Data de Emissão', a.nde_des_uf as UF, sum(a.ndi_num_vProd) as Valor from (
        //                    select b.nfd_int_nNf, b.nfd_des_natPo, b.nfd_dt_dhEmi, d.nde_des_uf, a.ndi_num_vProd as ndi_num_vProd
        //                    from SealNotaFiscalDanfeItem a
        //                    inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
        //                    left join SealNotaFiscalRecebida c on b.nfd_int_id = c.nfd_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa d on b.nde_int_emit = d.nde_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa e on b.nde_int_dest = e.nde_int_id
        //                    inner join SealCliente f on e.nde_des_cnpj  = f.cli_des_cnpj
        //                    where (f.cli_des_grupo = 'CONTRIB - Lei 9718' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718') and 
        //                    c.nfr_int_id is null and d.nde_des_uf = @uf and (b.nfd_dt_dhEmi >= @inicio and b.nfd_dt_dhEmi <= @fim)
        //                    and (a.ndi_int_cfop = 6102 or a.ndi_int_cfop = 5102 or a.ndi_int_cfop = 6108 or 
        //                    a.ndi_int_cfop = 6119 or a.ndi_int_cfop = 5119 or a.ndi_int_cfop = 6120 or 
        //                    a.ndi_int_cfop = 5120 or a.ndi_int_cfop = 6922 or a.ndi_int_cfop = 5922 or 
        //                    a.ndi_int_cfop = 5405 or a.ndi_int_cfop = 6405 or a.ndi_int_cfop = 6403)
        //                    ) a
        //                    group by a.nfd_int_nNf, a.nfd_des_natPo, a.nfd_dt_dhEmi, a.nde_des_uf
        //                    order by a.nfd_int_nNf desc
        //                    ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetDanfesPublicoAdicao(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfd_int_nNf as 'Número', a.nfd_des_natPo as 'Natureza de Operação', 
        //                     a.nfd_dt_dhEmi as 'Data de Emissão', a.nde_des_uf as UF, sum(a.ndi_num_vProd) as Valor from (
        //                    select b.nfd_int_nNf, b.nfd_des_natPo, b.nfd_dt_dhEmi, d.nde_des_uf, a.ndi_num_vProd as ndi_num_vProd
        //                    from SealNotaFiscalDanfeItem a
        //                    inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
        //                    left join SealNotaFiscalRecebida c on b.nfd_int_id = c.nfd_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa d on b.nde_int_emit = d.nde_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa e on b.nde_int_dest = e.nde_int_id
        //                    inner join SealCliente f on e.nde_des_cnpj  = f.cli_des_cnpj
        //                    where (f.cli_des_grupo = 'CONTRIB - Lei 9718' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718') and 
        //                    c.nfr_int_id is not null and d.nde_des_uf = @uf and b.nfd_dt_dhEmi < @inicio 
        //                    and (a.ndi_int_cfop = 6102 or a.ndi_int_cfop = 5102 or a.ndi_int_cfop = 6108 or 
        //                    a.ndi_int_cfop = 6119 or a.ndi_int_cfop = 5119 or a.ndi_int_cfop = 6120 or 
        //                    a.ndi_int_cfop = 5120 or a.ndi_int_cfop = 6922 or a.ndi_int_cfop = 5922 or 
        //                    a.ndi_int_cfop = 5405 or a.ndi_int_cfop = 6405 or a.ndi_int_cfop = 6403)
        //                    ) a
        //                    group by a.nfd_int_nNf, a.nfd_des_natPo, a.nfd_dt_dhEmi, a.nde_des_uf
        //                    order by a.nfd_int_nNf desc";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetDanfesPublicoSaldo(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfd_int_nNf as 'Número', a.nfd_des_natPo as 'Natureza de Operação', 
        //                     a.nfd_dt_dhEmi as 'Data de Emissão', a.nde_des_uf as UF, sum(a.ndi_num_vProd) as Valor from (
        //                    select b.nfd_int_nNf, b.nfd_des_natPo, b.nfd_dt_dhEmi, d.nde_des_uf, a.ndi_num_vProd as ndi_num_vProd
        //                    from SealNotaFiscalDanfeItem a
        //                    inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
        //                    left join SealNotaFiscalRecebida c on b.nfd_int_id = c.nfd_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa d on b.nde_int_emit = d.nde_int_id
        //                    inner join SealNotaFiscalDanfeEmpresa e on b.nde_int_dest = e.nde_int_id
        //                    inner join SealCliente f on e.nde_des_cnpj  = f.cli_des_cnpj
        //                    where (f.cli_des_grupo = 'CONTRIB - Lei 9718' or f.cli_des_grupo = 'Ñ CONTRIB - Lei 9718') and 
        //                    d.nde_des_uf = @uf and b.nfd_dt_dhEmi < @inicio and 
        //                    (a.ndi_int_cfop = 6102 or a.ndi_int_cfop = 5102 or a.ndi_int_cfop = 6108 or 
        //                    a.ndi_int_cfop = 6119 or a.ndi_int_cfop = 5119 or a.ndi_int_cfop = 6120 or 
        //                    a.ndi_int_cfop = 5120 or a.ndi_int_cfop = 6922 or a.ndi_int_cfop = 5922 or 
        //                    a.ndi_int_cfop = 5405 or a.ndi_int_cfop = 6405 or a.ndi_int_cfop = 6403)
        //                    ) a
        //                    group by a.nfd_int_nNf, a.nfd_des_natPo, a.nfd_dt_dhEmi, a.nde_des_uf
        //                    order by a.nfd_int_nNf desc";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}


        //public DataTable GetServicoRecebidas(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfs_int_numero as 'Número', a.nfs_des_dicriminacao as 'Natureza de Operação', 
        //                     a.nfs_dt_emissao as 'Data de Emissão', a.nfs_des_ufmunicipio as UF, sum(a.nfs_num_vlrservico) as Valor from (
        //                    select a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio, a.nfs_num_vlrservico 
        //                --select SUM(a.nfs_num_vlrservico) as Valor
        //                from SealNotaFiscalServico a
        //                left join SealNotaFiscalRecebida b on a.nfs_int_id = b.nfs_int_id
        //                where b.nfr_int_id is not null and (a.nfs_des_ufmunicipio = @uf or (@uf = 'SP' and a.nfs_des_ufmunicipio != 'MS'))
        //                and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) 
        //                ) a
        //                group by a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio 
        //                order by a.nfs_int_numero desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetServicoNRecebidas(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfs_int_numero as 'Número', a.nfs_des_dicriminacao as 'Natureza de Operação', 
        //                     a.nfs_dt_emissao as 'Data de Emissão', a.nfs_des_ufmunicipio as UF, sum(a.nfs_num_vlrservico) as Valor from (
        //                    select a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio, a.nfs_num_vlrservico 
        //                --select SUM(a.nfs_num_vlrservico) as Valor
        //                from SealNotaFiscalServico a
        //                left join SealNotaFiscalRecebida b on a.nfs_int_id = b.nfs_int_id
        //                where b.nfr_int_id is null and (a.nfs_des_ufmunicipio = @uf or (@uf = 'SP' and a.nfs_des_ufmunicipio != 'MS'))
        //                and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) 
        //                ) a
        //                group by a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio 
        //                order by a.nfs_int_numero desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetServicoTotal(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfs_int_numero as 'Número', a.nfs_des_dicriminacao as 'Natureza de Operação', 
        //                         a.nfs_dt_emissao as 'Data de Emissão', a.nfs_des_ufmunicipio as UF, sum(a.nfs_num_vlrservico) as Valor from (
        //                        select a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio, a.nfs_num_vlrservico 
        //                    --select SUM(a.nfs_num_vlrservico) as Valor
        //                    from SealNotaFiscalServico a
        //                    left join SealNotaFiscalServicoEmpresa b on a.nse_int_prestador = b.nse_int_id
        //                    where ((a.nfs_des_ufmunicipio = @uf and b.nse_int_id is not null) or (@uf = 'SP' and a.nse_int_prestador is null))
        //                    and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) 
        //                    ) a
        //                    group by a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio 
        //                    order by a.nfs_int_numero desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetServicoPrivadoCum(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfs_int_numero as 'Número', a.nfs_des_dicriminacao as 'Natureza de Operação', 
        //                     a.nfs_dt_emissao as 'Data de Emissão', a.nfs_des_ufmunicipio as UF, sum(a.nfs_num_vlrservico) as Valor, a.nfs_int_listaservico from (
        //                    select a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio, a.nfs_num_vlrservico, a.nfs_int_listaservico
        //                --select SUM(a.nfs_num_vlrservico) as Valor
        //                from SealNotaFiscalServico a
        //                left join SealNotaFiscalServicoEmpresa b on a.nse_int_prestador = b.nse_int_id
        //                left join SealNotaFiscalServicoEmpresa c on a.nse_int_tomador = c.nse_int_id
        //                inner join SealCliente d on c.nse_des_cnpj  = d.cli_des_cnpj
        //                where (d.cli_des_grupo = 'CONTRIBUINTE' or d.cli_des_grupo = 'NÃO CONTRIBUINTE') and 
        //                (a.nfs_int_listaservico = 702 or a.nfs_int_listaservico = 103 or a.nfs_int_listaservico = 105 or 
        //                a.nfs_int_listaservico = 107 or a.nfs_int_listaservico = 104 or a.nfs_int_listaservico = 106) 
        //                and 
        //                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) 
        //                and ((a.nfs_des_ufmunicipio = @uf and b.nse_int_id is not null) or (@uf = 'SP' and a.nse_int_prestador is null))
        //                ) a
        //                group by a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio , a.nfs_num_vlrservico, a.nfs_int_listaservico 
        //                order by a.nfs_int_numero desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetServicoPrivadoNCum(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfs_int_numero as 'Número', a.nfs_des_dicriminacao as 'Natureza de Operação', 
        //                     a.nfs_dt_emissao as 'Data de Emissão', a.nfs_des_ufmunicipio as UF, sum(a.nfs_num_vlrservico) as Valor from (
        //                    select a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio, a.nfs_num_vlrservico 
        //                --select SUM(a.nfs_num_vlrservico) as Valor
        //                from SealNotaFiscalServico a
        //                left join SealNotaFiscalServicoEmpresa b on a.nse_int_prestador = b.nse_int_id
        //                left join SealNotaFiscalServicoEmpresa c on a.nse_int_tomador = c.nse_int_id
        //                inner join SealCliente d on c.nse_des_cnpj  = d.cli_des_cnpj
        //                where (d.cli_des_grupo = 'CONTRIBUINTE' or d.cli_des_grupo = 'NÃO CONTRIBUINTE') and 
        //                (a.nfs_int_listaservico = 1402 or a.nfs_int_listaservico = 2801 or a.nfs_int_listaservico = 3201 or 
        //                a.nfs_int_listaservico = 1701 or a.nfs_int_listaservico = 1717 or a.nfs_int_listaservico = 1009 or 
        //                a.nfs_int_listaservico = 1406 or a.nfs_int_listaservico = 1401 or a.nfs_int_listaservico = 1706 or a.nfs_int_listaservico = 703) 
        //                and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) 
        //                and ((a.nfs_des_ufmunicipio = @uf and b.nse_int_id is not null) or (@uf = 'SP' and a.nse_int_prestador is null))
        //                ) a
        //                group by a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio 
        //                order by a.nfs_int_numero desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetServicoPrivado(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfs_int_numero as 'Número', a.nfs_des_dicriminacao as 'Natureza de Operação', 
        //                     a.nfs_dt_emissao as 'Data de Emissão', a.nfs_des_ufmunicipio as UF, sum(a.nfs_num_vlrservico) as Valor from (
        //                    select a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio, a.nfs_num_vlrservico 
        //                --select SUM(a.nfs_num_vlrservico) as Valor
        //                from SealNotaFiscalServico a
        //                left join SealNotaFiscalServicoEmpresa b on a.nse_int_prestador = b.nse_int_id
        //                where (a.nfs_int_listaservico = 702 or a.nfs_int_listaservico = 103 or a.nfs_int_listaservico = 105 or 
        //                a.nfs_int_listaservico = 107 or a.nfs_int_listaservico = 104 or a.nfs_int_listaservico = 106 or 
        //                a.nfs_int_listaservico = 1402 or a.nfs_int_listaservico = 2801 or a.nfs_int_listaservico = 3201 or 
        //                a.nfs_int_listaservico = 1701 or a.nfs_int_listaservico = 1717 or a.nfs_int_listaservico = 1009 or 
        //                a.nfs_int_listaservico = 1406 or a.nfs_int_listaservico = 1401 or a.nfs_int_listaservico = 1706) 
        //                and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) 
        //                and ((a.nfs_des_ufmunicipio = @uf and b.nse_int_id is not null) or (@uf = 'SP' and a.nse_int_prestador is null))
        //                ) a
        //                group by a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio 
        //                order by a.nfs_int_numero desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetServicoPublicoCum(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfs_int_numero as 'Número', a.nfs_des_dicriminacao as 'Natureza de Operação', 
        //                     a.nfs_dt_emissao as 'Data de Emissão', a.nfs_des_ufmunicipio as UF, sum(a.nfs_num_vlrservico) as Valor, a.nfs_int_listaservico from (
        //                    select a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio, a.nfs_num_vlrservico, a.nfs_int_listaservico
        //                --select SUM(a.nfs_num_vlrservico) as Valor
        //                from SealNotaFiscalServico a
        //                left join SealNotaFiscalServicoEmpresa b on a.nse_int_prestador = b.nse_int_id
        //                left join SealNotaFiscalServicoEmpresa c on a.nse_int_tomador = c.nse_int_id
        //                inner join SealCliente d on c.nse_des_cnpj  = d.cli_des_cnpj
        //                where (d.cli_des_grupo = 'CONTRIB - Lei 9718' or d.cli_des_grupo = 'Ñ CONTRIB - Lei 9718') and 
        //                (a.nfs_int_listaservico = 702 or a.nfs_int_listaservico = 103 or a.nfs_int_listaservico = 105 or 
        //                a.nfs_int_listaservico = 107 or a.nfs_int_listaservico = 104 or a.nfs_int_listaservico = 106) 
        //                and 
        //                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) 
        //                and ((a.nfs_des_ufmunicipio = @uf and b.nse_int_id is not null) or (@uf = 'SP' and a.nse_int_prestador is null))
        //                ) a
        //                group by a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio , a.nfs_num_vlrservico, a.nfs_int_listaservico 
        //                order by a.nfs_int_numero desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetServicoPublicoNCum(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfs_int_numero as 'Número', a.nfs_des_dicriminacao as 'Natureza de Operação', 
        //                     a.nfs_dt_emissao as 'Data de Emissão', a.nfs_des_ufmunicipio as UF, sum(a.nfs_num_vlrservico) as Valor, a.nfs_int_listaservico from (
        //                    select a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio, a.nfs_num_vlrservico, a.nfs_int_listaservico
        //                --select SUM(a.nfs_num_vlrservico) as Valor
        //                from SealNotaFiscalServico a
        //                left join SealNotaFiscalServicoEmpresa b on a.nse_int_prestador = b.nse_int_id
        //                left join SealNotaFiscalServicoEmpresa c on a.nse_int_tomador = c.nse_int_id
        //                inner join SealCliente d on c.nse_des_cnpj  = d.cli_des_cnpj
        //                where (d.cli_des_grupo = 'CONTRIB - Lei 9718' or d.cli_des_grupo = 'Ñ CONTRIB - Lei 9718') and 
        //                (a.nfs_int_listaservico = 1402 or a.nfs_int_listaservico = 2801 or a.nfs_int_listaservico = 3201 or 
        //                a.nfs_int_listaservico = 1701 or a.nfs_int_listaservico = 1717 or a.nfs_int_listaservico = 1009 or 
        //                a.nfs_int_listaservico = 1406 or a.nfs_int_listaservico = 1401 or a.nfs_int_listaservico = 1706 or a.nfs_int_listaservico = 703) 
        //                and 
        //                (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) 
        //                and ((a.nfs_des_ufmunicipio = @uf and b.nse_int_id is not null) or (@uf = 'SP' and a.nse_int_prestador is null))
        //                ) a
        //                group by a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio , a.nfs_num_vlrservico, a.nfs_int_listaservico 
        //                order by a.nfs_int_numero desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetServicoPublico(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfd_int_nNf as 'Número', a.nfd_des_natPo as 'Natureza de Operação', 
        //                     a.nfd_dt_dhEmi as 'Data de Emissão', a.nde_des_uf as UF, sum(a.ndi_num_vProd) as Valor from (
        //                    select b.nfd_int_nNf, b.nfd_des_natPo, b.nfd_dt_dhEmi, f.nde_des_uf, a.ndi_num_vProd as ndi_num_vProd
        //                    from SealNotaFiscalDanfeItem a
        //                    inner join SealNotaFiscalDanfe b on a.nfd_int_id = b.nfd_int_id
        //                    inner join SealNotaFiscalRecebida c on b.nfd_int_id = c.nfd_int_id
        //                    inner join SealContaReceber d on c.nfr_int_id = d.nfr_int_id
        //                    inner join SealCliente e on d.crb_des_codigo = e.cli_des_codigo
        //                    inner join SealNotaFiscalDanfeEmpresa f on b.nde_int_emit = f.nde_int_id
        //                    where f.nde_des_uf = @uf and (d.crb_dt_recebimento >= @inicio and d.crb_dt_recebimento <= @fim) and 
        //                    (a.nfs_int_listaservico = 702 or a.nfs_int_listaservico = 103 or a.nfs_int_listaservico = 105 or 
        //                    a.nfs_int_listaservico = 107 or a.nfs_int_listaservico = 104 or a.nfs_int_listaservico = 106 or 
        //                    a.nfs_int_listaservico = 1402 or a.nfs_int_listaservico = 2801 or a.nfs_int_listaservico = 3201 or 
        //                    a.nfs_int_listaservico = 1701 or a.nfs_int_listaservico = 1717 or a.nfs_int_listaservico = 1009 or 
        //                    a.nfs_int_listaservico = 1406 or a.nfs_int_listaservico = 1401 or a.nfs_int_listaservico = 1706) 
        //                    ) a
        //                    group by a.nfd_int_nNf, a.nfd_des_natPo, a.nfd_dt_dhEmi, a.nde_des_uf
        //                    order by a.nfd_int_nNf desc";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetServicoPublicoExcluido(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfs_int_numero as 'Número', a.nfs_des_dicriminacao as 'Natureza de Operação', 
        //                     a.nfs_dt_emissao as 'Data de Emissão', a.nfs_des_ufmunicipio as UF, sum(a.nfs_num_vlrservico) as Valor from (
        //                    select a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio, a.nfs_num_vlrservico 
        //                from SealNotaFiscalServico a
        //                left join SealNotaFiscalServicoEmpresa b on a.nse_int_prestador = b.nse_int_id
        //                left join SealNotaFiscalServicoEmpresa c on a.nse_int_tomador = c.nse_int_id
        //                inner join SealCliente d on c.nse_des_cnpj  = d.cli_des_cnpj
        //                left join SealNotaFiscalRecebida e on a.nfs_int_id = e.nfs_int_id
        //                where (d.cli_des_grupo = 'CONTRIB - Lei 9718' or d.cli_des_grupo = 'Ñ CONTRIB - Lei 9718') and 
        //                (a.nfs_int_listaservico = 702 or a.nfs_int_listaservico = 103 or a.nfs_int_listaservico = 105 or 
        //                a.nfs_int_listaservico = 107 or a.nfs_int_listaservico = 104 or a.nfs_int_listaservico = 106 or 
        //                a.nfs_int_listaservico = 1402 or a.nfs_int_listaservico = 2801 or a.nfs_int_listaservico = 3201 or 
        //                a.nfs_int_listaservico = 1701 or a.nfs_int_listaservico = 1717 or a.nfs_int_listaservico = 1009 or 
        //                a.nfs_int_listaservico = 1406 or a.nfs_int_listaservico = 1401 or a.nfs_int_listaservico = 1706) 
        //                and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and e.nfr_int_id is null 
        //                and ((a.nfs_des_ufmunicipio = @uf and b.nse_int_id is not null) or (@uf = 'SP' and a.nse_int_prestador is null))
        //                ) a
        //                group by a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio 
        //                order by a.nfs_int_numero desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetServicoPublicoAdicao(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfs_int_numero as 'Número', a.nfs_des_dicriminacao as 'Natureza de Operação', 
        //                     a.nfs_dt_emissao as 'Data de Emissão', a.nfs_des_ufmunicipio as UF, sum(a.nfs_num_vlrservico) as Valor from (
        //                    select a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio, a.nfs_num_vlrservico 
        //                from SealNotaFiscalServico a
        //                left join SealNotaFiscalServicoEmpresa b on a.nse_int_prestador = b.nse_int_id
        //                left join SealNotaFiscalServicoEmpresa c on a.nse_int_tomador = c.nse_int_id
        //                inner join SealCliente d on c.nse_des_cnpj  = d.cli_des_cnpj
        //                left join SealNotaFiscalRecebida e on a.nfs_int_id = e.nfs_int_id
        //                where (d.cli_des_grupo = 'CONTRIB - Lei 9718' or d.cli_des_grupo = 'Ñ CONTRIB - Lei 9718') and 
        //                (a.nfs_int_listaservico = 702 or a.nfs_int_listaservico = 103 or a.nfs_int_listaservico = 105 or 
        //                a.nfs_int_listaservico = 107 or a.nfs_int_listaservico = 104 or a.nfs_int_listaservico = 106 or 
        //                a.nfs_int_listaservico = 1402 or a.nfs_int_listaservico = 2801 or a.nfs_int_listaservico = 3201 or 
        //                a.nfs_int_listaservico = 1701 or a.nfs_int_listaservico = 1717 or a.nfs_int_listaservico = 1009 or 
        //                a.nfs_int_listaservico = 1406 or a.nfs_int_listaservico = 1401 or a.nfs_int_listaservico = 1706) 
        //                and a.nfs_dt_emissao < @inicio and e.nfr_int_id is not null 
        //                and ((a.nfs_des_ufmunicipio = @uf and b.nse_int_id is not null) or (@uf = 'SP' and a.nse_int_prestador is null))
        //                ) a
        //                group by a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio 
        //                order by a.nfs_int_numero desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetServicoPublicoSaldo(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.nfs_int_numero as 'Número', a.nfs_des_dicriminacao as 'Natureza de Operação', 
        //                     a.nfs_dt_emissao as 'Data de Emissão', a.nfs_des_ufmunicipio as UF, sum(a.nfs_num_vlrservico) as Valor from (
        //                    select a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio, a.nfs_num_vlrservico 
        //                from SealNotaFiscalServico a
        //                left join SealNotaFiscalServicoEmpresa b on a.nse_int_prestador = b.nse_int_id
        //                left join SealNotaFiscalServicoEmpresa c on a.nse_int_tomador = c.nse_int_id
        //                inner join SealCliente d on c.nse_des_cnpj  = d.cli_des_cnpj
        //                left join SealNotaFiscalRecebida e on a.nfs_int_id = e.nfs_int_id
        //                where (d.cli_des_grupo = 'CONTRIB - Lei 9718' or d.cli_des_grupo = 'Ñ CONTRIB - Lei 9718') and 
        //                (a.nfs_int_listaservico = 702 or a.nfs_int_listaservico = 103 or a.nfs_int_listaservico = 105 or 
        //                a.nfs_int_listaservico = 107 or a.nfs_int_listaservico = 104 or a.nfs_int_listaservico = 106 or 
        //                a.nfs_int_listaservico = 1402 or a.nfs_int_listaservico = 2801 or a.nfs_int_listaservico = 3201 or 
        //                a.nfs_int_listaservico = 1701 or a.nfs_int_listaservico = 1717 or a.nfs_int_listaservico = 1009 or 
        //                a.nfs_int_listaservico = 1406 or a.nfs_int_listaservico = 1401 or a.nfs_int_listaservico = 1706) 
        //                and (a.nfs_dt_emissao >= @inicio and a.nfs_dt_emissao <= @fim) and e.nfr_int_id is null 
        //                and ((a.nfs_des_ufmunicipio = @uf and b.nse_int_id is not null) or (@uf = 'SP' and a.nse_int_prestador is null))
        //                ) a
        //                group by a.nfs_int_numero, a.nfs_des_dicriminacao, a.nfs_dt_emissao, a.nfs_des_ufmunicipio 
        //                order by a.nfs_int_numero desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}


        //public DataTable GetRelPrivadoItau(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.crb_int_notafiscal as 'Número', a.crb_des_observacao as 'Natureza de Operação', 
        //                         a.crb_dt_emissao as 'Data de Emissão', sum(a.crb_num_valorrecebido) as Valor from (
        //                        select a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    from SealContaReceber a 
        //                    inner join SealCliente b on a.crb_des_codigo = b.cli_des_codigo
        //                    where (b.cli_des_grupo = 'CONTRIBUINTE' or b.cli_des_grupo = 'NÃO CONTRIBUINTE') and
        //                    (a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim) and 
        //                    (a.crb_des_observacao = 'Banco Itau S/A   -  02539-2' or a.crb_des_observacao = 'Banco Itau Vinculada   -  06448-2')
        //                    ) a
        //                    group by a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    order by a.crb_int_notafiscal desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        //cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetRelPrivadoBB(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.crb_int_notafiscal as 'Número', a.crb_des_observacao as 'Natureza de Operação', 
        //                         a.crb_dt_emissao as 'Data de Emissão', sum(a.crb_num_valorrecebido) as Valor from (
        //                        select a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    from SealContaReceber a 
        //                    inner join SealCliente b on a.crb_des_codigo = b.cli_des_codigo
        //                    where (b.cli_des_grupo = 'CONTRIBUINTE' or b.cli_des_grupo = 'NÃO CONTRIBUINTE') and
        //                    (a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim) and 
        //                    (a.crb_des_observacao = 'Banco Do Brasil   -  18642-2' or a.crb_des_observacao = 'Banco do Brasil - 6034-8')
        //                    ) a
        //                    group by a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    order by a.crb_int_notafiscal desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        //cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetRelPrivadoCaixa(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.crb_int_notafiscal as 'Número', a.crb_des_observacao as 'Natureza de Operação', 
        //                         a.crb_dt_emissao as 'Data de Emissão', sum(a.crb_num_valorrecebido) as Valor from (
        //                        select a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    from SealContaReceber a 
        //                    inner join SealCliente b on a.crb_des_codigo = b.cli_des_codigo
        //                    where (b.cli_des_grupo = 'CONTRIBUINTE' or b.cli_des_grupo = 'NÃO CONTRIBUINTE') and
        //                    (a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim) and 
        //                    (a.crb_des_observacao = 'Caixa Economica - 0274')
        //                    ) a
        //                    group by a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    order by a.crb_int_notafiscal desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        //cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetRelPrivadoBradesco(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.crb_int_notafiscal as 'Número', a.crb_des_observacao as 'Natureza de Operação', 
        //                         a.crb_dt_emissao as 'Data de Emissão', sum(a.crb_num_valorrecebido) as Valor from (
        //                        select a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    from SealContaReceber a 
        //                    inner join SealCliente b on a.crb_des_codigo = b.cli_des_codigo
        //                    where (b.cli_des_grupo = 'CONTRIBUINTE' or b.cli_des_grupo = 'NÃO CONTRIBUINTE') and
        //                    (a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim) and 
        //                    (a.crb_des_observacao = 'Banco Bradesco   -  107565')
        //                    ) a
        //                    group by a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    order by a.crb_int_notafiscal desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        //cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetRelPrivadoDaycoval(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.crb_int_notafiscal as 'Número', a.crb_des_observacao as 'Natureza de Operação', 
        //                         a.crb_dt_emissao as 'Data de Emissão', sum(a.crb_num_valorrecebido) as Valor from (
        //                        select a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    from SealContaReceber a 
        //                    inner join SealCliente b on a.crb_des_codigo = b.cli_des_codigo
        //                    where (b.cli_des_grupo = 'CONTRIBUINTE' or b.cli_des_grupo = 'NÃO CONTRIBUINTE') and
        //                    (a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim) and 
        //                    (a.crb_des_observacao = 'Banco Daycoval S.A - 716415-9' or a.crb_des_observacao = 'Banco Daycoval S.A.   -  708077-0')
        //                    ) a
        //                    group by a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    order by a.crb_int_notafiscal desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        //cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetRelPrivadoDesconto(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.crb_int_notafiscal as 'Número', a.crb_des_observacao as 'Natureza de Operação', 
        //                         a.crb_dt_emissao as 'Data de Emissão', sum(a.crb_num_valorrecebido) as Valor from (
        //                        select a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    from SealContaReceber a 
        //                    inner join SealCliente b on a.crb_des_codigo = b.cli_des_codigo
        //                    where (b.cli_des_grupo = 'CONTRIBUINTE' or b.cli_des_grupo = 'NÃO CONTRIBUINTE') and
        //                    (a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim) and 
        //                    (a.crb_des_observacao = 'Descontos concedidos')
        //                    ) a
        //                    group by a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    order by a.crb_int_notafiscal desc";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        //cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}


        //public DataTable GetRelPublicoItau(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.crb_int_notafiscal as 'Número', a.crb_des_observacao as 'Natureza de Operação', 
        //                         a.crb_dt_emissao as 'Data de Emissão', sum(a.crb_num_valorrecebido) as Valor from (
        //                        select a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    from SealContaReceber a 
        //                    inner join SealCliente b on a.crb_des_codigo = b.cli_des_codigo
        //                    where (b.cli_des_grupo = 'CONTRIBUINTE' or b.cli_des_grupo = 'NÃO CONTRIBUINTE') and
        //                    (a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim) and 
        //                    (a.crb_des_observacao = 'Banco Itau S/A   -  02539-2' or a.crb_des_observacao = 'Banco Itau Vinculada   -  06448-2')
        //                    ) a
        //                    group by a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    order by a.crb_int_notafiscal desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        //cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetRelPublicoBB(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.crb_int_notafiscal as 'Número', a.crb_des_observacao as 'Natureza de Operação', 
        //                         a.crb_dt_emissao as 'Data de Emissão', sum(a.crb_num_valorrecebido) as Valor from (
        //                        select a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    from SealContaReceber a 
        //                    inner join SealCliente b on a.crb_des_codigo = b.cli_des_codigo
        //                    where (b.cli_des_grupo = 'CONTRIBUINTE' or b.cli_des_grupo = 'NÃO CONTRIBUINTE') and
        //                    (a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim) and 
        //                    (a.crb_des_observacao = 'Banco Do Brasil   -  18642-2' or a.crb_des_observacao = 'Banco do Brasil - 6034-8')
        //                    ) a
        //                    group by a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    order by a.crb_int_notafiscal desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        //cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetRelPublicoCaixa(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.crb_int_notafiscal as 'Número', a.crb_des_observacao as 'Natureza de Operação', 
        //                         a.crb_dt_emissao as 'Data de Emissão', sum(a.crb_num_valorrecebido) as Valor from (
        //                        select a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    from SealContaReceber a 
        //                    inner join SealCliente b on a.crb_des_codigo = b.cli_des_codigo
        //                    where (b.cli_des_grupo = 'CONTRIBUINTE' or b.cli_des_grupo = 'NÃO CONTRIBUINTE') and
        //                    (a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim) and 
        //                    (a.crb_des_observacao = 'Caixa Economica - 0274')
        //                    ) a
        //                    group by a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    order by a.crb_int_notafiscal desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        //cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetRelPublicoBradesco(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.crb_int_notafiscal as 'Número', a.crb_des_observacao as 'Natureza de Operação', 
        //                         a.crb_dt_emissao as 'Data de Emissão', sum(a.crb_num_valorrecebido) as Valor from (
        //                        select a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    from SealContaReceber a 
        //                    inner join SealCliente b on a.crb_des_codigo = b.cli_des_codigo
        //                    where (b.cli_des_grupo = 'CONTRIBUINTE' or b.cli_des_grupo = 'NÃO CONTRIBUINTE') and
        //                    (a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim) and 
        //                    (a.crb_des_observacao = 'Banco Bradesco   -  107565')
        //                    ) a
        //                    group by a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    order by a.crb_int_notafiscal desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        //cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetRelPublicoDaycoval(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.crb_int_notafiscal as 'Número', a.crb_des_observacao as 'Natureza de Operação', 
        //                         a.crb_dt_emissao as 'Data de Emissão', sum(a.crb_num_valorrecebido) as Valor from (
        //                        select a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    from SealContaReceber a 
        //                    inner join SealCliente b on a.crb_des_codigo = b.cli_des_codigo
        //                    where (b.cli_des_grupo = 'CONTRIBUINTE' or b.cli_des_grupo = 'NÃO CONTRIBUINTE') and
        //                    (a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim) and 
        //                    (a.crb_des_observacao = 'Banco Daycoval S.A - 716415-9' or a.crb_des_observacao = 'Banco Daycoval S.A.   -  708077-0')
        //                    ) a
        //                    group by a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    order by a.crb_int_notafiscal desc ";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        //cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}

        //public DataTable GetRelPublicoDesconto(DateTime inicio, DateTime fim, string uf, out string erro)
        //{
        //    string query = "";
        //    DataTable retorno = new DataTable();
        //    erro = "OK";
        //    try
        //    {
        //        conn.Open();
        //        query = @"select a.crb_int_notafiscal as 'Número', a.crb_des_observacao as 'Natureza de Operação', 
        //                         a.crb_dt_emissao as 'Data de Emissão', sum(a.crb_num_valorrecebido) as Valor from (
        //                        select a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    from SealContaReceber a 
        //                    inner join SealCliente b on a.crb_des_codigo = b.cli_des_codigo
        //                    where (b.cli_des_grupo = 'CONTRIBUINTE' or b.cli_des_grupo = 'NÃO CONTRIBUINTE') and
        //                    (a.crb_dt_recebimento >= @inicio and a.crb_dt_recebimento <= @fim) and 
        //                    (a.crb_des_observacao = 'Descontos concedidos')
        //                    ) a
        //                    group by a.crb_int_notafiscal, a.crb_des_observacao, a.crb_dt_emissao, a.crb_num_valorrecebido 
        //                    order by a.crb_int_notafiscal desc";

        //        cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@inicio", inicio);
        //        cmd.Parameters.AddWithValue("@fim", fim);
        //        //cmd.Parameters.AddWithValue("@uf", uf);

        //        DbDataReader dr = cmd.ExecuteReader();
        //        retorno.Load(dr);
        //    }
        //    catch (Exception ex)
        //    {
        //        erro = "ERRO: " + ex.Message;
        //    }
        //    finally
        //    {
        //        //dr.Close();
        //        conn.Close();
        //    }
        //    return retorno;
        //}
        #endregion
    }
}
