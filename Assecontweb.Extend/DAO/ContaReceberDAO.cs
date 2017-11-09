using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class ContaReceberDAO : Connection
    {
        public ContaReceberDAO() 
            : base()
        {
            conn = new SqlConnection(connectionstring);
        }

        public bool SetContaReceber(List<Model.Seal.ContaReceber> contasreceber, int documento, out string erro)
        {
            string query = "";
            bool retorno = false;
            int i;
            erro = "OK";
            int cont = 0;
            try
            {
                conn.Open();
                foreach (Model.Seal.ContaReceber conta in contasreceber)
                {
                    if (!string.IsNullOrEmpty(conta.notaFiscal))
                    {
                        cont++;
                        if (conta.emissao != DateTime.MinValue)
                        {
                            if (conta.vencimento == DateTime.MinValue)
                                conta.vencimento = conta.emissao;
                            if (conta.dataRecto == DateTime.MinValue)
                                conta.dataRecto = conta.emissao;
                        }
                        if (conta.vencimento != DateTime.MinValue)
                        {
                            if (conta.emissao == DateTime.MinValue)
                                conta.emissao = conta.vencimento;
                            if (conta.dataRecto == DateTime.MinValue)
                                conta.dataRecto = conta.vencimento;
                        }
                        if (conta.dataRecto != DateTime.MinValue)
                        {
                            if (conta.emissao == DateTime.MinValue)
                                conta.emissao = conta.dataRecto;
                            if (conta.dataRecto == DateTime.MinValue)
                                conta.vencimento = conta.dataRecto;
                        }

                        //query = @"declare @d_codigo varchar(6) = @codigo, 
                        //              @d_cnpjorigem varchar(14) = @cnpjorigem, 
                        //              @d_docorigem int = @docorigem, 
                        //              @d_notafiscal int = @notafiscal, 
                        //              @d_prest varchar(10) = @prest, 
                        //              @d_emissao datetime = @emissao, 
                        //              @d_vencimento datetime = @vencimento, 
                        //              @d_valor numeric(18, 6) = @valor, 
                        //              @d_desconto numeric(18, 6) = @desconto, 
                        //              @d_juros numeric(18, 6) = @juros, 
                        //              @d_valorrecebido numeric(18, 6) = @valorrecebido, 
                        //              @d_recebimento datetime = @recebimento, 
                        //              @d_forma varchar(20) = @forma, 
                        //              @d_observacao varchar(250) = @observacao, 
                        //              @d_doc int = @doc
                        //            if (not exists(select * from SealContaReceber 
                        //             where crb_des_codigo = @d_codigo and crb_des_cnpjorigem = @d_cnpjorigem and crb_int_docorigem = @d_docorigem and 
                        //             crb_int_notafiscal = @d_notafiscal and crb_des_prest = @d_prest and crb_dt_emissao = @d_emissao and 
                        //             crb_dt_vencimento = @d_vencimento and crb_num_valor = @d_valor and crb_num_desconto = @d_desconto and 
                        //             crb_num_juros = @d_juros and crb_num_valorrecebido = @d_valorrecebido and crb_dt_recebimento = @d_recebimento and 
                        //             crb_des_forma = @d_forma and crb_des_observacao = @d_observacao))
                        //            begin
                        //             INSERT INTO SealContaReceber 
                        //             (crb_des_codigo,crb_des_cnpjorigem,crb_int_docorigem,crb_int_notafiscal,crb_des_prest,crb_dt_emissao,crb_dt_vencimento
                        //             ,crb_num_valor,crb_num_desconto,crb_num_juros,crb_num_valorrecebido,crb_dt_recebimento,crb_des_forma,crb_des_observacao,doc_int_id)
                        //             VALUES
                        //             (@d_codigo, @d_cnpjorigem, @d_docorigem, @d_notafiscal, @d_prest, @d_emissao, @d_vencimento, @d_valor, @d_desconto, 
                        //              @d_juros, @d_valorrecebido, @d_recebimento, @d_forma, @d_observacao, @d_doc)
                        //              select IDENT_CURRENT('SealContaReceber')
                        //            end else begin 
                        //              select -100
                        //            end";
                        query = @"INSERT INTO SealContaReceber 
                                (crb_des_codigo, crb_des_cnpjorigem, crb_int_docorigem, crb_int_notafiscal, crb_des_prest, crb_dt_emissao, crb_dt_vencimento,
                                crb_num_valor, crb_num_desconto, crb_num_juros, crb_num_valorrecebido, crb_dt_recebimento, crb_des_forma, crb_des_observacao, doc_int_id)
                                VALUES
                                (@codigo, @cnpjorigem, @docorigem, @notafiscal, @prest, @emissao, @vencimento, @valor, @desconto,
                                 @juros, @valorrecebido, @recebimento, @forma, @observacao, @doc)
                                select IDENT_CURRENT('SealContaReceber')";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@codigo", conta.codigo);
                        cmd.Parameters.AddWithValue("@cnpjorigem", conta.documentoOrigem.Replace(".", "").Replace("/", "").Replace("-", ""));
                        cmd.Parameters.AddWithValue("@docorigem", conta.docOrig);
                        string[] split = conta.notaFiscal.Split(new string[] { " " }, StringSplitOptions.None);
                        string nota = split[1];
                        cmd.Parameters.AddWithValue("@notafiscal", Convert.ToInt32(nota));
                        cmd.Parameters.AddWithValue("@prest", conta.prest);
                        cmd.Parameters.AddWithValue("@emissao", conta.emissao);
                        cmd.Parameters.AddWithValue("@vencimento", conta.vencimento);
                        cmd.Parameters.AddWithValue("@valor", conta.valor);
                        cmd.Parameters.AddWithValue("@desconto", conta.desconto);
                        cmd.Parameters.AddWithValue("@juros", conta.juros);
                        cmd.Parameters.AddWithValue("@valorrecebido", conta.valorRecebido);
                        cmd.Parameters.AddWithValue("@recebimento", conta.dataRecto);
                        cmd.Parameters.AddWithValue("@forma", conta.forma);
                        cmd.Parameters.AddWithValue("@observacao", conta.descricao);
                        cmd.Parameters.AddWithValue("@doc", documento);

                        i = Convert.ToInt32(cmd.ExecuteScalar());
                        retorno = (i > 0 || i == -100);
                        if (!retorno)
                        {
                            erro = "Erro de inserção do Conta Receber";
                            break;
                        }
                    }
                    else
                        continue;
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

        public bool GetContaReceber(ref List<Model.Seal.ContaReceber> contasreceber, out string erro)
        {
            string query = "";
            bool retorno = true;
            int i;
            erro = "OK";
            try
            {
                conn.Open();
                query = @"select cli_des_codigo, crb_des_cnpjorigem, crb_int_docorigem, crb_des_notafiscal, crb_des_prest, crb_dt_emissao, 
								crb_dt_vencimento, crb_num_valor, crb_num_desconto, crb_num_juros, crb_num_valorrecebido, crb_dt_recebimento, 
								crb_des_forma, crb_des_observacao, doc_int_id from SealContaReceber ";
                cmd = new SqlCommand(query, conn);
                //cmd.Parameters.AddWithValue("@cli_des_codigo", conta.codigo);
                //cmd.Parameters.AddWithValue("@crb_des_cnpjorigem", conta.documentoOrigem);
                //cmd.Parameters.AddWithValue("@crb_int_docorigem", conta.docOrig);
                //cmd.Parameters.AddWithValue("@crb_des_notafiscal", conta.notaFiscal);
                //cmd.Parameters.AddWithValue("@crb_des_prest", conta.prest);
                //cmd.Parameters.AddWithValue("@crb_dt_emissao", conta.emissao);
                //cmd.Parameters.AddWithValue("@crb_dt_vencimento", conta.vencimento);
                //cmd.Parameters.AddWithValue("@crb_num_valor", conta.valor);
                //cmd.Parameters.AddWithValue("@crb_num_desconto", conta.desconto);
                //cmd.Parameters.AddWithValue("@crb_num_juros", conta.juros);
                //cmd.Parameters.AddWithValue("@crb_num_valorrecebido", conta.valorRecebido);
                //cmd.Parameters.AddWithValue("@crb_dt_recebimento", conta.dataRecto);
                //cmd.Parameters.AddWithValue("@crb_des_forma", conta.forma);
                //cmd.Parameters.AddWithValue("@crb_des_observacao", conta.desconto);
                //cmd.Parameters.AddWithValue("@doc_int_id", documento);

                dr = cmd.ExecuteReader();

                Model.Seal.ContaReceber contareceber;
                while (dr.Read())
                {
                    contareceber = new Model.Seal.ContaReceber();
                    contareceber.codigo = dr["cli_des_codigo"].ToString();
                    contareceber.docOrig = dr["crb_des_cnpjorigem"].ToString();
                    contareceber.documentoOrigem = dr["crb_int_docorigem"].ToString();
                    contareceber.notaFiscal = dr["crb_des_notafiscal"].ToString();
                    contareceber.prest = dr["crb_des_prest"].ToString();
                    contareceber.emissao = Convert.ToDateTime(dr["crb_dt_emissao"]);
                    contareceber.vencimento = Convert.ToDateTime(dr["crb_dt_vencimento"]);
                    contareceber.valor = Convert.ToDouble(dr["crb_num_valor"]);
                    contareceber.desconto = Convert.ToDouble(dr["crb_num_desconto"]);
                    contareceber.juros = Convert.ToDouble(dr["crb_num_juros"]);
                    contareceber.valorRecebido = Convert.ToDouble(dr["crb_num_valorrecebido"]);
                    contareceber.dataRecto = Convert.ToDateTime(dr["crb_dt_recebimento"]);
                    contareceber.forma = dr["crb_des_forma"].ToString();
                    contareceber.descricao = dr["crb_des_observacao"].ToString();
                    contasreceber.Add(contareceber);
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

        public bool GetContaReceberByDateRec(ref List<Model.Seal.ContaReceber> contasreceber, out string erro)
        {
            string query = "";
            bool retorno = true;
            int i;
            erro = "OK";
            try
            {
                conn.Open();
                query = @"select cli_des_codigo, crb_des_cnpjorigem, crb_int_docorigem, crb_des_notafiscal, crb_des_prest, crb_dt_emissao, 
								crb_dt_vencimento, crb_num_valor, crb_num_desconto, crb_num_juros, crb_num_valorrecebido, crb_dt_recebimento, 
								crb_des_forma, crb_des_observacao, doc_int_id from SealContaReceber 
                                where crb_dt_recebimento >= @inicio and crb_dt_recebimento <= @fim";
                cmd = new SqlCommand(query, conn);

                DateTime inicio = Convert.ToDateTime("01/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString());
                DateTime fim = Convert.ToDateTime("01/" + (DateTime.Now.Month + 1).ToString() + "/" + DateTime.Now.Year.ToString());
                fim = fim.AddDays(-1);

                cmd.Parameters.AddWithValue("@inicio", inicio);
                cmd.Parameters.AddWithValue("@fim", fim);

                dr = cmd.ExecuteReader();

                Model.Seal.ContaReceber contareceber;
                while (dr.Read())
                {
                    contareceber = new Model.Seal.ContaReceber();
                    contareceber.codigo = dr["cli_des_codigo"].ToString();
                    contareceber.docOrig = dr["crb_des_cnpjorigem"].ToString();
                    contareceber.documentoOrigem = dr["crb_int_docorigem"].ToString();
                    contareceber.notaFiscal = dr["crb_des_notafiscal"].ToString();
                    contareceber.prest = dr["crb_des_prest"].ToString();
                    contareceber.emissao = Convert.ToDateTime(dr["crb_dt_emissao"]);
                    contareceber.vencimento = Convert.ToDateTime(dr["crb_dt_vencimento"]);
                    contareceber.valor = Convert.ToDouble(dr["crb_num_valor"]);
                    contareceber.desconto = Convert.ToDouble(dr["crb_num_desconto"]);
                    contareceber.juros = Convert.ToDouble(dr["crb_num_juros"]);
                    contareceber.valorRecebido = Convert.ToDouble(dr["crb_num_valorrecebido"]);
                    contareceber.dataRecto = Convert.ToDateTime(dr["crb_dt_recebimento"]);
                    contareceber.forma = dr["crb_des_forma"].ToString();
                    contareceber.descricao = dr["crb_des_observacao"].ToString();
                    contasreceber.Add(contareceber);
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

        public bool GetContasReceberNaoProcessadas(ref List<Model.Seal.ContaReceber> contasreceber, out string erro)
        {
            string query = "";
            bool retorno = true;
            int i;
            erro = "OK";
            try
            {
                conn.Open();
                query = @"select crb_int_id, crb_des_codigo, crb_des_cnpjorigem, crb_int_docorigem, crb_int_notafiscal, crb_des_prest, crb_dt_emissao,
                                crb_dt_vencimento, crb_num_valor, crb_num_desconto, crb_num_juros, crb_num_valorrecebido, crb_dt_recebimento, crb_des_forma,
                                crb_des_observacao, doc_int_id, cli_int_id from SealContaReceber where nfr_int_id is null";
                cmd = new SqlCommand(query, conn);

                dr = cmd.ExecuteReader();

                Model.Seal.ContaReceber contareceber;
                while (dr.Read())
                {
                    contareceber = new Model.Seal.ContaReceber();
                    contareceber.codigo = dr["crb_des_codigo"].ToString();
                    contareceber.docOrig = dr["crb_des_cnpjorigem"].ToString();
                    contareceber.documentoOrigem = dr["crb_int_docorigem"].ToString();
                    contareceber.notaFiscal = dr["crb_int_notafiscal"].ToString();
                    contareceber.prest = dr["crb_des_prest"].ToString();
                    contareceber.emissao = Convert.ToDateTime(dr["crb_dt_emissao"]);
                    contareceber.vencimento = Convert.ToDateTime(dr["crb_dt_vencimento"]);
                    contareceber.valor = Convert.ToDouble(dr["crb_num_valor"]);
                    contareceber.desconto = Convert.ToDouble(dr["crb_num_desconto"]);
                    contareceber.juros = Convert.ToDouble(dr["crb_num_juros"]);
                    contareceber.valorRecebido = Convert.ToDouble(dr["crb_num_valorrecebido"]);
                    contareceber.dataRecto = Convert.ToDateTime(dr["crb_dt_recebimento"]);
                    contareceber.forma = dr["crb_des_forma"].ToString();
                    contareceber.descricao = dr["crb_des_observacao"].ToString();
                    contareceber.doc_int_id = Convert.ToInt32(dr["doc_int_id"] is DBNull ? 0 : dr["doc_int_id"]);
                    contareceber.cli_int_id = Convert.ToInt32(dr["cli_int_id"] is DBNull ? 0 : dr["cli_int_id"]);
                    contareceber.nfr_int_id = null;
                    contareceber.crb_int_id = Convert.ToInt32(dr["crb_int_id"] is DBNull ? 0 : dr["crb_int_id"]);
                    contasreceber.Add(contareceber);
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

        public bool SetRecebidas(List<Model.Seal.ContaReceber> contasreceber, out string erro)
        {
            string query = "";
            bool retorno = false;
            int i;
            erro = "OK";
            try
            {
                conn.Open();
                foreach (Model.Seal.ContaReceber conta in contasreceber)
                {
                    query = @"declare @nfd_int_id int = @nfd, 
		                                @nfs_int_id int = @nfs, 
		                                @crb_int_id int = @crb, 
		                                @nfr_int_id int  

                                if @nfd_int_id != 0 begin
	                                insert into SealNotaFiscalRecebida
	                                (nfd_int_id) values (@nfd_int_id)
	
	                                set @nfr_int_id = (select IDENT_CURRENT('SealNotaFiscalRecebida'))

                                end else begin
	                                insert into SealNotaFiscalRecebida
	                                (nfs_int_id) values (@nfs_int_id)

	                                set @nfr_int_id = (select IDENT_CURRENT('SealNotaFiscalRecebida'))
                                end
                                update SealContaReceber
                                set nfr_int_id = @nfr_int_id
                                where crb_int_id = @crb_int_id";

                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nfd", conta.nfr_int_id.nfd_int_id);
                    cmd.Parameters.AddWithValue("@nfs", conta.nfr_int_id.nfs_int_id);
                    cmd.Parameters.AddWithValue("@crb", conta.crb_int_id);

                    i = cmd.ExecuteNonQuery();
                    retorno = (i > 0);
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
