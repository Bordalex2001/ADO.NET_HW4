using System;
using System.Collections.Generic;
using System.Data;
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

namespace ADO.NET_HW4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly LibraryDb db;

        public MainWindow()
        {
            InitializeComponent();
            db = new LibraryDb();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable books = db.GetBooks();
            bookDataGrid.ItemsSource = books.DefaultView;

            DataTable categories = db.GetCategories();
            categoryComboBox.ItemsSource = categories.DefaultView;
            categoryComboBox.DisplayMemberPath = "Name";
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = nameTxtBox.Text;
            string author = authorTxtBox.Text;
            string category = (categoryComboBox.SelectedItem as DataRowView)?["Name"].ToString();

            string authorFirstName;
            string authorLastName;
            string[] authorNameParts = author.Split(new char[] { ' ' }, 2);
            if (authorNameParts.Length == 1)
            {
                authorFirstName = string.Empty;
                authorLastName = authorNameParts[0];
            }
            else
            {
                authorFirstName = authorNameParts[0];
                authorLastName = authorNameParts[1];
            }

            DataTable searchResult = db.SearchBooks(name, authorFirstName, authorLastName, category);
            bookDataGrid.ItemsSource = searchResult.DefaultView;
        }
    }
}
