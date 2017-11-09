using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using Excel;
using System.IO;
using Assecontweb.Extend.CFechamento;
using Assecontweb.Extend.Nfe;
using System.Linq;
using System.Xml;
using System.Text;

namespace Assecontweb.Extend
{
    public enum FileTypes
    {
        txt = 0,
        xls = 1,
        xlsx = 2,
        csv = 3,
        pdf = 4
    }

    public class File
    {
        public FileTypes type { get; set; }
        public string path { get; set; }
        public string filename { get; set; }
        public string sheet { get; set; }
        public DataSet dataset { get; set; }

        public File(string filePath, string fileSheet = null)
        {
            string[] split;
            string extention;
            //set file path
            path = filePath;
            split = path.Split(new string[] { "\\" }, StringSplitOptions.None);
            //set filename
            filename = split[split.Length - 1];
            split = filename.Split(new string[] { "." }, StringSplitOptions.None);
            extention = split[split.Length - 1];
            //set type
            switch (extention)
            {
                case "xls":
                    type = FileTypes.xls;
                    break;
                case "xlsx":
                    type = FileTypes.xlsx;
                    break;
                case "csv":
                    type = FileTypes.csv;
                    break;
                default:
                    type = FileTypes.xlsx;
                    break;
            }
            //set sheet where File is a Excel document
            sheet = fileSheet;
        }

        public DataTable GetDataTableOleDb()
        {
            string strConexao = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=0\"", path);
            OleDbConnection conn = new OleDbConnection(strConexao);
            conn.Open();
            DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            //Cria o objeto dataset para receber o conteúdo do arquivo Excel
            DataSet output = new DataSet();
            DataTable outputTable = null;

            // obtem o noma da planilha corrente
            // string sheet = row["Inventario_2016"].ToString();
            // obtem todos as linhas da planilha corrente
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + sheet + "$]", conn);
            cmd.CommandType = CommandType.Text;
            // copia os dados da planilha para o datatable
            outputTable = new DataTable(sheet);
            output.Tables.Add(outputTable);
            new OleDbDataAdapter(cmd).Fill(outputTable);

