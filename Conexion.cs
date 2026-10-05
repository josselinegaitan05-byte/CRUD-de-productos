using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace ProyectoProductos
{
    public static class Conexion
    {
        // Ajusta Uid y Pwd con los datos de TU instalación de MySQL
        private static string cadenaConexion =
            "Server=localhost;Database=productosdb;Uid=root;Pwd=202202;";

        public static MySqlConnection ObtenerConexion()
        {
            try
            {
                MySqlConnection conexion = new MySqlConnection(cadenaConexion);
                conexion.Open();
                return conexion;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error al conectar: " + ex.Message);
                return null;
            }
        }

        public static List<Producto> GetProductos(string filtro)
        {
            List<Producto> listaProductos = new List<Producto>();
            string query = "SELECT id, nombre, precio, cantidad, imagen FROM productos";

            if (!string.IsNullOrEmpty(filtro))
            {
                query += " WHERE id LIKE @filtro OR nombre LIKE @filtro " +
                         " OR precio LIKE @filtro OR cantidad LIKE @filtro";
            }

            using (MySqlConnection conn = ObtenerConexion())
            {
                if (conn == null) return listaProductos;

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(filtro))
                    {
                        cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
                    }

                    using (MySqlDataReader mReader = cmd.ExecuteReader())
                    {
                        while (mReader.Read())
                        {
                            Producto prod = new Producto();
                            prod.Id = Convert.ToInt32(mReader["id"]);
                            prod.Nombre = mReader["nombre"].ToString();
                            prod.Precio = Convert.ToDecimal(mReader["precio"]);
                            prod.Cantidad = Convert.ToInt32(mReader["cantidad"]);
                            prod.Imagen = mReader["imagen"] != DBNull.Value ? (byte[])mReader["imagen"] : null;
                            listaProductos.Add(prod);
                        }
                    }
                }
            }
            return listaProductos;
        }

        // INSERT con parámetros a partir de un Dictionary<string, object>
        public static bool InsertSeguro(string tbName, Dictionary<string, object> data)
        {
            var columns = string.Join(", ", data.Keys);
            var placeholders = "@" + string.Join(", @", data.Keys);
            string sql = $"INSERT INTO {tbName} ({columns}) VALUES ({placeholders})";

            try
            {
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion == null) return false;
                    using (MySqlCommand stmt = new MySqlCommand(sql, conexion))
                    {
                        foreach (var kvp in data)
                            stmt.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);

                        stmt.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error en INSERT: " + ex.Message);
                return false;
            }
        }

        // UPDATE con parámetros: SET col1=@col1, col2=@col2 ... WHERE id=@id
        public static bool UpdateSeguro(string tbName, Dictionary<string, object> data, int id)
        {
            var sets = new List<string>();
            foreach (var key in data.Keys)
                sets.Add(key + " = @" + key);

            string sql = $"UPDATE {tbName} SET {string.Join(", ", sets)} WHERE id = @idWhere";

            try
            {
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion == null) return false;
                    using (MySqlCommand stmt = new MySqlCommand(sql, conexion))
                    {
                        foreach (var kvp in data)
                            stmt.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);

                        stmt.Parameters.AddWithValue("@idWhere", id);
                        return stmt.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error en UPDATE: " + ex.Message);
                return false;
            }
        }

        // DELETE por id
        public static bool DeleteSeguro(string tbName, int id)
        {
            string sql = $"DELETE FROM {tbName} WHERE id = @id";

            try
            {
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion == null) return false;
                    using (MySqlCommand stmt = new MySqlCommand(sql, conexion))
                    {
                        stmt.Parameters.AddWithValue("@id", id);
                        return stmt.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error en DELETE: " + ex.Message);
                return false;
            }
        }
    }
}
