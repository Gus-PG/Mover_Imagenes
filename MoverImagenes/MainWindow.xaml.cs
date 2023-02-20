using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MoverImagenes
{
    /// <summary>
    /// Sencilla app para detectar cuantos archivos ('png' en este caso) hay en 
    /// und directorio y moverlos a una carpeta elegida por nosotros.
    /// Hace uso de:
    ///     - Dialogs 'hackeados' para seleccionar Directorios (origen/destino).
    ///     - 'File', 'Directory' para seleccionar archivos ('png' en este caso)
    /// y moverlos de directorio (carpeta destino).
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] archivos;
        public MainWindow()
        {
            InitializeComponent();
        }
        

        /// <summary>
        /// Método que crea un 'hack' sobre un 'SaveFileDialog'.
        /// En vez de seleccionar un archivo, le damos un falso nombre y luego lo elimina 
        /// de la ruta del directorio en que está contenido, así nos queda la dirección de la carpeta (sin archivo).
        /// Evita: 
        ///     - Usar Windows.Forms en WPF.
        ///     - Usar otras librerías.
        /// Origen de  esta solución:     
        /// "https://stackoverflow.com/questions/4007882/select-folder-dialog-wpf/17712949#17712949"
        /// MEJORAS:
        ///     - Crear combo para elegir tipo de archivos que podemos mover.
        /// </summary>
        /// <returns>Cadena con la ruta del directorio seleccionado.</returns>
        private string SeleccionarDirectorio()
        {
            string path="";            
            var dialog = new SaveFileDialog();
            //Configuramos el diálogo.
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); //Fijamos directorio inicial.
            dialog.Title = "Select a Directory"; // En vez del "Save As"
            dialog.Filter = "Directory|*.this.directory"; // (Así no mostramos archivos ni extensiones).
            dialog.FileName = "select"; // El 'Filename' será: "select.this.directory"
            //En caso de seleccionar directorio.
            if (dialog.ShowDialog() == true)
            {
                path = dialog.FileName;
                // Quita nuestro nombre de archivo falso de la ruta del directorio.
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                // Si ha añadido un nombre nuevo, crea el directorio con ese nombre.
                if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);                
            }
            // Devolvemos la cadena con nuestra ruta al directorio.
            return path;
        }


        /// <summary>
        /// Cuenta cuantos archivos hay en la carpeta y llenar el array 'archivos' con sus rutas.
        /// </summary>
        /// <param name="path">Cadena con el directorio en el que buscar los archivos.</param>
        private void ContarImagenes(string path)
        {            
            archivos = Directory.GetFiles(path, "*.png");
            int cnt = 0;
            foreach(string s in archivos)
            {
                cnt++;
            }
            lblFound.Content = cnt.ToString() + " archivos 'PNG' encontrados.";
        }


        /// <summary>
        /// Establece directorio Origen
        /// </summary>
        private void BtnOrigen_Click(object sender, RoutedEventArgs e)
        {
            txbOrigen.Text = SeleccionarDirectorio();            
            ContarImagenes(txbOrigen.Text);
        }


        /// <summary>
        /// Establece directorio destino.
        /// </summary>
        private void BtnDestino_Click(object sender, RoutedEventArgs e)
        {
            txbDestino.Text = SeleccionarDirectorio(); 
        }


        /// <summary>
        /// Mueve (no copia) los archivos a la carpeta destino.
        /// </summary>
        private void BtnMover_Click(object sender, RoutedEventArgs e)
        {
            int cnt = 0;
            if (!String.IsNullOrEmpty(txbDestino.Text) && !String.IsNullOrEmpty(txbOrigen.Text))
            {
                if (Directory.Exists(txbDestino.Text))
                {
                    archivos = Directory.GetFiles(txbOrigen.Text, "*.png");
                    foreach (string s in archivos)
                    {
                        cnt++;
                        string nombreArchivo = System.IO.Path.GetFileName(s);
                        string rutaDestino = System.IO.Path.Combine(txbDestino.Text, nombreArchivo);
                        File.Move(s, rutaDestino, true);
                    }
                    lblMoved.Content = cnt.ToString() + " archivos movidos.";
                    txbOrigen.Text = "";
                    txbDestino.Text = "";
                }
                else MessageBox.Show("Carpeta Destino no existe.");
            }
            else MessageBox.Show("No están indicadas todas las carpetas.");
        }

    }
}
