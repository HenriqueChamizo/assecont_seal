using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diferimento
{
    public static class Convert
    {
        public static List<Model.Seal.Cliente> Clientes(List<Assecontweb.Extend.CFechamento.Cliente> clientes)
        {
            if (clientes == null)
                return null;
            List<Model.Seal.Cliente> retorno = new List<Model.Seal.Cliente>();
            Model.Seal.Cliente cli;
            foreach (Assecontweb.Extend.CFechamento.Cliente cliente in clientes)
            {
                cli = new Model.Seal.Cliente();
                cli.codigo = cliente.codigo;
                cli.codigoGrupo = cliente.codigoGrupo;
                cli.nome = cliente.nome;
                cli.tipo = cliente.tipo;
                cli.cnpj = cliente.cnpj;

                retorno.Add(cli);
            }
            return retorno;
        }
        public static List<Model.Seal.ContaReceber> ContasReceber(List<Assecontweb.Extend.CFechamento.ContaReceber> contasreceber)
        {
            if (contasreceber == null)
                return null;
            List<Model.Seal.ContaReceber> retorno = new List<Model.Seal.ContaReceber>();
            Model.Seal.ContaReceber cr;
            foreach (Assecontweb.Extend.CFechamento.ContaReceber conta in contasreceber)
            {
                cr = new Model.Seal.ContaReceber();
                cr.cliente = conta.cliente;
                cr.codigo = conta.codigo;
                cr.dataRecto = conta.dataRecto;
                cr.desconto = conta.desconto;
                cr.descricao = conta.descricao;
                cr.docOrig = conta.docOrig;
                cr.documentoOrigem = conta.documentoOrigem;
                cr.emissao = conta.emissao;
                cr.forma = conta.forma;
                cr.juros = conta.juros;
                cr.notaFiscal = conta.notaFiscal;
                cr.prest = conta.prest;
                cr.valor = conta.valor;
                cr.valorRecebido = conta.valorRecebido;
                cr.vencimento = conta.vencimento;

                retorno.Add(cr);
            }
            return retorno;
        }
        public static List<Assecontweb.Extend.CFechamento.Cliente> ClientesInverse(List<Model.Seal.Cliente> clientes)
        {
            if (clientes == null)
                return null;
            List<Assecontweb.Extend.CFechamento.Cliente> retorno = new List<Assecontweb.Extend.CFechamento.Cliente>();
            Assecontweb.Extend.CFechamento.Cliente cli;
            foreach (Model.Seal.Cliente cliente in clientes)
            {
                cli = new Assecontweb.Extend.CFechamento.Cliente();
                cli.codigo = cliente.codigo;
                cli.codigoGrupo = cliente.codigoGrupo;
                cli.nome = cliente.nome;
                cli.tipo = cliente.tipo;
                cli.cnpj = cliente.cnpj;

                retorno.Add(cli);
            }
            return retorno;
        }
        public static List<Assecontweb.Extend.CFechamento.ContaReceber> ContasReceberInverse(List<Model.Seal.ContaReceber> contasreceber)
        {
            if (contasreceber == null)
                return null;
            List<Assecontweb.Extend.CFechamento.ContaReceber> retorno = new List<Assecontweb.Extend.CFechamento.ContaReceber>();
            Assecontweb.Extend.CFechamento.ContaReceber cr;
            foreach (Model.Seal.ContaReceber conta in contasreceber)
            {
                cr = new Assecontweb.Extend.CFechamento.ContaReceber();
                cr.cliente = conta.cliente;
                cr.codigo = conta.codigo;
                cr.dataRecto = conta.dataRecto;
                cr.desconto = conta.desconto;
                cr.descricao = conta.descricao;
                cr.docOrig = conta.docOrig;
                cr.documentoOrigem = conta.documentoOrigem;
                cr.emissao = conta.emissao;
                cr.forma = conta.forma;
                cr.juros = conta.juros;
                cr.notaFiscal = conta.notaFiscal;
                cr.prest = conta.prest;
                cr.valor = conta.valor;
                cr.valorRecebido = conta.valorRecebido;
                cr.vencimento = conta.vencimento;

                retorno.Add(cr);
            }
            return retorno;
        }
    }
}
