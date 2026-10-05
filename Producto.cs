using System;

namespace ProyectoProductos
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public byte[] Imagen { get; set; } // Representa el campo LONGBLOB de MySQL
    }
}