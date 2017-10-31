using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class ClienteDAO : Connection
    {
        private DbDataReader dr;
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
		                              @grupo varchar(50) = @cli_des_grupo, 
		                              @cnpj varchar(50) = @cli_des_cnpj
                              if exists(select cli_int_id from SealCliente where cli_des_cnpj = @cnpj)
                              begin 
	                              update SealCliente 
	                              set cli_des_nome = @nome, 
		                              cli_des_tipo = @tipo, 
		                              cli_des_grupo = @grupo, 
		                              cli_des_codigo = @codigo 
	                              where cli_des_cnpj = @cnpj
                              end else 
                              begin
	                              insert into SealCliente (cli_des_codigo, cli_des_nome, cli_des_tipo, cli_des_grupo, cli_des_cnpj) 
	                              values (@codigo, @nome, @tipo, @grupo, @cnpj)
                              end ";

                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@cli_des_codigo", cliente.codigo);
                    cmd.Parameters.AddWithValue("@cli_des_nome", cliente.nome);
                    cmd.Parameters.AddWithValue("@cli_des_tipo", cliente.tipo);
                    cmd.Parameters.AddWithValue("@cli_des_grupo", cliente.codigoGrupo);
                    cmd.Parameters.AddWithValue("@cli_des_cnpj", cliente.cnpj.Replace(".", "").Replace("-", "").Replace("/", ""));

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

        public bool GetClientes(ref List<Model.Seal.Cliente> clientes, out string erro)
        {
            string query = "";
            bool retorno = true;
            int i;
            erro = "OK";
            try
            {
                conn.Open();
                query = @"select cli_des_codigo, cli_des_nome, cli_des_tipo, cli_des_grupo from SealCliente";

                cmd = new SqlCommand(query, conn);

                dr = cmd.ExecuteReader();

                Model.Seal.Cliente cliente;
                while (dr.Read())
                {
                    cliente = new Model.Seal.Cliente();
                    cliente.codigo = dr["cli_des_codigo"].ToString();
                    cliente.nome = dr["cli_des_nome"].ToString();
                    cliente.tipo = dr["cli_des_tipo"].ToString();
                    cliente.codigoGrupo = dr["cli_des_grupo"].ToString();
                    clientes.Add(cliente);
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
