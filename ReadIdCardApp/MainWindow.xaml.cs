using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ReadIdCardApp.db;
using ReadIdCardApp.id_card;

namespace ReadIdCardApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    public static readonly ObservableCollection<Schedule> Schedules = new();
    public static readonly ObservableCollection<People> PeopleList = new();

    public MainWindow() {
        DbUtil.Init();

        InitializeComponent();
        DataGridSchedule.ItemsSource = Schedules;
        DataGridPeopleManagement.ItemsSource = PeopleList;
        ComboBoxScheduleList.ItemsSource = Schedules;
        LoadSchedules();
        LoadPeopleList();
    }

    private void ReadCard(object sender, RoutedEventArgs e) {
        TextBoxInfo.Text = "";
        var person = ReadIdCard.ReadWithException(true);
        ShowPersonInfo(person);
    }

    public void ShowPersonInfo(Person person) {
        if (!person.Success) {
            TextBoxName.Text = "";
            TextBoxSex.Text = "";
            TextBoxBirth.Text = "";
            TextBoxId.Text = "";
            TextBoxNation.Text = "";
            TextBoxRegDept.Text = "";
            TextBoxAddress.Text = "";
            TextBoxValidTime.Text = "";
            ShowInfo(person.Msg);
            return;
        }

        TextBoxName.Text = person.Name;
        TextBoxSex.Text = person.SexStr;
        TextBoxBirth.Text = person.GetBirthStr();
        TextBoxId.Text = person.Id;
        TextBoxNation.Text = person.NationStr;
        TextBoxRegDept.Text = person.RegDept;
        TextBoxAddress.Text = person.Address;
        TextBoxValidTime.Text = person.GetValidTimeStr();
        ShowInfo(string.IsNullOrEmpty(person.Msg) ? "读取成功" : person.Msg);
    }

    public void ShowInfo(string s) {
        TextBoxInfo.Text = "";
        new Thread(() => {
            Dispatcher.Invoke(() => {
                Thread.Sleep(150);
                TextBoxInfo.Text = $"[{DateTime.Now:HH:mm:ss.fff}] {s}";
            });
        }).Start();
    }

    private void OpenModifySchedule(Schedule schedule) {
        new ScheduleModifyWindow(schedule) { Owner = Application.Current.MainWindow }.ShowDialog();
    }

    private void AddSchedule(object sender, RoutedEventArgs e) {
        OpenModifySchedule(new Schedule());
    }

    private void ScheduleViewDoubleClick(object sender, MouseButtonEventArgs e) {
        var selectedItem = DataGridSchedule.SelectedItem;
        Schedule schedule = selectedItem as Schedule;
        if (schedule == null) {
            return;
        }

        OpenModifySchedule(schedule);
    }

    private void DeleteSchedule(object sender, RoutedEventArgs e) {
        List<Schedule> deleteList = new List<Schedule>();
        var selectedItems = DataGridSchedule.SelectedItems;
        var removeList = selectedItems.Cast<object?>().ToList();
        foreach (var selectedItem in removeList) {
            Schedule? schedule = selectedItem as Schedule;
            if (schedule == null) {
                return;
            }

            deleteList.Add(schedule);
        }

        if (deleteList.Count == 0) {
            return;
        }

        List<int?> deleteScheduleIdList = new List<int?>();
        foreach (var schedule in deleteList) {
            deleteScheduleIdList.Add(schedule.Id);
        }

        var deletePeopleCount = new People().CountAll(deleteScheduleIdList);
        var text = $"确认删除所选的{deleteScheduleIdList.Count}条场次信息吗？涉及{deletePeopleCount}条用户";
        MessageBoxResult result = MessageBox.Show(text, "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes) {
            foreach (var schedule in deleteList) {
                schedule.DeleteFromDb();
            }

            new People().DeleteScheduleIdFromDb(deleteScheduleIdList);
            LoadSchedules();
            LoadPeopleList();
        }
    }

    public void LoadSchedules() {
        var selectedSchedule = ComboBoxScheduleList.SelectedItem as Schedule;
        var schedules = new Schedule().SelectByIds(null);
        Schedules.Clear();
        foreach (var schedule in schedules) {
            Schedules.Add(schedule);
        }

        if (selectedSchedule != null) {
            foreach (var schedule in Schedules) {
                if (schedule.Id == selectedSchedule.Id) {
                    ComboBoxScheduleList.SelectedItem = schedule;
                }
            }
        }
    }

    private void ReloadSchedule(object sender, RoutedEventArgs e) {
        LoadSchedules();
    }

    public void LoadPeopleList() {
        var selectedItem = ComboBoxScheduleList.SelectedItem;
        Schedule schedule = selectedItem as Schedule;
        int? schduleId = null;
        if (schedule != null) {
            schduleId = schedule.Id;
        }

        FilterPeopleListByScheduleId(schduleId);
    }

    private void PeopleManagementViewDoubleClick(object sender, MouseButtonEventArgs e) {
        var selectedItem = DataGridPeopleManagement.SelectedItem;
        People people = selectedItem as People;
        if (people == null) {
            return;
        }

        OpenModifyPeopleDialog(people);
    }

    private void OpenModifyPeopleDialog(object sender, RoutedEventArgs e) {
        OpenModifyPeopleDialog(null);
    }

    private void OpenModifyPeopleDialog(People? people) {
        new PeopleModifyWindow(people, this) { Owner = Application.Current.MainWindow }.ShowDialog();
    }

    private void DeletePeople(object sender, RoutedEventArgs e) {
        var selectedItems = DataGridPeopleManagement.SelectedItems;
        var removeList = selectedItems.Cast<object?>().ToList();
        foreach (var selectedItem in removeList) {
            People? people = selectedItem as People;
            if (people == null) {
                return;
            }

            people.DeleteFromDb();
            PeopleList.Remove(people);
        }
    }

    private void RefreshPeopleList(object sender, RoutedEventArgs e) {
        LoadPeopleList();
    }

    private void ComboBoxScheduleListSelectionChanged(object sender,
        System.Windows.Controls.SelectionChangedEventArgs e) {
        var selectionBoxItem = ComboBoxScheduleList.SelectedItem;
        if (selectionBoxItem == null) {
            return;
        }

        var schedule = selectionBoxItem as Schedule;
        FilterPeopleListByScheduleId(schedule.Id);
    }

    private void FilterPeopleListByScheduleId(int? id) {
        var peopleList = new People().SelectByIds(null);
        PeopleList.Clear();

        List<People> res = peopleList;
        if (id != null) {
            res = peopleList.Where(p => p.ScheduleId == id).ToList();
        }

        foreach (var people in res) {
            PeopleList.Add(people);
        }
    }

    private void ShowAllPeople(object sender, RoutedEventArgs e) {
        ComboBoxScheduleList.SelectedItem = null;
        LoadPeopleList();
    }

    private void DeleteAllPeople(int? scheduleId) {
        var rows = new People().DeleteScheduleIdFromDb(scheduleId);
    }

    private void OpenBatchImport(object sender, RoutedEventArgs e) {
        Schedule? schedule = GetComboBoxSelectedSchedule();
        new DataImportWindow(schedule, this) { Owner = Application.Current.MainWindow }.ShowDialog();
    }

    private void DeleteAllPeople(object sender, RoutedEventArgs e) {
        int? scheduleId = null;
        Schedule? schedule = GetComboBoxSelectedSchedule();
        if (schedule != null) {
            scheduleId = schedule.Id;
        }

        var countAll = new People().CountAll(scheduleId);
        MessageBoxResult result = MessageBox.Show($"确认删除全部用户吗？涉及{countAll}条用户", "确认", MessageBoxButton.YesNo,
            MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes) {
            // 用户选择了“确认”
            DeleteAllPeople(scheduleId);
            LoadPeopleList();
        }
    }

    private Schedule? GetComboBoxSelectedSchedule() {
        return ComboBoxScheduleList.SelectedItem as Schedule;
    }
}