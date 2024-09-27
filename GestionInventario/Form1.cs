using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite; // Asegúrate de tener la referencia a System.Data.SQLite
using System.Linq;
using System.Windows.Forms;

namespace GestionInventario
{
    public partial class Form1 : Form
    {
        private string connectionString = @"Data Source=C:\Users\fgfed\Desktop\GestionInventario\GestionInventario\db\listaProductos.db;Version=3;";
        private List<Producto> productos; // Lista para almacenar los productos

        public Form1()
        {
            InitializeComponent();
            productos = new List<Producto>(); // Inicializa la lista de productos
            // Configurar el evento de clic para el DataGridView solo una vez
            dataGridView1.CellContentClick += DataGridView1_CellContentClick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CargarProductos(); // Llama a cargar productos una vez al cargar el formulario
        }

        private void CargarProductos()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("SELECT Codigo, Nombre, Descripcion, Precio, Stock, Categoria FROM Productos", connection);
                var adapter = new SQLiteDataAdapter(command);
                var table = new DataTable();
                adapter.Fill(table);

                // Limpiar el DataGridView y agregar columnas
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();

                dataGridView1.Columns.Add("Codigo", "Código");
                dataGridView1.Columns.Add("Nombre", "Nombre");
                dataGridView1.Columns.Add("Descripcion", "Descripción");
                dataGridView1.Columns.Add("Precio", "Precio");
                dataGridView1.Columns.Add("Stock", "Stock");
                dataGridView1.Columns.Add("Categoria", "Categoría");
                dataGridView1.Columns.Add("Editar", "Editar");
                dataGridView1.Columns.Add("Eliminar", "Eliminar");

                // Limpiar la lista antes de cargar nuevos productos
                productos.Clear();

                foreach (DataRow row in table.Rows)
                {
                    // Crear un objeto Producto y agregarlo a la lista
                    var producto = new Producto(
                        row["Codigo"].ToString(),
                        row["Nombre"].ToString(),
                        row["Descripcion"].ToString(),
                        Convert.ToDecimal(row["Precio"]),
                        Convert.ToInt32(row["Stock"]),
                        row["Categoria"].ToString());

                    productos.Add(producto); // Agrega el producto a la lista

                    int rowIndex = dataGridView1.Rows.Add(row.ItemArray[0], row.ItemArray[1], row.ItemArray[2], row.ItemArray[3], row.ItemArray[4], row.ItemArray[5]);

                    // Crear y agregar botón de Editar
                    DataGridViewButtonCell editButton = new DataGridViewButtonCell();
                    editButton.Value = "Editar";
                    dataGridView1.Rows[rowIndex].Cells["Editar"] = editButton;

                    // Crear y agregar botón de Eliminar
                    DataGridViewButtonCell deleteButton = new DataGridViewButtonCell();
                    deleteButton.Value = "Eliminar";
                    dataGridView1.Rows[rowIndex].Cells["Eliminar"] = deleteButton;
                }
            }

            // Actualizar total de productos
            ActualizarTotalProductos(productos);
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == dataGridView1.Columns["Editar"].Index)
                {
                    MessageBox.Show("Se editó el producto: " + dataGridView1.Rows[e.RowIndex].Cells["Codigo"].Value);
                }
                else if (e.ColumnIndex == dataGridView1.Columns["Eliminar"].Index)
                {
                    // Confirmar la eliminación
                    var result = MessageBox.Show("¿Está seguro de que desea eliminar el producto?", "Confirmar eliminación", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        // Llamar al método para eliminar el producto
                        string codigoProducto = dataGridView1.Rows[e.RowIndex].Cells["Codigo"].Value.ToString();
                        EliminarProducto(codigoProducto);
                    }
                }
            }
        }

        private void BuscarProductos(string searchTerm)
        {
            // Limpiar el DataGridView antes de mostrar los resultados
            dataGridView1.Rows.Clear();

            // Filtrar la lista de productos
            var resultados = productos.Where(p =>
                p.Nombre.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                p.Codigo.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                p.Categoria.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0
            ).ToList();

            // Agregar los resultados al DataGridView
            foreach (var producto in resultados)
            {
                int rowIndex = dataGridView1.Rows.Add(producto.Codigo, producto.Nombre, producto.Descripcion, producto.Precio, producto.Stock, producto.Categoria);

                // Crear y agregar botón de Editar
                DataGridViewButtonCell editButton = new DataGridViewButtonCell();
                editButton.Value = "Editar";
                dataGridView1.Rows[rowIndex].Cells["Editar"] = editButton;

                // Crear y agregar botón de Eliminar
                DataGridViewButtonCell deleteButton = new DataGridViewButtonCell();
                deleteButton.Value = "Eliminar";
                dataGridView1.Rows[rowIndex].Cells["Eliminar"] = deleteButton;
            }

            // Actualizar total de productos
            ActualizarTotalProductos(resultados);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string searchTerm = txtBuscar.Text;
            BuscarProductos(searchTerm);
        }

        private void ActualizarTotalProductos(List<Producto> listaProductos)
        {
            // Contar la cantidad de productos en la lista
            int total = listaProductos.Count;

            // Actualizar el texto de la etiqueta lblTotal
            lblTotal.Text = "Total de Productos: " + total.ToString();
        }

        private void btnRecargar_Click(object sender, EventArgs e)
        {
            CargarProductos();
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            // Crear una instancia del formulario Nuevo
            Nuevo nuevoProducto = new Nuevo();

            // Ocultar el formulario actual
            this.Hide();

            // Mostrar el nuevo formulario
            nuevoProducto.Show();
        }

        private void EliminarProducto(string codigo)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Obtener el stock actual del producto
                var commandGetStock = new SQLiteCommand("SELECT Stock FROM Productos WHERE Codigo = @Codigo", connection);
                commandGetStock.Parameters.AddWithValue("@Codigo", codigo);
                int stockActual = Convert.ToInt32(commandGetStock.ExecuteScalar());

                if (stockActual > 0)
                {
                    // Descontar uno del stock
                    stockActual--;

                    if (stockActual <= 0)
                    {
                        // Si el stock llega a cero, eliminar el producto
                        var commandDelete = new SQLiteCommand("DELETE FROM Productos WHERE Codigo = @Codigo", connection);
                        commandDelete.Parameters.AddWithValue("@Codigo", codigo);
                        commandDelete.ExecuteNonQuery();
                        MessageBox.Show("Producto eliminado de la base de datos.", "Eliminación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Actualizar el stock
                        var commandUpdateStock = new SQLiteCommand("UPDATE Productos SET Stock = @Stock WHERE Codigo = @Codigo", connection);
                        commandUpdateStock.Parameters.AddWithValue("@Stock", stockActual);
                        commandUpdateStock.Parameters.AddWithValue("@Codigo", codigo);
                        commandUpdateStock.ExecuteNonQuery();

                        // Alertar si el stock es menor a 4
                        if (stockActual < 4)
                        {
                            MessageBox.Show("El producto " + codigo + " está bajo en stock. Considera reponerlo.", "Stock Bajo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    // Recargar los productos para actualizar la grilla
                    CargarProductos();
                }
            }
        }
    }
}