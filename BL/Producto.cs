using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using FastMember;
using System.Globalization;

namespace BL
{

    public class Producto
    {
        private static string[] linea;
        public string Nombre { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Stock { get; set; }

        public string Descripcion { get; set; }
       
        //public DateTime FechaP { get; set }
        public string RutaArchivo { get; set; }
        public int IdProveedor { get; set; }
        public static string Carga(string file)
        {
            string conexionString = "Data Source=.;Initial Catalog=lbastidaCarga;User ID=sa;Password=pass@word1";

            var table = new DataTable();
            table.Columns.Add("Nombre", typeof(string));
            table.Columns.Add("PrecioUnitario", typeof(decimal));
            table.Columns.Add("Stock", typeof(int));
            table.Columns.Add("Descripcion", typeof(string));
            table.Columns.Add("FechaRegistro", typeof(DateTime));
            table.Columns.Add("IdProveedor", typeof(int));
            table.Columns.Add("Proveedor", typeof(string));
            table.Columns.Add("Numero", typeof(int));
            table.Columns.Add("Direccion", typeof(string));

            using (SqlConnection connection = new SqlConnection(conexionString))
            {
                connection.Open();

                using (SqlTransaction transacrion = connection.BeginTransaction())
                {
                    using (SqlBulkCopy copy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transacrion))
                    {

                        using (var reader = ObjectReader.Create(file))
                        {

                            copy.DestinationTableName = "Producto";
                            copy.BatchSize = 1000;
                           
                          
                        }



                    }
                }
            }




            string[] lineas = File.ReadAllLines(file);
            bool cambios = false;

            for (int i = 1; i < lineas.Count(); i++)
            {
                linea = Regex.Split(lineas[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                Console.WriteLine(linea.ToList());

                Producto producto = new Producto();

              
                for (int j = 1; j < linea.Length; j++)
                {
                   
                    if (linea[j].Contains("$"))
                    {
                        linea[j] = linea[j].Replace("$", "");
                        cambios = true;
                    }
                    if (linea[j].GetType() == typeof(string) && string.IsNullOrEmpty(linea[j]))
                    {
                        linea[j] = "-";
                    }


                }

                string FechaMes = linea[4];
                string fechaLimpia = LimpiarFecha(FechaMes);

                producto.Nombre = linea[0];
                producto.PrecioUnitario = Convert.ToDecimal(linea[1]);
                producto.Stock = Convert.ToInt32(linea[2]);
                producto.Descripcion = linea[3];
                fechaLimpia.ToString();
                producto.IdProveedor = Convert.ToInt32(linea[5]);
                lineas[i] = string.Join(",", linea);


                if (cambios)
                {
                    File.WriteAllLines(file, lineas);
                }
              
            }
            return linea.ToString();
        }

         static string LimpiarFecha(string FechaMes)
        {
            DateTime fecha = DateTime.ParseExact(FechaMes, "MM/dd/yy", CultureInfo.InvariantCulture);
           

            string fechaLimpia = fecha.ToString("dd/MM/yy");

            return fechaLimpia;
        }
    }
}
