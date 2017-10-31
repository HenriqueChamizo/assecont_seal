using System.Collections.Generic;
using Assecontweb.Extend.CFechamento;
using Assecontweb.Extend.Nfe;
using System;

namespace Assecontweb.Extend
{
    public class Fechamento
    {
        public List<Cliente> clientes { get; set; }
        public List<ContaReceber> contasReceber { get; set; }
        public List<Nfe.Danfe.NotaFiscal> danfes { get; set; }
        public List<Nfe.Servico.NotaFiscal> servicos { get; set; }

        public Fechamento(List<Cliente> lClientes, List<ContaReceber> cReceber)
        {
            clientes = lClientes;
            contasReceber = cReceber;
        }

        public Fechamento(List<Cliente> lClientes, List<ContaReceber> cReceber, List<Nfe.Danfe.NotaFiscal> dFiscal, List<Nfe.Servico.NotaFiscal> sFiscal)
        {
            clientes = lClientes;
            contasReceber = cReceber;
            danfes = dFiscal;
            servicos = sFiscal;
        }

        public Cliente FindCliente(string codigo)
        {
            return clientes.Find(f => f.codigo == codigo);
        }

        public List<Cliente> FindClientes(string grupo)
        {
            return clientes.FindAll(f => f.codigoGrupo == grupo);
        }

        public List<ContaReceber> FindContasReceber(string codigo = null, string notafiscal = null, string grupo = null)
        {
            if (string.IsNullOrEmpty(codigo))
                return contasReceber.FindAll(f => f.codigo == codigo);
            else if (string.IsNullOrEmpty(notafiscal))
                return contasReceber.FindAll(f => f.notaFiscal == notafiscal);
            else if (string.IsNullOrEmpty(grupo))
                return contasReceber.FindAll(fr => clientes.Find(fc => fc.codigo == fr.codigo && fc.codigoGrupo == grupo) != null);
            else
                return null;
        }

        public double SumValorRecebido(List<ContaReceber> listBySum)
        {
            double retorno = 0;
            foreach (ContaReceber c in listBySum)
            {
                retorno = retorno + c.valorRecebido;
            }
            return retorno;
        }

        public double SumValorRecebidoDiferimento()
        {
            List<ContaReceber> l = GetContasReceberByDiferimento();
            double retorno = 0;
            foreach (ContaReceber c in l)
            {
                retorno = retorno + c.valorRecebido;
            }
            return retorno;
        }

        public List<Cliente> GetClientesByDiferimento()
        {
            return clientes.FindAll(f => f.codigoGrupo.Contains("Lei 9718"));
        }

        public List<ContaReceber> GetContasReceberByDiferimento()
        {
            return contasReceber.FindAll(fr => clientes.FindAll(fc => fc.codigoGrupo.Contains("Lei 9718") && fc.codigo == fr.codigo) != null);
        }

        public void GetDanfesNotas(ref List<Nfe.Danfe.NotaFiscal> recebidas, 
                                   ref List<Nfe.Danfe.NotaFiscal> nRecebidas)
        {
            List<ContaReceber> contas = new List<ContaReceber>();
            foreach (Nfe.Danfe.NotaFiscal danfe in danfes)
            {
                List<ContaReceber> cts = contasReceber.FindAll(f => Convert.ToInt32(f.notaFiscal.Split(new string[] { " " }, StringSplitOptions.None)[1]) == danfe.Ide.nNf);
                if (cts != null && cts.Count > 0)
                    recebidas.Add(danfe);
                else
                    nRecebidas.Add(danfe);
                contas.AddRange(cts);
            }
        }

        public void GetServicosNotas(ref List<Nfe.Servico.NotaFiscal> recebidos,
                                     ref List<Nfe.Servico.NotaFiscal> nRecebidos)
        {
            List<ContaReceber> contas = new List<ContaReceber>();
            foreach (Nfe.Servico.NotaFiscal servico in servicos)
            {
                List<ContaReceber> cts = contasReceber.FindAll(f => Convert.ToInt32(f.notaFiscal.Split(new string[] { " " }, StringSplitOptions.None)[1]) == servico.Numero);
                if (cts != null && cts.Count > 0)
                    recebidos.Add(servico);
                else
                    nRecebidos.Add(servico);
                contas.AddRange(cts);
            }
        }

        public List<Cliente> GetClientesContribuintes()
        {
            List<Cliente> retorno = new List<Cliente>();
            foreach (Cliente cli in clientes)
            {
                if (cli.codigoGrupo == "CONTRIBUINTE")
                    retorno.Add(cli);
            }
            return retorno;
        }

        public List<Cliente> GetClientesNContribuintes()
        {
            List<Cliente> retorno = new List<Cliente>();
            foreach (Cliente cli in clientes)
            {
                if (cli.codigoGrupo == "NÃO CONTRIBUINTE")
                    retorno.Add(cli);
            }
            return retorno;
        }

        public List<Cliente> GetClientesLei()
        {
            List<Cliente> retorno = new List<Cliente>();
            foreach (Cliente cli in clientes)
            {
                if (cli.codigoGrupo == "CONTRIB - Lei 9718")
                    retorno.Add(cli);
            }
            return retorno;
        }

        public List<Cliente> GetClientesNLei()
        {
            List<Cliente> retorno = new List<Cliente>();
            foreach (Cliente cli in clientes)
            {
                if (cli.codigoGrupo == "Ñ CONTRIB - Lei 9718")
                    retorno.Add(cli);
            }
            return retorno;
        }
    }
}
