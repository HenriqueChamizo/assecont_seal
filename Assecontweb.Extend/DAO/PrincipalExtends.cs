using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class Detail
    {
        public DanfeFatu DanfeFaturada { get; set; }
        public ServicoFatu ServicoFaturado { get; set; }

        public Detail()
        {
            DanfeFaturada = new DanfeFatu();

            ServicoFaturado = new ServicoFatu();
        }
    }

    public class DanfeFatu
    {
        public double SPPrivado { get; set; }
        public double SPLei { get; set; }
        public double SPFatu { get; set; }

        public double MSPrivado { get; set; }
        public double MSLei { get; set; }
        public double MSFatu { get; set; }

        public DanfeFatu Recebidas { get; set; }
        public DanfeFatu NRecebidas { get; set; }
        public DanfeFatu Contribuinte { get; set; }
        public DanfeFatu NContribuinte { get; set; }

        public DanfeFatu(bool subclass = false)
        {
            SPPrivado = 0;
            SPLei = 0;
            SPFatu = 0;

            MSPrivado = 0;
            MSLei = 0;
            MSFatu = 0;

            if (!subclass)
            {
                Recebidas = new DanfeFatu(true);
                NRecebidas = new DanfeFatu(true);
                Contribuinte = new DanfeFatu(true);
                NContribuinte = new DanfeFatu(true);
            }
        }
    }

    public class ServicoFatu
    {
        public double SPPrivado { get; set; }
        public double SPLei { get; set; }
        public double SPFatu { get; set; }

        public double MSPrivado { get; set; }
        public double MSLei { get; set; }
        public double MSFatu { get; set; }

        public DanfeFatu Recebidas { get; set; }
        public DanfeFatu NRecebidas { get; set; }
        public DanfeFatu Cumulativo { get; set; }
        public DanfeFatu NCumulativo { get; set; }

        public ServicoFatu(bool subclass = false)
        {
            SPPrivado = 0;
            SPLei = 0;
            SPFatu = 0;

            MSPrivado = 0;
            MSLei = 0;
            MSFatu = 0;

            if (!subclass)
            {
                Recebidas = new DanfeFatu(true);
                NRecebidas = new DanfeFatu(true);
                Cumulativo = new DanfeFatu(true);
                NCumulativo = new DanfeFatu(true);
            }
        }
    }


    public class Consolidado
    {
        public DanfeConsu DanfeConsolidado { get; set; }
        public ServicoConsu ServicoConsuNCum { get; set; }
        public ServicoConsu ServicoConsuCum { get; set; }
        public RetencaoConsu RetencaoConsuNCum { get; set; }
        public RetencaoConsu RetencaoConsuCum { get; set; }

        public Consolidado()
        {
            DanfeConsolidado = new DanfeConsu();
            ServicoConsuNCum = new ServicoConsu();
            ServicoConsuCum = new ServicoConsu();
            RetencaoConsuNCum = new RetencaoConsu();
            RetencaoConsuCum = new RetencaoConsu();
        }
    }

    public class DanfeConsu
    {
        public double SP { get; set; }
        public double MS { get; set; }
        public double Tot { get; set; }

        public DanfeConsu Normais { get; set; }
        public DanfeConsu Exclusao { get; set; }
        public DanfeConsu Adicao { get; set; }

        public DanfeConsu(bool subclass = false)
        {
            SP = 0;
            MS = 0;
            Tot = 0;

            if (!subclass)
            {
                Normais = new DanfeConsu(true);
                Exclusao = new DanfeConsu(true);
                Adicao = new DanfeConsu(true);
            }
        }
    }

    public class ServicoConsu
    {
        public double SP { get; set; }
        public double MS { get; set; }
        public double Tot { get; set; }

        public ServicoConsu Normais { get; set; }
        public ServicoConsu Exclusao { get; set; }
        public ServicoConsu Adicao { get; set; }

        public ServicoConsu(bool subclass = false)
        {
            SP = 0;
            MS = 0;
            Tot = 0;

            if (!subclass)
            {
                Normais = new ServicoConsu(true);
                Exclusao = new ServicoConsu(true);
                Adicao = new ServicoConsu(true);
            }
        }
    }

    public class RetencaoConsu
    {
        public double PisPrivado { get; set; }
        public double CofinsPrivado { get; set; }
        public double IrPrivado { get; set; }
        public double PisPublico { get; set; }
        public double CofinsPublico { get; set; }
        public double IrPublico { get; set; }

        public RetencaoConsu()
        {
            PisPrivado = 0;
            CofinsPrivado = 0;
            IrPrivado = 0;
            PisPublico = 0;
            CofinsPublico = 0;
            IrPublico = 0;
        }
    }
}
