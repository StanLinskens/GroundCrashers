using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using groundCrashers_game.classes;
using Newtonsoft.Json;
using System.IO;

namespace groundCrashers_game
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private Manager gameManager;
        private Button fightButton;
        private Button bagButton;
        private Button groundCrashersButton;
        private Button runButton;
        private WrapPanel actionButtonsPanel;

        public GameWindow()
        {
            InitializeComponent();

            // Initialize the game manager
            gameManager = new Manager();
            //gameManager.LoadAllCreatures();

            // Find the WrapPanel in the XAML layout
            FindActionButtonsPanel();
        }

        private void FindActionButtonsPanel()
        {
            // Find the WrapPanel that contains our action buttons
            // This assumes the structure from your XAML where buttons are in a WrapPanel
            // inside a Grid in column 1 of the bottom row
            Grid mainGrid = (Grid)((Viewbox)Content).Child;
            Grid optionsGrid = (Grid)mainGrid.Children[mainGrid.Children.Count - 1]; // Bottom row grid

            // Get the wrap panel from column 1 of the options grid
            foreach (UIElement child in optionsGrid.Children)
            {
                if (child is WrapPanel panel && Grid.GetColumn(panel) == 1)
                {
                    actionButtonsPanel = panel;
                    // Store references to the original buttons
                    StoreButtonReferences();
                    break;
                }
            }

            if (actionButtonsPanel == null)
            {
                MessageBox.Show("Could not find action buttons panel. UI structure may have changed.");
            }
        }

        private void StoreButtonReferences()
        {
            // Get references to the original buttons for later use
            foreach (UIElement element in actionButtonsPanel.Children)
            {
                if (element is Button button)
                {
                    string content = button.Content.ToString();
                    switch (content)
                    {
                        case "FIGHT":
                            fightButton = button;
                            break;
                        case "BAG":
                            bagButton = button;
                            break;
                        case "GroundCrashers":
                            groundCrashersButton = button;
                            break;
                        case "RUN":
                            runButton = button;
                            break;
                    }
                }
            }
        }

        private void Fight_Button_Click(object sender, RoutedEventArgs e)
        {
            // Replace the current buttons with combat options
            ShowCombatOptions();
        }

        private void ShowCombatOptions()
        {
            // Save original buttons if not already saved
            if (fightButton == null)
            {
                StoreButtonReferences();
            }

            // Clear the current buttons from the panel
            actionButtonsPanel.Children.Clear();

            // Create new combat option buttons
            Button attackButton = CreateCombatButton("ATTACK", "LightCoral", "Red", Attack_Button_Click);
            Button elementButton = CreateCombatButton("ELEMENT", "LightGreen", "DarkGreen", Element_Button_Click);
            Button defendButton = CreateCombatButton("DEFEND", "LightBlue", "DarkBlue", Defend_Button_Click);
            Button backButton = CreateCombatButton("BACK", "Gray", "DarkGray", Back_Button_Click);

            // Add the new buttons to the panel
            actionButtonsPanel.Children.Add(attackButton);
            actionButtonsPanel.Children.Add(elementButton);
            actionButtonsPanel.Children.Add(defendButton);
            actionButtonsPanel.Children.Add(backButton);
        }

        private Button CreateCombatButton(string content, string background, string border, RoutedEventHandler clickHandler)
        {
            Button button = new Button
            {
                Content = content,
                Height = 60,
                Width = 500,
                Margin = new Thickness(10),
                Background = (Brush)new BrushConverter().ConvertFromString(background),
                BorderThickness = new Thickness(4),
                BorderBrush = (Brush)new BrushConverter().ConvertFromString(border),
                FontSize = 20
            };

            // Add style for rounded corners
            Style style = new Style(typeof(Border));
            style.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(30)));
            button.Resources.Add(typeof(Border), style);

            // Add click handler
            button.Click += clickHandler;

            return button;
        }

        private void Attack_Button_Click(object sender, RoutedEventArgs e)
        {
            // Implement elemental attack logic
            MessageBox.Show("Attack selected!");
            // After attack, restore main action buttons
            RestoreMainActionButtons();
        }

        private void Element_Button_Click(object sender, RoutedEventArgs e)
        {
            // Implement elemental attack logic
            MessageBox.Show("Element attack selected!");
            // After attack, restore main action buttons
            RestoreMainActionButtons();
        }

        private void Defend_Button_Click(object sender, RoutedEventArgs e)
        {
            // Implement defense logic
            MessageBox.Show("Defense selected!");
            // After defense, restore main action buttons
            RestoreMainActionButtons();
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            // Go back to main action buttons
            RestoreMainActionButtons();
        }

        private void RestoreMainActionButtons()
        {
            // Clear the current combat buttons
            actionButtonsPanel.Children.Clear();

            // Restore the original action buttons
            actionButtonsPanel.Children.Add(fightButton);
            actionButtonsPanel.Children.Add(bagButton);
            actionButtonsPanel.Children.Add(groundCrashersButton);
            actionButtonsPanel.Children.Add(runButton);
        }

        private void Bag_Button_Click(object sender, RoutedEventArgs e)
        {
            // Implement bag functionality
            MessageBox.Show("Bag selected!");
        }

        private void GroundCrashers_Button_Click_2(object sender, RoutedEventArgs e)
        {
            GroundCrasherWindow GroundCrasherWindow = new GroundCrasherWindow();
            GroundCrasherWindow.Show();
        }

        private void Run_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();
            this.Close();
        }
    }
}