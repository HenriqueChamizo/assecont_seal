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
            try
            {
                conn.Open();
                foreach (Model.Seal.ContaReceber conta in contasreceber)
                {
                    query = @"declare @codigo varchar(6) = @cli_des_codigo, 
		                              @doc int = @doc_int_id, 
  		                              @cnpjorigem varchar(14) = @crb_des_cnpjorigem, 
		                              @docorigem int = @crb_int_docorigem, 
  		                              @notafiscal varchar(20) = @crb_des_notafiscal, 
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
                                        
                              if exists(select cli_int_id from SealCliente where cli_des_codigo = @codigo) 
                              begin 
	                              if not exists(select doc_int_id from SealDocumento where doc_int_id = @doc)
	                              begin
		                              set @doc = (select doc_int_id from SealDocumento where doc_des_descricao = 'DOCUMENTO NÃO EXISTENTE')
	                              end
                              end else
                              begin
	                              if not exists(select doc_int_id from SealDocumento where doc_int_id = @doc)
	                              begin
		                              set @codigo = (select cli_int_id from SealCliente where cli_des_nome = 'DOCUMENTO NÃO EXISTENTE')
		                              set @doc = (select doc_int_id from SealDocumento where doc_des_descricao = 'DOCUMENTO NÃO EXISTENTE')
	                              end else
	                              begin 
		                              set @codigo = (select cli_int_id from SealCliente where cli_des_nome = 'CLIENTE NÃO EXISTENTE')
	                              end
                              end
  
                              insert into SealContaReceber (cli_des_codigo, crb_des_cnpjorigem, crb_int_docorigem, crb_des_notafiscal, crb_des_prest, crb_dt_emissao, 
								                          crb_dt_vencimento, crb_num_valor, crb_num_desconto, crb_num_juros, crb_num_valorrecebido, crb_dt_recebimento, 
								                          crb_des_forma, crb_des_observacao, doc_int_id) values 
                              (@codigo, @cnpjorigem, @docorigem, @notafiscal, @prest, 
                              @emissao, @vencimento, @valor, @desconto, @juros, 
                              @valorrecebido, @recebimento, @forma, @observacao, @doc)";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@cli_des_codigo", conta.codigo);
                    cmd.Parameters.AddWithValue("@crb_des_cnpjorigem", conta.documentoOrigem);
                    cmd.Parameters.AddWithValue("@crb_int_docorigem", conta.docOrig);
                    cmd.Parameters.AddWithValue("@crb_des_notafiscal", conta.notaFiscal);
                    cmd.Parameters.AddWithValue("@crb_des_prest", conta.prest);
                    cmd.Parameters.AddWithValue("@crb_dt_emissao", conta.emissao);
                    cmd.Parameters.AddWithValue("@crb_dt_vencimento", conta.vencimento);
                    cmd.Parameters.AddWithValue("@crb_num_valor", conta.valor);
                    cmd.Parameters.AddWithValue("@crb_num_desconto", conta.desconto);
                    cmd.Parameters.AddWithValue("@crb_num_juros", conta.juros);
                    cmd.Parameters.AddWithValue("@crb_num_valorrecebido", conta.valorRecebido);
                    cmd.Parameters.AddWithValue("@crb_dt_recebimento", conta.dataRecto);
                    cmd.Parameters.AddWithValue("@crb_des_forma", conta.forma);
                    cmd.Parameters.AddWithValue("@crb_des_observacao", conta.desconto);
                    cmd.Parameters.AddWithValue("@doc_int_id", documento);

                    i = cmd.ExecuteNonQuery();
                    retorno = (i > 0);
                    if (!retorno)
                        break;
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
