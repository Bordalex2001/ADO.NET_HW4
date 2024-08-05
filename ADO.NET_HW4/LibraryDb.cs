using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ADO.NET_HW4
{
    internal class LibraryDb
    {
        private readonly SqlConnection connection;
        private readonly DataSet dataSet;
        private readonly SqlDataAdapter bookDataAdapter;
        private readonly SqlDataAdapter authorDataAdapter;
        private readonly SqlDataAdapter categoryDataAdapter;

        public LibraryDb() 
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["LibraryDB"].ConnectionString);
            dataSet = new DataSet();

            bookDataAdapter = new SqlDataAdapter();
            authorDataAdapter = new SqlDataAdapter();
            categoryDataAdapter = new SqlDataAdapter();

            InitDataAdapters();
            FillDataSet();
            CreateDataRelations();
        }

        private void InitDataAdapters() 
        {
            bookDataAdapter.SelectCommand = new SqlCommand("Select * from Books", connection);
            bookDataAdapter.FillSchema(dataSet, SchemaType.Source, "Books");

            authorDataAdapter.SelectCommand = new SqlCommand("Select * from Authors", connection);
            authorDataAdapter.FillSchema(dataSet, SchemaType.Source, "Authors");

            categoryDataAdapter.SelectCommand = new SqlCommand("Select * from Categories", connection);
            categoryDataAdapter.FillSchema(dataSet, SchemaType.Source, "Categories");
        }

        private void FillDataSet() 
        {
            bookDataAdapter.Fill(dataSet, "Books");
            authorDataAdapter.Fill(dataSet, "Authors");
            categoryDataAdapter.Fill(dataSet, "Categories");
        }

        private void CreateDataRelations()
        {
            dataSet.Relations.Add("Book_Author", dataSet.Tables["Authors"].Columns["Id"], dataSet.Tables["Books"].Columns["Id_Author"]);
            dataSet.Relations.Add("Book_Category", dataSet.Tables["Categories"].Columns["Id"], dataSet.Tables["Books"].Columns["Id_Category"]);
        }

        public DataTable GetBooks()
        {
            return dataSet.Tables["Books"];
        }

        public DataTable GetAuthors()
        {
            return dataSet.Tables["Authors"];
        }

        public DataTable GetCategories()
        {
            return dataSet.Tables["Categories"];
        }

        public DataTable SearchBooks(string name, string authorFirstName, string authorLastName, string category)
        {
            DataTable booksTable = GetBooks();

            using (DataView dataView = new DataView(booksTable))
            {

                string filter = string.Empty;

                if (!string.IsNullOrEmpty(name))
                {
                    filter += $"Name like '{name}%'";
                }
                if (!string.IsNullOrEmpty(authorFirstName) || !string.IsNullOrEmpty(authorLastName))
                {
                    if (!string.IsNullOrEmpty(filter)) filter += " and ";

                    DataRow[] authorRows = dataSet.Tables["Authors"].Select($"FirstName like '{authorFirstName}%' and LastName like '{authorLastName}%'");
                    if (authorRows.Length > 0)
                    {
                        filter += "Id_Author in (";
                        filter += string.Join(", ", authorRows.Select(row => row["Id"].ToString()));
                        filter += ")";
                    }
                    else
                    {
                        filter += "1 = 0";
                    }
                }
                if (!string.IsNullOrEmpty(category))
                {
                    if (!string.IsNullOrEmpty(filter)) filter += " and ";

                    DataRow[] categoryRows = dataSet.Tables["Categories"].Select($"Name = '{category}'");
                    if (categoryRows.Length > 0)
                    {
                        filter += "Id_Category in (";
                        filter += string.Join(", ", categoryRows.Select(row => row["Id"].ToString()));
                        filter += ")";
                    }
                    else
                    {
                        filter += "1 = 0";
                    }
                }

                dataView.RowFilter = filter;
                return dataView.ToTable();
            }
        }
    }
}