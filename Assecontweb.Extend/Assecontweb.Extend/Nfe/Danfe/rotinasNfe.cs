using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;

namespace Assecontweb.Extend.Nfe.Danfe
{
    public class rotinasNfe
    {
        private string passo ="";
        //public static spdNFeX spdNFe;
        //private DAORelacional.DAORelacional dao;
        //private DataBaseHelper dHelper;
        public rotinasNfe()
        {
            //try
            //{
            //    passo = "dHerlper.spdNFeX";
            //    spdNFe = new spdNFeX();
            //    passo = "dHerlper.DataBaseHelper";
            //    dHelper = new DataBaseHelper();
            //    passo = "dHerlper.ConectaBanco";
            //    dao = new DAORelacional.DAORelacional(dHelper.ConectaBanco());

            //}
            //catch (Exception ex)
            //{
                
            //    throw  new Exception( "Falha na rotina rotinaNFe passo " + passo + "Mensagem :" + ex.Message);
            //}

        }

        #region Comments

        //public int consultaIDViaCNPJ(string cnpj, string nome)
        //{
        //    ConsultasDAO daonatOper = new DAORelacional.ConsultasDAO(dao);
        //    return daonatOper.ConsultaIdFornecedorViaCNPJ(cnpj, nome);

        //}

        //private int consultaIDGrupoViaCNPJ(string cnpj, string nome)
        //{
        //    ConsultasDAO daoIdGrupo = new DAORelacional.ConsultasDAO(dao);
        //    return daoIdGrupo.ConsultaIDGrupoViaCNPJ(cnpj, nome); 
        //}

        //public void cadastraNaturecaoOperacaoSenaoExistir(string natureza)
        //{
        //   string  strsql = "select count(*) as existe from NaturezasOp where nat_descricao = '" + natureza.ToUpper() + "'";
        //   int exist = 0; 
        //   try
        //   {
        //        DataTable dt = dao.consultarComRetornoDT(strsql,null);
        //        exist = Convert.ToInt32(dt.Rows[0]["existe"]);
        //        if (exist == 0)
        //        {
        //            strsql = "insert into NaturezasOp ( NAT_GRUPO, NAT_DESCRICAO) values (1,'" + natureza.ToUpper() + "')";

        //            try
        //            {
        //                dao.executaComando(strsql, null);
        //            }
        //            catch (Exception ex)
        //            {

        //                throw new Exception("Rotina RotinasNFe / cadastraNaturecaoOperacaoSenaoExistir : " + (char)13 + " Mensagem : " + ex.Message )  ;
        //            }

        //        }
        //   }
        //   catch (Exception ex)
        //   {

        //       throw new Exception("Rotina RotinasNFe / ConsultaNaturezasOp : " + (char)13 + " Mensagem : " + ex.Message);
        //   }
        //}
        #region rotinas manifesto_Nfe_e_download_XML
        //public string enviaManifestacao(int tipoEvento, string Documento, string idNfe, string Justificativa, string dataEvento, int sequencia, string fuso, string cOrgao)
        //{

        //    string xmlRetorno = "";
        //    try
        //    {
        //        xmlRetorno = spdNFe.EnviarManifestacaoDestinatario(tipoEvento, idNfe, Documento, Justificativa, dataEvento, sequencia, "", cOrgao);
        //        return xmlRetorno;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //}
        //public string consultaNFe(string UltimoNSU)
        //{
        //    /*
        //      aIndNFeIndicador de NF-e consultada. Utilizar: (0 para "Todas as NF-e"; 
        //    *                                                 1 para "Somente as NF-e sem manifestação";
        //    *                                                 aIndEmiIndicador do Emissor da NF-e. Utilizar: (0 para "Todos os Emitentes / Remetentes";
        //    *                                                 aUltNSUÚltimo NSU recebido pela Empresa (Caso seja informado com zero, ou com um NSU muito antigo, aaConfigFilePathcaminho para um arquivo ini com configurações
        //    */
        //    string xmlRetorno = "";
        //    xmlRetorno = spdNFe.ConsultarNFDestinadas(1, 1, UltimoNSU);
        //    return xmlRetorno;
        //}
        //public string retornaXmlManifesto(string chave)
        //{
        //    string xmlRetornado;
        //    chave = util.RetonaChaveNFeLimpa(chave);
        //    try
        //    {
        //        xmlRetornado = spdNFe.DownloadNFe(chave, "91");
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //    return xmlRetornado;
        //}

