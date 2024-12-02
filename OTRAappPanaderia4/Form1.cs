using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace OTRAappPanaderia2
{
    public partial class Form1 : Form
    {
        private List<Producto> productos = new List<Producto>();
        private const string archivo = "productos.bin";

        public Form1()
        {
            InitializeComponent();
            cargarDatos();
            comboBoxTipo.Items.Add("Producto Final");
            comboBoxTipo.Items.Add("Materia Prima");
            ActualizarTotales();
        }

        private bool ValidarCampos()
        {
            // Verificar que todos los campos están llenos
            if (string.IsNullOrWhiteSpace(textBoxCodigo.Text) ||
                string.IsNullOrWhiteSpace(textBoxNombre.Text) ||
                string.IsNullOrWhiteSpace(textBoxPrecio.Text) ||
                string.IsNullOrWhiteSpace(textBoxCantidad.Text) ||
                comboBoxTipo.SelectedItem == null)
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Verificar que el precio sea un double válido y en el formato correcto
            if (!double.TryParse(textBoxPrecio.Text, out double precio) || precio < 0)
            {
                MessageBox.Show("Por favor, ingrese un valor numérico válido y positivo para el precio (formato: 99999.99).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Verificar que la cantidad sea un entero válido
            if (!int.TryParse(textBoxCantidad.Text, out int cantidad) || cantidad < 0)
            {
                MessageBox.Show("Por favor, ingrese un valor numérico válido y positivo para la cantidad.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validar que el código del producto no exista
           /* if (CodigoProductoExiste(textBoxCodigo.Text))
            {
                MessageBox.Show("Ya existe un producto con el mismo código. Por favor, ingrese un código diferente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }*/

            return true;
        }

        private bool CodigoProductoExiste(string codigo)
        {
            // Verifica si ya existe un producto con el mismo código
            return productos.Exists(p => p.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase));
        }

        private void cargarDatos()
        {
            if (File.Exists(archivo))
            {
                try
                {
                    using (FileStream fs = new FileStream(archivo, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        productos = (List<Producto>)formatter.Deserialize(fs);
                    }
                }
                catch (SerializationException ex)
                {
                    MessageBox.Show($"Error de deserialización: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Error de entrada/salida: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            actualizarDataGridView();
        }

        private void actualizarDataGridView()
        {
            dataGridViewProductos.DataSource = null; // Desvincular la fuente de datos
            dataGridViewProductos.DataSource = productos; // Volver a vincular
            dataGridViewProductos.Columns["FechaIngreso"].HeaderText = "Fecha de Ingreso"; // Cambiar el encabezado
            dataGridViewProductos.Columns["FechaIngreso"].DefaultCellStyle.Format = "g"; // Formato de fecha y hora
        }

        private void buttonAgregar_Click(object sender, EventArgs e)
        {
            //if (ValidarCampos())
            //{
            //    var producto = ObtenerProductoDesdeControles();
            //    productos.Add(producto);
                actualizarDataGridView();
                //guardarDatos();
                LimpiarCajasDeTexto(); // Limpiar las cajas de texto después de agregar
                ActualizarTotales(); 
        }

        private void buttonEditar_Click(object sender, EventArgs e)
        {
            if (dataGridViewProductos.Rows.Count > 0 && dataGridViewProductos.SelectedRows.Count > 0 && ValidarCampos())
            {
                var selectedRow = dataGridViewProductos.SelectedRows[0];
                var producto = (Producto)selectedRow.DataBoundItem;

                producto.Nombre = textBoxNombre.Text;
                producto.Tipo = comboBoxTipo.SelectedItem.ToString();
                producto.Precio = double.Parse(textBoxPrecio.Text);
                producto.Cantidad = int.Parse(textBoxCantidad.Text);
                guardarDatos();
                actualizarDataGridView();
                ActualizarTotales();
                
               
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un producto para editar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private Producto ObtenerProductoDesdeControles()
        {
            return new Producto
            {
                Codigo = textBoxCodigo.Text,
                Nombre = textBoxNombre.Text,
                Tipo = comboBoxTipo.SelectedItem.ToString(),
                Precio = double.Parse(textBoxPrecio.Text),
                Cantidad = int.Parse(textBoxCantidad.Text),
                FechaIngreso = DateTime.Now
            };
        }

        private void buttonEliminar_Click(object sender, EventArgs e)
        {
            if (dataGridViewProductos.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridViewProductos.SelectedRows[0];
                var producto = (Producto)selectedRow.DataBoundItem;
                productos.Remove(producto);
                actualizarDataGridView();
                ActualizarTotales();
                guardarDatos();
                
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un producto para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void ActualizarTotales()
        {
            // Contar total de productos
            int totalProductos = productos.Count;

            // Contar total de insumos clasificados como "Materia Prima" y "Producto Final"
            int totalInsumosMP = productos.Count(p => p.Tipo.Equals("Materia Prima", StringComparison.OrdinalIgnoreCase));
            int totalInsumosPF = productos.Count(p => p.Tipo.Equals("Producto Final", StringComparison.OrdinalIgnoreCase));

            // Sumar precios de productos
            double precioTotalProductos = productos.Sum(p => p.Precio * p.Cantidad); // Multiplicamos por la cantidad
            double precioTotalInsumosMP = productos.Where(p => p.Tipo.Equals("Materia Prima", StringComparison.OrdinalIgnoreCase)).Sum(p => p.Precio * p.Cantidad); // Multiplicamos por la cantidad
            double precioTotalInsumosPF = productos.Where(p => p.Tipo.Equals("Producto Final", StringComparison.OrdinalIgnoreCase)).Sum(p => p.Precio * p.Cantidad); // Multiplicamos por la cantidad

            // Asignar los valores a los text boxes
            textBoxTotalProd.Text = totalProductos.ToString();
            textBoxTotalInsumMP.Text = totalInsumosMP.ToString();
            textBoxInsumoPF.Text = totalInsumosPF.ToString();
            textBoxPrecioTOTALPROD.Text = precioTotalProductos.ToString("C");
            textBoxTotalPrecioMP.Text = precioTotalInsumosMP.ToString("C");
            textBoxTotalPrecioPF.Text = precioTotalInsumosPF.ToString("C");
        }


        private void guardarDatos()
        {
            try
            {
                using (FileStream fs = new FileStream(archivo, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, productos);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewProductos_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewProductos.Rows.Count)
            {
                var producto = (Producto)dataGridViewProductos.Rows[e.RowIndex].DataBoundItem;
                CargarDatosEnControles(producto);
            }
        }

        private void dataGridViewProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewProductos.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridViewProductos.SelectedRows[0];
                var producto = (Producto)selectedRow.DataBoundItem;
                CargarDatosEnControles(producto);
            }
        }

        private void CargarDatosEnControles(Producto producto)
        {
            if (producto != null)
            {
                textBoxCodigo.Text = producto.Codigo;
                textBoxNombre.Text = producto.Nombre;
                comboBoxTipo.SelectedItem = producto.Tipo;
                textBoxPrecio.Text = producto.Precio.ToString("F2");
                textBoxCantidad.Text = producto.Cantidad.ToString();
            }
        }

        private void buttonGuardar_Click(object sender, EventArgs e)
        {
            if (ValidarCampos())
            {
                var producto = ObtenerProductoDesdeControles();
                productos.Add(producto);
                actualizarDataGridView();
                guardarDatos();
                LimpiarCajasDeTexto();
            }
        }

        private void dataGridViewProductos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Evitar que se procese el Enter
            }
        }

        private void LimpiarCajasDeTexto()
        {
            textBoxCodigo.Clear();
            textBoxNombre.Clear();
            textBoxPrecio.Clear();
            textBoxCantidad.Clear();
            comboBoxTipo.SelectedItem = null; // Restablecer el comboBox
        }

       
    }
}