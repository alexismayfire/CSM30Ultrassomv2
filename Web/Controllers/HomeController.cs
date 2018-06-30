using System;
using System.Web.Mvc;
using CSM30Ultrassom;

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
        public string Save(int larguraDaImagem, string nomeDoArquivo, int intensidade, double[] resultadoFinal)
        {
            string nomeArquivoImagem = nomeDoArquivo.Replace(".txt", ".bmp");

            //TODO: Alterar para retornar o arquivo com a imagem ou retornar uma mensagem de processando 
            if (System.IO.File.Exists(@".\" + nomeArquivoImagem)) 
                return "File exists."; //retorna o bitmap

            else if(processamento.getElementoFila(nomeDoArquivo))
            {
                return "O arquivo já está sendo processado.";
            }
            else
            {
                processamento.processa("nomeDoArquivo", resultadoFinal);
                return "Adicionando na fila de processamento";
            }
                


            //return "Largura:" + larguraDaImagem + ", Nome:" + nomeDoArquivo + ", Intensidade:" + intensidade;

        }
    }
}