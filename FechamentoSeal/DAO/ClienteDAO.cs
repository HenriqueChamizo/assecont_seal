using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class ClienteDAO : Connection
    {
        public ClienteDAO() 
            : base()
        {
            conn = new SqlConnection(connectionstring);
        }

        public bool SetClientes(List<Model.Seal.Cliente> clientes, out string erro)
        {
            string query = "";
            bool retorno = false;
            int i;
            erro = "OK";
            try
            {
                conn.Open();
                foreach (Model.Seal.Cliente cliente in clientes)
                {
                    query = @"declare @codigo varchar(6) = @cli_des_codigo, 
		                              @nome varchar(150) = @cli_des_nome, 
		                              @tipo varchar(50) = @cli_des_tipo, 
		                              @grupo varchar(50) = @cli_des_grupo 
                              if exists(select cli_int_id from SealCliente where cli_des_codigo = @codigo)
                              begin 
	                              update SealCliente 
	                              set cli_des_nome = @nome, 
		                              cli_des_tipo = @tipo, 
		                              cli_des_grupo = @grupo 
	                              where cli_des_codigo = @codigo
                              end else 
                              begin
	                              insert into SealCliente (cli_des_codigo, cli_des_nome, cli_des_tipo, cli_des_grupo) 
	                              values (@codigo, @nome, @tipo, @grupo)
                              end ";

                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@cli_des_codigo", cliente.codigo);
                    cmd.Parameters.AddWithValue("@cli_des_nome", cliente.nome);
                    cmd.Parameters.AddWithValue("@cli_des_tipo", cliente.tipo);
                    cmd.Parameters.AddWithValue("@cli_des_grupo", cliente.codigoGrupo);

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
