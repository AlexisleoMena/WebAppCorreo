using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using WebAppCorreo.Models;
using System.Data;
using System.Data.SqlClient;

namespace WebAppCorreo.Datos
{
    public class DBUsuario
    {
        private static string CadenaSQL =
            "Server=DESKTOP-L2BFH14\\SQLEXPRESS; DataBase=DBPrueba; Trusted_Connection=True; TrustServerCertificate=True;";

        public static bool Registrar(UsuarioDTO usuario)
        {
            bool respuesta = false;
            try
            {
                // Creamos una nueva conexión utilizando la cadena de conexión establecida en "CadenaSQL"
                using (SqlConnection oconexion = new SqlConnection(CadenaSQL))
                {
                    // Construimos la consulta SQL para insertar un nuevo registro en la tabla "Usuario"
                    string query =
                        "insert into Usuario(Nombre,Correo,Clave,Restablecer,Confirmado,Token)";
                    query += " values(@nombre,@correo,@clave,@restablecer,@confirmado,@token)";

                    // Creamos un objeto SqlCommand y le pasamos la consulta SQL y la conexión
                    SqlCommand cmd = new SqlCommand(query, oconexion);

                    // Agregamos los parámetros necesarios a la consulta utilizando el objeto SqlCommand
                    cmd.Parameters.AddWithValue("@nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@correo", usuario.Correo);
                    cmd.Parameters.AddWithValue("@clave", usuario.Clave);
                    cmd.Parameters.AddWithValue("@restablecer", usuario.Restablecer);
                    cmd.Parameters.AddWithValue("@confirmado", usuario.Confirmado);
                    cmd.Parameters.AddWithValue("@token", usuario.Token);

                    // Indicamos que estamos utilizando una consulta SQL de tipo texto
                    cmd.CommandType = CommandType.Text;

                    // Abrimos la conexión a la base de datos
                    oconexion.Open();

                    // Ejecutamos la consulta utilizando el método ExecuteNonQuery, que devuelve el número de filas afectadas
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    // Verificamos si el número de filas afectadas es mayor que cero, lo que indica que el registro se insertó correctamente
                    if (filasAfectadas > 0)
                        respuesta = true; // Actualizamos la variable "respuesta" a "true"
                }

                // Devolvemos el valor de la variable "respuesta", que indica si el registro se realizó exitosamente o no
                return respuesta;
            }
            catch (Exception ex)
            {
                // Si ocurre alguna excepción durante la ejecución del código, la lanzamos para que pueda ser manejada en otra parte del programa
                throw ex;
            }
        }