            return outputTable;
        }

        public DataTable GetDataTableExcel()
        {
            IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(new StreamReader(path).BaseStream);
            reader.IsFirstRowAsColumnNames = true;
            DataSet ds = reader.AsDataSet(false);
            DataTable dt;
            if (ds.Tables.Contains(sheet))
                dt = ds.Tables[sheet];
            else
                dt = ds.Tables[0];

            return dt;
        }

        public DataSet GetDataSetXml()
        {
            DataSet ds = new DataSet();
            //TextReader xmlSR = new StringReader(path);
            StreamReader xmlStream = new StreamReader(path);
            string textxml = xmlStream.ReadToEnd();
            xmlStream.Close();
            string textexmlNovo = "";
            string textControle = textxml;
            string endereco = "tc:Endereco>";
            string rua = "tc:Rua>";
            //string Signature = "tc:Endereco>";

            if (!textxml.Contains(rua))
            {
                while (textxml.Contains(endereco))
                {
                    //if (!string.IsNullOrEmpty(textexmlNovo))
                    //{
                    //    textControle = textxml;
                    //    textxml = textexmlNovo;
                    //}
                    int intprimeiro = textxml.IndexOf(endereco);
                    int intprimeirook = intprimeiro + endereco.Length;
                    string xmlprimeiro = textxml.Substring(0, intprimeirook);
                    textxml = textxml.Replace(xmlprimeiro, "");
                    textexmlNovo += xmlprimeiro;

                    int intsegundo = textxml.IndexOf(endereco);
                    int intsegundook = intsegundo + endereco.Length;
                    string xmlsegundo = textxml.Substring(intsegundo, intsegundook);
                    string xmlsegundook = xmlsegundo.Replace(endereco, rua);
                    textxml = textxml.Replace(xmlsegundo, xmlsegundook);

                    int intterceiro = textxml.IndexOf(endereco);
                    int intterceirook = intterceiro + endereco.Length;
                    string xmlterceiro = textxml.Substring(intterceiro, intterceirook);
                    string xmlterceirook = xmlterceiro.Replace(endereco, rua);
                    textxml = textxml.Replace(xmlterceiro, xmlterceirook);

                    int intultimo = textxml.IndexOf(endereco);
                    int intultimook = intultimo + endereco.Length;
                    string xmlultimo = textxml.Substring(0, intultimook);
                    textxml = textxml.Replace(xmlultimo, "");
                    textexmlNovo += xmlultimo;
                }
                string xml = textexmlNovo + textxml;
                StringReader xmlSR = new StringReader(xml.Replace("?<", "<"));
                ds.ReadXml(xmlSR, XmlReadMode.Auto);
            }
            else
                ds.ReadXml(path, XmlReadMode.Auto);
            //ds.ReadXml(path, XmlReadMode.Auto);
            return ds;
        }

        public DataSet GetDataSetTxt()
        {
            string erro = "";
            DataSetNotasTxt ds = new DataSetNotasTxt();
            string[] lines;
            try
            {
                string nNFS = "";
                lines = System.IO.File.ReadAllLines(path, UTF7Encoding.UTF7);
                for(int i = 0; i < lines.Count(); i++)
                {
                    string line = lines[i];
                    string controle = line.Substring(0, 1);
                    if (controle == "1")
                        nNFS = line.Substring(4, 8);
                    else if (controle == "2")
                    {
//2949984000001143520160419115544PNRCHEZRRPS       00000001143520160419        T000000000165917000000000000000029170300000000000004977000000000000000220256622400019000000000000000000000TRIBUNAL REGIONAL DO TRABALHO DA 6A REGIAO                                 AV CAIS DO APOLO                                     739       QUARTO ANDAR                  RECIFE                        Recife                                            PE 5003023                                                                           SERVICO DE SUPORTE E ASSISTENCIA TECNICA|EMPENHO: 2015NE001300|PROCESSO ADM: TRT6 No 151/2014||PERIODO: ABRIL/2016||VENCIMENTO: 19/05/2016||DADOS BANCARIOS: BANCO ITAU - AG: 3171 - C/C: 02539-2||VALOR APROXIMADO DOS TRIBUTOS R$ 228,96 (13,80%) - FONTE IBPT|SERVICO NAO SUJEITO A RETENCAO DO INSS NOS TERMOS DA INSTRUCAO NORMATIVA N.071 DE 10/05/2002
//2949984000001178420170424082703UDMTD8KARPS       00000001178420170420        C000000009864737000000000000000072850500000000000493236000000000000000223156510402753900000000000000000000PEPSICO DO BRASIL LTDA                                                     RuaAVENIDA INDEPENDENCIA                             417 - IPORANGA                      Sorocaba SP18087000cristiane.andrade @pepsico.com REFERENTE AO PEDIDO : M300214707 - | REQUISITANTE: ELAINE CRUZ DOS SANTOS-elaine.cruz@pepsico.com || ITEM: 0220 - 700000710 - SERVICOS DE MONTAGEM DE APARELHOS| INFRA ESTRUTURA - MAQ.E  EQUIPAMENTOS = R$ 96.647,37 || ROQUE AMBROZINI FILHO -(15) 3416 - 1658 - (11)98604 - 0172 | EMERGENCIAS:  GERALDO AYALA -(15) 99812 - 4030 | DADOS BANCARIOS: BANCO DAYCOVAL (707) | AGENCIA: 0001 - 9 - CONTA CORRENTE: 716415 - 9
                        
                        string CodigoMunicipio = "0";//ok
                        string Uf = line.Substring(431, 2);//ok
                        ds.OrgaoGerador.AddOrgaoGeradorRow(CodigoMunicipio, Uf);

                        string Numero = line.Substring(9, 8);//ok
                        if (Numero == "00011784")
                            CodigoMunicipio = "0";//ok
                        CodigoMunicipio = "0";//ok
                        string DataEmissao = line.Substring(17, 14);//ok
                        string NaturezaOperacao = "0";//line.Substring(517, line.Length - 518);//ok
                        string RegimeEspecialTributacao = "0";//line.Substring(418, 1);
                        string OptanteSimplesNacional = "0";//ok
                        string IncentivadorCultural = "0";//ok
                        string Competencia = "0";//ok
                        string NfseSubstituida = "0";//line.Substring(517, 8);//ok
                        string OutrasInformacoes = "";//ok
                        string ValorCredito = (Convert.ToDouble(line.Substring(133, 15)) / 100).ToString();//ok
//2949984000001176320170404122933SG5JQB4XRPS       00000001176320170404        T000000003147255000000000000000029170300000000000094417000000000000000224637722200373048384666000000000000DEPARTAMENTO DE TECNOLOGIA DA INFORMAÇÃO - DTI                             AV RANGEL PESTANA                                    00300     4º ANDAR                      SE                            São Paulo                                         SP 1017000                                                                           CONTRATO DE MANUTENCAO E SUPORTE|PROCESSO SF No 23643 - 729821/2014|CONTRATO 23673 - SAAC - 0023/2015|EMPENHO 2017NE00027|MEDICAO: MARCO/2017||VENCIMENTO: 04/05/2017||VALOR APROXIMADO DOS TRIBUTOS R$ 3.933,00 (13,80%) - FONTE IBPT|SERVICO NAO SUJEITO A RETENCAO DO INSS NOS TERMOS DA INSTRUCAO NORMATIVA N.071 DE 10/05/2002
                        ds.InfNfse.AddInfNfseRow(Numero, CodigoMunicipio, DataEmissao, NaturezaOperacao, RegimeEspecialTributacao, OptanteSimplesNacional, IncentivadorCultural, Competencia,
                            NfseSubstituida, OutrasInformacoes, ValorCredito);

                        string Cancelada = line.Substring(77, 1).ToString();//ok
                        ds.Cancelada.AddCanceladaRow(Cancelada);

                        string ValorServicos = (Convert.ToDouble(line.Substring(78, 15)) / 100).ToString();//ok
                        string ValorDeducoes = (Convert.ToDouble(line.Substring(93, 15)) / 100).ToString();//ok
                        string ValorPis = "0";//ok
                        string ValorCofins = "0";//ok
                        string ValorCsll = "0";//ok     
                        string IssRetido = (Convert.ToDouble(line.Substring(148, 1))).ToString();//ok
                        string OutrasRetencoes = "0";//ok
                        string BaseCalculo = "0";//ok 
                        string Aliquota = (Convert.ToDouble(line.Substring(113, 4)) / 100).ToString();//ok
                        string ValorLiquidoNfse = "0";//ok
                        string ValorIssRetido = "0";//ok
                        string ValorIss = (Convert.ToDouble(line.Substring(117, 15)) / 100).ToString();//ok
                        string DescontoCondicionado = "0";//ok
                        string DescontoIncondicionado = "0";//ok
                        ds.Valores.AddValoresRow(ValorServicos, ValorDeducoes, ValorPis, ValorCofins, ValorCsll, IssRetido, OutrasRetencoes, BaseCalculo, Aliquota, ValorLiquidoNfse,
                            ValorIssRetido, ValorIss, DescontoCondicionado, DescontoIncondicionado);

                        string ItemListaServico = (Convert.ToDouble(line.Substring(108, 5))).ToString();//ok
                        string CodigoCnae = "0";
                        string CodigoTributacaoMunicipio = "0";
                        string Discriminacao = "0";
                        string MunicipioPrestacaoServico = "0";
                        ds.Servico.AddServicoRow(ItemListaServico, CodigoCnae, CodigoTributacaoMunicipio, Discriminacao, MunicipioPrestacaoServico);

                        string RazaoSocial = line.Substring(183, 75);//ok
                        ds.TomadorServico.AddTomadorServicoRow(RazaoSocial);

                        string Cnpj = line.Substring(149, 14);//ok
                        ds.CpfCnpjTomador.AddCpfCnpjTomadorRow(Cnpj);

                        string Rua = line.Substring(261, 50);//ok
                        Numero = line.Substring(311, 10);//ok
                        string Bairro = line.Substring(351, 30);//ok
                        string Cidade = line.Substring(381, 50);//ok
                        string Estado = line.Substring(431, 2);//ok
                        string Cep = line.Substring(434, 7);//ok
                        ds.EnderecoTomador.AddEnderecoTomadorRow(Rua, Numero, Bairro, Cidade, Estado, Cep);

                        string Telefone = "";//ok
                        string Email = line.Substring(441, 75);//ok
                        ds.ContatoTomador.AddContatoTomadorRow(Telefone, Email);
                    }
                    else
                        erro = "OK";
                }
            }
            catch (Exception ex)
            {
                erro = ex.Message;
            }
            
            return ds;
        }

        public DataTable GetDataTableXml()
        {
            DataSet ds = new DataSet();
            StringReader xmlSR = new StringReader(path);

            ds.ReadXml(xmlSR, XmlReadMode.Auto);
            DataTable dt = ds.Tables[sheet];
            return dt;
        }
    }

    public class FileCliente : File
    {
        public List<Cliente> clientes { get; set; }

        public FileCliente(string filePath, string fileSheet = null)
            :base(filePath, fileSheet)
        {
            clientes = GetClientes();
        }

        public List<Cliente> GetClientes()
        {
            clientes = new List<CFechamento.Cliente>();
            CFechamento.Cliente cliente;
            //DataTable dt = base.GetDataTableOleDb();
            DataTable dt = base.GetDataTableExcel();

            foreach (DataRow row in dt.Rows)
            {
                cliente = new CFechamento.Cliente();
                cliente.codigo = (row[dt.Columns["Código do PN"].Ordinal] is DBNull ? "" : row[dt.Columns["Código do PN"].Ordinal]).ToString().Replace("\r", "").Replace("\n", "").Replace("\\", "").Trim();
                cliente.nome = (row[dt.Columns["Nome do PN"].Ordinal] is DBNull ? "" : row[dt.Columns["Nome do PN"].Ordinal]).ToString().Replace("\r", "").Replace("\n", "").Replace("\\", "").Trim();
                if (dt.Columns.Contains("Tipo de PN"))
                    cliente.tipo = (row[dt.Columns["Tipo de PN"].Ordinal] is DBNull ? "" : row[dt.Columns["Tipo de PN"].Ordinal]).ToString().Replace("\r", "").Replace("\n", "").Replace("\\", "").Trim();
                else
                    cliente.tipo = "";
                cliente.codigoGrupo = (row[dt.Columns["Nome do grupo"].Ordinal] is DBNull ? "" : row[dt.Columns["Nome do grupo"].Ordinal]).ToString().Replace("\r", "").Replace("\n", "").Replace("\\", "").Trim();
                cliente.cnpj = (row[dt.Columns["ID 0 impos."].Ordinal] is DBNull ? "" : row[dt.Columns["ID 0 impos."].Ordinal]).ToString().Replace("\r", "").Replace("\n", "").Replace("\\", "").Trim();

                if (cliente.codigo == "Código do PN" ||
                    (string.IsNullOrEmpty(cliente.codigo) &&
                     string.IsNullOrEmpty(cliente.nome) &&
                     string.IsNullOrEmpty(cliente.codigoGrupo)))
                    continue;
                else
                    clientes.Add(cliente);
            }
            return clientes;
        }
    }

    public class FileContaReceber : File
    {
        public List<ContaReceber> contas { get; set; }

        public FileContaReceber(string filePath, bool antigo = false, string fileSheet = null)
            :base(filePath, fileSheet)
        {
            contas = GetContas(antigo);
        }

        public List<ContaReceber> GetContas(bool antigo)
        {
            contas = new List<ContaReceber>();
            ContaReceber conta;
            //DataTable dt = base.GetDataTableOleDb();
            DataTable dt = base.GetDataTableExcel();

            string ccliente = "";
            string cliente = "";
            string documentoOrigem = "";
            string codigo = "";
            if (antigo)
            {
                try
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string old = ccliente;
                        ccliente = (row[dt.Columns["Cliente"].Ordinal] is DBNull ? old : row[dt.Columns["Cliente"].Ordinal]).ToString().Replace("\r", "").Replace("\n", "").Replace("\\", "").Trim();
                        if (old != ccliente)
                        {
                            if (ccliente.Trim() != "Sub-total:")
                            {
                                string[] split = ccliente.Split(new string[] { "CNPJ: " }, StringSplitOptions.None);
                                cliente = split[0].ToString().Replace("\r", "").Replace("\n", "").Replace("\\", "").Trim();

                                string[] split2 = split[1].Split(new string[] { "(" }, StringSplitOptions.None);
                                documentoOrigem = split2[0].ToString().Replace("\r", "").Replace("\n", "").Replace("\\", "").Trim();

                                codigo = split2[1].Replace("(", "").Replace(")", "").Replace(" ", "");
                            }
                            else
                            {
                                cliente = "";
                                documentoOrigem = "";
                                codigo = "";
                                continue;
                            }
                        }
                        else
                        {
                            conta = new ContaReceber();
                            conta.cliente = cliente;
                            conta.documentoOrigem = documentoOrigem;
                            conta.codigo = codigo;
                            conta.docOrig = (row[dt.Columns["Doc Origem"].Ordinal] is DBNull ? "" : row[dt.Columns["Doc Origem"].Ordinal]).ToString();
                            conta.notaFiscal = (row[dt.Columns["N.Fiscal"].Ordinal] is DBNull ? "" : row[dt.Columns["N.Fiscal"].Ordinal]).ToString();
                            conta.prest = (row[dt.Columns["Parc."].Ordinal] is DBNull ? "" : row[dt.Columns["Parc."].Ordinal]).ToString();
                            conta.emissao = Convert.ToDateTime((row[dt.Columns["Emissão"].Ordinal] is DBNull ? DateTime.MinValue : row[dt.Columns["Emissão"].Ordinal]));
                            conta.vencimento = Convert.ToDateTime((row[dt.Columns["Vencto."].Ordinal] is DBNull ? DateTime.MinValue : row[dt.Columns["Vencto."].Ordinal]));
                            conta.valor = Convert.ToDouble((row[dt.Columns["Valor Original"].Ordinal] is DBNull ? 0 : row[dt.Columns["Valor Original"].Ordinal]));
                            conta.desconto = Convert.ToDouble((row[dt.Columns["Desconto"].Ordinal] is DBNull ? 0 : row[dt.Columns["Desconto"].Ordinal]));
                            conta.juros = Convert.ToDouble((row[dt.Columns["Juros"].Ordinal] is DBNull ? 0 : row[dt.Columns["Juros"].Ordinal]));
                            conta.valorRecebido = Convert.ToDouble((row[dt.Columns["Valor"].Ordinal] is DBNull ? 0 : row[dt.Columns["Valor"].Ordinal]));
                            conta.dataRecto = Convert.ToDateTime((row[dt.Columns["Data Recto"].Ordinal] is DBNull ? DateTime.MinValue : row[dt.Columns["Data Recto"].Ordinal]));
                            conta.forma = (row[dt.Columns["Tipo"].Ordinal] is DBNull ? "" : row[dt.Columns["Tipo"].Ordinal]).ToString();
                            conta.descricao = (row["Banco"] is DBNull ? "" : row["Banco"]).ToString();
                            contas.Add(conta);
                        }
                        //conta.cliente = (row[dt.Columns["Cliente"].Ordinal] is DBNull ? "" : row[dt.Columns["Cliente"].Ordinal]).ToString().Replace("\r", "").Replace("\n", "").Replace("\\", "").Trim();
                        //conta.documentoOrigem = (row[dt.Columns["Documento Origem"].Ordinal] is DBNull ? "" : row[dt.Columns["Documento Origem"].Ordinal]).ToString();
                        //conta.codigo = (row[dt.Columns["Código"].Ordinal] is DBNull ? "" : row[dt.Columns["Código"].Ordinal]).ToString();
                    }
                }
                catch (Exception ex)
                {
                    string hehe = ex.Message;
                }
            }
            else
            {
                try
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        conta = new ContaReceber();
                        conta.cliente = (row[dt.Columns["Cliente"].Ordinal] is DBNull ? "" : row[dt.Columns["Cliente"].Ordinal]).ToString().Replace("\r", "").Replace("\n", "").Replace("\\", "").Trim();

                        if (dt.Columns.Contains("Documento Origem"))
                            conta.documentoOrigem = (row[dt.Columns["Documento Origem"].Ordinal] is DBNull ? "" : row[dt.Columns["Documento Origem"].Ordinal]).ToString();
                        else if (dt.Columns.Contains("DocumentoOrigem"))
                            conta.documentoOrigem = (row[dt.Columns["DocumentoOrigem"].Ordinal] is DBNull ? "" : row[dt.Columns["DocumentoOrigem"].Ordinal]).ToString();

                        if (dt.Columns.Contains("Código"))
                            conta.codigo = (row[dt.Columns["Código"].Ordinal] is DBNull ? "" : row[dt.Columns["Código"].Ordinal]).ToString();
                        else if (dt.Columns.Contains("Codigo"))
                            conta.codigo = (row[dt.Columns["Codigo"].Ordinal] is DBNull ? "" : row[dt.Columns["Codigo"].Ordinal]).ToString();

                        if (dt.Columns.Contains("Doc Origem"))
                            conta.docOrig = (row[dt.Columns["Doc Origem"].Ordinal] is DBNull ? "" : row[dt.Columns["Doc Origem"].Ordinal]).ToString();
                        else if (dt.Columns.Contains("Doc"))
                            conta.docOrig = (row[dt.Columns["Doc"].Ordinal] is DBNull ? "" : row[dt.Columns["Doc"].Ordinal]).ToString();

                        if (dt.Columns.Contains("N.Fiscal"))
                            conta.notaFiscal = (row[dt.Columns["N.Fiscal"].Ordinal] is DBNull ? "" : row[dt.Columns["N.Fiscal"].Ordinal]).ToString();
                        else if (dt.Columns.Contains("N. Fiscal"))
                            conta.notaFiscal = (row[dt.Columns["N. Fiscal"].Ordinal] is DBNull ? "" : row[dt.Columns["N. Fiscal"].Ordinal]).ToString();

                        if (dt.Columns.Contains("Parc."))
                            conta.prest = (row[dt.Columns["Parc."].Ordinal] is DBNull ? "" : row[dt.Columns["Parc."].Ordinal]).ToString();
                        else if (dt.Columns.Contains("Parc"))
                            conta.prest = (row[dt.Columns["Parc"].Ordinal] is DBNull ? "" : row[dt.Columns["Parc"].Ordinal]).ToString();

                        conta.emissao = Convert.ToDateTime((row[dt.Columns["Emissão"].Ordinal] is DBNull ? DateTime.MinValue : row[dt.Columns["Emissão"].Ordinal]));

                        if (dt.Columns.Contains("Vencto."))
                            conta.vencimento = Convert.ToDateTime((row[dt.Columns["Vencto."].Ordinal] is DBNull ? DateTime.MinValue : row[dt.Columns["Vencto."].Ordinal]));
                        else if (dt.Columns.Contains("Vencto"))
                            conta.vencimento = Convert.ToDateTime((row[dt.Columns["Vencto"].Ordinal] is DBNull ? DateTime.MinValue : row[dt.Columns["Vencto"].Ordinal]));

                        if (dt.Columns.Contains("Valor Original"))
                            conta.valor = Convert.ToDouble((row[dt.Columns["Valor Original"].Ordinal] is DBNull ? 0 : row[dt.Columns["Valor Original"].Ordinal]));
                        else if(dt.Columns.Contains("Valor"))
                            conta.valor = Convert.ToDouble((row[dt.Columns["Valor"].Ordinal] is DBNull ? 0 : row[dt.Columns["Valor"].Ordinal]));

                        if (dt.Columns.Contains("Desconto"))
                            conta.desconto = Convert.ToDouble((row[dt.Columns["Desconto"].Ordinal] is DBNull ? 0 : row[dt.Columns["Desconto"].Ordinal]));
                        else if (dt.Columns.Contains("Desc"))
                            conta.desconto = Convert.ToDouble((row[dt.Columns["Desc"].Ordinal] is DBNull ? 0 : row[dt.Columns["Desc"].Ordinal]));

                        conta.juros = Convert.ToDouble((row[dt.Columns["Juros"].Ordinal] is DBNull ? 0 : row[dt.Columns["Juros"].Ordinal]));

                        if (dt.Columns.Contains("Valor Pgo"))
                            conta.valorRecebido = Convert.ToDouble((row[dt.Columns["Valor Pgo"].Ordinal] is DBNull ? 0 : row[dt.Columns["Valor Pgo"].Ordinal]));
                        else if (dt.Columns.Contains("Vr. Recebido"))
                            conta.valorRecebido = Convert.ToDouble((row[dt.Columns["Vr. Recebido"].Ordinal] is DBNull ? 0 : row[dt.Columns["Vr. Recebido"].Ordinal]));
                        else if (dt.Columns.Contains("Valor"))
                            conta.valorRecebido = Convert.ToDouble((row[dt.Columns["Valor"].Ordinal] is DBNull ? 0 : row[dt.Columns["Valor"].Ordinal]));

                        if (dt.Columns.Contains("Data Recto"))
                            conta.dataRecto = Convert.ToDateTime((row[dt.Columns["Data Recto"].Ordinal] is DBNull ? DateTime.MinValue : row[dt.Columns["Data Recto"].Ordinal]));
                        else if (dt.Columns.Contains("Data"))
                            conta.dataRecto = Convert.ToDateTime((row[dt.Columns["Data"].Ordinal] is DBNull ? DateTime.MinValue : row[dt.Columns["Data"].Ordinal]));

                        if (dt.Columns.Contains("Forma"))
                            conta.forma = (row[dt.Columns["Forma"].Ordinal] is DBNull ? "" : row[dt.Columns["Forma"].Ordinal]).ToString();
                        else if (dt.Columns.Contains("Tipo"))
                            conta.forma = (row[dt.Columns["Tipo"].Ordinal] is DBNull ? "" : row[dt.Columns["Tipo"].Ordinal]).ToString();

                        if (dt.Columns.Contains("Banco"))
                            conta.descricao = (row["Banco"] is DBNull ? "" : row["Banco"]).ToString();
                        else
                            conta.descricao = (row[14] is DBNull ? "" : row[14]).ToString();
                        contas.Add(conta);
                    }
                }
                catch (Exception ex)
                {
                    string hehe = ex.Message;
                }
            }
            return contas;
        }
    }

    public class FileNfeDanfe : File
    {
        public Nfe.Danfe.NotaFiscal notafiscal;

        public List<Nfe.Danfe.NotaFiscal> notasfiscais;

        public FileNfeDanfe(string filePath, string fileSheet = null) : base(filePath, fileSheet)
        {
            notafiscal = GetNotaFiscal();
        }

        public Nfe.Danfe.NotaFiscal GetNotaFiscal()
        {
            DataSet ds = this.GetDataSetXml();
            this.notafiscal = new Nfe.Danfe.NotaFiscal();
            this.notafiscal.idFornecedor = -1;
            if (string.IsNullOrEmpty(ds.Prefix) &&
                ds.Tables.Contains("infNFe") &&
                ds.Tables.Contains("ide") &&
                ds.Tables.Contains("emit") &&
                ds.Tables.Contains("enderEmit") &&
                ds.Tables.Contains("dest") &&
                ds.Tables.Contains("enderDest") &&
                ds.Tables.Contains("prod") &&
                (ds.Tables.Contains("transportadora") || ds.Tables.Contains("transp")) &&
                ds.Tables.Contains("vol"))
            {
                #region NotaFiscal
                DataTable dt = ds.Tables["infNFe"];
                DataColumn dtid;
                if (dt.Columns.Contains("id"))
                    dtid = dt.Columns["id"];
                else if (dt.Columns.Contains("NFe_Id"))
                    dtid = dt.Columns["NFe_Id"];
                else
                    dtid = new DataColumn();
                notafiscal.chave = dtid.Table.Rows[0].ItemArray[1].ToString().Replace("NFe", "");

                Nfe.Danfe.IdeModel ide = new Nfe.Danfe.IdeModel();
                Nfe.Danfe.EmitModel emitente = new Nfe.Danfe.EmitModel();
                Nfe.Danfe.enderEmitModel enderEmit = new Nfe.Danfe.enderEmitModel();
                Nfe.Danfe.DestModel dest = new Nfe.Danfe.DestModel();

                Nfe.Danfe.enderDestModel enderDest = new Nfe.Danfe.enderDestModel();
                Nfe.Danfe.ICMSTotModel total = new Nfe.Danfe.ICMSTotModel();
                Nfe.Danfe.InfProt prot = new Nfe.Danfe.InfProt();
                IList<Nfe.Danfe.detModel> itens = new List<Nfe.Danfe.detModel>();
                List<Nfe.Danfe.Duplicata> cobr = new List<Nfe.Danfe.Duplicata>();
                Nfe.Danfe.TranspModel transport = new Nfe.Danfe.TranspModel();
                Nfe.Danfe.InfAdicModel inf = new Nfe.Danfe.InfAdicModel();

                ///
                // preenchendo o Ide da nota
                //
                ide = (Nfe.Danfe.IdeModel)preencheObjXML(ide, ds.Tables["ide"]);
                //
                // Prenchendo o Emitente da nota
                //
                emitente = (Nfe.Danfe.EmitModel)preencheObjXML(emitente, ds.Tables["emit"], ds.Tables["enderEmit"]);
                enderEmit = (Nfe.Danfe.enderEmitModel)preencheObjXML(enderEmit, ds.Tables["enderEmit"]);
                emitente.enderEmit = enderEmit;
                //
                // Prenchendo o destinatario da nota
                //
                dest = (Nfe.Danfe.DestModel)preencheObjXML(dest, ds.Tables["dest"], ds.Tables["enderDest"]);
                enderDest = (Nfe.Danfe.enderDestModel)preencheObjXML(enderDest, ds.Tables["enderDest"]);
                dest.enderDest = enderDest;
                //
                // preenchendo os produtos da nota
                //
                itens = (List<Nfe.Danfe.detModel>)preencheObjXML(itens, ds.Tables["prod"], null);
                // falta importar os impostos ... 

                //
                // Preenchendo cobrança
                //
                try { cobr = (List<Nfe.Danfe.Duplicata>)preencheObjXML(cobr, ds.Tables["dup"], null); }
                catch { };

                //
                // Preenchendo os dados da transportadora
                //

                try
                {
                    if (ds.Tables.Contains("transportadora"))
                        transport = (Nfe.Danfe.TranspModel)preencheObjXML(transport, ds.Tables["transportadora"]);
                    else
                        transport = (Nfe.Danfe.TranspModel)preencheObjXML(transport, ds.Tables["transp"]);
                }
                catch { };
                try { transport = (Nfe.Danfe.TranspModel)preencheObjXML(transport, ds.Tables["vol"]); }
                catch { };

                if (ds.Tables.Contains("infAdic"))
                {
                    try { inf = (Nfe.Danfe.InfAdicModel)preencheObjXML(inf, ds.Tables["infAdic"]); }
                    catch { };
                }

                total = (Nfe.Danfe.ICMSTotModel)preencheObjXML(total, ds.Tables["ICMSTot"]);
                if (ds.Tables.Contains("infProt"))
                    prot = (Nfe.Danfe.InfProt)preencheObjXML(prot, ds.Tables["infProt"]);

                //NotaFiscal notafiscal = new NotaFiscal();

                int idFornecedor = 0;
                try
                {
                    idFornecedor = 0;//consultaIDViaCNPJ(emitente.CNPJ, emitente.xNome);
                    if (idFornecedor == 0)
                        throw new Exception(" O Fornecedor ( " + notafiscal.Emit.xNome + " )  não esta cadastrado na base de dados.");
                }
                catch (Exception)
                {
                    idFornecedor = 0;
                }

                int idGrupo = 0;
                try
                {
                    idGrupo = 0;// consultaIDGrupoViaCNPJ(cnpj, dest.xNome);

                    if (idGrupo == 0)
                        throw new Exception(" O Destnatário ( " + notafiscal.Emit.xNome + " )  não esta cadastrado na base de dados.");
                }
                catch (Exception)
                {
                    idGrupo = 0;
                }


                notafiscal.idFornecedor = idFornecedor;
                notafiscal.idGrupo = idGrupo;
                notafiscal.Ide = ide;
                notafiscal.Emit = emitente;
                notafiscal.Dest = dest;
                notafiscal.Itens = (List<Nfe.Danfe.detModel>)itens;
                notafiscal.Cobranca = cobr;
                notafiscal.Transp = transport;
                notafiscal.Total = total;
                notafiscal.InfAdic = inf;
                notafiscal.Prot = prot;
                #endregion
            }
            else
                dataset = ds;

            notafiscal.xmlImportado = this.path;
            notafiscal.xmlConteudo = System.IO.File.ReadAllText(this.path);
            string[] split = this.path.Split(new string[] { "\\" }, StringSplitOptions.None);
            notafiscal.nomeArquivo = split[split.Length - 1];

            return notafiscal;
        }

        private void CriaDiretoriosSenaoExistir(string p)
        {
            if (p.PadRight(1) == @"\")
                p = p.PadLeft(p.Length - 1);

            if (!System.IO.Directory.Exists(p))
            {
                System.IO.Directory.CreateDirectory(p);
            }
        }

        public List<string> CarregaXmlsParaApresentarNagradeDeImportacao(string caminhaDoXml)
        {

            string linhaXml = null;
            string[] Arquivos = null;
            List<string> arrayXML = new List<string>();
            if (System.IO.Directory.Exists(caminhaDoXml))
            {
                // carrego a lista de nomes de arquivos contidos na pasta
                Arquivos = Directory.GetFiles(caminhaDoXml, "*.xml");
            }

            foreach (string arquivo in Arquivos)
            {
                using (StreamReader leitor = new StreamReader(arquivo))
                {

                    linhaXml = leitor.ReadToEnd();
                    linhaXml += "¨" + arquivo;
                    arrayXML.Add(linhaXml);

                    //Application.DoEvents();
                }
            }

            return arrayXML;

        }

        public void renomeiaArquivoAposImportacao(string caminhaDoXml, string nomeArquivo)
        {


            List<string> arrayXML = new List<string>();
            string novoNomeArquivo = "";
            novoNomeArquivo = nomeArquivo.Replace(".xml", ".okxml");

            if (System.IO.Directory.Exists(caminhaDoXml))
            {

                //if (File.Exists(caminhaDoXml + "\\" + nomeArquivo))
                if (System.IO.File.Exists(nomeArquivo))
                {
                    // carrego a lista de nomes de arquivos contidos na pasta
                    Directory.Move(nomeArquivo, novoNomeArquivo);
                }
                else
                {
                    novoNomeArquivo = novoNomeArquivo.Replace("ok", "ok" + DateTime.Now.Minute.ToString());
                    // carrego a lista de nomes de arquivos contidos na pasta com o minuto pra nao repitir
                    Directory.Move(nomeArquivo, novoNomeArquivo);
                }
            }


        }

        public List<Nfe.Danfe.NotaFiscal> dadosNFeToImportar(List<string> Lista, string cnpj)
        {
            List<Nfe.Danfe.NotaFiscal> notas = new List<Nfe.Danfe.NotaFiscal>();

            foreach (string sXml in Lista)
            {
                string[] ssXml = sXml.Split('¨');

                DataSet ds = this.GetDataSetXml();

                Nfe.Danfe.IdeModel ide = new Nfe.Danfe.IdeModel();
                Nfe.Danfe.EmitModel emitente = new Nfe.Danfe.EmitModel();
                Nfe.Danfe.enderEmitModel enderEmit = new Nfe.Danfe.enderEmitModel();
                Nfe.Danfe.DestModel dest = new Nfe.Danfe.DestModel();

                Nfe.Danfe.enderDestModel enderDest = new Nfe.Danfe.enderDestModel();
                Nfe.Danfe.ICMSTotModel total = new Nfe.Danfe.ICMSTotModel();
                Nfe.Danfe.InfProt prot = new Nfe.Danfe.InfProt();
                IList<Nfe.Danfe.detModel> itens = new List<Nfe.Danfe.detModel>();
                List<Nfe.Danfe.Duplicata> cobr = new List<Nfe.Danfe.Duplicata>();
                Nfe.Danfe.TranspModel transport = new Nfe.Danfe.TranspModel();
                Nfe.Danfe.InfAdicModel inf = new Nfe.Danfe.InfAdicModel();

                ///
                // preenchendo o Ide da nota
                //
                ide = (Nfe.Danfe.IdeModel)preencheObjXML(ide, ds.Tables["ide"]);
                //
                // Prenchendo o Emitente da nota
                //
                emitente = (Nfe.Danfe.EmitModel)preencheObjXML(emitente, ds.Tables["emit"], ds.Tables["enderEmit"]);
                enderEmit = (Nfe.Danfe.enderEmitModel)preencheObjXML(enderEmit, ds.Tables["enderEmit"]);
                emitente.enderEmit = enderEmit;
                //
                // Prenchendo o destinatario da nota
                //
                dest = (Nfe.Danfe.DestModel)preencheObjXML(dest, ds.Tables["dest"], ds.Tables["enderDest"]);
                enderDest = (Nfe.Danfe.enderDestModel)preencheObjXML(enderDest, ds.Tables["enderDest"]);
                dest.enderDest = enderDest;
                //
                // preenchendo os produtos da nota
                //
                itens = (List<Nfe.Danfe.detModel>)preencheObjXML(itens, ds.Tables["prod"], null);
                // falta importar os impostos ... 

                //
                // Preenchendo cobrança
                //
                try { cobr = (List<Nfe.Danfe.Duplicata>)preencheObjXML(cobr, ds.Tables["dup"], null); }
                catch { };

                //
                // Preenchendo os dados da transportadora
                //

                try { transport = (Nfe.Danfe.TranspModel)preencheObjXML(transport, ds.Tables["transportadora"]); }
                catch { };
                try { transport = (Nfe.Danfe.TranspModel)preencheObjXML(transport, ds.Tables["vol"]); }
                catch { };

                try { inf = (Nfe.Danfe.InfAdicModel)preencheObjXML(inf, ds.Tables["infAdic"]); }
                catch { };

                total = (Nfe.Danfe.ICMSTotModel)preencheObjXML(total, ds.Tables["ICMSTot"]);
                prot = (Nfe.Danfe.InfProt)preencheObjXML(prot, ds.Tables["infProt"]);

                Nfe.Danfe.NotaFiscal nota = new Nfe.Danfe.NotaFiscal();

                int idFornecedor = 0;
                try
                {
                    idFornecedor = 0;//consultaIDViaCNPJ(emitente.CNPJ, emitente.xNome);
                    if (idFornecedor == 0)
                        throw new Exception(" O Fornecedor ( " + nota.Emit.xNome + " )  não esta cadastrado na base de dados.");
                }
                catch (Exception)
                {
                    idFornecedor = 0;
                }

                int idGrupo = 0;
                try
                {
                    idGrupo = 0;// consultaIDGrupoViaCNPJ(cnpj, dest.xNome);

                    if (idGrupo == 0)
                        throw new Exception(" O Destnatário ( " + nota.Emit.xNome + " )  não esta cadastrado na base de dados.");
                }
                catch (Exception)
                {
                    idGrupo = 0;
                }


                nota.idFornecedor = idFornecedor;
                nota.idGrupo = idGrupo;
                nota.Ide = ide;
                nota.Emit = emitente;
                nota.Dest = dest;
                nota.Itens = (List<Nfe.Danfe.detModel>)itens;
                nota.Cobranca = cobr;
                nota.Transp = transport;
                nota.Total = total;
                nota.InfAdic = inf;
                nota.Prot = prot;

                nota.xmlImportado = sXml;
                nota.nomeArquivo = ssXml[1];
                notas.Add(nota);
            }
            return notas.Where<Nfe.Danfe.NotaFiscal>(p => p.Dest.CNPJ == cnpj).ToList<Nfe.Danfe.NotaFiscal>(); ;
        }

        private object preencheObjXML(Object obj, DataTable dt, object SemiPreechido = null)
        {
            object retorno = null;

            if (obj.GetType() == typeof(Nfe.Danfe.IdeModel))
                retorno = preencheIDE(dt);
            else if (obj.GetType() == typeof(Nfe.Danfe.EmitModel))
                retorno = preencheEMIT(dt);
            else if (obj.GetType() == typeof(Nfe.Danfe.enderEmitModel))
                retorno = preencheEnderEmit(dt);
            else if (obj.GetType() == typeof(Nfe.Danfe.DestModel))
                retorno = preencheDEST(dt);
            else if (obj.GetType() == typeof(Nfe.Danfe.enderDestModel))
                retorno = preencheEnderDest(dt);
            else if (obj.GetType() == typeof(Nfe.Danfe.ICMSTotModel))
                retorno = preencheTotal(dt);
            else if (obj.GetType() == typeof(Nfe.Danfe.InfProt))
                retorno = preencheProt(dt);
            else if (obj.GetType() == typeof(Nfe.Danfe.InfAdicModel))
                retorno = preencheInfAdic(dt);
            else if (obj.GetType() == typeof(List<Nfe.Danfe.detModel>))
                retorno = preencheItens(dt);
            else if (obj.GetType() == typeof(List<Nfe.Danfe.Duplicata>))
                retorno = preencheCobranca(dt);
            else if (obj.GetType() == typeof(Nfe.Danfe.TranspModel))
                if (dt.TableName == "transportadora")
                    retorno = preencheTransportadora(dt);
                else // volume
                    retorno = preencheVolume(dt, (Nfe.Danfe.TranspModel)obj);



            return retorno;
        }

        private object preencheInfAdic(DataTable dt, int i = 0)
        {
            Nfe.Danfe.InfAdicModel inf = new Nfe.Danfe.InfAdicModel();
            inf.infCpl = dt.Rows[i]["infCpl"].ToString();

            return inf;
        }

        private object preencheVolume(DataTable dt, Nfe.Danfe.TranspModel transpModel, int i = 0)
        {
            transpModel.vol.esp = dt.Rows[i]["esp"].ToString();
            transpModel.vol.qVol = Convert.ToInt32(dt.Rows[i]["qvol"]);
            transpModel.vol.pesoL = Convert.ToDouble((dt.Rows[i]["pesoL"]));//util.formataDecimal(dt.Rows[i]["pesoL"].ToString());
            transpModel.vol.pesoB = Convert.ToDouble(dt.Rows[i]["pesoB"].ToString());

            return transpModel;
        }

        private object preencheTransportadora(DataTable dt, int i = 0)
        {
            Nfe.Danfe.TranspModel trans = new Nfe.Danfe.TranspModel();

            trans.transportadora.CNPJ = dt.Rows[i]["CNPJ"].ToString();
            trans.transportadora.IE = dt.Rows[i]["IE"].ToString();
            trans.transportadora.UF = dt.Rows[i]["UF"].ToString();
            trans.transportadora.xEnder = dt.Rows[i]["xEnder"].ToString();
            trans.transportadora.xMun = dt.Rows[i]["xMun"].ToString();
            trans.transportadora.xNome = dt.Rows[i]["xNome"].ToString();

            return trans;
        }

        private object preencheCobranca(DataTable dt, int i = 0)
        {
            List<Nfe.Danfe.Duplicata> cbr = new List<Nfe.Danfe.Duplicata>();
            Nfe.Danfe.Duplicata dup = null;
            double valorAcumulo = 0.00;

            //int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                valorAcumulo += Convert.ToDouble(dr["vdup"].ToString());
            }

            foreach (DataRow dr in dt.Rows)
            {
                dup = new Nfe.Danfe.Duplicata();
                dup.Fatura = dr["ndup"].ToString();
                dup.Vencimento = Convert.ToDateTime(dr["dVenc"].ToString());
                dup.Valor = Convert.ToDouble(dr["vdup"].ToString());
                dup.valor_totalDuplicata = valorAcumulo;
                cbr.Add(dup);
            }
            return cbr;
        }

        private List<Nfe.Danfe.detModel> preencheItens(DataTable dt, int i = 0)
        {
            List<Nfe.Danfe.detModel> ListItens = new List<Nfe.Danfe.detModel>();
            Nfe.Danfe.detModel item = null;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    item = new Nfe.Danfe.detModel();
                    item.prod.xProd = dr["xProd"].ToString();
                    item.prod.cProd = dr["cProd"].ToString();
                    try { item.prod.cEAN = dr["cEAN"].ToString(); }
                    catch { }
                    item.prod.NCM = dr["NCM"].ToString();
                    item.prod.CFOP = Convert.ToInt32(dr["CFOP"]);
                    item.prod.uCom = dr["uCom"].ToString();
                    string qCom = dr["qCom"].ToString().Replace(".", "");
                    string qComI = qCom.Substring(0, qCom.Length - 4) + "," + qCom.Substring(qCom.Length - 4, 4);
                    item.prod.qCom = Convert.ToDouble(qComI);
                    string vUnCom = dr["vunCom"].ToString().Replace(".", "");
                    if (vUnCom.Length >= 10)
                        vUnCom = vUnCom.Substring(0, vUnCom.Length - 10) + "," + vUnCom.Substring(vUnCom.Length - 10, 10);
                    else
                        vUnCom = vUnCom.Substring(0, vUnCom.Length - 2) + "," + vUnCom.Substring(vUnCom.Length - 2, 2);
                    item.prod.vUnCom = Convert.ToDouble(vUnCom);
                    string vProd = dr["vProd"].ToString().Replace(".", "");
                    string vProdI = vProd.Substring(0, vProd.Length - 2) + "," + vProd.Substring(vProd.Length - 2, 2);
                    item.prod.vProd = Convert.ToDouble(vProdI);
                    try { item.prod.cEANTrib = dr["cEANTrib"].ToString(); }
                    catch { }
                    item.prod.uTrib = dr["UTrib"].ToString();
                    string qTrib = dr["qTrib"].ToString().Replace(".", "");
                    string qTribI = qTrib.Substring(0, qTrib.Length - 4) + "," + qTrib.Substring(qTrib.Length - 4, 4);
                    item.prod.qTrib = Convert.ToDouble(qTribI);
                    string vUnTrib = dr["vUnTrib"].ToString().Replace(".", "");
                    if (vUnTrib.Length >= 10)
                        vUnTrib = vUnTrib.Substring(0, vUnTrib.Length - 10) + "," + vUnTrib.Substring(vUnTrib.Length - 10, 10);
                    else
                        vUnTrib = vUnTrib.Substring(0, vUnTrib.Length - 2) + "," + vUnTrib.Substring(vUnTrib.Length - 2, 2);
                    item.prod.vUnTrib = Convert.ToDouble(vUnTrib);
                    try
                    {
                        string vFrete = dr["vFrete"].ToString().Replace(".", "");
                        string vFreteI = vFrete.Substring(0, vFrete.Length - 2) + "," + vFrete.Substring(vFrete.Length - 2, 2);
                        item.prod.vFrete = Convert.ToDouble(vFreteI);
                    }
                    catch
                    {
                        item.prod.vFrete = Convert.ToDouble(0);
                    }
                    //item.prod.vFrete = Convert.ToDouble(dr["vFrete"].ToString());
                    item.prod.indTot = Convert.ToInt32(dr["indTot"]);
                    item.prod.vOutros = item.prod.vFrete;
                    item.prod.vTotal = Convert.ToDouble((item.prod.vProd + item.prod.vOutros).ToString());
                    ListItens.Add(item);
                    item = new Nfe.Danfe.detModel();
                }
            }

            return ListItens;
        }

        private object preencheProt(DataTable dt, int i = 0)
        {

            Nfe.Danfe.InfProt inf = new Nfe.Danfe.InfProt();
            inf.chNFe = dt.Rows[i]["chNFe"].ToString();
            inf.cStat = Convert.ToInt32(dt.Rows[i]["cStat"]);
            inf.dhRecbto = Convert.ToDateTime(dt.Rows[i]["dhRecbto"].ToString()).ToString("dd/MM/yyyy HH:mm:ss");
            inf.digVal = dt.Rows[i]["digVal"].ToString();
            if (dt.Columns.Contains("nProt"))
                inf.nProt = Convert.ToInt64(dt.Rows[i]["nProt"]);
            inf.tpAmb = Convert.ToInt32(dt.Rows[i]["tpAmb"]);
            inf.verAplic = dt.Rows[i]["verAplic"].ToString();
            inf.xMotivo = dt.Rows[i]["xMotivo"].ToString();
            return inf;

        }

        private object preencheTotal(DataTable dt, int i = 0)
        {
            Nfe.Danfe.ICMSTotModel total = new Nfe.Danfe.ICMSTotModel();
            total.vBC = Convert.ToDouble(dt.Rows[i]["vBC"].ToString().Replace(".", ","));
            total.vBCST = Convert.ToDouble(dt.Rows[i]["vBCST"].ToString().Replace(".", ","));
            total.vCOFINS = Convert.ToDouble(dt.Rows[i]["vCOFINS"].ToString().Replace(".", ","));
            total.vDesc = Convert.ToDouble(dt.Rows[i]["vDesc"].ToString().Replace(".", ","));
            try { total.vFCPUFDest = Convert.ToDouble(dt.Rows[i]["vFCPUFDest"].ToString().Replace(".", ",")); }
            catch { }
            total.vFrete = Convert.ToDouble(dt.Rows[i]["vFrete"].ToString().Replace(".", ","));
            total.vICMS = Convert.ToDouble(dt.Rows[i]["vICMS"].ToString().Replace(".", ","));

            try { total.vICMSDeson = Convert.ToDouble(dt.Rows[i]["vICMSDeson"].ToString().Replace(".", ",")); }
            catch { };
            try { total.vICMSUFDest = Convert.ToDouble(dt.Rows[i]["vICMSUFDest"].ToString().Replace(".", ",")); }
            catch { };
            try { total.vICMSUFRemet = Convert.ToDouble(dt.Rows[i]["vICMSUFRemet"].ToString().Replace(".", ",")); }
            catch { };
            total.vII = Convert.ToDouble(dt.Rows[i]["vII"].ToString().Replace(".", ","));
            total.vIPI = Convert.ToDouble(dt.Rows[i]["vIPI"].ToString().Replace(".", ","));
            total.vNF = Convert.ToDouble(dt.Rows[i]["vNF"].ToString().Replace(".", ","));
            total.vOutro = Convert.ToDouble(dt.Rows[i]["vOutro"].ToString().Replace(".", ","));
            total.vPIS = Convert.ToDouble(dt.Rows[i]["vPIS"].ToString().Replace(".", ","));
            total.vProd = Convert.ToDouble(dt.Rows[i]["vProd"].ToString().Replace(".", ","));
            total.vSeg = Convert.ToDouble(dt.Rows[i]["vSeg"].ToString().Replace(".", ","));
            total.vST = Convert.ToDouble(dt.Rows[i]["vST"].ToString().Replace(".", ","));
            if (dt.Columns.Contains("vTottrib"))
                total.vTotTrib = Convert.ToDouble(dt.Rows[i]["vTotTrib"].ToString().Replace(".", ","));
            else
                total.vTotTrib = -1;
            return total;
        }

        private object preencheEMIT(DataTable dt, int i = 0)
        {
            Nfe.Danfe.EmitModel emit = new Nfe.Danfe.EmitModel();
            
            if (dt.Columns.Contains("CNAE"))
                emit.CNAE = dt.Rows[i]["CNAE"].ToString();

            if (dt.Columns.Contains("CNPJ"))
                emit.CNPJ = dt.Rows[i]["CNPJ"].ToString();

            if (dt.Columns.Contains("CRT"))
                emit.CRT = Convert.ToInt32(dt.Rows[i]["CRT"]);

            if (dt.Columns.Contains("XFant"))
                emit.xFant = dt.Rows[i]["XFant"].ToString();

            if (dt.Columns.Contains("XNome"))
                emit.xNome = dt.Rows[i]["XNome"].ToString();


            return emit;
        }

        private object preencheDEST(DataTable dt, int i = 0)
        {
            Nfe.Danfe.DestModel dest = new Nfe.Danfe.DestModel();

            if (dt.Columns.Contains("CNPJ"))
                dest.CNPJ = dt.Rows[i]["CNPJ"].ToString();

            if (dt.Columns.Contains("CPF"))
                dest.CNPJ = dt.Rows[i]["CPF"].ToString();

            if (dt.Columns.Contains("XNome"))
                dest.xNome = dt.Rows[i]["XNome"].ToString();

            if (dt.Columns.Contains("IM"))
                dest.IM = dt.Rows[i]["IM"].ToString();

            if (dt.Columns.Contains("indIEDEst"))
                dest.indIEDest = Convert.ToInt32(dt.Rows[i]["indIEDEst"].ToString());

            return dest;
        }

        private object preencheEnderEmit(DataTable dt, int i = 0)
        {
            Nfe.Danfe.enderEmitModel ender = new Nfe.Danfe.enderEmitModel();



            ender = new Nfe.Danfe.enderEmitModel();
            ender.CEP = dt.Rows[i]["Cep"].ToString();
            ender.cMun = dt.Rows[i]["cMun"].ToString();
            if (dt.Columns.Contains("cPais"))
                ender.cPais = Convert.ToInt32(dt.Rows[i]["cPais"]);

            ender.nro = dt.Rows[i]["nro"].ToString();
            ender.UF = dt.Rows[i]["UF"].ToString();
            ender.xBairro = dt.Rows[i]["xBairro"].ToString();

            if (dt.Columns.Contains("xCpl"))
                ender.xCpl = dt.Rows[i]["xCpl"].ToString();

            ender.xLgr = dt.Rows[i]["xLgr"].ToString();
            ender.xMun = dt.Rows[i]["xMun"].ToString();

            if (dt.Columns.Contains("xPais"))
                ender.xPais = dt.Rows[i]["xPais"].ToString();

            if (dt.Columns.Contains("fone"))
                ender.Fone = dt.Rows[i]["fone"].ToString();

            return ender;
        }

        private object preencheEnderDest(DataTable dt, int i = 0)
        {
            Nfe.Danfe.enderDestModel ender = new Nfe.Danfe.enderDestModel();


            ender = new Nfe.Danfe.enderDestModel();
            ender.CEP = dt.Rows[i]["Cep"].ToString();
            ender.cMun = dt.Rows[i]["cMun"].ToString();

            ender.nro = dt.Rows[i]["nro"].ToString();
            ender.UF = dt.Rows[i]["UF"].ToString();
            ender.xBairro = dt.Rows[i]["xBairro"].ToString();

            if (dt.Columns.Contains("xCpl"))
                ender.xCpl = dt.Rows[i]["xCpl"].ToString();

            ender.xLgr = dt.Rows[i]["xLgr"].ToString();
            ender.xMun = dt.Rows[i]["xMun"].ToString();

            if (dt.Columns.Contains("xPais"))
                ender.xPais = dt.Rows[i]["xPais"].ToString();

            if (dt.Columns.Contains("cPais"))
                ender.cPais = Convert.ToInt32(dt.Rows[i]["cPais"].ToString());


            return ender;
        }

        private object preencheIDE(DataTable dt, int i = 0)
        {
            Nfe.Danfe.IdeModel ide = new Nfe.Danfe.IdeModel();
            if (dt.Columns.Contains("cDV"))
                ide.cDV = Convert.ToInt32(dt.Rows[i]["cDV"]);

            if (dt.Columns.Contains("cMunFG"))
                ide.cMunFG = Convert.ToInt32(dt.Rows[i]["cMunFG"]);

            if (dt.Columns.Contains("cNF"))
                ide.cNF = Convert.ToInt32(dt.Rows[i]["cNF"]);
            
            if (dt.Columns.Contains("cUF"))
                ide.cUF = dt.Rows[i]["cUF"].ToString();

            if (dt.Columns.Contains("dhEmi"))
                ide.dhEmi = Convert.ToDateTime(dt.Rows[i]["dhEmi"].ToString()).ToString("dd/MM/yyyy HH:mm:ss");

            if (dt.Columns.Contains("dhSaiEnt"))
                ide.dhSaiEnt = Convert.ToDateTime(dt.Rows[i]["dhSaiEnt"].ToString()).ToString("dd/MM/yyyy HH:mm:ss");

            if (dt.Columns.Contains("finNFe"))
                ide.finNFe = Convert.ToInt32(dt.Rows[i]["finNFe"]);

            if (dt.Columns.Contains("indFinal"))
                ide.indFinal = Convert.ToInt32(dt.Rows[i]["indFinal"]);

            if (dt.Columns.Contains("indPag"))
                ide.indPag = Convert.ToInt32(dt.Rows[i]["indPag"]);

            if (dt.Columns.Contains("indPres"))
                ide.indPres = Convert.ToInt32(dt.Rows[i]["indPres"]);

            if (dt.Columns.Contains("mod"))
                ide.mod = Convert.ToInt32(dt.Rows[i]["mod"]);

            if (dt.Columns.Contains("natOp"))
                ide.natOp = dt.Rows[i]["natOp"].ToString();

            if (dt.Columns.Contains("nNF"))
                ide.nNf = Convert.ToInt32(dt.Rows[i]["nNF"]);

            if (dt.Columns.Contains("ProcEmi"))
                ide.procEmi = Convert.ToInt32(dt.Rows[i]["ProcEmi"]);

            if (dt.Columns.Contains("serie"))
                ide.serie = Convert.ToInt32(dt.Rows[i]["serie"]);

            if (dt.Columns.Contains("tpAmb"))
                ide.tpAmb = Convert.ToInt32(dt.Rows[i]["tpAmb"]);

            if (dt.Columns.Contains("tpEmis"))
                ide.tpEmis = Convert.ToInt32(dt.Rows[i]["tpEmis"]);

            if (dt.Columns.Contains("tpImp"))
                ide.tpImp = Convert.ToInt32(dt.Rows[i]["tpImp"]);

            if (dt.Columns.Contains("tpNF"))
                ide.tpNF = Convert.ToInt32(dt.Rows[i]["tpNF"]);

            if (dt.Columns.Contains("tpAmb"))
                ide.veProc = dt.Rows[i]["tpAmb"].ToString();
            
            return ide;
        }

        public bool validaCruzamentodeComprasComaNota(string vlfalta)
        {
            double faltas = Convert.ToDouble(vlfalta);
            return faltas != 0;
        }
    }

    public class FileNfeServico : File
    {
        public Nfe.Servico.NotaFiscal notafiscal;

        public List<Nfe.Servico.NotaFiscal> notasfiscais;

        public FileNfeServico(string filePath, string fileSheet = null) : base(filePath, fileSheet)
        {
            if (filePath.Contains(".xml"))
                notafiscal = GetNotaFiscal();
            else
                notasfiscais = GetNotasFiscais();
        }

        public Nfe.Servico.NotaFiscal GetNotaFiscal()
        {
            DataSet ds = this.GetDataSetXml();
            this.notafiscal = new Nfe.Servico.NotaFiscal();
            this.notafiscal.Numero = -1;
            if (ds.Tables.Contains("Valores") &&
                ds.Tables.Contains("Servico") &&
                ds.Tables.Contains("PrestadorServico") &&
                ds.Tables.Contains("CpfCnpj") &&
                ds.Tables.Contains("Endereco") &&
                ds.Tables.Contains("Contato") &&
                ds.Tables.Contains("TomadorServico") &&
                ds.Tables.Contains("CpfCnpj") &&
                ds.Tables.Contains("OrgaoGerador") &&
                ds.Tables.Contains("InfNfse"))
            {
                //DataTable dt = ds.Tables["infNFe"];
                //DataColumn dtid = dt.Columns["id"];
                //notafiscal.chave = dtid.Table.Rows[0].ItemArray[1].ToString().Replace("NFe", "");

                Nfe.Servico.ServicoModel Servico = new Nfe.Servico.ServicoModel();
                Nfe.Servico.ValoresModel Valores = new Nfe.Servico.ValoresModel();

                Nfe.Servico.PrestadorServicoModel PrestadorServico = new Nfe.Servico.PrestadorServicoModel();
                Nfe.Servico.IdentificacaoModel IdentificadorPrestador = new Nfe.Servico.IdentificacaoModel();
                Nfe.Servico.EnderecoModel Endereco = new Nfe.Servico.EnderecoModel();
                Nfe.Servico.ContatoModel Contato = new Nfe.Servico.ContatoModel();

                Nfe.Servico.TomadorServicoModel TomadorServico = new Nfe.Servico.TomadorServicoModel();
                Nfe.Servico.IdentificacaoModel IdentificadorTomador = new Nfe.Servico.IdentificacaoModel();

                Nfe.Servico.OrgaoGeradorModel OrgaoGerador = new Nfe.Servico.OrgaoGeradorModel();

                ///
                // preenchendo o Servico da nota
                //
                Valores = (Nfe.Servico.ValoresModel)preencheObjXML(Valores, ds.Tables["Valores"]);
                Servico = (Nfe.Servico.ServicoModel)preencheObjXML(Servico, ds.Tables["Servico"]);
                Servico.Valores = Valores;
                //
                // Prenchendo o PrestadorServico da nota
                //
                PrestadorServico = (Nfe.Servico.PrestadorServicoModel)preencheObjXML(PrestadorServico, ds.Tables["PrestadorServico"]);
                IdentificadorPrestador = (Nfe.Servico.IdentificacaoModel)preencheObjXML(IdentificadorPrestador, ds.Tables["CpfCnpj"], null, 0);
                Endereco = (Nfe.Servico.EnderecoModel)preencheObjXML(Endereco, ds.Tables["Endereco"]);
                Contato = (Nfe.Servico.ContatoModel)preencheObjXML(Contato, ds.Tables["Contato"]);
                PrestadorServico.IdentificacaoPrestador = IdentificadorPrestador;
                PrestadorServico.Endereco = Endereco;
                PrestadorServico.Contato = Contato;
                //
                // Prenchendo o TomadorServico da nota
                //
                TomadorServico = (Nfe.Servico.TomadorServicoModel)preencheObjXML(TomadorServico, ds.Tables["TomadorServico"]);
                IdentificadorTomador = (Nfe.Servico.IdentificacaoModel)preencheObjXML(IdentificadorTomador, ds.Tables["CpfCnpj"], null, 1);
                TomadorServico.IdentificadorTomador = IdentificadorTomador;
                TomadorServico.Endereco = Endereco;
                TomadorServico.Contato = Contato;
                //
                // preenchendo o OrgaoGerador da nota
                //
                OrgaoGerador = (Nfe.Servico.OrgaoGeradorModel)preencheObjXML(OrgaoGerador, ds.Tables["OrgaoGerador"]);

                notafiscal = (Nfe.Servico.NotaFiscal)preencheObjXML(notafiscal, ds.Tables["InfNfse"]);
                notafiscal.Servico = Servico;
                notafiscal.PrestadorServico = PrestadorServico;
                notafiscal.TomadorServico = TomadorServico;
                notafiscal.OrgaoGerador = OrgaoGerador;
            }
            else
                dataset = ds;


            notafiscal.xmlImportado = this.path;
            string[] split = this.path.Split(new string[] { "\\" }, StringSplitOptions.None);
            notafiscal.nomeArquivo = split[split.Length - 1];

            return notafiscal;
        }

        public List<Nfe.Servico.NotaFiscal> GetNotasFiscais()
        {
            DataSet ds = this.GetDataSetTxt();
            this.notasfiscais = new List<Nfe.Servico.NotaFiscal>();
            this.notafiscal = new Nfe.Servico.NotaFiscal();
            this.notafiscal.Numero = -1;
            if (ds.Tables.Contains("Cancelada") && 
                ds.Tables.Contains("Valores") &&
                ds.Tables.Contains("Servico") &&
                ds.Tables.Contains("PrestadorServico") &&
                ds.Tables.Contains("CpfCnpjTomador") &&
                ds.Tables.Contains("EnderecoTomador") &&
                ds.Tables.Contains("ContatoTomador") &&
                ds.Tables.Contains("TomadorServico") &&
                ds.Tables.Contains("OrgaoGerador") &&
                ds.Tables.Contains("InfNfse"))
            {
                #region NotaFiscal
                //DataTable dt = base.GetDataTableOleDb();

                for (int i = 0; i < ds.Tables["Valores"].Rows.Count; i++)
                {

                    Nfe.Servico.ServicoModel Servico = new Nfe.Servico.ServicoModel();
                    Nfe.Servico.ValoresModel Valores = new Nfe.Servico.ValoresModel();

                    Nfe.Servico.PrestadorServicoModel PrestadorServico = new Nfe.Servico.PrestadorServicoModel();
                    Nfe.Servico.IdentificacaoModel IdentificadorPrestador = new Nfe.Servico.IdentificacaoModel();
                    Nfe.Servico.EnderecoModel Endereco = new Nfe.Servico.EnderecoModel();
                    Nfe.Servico.ContatoModel Contato = new Nfe.Servico.ContatoModel();

                    Nfe.Servico.TomadorServicoModel TomadorServico = new Nfe.Servico.TomadorServicoModel();
                    Nfe.Servico.IdentificacaoModel IdentificadorTomador = new Nfe.Servico.IdentificacaoModel();

                    Nfe.Servico.OrgaoGeradorModel OrgaoGerador = new Nfe.Servico.OrgaoGeradorModel();

                    ///
                    // preenchendo o Servico da nota
                    //
                    Valores = (Nfe.Servico.ValoresModel)preencheObjXML(Valores, ds.Tables["Valores"], null, 0, i);
                    Servico = (Nfe.Servico.ServicoModel)preencheObjXML(Servico, ds.Tables["Servico"], null, 0, i);
                    Servico.Valores = Valores;
                    //
                    // Prenchendo o PrestadorServico da nota
                    //
                    //PrestadorServico = (Nfe.Servico.PrestadorServicoModel)preencheObjXML(PrestadorServico, ds.Tables["PrestadorServico"]);
                    //IdentificadorPrestador = (Nfe.Servico.IdentificacaoModel)preencheObjXML(IdentificadorPrestador, ds.Tables["CpfCnpj"], null, 0);
                    //Endereco = (Nfe.Servico.EnderecoModel)preencheObjXML(Endereco, ds.Tables["Endereco"]);
                    //Contato = (Nfe.Servico.ContatoModel)preencheObjXML(Contato, ds.Tables["Contato"]);
                    //PrestadorServico.IdentificacaoPrestador = IdentificadorPrestador;
                    //PrestadorServico.Endereco = Endereco;
                    //PrestadorServico.Contato = Contato;
                    //
                    // Prenchendo o TomadorServico da nota
                    //
                    TomadorServico = (Nfe.Servico.TomadorServicoModel)preencheObjXML(TomadorServico, ds.Tables["TomadorServico"], null, 0, i);
                    IdentificadorTomador = (Nfe.Servico.IdentificacaoModel)preencheObjXML(IdentificadorTomador, ds.Tables["CpfCnpjTomador"], null, 1, i);
                    Endereco = (Nfe.Servico.EnderecoModel)preencheObjXML(Endereco, ds.Tables["EnderecoTomador"], null, 0, i);
                    Contato = (Nfe.Servico.ContatoModel)preencheObjXML(Contato, ds.Tables["ContatoTomador"], null, 0, i);
                    TomadorServico.IdentificadorTomador = IdentificadorTomador;
                    TomadorServico.Endereco = Endereco;
                    TomadorServico.Contato = Contato;
                    //
                    // preenchendo o OrgaoGerador da nota
                    //
                    OrgaoGerador = (Nfe.Servico.OrgaoGeradorModel)preencheObjXML(OrgaoGerador, ds.Tables["OrgaoGerador"], null, 0, i);

                    notafiscal = (Nfe.Servico.NotaFiscal)preencheObjXML(notafiscal, ds.Tables["InfNfse"], null, 0, i);
                    if (notafiscal.Numero == 11784)
                    {

                    }
                    notafiscal.Cancelada = ds.Tables["Cancelada"].Rows[i]["Cancelada"].ToString();
                    notafiscal.Servico = Servico;
                    notafiscal.PrestadorServico = PrestadorServico;
                    notafiscal.TomadorServico = TomadorServico;
                    notafiscal.OrgaoGerador = OrgaoGerador;
                    notafiscal.xmlImportado = this.path;
                    string[] split = this.path.Split(new string[] { "\\" }, StringSplitOptions.None);
                    notafiscal.nomeArquivo = split[split.Length - 1];

                    notasfiscais.Add(notafiscal);
                }
                #endregion
            }
            else
                dataset = ds;

            return notasfiscais;
            //return new List<Nfe.Servico.NotaFiscal>();
        }

        private void CriaDiretoriosSenaoExistir(string p)
        {
            if (p.PadRight(1) == @"\")
                p = p.PadLeft(p.Length - 1);

            if (!System.IO.Directory.Exists(p))
            {
                System.IO.Directory.CreateDirectory(p);
            }
        }

        public List<string> CarregaXmlsParaApresentarNagradeDeImportacao(string caminhaDoXml)
        {

            string linhaXml = null;
            string[] Arquivos = null;
            List<string> arrayXML = new List<string>();
            if (System.IO.Directory.Exists(caminhaDoXml))
            {
                // carrego a lista de nomes de arquivos contidos na pasta
                Arquivos = Directory.GetFiles(caminhaDoXml, "*.xml");
            }

            foreach (string arquivo in Arquivos)
            {
                using (StreamReader leitor = new StreamReader(arquivo))
                {

                    linhaXml = leitor.ReadToEnd();
                    linhaXml += "¨" + arquivo;
                    arrayXML.Add(linhaXml);

                    //Application.DoEvents();
                }
            }

            return arrayXML;

        }

        public void renomeiaArquivoAposImportacao(string caminhaDoXml, string nomeArquivo)
        {


            List<string> arrayXML = new List<string>();
            string novoNomeArquivo = "";
            novoNomeArquivo = nomeArquivo.Replace(".xml", ".okxml");

            if (System.IO.Directory.Exists(caminhaDoXml))
            {

                //if (File.Exists(caminhaDoXml + "\\" + nomeArquivo))
                if (System.IO.File.Exists(nomeArquivo))
                {
                    // carrego a lista de nomes de arquivos contidos na pasta
                    Directory.Move(nomeArquivo, novoNomeArquivo);
                }
                else
                {
                    novoNomeArquivo = novoNomeArquivo.Replace("ok", "ok" + DateTime.Now.Minute.ToString());
                    // carrego a lista de nomes de arquivos contidos na pasta com o minuto pra nao repitir
                    Directory.Move(nomeArquivo, novoNomeArquivo);
                }
            }


        }

        public List<Nfe.Danfe.NotaFiscal> dadosNFeToImportar(List<string> Lista, string cnpj)
        {
            List<Nfe.Danfe.NotaFiscal> notas = new List<Nfe.Danfe.NotaFiscal>();

            foreach (string sXml in Lista)
            {
                string[] ssXml = sXml.Split('¨');

                DataSet ds = this.GetDataSetXml();

                Nfe.Danfe.IdeModel ide = new Nfe.Danfe.IdeModel();
                Nfe.Danfe.EmitModel emitente = new Nfe.Danfe.EmitModel();
                Nfe.Danfe.enderEmitModel enderEmit = new Nfe.Danfe.enderEmitModel();
                Nfe.Danfe.DestModel dest = new Nfe.Danfe.DestModel();

                Nfe.Danfe.enderDestModel enderDest = new Nfe.Danfe.enderDestModel();
                Nfe.Danfe.ICMSTotModel total = new Nfe.Danfe.ICMSTotModel();
                Nfe.Danfe.InfProt prot = new Nfe.Danfe.InfProt();
                IList<Nfe.Danfe.detModel> itens = new List<Nfe.Danfe.detModel>();
                List<Nfe.Danfe.Duplicata> cobr = new List<Nfe.Danfe.Duplicata>();
                Nfe.Danfe.TranspModel transport = new Nfe.Danfe.TranspModel();
                Nfe.Danfe.InfAdicModel inf = new Nfe.Danfe.InfAdicModel();

                ///
                // preenchendo o Ide da nota
                //
                ide = (Nfe.Danfe.IdeModel)preencheObjXML(ide, ds.Tables["ide"]);
                //
                // Prenchendo o Emitente da nota
                //
                emitente = (Nfe.Danfe.EmitModel)preencheObjXML(emitente, ds.Tables["emit"], ds.Tables["enderEmit"]);
                enderEmit = (Nfe.Danfe.enderEmitModel)preencheObjXML(enderEmit, ds.Tables["enderEmit"]);
                emitente.enderEmit = enderEmit;
                //
                // Prenchendo o destinatario da nota
                //
                dest = (Nfe.Danfe.DestModel)preencheObjXML(dest, ds.Tables["dest"], ds.Tables["enderDest"]);
                enderDest = (Nfe.Danfe.enderDestModel)preencheObjXML(enderDest, ds.Tables["enderDest"]);
                dest.enderDest = enderDest;
                //
                // preenchendo os produtos da nota
                //
                itens = (List<Nfe.Danfe.detModel>)preencheObjXML(itens, ds.Tables["prod"], null);
                // falta importar os impostos ... 

                //
                // Preenchendo cobrança
                //
                try { cobr = (List<Nfe.Danfe.Duplicata>)preencheObjXML(cobr, ds.Tables["dup"], null); }
                catch { };

                //
                // Preenchendo os dados da transportadora
                //

                try { transport = (Nfe.Danfe.TranspModel)preencheObjXML(transport, ds.Tables["transportadora"]); }
                catch { };
                try { transport = (Nfe.Danfe.TranspModel)preencheObjXML(transport, ds.Tables["vol"]); }
                catch { };

                try { inf = (Nfe.Danfe.InfAdicModel)preencheObjXML(inf, ds.Tables["infAdic"]); }
                catch { };

                total = (Nfe.Danfe.ICMSTotModel)preencheObjXML(total, ds.Tables["ICMSTot"]);
                prot = (Nfe.Danfe.InfProt)preencheObjXML(prot, ds.Tables["infProt"]);

                Nfe.Danfe.NotaFiscal nota = new Nfe.Danfe.NotaFiscal();

                int idFornecedor = 0;
                try
                {
                    idFornecedor = 0;//consultaIDViaCNPJ(emitente.CNPJ, emitente.xNome);
                    if (idFornecedor == 0)
                        throw new Exception(" O Fornecedor ( " + nota.Emit.xNome + " )  não esta cadastrado na base de dados.");
                }
                catch (Exception)
                {
                    idFornecedor = 0;
                }

                int idGrupo = 0;
                try
                {
                    idGrupo = 0;// consultaIDGrupoViaCNPJ(cnpj, dest.xNome);

                    if (idGrupo == 0)
                        throw new Exception(" O Destnatário ( " + nota.Emit.xNome + " )  não esta cadastrado na base de dados.");
                }
                catch (Exception)
                {
                    idGrupo = 0;
                }


                nota.idFornecedor = idFornecedor;
                nota.idGrupo = idGrupo;
                nota.Ide = ide;
                nota.Emit = emitente;
                nota.Dest = dest;
                nota.Itens = (List<Nfe.Danfe.detModel>)itens;
                nota.Cobranca = cobr;
                nota.Transp = transport;
                nota.Total = total;
                nota.InfAdic = inf;
                nota.Prot = prot;

                nota.xmlImportado = sXml;
                nota.nomeArquivo = ssXml[1];
                notas.Add(nota);
            }
            return notas.Where<Nfe.Danfe.NotaFiscal>(p => p.Dest.CNPJ == cnpj).ToList<Nfe.Danfe.NotaFiscal>(); ;
        }

        private object preencheObjXML(Object obj, DataTable dt, object SemiPreechido = null, int identificacao = 0, int i = 0)
        {
            object retorno = null;

            if (obj.GetType() == typeof(Nfe.Servico.ServicoModel))
                retorno = preencheServico(dt, i);
            else if (obj.GetType() == typeof(Nfe.Servico.ValoresModel))
                retorno = preencheValores(dt, i);
            else if (obj.GetType() == typeof(Nfe.Servico.PrestadorServicoModel))
                retorno = preenchePrestadorServico(dt, i);
            else if (obj.GetType() == typeof(Nfe.Servico.IdentificacaoModel))
                retorno = preencheIdentificacao(dt, identificacao, i);
            else if (obj.GetType() == typeof(Nfe.Servico.EnderecoModel))
                retorno = preencheEndereco(dt, i);
            else if (obj.GetType() == typeof(Nfe.Servico.ContatoModel))
                retorno = preencheContato(dt, i);
            else if (obj.GetType() == typeof(Nfe.Servico.TomadorServicoModel))
                retorno = preencheTomadorServico(dt, i);
            else if (obj.GetType() == typeof(Nfe.Servico.OrgaoGeradorModel))
                retorno = preencheOrgaoGerador(dt, i);
            else if (obj.GetType() == typeof(Nfe.Servico.NotaFiscal))
                retorno = preencheNotaFiscal(dt, i);

            return retorno;
        }

        private object preencheNotaFiscal(DataTable dt, int i = 0)
        {
            Nfe.Servico.NotaFiscal nf = new Nfe.Servico.NotaFiscal();
            nf.Numero = Convert.ToInt32(dt.Rows[i]["Numero"]);
            nf.CodigoVerificacao = dt.Rows[i]["CodigoVerificacao"].ToString();
            nf.DataEmissao = dt.Rows[i]["DataEmissao"].ToString();
            nf.NaturezaOperacao = Convert.ToInt32(dt.Rows[i]["NaturezaOperacao"]);
            nf.RegimeEspecialTributacao = Convert.ToInt32(dt.Rows[i]["RegimeEspecialTributacao"]);
            nf.OptanteSimplesNacional = Convert.ToInt32(dt.Rows[i]["OptanteSimplesNacional"]);
            nf.IncentivadorCultural = Convert.ToInt32(dt.Rows[i]["IncentivadorCultural"]);
            nf.Competencia = dt.Rows[i]["Competencia"].ToString();
            nf.NfseSubstituida = Convert.ToInt32(dt.Rows[i]["NfseSubstituida"]);
            nf.OutrasInformacoes = dt.Rows[i]["OutrasInformacoes"].ToString();
            nf.ValorCredito = Convert.ToDouble(dt.Rows[i]["ValorCredito"]);

            return nf;
        }

        private object preencheOrgaoGerador(DataTable dt, int i = 0)
        {
            Nfe.Servico.OrgaoGeradorModel o = new Nfe.Servico.OrgaoGeradorModel();
            o.CodigoMunicipio = dt.Rows[i]["CodigoMunicipio"].ToString();
            o.Uf = dt.Rows[i]["Uf"].ToString();

            return o;
        }

        private object preencheContato(DataTable dt, int i = 0)
        {
            Nfe.Servico.ContatoModel c = new Nfe.Servico.ContatoModel();

            if (dt.Columns.Contains("Telefone"))
                c.Telefone = dt.Rows[i]["Telefone"].ToString();
            c.Email = dt.Rows[i]["Email"].ToString();

            return c;
        }

        private object preencheEndereco(DataTable dt, int i = 0)
        {
            Nfe.Servico.EnderecoModel e = new Nfe.Servico.EnderecoModel();

            e.Endereco = dt.Rows[i]["Rua"].ToString();
            e.Numero = dt.Rows[i]["Numero"].ToString();
            e.Bairro = dt.Rows[i]["Bairro"].ToString();
            e.Cidade = dt.Rows[i]["Cidade"].ToString();
            e.Estado = dt.Rows[i]["Estado"].ToString();
            e.Cep = dt.Rows[i]["Cep"].ToString();

            return e;
        }

        private object preencheIdentificacao(DataTable dt, int id, int i = 0)
        {
            Nfe.Servico.IdentificacaoModel im = new Nfe.Servico.IdentificacaoModel();
            im.CpfCnpj = new Nfe.Servico.CpfCnpjModel();
            im.CpfCnpj.Cnpj = dt.Rows[id]["Cnpj"].ToString();

            return im;
        }

        private object preencheTomadorServico(DataTable dt, int i = 0)
        {
            Nfe.Servico.TomadorServicoModel p = new Nfe.Servico.TomadorServicoModel();

            p.RazaoSocial = dt.Rows[i]["RazaoSocial"].ToString();

            return p;
        }

        private object preenchePrestadorServico(DataTable dt, int i = 0)
        {
            Nfe.Servico.PrestadorServicoModel p = new Nfe.Servico.PrestadorServicoModel();
            if (dt.Rows.Count > 0)
            {
                p.RazaoSocial = dt.Rows[i]["RazaoSocial"].ToString();
                p.NomeFantasia = dt.Rows[i]["NomeFantasia"].ToString();
            }
            return p;
        }

        private object preencheValores(DataTable dt, int i = 0)
        {
            Nfe.Servico.ValoresModel v = new Nfe.Servico.ValoresModel();

            if (dt.Columns.Contains("ValorServicos"))
                v.ValorServicos = Convert.ToDouble(dt.Rows[i]["ValorServicos"].ToString().Replace(".", ","));
            if (dt.Columns.Contains("ValorDeducoes"))
                v.ValorDeducoes = Convert.ToDouble(dt.Rows[i]["ValorDeducoes"].ToString().Replace(".", ","));
            if (dt.Columns.Contains("ValorPis"))
                v.ValorPis = Convert.ToDouble(dt.Rows[i]["ValorPis"].ToString().Replace(".", ","));
            if (dt.Columns.Contains("ValorCofins"))
                v.ValorCofins = Convert.ToDouble(dt.Rows[i]["ValorCofins"].ToString().Replace(".", ","));
            if (dt.Columns.Contains("ValorIr"))
                v.ValorIr = Convert.ToDouble(dt.Rows[i]["ValorIr"].ToString().Replace(".", ","));
            if (dt.Columns.Contains("ValorCsll"))
                v.ValorCsll = Convert.ToDouble(dt.Rows[i]["ValorCsll"].ToString().Replace(".", ","));
            if (dt.Columns.Contains("IssRetido"))
                v.IssRetido = Convert.ToDouble(dt.Rows[i]["IssRetido"].ToString().Replace(".", ","));
            if (dt.Columns.Contains("OutrasRetencoes"))
                v.OutrasRetencoes = Convert.ToDouble(dt.Rows[i]["OutrasRetencoes"].ToString().Replace(".", ","));
            if (dt.Columns.Contains("BaseCalculo"))
                v.BaseCalculo = Convert.ToDouble(dt.Rows[i]["BaseCalculo"].ToString().Replace(".", ","));
            if (dt.Columns.Contains("Aliquota"))
                v.Aliquota = Convert.ToDouble(dt.Rows[i]["Aliquota"].ToString().Replace(".", ","));
            if (dt.Columns.Contains("ValorLiquidoNfse"))
                v.ValorLiquidoNfse = Convert.ToDouble(dt.Rows[i]["ValorLiquidoNfse"].ToString().Replace(".", ","));
            if (dt.Columns.Contains("ValorIssRetido"))
                v.ValorIssRetido = Convert.ToDouble(dt.Rows[i]["ValorIssRetido"].ToString().Replace(".", ","));
            if (dt.Columns.Contains("ValorIss"))
                v.ValorIss = Convert.ToDouble(dt.Rows[i]["ValorIss"].ToString().Replace(".", ","));
            if (dt.Columns.Contains("DescontoCondicionado"))
                v.DescontoCondicionado = Convert.ToDouble(dt.Rows[i]["DescontoCondicionado"].ToString().Replace(".", ","));
            if (dt.Columns.Contains("DescontoIncondicionado"))
                v.DescontoIncondicionado = Convert.ToDouble(dt.Rows[i]["DescontoIncondicionado"].ToString().Replace(".", ","));

            return v;
        }

        private object preencheServico(DataTable dt, int i = 0)
        {
            Nfe.Servico.ServicoModel s = new Nfe.Servico.ServicoModel();
            s.ItemListaServico = Convert.ToInt32(dt.Rows[i]["ItemListaServico"]);
            s.CodigoCnae = Convert.ToString(dt.Rows[i]["CodigoCnae"]);
            s.CodigoTributacaoMunicipio = Convert.ToInt32(dt.Rows[i]["CodigoTributacaoMunicipio"]);
            s.Discriminacao = dt.Rows[i]["Discriminacao"].ToString();
            s.MunicipioPrestacaoServico = Convert.ToInt32(dt.Rows[i]["MunicipioPrestacaoServico"].ToString());

            return s;
        }

        public bool validaCruzamentodeComprasComaNota(string vlfalta)
        {
            double faltas = Convert.ToDouble(vlfalta);
            return faltas != 0;
        }
    }
}
