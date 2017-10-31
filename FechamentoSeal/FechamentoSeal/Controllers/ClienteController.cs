using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FechamentoSeal.Controllers
{
    public class ClienteController : HController
    {
        // GET: Cliente
        public ActionResult Index()
        {
            if (!Logado())
                return RedirectToAction("Inicio", "Home");

            return View();
        }
        
        public ActionResult SaveUploadedFileClientes()
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            string fPath = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {

                        var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\WallImages", Server.MapPath(@"\")));

                        string pathString = Path.Combine(originalDirectory.ToString(), "imagepath");

                        var fileName1 = Path.GetFileName(file.FileName);

                        bool isExists = Directory.Exists(pathString);

                        if (!isExists)
                            Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, file.FileName);
                        file.SaveAs(path);
                        fPath = path;
                    }

                }

            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }


            if (isSavedSuccessfully)
            {
                //Teste(fPath);
                return Json(new { Message = fName });
            }
            else
            {
                return Json(new { Message = "Error in saving file" });
            }
        }
        
        public ActionResult SaveUploadedFileContasReceber()
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            string fPath = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {

                        var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\WallImages", Server.MapPath(@"\")));

                        string pathString = Path.Combine(originalDirectory.ToString(), "imagepath");

                        var fileName1 = Path.GetFileName(file.FileName);

                        bool isExists = Directory.Exists(pathString);

                        if (!isExists)
                            Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, file.FileName);
                        file.SaveAs(path);
                        fPath = path;
                    }

                }

            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }


            if (isSavedSuccessfully)
            {
                //Teste(fPath);
                return Json(new { Message = fName });
            }
            else
            {
                return Json(new { Message = "Error in saving file" });
            }
        }

        public void Teste(string arquivo)
        {
            //string arquivo = @"C:\Users\edvaldob\Documents\Visual Studio 2012\Projects\ProgramasAcessorios\importadorArquivoXLS\importadorArquivoXLS\bin\Debug\Inventario_2016_PA.xlsx";
            string strConexao = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=0\"", arquivo);
            //string strConexao = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=0\"", arquivo);
            OleDbConnection conn = new OleDbConnection(strConexao);
            conn.Open();
            DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            //Cria o objeto dataset para receber o conteúdo do arquivo Excel
            DataSet output = new DataSet();
            DataTable outputTable = null;
            // obtem o noma da planilha corrente
            // string sheet = row["Inventario_2016"].ToString();
            string sheet = "Original";
            // obtem todos as linhas da planilha corrente
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + sheet + "]", conn);
            cmd.CommandType = CommandType.Text;
            // copia os dados da planilha para o datatable
            outputTable = new DataTable(sheet);
            output.Tables.Add(outputTable);
            new OleDbDataAdapter(cmd).Fill(outputTable);

            formataLayoutArquivo(outputTable);
        }

        private void formataLayoutArquivo(DataTable outputTable)
        {
            string dtInventario, mesAnoIni, mesAnoFim, cdProduto, sitProd = "1", cnpj, ie, UF, filler5, qtde, vlUnit, vltot, icmsRecup, obs60, descricao, Grupo, NCM, filler30, un, dsGrupo, dsUnidade, vlImpostoRenda, motvo = "01";
            List<string> linhas = new List<string>();
            string linha = "";

            foreach (DataRow row in outputTable.Rows)
            {
                if (row["dtInvent"].ToString().Replace("/", "").Trim() == "")
                    continue;
                dtInventario = row["dtInvent"].ToString().Replace("/", "");
                mesAnoIni = row["MMYYI"].ToString().Replace("/", "");
                mesAnoFim = row["MMYYF"].ToString().Replace("/", "");
                cdProduto = row["codigo"].ToString().PadLeft(20, ' ');
                //sitprod
                cnpj = "".PadRight(14, '0');
                ie = "".PadRight(20, '0');
                UF = "".PadRight(2, ' ');
                filler5 = "".PadRight(5, ' ');
                qtde = row["qtde"].ToString().PadLeft(16, '0').Replace(",", ".");
                vlUnit = Convert.ToDouble(row["Unit"]).ToString("F2").PadLeft(17, '0').Replace(",", ".");
                vltot = Convert.ToDouble(row["Total"]).ToString("F2").PadLeft(17, '0').Replace(",", ".");
                icmsRecup = "".PadLeft(15, '0') + ".00";
                obs60 = "".PadLeft(60, ' ');
                if (row["Descricao"].ToString().Length > 80)
                    descricao = row["Descricao"].ToString().Substring(0, 80);
                else
                    descricao = row["Descricao"].ToString().PadRight(80, ' ');

                Grupo = "".PadRight(4, '0');
                NCM = row["NCM"].ToString().PadRight(10, ' ');
                filler30 = "".PadRight(29, ' ');
                un = row["UN"].ToString().PadRight(3, ' ');
                //filler30 = "".PadRight(30, ' ');
                dsUnidade = row["UN"].ToString().PadRight(6, ' ');
                vlImpostoRenda = Convert.ToDouble(row["Total"]).ToString("F2").PadLeft(17, '0').Replace(",", ".");
                //motvo
                linha =
                         dtInventario + mesAnoIni + mesAnoFim + cdProduto + sitProd + cnpj + ie + UF;
                linha += filler5 + qtde + vlUnit + vltot + icmsRecup + obs60 + descricao;
                linha += Grupo + NCM + filler30 + un + filler30 + dsUnidade;
                linha += vlImpostoRenda + motvo;

                linhas.Add(linha);
            }

            gravalinhasNoArquivo(linhas);
        }

        private void gravalinhasNoArquivo(List<string> linhas)
        {
            string path = @"ArquivoSaida.txt";

            // This text is added only once to the file.
            if (!System.IO.File.Exists(path))
            {
                // Create a file to write to.

                // File.WriteAllLines(path, linhas, Encoding.UTF8);
                System.IO.File.WriteAllLines(path, linhas, Encoding.ASCII);
            }
        }
    }
}