        #endregion

        //public spdNFeX setaPropriedadesSpdNFe(string cnpjParaUsar)
        //{
        //    #region comentado
        //    /*SqlDataReader Q ;
        //    string strsql = "SELECT dbo.fn_trim_cnpj2(GRU_CNPJ) AS GRU_CNPJ, GRU_UF, CFG_NFE_AMBIENTE, GRU_CERTIFICADO, CFG_NFE_DIR, CFG_NFE_DIR_LOG FROM Grupos INNER JOIN Config ON CFG_GRUPO = GRU_IND  inner join notas n on (GRU_IND = n.NOT_GRUPO) WHERE N.NOT_IND = " + Convert.ToString(Not_Ind);
        //    try 
        // {	        
        //         SqlParameter parm  = dao.CriaParametro("NOT_IND",System.Data.DbType.Int32,Convert.ToString(Not_Ind));
        //          Q = dao.consultar(strsql,dao.addListaParametros(parm));   
        // }
        // catch (Exception ex)
        // {

        //  throw ex;
        // }
        //    */
        //    #endregion

        //    var dados = dadosGrupo.listaDeGrupos.Where(p => p.GRU_CNPJ == cnpjParaUsar).FirstOrDefault();

        //    if (dados.CFG_NFE_AMBIENTE == 1)
        //        spdNFe.Ambiente = Ambiente.akProducao;
        //    else if (dados.CFG_NFE_AMBIENTE == 2)
        //        spdNFe.Ambiente = Ambiente.akHomologacao;

        //    spdNFe.CNPJ = dados.GRU_CNPJ.Trim();
        //    spdNFe.UF = dados.GRU_UF.Trim();

        //    try
        //    {
        //        spdNFe.NomeCertificado = dados.GRU_CERTIFICADO;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //    spdNFe.ArquivoServidoresHom = "nfeServidoresHom.ini";
        //    spdNFe.ArquivoServidoresProd = "nfeServidoresProd.ini";
        //    spdNFe.DiretorioXmlDestinatario = dados.CFG_NFE_DIR == "" ? @"\Downloads" : dados.CFG_NFE_DIR;
        //    spdNFe.DiretorioLog = dados.CFG_NFE_DIR_LOG == "" ? @"\Downloads\log" : dados.CFG_NFE_DIR_LOG;
        //    spdNFe.DiretorioLogErro = spdNFe.DiretorioLog + "Erros";
        //    spdNFe.DiretorioTemporario = System.IO.Path.GetTempPath();
        //    //spdNFe.DiretorioDownloads := GetDirRazaoSocialFormatado(Grupo);
        //    spdNFe.DiretorioDownloads = @"\Downloads";

        //    CriaDiretoriosSenaoExistir(spdNFe.DiretorioXmlDestinatario);
        //    CriaDiretoriosSenaoExistir(spdNFe.DiretorioLog);

        //    //spdNFe.Usuario := 'teste';
        //    //spdNFe.Senha := 'teste';
        //    //spdNFe.Proxy := 'teste';
        //    spdNFe.TimeOut = 60000;
        //    spdNFe.MaxSizeLoteEnvio = 500;
        //    spdNFe.DiretorioEsquemas = @"Esquemas4\";
        //    spdNFe.DiretorioTemplates = @"Templates4\";
        //    spdNFe.IgnoreInvalidCertificates = true;
        //    spdNFe.AnexarDanfePDF = true;
        //    spdNFe.ValidarEsquemaAntesEnvio = true;
        //    spdNFe.VersaoManual = "5.0a";

        //    spdNFe.CaracteresRemoverAcentos = "áéíóúàèìòùâêîôûäëïöüãõñçÁÉÍÓÚÀÈÌÒÙÂÊÎÔÛÄËÏÖÜÃÕÑÇºª";
        //    //' DanfeSettings
        //    spdNFe.FraseContingencia = "Danfe em contingência - Impresso em decorrência de problemas técnicos";
        //    spdNFe.FraseHomologacao = "SEM VALOR FISCAL";
        //    spdNFe.QtdeCopias = 1;
        //    spdNFe.LineDelimiter = "|";
        //    spdNFe.ModeloRetrato = @"Templates4\Vm50a\Danfe\retrato.rtm";
        //    spdNFe.ModeloPaisagem = @"Templates4\Vm50a\Danfe\paisagem.rtm";

