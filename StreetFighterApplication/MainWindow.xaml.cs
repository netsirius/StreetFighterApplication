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
using System.Collections;
using System.Collections.ObjectModel;

namespace StreetFighterApplication
{
    public enum OperationType
    {
        INSERT,
        UPDATE,
        DELETE,
        RESET
    }

    public enum ResultType {
        Player1,
        Player2,
        Draw
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
            city_id_textbox.Text = (this.getAllCities().Last().Id + 1).ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.updateCitiesDataGrid();
            this.updateFightersDataGrid();
            this.updateGamesDataGrid();
        }

        private void Insert_btn_Click(object sender, RoutedEventArgs e)
        {
            this.cityAUD(OperationType.INSERT);
        }

        private void Update_btn_Click(object sender, RoutedEventArgs e)
        {
            this.cityAUD(OperationType.UPDATE);
            this.resetAll();
        }

        private void Delete_btn_Click(object sender, RoutedEventArgs e)
        {
            this.cityAUD(OperationType.DELETE);
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
            this.updateCitiesDataGrid();
        }

        private void cityAUD(OperationType operation)
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
            if (CitiesDataGrid.SelectedItem != null)
            {
                City city = CitiesDataGrid.SelectedItem as City;
                city_id_textbox.Text = city.Id.ToString();
                city_name_textbox.Text = city.Name.ToString();
                insert_btn.IsEnabled = false;
                update_btn.IsEnabled = true;
                delete_btn.IsEnabled = true;
            }
        }
        
        private void updateFightersDataGrid()
        {
            Fighter_datagrid.ItemsSource = this.getAllFighters();
            fighter_id_textbox.Text = (this.getLastPlayerId() + 1).ToString();
        }

        private int getLastPlayerId()
        {
            return this.getAllFighters().ToList().Last().Id;
        }

        private ObservableCollection<Fighter> getAllFighters()
        {
            using (var db = new LppDB())
            {
                var fighters = (from game in db.Games
                                from player in db.Players
                                where game.PlayerOneId == player.Id || game.PlayerTwoId == player.Id
                                group game by player into pg
                                orderby pg.Key.Id ascending
                                let score = pg.Sum(g => (g.PlayerOneId == pg.Key.Id && g.GameResult.ToString() == "Player1") ||
                                                        (g.PlayerTwoId == pg.Key.Id && g.GameResult.ToString() == "Player2")
                                                        ? 1.0f : g.GameResult.ToString() == "Draw"
                                                        ? 0.5f : 0.0f)
                                select new Fighter()
                                {
                                    Id = pg.Key.Id,
                                    Name = pg.Key.Name,
                                    Score = score,
                                }).ToList();

                var allFighters = from fighter in db.Players
                                  select fighter;
                // Set player score for recet added player taht don't have played any game.
                foreach (var fighter in allFighters)
                {
                    if (fighters.All(f => f.Id != fighter.Id))
                    {
                        fighters.Add(new Fighter() { Id = fighter.Id, Name = fighter.Name, Score = 0.0f });
                    }
                }
                return new ObservableCollection<Fighter>(fighters);
            }
        }

        private void Fighter_filter_btn_Click(object sender, RoutedEventArgs e)
        {
            var fighterName = fighter_filter_textbox.Text;
            Fighter_datagrid.ItemsSource =this.getAllFighters().Where(p => p.Name.ToLower().Contains(fighterName.ToLower()));
        }

        private void Fighter_reset_btn_Click(object sender, RoutedEventArgs e)
        {
            fighter_filter_textbox.Text = "";
            this.updateFightersDataGrid();
        }

        private void Fighter_add_btn_Click(object sender, RoutedEventArgs e)
        {
            var player = new Player()
            {
                Id = int.Parse(fighter_id_textbox.Text),
                Name = fighter_name_textbox.Text,
            };
            using (var db = new LppDB())
            {
                 var result = db.Players
                    .Value(p => p.Id, player.Id)
                    .Value(c => c.Name, player.Name)
                    .Insert();

                if (result == 1)
                {
                    MessageBox.Show("Player row inserted successfully!");
                }
            }
            this.updateFightersDataGrid();
        }
        private void updateGamesDataGrid()
        {
            using (var db = new LppDB())
            {
                var games = from game in db.Games
                             orderby game.Id ascending
                             select game;
                Game_datagrid.ItemsSource = games;
            }
            game_id_textbox.Text = (this.getLastGameId() + 1).ToString();
        }

        private void City_id_combobox_DropDownOpened(object sender, EventArgs e)
        {
            city_id_combobox.ItemsSource = this.getAllCities().Select(city => city.Id);
        }

        private ObservableCollection<City> getAllCities()
        {
            using (var db = new LppDB())
            {
                var cities = (from city in db.Cities
                              orderby city.Id ascending
                              select city).ToList();
                return new ObservableCollection<City>(cities);
            }
        }

        private void Fighter_one_id_combobox_DropDownOpened(object sender, EventArgs e)
        {
            fighter_one_id_combobox.ItemsSource = this.getAllFighters().Select(fighter => fighter.Id);
        }

        private void Fighter_two_id_combobox_DropDownOpened(object sender, EventArgs e)
        {
            fighter_two_id_combobox.ItemsSource = this.getAllFighters().Select(fighter => fighter.Id);
        }

        private void Game_result_combobox_DropDownOpened(object sender, EventArgs e)
        {
            game_result_combobox.ItemsSource = Enum.GetValues(typeof(ResultType)).Cast<ResultType>();
        }

        private void Game_add_btn_Click(object sender, RoutedEventArgs e)
        {
            var game = new Game()
            {
                Id = int.Parse(game_id_textbox.Text),
                CityId = int.Parse(city_id_combobox.Text),
                PlayerOneId = int.Parse(fighter_one_id_combobox.Text),
                PlayerTwoId = int.Parse(fighter_two_id_combobox.Text),
                GameResult = game_result_combobox.Text,
            };

            if (game.PlayerOneId == game.PlayerTwoId)
            {
                MessageBox.Show("Player id's must be different!");
            }
            else
            {
                using (var db = new LppDB())
                {
                    var result = db.Games
                       .Value(g => g.Id, game.Id)
                       .Value(gc => gc.CityId, game.CityId)
                       .Value(gp1 => gp1.PlayerOneId, game.PlayerOneId)
                       .Value(gp2 => gp2.PlayerTwoId, game.PlayerTwoId)
                       .Value(gr => gr.GameResult, game.GameResult)
                       .Insert();

                    if (result == 1)
                    {
                        MessageBox.Show("Game row inserted successfully!");
                    }
                }
                this.updateGamesDataGrid();
                this.updateFightersDataGrid();
            }
        }

        private int getLastGameId()
        {
            using (var db = new LppDB())
            {
                var games = (from game in db.Games
                              orderby game.Id ascending
                              select game.Id).ToList();
                return games.Last();
            }
        }

        private void Game_update_btn_Click(object sender, RoutedEventArgs e)
        {
            var game = new Game()
            {
                Id = int.Parse(game_id_textbox.Text),
                CityId = int.Parse(city_id_combobox.Text),
                PlayerOneId = int.Parse(fighter_one_id_combobox.Text),
                PlayerTwoId = int.Parse(fighter_two_id_combobox.Text),
                GameResult = game_result_combobox.Text,
            };

            if (game.PlayerOneId == game.PlayerTwoId)
            {
                MessageBox.Show("Player id's must be different!");
            }
            else
            {
                using (var db = new LppDB())
                {
                    var result = db.Games
                        .Where(g => g.Id == game.Id)
                        .Set(g => g.CityId, game.CityId)
                        .Set(g => g.PlayerOneId, game.PlayerOneId)
                        .Set(g => g.PlayerTwoId, game.PlayerTwoId)
                        .Set(g => g.GameResult, game.GameResult)
                        .Update();

                    if (result == 1)
                    {
                        MessageBox.Show("Game row updated inserted successfully!");
                    }
                }

                this.updateGamesDataGrid();
                this.updateFightersDataGrid();
            }
        }

        private void Game_delete_btn_Click(object sender, RoutedEventArgs e)
        {
            var game = new Game()
            {
                Id = int.Parse(game_id_textbox.Text),
                CityId = int.Parse(city_id_combobox.Text),
                PlayerOneId = int.Parse(fighter_one_id_combobox.Text),
                PlayerTwoId = int.Parse(fighter_two_id_combobox.Text),
                GameResult = game_result_combobox.Text,
            };

            using (var db = new LppDB())
            {
                var result = db.Games
                .Where(g => g.Id == game.Id)
                .Delete();

                if (result == 1)
                {
                    MessageBox.Show("Game row deleted inserted successfully!");
                }
            }

            this.updateGamesDataGrid();
            this.updateFightersDataGrid();
        }

        private void Game_reset_btn_Click(object sender, RoutedEventArgs e)
        {
            game_add_btn.IsEnabled = true;
            game_update_btn.IsEnabled = false;
            game_delete_btn.IsEnabled = false;
        }

        private void Game_datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Game_datagrid.SelectedItem != null)
            {
                Game game = Game_datagrid.SelectedItem as Game;
                game_id_textbox.Text = game.Id.ToString();
                city_id_combobox.Text = game.CityId.ToString();
                fighter_one_id_combobox.Text = game.PlayerOneId.ToString();
                fighter_two_id_combobox.Text = game.PlayerTwoId.ToString();
                game_result_combobox.Text = game.GameResult.ToString();

                game_add_btn.IsEnabled = false;
                game_update_btn.IsEnabled = true;
                game_delete_btn.IsEnabled = true;
            }
        }
    }
}