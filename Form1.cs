using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace ProyectoProductos
{
    public partial class Form1 : Form
    {
        private List<Producto> listaProductos;
        private Dictionary<string, object> myProducto = new Dictionary<string, object>();

        private int idSeleccionado = 0;          // 0 = ningún producto seleccionado
        private Image imagenPorDefecto;          // imagen "agregar foto" del diseñador
        private bool hayImagen = false;          // true si el usuario eligió/cargó una imagen real

        public Form1()
        {
            InitializeComponent();
            listaProductos = new List<Producto>();
            imagenPorDefecto = pictureBox1.Image;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cargarProductos();
        }

        // Carga los registros en el DataGridView manejando imágenes binarias
        private void cargarProductos(string filtro = "")
        {
            dgvProductos.Rows.Clear();
            dgvProductos.Refresh();
            listaProductos = Conexion.GetProductos(filtro);

            foreach (var prod in listaProductos)
            {
                Image img = null;
                if (prod.Imagen != null && prod.Imagen.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(prod.Imagen))
                    {
                        using (Bitmap bmp = new Bitmap(ms))
                        {
                            img = new Bitmap(bmp); // Clona la imagen para desvincularla del MemoryStream
                        }
                    }
                }
                dgvProductos.Rows.Add(prod.Id, prod.Nombre, prod.Precio, prod.Cantidad, img);
            }
        }

        // Evento de búsqueda dinámica
        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            cargarProductos(txtBusqueda.Text.Trim());
        }

        // Selección de imagen mediante OpenFileDialog
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Seleccionar imagen del producto";
                openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Se copia a memoria para no dejar el archivo bloqueado en disco
                    using (Image original = Image.FromFile(openFileDialog.FileName))
                    {
                        pictureBox1.Image = new Bitmap(original);
                    }
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    hayImagen = true;
                }
            }
        }

        // Guardar registro
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!datosCorrectos())
            {
                return;
            }

            CargarDatosProductos();

            if (Conexion.InsertSeguro("productos", myProducto))
            {
                MessageBox.Show("Se ha guardado satisfactoriamente el registro.");
                limpiarCampos();
                cargarProductos();
            }
            else
            {
                MessageBox.Show("No se pudo guardar el registro. Revise la conexión a MySQL.");
            }
        }

        // Modificar registro seleccionado
        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un producto de la tabla para modificarlo.");
                return;
            }

            if (!datosCorrectos())
            {
                return;
            }

            CargarDatosProductos();

            if (Conexion.UpdateSeguro("productos", myProducto, idSeleccionado))
            {
                MessageBox.Show("Se ha modificado satisfactoriamente el registro.");
                limpiarCampos();
                cargarProductos(txtBusqueda.Text.Trim());
            }
            else
            {
                MessageBox.Show("No se pudo modificar el registro.");
            }
        }

        // Eliminar registro seleccionado
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un producto de la tabla para eliminarlo.");
                return;
            }

            DialogResult r = MessageBox.Show("¿Está seguro de eliminar este producto?",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r != DialogResult.Yes) return;

            if (Conexion.DeleteSeguro("productos", idSeleccionado))
            {
                MessageBox.Show("Se ha eliminado el registro.");
                limpiarCampos();
                cargarProductos(txtBusqueda.Text.Trim());
            }
            else
            {
                MessageBox.Show("No se pudo eliminar el registro.");
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiarCampos();
        }

        private void limpiarCampos()
        {
            txtNombre.Clear();
            txtPrecio.Clear();
            txtCantidad.Clear();
            pictureBox1.Image = imagenPorDefecto;
            hayImagen = false;
            idSeleccionado = 0;
            dgvProductos.ClearSelection();
            txtNombre.Focus();
        }

        // Al hacer clic en una fila, se cargan sus datos en los campos para modificar/eliminar
        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];
            if (fila.Cells[0].Value == null) return;

            idSeleccionado = Convert.ToInt32(fila.Cells[0].Value);
            txtNombre.Text = Convert.ToString(fila.Cells[1].Value);
            txtPrecio.Text = Convert.ToString(fila.Cells[2].Value);
            txtCantidad.Text = Convert.ToString(fila.Cells[3].Value);

            Image img = fila.Cells[4].Value as Image;
            if (img != null)
            {
                pictureBox1.Image = new Bitmap(img);
                hayImagen = true;
            }
            else
            {
                pictureBox1.Image = imagenPorDefecto;
                hayImagen = false;
            }
        }

        private void CargarDatosProductos()
        {
            myProducto.Clear();
            myProducto["nombre"] = txtNombre.Text.Trim();
            myProducto["precio"] = decimal.Parse(txtPrecio.Text.Trim());
            myProducto["cantidad"] = int.Parse(txtCantidad.Text.Trim());
            // La columna imagen es NOT NULL: si no hay foto se guarda un arreglo vacío
            myProducto["imagen"] = hayImagen ? ImageToByteArray(pictureBox1.Image) : new byte[0];
        }

        // Convierte un objeto Image a un arreglo de bytes (byte[]) usando MemoryStream
        private byte[] ImageToByteArray(Image image)
        {
            if (image == null)
                return new byte[0];

            using (MemoryStream mMemoryStream = new MemoryStream())
            {
                // Una imagen clonada en memoria (MemoryBmp) no se puede guardar con RawFormat: se usa PNG
                ImageFormat formato = ImageFormat.Png;
                if (image.RawFormat.Equals(ImageFormat.Jpeg) ||
                    image.RawFormat.Equals(ImageFormat.Png) ||
                    image.RawFormat.Equals(ImageFormat.Bmp))
                {
                    formato = image.RawFormat;
                }
                image.Save(mMemoryStream, formato);
                return mMemoryStream.ToArray();
            }
        }

        // Validación de campos
        private bool datosCorrectos()
        {
            if (string.IsNullOrEmpty(txtNombre.Text.Trim()))
            {
                MessageBox.Show("Ingrese el Nombre del Producto");
                return false;
            }

            if (string.IsNullOrEmpty(txtPrecio.Text.Trim()))
            {
                MessageBox.Show("Ingrese el Precio");
                return false;
            }

            if (string.IsNullOrEmpty(txtCantidad.Text.Trim()))
            {
                MessageBox.Show("Ingrese la Cantidad");
                return false;
            }

            if (!decimal.TryParse(txtPrecio.Text.Trim(), out decimal precio))
            {
                MessageBox.Show("Ingrese un Precio correcto");
                return false;
            }

            if (!int.TryParse(txtCantidad.Text.Trim(), out int cantidad))
            {
                MessageBox.Show("Ingrese una cantidad correcta");
                return false;
            }

            return true;
        }

    }
}