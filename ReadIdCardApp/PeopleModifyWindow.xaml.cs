using System.Windows;
using System.Windows.Input;
using ReadIdCardApp.db;

namespace ReadIdCardApp;

public partial class PeopleModifyWindow : Window {
    public People People;
    public MainWindow MainWindow;

    public PeopleModifyWindow() {
        InitializeComponent();
        Init();
    }

    public PeopleModifyWindow(People? people, MainWindow mainWindow) {
        people ??= new People();
        MainWindow = mainWindow;

        People = people;
        InitializeComponent();
        Init();
    }

    public void Init() {
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        ComboBoxPeopleModifySchedule.ItemsSource = MainWindow.Schedules;
        DataContext = People;
        Title = People.Id == null ? "新增人员信息" : "修改人员信息";

        if (People.ScheduleId != null) {
            foreach (var schedule in MainWindow.Schedules) {
                if (schedule.Id == People.ScheduleId) {
                    ComboBoxPeopleModifySchedule.SelectedItem = schedule;
                    break;
                }
            }
        }
        TextBoxName.Focus();
    }

    private void Modify() {
        if (People.IdCard == null || string.IsNullOrEmpty(People.IdCard) || string.IsNullOrWhiteSpace(People.IdCard)) {
            MessageBox.Show("身份证号不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            TextBoxIdCard.Focus();
            return;
        }

        People.IdCard = People.IdCard.Trim();
        if (ComboBoxPeopleModifySchedule.SelectedItem is Schedule schedule) {
            People.ScheduleId = schedule.Id;
        }

        if (People.ScheduleId == null) {
            MessageBox.Show("场次不能为空，需要选择！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (People.Id == null) {
            var id = People.InsertToDb();
            People.Id = id;
            MainWindow.PeopleList.Add(People);
        } else {
            for (var i = 0; i < MainWindow.PeopleList.Count; i++) {
                if (MainWindow.PeopleList[i].Id == People.Id) {
                    People.UpdateDb();
                    MainWindow.PeopleList[i] = People;
                }
            }
        }
        
        MainWindow.LoadPeopleList();
        Close();
    }

    private void Confirm(object sender, RoutedEventArgs e) {
        Modify();
    }

    private void KeyDownAction(object sender, KeyEventArgs e) {
        if (e.Key == Key.Escape) {
            Close();
        } else if (e.Key == Key.Enter) {
            TextBoxName.Focus();
            Modify();
        }
    }
}