﻿using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Web.Lib
{
    public class ProcessaUltrassom
    {
        List<string> filaDeProcessos;

        public ProcessaUltrassom()
        {
            filaDeProcessos = new List<string>();
        }

        public bool getElementoFila(string e)
        {
            return filaDeProcessos.Contains(e);
        }

        public void processa(string NomeArquivo, double[] g)
        {
            salvaVetor(NomeArquivo, g);
            filaDeProcessos.Add(NomeArquivo);
            //TODO: Chamar o método que executa a thread
        }

        public void salvaVetor(string NomeArquivo, double[] g)
        {
            try
            {
                var path = HttpContext.Current.Server.MapPath("~/Content/Signals/Sent/");
                StreamWriter vetorSinal = new StreamWriter(path + NomeArquivo);//(@".\" + NomeArquivo);

                foreach (var element in g)
                {
                    vetorSinal.WriteLine(element.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Can't write to file in:");
                Console.WriteLine(e.Message);
            }
        }

        // Apenas um teste, não é pra ser executado pela aplicação
        public void GeraImagem()
        {
            String im = ".\\SaidaProcessadaVetor.txt";
            StreamReader sr = new StreamReader(im);

            double[] temp = new double[3600];
            using (sr)
            {
                string line;
                int i = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    temp[i] = Double.Parse(line);
                    i++;
                }
            }

            new GeraBitmap().ToBitmap(temp, im.Replace(".\\", ""));
        }

        public void CGNE()
        {
            try
            {
                bool debug = false;
                String gFile;
                String hFile;
                int rows;
                int columns;

                if (debug)
                {
                    gFile = @"C:\Users\alexismayfire\Desktop\CSM30\Trabalho 2\Imagem-B\g-1.txt";
                    hFile = @"C:\Users\alexismayfire\Desktop\CSM30\Trabalho 2\Imagem-B\H-1.txt";
                    rows = 10;
                    columns = 6;
                }
                else
                {
                    gFile = @"C:\Users\alexismayfire\Desktop\CSM30\Trabalho 2\Imagem-A\g-1.txt";
                    hFile = @"C:\Users\alexismayfire\Desktop\CSM30\Trabalho 2\Imagem-A\H-1.txt";
                    rows = 50816;
                    columns = 3600;
                }

                var M = Matrix<double>.Build;
                var V = Vector<double>.Build;

                StreamReader sr = new StreamReader(gFile);

                var g = V.Dense(rows);

                try
                {
                    Console.WriteLine(DateTime.Now);

                    double[] temp = new double[rows];
                    using (sr)
                    {
                        string line;
                        int i = 0;
                        while ((line = sr.ReadLine()) != null)
                        {
                            temp[i] = Double.Parse(line);
                            i++;
                        }
                    }

                    g = V.Dense(temp);
                    Console.WriteLine("Arquivo g lido");
                    Console.WriteLine(DateTime.Now);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }

                sr.Dispose();
                sr = new StreamReader(hFile);

                try
                {
                    //var f = CGNECall(hFile, rows, columns, g);
                    Console.WriteLine(DateTime.Now);
                    Matrix<double> h;

                    using (StreamReader reader = new StreamReader(File.OpenRead(hFile)))
                    {
                        var blasMatrix = new BlasMatrix(reader, columns);
                        int i = 0;
                        double[,] matrixTemp = new double[rows, columns];
                        foreach (var element in blasMatrix.Records)
                        {
                            for (int j = 0; j < columns; j++)
                            {
                                matrixTemp[i, j] = element.getRow(0, j);
                            }
                            i++;
                        }

                        Console.WriteLine("Arquivo H lido");
                        Console.WriteLine(DateTime.Now);

                        h = M.Sparse(SparseCompressedRowMatrixStorage<double>.OfArray(matrixTemp));

                        //h = M.DenseOfArray(matrixTemp);
                        /*
                        * Usando Dense dobra o uso de memória (é feita uma cópia de matrixTemp)
                        * 
                        * h = M.Dense(DenseColumnMajorMatrixStorage<double>.OfArray(matrixTemp)) 
                        * 
                        */
                    }

                    sr.Dispose();
                    /*
                    * Aqui são as operações de inicialização
                    */

                    Vector<double> f = V.Dense(columns);
                    Vector<double> f_aux = V.Dense(columns);

                    //var r = g - h.Multiply(f);
                    var r = g;
                    Vector<double> r_aux;
                    var hT = h.Transpose();
                    var p = hT.Multiply(r);
                    Vector<double> p_aux;

                    for (int i = 0; i < 15; i++)
                    {
                        var r_T = r.ToRowMatrix();
                        var alfa_upper = r_T.Multiply(r);
                        var p_T = p.ToRowMatrix();
                        var alfa_down = p_T.Multiply(p);

                        var alfa = alfa_upper.PointwiseDivide(alfa_down);

                        //f = f_aux;
                        double alfa_scalar = alfa.Single();
                        f_aux += f.Add(p.Multiply(alfa_scalar));
                        f = f_aux;
                        //var temp = h.Multiply(alfa_scalar);
                        //r_aux = r.Subtract(temp.Multiply(p));
                        r_aux = r.Subtract(h.Multiply(alfa_scalar).Multiply(p));

                        var r_auxT = r_aux.ToRowMatrix();
                        var beta_upper = r_auxT.Multiply(r_aux);

                        var beta = beta_upper.PointwiseDivide(alfa_upper);

                        Console.WriteLine(DateTime.Now);
                        //Console.WriteLine(GC.GetTotalMemory(true) / 1024 / 1024);
                        //Console.Read();
                        Console.WriteLine("before matrix 2");



                        double beta_scalar = beta.Single();
                        p_aux = hT.Multiply(r_aux);
                        var pplus = p_aux.Add(p.Multiply(beta_scalar));
                        p = pplus;
                        r = r_aux;

                        Console.WriteLine(DateTime.Now);
                        Console.WriteLine($"iteração {i}");

                    }

                    //COMENTAR ESSA LINHA PARA OTIMIZAR
                    salvaVetor("SaidaProcessadaVetor.txt", f_aux.ToArray());

                    new GeraBitmap().ToBitmap(f_aux.ToArray(), filaDeProcessos.First());

                    StreamWriter arquivo_gerado = new StreamWriter(@"C:\Users\alexismayfire\Desktop\CSM30\Trabalho 2\Imagem-B\arquivo_f1.txt");

                    foreach (var element in f_aux)
                    {
                        arquivo_gerado.WriteLine(element.ToString());
                    }
                    Console.Read();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }

                sr.Dispose();

                Console.Read();
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }


        /*
        public static Vector<double> CGNECall(String hFile, int rows, int columns, Vector<double> g)
        {
            var M = Matrix<double>.Build;
            var V = Vector<double>.Build;
        }
        */

        public static double[,] GenerateMatrix(String hFile, int rows, int columns)
        {
            Console.WriteLine(DateTime.Now);
            using (StreamReader reader = new StreamReader(File.OpenRead(hFile)))
            {
                var blasMatrix = new BlasMatrix(reader, columns);
                int i = 0;
                double[,] matrixTemp = new double[rows, columns];
                foreach (var element in blasMatrix.Records)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        matrixTemp[i, j] = element.getRow(0, j);
                    }
                    i++;
                }

                Console.WriteLine("Arquivo H lido");
                Console.WriteLine(DateTime.Now);

                return matrixTemp;
            }
        }
    }

    public class BlasMatrix
    {
        private readonly StreamReader reader;
        private int columns;

        public BlasMatrix(StreamReader reader, int columns)
        {
            this.reader = reader;
            this.columns = columns;
        }

        public IEnumerable<MatrixLine> Records
        {
            get
            {
                while (!this.reader.EndOfStream)
                {
                    yield return new MatrixLine(this.reader.ReadLine(), this.columns);
                }
            }
        }
    }

    public class MatrixLine
    {
        private double[,] row;

        public MatrixLine(String line, int columns)
        {
            this.row = new double[1, columns];
            int index = 0;
            int last_index = -1;
            for (int j = 0; j < columns; j++)
            {
                index = line.IndexOf(",", last_index + 1);
                if (index == -1)
                {
                    this.row[0, j] = Double.Parse(line.Substring(last_index + 1));
                }
                else
                {
                    int n = index - last_index - 1;
                    this.row[0, j] = Double.Parse(line.Substring(last_index + 1, n));
                }
                last_index = index;
            }
        }

        public double getRow(int i, int j)
        {
            return this.row[i, j];
        }
    }
}