using System;
using System.Data.SQLite; // Asegúrate de tener la referencia a System.Data.SQLite
using System.Windows.Forms;

namespace GestionInventario
{
    public partial class Editar : Form
    {
        private Producto producto;
        private string connectionString = @"Data Source=C:\Users\fgfed\Desktop\GestionInventario\GestionInventario\db\listaProductos.db;Version=3;"; // Reemplaza con tu cadena de conexión

        // Constructor que recibe un objeto Producto
        public Editar(Producto producto)
        {
            InitializeComponent();
            this.producto = producto;
        }

        private void Editar_Load(object sender, EventArgs e)
        {
            // Hacer que el txtCodigo sea de solo lectura
            txtCodigo.ReadOnly = true; // Establecer el campo como solo lectura
        }

        private void MostrarDatos()
        {
            // Mostrar los datos del producto en los campos correspondientes
            txtCodigo.Text = producto.Codigo; // Código del producto
            txtNombre.Text = producto.Nombre; // Nombre del producto
            txtDescripcion.Text = producto.Descripcion; // Descripción del producto
            txtPrecio.Text = producto.Precio.ToString(); // Precio del producto
            txtStock.Text = producto.Stock.ToString(); // Stock del producto
            txtCategoria.Text = producto.Categoria; // Categoría del producto
        }

        private void LlenarComboCategorias()
        {
            // Conectar a la base de datos SQLite
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Consulta para obtener las categorías únicas
                string query = "SELECT DISTINCT Categoria FROM Productos"; // Asegúrate de que la tabla sea 'Productos'

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        // Limpiar el ComboBox antes de cargar las categorías
                        comboCategoria.Items.Clear();

                        // Agregar cada categoría al ComboBox
                        while (reader.Read())
                        {
                            string categoria = reader["Categoria"].ToString();
                            comboCategoria.Items.Add(categoria);
                        }
                    }
                }
            }
        }

        private void btnGrilla_Click(object sender, EventArgs e)
        {
            // Crear una instancia del formulario Nuevo
            Form1 volver = new Form1();

            // Ocultar el formulario actual
            this.Hide();

            // Mostrar el nuevo formulario
            volver.Show();
        }

        private void Editar_Shown(object sender, EventArgs e)
        {
            // Mostrar los datos del producto en las etiquetas o campos de texto correspondientes
            MostrarDatos();

            // Llenar el combo box con las categorías
            LlenarComboCategorias();

            // Hacer que el txtCodigo sea de solo lectura
            txtCodigo.ReadOnly = true; // Establecer el campo como solo lectura
        }

        private void comboCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Cambiar el txtCategoria al valor seleccionado en el ComboBox
            txtCategoria.Text = comboCategoria.SelectedItem.ToString();
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            ActualizarProducto();
            LimpiarCampos();
        }


        private void LimpiarCampos()
        {
            // Limpiar los campos de texto
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtPrecio.Clear();
            txtStock.Clear();
            txtCategoria.Clear();

            // Restablecer el ComboBox a su primer valor
            if (comboCategoria.Items.Count > 0)
            {
                comboCategoria.SelectedIndex = 0; // Seleccionar la primera categoría
            }
        }


        private void ActualizarProducto()
        {
            // Validar que los campos no estén vacíos
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtDescripcion.Text) ||
                string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                string.IsNullOrWhiteSpace(txtStock.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios. Por favor, complete todos los campos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Capturar los datos de los campos de texto
            string codigo = txtCodigo.Text;
            string nombre = txtNombre.Text;
            string descripcion = txtDescripcion.Text;
            string categoria = txtCategoria.Text; // Usar el valor de txtCategoria directamente
            decimal precio;
            int stock;

            // Validar que el precio sea un número válido
            if (!decimal.TryParse(txtPrecio.Text, out precio))
            {
                MessageBox.Show("El precio debe ser un número válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validar que el stock sea un número entero válido
            if (!int.TryParse(txtStock.Text, out stock))
            {
                MessageBox.Show("El stock debe ser un número entero válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Actualizar el producto en la base de datos
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Consulta de actualización
                string query = "UPDATE Productos SET Nombre = @Nombre, Descripcion = @Descripcion, Precio = @Precio, Stock = @Stock, Categoria = @Categoria WHERE Codigo = @Codigo";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    // Asignar parámetros
                    command.Parameters.AddWithValue("@Nombre", nombre);
                    command.Parameters.AddWithValue("@Descripcion", descripcion);
                    command.Parameters.AddWithValue("@Precio", precio);
                    command.Parameters.AddWithValue("@Stock", stock);
                    command.Parameters.AddWithValue("@Categoria", categoria);
                    command.Parameters.AddWithValue("@Codigo", codigo);

                    // Ejecutar el comando
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Producto actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el producto con el código especificado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