        //    return spdNFe;
        //}

        //public string mostraDownload()
        //{
        //    return spdNFe.DiretorioDownloads;

        //}

        //public List<dadosApresentacaoNotaImportar> defineCabecalhoNF(List<dadosNFeImportada> notas)
        //{
        //    List<dadosApresentacaoNotaImportar> cabecalhos = new List<dadosApresentacaoNotaImportar>();
        //    dadosApresentacaoNotaImportar cabecalho = new dadosApresentacaoNotaImportar();
        //    foreach (dadosNFeImportada nf in notas)
        //    {
        //        cabecalho.Nota = nf.Ide.nNf;
        //        cabecalho.dhEmissao = nf.Ide.dhEmi;
        //        cabecalho.dtAltorizacao = nf.Prot.dhRecbto;
        //        cabecalho.Emitente = nf.Emit.xNome;
        //        cabecalho.nProtocolo = nf.Prot.nProt.ToString();
        //        cabecalho.dtSaida = nf.Ide.dhSaiEnt;
        //        //cabecalho.tpMovito = "E";
        //        cabecalho.Valor = nf.Total.vNF;

        //        cabecalhos.Add(cabecalho);
        //        cabecalho = new dadosApresentacaoNotaImportar();
        //    }

        //    return cabecalhos;
        //}

        //public ListNatOper consultaNaturesaOperacao()
        //{
        //    ConsultasDAO daonatOper = new DAORelacional.ConsultasDAO(dao);

        //    ListNatOper listaN = new ListNatOper();

        //    listaN = daonatOper.consultaNaturesaOperacao();
        //    return listaN;
        //}

        //public List<prodModel> carregaListadeProdutosDaNota(List<detModel> list)
        //{
        //    List<prodModel> lstProd = new List<prodModel>();
        //    prodModel prod;
        //    foreach (detModel det in list)
        //    {
        //        prod = new prodModel();
        //        prod.xProd = det.prod.xProd;

        //        try { prod.cEAN = det.prod.cEAN; }
        //        catch { }
        //        if (util.isNumeric(prod.cEAN))
        //            prod.cProd = det.prod.cEAN;
        //        else
        //            prod.cProd = det.prod.cProd
        //        ;
        //        prod.NCM = det.prod.NCM;

        //        prod.CFOP = det.prod.CFOP;
        //        prod.uCom = det.prod.uCom; ;
        //        prod.qCom = det.prod.qCom;
        //        prod.vUnCom = det.prod.vUnCom;
        //        prod.vProd = det.prod.vProd;
        //        prod.cEANTrib = det.prod.cEANTrib;
        //        prod.uTrib = det.prod.uTrib;
        //        prod.qTrib = det.prod.qTrib;
        //        prod.vUnTrib = det.prod.vUnCom;
        //        prod.vFrete = det.prod.vFrete;
        //        prod.vOutros = (det.prod.vFrete); // somar todos o valores acessórios
        //        prod.vTotal = (det.prod.vProd + det.prod.vFrete);


        //        prod.indTot = det.prod.indTot;

        //        lstProd.Add(prod);
        //    }

        //    return lstProd;
        //}
        //public ListaDeXPara carregaListaDexPara(dadosNFeImportada nota)
        //{

        //    dadosDeParaModel item = new dadosDeParaModel();
        //    ListaDeXPara lista = new ListaDeXPara();

        //    ConsultasDAO cDao = new DAORelacional.ConsultasDAO(dao);
        //    string sListaProdutos = "'";
        //    string idFornecedor = "";

        //    foreach (detModel prod in nota.Itens)
        //    {
        //        item.codigoNota = prod.prod.cProd;
        //        item.descricaoProduto = prod.prod.xProd;
        //        try { item.FornecedorInd = nota.idFornecedor; }
        //        catch { }
        //        lista.ListaDePara.Add(item);

