using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class LoginDAO : Connection
    {
        public LoginDAO() 
            : base()
        {
            conn = new SqlConnection(connectionstring);
        }

        public string Logar(string user, string pass)
        {
            string retorno;
            string query;

            try
            {
                query = @"select b.LOGE_EMAIL as 'user', b.LOGE_PWD as 'pass' 
                          from LoginsExternosEmpresas a
                          left join LoginsExternos b on a.LOGEE_LOGIN_EXTERNO = b.LOGE_IND
                          where a.LOGEE_EMPRESA = 1062 and b.LOGE_EMAIL = @user and b.LOGE_PWD = @pass";

                conn.Open();
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user", user);
                cmd.Parameters.AddWithValue("@pass", pass);

                dr = cmd.ExecuteReader();

                string email = null;
                string senha = null;
                while (dr.Read())
                {
                    email = (dr["user"] is DBNull ? "" : dr["user"]).ToString().Trim();
                    senha = (dr["pass"] is DBNull ? "" : dr["pass"]).ToString().Trim();
                }

                dr.Close();

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
                {
                    query = @"select b.LOGI_EMAIL as 'user', b.LOGI_PWD as 'pass'  
                              from Clientes a 
                              left join LoginsInternos b on a.CLI_FISCAL = b.LOGI_IND or 
							                                a.CLI_CONTABIL = b.LOGI_IND or 
							                                a.CLI_PESSOAL = b.LOGI_IND
                              where a.CLI_IND = 1062 and b.LOGI_EMAIL = @user and b.LOGI_PWD = @pass";

                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user", user);
                    cmd.Parameters.AddWithValue("@pass", pass);

                    dr = cmd.ExecuteReader();
                    
                    while (dr.Read())
                    {
                        email = (dr["user"] is DBNull ? "" : dr["user"]).ToString().Trim();
                        senha = (dr["pass"] is DBNull ? "" : dr["pass"]).ToString().Trim();
                    }

                    dr.Close();

                    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
                        retorno = "";
                    else
                        retorno = email + "%||%" + senha + "%||%INTERNO";
                }
                else
                    retorno = email + "%||%" + senha + "%||%EXTERNO";
            }
            catch (Exception ex)
            {
                retorno = "ERRO%||%" + ex.Message;
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
