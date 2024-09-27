using System;
using System.Data;
using System.Data.SQLite; // Asegúrate de tener la referencia a System.Data.SQLite
using System.Windows.Forms;

namespace GestionInventario
{
    public partial class Nuevo : Form
    {
        private string connectionString = @"Data Source=C:\Users\fgfed\Desktop\GestionInventario\GestionInventario\db\listaProductos.db;Version=3;";

        public Nuevo()
        {
            InitializeComponent();
        }

        private void Nuevo_Load(object sender, EventArgs e)
        {
           // CargarCategorias();
        }

        private void CargarCategorias()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("SELECT DISTINCT Categoria FROM Productos", connection);
                var reader = command.ExecuteReader();

                // Limpiar el ComboBox antes de cargar nuevas categorías
                comboCategoria.Items.Clear();

                while (reader.Read())
                {
                    // Agregar cada categoría al ComboBox
                    comboCategoria.Items.Add(reader["Categoria"].ToString());
                }
            }
        }

        private bool ValidarCampos()
        {
            // Verificar que ninguno de los campos esté vacío
            return !string.IsNullOrWhiteSpace(txtCodigo.Text) &&
                   !string.IsNullOrWhiteSpace(txtNombre.Text) &&
                   !string.IsNullOrWhiteSpace(txtDescripcion.Text) &&
                   !string.IsNullOrWhiteSpace(txtPrecio.Text) &&
                   !string.IsNullOrWhiteSpace(txtStock.Text) &&
                   comboCategoria.SelectedItem != null;
        }

        private void CrearProducto()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Comando para insertar el nuevo producto
                var command = new SQLiteCommand("INSERT INTO Productos (Codigo, Nombre, Descripcion, Precio, Stock, Categoria) VALUES (@Codigo, @Nombre, @Descripcion, @Precio, @Stock, @Categoria)", connection);

                // Asignar los valores de los parámetros
                command.Parameters.AddWithValue("@Codigo", txtCodigo.Text);
                command.Parameters.AddWithValue("@Nombre", txtNombre.Text);
                command.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text);
                command.Parameters.AddWithValue("@Precio", decimal.Parse(txtPrecio.Text));
                command.Parameters.AddWithValue("@Stock", int.Parse(txtStock.Text));
                command.Parameters.AddWithValue("@Categoria", comboCategoria.SelectedItem.ToString());

                // Ejecutar el comando
                command.ExecuteNonQuery();

                MessageBox.Show("Producto creado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar los campos después de crear el producto
                LimpiarCampos();
            }
        }

        private void LimpiarCampos()
        {
            txtCodigo.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtPrecio.Clear();
            txtStock.Clear();
            comboCategoria.SelectedIndex = -1; // Restablecer el ComboBox
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (ValidarCampos())
            {
                CrearProducto();
            }
            else
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Nuevo_Shown(object sender, EventArgs e)
        {
            CargarCategorias();
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            // Crear una instancia del formulario Nuevo
            Form1 volver = new Form1();

            // Ocultar el formulario actual
            this.Hide();

            // Mostrar el nuevo formulario
            volver.Show();
        }
    }
}