        //        idFornecedor = item.FornecedorInd.ToString();
        //        sListaProdutos += item.codigoNota.ToString() + "','";


        //        item = new dadosDeParaModel();
        //    }
        //     if (sListaProdutos.Length > 0)
        //      sListaProdutos = sListaProdutos.Substring(0, sListaProdutos.Length - 2);

        //     SqlDataReader dr =  cDao.consultaProdutosVinculados(sListaProdutos, idFornecedor);
        //     while (dr.Read())
        //     {
        //       dadosDeParaModel reg = (dadosDeParaModel)  lista.ListaDePara.Find(p=>p.codigoNota.Equals(dr["PRODFOR_INDPRODFOR"].ToString()));
        //       reg.produtoInd = Convert.ToInt32(dr["PRODFOR_INDPRO"]);
        //       reg.Nome_Catalogo = dr["PRODFOR_dsCatalogo"] == null ? "" : dr["PRODFOR_dsCatalogo"].ToString() ;
        //       reg.ClaInd =  Convert.ToInt32(dr["CLA_IND"]);
        //       reg.dsClassicacao = dr["CLA_DESCRICAO"] == null ? "" : dr["CLA_DESCRICAO"].ToString();
        //     }

        //    return lista;

        //}



        //public List<Duplicata> desmembraParcelas(List<Duplicata> list, int quatasParcelas)
        //{
        //    double total = 0;

        //    double novaParcela = 0;
        //    string primeiraFatura = list[0].Fatura;
        //    string baseFatura = primeiraFatura.Substring(0, primeiraFatura.Length - 2);
        //    string fatura = "";
        //    DateTime dataVencimento = list[0].Vencimento;
        //    Duplicata xdup;
        //    foreach (Duplicata dup in list)
        //    {
        //        total += dup.Valor;
        //    }

        //    list.RemoveAll(item => item.Valor > 0 || item.Fatura == null);

        //    novaParcela = (total / quatasParcelas);
        //    for (int i = 0; i < quatasParcelas; i++)
        //    {
        //        fatura = baseFatura + "/" + Convert.ToString(i + 1);
        //        xdup = new Duplicata();
        //        xdup.Fatura = fatura;
        //        xdup.Vencimento = dataVencimento;
        //        xdup.Valor = util.formataDecimal(novaParcela.ToString());
        //        list.Add(xdup);
        //    }
        //    return list;


        //}

        //public List<Duplicata> reajustaDatasVencimento(List<Duplicata> list, string Periodo, DateTime dataInicial)
        //{

        //    for (int passo = 1; passo < list.Count; passo++)
        //    {
        //        list[passo].Vencimento = util.SomaPeriodoNadata(Periodo, dataInicial, passo);

        //    }

        //    return list;
        //}

        //public List<SqlParameter> preparaParametrosPesquisa(string tipo, List<string> parm)
        //{
        //    filtro fil = new filtro();
        //    List<SqlParameter> parametros = new List<SqlParameter>();
        //    int iAux = 0;
        //    if (tipo == "movimentacaopesqE" || tipo == "catalogo")
        //            {
        //                /// pesquisa no catalogo de produtos

        //                foreach (string str in parm)
        //                {
        //                    if (util.isNumeric(str) && iAux ==0  )
        //                    {
        //                        fil.nomeParametro = "@GRUPO";
        //                        fil.tipo = SqlDbType.Int;
        //                        fil.valor = str;
        //                    }
        //                    else if (iAux > 0 && util.isNumeric(str))
        //                    {
        //                        fil.nomeParametro = "@Produto";
        //                        fil.tipo = SqlDbType.Int;
        //                        fil.valor = str;

        //                    }
        //                    else if (!util.isNumeric(str))
        //                    {
        //                        fil.nomeParametro = "@DESCRICAO";
        //                        fil.tipo = SqlDbType.NVarChar;
        //                        fil.valor =  str ;
        //                    }

        //                    parametros =  dao.retornaListadeParametros(fil, ref parametros);
        //                    iAux += 1;   
        //                }

        //            }
        //    else if (tipo == "movimentacaopesqS")
        //    {
        //         iAux = 0;
        //        foreach (string str in parm)
        //        {
        //            if (util.isNumeric(str))
        //            {

