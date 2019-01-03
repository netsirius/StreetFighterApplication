using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using DataModels;
using LinqToDB;

namespace StreetFighterApplication
{
    public enum OperationType
    {
        INSERT,
        UPDATE,
        DELETE,
        RESET
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();

        }

        private void updateCitiesDataGrid()
        {
            using (var db = new LppDB())
            {
                var cities = from city in db.Cities
                             orderby city.Id ascending
                             select city;
                CitiesDataGrid.ItemsSource = cities;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.updateCitiesDataGrid();
        }

        private void Insert_btn_Click(object sender, RoutedEventArgs e)
        {
            this.AUD(OperationType.INSERT);
        }

        private void Update_btn_Click(object sender, RoutedEventArgs e)
        {
            this.AUD(OperationType.UPDATE);
            this.resetAll();
        }

        private void Delete_btn_Click(object sender, RoutedEventArgs e)
        {
            this.AUD(OperationType.DELETE);
            this.resetAll();
        }

        private void Reset_btn_Click(object sender, RoutedEventArgs e)
        {
            this.resetAll();
        }

        private void resetAll()
        {
            city_id_textbox.Text = "";
            city_name_textbox.Text = "";
            insert_btn.IsEnabled = true;
            update_btn.IsEnabled = false;
            delete_btn.IsEnabled = false;
        }

        private void AUD(OperationType operation)
        {
            String msg = "";
            var result = 0;
            var city = new City()
            {
                Id = int.Parse(city_id_textbox.Text),
                Name = city_name_textbox.Text,
            };
            try
            {
                switch (operation)
                {
                    case OperationType.INSERT:

                        using (var db = new LppDB())
                        {
                            result = db.Cities
                            .Value(c => c.Id, city.Id)
                            .Value(c => c.Name, city.Name)
                            .Insert();

                            if (result == 1)
                            {
                                msg = "Row inserted successfully!";
                            }
                        }
                        break;
                    case OperationType.UPDATE:
                        using (var db = new LppDB())
                        {
                            result = db.Cities
                            .Where(c => c.Id == city.Id)
                            .Set(c => c.Name, city.Name)
                            .Update();

                            if (result == 1)
                            {
                                msg = "Row updated successfully!";
                            }
                        }
                        break;
                    case OperationType.DELETE:
                        using (var db = new LppDB())
                        {
                            result = db.Cities
                            .Where(c => c.Id == city.Id)
                            .Delete();

                            if (result == 1)
                            {
                                msg = "Row deleted successfully!";
                            }
                        }
                        break;
                }

                if (result > 0)
                {
                    MessageBox.Show(msg);
                    this.updateCitiesDataGrid();
                }
            }
            catch (Exception e) { }
            
        }

        private void CitiesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CitiesDataGrid.SelectedCells.Any())
            {
                City city = CitiesDataGrid.SelectedItem as City;
                city_id_textbox.Text = city.Id.ToString();
                city_name_textbox.Text = city.Name.ToString();
                insert_btn.IsEnabled = false;
                update_btn.IsEnabled = true;
                delete_btn.IsEnabled = true;
            }
        }
    }
}