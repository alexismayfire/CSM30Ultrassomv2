using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Web.Lib;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        static ProcessaUltrassom processamento = new ProcessaUltrassom();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
   
        [HttpPost]
        public string Save(string nomeDoArquivo, int intensidade, string resultadoFinal)
        {
            var path = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/");
            double intensidadeCalculada = 1 + (intensidade / 100.0);
            nomeDoArquivo = "#" + intensidadeCalculada.ToString() + "#" + nomeDoArquivo;
            string nomeDoArquivoBuscado = nomeDoArquivo.Replace(".txt", "");
            
            string nomeArquivoImagem = nomeDoArquivo.Replace(".txt", ".bmp");

            //TODO: Alterar para retornar o arquivo com a imagem ou retornar uma mensagem de processando 
            // O parâmetro nomeDoArquivo recebe o valor que o usuário digita no formulário!

            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (FileInfo file in dir.GetFiles())
            {
                if (file.ToString().Contains(nomeDoArquivoBuscado))
                    return file.Name;
            }

            if(processamento.getElementoFila(nomeDoArquivo))
            {
                return "O arquivo já está sendo processado.";
            }
            else
            {
                int rows = 50816;
                double[] g = new double[rows];
                var lines = resultadoFinal.Split(',');
                for (int i = 0; i < rows; i++)
                {
                    g[i] = Double.Parse(lines[i]);
                }

                processamento.processa(nomeDoArquivo, g, intensidadeCalculada);
                return "Adicionando na fila de processamento";
            }
        }
    }
}