        //                if (iAux ==0)
        //                {
        //                    // E codigo de empresa
        //                    fil.nomeParametro = "@GRUPO";
        //                    fil.tipo = SqlDbType.Int;
        //                    fil.valor = str;
        //                }
        //                else
        //                {
        //                    // e codigo de produto
        //                    fil.nomeParametro = "@Produto";
        //                    fil.tipo = SqlDbType.Int;
        //                    fil.valor = str;

        //                }
        //            }
        //            else
        //            {
        //                fil.nomeParametro = "@DESCRICAO";
        //                fil.tipo = SqlDbType.NVarChar;
        //                fil.valor = str;
        //            }



        //            parametros = dao.retornaListadeParametros(fil, ref parametros);
        //            iAux += 1;
        //        }
        //    }


        //    return parametros;
        //}


        //public DataTable consultaProdutosCatalogo(List<SqlParameter> parametros)
        //{
        //    string strsql = "select PRO_IND,UPPER(PRO_DESCRICAO) AS 'PRO_DESCRICAO',PRO_PRODUTO_GRUPO,";
        //    strsql += " ISNULL(PROG_DESCRICAO,'') AS 'PROG_DESCRICAO',c.CLA_IND, c.CLA_DESCRICAO  from produtos AS a LEFT OUTER JOIN ";
        //                 strsql += " ProdutosGrupos AS b ON a.PRO_PRODUTO_GRUPO = b.PROG_IND ";
        //                 strsql += "left join Classificacoes c on (a.PRO_CLASSIFICACAO_CTPG = c.CLA_IND) ";
        //                 strsql += " where pro_grupo=@Grupo ";
        //    string strcompl = "";
        //    int iaux = 0;
        //    foreach (SqlParameter parm in parametros)
        //    {

        //        if (parm.SqlDbType == SqlDbType.NVarChar)
        //        {
        //            parm.Size = 300;
        //            strcompl = " and  PRO_DESCRICAO LIKE '%" + parm.Value +  "%'";

        //        }
        //        else if (parm.SqlDbType == SqlDbType.Int && iaux > 0)
        //            strcompl += " and  ltrim(rtrim(pro_codigo)) like '" + parm.Value + "%'";
        //        iaux += 1;
        //    }

        //    strsql += strcompl;

        //    try
        //    {

        //        //parametros = null;
        //        DataTable dt = dao.consultarComRetornoDT(strsql + strcompl, parametros);
        //        DataRow drow = dt.NewRow();
        //        drow["PRO_IND"] = "0";
        //        drow["PRO_DESCRICAO"] = "Selecione ...";
        //        dt.Rows.InsertAt(drow, 0);
        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //}


        //public bool validaDePara(ListaDeXPara lista, bool p)
        //{
        //    bool todosOsProdutosTemCorrelacao = false;
        //    foreach (dadosDeParaModel dep in lista.ListaDePara){
        //        todosOsProdutosTemCorrelacao = (dep.produtoInd > 0);
        //        if (!todosOsProdutosTemCorrelacao)
        //            break;
        //    }

        //   return todosOsProdutosTemCorrelacao;
        //}

        //public bool validaClassificacaoProduto(ListaDeXPara lista, bool p)
        //{
        //    bool todosOsProdutosTemClassificacao = false;
        //    foreach (dadosDeParaModel dep in lista.ListaDePara)
        //    {
        //        todosOsProdutosTemClassificacao = (dep.ClaInd > 0);
        //        if (!todosOsProdutosTemClassificacao)
        //            break;
        //    }

        //    return todosOsProdutosTemClassificacao;
        //}

        //internal void adicionaCFOPNaLista(string key, string valor)
        //{
        //    CfopModel obj = new CfopModel();
        //    obj.cfop = key.Replace("_", "");
        //    obj.descricao = valor;
        //    ListaCFOP.cfops.Add(obj);
        //}

        //internal void adicionaListadeRecurso(string key, string valor, ref List<cstGenericoModel> lista)
        //{
        //        cstGenericoModel obj = new cstGenericoModel();
        //        obj.codigo = key.Replace("_", "");
        //        obj.Descricao = valor;
        //        lista.Add(obj);
        //}
        #endregion
    }
}
