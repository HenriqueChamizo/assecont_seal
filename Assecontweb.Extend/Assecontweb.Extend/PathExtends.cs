using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assecontweb.Extend
{
    public class Path : IDisposable
    {
        public string path { get; set; }
        public DirectoryInfo directoryInfo { get; set; }
        public string[] subpaths { get; set; }
        public string[] subfiles { get; set; }

        public Path(string sPath)
        {
            path = sPath;
            subpaths = GetAllDirectories(path);
            subfiles = GetAllFiles();
        }

        public DirectoryInfo GetDirectoryInfo()
        {
            return new DirectoryInfo(path);
        }

        public string[] GetAllDirectories(string sPath)
        {
            string[] retorno;
            string[] subs = Directory.GetDirectories(sPath);
            List<string> paths = new List<string>();
            if (!paths.Contains(sPath))
                paths.Add(sPath);
            foreach (string sub in subs)
            {
                if (!paths.Contains(sub))
                    paths.Add(sub);
                string[] subsubs = GetAllDirectories(sub);
                foreach (string subsub in subsubs)
                {
                    if (!paths.Contains(subsub))
                        paths.Add(subsub);
                }
            }
            retorno = new string[paths.Count];
            int count = 0;
            foreach (string p in paths)
            {
                retorno[count] = p;
                count++;
            }
            return retorno;
        }

        public string[] GetAllFiles()
        {
            string[] retorno;
            string[] paths = subpaths;
            List<string> files = new List<string>();
            foreach (string p in paths)
            {
                string[] fs = Directory.GetFiles(p);
                foreach (string f in fs)
                {
                    if (!files.Contains(f))
                        files.Add(f);
                }
            }
            retorno = new string[files.Count];
            int count = 0;
            foreach (string file in files)
            {
                retorno[count] = file;
                count++;
            }
            return retorno;
        }

        public string[] GetAllFiles(string sPath)
        {
            string[] retorno;
            string[] paths = GetAllDirectories(sPath);
            List<string> files = new List<string>();
            foreach (string p in paths)
            {
                string[] fs = Directory.GetFiles(p);
                foreach (string f in fs)
                {
                    if (!files.Contains(f))
                        files.Add(f);
                }
            }
            retorno = new string[files.Count];
            int count = 0;
            foreach (string file in files)
            {
                retorno[count] = file;
                count++;
            }
            return retorno;
        }

        public string[] GetAllFiles(string[] paths)
        {
            string[] retorno;
            List<string> files = new List<string>();
            foreach (string p in paths)
            {
                string[] fs = Directory.GetFiles(p);
                foreach (string f in fs)
                {
                    if (!files.Contains(f))
                        files.Add(f);
                }
            }
            retorno = new string[files.Count];
            int count = 0;
            foreach (string file in files)
            {
                retorno[count] = file;
                count++;
            }
            return retorno;
        }

        public void Dispose()
        {

        }
    }

    public class PathNfe : Path, IDisposable
    {
        public List<FileNfeDanfe> FilesDanfes { get; set; }
        public List<FileNfeServico> FilesServicos { get; set; }

        public PathNfe(string sPath)
            : base(sPath)
        {
            GetFilesNfe();
        }

        public void GetFilesNfe()
        {
            List<FileNfeDanfe> danfes = new List<FileNfeDanfe>();
            FileNfeDanfe danfe;
            List<FileNfeServico> servicos = new List<FileNfeServico>();
            FileNfeServico servico;
            foreach (string file in subfiles)
            {
                if (file.Contains(".xml"))//&& file.Contains("procNFE"))
                {
                    danfe = new FileNfeDanfe(file);
                    if (danfe.notafiscal.idFornecedor != -1)
                        danfes.Add(danfe);
                    else
                    {
                        servico = new FileNfeServico(file);
                        if (servico.notafiscal.Numero != -1)
                            servicos.Add(servico);
                    }
                }
            }

            FilesDanfes = danfes;
            FilesServicos = servicos;
        }
    }

    public class PathNfeDanfe : Path, IDisposable
    {
        public List<FileNfeDanfe> FilesDanfes { get; set; }

        public PathNfeDanfe(string sPath)
            : base(sPath)
        {
            FilesDanfes = GetFilesDanfes();
        }

        public List<FileNfeDanfe> GetFilesDanfes()
        {
            List<FileNfeDanfe> danfes = new List<FileNfeDanfe>();
            FileNfeDanfe danfe;
            foreach (string file in subfiles)
            {
                if (file.Contains(".xml"))//&& file.Contains("procNFE"))
                {
                    danfe = new FileNfeDanfe(file);
                    if (danfe.notafiscal.idFornecedor != -1)
                        danfes.Add(danfe);
                }
            }
            return danfes;
        }
    }

    public class PathNfeServico : Path, IDisposable
    {
        public List<FileNfeServico> FilesServicos { get; set; }

        public PathNfeServico(string sPath)
            : base(sPath)
        {
            FilesServicos = GetFilesServicos();
        }

        public List<FileNfeServico> GetFilesServicos()
        {
            List<FileNfeServico> servicos = new List<FileNfeServico>();
            FileNfeServico servico;
            foreach (string file in subfiles)
            {
                if (file.Contains(".xml"))//&& file.Contains("procNFE"))
                {
                    servico = new FileNfeServico(file);
                    if (servico.notafiscal.Numero != -1)
                        servicos.Add(servico);
                }
            }
            return servicos;
        }
    }
}
