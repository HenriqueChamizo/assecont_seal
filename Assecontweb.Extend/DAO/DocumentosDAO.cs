using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class DocumentosDAO : Connection
    {
        public DocumentosDAO() 
            : base()
        {
            conn = new SqlConnection(connectionstring);
        }

        public void GetDocumentos(ref List<Model.Seal.Documento> documentos, ref List<Model.Seal.DocumentoDados> dados)
        {
            string query;
            try
            {
                query = @"select doc_int_id, doc_des_descricao, doc_dt_data, doc_bit_erro from SealDocumento order by doc_dt_data desc";

                conn.Open();
                cmd = new SqlCommand(query, conn);

                dr = cmd.ExecuteReader();

                Model.Seal.Documento doc;
                while (dr.Read())
                {
                    doc = new Model.Seal.Documento();
                    doc.doc_int_id = Convert.ToInt32(dr["doc_int_id"]);
                    doc.doc_des_descricao = Convert.ToString(dr["doc_des_descricao"]);
                    doc.doc_dt_data = Convert.ToDateTime(dr["doc_dt_data"]);
                    doc.doc_bit_erro = Convert.ToBoolean(dr["doc_bit_erro"]);

                    documentos.Add(doc);
                }

                dr.Close();

                query = @"select dcd_int_id, dcd_int_aprovacao, dcd_num_privadocontribuinte, dcd_num_privadoncontribuinte, dcd_num_publicocontribuinte, dcd_num_publiconcontribuinte, doc_int_id 
                          from SealDocumentoDados";

                cmd = new SqlCommand(query, conn);

                dr = cmd.ExecuteReader();

                Model.Seal.DocumentoDados dado;
                while (dr.Read())
                {
                    dado = new Model.Seal.DocumentoDados();
                    dado.dcd_int_id = Convert.ToInt32(dr["dcd_int_id"]);
                    dado.dcd_int_aprovacao = Convert.ToInt32(dr["dcd_int_aprovacao"]);
                    dado.dcd_num_privadocontribuinte = Convert.ToDouble(dr["dcd_num_privadocontribuinte"]);
                    dado.dcd_num_privadoncontribuinte = Convert.ToDouble(dr["dcd_num_privadoncontribuinte"]);
                    dado.dcd_num_publicocontribuinte = Convert.ToDouble(dr["dcd_num_publicocontribuinte"]);
                    dado.dcd_num_publiconcontribuinte = Convert.ToDouble(dr["dcd_num_publiconcontribuinte"]);
                    dado.doc_int_id = Convert.ToInt32(dr["doc_int_id"]);

                    dados.Add(dado);
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                throw new Exception(ex.Message);
            }
            finally
            {
                dr.Close();
                conn.Close();
            }
        }

        public void GetDocumento(ref Model.Seal.Documento documento, ref Model.Seal.DocumentoDados dados)
        {
            string query;
            try
            {
                query = @"select doc_int_id, doc_des_descricao, doc_des_nameclientes, doc_file_clientes, doc_des_namecontasreceber, doc_file_contasreceber, doc_dt_data, doc_bit_erro 
                          from SealDocumento where doc_int_id = @id";

                conn.Open();
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", documento.doc_int_id);

                dr = cmd.ExecuteReader();
                
                while (dr.Read())
                {
                    documento.doc_int_id = Convert.ToInt32(dr["doc_int_id"]);
                    documento.doc_des_descricao = Convert.ToString(dr["doc_des_descricao"]);
                    documento.doc_des_nameclientes = Convert.ToString(dr["doc_des_nameclientes"]);
                    documento.doc_file_clientes = (byte[])(dr["doc_file_clientes"]);
                    documento.doc_des_namecontasreceber = Convert.ToString(dr["doc_des_namecontasreceber"]);
                    documento.doc_file_contasreceber = (byte[])(dr["doc_file_contasreceber"]);
                    documento.doc_dt_data = Convert.ToDateTime(dr["doc_dt_data"]);
                    documento.doc_bit_erro = Convert.ToBoolean(dr["doc_bit_erro"]);
                }

                dr.Close();

                query = @"select dcd_int_id, dcd_int_aprovacao, dcd_num_privadocontribuinte, dcd_num_privadoncontribuinte, dcd_num_publicocontribuinte, dcd_num_publiconcontribuinte, doc_int_id 
                          from SealDocumentoDados where doc_int_id = @id";
                
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", documento.doc_int_id);

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    dados.dcd_int_id = Convert.ToInt32(dr["dcd_int_id"]);
                    dados.dcd_int_aprovacao = Convert.ToInt32(dr["dcd_int_aprovacao"]);
                    dados.dcd_num_privadocontribuinte = Convert.ToDouble(dr["dcd_num_privadocontribuinte"]);
                    dados.dcd_num_privadoncontribuinte = Convert.ToDouble(dr["dcd_num_privadoncontribuinte"]);
                    dados.dcd_num_publicocontribuinte = Convert.ToDouble(dr["dcd_num_publicocontribuinte"]);
                    dados.dcd_num_publiconcontribuinte = Convert.ToDouble(dr["dcd_num_publiconcontribuinte"]);
                    dados.doc_int_id = Convert.ToInt32(dr["doc_int_id"]);
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                throw new Exception(ex.Message);
            }
            finally
            {
                dr.Close();
                conn.Close();
            }
        }

        public int SetDocumento(Model.Seal.Documento documento, out string erro)
        {
            string query = "";
            int retorno = 0;
            erro = "OK";

            try
            {
                conn.Open();
                if (!string.IsNullOrEmpty(documento.doc_des_descricao))
                {
                    query = @"insert into SealDocumento (doc_des_descricao, doc_dt_data, doc_des_nameclientes, doc_file_clientes, doc_des_namecontasreceber, doc_file_contasreceber) 
                                  values (@doc_des_descricao, @doc_dt_data, @doc_des_nameclientes, @doc_file_clientes, @doc_des_namecontasreceber, @doc_file_contasreceber)
                              select IDENT_CURRENT('SealDocumento')";
                }
                else
                {
                    query = @"insert into SealDocumento (doc_dt_data, doc_des_nameclientes, doc_file_clientes, doc_des_namecontasreceber, doc_file_contasreceber) 
                                  values (@doc_dt_data, @doc_des_nameclientes, @doc_file_clientes, @doc_des_namecontasreceber, @doc_file_contasreceber)
                              select IDENT_CURRENT('SealDocumento')";
                }

                if (string.IsNullOrEmpty(documento.doc_des_nameclientes))
                    query = query.Replace(", doc_des_nameclientes", "").Replace(", @doc_des_nameclientes", "");
                if (documento.doc_file_clientes == null)
                    query = query.Replace(", doc_file_clientes", "").Replace(", @doc_file_clientes", "");

                if (string.IsNullOrEmpty(documento.doc_des_namecontasreceber))
                    query = query.Replace(", doc_des_namecontasreceber", "").Replace(", @doc_des_namecontasreceber", "");
                if (documento.doc_file_contasreceber == null)
                    query = query.Replace(", doc_file_contasreceber", "").Replace(", @doc_file_contasreceber", "");

                cmd = new SqlCommand(query, conn);
                if (!string.IsNullOrEmpty(documento.doc_des_descricao))
                    cmd.Parameters.AddWithValue("@doc_des_descricao", documento.doc_des_descricao);
                cmd.Parameters.AddWithValue("@doc_dt_data", documento.doc_dt_data);

                if (!string.IsNullOrEmpty(documento.doc_des_nameclientes))
                    cmd.Parameters.AddWithValue("@doc_des_nameclientes", documento.doc_des_nameclientes);
                if (documento.doc_file_clientes != null)
                    cmd.Parameters.AddWithValue("@doc_file_clientes", documento.doc_file_clientes);
                if (!string.IsNullOrEmpty(documento.doc_des_namecontasreceber))
                    cmd.Parameters.AddWithValue("@doc_des_namecontasreceber", documento.doc_des_namecontasreceber);
                if (documento.doc_file_contasreceber != null)
                    cmd.Parameters.AddWithValue("@doc_file_contasreceber", documento.doc_file_contasreceber);

                retorno = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                conn.Close();
                throw new Exception(ex.Message);
            }
            finally
            {
                //dr.Close();
                conn.Close();
            }
            return retorno;
        }

        public int SetDocumentoDados(Model.Seal.DocumentoDados dados, out string erro)
        {
            string query = "";
            int retorno = 0;
            erro = "OK";

            try
            {
                conn.Open();
                query = @"insert into SealDocumentoDados (dcd_num_privadocontribuinte, dcd_num_privadoncontribuinte, dcd_num_publicocontribuinte, dcd_num_publiconcontribuinte, doc_int_id)
                          values (@dcd_num_privadocontribuinte, @dcd_num_privadoncontribuinte, @dcd_num_publicocontribuinte, @dcd_num_publiconcontribuinte, @doc_int_id)";
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dcd_num_privadocontribuinte", dados.dcd_num_privadocontribuinte);
                cmd.Parameters.AddWithValue("@dcd_num_privadoncontribuinte", dados.dcd_num_privadoncontribuinte);
                cmd.Parameters.AddWithValue("@dcd_num_publicocontribuinte", dados.dcd_num_publicocontribuinte);
                cmd.Parameters.AddWithValue("@dcd_num_publiconcontribuinte", dados.dcd_num_publiconcontribuinte);
                cmd.Parameters.AddWithValue("@doc_int_id", dados.doc_int_id);

                retorno = Convert.ToInt32(cmd.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                conn.Close();
                throw new Exception(ex.Message);
            }
            finally
            {
                //dr.Close();
                conn.Close();
            }
            return retorno;
        }

        public int SetDocumentoDadosAprova(int id, int aprova)
        {
            string query = "";
            int retorno = 0;

            try
            {
                conn.Open();
                query = @"update SealDocumentoDados 
                          set dcd_int_aprovacao = @aprova
                          where dcd_int_id = @id";
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@aprova", aprova);
                cmd.Parameters.AddWithValue("@id", id);

                retorno = Convert.ToInt32(cmd.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                conn.Close();
                throw new Exception(ex.Message);
            }
            finally
            {
                //dr.Close();
                conn.Close();
            }
            return retorno;
        }

        public bool SetErrorDocumento(Model.Seal.Documento documento, out string erro)
        {
            string query = "";
            int retorno = 0;
            erro = "OK";

            try
            {
                conn.Open();
                query = @"update SealDocumento 
                            set doc_bit_erro = 1 
                            where doc_int_id = @id";
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", documento.doc_int_id);

                retorno = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                conn.Close();
                throw new Exception(ex.Message);
            }
            finally
            {
                //dr.Close();
                conn.Close();
            }
            return retorno > 0;
        }
         
        public bool GetExistDocPeriodo()
        {
            bool retorno;
            try
            {
                string query = @"declare @exists bit = (case when exists(select a.doc_int_id from SealDocumento a 
							                                         inner join SealDocumentoDados b on a.doc_int_id = b.doc_int_id 
							                                         where a.doc_bit_erro = 0 and DATEDIFF(DAY, a.doc_dt_data, GETDATE()) < 30 and 
								                                           b.dcd_int_aprovacao != 0 and b.dcd_num_privadocontribuinte != 0 and b.dcd_num_privadoncontribuinte != 0 and 
								                                           b.dcd_num_publicocontribuinte != 0 and b.dcd_num_publiconcontribuinte != 0) then 1 else 0 end) 
                             select @exists as Existe";

                conn.Open();
                cmd = new SqlCommand(query, conn);
                retorno = Convert.ToBoolean(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                conn.Close();
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return retorno;
        }

        public bool GetFilesCount(DateTime inicio, DateTime fim, ref int clientes, ref int recebidas, ref int danfes, ref int servicos, ref string erro)
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
                            select count(*) as Clientes, a.ContasReceber, b.Danfes, c.Servicos
                            from SealDocumento 
                            outer apply(
	                            select count(*) as ContasReceber
	                            from SealDocumento 
	                            where doc_dt_data >= @inicio and doc_dt_data <= @fim and doc_file_contasreceber is not null
                            ) a
                            outer apply(
	                            select count(*) as Danfes
	                            from SealNotaFiscalDanfe 
	                            where nfd_dt_dhEmi >= @inicio and nfd_dt_dhEmi <= @fim
                            ) b
                            outer apply(
	                            select count(*) as Servicos
	                            from SealNotaFiscalServico 
	                            where nfs_dt_emissao >= @inicio and nfs_dt_emissao <= @fim
                            ) c
                            where doc_dt_data >= @inicio and doc_dt_data <= @fim and doc_file_clientes is not null
                            group by a.ContasReceber, b.Danfes, c.Servicos
		                        ";
                #endregion
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dt_inicio", inicio);
                cmd.Parameters.AddWithValue("@dt_fim", fim);

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    #region Getting Values 
                    clientes = Convert.ToInt32(dr["Clientes"] is DBNull ? 0 : dr["Clientes"]);
                    recebidas = Convert.ToInt32(dr["ContasReceber"] is DBNull ? 0 : dr["ContasReceber"]);
                    danfes = Convert.ToInt32(dr["Danfes"] is DBNull ? 0 : dr["Danfes"]);
                    servicos = Convert.ToInt32(dr["Servicos"] is DBNull ? 0 : dr["Servicos"]);
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
    }
}
