using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class ExceptionDAO : Connection
    {
        public ExceptionDAO() 
            : base()
        {
            conn = new SqlConnection(connectionstring);
        }

        public Model.ConfigException GetEmailException(out string erro)
        {
            Model.ConfigException retorno = new Model.ConfigException();
            string query;
            erro = "OK";
            try
            {
                query = @"select a.cfg_int_id, a.cfg_des_host, a.cfg_int_port, a.cfg_bit_ssl, a.cfg_des_from, a.cfg_des_user, a.cfg_des_pass, a.cfg_dt_data 
                            from SealEmailConfig a where a.cfg_bit_ativo = 1";

                conn.Open();
                cmd = new SqlCommand(query, conn);

                dr = cmd.ExecuteReader();
                
                while (dr.Read())
                {
                    retorno.id = Convert.ToInt32(dr["cfg_int_id"]);
                    retorno.host = Convert.ToString(dr["cfg_des_host"]);
                    retorno.port = Convert.ToInt32(dr["cfg_int_port"]);
                    retorno.ssl = Convert.ToBoolean(dr["cfg_bit_ssl"]);
                    retorno.email_from = Convert.ToString(dr["cfg_des_from"]);
                    retorno.user = Convert.ToString(dr["cfg_des_user"]);
                    retorno.pass = Convert.ToString(dr["cfg_des_pass"]);
                    retorno.data = Convert.ToDateTime(dr["cfg_dt_data"]);
                }

                dr.Close();

                query = @"select a.sed_int_id, a.sed_des_nome, a.sed_des_email  
                            from SealEmailConfigSend a
                            inner
                            join SealEmailConfig_CongigSend b on a.sed_int_id = b.sed_int_id
                            where b.cfg_int_id = @id";

                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", retorno.id);

                dr = cmd.ExecuteReader();

                retorno.emails = new Dictionary<string, string>();
                while (dr.Read())
                {
                    retorno.emails.Add(dr["sed_des_email"].ToString(), dr["sed_des_nome"].ToString());
                }
            }
            catch (Exception ex)
            {
                erro = "ERRO%: " + ex.Message;
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