        // Método para validar el inicio de sesión de un usuario en la base de datos
        public static UsuarioDTO Validar(string correo, string clave)
        {
            UsuarioDTO usuario = null;
            try
            {
                // Creamos una nueva conexión utilizando la cadena de conexión establecida en "CadenaSQL"
                using (SqlConnection oconexion = new SqlConnection(CadenaSQL))
                {
                    // Construimos la consulta SQL para buscar un usuario en la tabla "Usuario" que coincida con el correo y clave proporcionados
                    string query = "select Nombre,Restablecer,Confirmado from Usuario";
                    query += " where Correo=@correo and Clave = @clave";

                    // Creamos un objeto SqlCommand y le pasamos la consulta SQL y la conexión
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@clave", clave);
                    cmd.CommandType = CommandType.Text;

                    // Abrimos la conexión a la base de datos
                    oconexion.Open();

                    // Ejecutamos la consulta utilizando el método ExecuteReader, que devuelve un objeto SqlDataReader para leer los resultados de la consulta
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        // Verificamos si la consulta devolvió un resultado (un usuario válido)
                        if (dr.Read())
                        {
                            // Si se encontró un resultado, creamos un objeto UsuarioDTO con los datos obtenidos de la consulta
                            usuario = new UsuarioDTO()
                            {
                                Nombre = dr["Nombre"].ToString(),
                                Restablecer = (bool)dr["Restablecer"],
                                Confirmado = (bool)dr["Confirmado"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Si ocurre alguna excepción durante la ejecución del código, la lanzamos para que pueda ser manejada en otra parte del programa
                throw ex;
            }

            // Devolvemos el objeto UsuarioDTO encontrado en la base de datos (puede ser null si no se encontró ningún usuario)
            return usuario;
        }

        // Método para obtener un usuario por su correo electrónico desde la base de datos
        public static UsuarioDTO Obtener(string correo)
        {
            UsuarioDTO usuario = null;
            try
            {
                // Creamos una nueva conexión utilizando la cadena de conexión establecida en "CadenaSQL"
                using (SqlConnection oconexion = new SqlConnection(CadenaSQL))
                {
                    // Construimos la consulta SQL para buscar un usuario en la tabla "Usuario" que coincida con el correo proporcionado
                    string query = "select Nombre,Clave,Restablecer,Confirmado,Token from Usuario";
                    query += " where Correo=@correo";

                    // Creamos un objeto SqlCommand y le pasamos la consulta SQL y la conexión
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.CommandType = CommandType.Text;

                    // Abrimos la conexión a la base de datos
                    oconexion.Open();

                    // Ejecutamos la consulta utilizando el método ExecuteReader, que devuelve un objeto SqlDataReader para leer los resultados de la consulta
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        // Verificamos si la consulta devolvió un resultado (un usuario válido)
                        if (dr.Read())
                        {
                            // Si se encontró un resultado, creamos un objeto UsuarioDTO con los datos obtenidos de la consulta
                            usuario = new UsuarioDTO()
                            {
                                Nombre = dr["Nombre"].ToString(),
                                Clave = dr["Clave"].ToString(),
                                Restablecer = (bool)dr["Restablecer"],
                                Confirmado = (bool)dr["Confirmado"],
                                Token = dr["Token"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Si ocurre alguna excepción durante la ejecución del código, la lanzamos para que pueda ser manejada en otra parte del programa
                throw ex;
            }

            // Devolvemos el objeto UsuarioDTO encontrado en la base de datos (puede ser null si no se encontró ningún usuario)
            return usuario;
        }

        // Método para actualizar los campos Restablecer y Clave de un usuario en la base de datos
        // mediante el token proporcionado como identificador
        public static bool RestablecerActualizar(int restablecer, string clave, string token)
        {
            bool respuesta = false;
            try
            {
                // Creamos una nueva conexión utilizando la cadena de conexión establecida en "CadenaSQL"
                using (SqlConnection oconexion = new SqlConnection(CadenaSQL))
                {
                    // Construimos la consulta SQL para actualizar los campos Restablecer y Clave del usuario en la tabla "Usuario"
                    // que coincida con el token proporcionado
                    string query =
                        @"update Usuario set "
                        + "Restablecer= @restablecer, "
                        + "Clave=@clave "
                        + "where Token=@token";

                    // Creamos un objeto SqlCommand y le pasamos la consulta SQL y la conexión
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@restablecer", restablecer);
                    cmd.Parameters.AddWithValue("@clave", clave);
                    cmd.Parameters.AddWithValue("@token", token);
                    cmd.CommandType = CommandType.Text;

                    // Abrimos la conexión a la base de datos
                    oconexion.Open();

                    // Ejecutamos la consulta utilizando el método ExecuteNonQuery, que devuelve el número de filas afectadas por la actualización
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    // Verificamos si el número de filas afectadas es mayor que cero, lo que indica que la actualización se realizó correctamente
                    if (filasAfectadas > 0)
                        respuesta = true; // Actualizamos la variable "respuesta" a "true"
                }

                // Devolvemos el valor de la variable "respuesta", que indica si la actualización se realizó exitosamente o no
                return respuesta;
            }
            catch (Exception ex)
            {
                // Si ocurre alguna excepción durante la ejecución del código, la lanzamos para que pueda ser manejada en otra parte del programa
                throw ex;
            }
        }

        // Método para confirmar el registro de un usuario en la base de datos
        // mediante el token proporcionado como identificador
        public static bool Confirmar(string token)
        {
            bool respuesta = false;
            try
            {
                // Creamos una nueva conexión utilizando la cadena de conexión establecida en "CadenaSQL"
                using (SqlConnection oconexion = new SqlConnection(CadenaSQL))
                {
                    // Construimos la consulta SQL para actualizar el campo Confirmado del usuario en la tabla "Usuario"
                    // que coincida con el token proporcionado
                    string query = @"update Usuario set " + "Confirmado= 1 " + "where Token=@token";

                    // Creamos un objeto SqlCommand y le pasamos la consulta SQL y la conexión
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@token", token);
                    cmd.CommandType = CommandType.Text;

                    // Abrimos la conexión a la base de datos
                    oconexion.Open();

                    // Ejecutamos la consulta utilizando el método ExecuteNonQuery, que devuelve el número de filas afectadas por la actualización
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    // Verificamos si el número de filas afectadas es mayor que cero, lo que indica que la actualización se realizó correctamente
                    if (filasAfectadas > 0)
                        respuesta = true; // Actualizamos la variable "respuesta" a "true"
                }

                // Devolvemos el valor de la variable "respuesta", que indica si la actualización se realizó exitosamente o no
                return respuesta;
            }
            catch (Exception ex)
            {
                // Si ocurre alguna excepción durante la ejecución del código, la lanzamos para que pueda ser manejada en otra parte del programa
                throw ex;
            }
        }
    }
}
